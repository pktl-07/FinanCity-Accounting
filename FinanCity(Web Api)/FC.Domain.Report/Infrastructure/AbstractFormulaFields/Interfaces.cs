namespace FC.Domain.Report
{
    public interface IYCStartFor
    {
        string YCStart { get; set; }
    }

    public interface IYCEndFor
    {
        string YCEnd { get; set; }
    }

    public interface IAccountingDateFor
    {
        int? AccountingDate { get; set; }
    }

    public interface ISaleDateFor
    {
        int? SalesDate { get; set; }
    }

    public interface IGLRef
    {
        string GLRef { get; set; }
    }

    public interface ISecondRef
    {
        string SecondRef { get; set; }
    }

    public interface IPaidDate
    {
        int? PaidDate { get; set; }
    }

    public interface ISalespersonFor
    {
        string SPSK { get; set; }
    }

    public interface IT1
    {
        string T1 { get; set; }
    }

    public interface IVendorFor
    {
        string VendorSK { get; set; }
    }

    public interface ITNC_PropertyTypeFor
    {
        string PropertyType { get; set; }
    }

    public interface ITNC_TransactionTypeFor
    {
        string TransactionType { get; set; }
    }

    public interface ICommissionReceivedStatusFor
    {
        int ReceivedStatus { get; set; }
    }

    public interface ICommissionPaidStatusFor
    {
        int PaidStatus { get; set; }
    }

    public interface IClientFor
    {
        string ClientSK { get; set; }
    }

    public interface IMaxTopFor
    {
        int Top { get; set; }
    }

    public interface ICashbookFor
    {
        string CBSK { get; set; }
    }

    public interface IRankFor
    {
        int Rank { get; set; }
    }

    public interface ISalespersonTypeFor
    {
        int SalespersonType { get; set; }
    }

    public interface ICobrokerAccuralChangeDateFor
    {
        int? CobrokerAccuralChangeDate { get; set; }
    }

    public interface ITaxFor
    {
        string TaxSK { get; set; }
    }

    public interface IAccountCodeFor
    {
        string AccountSK { get; set; }
    }

    public interface ITransTypeFor
    {
        int? TransType { get; set; }
    }

    public interface IJobSK
    {
        string JobSK { get; set; }
    }

    public interface IRecordStatusFor
    {
        int? RecordStatus { get; set; }
    } 

    public interface IFranchiseeFor
    {
        string FranchiseeSK { get; set; }
    }
}
