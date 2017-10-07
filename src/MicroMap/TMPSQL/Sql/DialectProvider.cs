using System;

namespace MicroMap.TMP.Sql
{
    public class DialectProvider : BaseDialectProvider
    {
        const string Iso8601Format = "yyyy-MM-dd";

        static DialectProvider instance;
        public static DialectProvider Instance
        {
            get
            {
                if (instance == null)
                    instance = new DialectProvider();
                return instance;
            }
        }

        public override string GetQuotedValue(object value, Type fieldType)
        {
            if (value == null)
            {
                return "NULL";
            }

            if (fieldType == typeof(Guid))
            {
                return $"CAST('{(Guid)value}' AS {StringGuidDefinition})";
            }

            if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
            {
                return base.GetQuotedValue(((DateTime)value).ToString(Iso8601Format), typeof(string));
            }

            if ((value is TimeSpan) && (fieldType == typeof(Int64) || fieldType == typeof(Int64?)))
            {
                return base.GetQuotedValue(((TimeSpan)value).Ticks, fieldType);
            }

            if (fieldType == typeof(bool?) || fieldType == typeof(bool))
            {
                return base.GetQuotedValue((bool)value ? "1" : "0", typeof(string));
            }

            if (fieldType == typeof(decimal?) || fieldType == typeof(decimal) || fieldType == typeof(double?) || fieldType == typeof(double) || fieldType == typeof(float?) || fieldType == typeof(float))
            {
                var s = base.GetQuotedValue(value, fieldType);
                if (s.Length > 20)
                {
                    s = s.Substring(0, 20);
                }

                // when quoted exception is more clear!
                return "'" + s + "'"; 
            }

            return base.GetQuotedValue(value, fieldType);
        }
    }
}
