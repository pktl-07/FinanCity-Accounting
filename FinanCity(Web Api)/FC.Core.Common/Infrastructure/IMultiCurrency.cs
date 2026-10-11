namespace FC.Core
{
    public interface IMultiCurrency
    {
        string CURRSK { get; set; }

        decimal CURRATE { get; set; }

        string CURROP { get; set; }
    }
}
