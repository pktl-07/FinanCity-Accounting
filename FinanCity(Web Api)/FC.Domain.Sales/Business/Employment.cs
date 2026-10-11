namespace FC.Domain.Sales.Business
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.Entity;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Infrastructure;

    public class Employment : BusinessLogic<EmploymentEntity, EmploymentDAO>
    {
        #region Fields

        private Logger _logger
        {
            get { return DI.Current.Get<Logger>(); }
        }

        private EmploymentLogger _employmentLogger;

        #endregion

        #region Ctor

        public Employment(EmploymentLogger employmentLogger)
        {
            _employmentLogger = employmentLogger;
        }

        public CareerManagementEntity CareerManagementEntity { get; set; } = new CareerManagementEntity();

        #endregion

        #region Methods

        public override EmploymentEntity Insert(EmploymentEntity entity)
        {
            try
            {
                entity = base.Insert(entity);
             
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override void Update(EmploymentEntity entity,
            Expression<Func<EmploymentEntity, bool>> filter)
        {
            try
            {
                LogProfile(entity);
                base.Update(entity: entity, filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void RunTerminationProcess(PersonEntity personEntity)
        {
            try
            {
                DAO.RunTerminationProcess(personEntity);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void LogProfile(EmploymentEntity entity)
        {
            try
            {
                Guard.IsNotNullOrEmpty(entity.SysKey, "SysKey");

                var orginSource = Finds(filter: f => f.SysKey == entity.SysKey).Single();
                var newSource = entity;

                _employmentLogger.Subscribe(originSource: orginSource, newSource: newSource, 
                    action: data =>
                {
                    _logger.Insert(new LoggerEntity
                    {
                        RefSK = entity.SysKey,
                        OldValue = data.OriginData.ToString(),
                        NewValue = data.NewData.ToString(),
                        Description = data.Description,
                        TableName = data.TableName,
                        Type = data.LoggerType,
                        CMSK = CareerManagementEntity.SysKey
                    });
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
    }

    #region Logger

    public class EmploymentLogger : EntityLoggerBase<EmploymentEntity>
    {
        #region Members

        public CommissionStructureDetail CommissionSchemeDetail
        {
            get { return DI.Current.Get<CommissionStructureDetail>(); }
        }

        public Person Person
        {
            get { return DI.Current.Get<Person>(); }
        }

        #endregion

        #region Methods

        public override void Subscribe(EmploymentEntity originSource,
                  EmploymentEntity newSource,
                  Action<LoggerContext> action)
        {
            // notify scheme detail change
            if (originSource.SchemeDetailSysKey != newSource.SchemeDetailSysKey)
            {
                var oldschemeDetail = CommissionSchemeDetail.Finds(filter: f => f.SysKey == originSource.SchemeDetailSysKey).Single();
                var newschemeDetail = CommissionSchemeDetail.Finds(filter: f => f.SysKey == newSource.SchemeDetailSysKey).Single();

                action(new LoggerContext
                {
                    OriginData = $@"{oldschemeDetail.AgencyPercent} / {oldschemeDetail.AssociatedPercent}",
                    NewData = $@"{newschemeDetail.AgencyPercent} / {newschemeDetail.AssociatedPercent}",
                    Description = $@"Promote to {CommissionSchemeDetail.Finds(filter: f => f.SysKey == newSource.SchemeDetailSysKey).Single().Designation}",
                    TableName = "Employment",
                    LoggerType = (int)LoggerType.PromotionSchemeChange,
                });
            }

            // notify scheme on rank
            if (originSource.Rank != newSource.Rank)
            {
                var oldschemeDetail = CommissionSchemeDetail.Finds(filter: f => f.SysKey == originSource.SchemeDetailSysKey).Single();
                var newschemeDetail = CommissionSchemeDetail.Finds(filter: f => f.SysKey == newSource.SchemeDetailSysKey).Single();

                action(new LoggerContext
                {
                    OriginData = ((RankType)originSource.Rank).ToDescription(),
                    NewData = ((RankType)newSource.Rank).ToDescription(),
                    Description = $"Promote to {((RankType)newSource.Rank).ToDescription()}",
                    TableName = "Employment",
                    LoggerType = (int)LoggerType.RankChange,
                });
            }

            //notify opening acc change
            if (originSource.OpeningAccSales != newSource.OpeningAccSales)
            {
                action(new LoggerContext
                {
                    OriginData = originSource.OpeningAccSales.ToString("c"),
                    NewData = newSource.OpeningAccSales.ToString("c"),
                    Description = $@"Opening Account Sales changed to {newSource.OpeningAccSales}",
                    TableName = "Employment",
                    LoggerType = (int)LoggerType.OpeningAccSalesChange
                });
            }

            //notify T1 change
            if (originSource.L1 != newSource.L1)
            {
                action(new LoggerContext
                {
                    OriginData = originSource.L1 != null ? Person.Finds(filter: f => f.SysKey == originSource.L1).Single().Name : "",
                    NewData = Person.Finds(filter: f => f.SysKey == newSource.L1).Single().Name,
                    Description = $"Changed tier to {Person.Finds(filter: f => f.SysKey == newSource.L1).Single().Name}",
                    TableName = "Employment",
                    LoggerType = (int)LoggerType.TireChange
                });
            }
        }

        #endregion
    }

    #endregion
}
