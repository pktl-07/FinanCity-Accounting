namespace FC.Core
{
    using FC.Core;
    using System.IO;

    public interface ICrystalReporter
    {
        void Export(PeasExportOption exportOption);

        Stream ExportToStream();

        void ExportToDisk(ref string path);

        void SetFormulaField(string id, object value);

        void SetDbParameterField(string id, object value);

        void SetDataSource(object dataSet);

        ReportConfigurator Configurator { get; }
    }
}
