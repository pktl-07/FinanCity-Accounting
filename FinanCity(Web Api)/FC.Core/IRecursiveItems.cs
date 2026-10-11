namespace FC.Core
{
    public interface IRecursiveItems : IRecursiveArray
    {
        string SysKey { get; set; }

        string ParentSK { get; set; }

        int Indent { get; set; }
    }
}
