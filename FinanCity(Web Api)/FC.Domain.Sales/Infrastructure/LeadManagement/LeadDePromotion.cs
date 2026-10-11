namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;

    public class LeadDePromotion : ILeadManagement
    {
        #region Fields

        private LeadManagementContext _context;
        public LeadManagementContext Context
        {
            get
            {
                return _context;
            }
        }

        #endregion

        #region Ctor

        public LeadDePromotion(LeadManagementContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        public IEnumerable<string> DePromoteLeadCollection
        {
            get
            {
                return 
                    _context.OriginClient
                    .Union(_context.UpdatedClient)
                    .Union(_context.InsertedClient)
                    .Except(
                        _context.HadTransactionClient
                        .Union(_context.AlwaysBeenClient));
            }
        }

        public IEnumerable<string> PromoteClientCollection
        {
            get
            {
                return null;
            }
        }
      
        #endregion
    }
}
