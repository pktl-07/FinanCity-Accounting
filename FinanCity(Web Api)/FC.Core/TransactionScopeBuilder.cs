namespace FC.Core
{
    using System.Transactions;

    public class TransactionScopeBuilder
    {
        public static TransactionScope CreateReadCommitted()
        {
            var options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
            };

            return new TransactionScope(TransactionScopeOption.Required, options);
        }
    }
}
