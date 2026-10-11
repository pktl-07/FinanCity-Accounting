namespace FC.Domain.Accounts.DTO
{
    using FC.Domain.Accounts.Entity;

    public class BTableExtension : BTableEntity
    {
        public decimal Debit
        {
            get;
            set;
        }
        public decimal Credit
        {
            get;
            set;
        }
    }
}
