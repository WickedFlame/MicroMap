using System;
using System.Globalization;
using MicroMap.TypeDefinition;

namespace MicroMap.Sql
{
    public class DialectProvider
    {
        private const string Iso8601Format = "yyyy-MM-dd";
        private const string StringGuidDefinition = "VARCHAR2(37)";

        private string _stringLengthNonUnicodeColumnDefinitionFormat = "VARCHAR({0})";
        private string _stringLengthUnicodeColumnDefinitionFormat = "NVARCHAR({0})";

        private string _stringColumnDefinition;
        private string _stringLengthColumnDefinitionFormat;
        
        private readonly string _intColumnDefinition = "INTEGER";
        private readonly string _longColumnDefinition = "BIGINT";
        private readonly string _boolColumnDefinition = "BOOL";
        private readonly string _realColumnDefinition = "DOUBLE";
        private readonly string _decimalColumnDefinition = "DECIMAL";

        private static DialectProvider _instance;

        public static DialectProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DialectProvider();
                }

                return _instance;
            }
        }

        public DialectProvider()
        {
            UpdateStringColumnDefinitions();
        }


        private bool _useUnicode;

        public virtual bool UseUnicode
        {
            get
            {
                return _useUnicode;
            }
            set
            {
                _useUnicode = value;
                UpdateStringColumnDefinitions();
            }
        }

        private int _defaultStringLength = 8000; //SqlServer express limit

        public int DefaultStringLength
        {
            get
            {
                return _defaultStringLength;
            }
            set
            {
                _defaultStringLength = value;
                UpdateStringColumnDefinitions();
            }
        }

        private string _maxStringColumnDefinition;

        public string MaxStringColumnDefinition
        {
            get
            {
                return _maxStringColumnDefinition ?? _stringColumnDefinition;
            }
            set
            {
                _maxStringColumnDefinition = value;
            }
        }

        public string GetQuotedValue(object value, Type fieldType)
        {
            if (value == null)
            {
                return "NULL";
            }

            if (fieldType == typeof(Guid))
            {
                return $"CAST('{(Guid) value}' AS {StringGuidDefinition})";
            }

            if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
            {
                return ShouldQuoteValue(fieldType) ? GetQuotedValue(((DateTime) value).ToString(Iso8601Format)) : ((DateTime) value).ToString(Iso8601Format);
            }

            if (value is TimeSpan && (fieldType == typeof(Int64) || fieldType == typeof(Int64?)))
            {
                return ((TimeSpan) value).Ticks.ToString(CultureInfo.InvariantCulture);
            }

            if (fieldType == typeof(bool?) || fieldType == typeof(bool))
            {
                return ShouldQuoteValue(fieldType) ? GetQuotedValue((bool) value ? "1" : "0") : (bool) value ? "1" : "0";
            }

            if (fieldType == typeof(decimal?) || fieldType == typeof(decimal) || fieldType == typeof(double?) || fieldType == typeof(double) || fieldType == typeof(float?) || fieldType == typeof(float))
            {
                var s = GetQuotedValueFromTypeCode(value, fieldType);
                if (s.Length > 20)
                {
                    s = s.Substring(0, 20);
                }

                // when quoted exception is more clear!
                return "'" + s + "'";
            }

            return GetQuotedValueFromTypeCode(value, fieldType);
        }

        public string GetQuotedValueFromTypeCode(object value, Type fieldType)
        {
            var typeCode = fieldType.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Single:
                {
                    return ((float) value).ToString(CultureInfo.InvariantCulture);
                }

                case TypeCode.Double:
                {
                    return ((double) value).ToString(CultureInfo.InvariantCulture);
                }

                case TypeCode.Decimal:
                {
                    return ((decimal) value).ToString(CultureInfo.InvariantCulture);
                }

                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    if (fieldType.IsNumericType())
                    {
                        return Convert.ChangeType(value, fieldType).ToString();
                    }

                    break;
            }

            return ShouldQuoteValue(fieldType) ? DialectProvider.Instance.GetQuotedValue(value.ToString()) : value.ToString();
        }
        
        public void UpdateStringColumnDefinitions()
        {
            _stringLengthColumnDefinitionFormat = UseUnicode ? _stringLengthUnicodeColumnDefinitionFormat : _stringLengthNonUnicodeColumnDefinitionFormat;

            _stringColumnDefinition = string.Format(_stringLengthColumnDefinitionFormat, DefaultStringLength);
        }

        public bool ShouldQuoteValue(Type fieldType)
        {
            var fieldDefinition = GetUndefinedColumnDefinition(fieldType, null);

            return fieldDefinition != _intColumnDefinition
                   && fieldDefinition != _longColumnDefinition
                   && fieldDefinition != _realColumnDefinition
                   && fieldDefinition != _decimalColumnDefinition
                   && fieldDefinition != _boolColumnDefinition;
        }

        protected string GetUndefinedColumnDefinition(Type fieldType, int? fieldLength)
        {
            return fieldLength.HasValue
                ? string.Format(_stringLengthColumnDefinitionFormat, fieldLength.GetValueOrDefault(DefaultStringLength))
                : MaxStringColumnDefinition;
        }

        public virtual string GetQuotedValue(string paramValue)
        {
            return $"'" + paramValue.Replace("'", "''") + "'";
        }

        public string GetQuotedColumnName(string columnName)
        {
            return $"\"{columnName}\"";
        }

        public string GetQuotedColumnName(string tableName, string fieldName)
        {
            return tableName + "." + fieldName;
        }

        public string EscapeWildcards(string value)
        {
            if (value == null)
            {
                return null;
            }

            return value.Replace("^", @"^^")
                .Replace(@"\", @"^\")
                .Replace("_", @"^_")
                .Replace("%", @"^%");
        }
    }
}
