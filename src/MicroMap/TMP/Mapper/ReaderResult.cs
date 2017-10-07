using System.Collections;
using System.Collections.Generic;

namespace MicroMap.Mapper
{
    /// <summary>
    /// The resultset as a collection of DataRows
    /// </summary>
    public class ReaderResult : IEnumerable<DataRow>
    {
        private readonly List<DataRow> _rows;

        public ReaderResult()
        {
            _rows = new List<DataRow>();
        }

        /// <summary>
        /// Adds a new row to the resultset
        /// </summary>
        /// <param name="row">The row conaining the data</param>
        public void Add(DataRow row)
        {
            _rows.Add(row);
        }

        /// <summary>
        /// Gets the enumerator for DataRows
        /// </summary>
        /// <returns>The rows</returns>
        public IEnumerator<DataRow> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Represents a Datarow containing key-value data collected from the resultset
    /// </summary>
    public class DataRow
    {
        private readonly Dictionary<string, object> _columns;

        public DataRow()
        {
            _columns = new Dictionary<string, object>();
        }

        /// <summary>
        /// Adds a new column to the resultset
        /// </summary>
        /// <param name="field">The name of the field</param>
        /// <param name="value">The value contained in the field</param>
        /// <returns></returns>
        public DataRow Add(string field, object value)
        {
            _columns.Add(field.ToLower(), value);

            return this;
        }

        /// <summary>
        /// Gets a value indicating if the field is contained in the resultset
        /// </summary>
        /// <param name="field">The name of the field</param>
        /// <returns>The value indicating if the field exists</returns>
        public bool ContainsField(string field)
        {
            return _columns.ContainsKey(field.ToLower());
        }

        /// <summary>
        /// Gets the value for the field
        /// </summary>
        /// <param name="id">The name of the field</param>
        /// <returns>The value of the field</returns>
        public object this[string id]
        {
            get
            {
                return _columns[id.ToLower()];
            }
        }
    }
}
