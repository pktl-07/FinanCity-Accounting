namespace FC.Domain.Version.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Ninject;
    using FC.Core;
    using FC.Core.Data;
    using FC.Domain.Version.DAO;
    using FC.Domain.Version.Entity;
    using FC.Domain.Version.Infrastructure;
  
    public class Versions : BusinessLogic<VersionEntity, VersionsDAO>
    {
        #region Fileds

        IEssAppRepository _app;

        #endregion

        #region Ctor

        public Versions(IEssAppRepository app)
        {
            _app = app;
        }

        #endregion

        #region Method

        public VersionEntity GetLatestVersionInfo()
        {
            try
            {
                return DAO.GetLastVersionNo();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Upgrade()
        {
            try
            {
                if (DAO.HasAlready(_app.AppVersionInfo))
                    throw new Exception($"{_app.AppVersionInfo} is already updated.");

                var versionList = new List<IEssVersion>();
                var previousVersionNum = "";

                PoppulateVersionInfo(versionList, ref previousVersionNum);

                foreach (var result in versionList.Where(x => Between(x, previousVersionNum)).OrderBy(x => x, new VersionComparer()))
                    result.Excute();

                var appVersion = _app.AppVersionInfo;
                Insert(new VersionEntity
                {
                    VersionNo = appVersion
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Utilities

        private void PoppulateVersionInfo(List<IEssVersion> version, ref string previousVersionNum)
        {
            previousVersionNum = GetLatestVersionInfo().VersionNo;

            // Load Assembly Info
            var assemblyList = from t in Assembly.Load("FC.Domain.Version").GetTypes()
                               where t.IsClass && t.Namespace == "FC.Domain.Version.DAO"
                               select t;

            foreach (var v in assemblyList)
            {
                if (v.IsClass)
                {
                    var obj = DI.Current.Get(v);
                    if (obj is IEssVersion)
                    {
                        var versionName = obj.GetType().GetCustomAttribute<ESSVersionAttribute>().Name;
                        version.Add((IEssVersion)obj);
                    }
                }
            }
        }

        private class VersionComparer : IComparer<IEssVersion>
        {
            public int Compare(IEssVersion x, IEssVersion y)
            {
                var currentVersion = new Version(x.GetType().GetCustomAttribute<ESSVersionAttribute>().Name);
                var nextVersion = new Version(y.GetType().GetCustomAttribute<ESSVersionAttribute>().Name);

                return currentVersion.CompareTo(nextVersion);
            }
        }

        private bool Between(IEssVersion filter, string currentVersionNum)
        {
            var previousVersion = new Version(currentVersionNum);
            var daoVersion = new Version(filter.GetType().GetCustomAttribute<ESSVersionAttribute>().Name);
            var currentVersion = new Version(_app.AppVersionInfo);

            if (daoVersion.CompareTo(previousVersion) > 0)
            {
                if (daoVersion.CompareTo(currentVersion) <= 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
