namespace FC.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class KendoGridDataCollector<T> where T : class
    {
        #region Fields

        private IList<T> _originSource;
        private IList<T> _deleteSource;
        private IList<T> _updateSource;
        private IList<T> _insertSource;
        private IList<string> _propertyNames;
        private IList<T> _beforeSource;

        #endregion

        #region Methods

        public KendoGridDataCollector()
        {
            _originSource = new List<T>();
            _insertSource = new List<T>();
            _updateSource = new List<T>();
            _deleteSource = new List<T>();
        }

        /// <summary>
        /// Set Array Of the Data Grid's Keys 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public KendoGridDataCollector<T> SetKeys(Action<DataGridKeyFactory<T>> expression)
        {
            var keyFactory = new DataGridKeyFactory<T>();

            expression(keyFactory);

            _propertyNames = keyFactory.Keys;

            return this;
        }

        /// <summary>
        /// Set Origine Source From the DB
        /// </summary>
        /// <param name="originSource"></param>
        /// <returns></returns>
        public KendoGridDataCollector<T> SetOriginSource(IList<T> originSource)
        {
            _originSource = originSource;

            return this;
        }

        /// <summary>
        /// Set Deleted Source From the DB
        /// </summary>
        /// <param name="deleteSource"></param>
        /// <returns></returns>
        public KendoGridDataCollector<T> SetDeleteSource(IList<T> deleteSource)
        {
            _deleteSource = deleteSource;

            return this;
        }

        /// <summary>
        /// Set Updated Source From Datagrid
        /// </summary>
        /// <param name="updateSource"></param>
        /// <returns></returns>
        public KendoGridDataCollector<T> SetUpdateSource(IList<T> updateSource)
        {
            _updateSource = updateSource;

            return this;
        }

        /// <summary>
        /// Set Inserted Source from the Datagrid
        /// </summary>
        /// <param name="insertSource"></param>
        /// <returns>data grid collection helper</returns>
        public KendoGridDataCollector<T> SetInsertSource(IList<T> insertSource)
        {
            _insertSource = insertSource;

            return this;
        }

        /// <summary>
        /// To Get Result Value
        /// </summary>
        /// <returns>List of data from the data grid</returns>
        public List<T> GenerateResult()
        {
            if (_deleteSource != null)
            {
                if (_deleteSource.Count != 0)
                {
                    foreach (var data in _deleteSource)
                    {
                        _originSource.Remove(FindRemoveList(data));
                    }
                }
            }

            if (_updateSource != null)
            {
                if (_updateSource.Count != 0)
                {
                    foreach (var data in _updateSource)
                    {
                        _originSource.Remove(FindRemoveList(data));
                    }

                    foreach (var data in _updateSource)
                    {
                        _originSource.Add(data);
                    }
                }
            }

            if (_insertSource != null)
            {
                if (_insertSource.Count != 0)
                {
                    foreach (var insertSource in _insertSource)
                    {
                        _originSource.Add(insertSource);
                    }
                }
            }

            return _originSource != null ? _originSource.ToList() : _originSource.ToList();
        }

        #endregion

        #region Util

        private T FindRemoveList(T target)
        {
            Func<T, string, string, bool> Filter = TypeArrayFilter;

            var tempResult = new List<T>();

            for (int index = 0; index < _propertyNames.Count; index++)
            {
                string compareValue = GetValue(target, _propertyNames[index]);

                if (index == 0)
                {
                    _beforeSource = new List<T>();

                    _beforeSource = _originSource.Where(x => Filter(x, _propertyNames[index], compareValue)).ToList(); // COPY SOURCE FOR TEMPORY STORAGE 
                }
                else
                {
                    _beforeSource = _beforeSource.Where(x => Filter(x, _propertyNames[index], compareValue)).ToList(); // OVERRIDE SOURCE FOR UPDATE
                }
            }

            if (_beforeSource.Count > 1)
            {
                throw new System.Exception("Duplicated fields are involved in the data grid");
            }

            return _beforeSource.Single();
        }

        private bool TypeArrayFilter(T target, string propertyName, string compareValue)
        {
            var flag = false;

            var propertyValue = GetValue(target, propertyName);

            if (propertyValue == compareValue)
            {
                flag = true;
            }

            return flag;
        }

        private string GetValue(T type, string propertyName)
        {
            return (string)type.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, type, null);
        }

        #endregion
    }
}
