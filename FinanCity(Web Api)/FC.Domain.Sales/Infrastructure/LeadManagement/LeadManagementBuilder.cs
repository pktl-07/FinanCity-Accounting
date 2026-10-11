namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using Ninject;
    using FC.Core;
    using FC.Domain.Sales.Business;

    public class LeadManagementBuilder
    {
        #region Fields

        private Lead _lead;
        private PTOClient _clientTransaction;
        private IEnumerable<string> _origin;
        private IEnumerable<string> _inserted;
        private IEnumerable<string> _updated;
        private IEnumerable<string> _deleted;
        private string _ptosk;
     
        #endregion

        #region Ctor

        public LeadManagementBuilder(Lead lead,
          PTOClient clientTransaction)
        {
            _lead = lead;
            _clientTransaction = clientTransaction;
        }

        #endregion

        #region Methods

        public ILeadManagement GetLeadManagement(
            IEnumerable<string> origin,
            IEnumerable<string> inserted,
            IEnumerable<string> updated,
            IEnumerable<string> deleted,
            string ptosk,
            int recordStatus)
        {
            _origin = origin ?? new string[] { };
            _inserted = inserted ?? new string[] { };
            _updated = updated ?? new string[] { };
            _deleted = deleted ?? new string[] { };
            _ptosk = ptosk;
          
            if (recordStatus != (int)RecordStatus.Void
                && recordStatus != (int)RecordStatus.Reject)
            {
                DI.Current.Rebind<ILeadManagement>()
                    .To<LeadPromotion>()
                    .WithConstructorArgument("context", BuildContext());

                return DI.Current.Get<ILeadManagement>();
            }

            DI.Current.Rebind<ILeadManagement>()
                .To<LeadDePromotion>()
                .WithConstructorArgument("context", BuildContext());

            return DI.Current.Get<ILeadManagement>();
        }


        #endregion

        #region Utilities

        private LeadManagementContext BuildContext()
        {
            var always = BuildAlwaysClient();
            return new LeadManagementContext
            {
                OriginClient = _origin,
                InsertedClient = _inserted,
                UpdatedClient = _updated,
                DeletedClient = _deleted,
                AlwaysBeenClient = always,
                HadTransactionClient = BuildHadTransaction(always)
            };
        }

        private string[] BuildAlwaysClient()
        {
            var result = new List<string>();

            _origin
                .Union(_inserted)
                .Union(_updated)
                .Union(_deleted)
                .GroupBy(x => x)
                .Where(c => !string.IsNullOrEmpty(c.Key))
                .Each(c =>
                {
                    var k = c.Key;
                    if (_lead.Finds(filter: f => f.SysKey == k).Count() == 0)
                        result.Add(k);
                });

            return result.ToArray();
        }

        private string[] BuildHadTransaction(string[] always)
        {
            var result = new List<string>();
            if (_ptosk != null)
            {
                _origin
                    .Union(_inserted)
                    .Union(_updated)
                    .Union(_deleted)
                    .Except(always)
                    .GroupBy(x => x)
                    .Where(c => !string.IsNullOrEmpty(c.Key))
                    .Each(c =>
                    {
                        var k = c.Key;
                        var p = _ptosk;
                        var m = _clientTransaction.Finds(filter: f =>
                        f.RecordStatus != (int)RecordStatus.Void
                        && f.RecordStatus != (int)RecordStatus.Reject
                        && f.PTOSK != p
                        && f.ClientSK == k);

                        if (m.Count() > 0)
                        {
                            result.Add(k);
                        }
                    });
            }

            return result.ToArray();
        }

        #endregion
    }
}
