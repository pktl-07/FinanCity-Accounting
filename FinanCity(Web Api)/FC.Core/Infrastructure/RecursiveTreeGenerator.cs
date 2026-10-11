namespace FC.Core
{
    using System.Linq;
    using System.Collections.Generic;

    public class RecursiveTreeGenerator<T> : IRecursiveTreeGenerator<T>
          where T : IRecursiveItems, new()
    {
        #region  Fields

        private List<T> _traversal = new List<T>();
        private int _indentCounter = 0;

        #endregion

        #region Methods

        /// <summary>
        /// To set parent child relation.
        /// </summary>
        /// <param name="items"></param>
        public void SetRelation(T items)
        {
            SetRootLevelRelation(items);

            SetMultiLevelRelation(items.data, items.SysKey);
        }

        /// <summary>
        /// To Traverse array.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T[] Traverse(T item)
        {
            Clear();

            TraverseRootLevel(item);

            TraverseMultiLevel(item); // add multi level items

            return _traversal.ToArray();
        }

        private void TraverseRootLevel(T items)
        {
            _traversal.Add(items);
        }

        private void TraverseMultiLevel(T items)
        {
            foreach (var item in items.data)
            {
                _traversal.Add((T)item);
                if (item.data != null && item.data.Count() > 0)
                {
                    TraverseMultiLevel((T)item);
                }
            }
        }

        /// <summary>
        /// To Clean array.
        /// </summary>
        public void Clear()
        {
            _traversal.Clear();
        }

        #endregion

        #region Util

        private void SetMultiLevelRelation(IRecursiveItems[] items, string parentsk)
        {
            int counter = 0;
            foreach (var item in items)
            {
                item.SysKey = GeneralUtility.Key;
                item.ParentSK = parentsk;
                item.Indent = counter++;
                if (item.data != null && item.data.Count() > 0)
                {
                    SetMultiLevelRelation(item.data, item.SysKey);
                }
            }
        }

        private void SetRootLevelRelation(T items)
        {
            if (items != null)
            {
                items.SysKey = GeneralUtility.Key.ToString();
                items.Indent = _indentCounter++;
                items.ParentSK = "0";
            }
        }

        #endregion
    }
}
