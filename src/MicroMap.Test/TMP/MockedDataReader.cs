﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace MicroMap.UnitTest.Datareader
{
    /// <summary>
    /// Creates an IDataReader over an instance of IEnumerable&lt;> or IEnumerable.
    /// Anonymous type arguments are acceptable.
    /// </summary>
    public class MockedDataReader : ObjectDataReader
    {
        private readonly Queue<Result> _results;
        private IEnumerator _enumerator;
        private Type _type;
        private object _current;
        
        /// <summary>
        /// Create an IDataReader over an instance of IEnumerable.
        /// Use other constructor for IEnumerable&lt;>
        /// </summary>
        /// <param name="collection">The collection</param>
        public MockedDataReader(IEnumerable collection, Type type)
            : base(type)
        {
            _type = type;
            _enumerator = collection?.GetEnumerator();

            _results = new Queue<Result>();
        }
        
        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Object"/> which will contain the field value upon return.
        /// </returns>
        /// <param name="i">The index of the field to find. 
        /// </param><exception cref="T:System.IndexOutOfRangeException">The index passed was outside the range of 0 through <see cref="P:System.Data.IDataRecord.FieldCount"/>. 
        /// </exception><filterpriority>2</filterpriority>
        public override object GetValue(int i)
        {
            if (i < 0 || i >= Fields.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return Fields[i].Getter(_current);
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader"/> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows
        /// </returns>
        public override bool Read()
        {
            if (_enumerator == null)
            {
                throw new InvalidOperationException("MockedDataReader is executed without a collection to return");
            }

            bool returnValue = _enumerator.MoveNext();
            _current = returnValue ? _enumerator.Current : _type.IsValueType ? Activator.CreateInstance(_type) : null;
            return returnValue;
        }

        public override bool NextResult()
        {
            if (_results.Count <= 0)
            {
                return false;
            }

            var next = _results.Dequeue();
            _enumerator = next.Enumerator;
            _type = next.Type;

            SetFields(_type);

            return true;
        }

        public MockedDataReader AddResult<T2>(Func<IEnumerable<T2>> collection)
        {
            _results.Enqueue(new Result(collection, typeof(T2)));

            return this;
        }

        internal MockedDataReader AddResult(Result result)
        {
            _results.Enqueue(result);

            return this;
        }
    }
}
