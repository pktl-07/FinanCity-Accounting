namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Core.Common;

    public class ReferenceIndexTree
    {
        #region Fields

        private IDIndexing _indexing;
        private List<ReferenceIndexTreeModel> _source;
        private GenericTree<ReferenceIndexTreeModel> _genericTree;

        #endregion

        #region Ctor

        public ReferenceIndexTree(IDIndexing indexing)
        {
            _indexing = indexing;

            _source = new List<ReferenceIndexTreeModel>();
            _genericTree = new ReferenceIndexTreeModel();
            IntializeSource();
        }

        #endregion

        #region Methods

        public GenericTree<ReferenceIndexTreeModel> GenericTree
        {
            get
            {
                foreach (var item in _source.GroupBy(x => new { ModuleType = x.ModuleType, ModuleName = x.ModuleName }))
                {
                    // simple root
                    _genericTree.AddChild(new ReferenceIndexTreeModel
                    {
                        IsHeader = true,
                        ModuleName = item.Key.ModuleName ?? "",
                        ModuleType = item.Key.ModuleType,
                    });

                    // add more child
                    foreach (var subItem in _source.Where(x => x.ModuleType == item.Key.ModuleType).OrderBy(x => x.TransType))
                    {
                        subItem.IsHeader = false;
                        _genericTree.AddChild(subItem);
                    }
                }

                _genericTree.Traverse(TreeWorker);

                return _genericTree;
            }
        }

        #endregion

        #region Utilities

        private void IntializeSource()
        {
            try
            {
                foreach (var v in _indexing.Finds())
                {
                    _source.Add(new ReferenceIndexTreeModel
                    {
                        SysKey = v.SysKey,
                        Format = ((IndexingFormat)v.Format).ToDescription(),
                        Prefix = v.Prefix,
                        Postfix = v.Postfix,
                        StartValue = v.StartValue,
                        EndValue = v.EndValue,
                        FormType = v.Description,
                        ModuleType = (int)((TransType)v.TransType).ToModuleType(),
                        ModuleName = ((TransType)v.TransType).ToModuleType().ToDescription(),
                        TransType = v.TransType,
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void TreeWorker(int indent, ReferenceIndexTreeModel model)
        {
            if (model.IsHeader)
                model.Indent = 15;
            else
                model.Indent = 30;
        }

        #endregion
    }

    public class ReferenceIndexTreeModel : GenericTree<ReferenceIndexTreeModel>
    {
        #region Properties

        public string SysKey { get; set; }
        public string FormType { get; set; }
        public int? ModuleType { get; set; }
        public string ModuleName { get; set; }
        public int VoucherId { get; set; }
        public string Prefix { get; set; }
        public string Postfix { get; set; }
        public int StartValue { get; set; }
        public int EndValue { get; set; }
        public string Format { get; set; }
        public bool IsHeader { get; set; }
        public int Indent { get; set; }
        public int TransType { get; set; }

        #endregion
    }
}
