namespace FC.Domain.Sales.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Ninject;
    using FC.Core;
    using FC.Core.Data;
    using FC.Core.Infrastructure.Interfaces;
    using FC.Domain.Sales.DAO;
    using FC.Domain.Sales.DTO;
    using FC.Domain.Sales.Entity;
    using FC.Core.Infrastructure.Implements.RecordIndexer;
    using FC.Domain.Sales.Infrastructure;

    public class PropertyTransaction : BusinessLogic<PropertyTransactionEntity, PropertyTransactionDAO>
    {
        #region Fields

        private Contact _contact;
        private PropertyTransactionDetail _propertyDetail;
        private OverridingSchemeFactory _factory;
        private OverridingScheme _overridingScheme;
        private CurrencyTransaction _currencyTransaction;
        private CurrencyRate _currencyRate;
        private OverridingProfile _overridingProfile;
        private Person _person;
        private PTOClient _ptoClient;
        private TNC _tnc;
        private IFinanCityIdentity _finanCityIdentity;
        private MarketingExpense _promotionItemsExpense;
        private ClientShare _clientShare;
        private LeadManagementBuilder _leadManagementBuilder;
        private OverridingProfileCollection _overrdingProfileCollection;
        private KendoGridDataCollector<PTOClientEntity> _clientCollector;
        private KendoGridDataCollector<PropertyTransactionDetailEntity> _salespersonCollector;
        private IMultiCurrencyConverter _multiCurrencyConverter;
        private CommissionStructureDetail _commissionStructureDetail;
        private RecordIndexerFactory _recordIndexer;
        private Notification _notification;
        private SalesInvoiceLog _salesInvoiceLog;

        private SalesInvoice _salesInvoice
        {
            get { return DI.Current.Get<SalesInvoice>(); }
        }

        #endregion

        #region Ctor

        public PropertyTransaction(Contact contact,
            RecordIndexerFactory recordIndexer,
            ClientShare clientShare,
            TNC tnc,
            CommissionStructureDetail commissionStructureDetail,
            MarketingExpense promotionItemExpense,
            PropertyTransactionDetail propertyDetail,
            OverridingScheme overridingScheme,
            CurrencyRate currencyRate,
            IFinanCityIdentity finanCityIdentity,
            OverridingProfile overridingProfile,
            CurrencyTransaction currencyTransaction,
            Person person,
            PTOClient ptoClient,
            KendoGridDataCollector<PTOClientEntity> clientCollector,
            KendoGridDataCollector<PropertyTransactionDetailEntity> salespersonCollector,
            OverridingSchemeFactory factory,
            IMultiCurrencyConverter multiCurrencyConverter,
            LeadManagementBuilder leadManagementBuilder,
            OverridingProfileCollection overridingProfileCollection,
            Notification notification,
            SalesInvoiceLog salesInvoiceLog)
        {
            _salesInvoiceLog = salesInvoiceLog;
            _overrdingProfileCollection = overridingProfileCollection;
            _recordIndexer = recordIndexer;
            _clientShare = clientShare;
            _commissionStructureDetail = commissionStructureDetail;
            _multiCurrencyConverter = multiCurrencyConverter;
            _salespersonCollector = salespersonCollector;
            _clientCollector = clientCollector;
            _promotionItemsExpense = promotionItemExpense;
            _finanCityIdentity = finanCityIdentity;
            _tnc = tnc;
            _ptoClient = ptoClient;
            _person = person;
            _overridingProfile = overridingProfile;
            _currencyRate = currencyRate;
            _factory = factory;
            _overridingScheme = overridingScheme;
            _contact = contact;
            _currencyTransaction = currencyTransaction;
            _propertyDetail = propertyDetail;
            _leadManagementBuilder = leadManagementBuilder;
            _notification = notification;

            this.CanReverseOrVoid = (key) =>
            {
                _salesInvoice.Finds(filter: f => f.SysKey == key && f.RecordStatus == (int)RecordStatus.Post).Each(inv =>
                {
                    throw new Exception($@"The regarding Invoice {inv.R1} should be void.");
                });

                return true;
            };
        }

        #endregion

        #region Methods

        public IDataPagerResult<PropertyTransactionDTO> GetPager(int currentPage, int pageSize, Expression<Func<PropertyTransactionDTO, bool>> where = null)
        {
            try
            {
                return DAO.GetPager(currentPage, pageSize, where);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public PropertyTransactionEntity InsertAsTransaction(PropertyTransactionEntity property,
            ContactEntity content,
            TNC_Entity tnc,
            IEnumerable<PTOClientEntity> clients,
            IEnumerable<PropertyTransactionDetailEntity> internals,
            IEnumerable<PropertyTransactionDetailEntity> externals,
            IEnumerable<MarketingExpenseEntity> insertedExpense)
        {
            try
            {
                var key = GeneralUtility.Key;
                property.SysKey = tnc.SysKey = content.SysKey = key;

                //Set mailing client and salesperson
                SetMailingClient(
                    property: property,
                    inserted: clients);

                SetMailingSalesperson(
                    property: property,
                    inserted: internals);

                Insert(property);

                _contact.Insert(content);

                tnc.CURRATE = property.CURRATE;
                tnc.CURROP = property.CURROP;
                tnc.CURRSK = property.CURRSK;

                _tnc.Insert(tnc);

                clients.Each(client =>
                {
                    client.PTOSK = property.SysKey;
                    client.TransType = (int)TransType.Client;
                    _ptoClient.Insert(client);
                });

                internals.Each(inter =>
                {
                    inter.PTOSK = property.SysKey;
                    inter.CURRATE = property.CURRATE;
                    inter.CURRSK = property.CURRSK;
                    inter.CURROP = property.CURROP;

                    _propertyDetail.Insert(inter);
                });

                externals.Each(ext =>
                {
                    ext.PTOSK = property.SysKey;
                    ext.CURRATE = property.CURRATE;
                    ext.CURRSK = property.CURRSK;
                    ext.CURROP = property.CURROP;

                    _propertyDetail.Insert(ext);
                });

                insertedExpense.Each(pro =>
                {
                    pro.PTOSK = property.SysKey;
                    _promotionItemsExpense.Insert(pro);
                });

                UpdateMarketingExpense(property.SysKey);

                BindSalespersonClient(clients: clients,
                     internals: internals);

                UpdateLeadToClientStauts(originClients: null,
                    insertedClients: clients,
                    recordStatus: property.RecordStatus,
                    ptosk: property.SysKey);

                SetNotification(entity: property);

                return property;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void UpdateAsTransaction(PropertyTransactionEntity property,
           ContactEntity contact,
           TNC_Entity tnc,
           IEnumerable<PTOClientEntity> insertedClients,
           IEnumerable<PTOClientEntity> updatedClients,
           IEnumerable<PTOClientEntity> deletedClients,
           IEnumerable<PropertyTransactionDetailEntity> insertedInternals,
           IEnumerable<PropertyTransactionDetailEntity> updatedInternals,
           IEnumerable<PropertyTransactionDetailEntity> deletedInternals,
           IEnumerable<PropertyTransactionDetailEntity> insertedExternals,
           IEnumerable<PropertyTransactionDetailEntity> updatedExternals,
           IEnumerable<PropertyTransactionDetailEntity> deletedExternals,
           IEnumerable<MarketingExpenseEntity> insertedExpense,
           IEnumerable<MarketingExpenseEntity> updatedExpense,
           IEnumerable<MarketingExpenseEntity> deletedExpense)
        {
            try
            {
                SetMailingClient(property: property,
                                inserted: insertedClients,
                                updated: updatedClients,
                                deleted: deletedClients);

                SetMailingSalesperson(property: property,
                                     inserted: insertedInternals,
                                     updated: updatedInternals,
                                     deleted: deletedInternals);

                Update(property, filter: f => f.SysKey == property.SysKey);

                _contact.Update(contact, filter: f => f.SysKey == contact.SysKey);

                tnc.CURROP = property.CURROP;
                tnc.CURRSK = property.CURRSK;
                tnc.CURRATE = property.CURRATE;
                tnc.RecordStatus = property.RecordStatus;

                _tnc.Update(tnc, filter: f => f.SysKey == property.SysKey);

                insertedClients.Each(client =>
                {
                    client.PTOSK = property.SysKey;
                    _ptoClient.Insert(client);
                });

                updatedClients.Each(client =>
                {
                    _ptoClient.Update(client, filter: f => f.SysKey == client.SysKey);
                });

                deletedClients.Each(client =>
                {
                    _ptoClient.Delete(filter: f => f.SysKey == client.SysKey);
                });

                insertedInternals.Each(inter =>
                {
                    inter.PTOSK = property.SysKey;

                    inter.CURROP = property.CURROP;
                    inter.CURRSK = property.CURRSK;
                    inter.CURRATE = property.CURRATE;

                    _propertyDetail.Insert(inter);
                });

                updatedInternals.Each(inter =>
                {
                    inter.CURROP = property.CURROP;
                    inter.CURRSK = property.CURRSK;
                    inter.CURRATE = property.CURRATE;

                    _propertyDetail.Update(inter, filter: f => f.SysKey == inter.SysKey);
                });

                deletedInternals.Each(inter =>
                {
                    _propertyDetail.Delete(filter: f => f.SysKey == inter.SysKey);
                });

                insertedExternals.Each(ext =>
                {
                    ext.PTOSK = property.SysKey;
                    ext.CURROP = property.CURROP;
                    ext.CURRSK = property.CURRSK;
                    ext.CURRATE = property.CURRATE;

                    _propertyDetail.Insert(ext);
                });

                updatedExternals.Each(ext =>
                {
                    ext.CURROP = property.CURROP;
                    ext.CURRSK = property.CURRSK;
                    ext.CURRATE = property.CURRATE;

                    _propertyDetail.Update(ext, filter: f => f.SysKey == ext.SysKey);
                });

                deletedExternals.Each(ext =>
                {
                    _propertyDetail.Delete(filter: f => f.SysKey == ext.SysKey);
                });

                insertedExpense.Each(pro =>
                {
                    pro.PTOSK = property.SysKey;
                    _promotionItemsExpense.Insert(pro);
                });

                updatedExpense.Each(pro =>
                {
                    _promotionItemsExpense.Update(pro, filter: f => f.SysKey == pro.SysKey);
                });

                deletedExpense.Each(pro =>
                {
                    _propertyDetail.Delete(filter: f => f.SysKey == pro.SysKey);
                });

                UpdateMarketingExpense(property.SysKey);

                BindSalespersonClient(clients: _ptoClient.Finds(filter: f => f.PTOSK == property.SysKey),
                internals: _propertyDetail.Finds(filter: f => f.PTOSK == property.SysKey));

                //BindSalespersonClient(insertedClients: insertedClients,
                //    updatedClients: updatedClients,
                //    deletedClients: deletedClients,
                //    insertedInternals: insertedInternals,
                //    updatedInternals: updatedInternals,
                //    deletedInternals: deletedInternals,
                //    ptosk: property.SysKey);

                UpdateLeadToClientStauts(originClients: _ptoClient.Finds(filter: f => f.PTOSK == property.SysKey),
                                        insertedClients: insertedClients,
                                        deletedClients: deletedClients,
                                        updatedClients: updatedClients,
                                        recordStatus: property.RecordStatus,
                                        ptosk: property.SysKey);

                SetNotification(entity: property);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void PostAsTransaction(PropertyTransactionEntity property,
            ContactEntity contact,
            TNC_Entity tnc,
            IEnumerable<PTOClientEntity> insertedClients,
            IEnumerable<PTOClientEntity> updatedClients,
            IEnumerable<PTOClientEntity> deletedClients,
            IEnumerable<PropertyTransactionDetailEntity> insertedInternals,
            IEnumerable<PropertyTransactionDetailEntity> updatedInternals,
            IEnumerable<PropertyTransactionDetailEntity> deletedInternals,
            IEnumerable<PropertyTransactionDetailEntity> insertedExternals,
            IEnumerable<PropertyTransactionDetailEntity> updatedExternals,
            IEnumerable<PropertyTransactionDetailEntity> deletedExternals,
            IEnumerable<MarketingExpenseEntity> insertedExpense,
            IEnumerable<MarketingExpenseEntity> updatedExpense,
            IEnumerable<MarketingExpenseEntity> deletedExpense)
        {
            try
            {
                var banker = _person.Finds(filter: f => f.SysKey == property.BankerSK).Single();
                var bank = _contact.Finds(filter: f => f.SysKey == banker.VendorSK).Single();

                //mailing default contact as bank
                contact.MailingBuildingHouseNo = bank.BuildingHouseNo;
                contact.MailingAddress1 = bank.Address1;
                contact.MailingAddress2 = bank.Address2;
                contact.MailingFloorUnit = bank.FloorUnit;
                contact.MailingPostalCode = bank.PostalCode;
                contact.MailingCountry = bank.Country;

                property.RecordStatus = (int)RecordStatus.Post;

                BindSalesDetail(property, insertedInternals);
                BindSalesDetail(property, updatedInternals);

                BindSalesDetail(property, insertedExternals);
                BindSalesDetail(property, updatedExternals);

                insertedExpense.Each(a => a.RecordStatus = (int)RecordStatus.Post);
                updatedExpense.Each(a => a.RecordStatus = (int)RecordStatus.Post);

                UpdateAsTransaction(property: property,
                    contact: contact,
                    tnc: tnc,
                    insertedClients: insertedClients,
                    updatedClients: updatedClients,
                    deletedClients: deletedClients,
                    insertedInternals: insertedInternals,
                    updatedInternals: updatedInternals,
                    deletedInternals: deletedInternals,
                    insertedExternals: insertedExternals,
                    updatedExternals: updatedExternals,
                    deletedExternals: deletedExternals,
                    insertedExpense: insertedExpense,
                    updatedExpense: updatedExpense,
                    deletedExpense: deletedExpense);

                #region OR

                // hook the event
                _factory.OnGetLastBatchNo += (batch) =>
                {
                    Update(selector: a => new PropertyTransactionEntity
                    {
                        RecordStatus = (int)RecordStatus.Post,
                        ORBatchNo = batch,
                    }, filter: f => f.SysKey == property.SysKey);

                    var invoice = _salesInvoice.Finds(filter: f => f.SysKey == property.SysKey).SingleOrDefault();

                    _multiCurrencyConverter.ToBaseCurrency(property); //convert to base currency.
                
                    _salesInvoice.Insert(entity: new SalesInvoiceEntity
                    {
                        SysKey = property.SysKey,
                        RecordStatus = (int)RecordStatus.New,
                        TAXSK = property.TaxSK,
                        ActualAmt = property.TotalAmount,
                        ActualGST = property.TaxPercent,
                        MailingType = (int)MailingType.MailAsVendor, // mailing default as vendor
                        ReceivableStatus = (int)SalesViewType.Pending,
                        PayableStatus = (int)SalesViewType.Pending,
                        PaymentTermSK = "2",
                        CURRSK = property.CURRSK,
                        CURRATE = property.CURRATE,
                        CURROP = property.CURROP
                    });
              
                    _currencyTransaction.BulkInsert(GetCurrencyHistory(property: property));

                    var details = _propertyDetail.Finds(filter: f => f.PTOSK == property.SysKey);

                    _overridingProfile.BulkInsert(BindOverridindgProfile(salesOrder: property,
                        saleOrderList: details));
                };

                // insert records if needed when make change on base commission scheme percentage
                _factory.OnRequireDbInsert += (data) =>
                {
                    _overridingScheme.BulkInsert(data as IList<OverridingSchemeEntity>);
                };

                //Run the Process
                _factory.Run(PromoteTypeEnum.OvrridingScheme);

                #endregion

                //needs to generate a new key
                //when the invoice is void and it should cut 
                //off entity relationship for CR,CP ect...
                property.SysKey = DAO.RunMigratingKeyChangeProcess(property);
                //return key;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ReverseAsTransaction(string syskey)
        {
            try
            {
                if (this.CanReverseOrVoid(syskey))
                {
                    var invoice = _salesInvoice.Finds(filter: f => f.SysKey == syskey).Single();

                    if (invoice.RecordStatus == (int)RecordStatus.Post)
                        throw new Exception($"Invoice No: {invoice.R1} had been posted regarding with this transaction.");

                    _currencyTransaction.Delete(filter: f => f.ParentSK == syskey);
                    _overridingProfile.Delete(filter: f => f.PTOSK == syskey);

                    _propertyDetail.Update(selector: a => new PropertyTransactionDetailEntity
                    {
                        RecordStatus = (int)RecordStatus.Update
                    }, filter: f => f.PTOSK == syskey);

                    _tnc.Update(selector: t => new TNC_Entity
                    {
                        RecordStatus = (int)RecordStatus.Update,
                    }, filter: f => f.SysKey == syskey);

                    _promotionItemsExpense.Update(selector: a => new MarketingExpenseEntity
                    {
                        RecordStatus = (int)RecordStatus.Update
                    }, filter: f => f.PTOSK == syskey);

                    Update(selector: (a) => new PropertyTransactionEntity { RecordStatus = (int)RecordStatus.Update },
                        filter: f => f.SysKey == syskey);

                    _salesInvoice.Delete(filter: f => f.SysKey == syskey);
                    _salesInvoiceLog.Insert(new SalesInvoiceLogEntity
                    {
                        R1 = invoice.R1,
                        Prefix = invoice.Prefix,
                        Postfix = invoice.Postfix,
                        VoucherId = invoice.VoucherId,
                        RecordStatus = (int)RecordStatus.Void,
                        PTOSK = syskey,
                        TAXSK = invoice.TAXSK,
                        ActualAmt = invoice.ActualAmt,
                        ReceivableStatus = 1,
                        PayableStatus = 1,
                        ActualGST = invoice.ActualGST,
                        InvoiceDate = invoice.InvoiceDate,
                        CompletionDate = invoice.CompletionDate,
                        CURRATE = invoice.CURRATE,
                        CURRSK = invoice.CURRSK,
                        CURROP = invoice.CURROP,
                        JRef = invoice.JRef,
                        Remark = invoice.Remark,
                        PaymentTermSK = invoice.PaymentTermSK,
                        MailingType = invoice.MailingType,
                        SecondRef = invoice.SecondRef,
                    });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void VoidAsTransaction(string syskey)
        {
            try
            {
                if (this.CanReverseOrVoid(syskey))
                {
                    var salesInvoice = _salesInvoice.Finds(filter: f => f.SysKey == syskey).SingleOrDefault();

                    if (salesInvoice != null && salesInvoice.RecordStatus == (int)RecordStatus.Post)
                        throw new Exception($@"The regarding Sales Invoice '${salesInvoice.R1}' should void.");

                    _salesInvoice.Update(selector: (s) => new SalesInvoiceEntity
                    {
                        RecordStatus = (int)RecordStatus.Void,
                    }, filter: f => f.SysKey == syskey);

                    _propertyDetail.Update(selector: (s) => new PropertyTransactionDetailEntity
                    {
                        RecordStatus = (int)RecordStatus.Void
                    },
                    filter: f => f.PTOSK == syskey);

                    _tnc.Update(selector: (s) => new TNC_Entity
                    {
                        RecordStatus = (int)RecordStatus.Void,
                    }, filter: f => f.SysKey == syskey);

                    _ptoClient.Update(selector: (s) => new PTOClientEntity
                    {
                        RecordStatus = (int)RecordStatus.Void,
                    }, filter: f => f.PTOSK == syskey);

                    Update(selector: (s) => new PropertyTransactionEntity { RecordStatus = (int)RecordStatus.Void },
                        filter: f => f.SysKey == syskey);

                    UpdateLeadToClientStauts(
                        originClients: _ptoClient.Finds(filter: f => f.PTOSK == syskey),
                        recordStatus: (int)RecordStatus.Void,
                        ptosk: syskey);

                    SetNotification(entity: this.Finds(filter: f => f.SysKey == syskey).Single());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetSingleKey(string key)
        {
            var log = _salesInvoiceLog.Finds(filter: f => f.SysKey == key).SingleOrDefault();
            if (log != null)
                return log.PTOSK;

            return key;
        }

        #endregion

        #region Utility

        private void UpdateMarketingExpense(string key)
        {
            var d = _propertyDetail.Finds(filter: f => f.PTOSK == key);
            var e = _promotionItemsExpense.Finds(filter: f => f.PTOSK == key);
            var t = e.Sum(x => x.Amount.ToRoundUp());

            //var avg = (t / d.Count()).ToRoundUp();
            //update marketing expense
            d.Each(mk =>
            {
                _propertyDetail.Update(selector: s => new PropertyTransactionDetailEntity
                {
                    //require to set record status explicitly.
                    RecordStatus = mk.RecordStatus,
                    MarketingExpense = (t / d.Count()).ToRoundUp(),
                }, filter: f => f.SysKey == mk.SysKey);
            });
        }

        private IEnumerable<CurrencyTransactionEntity> GetCurrencyHistory(
            PropertyTransactionEntity property)
        {
            var currencyTransactionRate = new List<CurrencyTransactionEntity>();
            var currencyRateList = _currencyRate.Finds();

            foreach (var curr in currencyRateList)
            {
                if (curr.SysKey == property.CURRSK)
                {
                    currencyTransactionRate.Add(new CurrencyTransactionEntity
                    {
                        ParentSK = property.SysKey,
                        CURRATE = property.CURRATE,
                        CURROP = property.CURROP,
                        CURRSK = property.CURRSK,
                    });
                }
                else
                {
                    currencyTransactionRate.Add(new CurrencyTransactionEntity
                    {
                        ParentSK = property.SysKey,
                        CURRATE = curr.Rate,
                        CURROP = curr.Operator,
                        CURRSK = curr.SysKey,
                    });
                }
            }
            return currencyTransactionRate;
        }

        private List<OverridingProfileEntity> BindOverridindgProfile(
            PropertyTransactionEntity salesOrder,
            IEnumerable<PropertyTransactionDetailEntity> saleOrderList)
        {
            var result = new List<OverridingProfileEntity>();

            _overrdingProfileCollection.AddContext(salesOrder,
                saleOrderList);

            foreach (var item in _overrdingProfileCollection)
            {
                var current = item.CurrentSalesperson;
                var tierOne = item.TierOne;
                var tierTwo = item.TierTwo;
                var tierThree = item.TierThree;
                var tierFour = item.TierFour;
                var tierFive = item.TierFive;

                var dd = item.GetSenior(RankType.DD);
                var tm = item.GetSenior(RankType.TM);

                result.Add(new OverridingProfileEntity
                {
                    PTOSK = item.SalesOrder.SysKey,
                    SPSK = current.SysKey,
                    L1SK = item.GetTierOrTerminationReason(tierOne),
                    L2SK = item.GetTierOrTerminationReason(tierTwo),
                    L3SK = item.GetTierOrTerminationReason(tierThree),
                    L4SK = item.GetTierOrTerminationReason(tierFour),
                    L5SK = item.GetTierOrTerminationReason(tierFive),
                    SPRank = current.Rank,
                    OpeningAccSales = current.OpeningAccSales,
                    ResetCommissionEffectiveStartDate = current.ResetCommissionEffectiveStartDate,
                    L1Rank = tierOne?.Rank ?? 0,
                    L2Rank = tierTwo?.Rank ?? 0,
                    L3Rank = tierThree?.Rank ?? 0,
                    L4Rank = tierFour?.Rank ?? 0,
                    L5Rank = tierFive?.Rank ?? 0,
                    Rank = current.Rank,
                    MGRSK = tm?.SPSK,
                    MGRPayoutPercent = tm?.Payout ?? 0,
                    DDSK = dd?.SPSK,
                    DDPayoutPercent = dd?.Payout ?? 0,
                    UpTo = item.GetCommissionLevel(),
                });
            }

            return result;
        }

        private void SetMailingSalesperson(
            PropertyTransactionEntity property,
            IEnumerable<PropertyTransactionDetailEntity> inserted)
        {
            this.SetMailingSalesperson(property: property,
                                       inserted: inserted,
                                       updated: new List<PropertyTransactionDetailEntity>(),
                                       deleted: new List<PropertyTransactionDetailEntity>());
        }

        private void SetMailingSalesperson(PropertyTransactionEntity property,
           IEnumerable<PropertyTransactionDetailEntity> inserted,
           IEnumerable<PropertyTransactionDetailEntity> updated,
           IEnumerable<PropertyTransactionDetailEntity> deleted)
        {
            var clients = _salespersonCollector.SetKeys(k => k.Add(x => x.SysKey))
                                               .SetOriginSource(_propertyDetail.Finds(filter: f => f.PTOSK == property.SysKey).ToList())
                                               .SetInsertSource(inserted.ToList())
                                               .SetUpdateSource(updated.ToList())
                                               .SetDeleteSource(deleted.ToList())
                                               .GenerateResult();

            var main = clients.Where(x => x.IsMain == true).Single();
            property.MailingSPSK = main.SPSK;
        }

        private void SetMailingClient(
           PropertyTransactionEntity property,
           IEnumerable<PTOClientEntity> inserted)
        {
            try
            {
                this.SetMailingClient(
                    property: property,
                    inserted: inserted,
                    updated: new List<PTOClientEntity>(),
                    deleted: new List<PTOClientEntity>());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void SetMailingClient(
           PropertyTransactionEntity property,
           IEnumerable<PTOClientEntity> inserted,
           IEnumerable<PTOClientEntity> updated,
           IEnumerable<PTOClientEntity> deleted)
        {
            try
            {
                var clients = _clientCollector.SetKeys(k => k.Add(x => x.SysKey))
                                              .SetOriginSource(_ptoClient.Finds(filter: f => f.PTOSK == property.SysKey).ToList())
                                              .SetInsertSource(inserted.ToList())
                                              .SetUpdateSource(updated.ToList())
                                              .SetDeleteSource(deleted.ToList())
                                              .GenerateResult();

                if (property.BillToCoBroke)
                    property.MailingClientSK = property.BankerSK;
                else
                {
                    var main = clients.Where(x => x.IsMain == true).Single();
                    property.MailingClientSK = main.ClientSK;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BindSalespersonClient(
           IEnumerable<PTOClientEntity> clients,
           IEnumerable<PropertyTransactionDetailEntity> internals)
        {
            var refsk = _person.FinanCityIdentity.AccountType == (int)UserAccountType.AdminAccount ?
                internals.Where(x => x.IsMain == true).Single().SPSK : "1";

            clients.Each(client =>
            {
                var c = _clientShare.Finds(filter: f => f.SysKey == client.ClientSK && f.RefSK == refsk).SingleOrDefault();

                if (c == null)
                {
                    _clientShare.Insert(new ClientShareEntity
                    {
                        SysKey = client.ClientSK,
                        RefSK = refsk,
                    });
                }
            });
        }

        //private void BindSalespersonClient(
        //  IEnumerable<PTOClientEntity> insertedClients,
        //  IEnumerable<PTOClientEntity> updatedClients,
        //  IEnumerable<PTOClientEntity> deletedClients,
        //  IEnumerable<PropertyTransactionDetailEntity> insertedInternals,
        //  IEnumerable<PropertyTransactionDetailEntity> updatedInternals,
        //  IEnumerable<PropertyTransactionDetailEntity> deletedInternals,
        //  string ptosk)
        //{

        //    var clients = _clientCollector.SetKeys(k => k.Add(x => x.SysKey))
        //       .SetOriginSource(_ptoClient.Finds(filter: f => f.PTOSK == ptosk).ToList())
        //       .SetInsertSource(insertedClients.ToList())
        //       .SetUpdateSource(updatedClients.ToList())
        //       .SetDeleteSource(deletedClients.ToList())
        //       .GenerateResult();

        //    var salespersons = _salespersonCollector.SetKeys(k => k.Add(x => x.SysKey))
        //        .SetOriginSource(_propertyDetail.Finds(filter: f => f.PTOSK == ptosk).ToList())
        //        .SetInsertSource(insertedInternals.ToList())
        //        .SetUpdateSource(updatedInternals.ToList())
        //        .SetDeleteSource(deletedInternals.ToList())
        //        .GenerateResult();

        //    BindSalespersonClient(clients: clients,
        //        internals: salespersons);
        //}

        private void BindSalesDetail(
            PropertyTransactionEntity transaction,
            IEnumerable<PropertyTransactionDetailEntity> detail)
        {
            detail.Each(a =>
            {
                if (a.TransType == (int)TransType.InternalSalesperson
                    || a.TransType == (int)TransType.MainSalesperson)
                {
                    var s = _commissionStructureDetail.Finds(filter: f => f.SysKey == a.SchemeDetailSK).Single();
                    if (s != null)
                        a.CurrentCommission = transaction.TransType == (int)TransType.ReferralSalesOrder ? a.CurrentCommission :
                                s.AssociatedPercent.Value;
                }
                else if (a.TransType == (int)TransType.ExternalSalesPerson)
                    a.CurrentCommission = 100;

                a.RecordStatus = (int)RecordStatus.Post;
                a.ReceivableStatus = (int)SalesViewType.Pending;
                a.PayableStatus = (int)SalesViewType.Pending;
                a.VendorSK = _person.Finds(filter: f => f.SysKey == a.SPSK).Single().VendorSK;
            });
        }

        private void UpdateLeadToClientStauts(
            string ptosk,
            int recordStatus,
            IEnumerable<PTOClientEntity> originClients,
            IEnumerable<PTOClientEntity> insertedClients = null,
            IEnumerable<PTOClientEntity> updatedClients = null,
            IEnumerable<PTOClientEntity> deletedClients = null)
        {
            var management = _leadManagementBuilder.GetLeadManagement(
                recordStatus: recordStatus,
                origin: originClients != null ? originClients.Select(x => x.ClientSK) : null,
                inserted: insertedClients != null ? insertedClients.Select(x => x.ClientSK) : new List<string>(),
                updated: updatedClients != null ? updatedClients.Select(x => x.ClientSK) : new List<string>(),
                deleted: deletedClients != null ? deletedClients.Select(x => x.ClientSK) : new List<string>(),
                ptosk: ptosk);

            management.DePromoteLeadCollection.Each(a =>
            {
                _person.Update(selector: s => new PersonEntity
                {
                    RecordStatus = (int)RecordStatus.Update,
                    TransType = (int)TransType.Lead,
                },
                filter: f => f.SysKey == a);
            });

            management.PromoteClientCollection.Each(a =>
            {
                var p = _person.Finds(filter: f => f.SysKey == a).Single();
                var c = _recordIndexer.Generate(TransType.Client, new PersonEntity());

                if (p.R1 != CommonConstant.DefaultIndexing.R1)
                {
                    _person.Update(selector: s => new PersonEntity
                    {
                        RecordStatus = (int)RecordStatus.Update,
                        TransType = (int)TransType.Client,
                    },
                   filter: f => f.SysKey == a);
                }
                else
                {
                    _person.Update(selector: s => new PersonEntity
                    {
                        RecordStatus = (int)RecordStatus.Update,
                        R1 = c.R1,
                        Prefix = c.Prefix,
                        Postfix = c.Postfix,
                        VoucherId = c.VoucherId,
                        TransType = (int)TransType.Client,
                    },
                    filter: f => f.SysKey == a);
                }
            });
        }

        private void SetNotification(PropertyTransactionEntity entity)
        {
            var identity = _person.FinanCityIdentity;
            var spsk = identity.SPSK;
         
            if (entity.RecordStatus == (int)RecordStatus.Submit)
            {
                _notification.Insert(new Entity.NotificationEntity
                {
                    SenderSK = spsk,
                    ReceiverSK = "1",
                    RefSK = entity.SysKey,
                    TransType = entity.TransType,
                    Description = $@"submit,{entity.R1}"
                });
            }
            else if (entity.RecordStatus == (int)RecordStatus.Save)
            {
                _notification.Insert(new Entity.NotificationEntity
                {
                    SenderSK = "1",
                    ReceiverSK = entity.MailingSPSK,
                    RefSK = entity.SysKey,
                    TransType = entity.TransType,
                    Description = $@"approved,{entity.R1}"
                });
            }
            else if (entity.RecordStatus == (int)RecordStatus.Reject
                || entity.RecordStatus == (int)RecordStatus.Void)
            {
                _notification.Insert(new Entity.NotificationEntity
                {
                    SenderSK = "1",
                    ReceiverSK = entity.MailingSPSK,
                    RefSK = entity.SysKey,
                    TransType = entity.TransType,
                    Description = $@"rejected,{entity.R1}"
                });
            }
        }

        #endregion
    }
}
