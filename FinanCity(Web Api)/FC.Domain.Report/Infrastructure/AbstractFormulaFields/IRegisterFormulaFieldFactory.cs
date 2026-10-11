namespace FC.Domain.Report
{
    public interface IRegisterFormulaFieldFactory
    {
        AbstractFormulaField GetFactory(string key);

        void RegisterdFactory(string key, AbstractFormulaField abstractFormula);
    }
}
