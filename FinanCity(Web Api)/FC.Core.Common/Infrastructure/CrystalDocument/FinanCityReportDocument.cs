namespace FC.Core.Infrastructure.CrystalDocument
{
    using System;
    using System.Web;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;
    using System.IO;
    using CrystalDecisions.Shared;
    using CrystalDecisions.CrystalReports.Engine;
    using FC.Core;

    public class FinanCityReportDocument : ReportDocument, ICrystalReporter
    {
        #region Fields

        private IEssAppRepository _appService;
        private IEssDataRepository _dataService;
        private bool _needTableLogOn = true;

        public ReportConfigurator Configurator
        {
            get;
            set;
        }

        public FinanCityReportDocument(IEssAppRepository appService,
            IEssDataRepository dataService)
        {
            _appService = appService;
            _dataService = dataService;

            this.Configurator = new ReportConfigurator { Attachment = new Attachment { } };
            this.Configurator.OnIntializeRptFileName += (rptFileName) =>
            {
                //Need to Intialize the Crystal Rpt File First
                base.FileName = GetFileExist(rptFileName);
                // Create Default Formula Field
                base.DataDefinition.FormulaFields["CompanyName"].Text = ("'" + _appService.CompanyName + "'");
                //base.DataDefinition.FormulaFields["Pic"].Text = ('"' + GetPicPath() + '"');
                base.DataDefinition.FormulaFields["PrintDate"].Text = ('"' + "Printed:" + GeneralUtility.ConvertDisplayDateFormat(GeneralUtility.ServerDate) + " " + GeneralUtility.ConvertDisplayDateFormat(GeneralUtility.ServerTime) + '"');
                base.DataDefinition.FormulaFields["CURRCODE"].Text = ('"' + _appService.CURRCODE + '"');
            };
        }

        #endregion

        #region Method

        //private string GetPicPath()
        //{
        //    if (this._pictureSize == Size.Medium)
        //    {
        //        if (File.Exists(PeasConfigManager.GeneralConfiguration.ResourceDir + "report_M.PNG"))
        //            return PeasConfigManager.GeneralConfiguration.ResourceDir + "report_M.PNG";

        //        return PeasConfigManager.GeneralConfiguration.ResourceDir + "report_M.TIF";
        //    }
        //    else
        //    {
        //        if (File.Exists(PeasConfigManager.GeneralConfiguration.ResourceDir + "report_S.PNG"))
        //            return PeasConfigManager.GeneralConfiguration.ResourceDir + "report_S.PNG";

        //        return PeasConfigManager.GeneralConfiguration.ResourceDir + "report_S.TIF";
        //    }
        //}

        public override void SetDataSource(object dataSet)
        {
            _needTableLogOn = false;

            base.SetDataSource(dataSet);
        }

        public void Export(PeasExportOption exportOption)
        {
            Validate();

            if (_needTableLogOn)
                ApplyTableLogOn(this.Configurator.Connection);

            FilterSection(exportOption);

            if (exportOption == PeasExportOption.Pdf)
                ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, true, AttachmentFormat);
            else if (exportOption == PeasExportOption.Excel)
                ExportToHttpResponse(ExportFormatType.ExcelRecord, System.Web.HttpContext.Current.Response, true, AttachmentFormat);
        }

        public void SetFormulaField(string id, object value)
        {
            if (value is string)
                base.DataDefinition.FormulaFields[id].Text = ($"'{((string)value).Replace("'", "''")}'");
            else
                base.DataDefinition.FormulaFields[id].Text = (string)value;
        }

        public void SetDbParameterField(string id, object value)
        {
            base.SetParameterValue(id, value);
        }

        public Stream ExportToStream()
        {
            try
            {
                return this.ExportToStream(ExportFormatType.PortableDocFormat);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ExportToDisk(ref string path)
        {
            try
            {
                path = HttpContext.Current.Server.MapPath($"~/Temp/{GeneralUtility.Key}.pdf");
                ExportToDisk(ExportFormatType.PortableDocFormat, path);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void FilterSection(PeasExportOption exportOption)
        {
            for (int index = 0; index < this.ReportDefinition.Sections.Count; index++)
            {
                if (ReportDefinition.Sections[index].Name.StartsWith(exportOption.ToDescription()))
                    ReportDefinition.Sections[index].SectionFormat.EnableSuppress = true;
            }

            for (int index = 0; index < this.ReportDefinition.ReportObjects.Count; index++)
            {
                if (this.ReportDefinition.ReportObjects[index].Name.StartsWith(exportOption.ToDescription()))
                    ReportDefinition.ReportObjects[index].ObjectFormat.EnableSuppress = true;
            }
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Configurator.RptFileName))
                throw new Exception("Crystal File Name should not be empty");

            if (!Configurator.RptFileName.EndsWith(".rpt"))
                throw new Exception("Crystal File Name should have .rpt extension");

            if (string.IsNullOrWhiteSpace(this.Configurator.Attachment.Title))
                throw new Exception("Report Title should not be empty");
        }

        private void ApplyTableLogOn(string daoConnection = CommonConstant.SalesConnection)
        {
            var constr = _dataService.DbConnection[daoConnection].ConnectionString;
            var tableInfo = new TableLogOnInfo();
            var crystalTables = base.Database.Tables;
            var peasConnection = new SqlConnectionStringBuilder(constr);

            foreach (Table crystal in crystalTables)
            {
                tableInfo.ConnectionInfo = new ConnectionInfo
                {
                    DatabaseName = peasConnection.InitialCatalog,
                    UserID = peasConnection.UserID,
                    Password = peasConnection.Password,
                    ServerName = peasConnection.DataSource,
                    IntegratedSecurity = false
                };
                crystal.ApplyLogOnInfo(tableInfo);
            }
        }

        private string AttachmentFormat
        {
            get
            {
                var title = Configurator.Attachment.Title + "-" + Configurator.Attachment.ReportDate;

                var replaceStr = Regex.Replace(title, @"\s+", "");

                return Regex.Replace(replaceStr, @"\/", ".");
            }
        }

        private string GetFileExist(string rptFileName)
        {
            var template = HttpContext.Current.Server.MapPath($"{_appService.ReportTemplateDirectory}/{rptFileName}");
            var shared = HttpContext.Current.Server.MapPath($"{_appService.SharedReportTemplateDirectory}/{rptFileName}");

            if (File.Exists(template))
                return template;

            return shared;
        }

        #endregion
    }
}
