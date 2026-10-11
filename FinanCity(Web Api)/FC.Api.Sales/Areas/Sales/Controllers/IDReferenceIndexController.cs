namespace FC.Api.SalesAreas.Areas.Sales.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http;
    using FC.Core;
    using FC.Api.SalesAreas.Areas.Sales.Models;
    using FC.Domain.Sales.Business;
    using FC.Domain.Sales.Entity;

    [RoutePrefix("IDReferenceIndex")]
    public class IDReferenceIndexController : BaseApiController
    {
        #region Fields

        IDIndexing _business;
        KendoGridFilterTransalator _kendoGridFilter;
        ReferenceIndexTree _referenceIndexTree;

        #endregion

        #region Ctor

        public IDReferenceIndexController(IDIndexing business,
            ReferenceIndexTree referenceIndexTree,
            KendoGridFilterTransalator kendoGridFilter)
        {
            _referenceIndexTree = referenceIndexTree;
            _business = business;
            _kendoGridFilter = kendoGridFilter;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("get")]
        public IEnumerable<ReferenceIndexTreeModel> Finds()
        {
            try
            {
                var genericTree = _referenceIndexTree.GenericTree;
                var setting = genericTree.Children;  //_converter.Convert(business.Finds()) as List<IndexReferenceModel>;

                setting = setting.Where(x => x.ModuleType != (int)ModuleType.SPO).ToList();
                setting = setting.Where(x => x.ModuleType != (int)ModuleType.POS).ToList();
                setting = setting.Where(x => x.ModuleType != (int)ModuleType.PMS).ToList();

                return setting;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpGet]
        [Route("getByKey/{key}")]
        public IndexReferenceModel Find(string key)
        {
            try
            {
                var model = new IndexReferenceModel
                {
                    Indexer = _business.Finds(filter: f => f.SysKey == key).Single(),
                };

                model.BaseDateCollection = new List<BaseDate>();

                PopuplateModel(model);

                #region Re Index

                var removeIndex = 0;
                var removeItem = model.BaseDateCollection.Where(x => x.PhysicalFieldName == "CreatedDate").SingleOrDefault();

                for (int i = 0; i < model.BaseDateCollection.Count(); i++)
                {
                    if (model.BaseDateCollection[i].PhysicalFieldName == "CreatedDate")
                    {
                        removeIndex = i;
                    }
                }

                if (removeItem != null)
                {
                    model.BaseDateCollection.RemoveAt(removeIndex);
                    model.BaseDateCollection.Insert(0, removeItem);
                }
                #endregion

                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut]
        [Route("update")]
        public IHttpActionResult Update(IDIndexingEntity model)
        {
            try
            {
                using (var scope = TransactionScopeBuilder.CreateReadCommitted())
                {
                    _business.Update(model, filter: f => f.SysKey == model.SysKey);

                    scope.Complete();
                }

                return Ok(_business.Finds(x => x.SysKey == model.SysKey).Single());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Utility

        private void PopuplateModel(IndexReferenceModel model)
        {

            var assemblyList = new string[]
            {
                "FC.Domain.Sales",
                "FC.Domain.Accounts",
            };

            var transType = (TransType)model.Indexer.TransType;

            foreach (var assemblyInfo in assemblyList)
            {
                var assembly = Assembly.Load(assemblyInfo);
                foreach (var type in assembly.GetTypes().Where(x => x.FullName.Contains("Entity")))
                {
                    if (type.IsClass && type.FullName.EndsWith("Entity"))
                    {
                        if (type.GetCustomAttribute<TableAttribute>() != null)
                        {
                            if (type.GetCustomAttribute<TableAttribute>().Name.Equals(transType.ToTableName(), StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (var property in type.GetProperties())
                                {
                                    if (property.Name != "ModifiedDate")
                                    {
                                        if (property.GetCustomAttribute<ColumnAttribute>() != null)
                                        {
                                            if (property.GetCustomAttribute<ColumnAttribute>().IsDate)
                                            {
                                                model.BaseDateCollection.Add(new BaseDate
                                                {
                                                    IsCheck = model.Indexer.BaseDate != null ?
                                                        model.Indexer.BaseDate.Equals(property.GetCustomAttribute<ColumnAttribute>().Name, StringComparison.OrdinalIgnoreCase) : false,

                                                    PhysicalFieldName = property.Name,

                                                    LogicalFieldName = property.GetCustomAttribute<DisplayAttribute>() != null ?
                                                        property.GetCustomAttribute<DisplayAttribute>().Name : property.Name,

                                                });
                                            }
                                        }
                                    }
                                }

                                return; // need to terminate to quite out the function
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
