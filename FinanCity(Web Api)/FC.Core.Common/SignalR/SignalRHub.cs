namespace FC.Core.SignalR
{
    using System;
    using System.IO;
    using System.Web.Hosting;
    using System.Xml.Serialization;
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;
  
    [HubName("FCHub")]
    public class FCHub : Hub
    {
        private static IHubContext context
            = GlobalHost.ConnectionManager.GetHubContext<FCHub>();

        public static void NotifyOnChattingChange()
        {
            context.Clients.All.NotifyOnChattingChange();
        }

        public static void NotifyOnReferralSalesChange()
        {
            context.Clients.All.NotifyOnReferralSalesChange();
        }

        public static void NotifyOnSalesMortgageChange()
        {
            context.Clients.All.NotifyOnSalesMortgageChange();
        }

        public static void NotifyOnLeadFollowUp()
        {
            context.Clients.All.NotifyOnLeadFollowUp();
        }

        public static void NotifyOnAndriodVersionChange()
        {
            context.Clients.All.NotifyOnAndriodVersionChange(VersionFileWatcher.Version.Android);
        }

        public static void NotifyOnIOSVersionChange()
        {
            context.Clients.All.NotifyOnIOSVersionChange(VersionFileWatcher.Version.IOS);
        }
    }

    public static class VersionFileWatcher
    {
        public delegate void FileTextChange(object sender, FileSystemEventArgs args);
        public static event FileTextChange Changed;
        private static FileSystemWatcher fWatcher;
        private static string xmlFilePath = HostingEnvironment.MapPath("~/App_Data/mobile-version.xml");
      
        static VersionFileWatcher()
        {
            Initialize();
        }

        private static void Initialize()
        {
            fWatcher = new FileSystemWatcher();
            fWatcher.Path = Path.GetDirectoryName(xmlFilePath);
            fWatcher.Filter = Path.GetFileName(xmlFilePath);
            fWatcher.Changed += (s, e) =>
            {
                try
                {
                    fWatcher.EnableRaisingEvents = false;
                    Changed(s, e);
                }
                finally
                {
                    fWatcher.EnableRaisingEvents = true;
                }
            };

            fWatcher.EnableRaisingEvents = true;
            fWatcher.Error += (s, e) =>
            {
                fWatcher.Dispose();
                Initialize();
            };
        }

        public static Version Version
        {
            get
            {
                var serializer = new XmlSerializer(typeof(Version));
                using(var reader = new StreamReader(xmlFilePath))
                {
                    return (Version)serializer.Deserialize(reader);
                }
            }
        }
    }

    [Serializable()]
    [XmlRoot("version")]
    public class Version
    {
        [XmlElement("andriod")]
        public string Android { get; set; }

        [XmlElement("ios")]
        public string IOS { get; set; }
    }
}