namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Data.Extensions;
    using Ninject;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Domain.Accounts.Business;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Domain.Auth.Business;
    using FC.Domain.Auth.Entity;
    using FC.Core.Infrastructure.Implements.RecordIndexer;
    using FC.Domain.Sales.Infrastructure;

    public class Person : BusinessLogic<PersonEntity, PersonDAO>, IDataPageable<PersonEntity>
    {
        #region Fields

        private CommissionStructureDetail _commissionStructureDetail;
        private CommissionStructure _commissionStructure;
        private Contact _contact;
        private TreeBuilder _treeBuilder;
        private Employment _employment;
        private PersonIndustry _personIndustry;
        private PersonEducation _personEducation;
        private EmploymentHistory _employmentHistory;
        private SalespersonLogger _salespersonLogger;
        private Logger _logger;
        private ClientShare _clientShare;
        private PayNow _payNow;
        private UserAccount _userAccount;
        private RecordIndexerFactory _recordIndexer;

        #endregion

        #region Members

        //It should be written getter setter due to the Globle.cs file
        //in which we have used timer thread.
        //The HttpContext single thread seem not allowing for creating or spwaning
        //other thread and it is causing User Identity null value. 
        public IFinanCityIdentity FinanCityIdentity
        {
            get
            {
                return DI.Current.Get<IFinanCityIdentity>();
            }
        }

        private CareerManagement _careerManagement
        {
            get { return DI.Current.Get<CareerManagement>(); }
        }

        #endregion

        #region Ctor

        public Person(Contact contact,
            ClientShare clientShare,
            RecordIndexerFactory recordIndexer,
            Employment employment,
            Logger logger,
            UserAccount userAccount,
            SalespersonLogger salespersonLogger,
            PersonEducation personEducation,
            EmploymentHistory employmentHistory,
            PersonIndustry personIndustry,
            CommissionStructureDetail commissionStructureDetail,
            CommissionStructure commissionStructure,
            PayNow payNow)
        {
            _commissionStructureDetail = commissionStructureDetail;
            _commissionStructure = commissionStructure;
            _userAccount = userAccount;
            _payNow = payNow;
            _clientShare = clientShare;
            _logger = logger;
            _salespersonLogger = salespersonLogger;
            _employmentHistory = employmentHistory;
            _contact = contact;
            _employment = employment;
            _personEducation = personEducation;
            _personIndustry = personIndustry;
            _recordIndexer = recordIndexer;
        }

        #endregion

        #region Methods

        public override PersonEntity Insert(PersonEntity entity)
        {
            try
            {
                entity.CreatedBy = FinanCityIdentity.SPSK;
                return base.Insert(entity);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override void Update(PersonEntity entity, Expression<Func<PersonEntity, bool>> filter)
        {
            try
            {
                //_userAccount.Update((s) => new UserAccountEntity
                //{
                //    RecordStatus = (int)RecordStatus.Update,
                //    FirstName = entity.FirstName,
                //    LastName = entity.LastName,
                //}, filter: f => f.SPSK == entity.SysKey);

                LogProfile(entity);

                base.Update(entity: entity, filter: filter);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<PersonEntity> GetClientAndLead(Expression<Func<PersonEntity, bool>> where = null)
        {
            try
            {
                return DAO.GetClientAndLead(where: where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<PersonEntity> GetPager(int currentPage, int pageSize, Expression<Func<PersonEntity, bool>> where = null)
        {
            try
            {
                var result = DAO.GetPager(currentPage, pageSize, where);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<LeadAndContactDTO> GetExternalSalesperson(int currentPage, int pageSize, Expression<Func<LeadAndContactDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetPersonalContactPager(currentPage, pageSize, where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<LeadAndContactDTO> GetClientPager(int currentPage,
            int pageSize,
            Expression<Func<LeadAndContactDTO, bool>> where = null)
        {
            try
            {
                var refSK = FinanCityIdentity.SPSK;
                where = where.Constant((x) => x.TransType == (int)TransType.Client && x.RefSK == refSK);
                return DAO.GetClientPager(currentPage,
                    pageSize,
                    where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IDataPagerResult<EmploymentDTO> GetEmploymentPager(int currentPage,
        int pageSize,
        Expression<Func<EmploymentDTO, bool>> where = null)
        {
            try
            {
                var result = DAO.GetEmploymentPager(currentPage, pageSize, where);
                var data = new List<EmploymentDTO>();

                result.Data.ToList().Each(item =>
                {
                    item.ManagerName = GetSenior(spsk: item.SysKey, rank: RankType.TM)?.Name;
                    item.DirectorName = GetSenior(spsk: item.SysKey, rank: RankType.DD)?.Name;

                    data.Add(item);
                });

                result.Data = data;

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<SalesPersonTireDTO> GetSalepersonTire()
        {
            try
            {
                return DAO.GetSalepersonTire();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<SalespersonRecursiveTree> GetSalespersonRecursiveTree(string key,
            IEnumerable<SalesPersonTireDTO> source,
            bool reverse = false,
            bool? downTree = null)
        {
            try
            {
                DI.Current.Rebind<TreeBuilder>()
                    .To<TreeBuilder>()
                    .WithConstructorArgument("spsk", key)
                    .WithConstructorArgument("source", source)
                    .WithConstructorArgument("downTree", downTree);

                _treeBuilder = DI.Current.Get<TreeBuilder>();

                var tree = _treeBuilder.BuildTree();

                if (reverse)
                {
                    var traverse = tree.Items.Traverse(x => x.Items).ToList();

                    traverse.Reverse();

                    var items = traverse.RecursiveJoin(element => element.SPSK, element => element.ParentSK,
                          (SalespersonRecursiveTree element, int depth, IEnumerable<SalespersonRecursiveTree> children) => new SalespersonRecursiveTree
                          {
                              SPSK = element.SPSK,
                              Name = element.Name,
                              Items = children,
                              Depth = depth,
                              IsActive = element.IsActive,
                              ParentSK = element.ParentSK,
                              Desingation = element.Desingation,
                              Rank = element.Rank,
                              PhotoPath = element.PhotoPath,
                          });
                    return items;
                }

                if (tree.Items == null)
                    return new List<SalespersonRecursiveTree>();

                return tree.Items;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PersonEntity InsertAsSalespersonTransaction(PersonEntity person,
            ContactEntity contact,
            EmploymentEntity employment,
            EmploymentHistoryEntity employmentHistory,
            IEnumerable<IndustryExamEntity> industry,
            IEnumerable<QualificationEntity> qualification,
            PayNowEntity payNow)
        {
            try
            {
                person = Insert(person);

                employment.SysKey = person.SysKey;// set relationship.
                _employment.Insert(employment);

                _careerManagement.Insert(new CareerManagementEntity
                {
                    SPSK = person.SysKey,
                    Rank = employment.Rank,
                    T1 = employment.L1,
                    IsActive = true,
                    ReasonToTerminate = 1,
                    SchemeDetailSK = employment.SchemeDetailSysKey,
                    ApprovedBy = "Default",
                    RecuriterSK = null,
                    CreateDate = GeneralUtility.ServerDate,
                    EffectiveDate = employmentHistory.DateJoin,
                    TransType = (int)TransType.BranchOut,
                    IsDefault = true,
                });

                contact.SysKey = person.SysKey;// set relationship.
                _contact.Insert(contact);

                payNow.ParentSK = person.SysKey;//set relationship.
                _payNow.Insert(payNow);

                employmentHistory.SysKey = person.SysKey;//set relationship
                _employmentHistory.Insert(employmentHistory);

                _personIndustry.Delete(filter: f => f.SPSysKey == person.SysKey);
                _personEducation.Delete(filter: f => f.SPSysKey == person.SysKey);

                industry.Each(d =>
                {
                    _personIndustry.Insert(new PersonIndustryEntity { SPSysKey = person.SysKey, IndusSysKey = d.SysKey });
                });

                qualification.Each(d =>
                {
                    _personEducation.Insert(new PersonEducationEntity { SPSysKey = person.SysKey, EduSysKey = d.SysKey });
                });

                return person;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public PersonEntity UpdateAsSalespersonTransaction(PersonEntity person,
            ContactEntity contact,
            EmploymentEntity employment,
            EmploymentHistoryEntity employmentHistory,
            IEnumerable<IndustryExamEntity> industry,
            IEnumerable<QualificationEntity> qualification,
            PayNowEntity payNow)
        {
            try
            {
                Update(person, filter: f => f.SysKey == person.SysKey);
                _employment.Update(employment, filter: f => f.SysKey == person.SysKey);

                //_careerManagement.Update(selector: s => new CareerManagementEntity { EffectiveDate = employmentHistory.DateJoin },
                //    filter: f => f.SysKey == person.SysKey);

                _contact.Update(contact, filter: f => f.SysKey == person.SysKey);
                _employmentHistory.Update(employmentHistory, filter: f => f.SysKey == person.SysKey);
                _payNow.Update(payNow, filter: f => f.ParentSK == person.SysKey);
                _personIndustry.Delete(filter: f => f.SPSysKey == person.SysKey);
                _personEducation.Delete(filter: f => f.SPSysKey == person.SysKey);

                industry.Each(d =>
                {
                    _personIndustry.Insert(new PersonIndustryEntity { SPSysKey = person.SysKey, IndusSysKey = d.SysKey });
                });

                qualification.Each(d =>
                {
                    _personEducation.Insert(new PersonEducationEntity { SPSysKey = person.SysKey, EduSysKey = d.SysKey });
                });

                _userAccount.Update((s) => new UserAccountEntity
                {
                    RecordStatus = (int)RecordStatus.Update,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                }, filter: f => f.SPSK == person.SysKey);

                return person;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void DeleteAsSalespersonTransaction(string spsk)
        {
            try
            {
                _personIndustry.Delete(filter: f => f.SPSysKey == spsk);
                _personEducation.Delete(filter: f => f.SPSysKey == spsk);
                _employmentHistory.Delete(filter: f => f.SysKey == spsk);
                _contact.Delete(filter: f => f.SysKey == spsk);
                _employment.Delete(filter: f => f.SysKey == spsk);

                Delete(filter: f => f.SysKey == spsk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string CreateAsClientTransaction(PersonEntity person,
            ContactEntity contact)
        {
            try
            {
                var c = _recordIndexer.Generate(TransType.Client, person);

                person.R1 = c.R1;
                person.Prefix = c.Prefix;
                person.Postfix = c.Postfix;
                person.VoucherId = c.VoucherId;

                Insert(person);

                contact.SysKey = person.SysKey;
                _contact.Insert(contact);

                if (FinanCityIdentity.UserAccountType == UserAccountType.SalespersonAccount)
                    _clientShare.Insert(new ClientShareEntity { SysKey = person.SysKey, RefSK = FinanCityIdentity.SPSK });

                _clientShare.Insert(new ClientShareEntity { SysKey = person.SysKey, RefSK = "1" });

                return person.SysKey;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void DeleteAsClientTransaction(string syskey)
        {
            try
            {
                Delete(filter: x => x.SysKey == syskey);
                _contact.Delete(filter: x => x.SysKey == syskey);
                _clientShare.Delete(filter: x => x.SysKey == syskey);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpdateAsClientTransaction(PersonEntity person,
                    ContactEntity contact)
        {
            try
            {
                Update(person, filter: f => f.SysKey == person.SysKey);
                _contact.Update(contact, filter: f => f.SysKey == contact.SysKey);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<IndustryExamEntity> GetIndustryExamByKey(string spsk)
        {
            try
            {
                return DAO.GetIndustryExamByKey(spsk);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<QualificationEntity> GetQualificationExamByKey(string spsk)
        {
            try
            {
                return DAO.GetQualificationExamByKey(spsk);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsTierCommission(string spsk)
        {
            try
            {
                var p = Finds(filter: f => f.SysKey == spsk).Single();

                if (p.TransType == (int)TransType.InternalSalesperson)
                {
                    var d = _employment.Finds(filter: f => f.SysKey == spsk).Single()
                        .SchemeDetailSysKey;
                    var s = _commissionStructureDetail.Finds(filter: f => f.SysKey == d).Single().ParentSK;

                    return _commissionStructure.Finds(filter: f => f.SysKey == s).Single().BaseHeaderSK == "02";
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public SalesPersonTireDTO GetSenior(string spsk, RankType rank)
        {
            var downTree = DAO.GetCareerManagementDownTree(date: GeneralUtility.ServerDate);
            var items = GetSalespersonRecursiveTree(key: spsk,
                    source: downTree,
                    downTree: false,
                    reverse: false);

            var preResult = items.RecursiveSelect(x => x.Items)
                .Where(x => x.Rank == (int)rank)
                .FirstOrDefault();

            if (preResult != null)
            {
                return downTree?.Where(x => x.SPSK == preResult?.SPSK
                && x.SPSK != spsk)?.SingleOrDefault();
            }

            return null;
        }

        #endregion

        #region Utilities

        private void LogProfile(PersonEntity entity)
        {
            try
            {
                var orginSource = Finds(filter: f => f.SysKey == entity.SysKey).Single();
                var newSource = entity;

                _salespersonLogger.Subscribe(originSource: orginSource,
                    newSource: newSource,
                    action: notify =>
                    {
                        _logger.Insert(new LoggerEntity
                        {
                            RefSK = entity.SysKey,
                            OldValue = notify.OriginData.ToString(),
                            NewValue = notify.NewData.ToString(),
                            Description = notify.Description,
                            TableName = notify.TableName,
                            Type = notify.LoggerType
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

    public class SalespersonLogger : EntityLoggerBase<PersonEntity>
    {
        private Department _department;

        public SalespersonLogger(Department department)
        {
            _department = department;
        }

        public override void Subscribe(PersonEntity originSource,
            PersonEntity newSource,
            Action<LoggerContext> action)
        {
            if (originSource.TransType == (int)TransType.InternalSalesperson)
            {
                //Department change
                if (originSource.DepartmentSK != newSource.DepartmentSK)
                {
                    var oldDepartment = _department.Finds(filter: f => f.SysKey == originSource.DepartmentSK).SingleOrDefault() ?? new Accounts.Entity.DepartmentEntity();
                    var newDepartment = _department.Finds(filter: f => f.SysKey == newSource.DepartmentSK).SingleOrDefault() ?? new Accounts.Entity.DepartmentEntity();

                    action(new LoggerContext
                    {
                        OriginData = oldDepartment.Description,
                        NewData = newDepartment.Description,
                        Description = $@"Change Department {oldDepartment.Description} To {newDepartment.Description}",
                        TableName = "Person",
                        LoggerType = (int)LoggerType.DepartmentChanage,
                    });
                }

                if (originSource.IsActive != newSource.IsActive)
                {
                    action(new LoggerContext
                    {
                        OriginData = originSource.IsActive == true ? "Active" : "In Active",
                        NewData = newSource.IsActive == true ? "Active" : "In Active",
                        Description = $@"The regarding salesperson has been {((SalepersonStatus)newSource.ReasonToTerminate).ToDescription()}",
                        TableName = "Person",
                        LoggerType = (int)LoggerType.EmploymentStatusChange,
                    });
                }

                if (originSource.ReasonToTerminate != newSource.ReasonToTerminate)
                {
                    action(new LoggerContext
                    {
                        OriginData = ((SalepersonStatus)originSource.ReasonToTerminate).ToDescription(),
                        NewData = ((SalepersonStatus)newSource.ReasonToTerminate).ToDescription(),
                        Description = $@"The regarding salesperson has been { ((SalepersonStatus)newSource.ReasonToTerminate).ToDescription()}",
                        TableName = "Person",
                        LoggerType = (int)LoggerType.EmploymentStatusChange,
                    });
                }
            }
        }
    }

    #endregion
}
