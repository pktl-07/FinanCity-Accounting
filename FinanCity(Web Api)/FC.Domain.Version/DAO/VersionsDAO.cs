namespace FC.Domain.Version.DAO
{
    using System;
    using System.Linq;
    using FC.Core.Data;
    using FC.Domain.Version.Entity;
   
    public class VersionsDAO : DAO<VersionEntity>
    {
        public VersionEntity GetLastVersionNo()
        {
            try
            {
                var query = $@"SELECT * FROM {Table<VersionEntity>()} 
                    WHERE {With<VersionEntity>(x => x.Auto)} =
                    (SELECT MAX({With<VersionEntity>(x => x.Auto)}) FROM {Table<VersionEntity>()})";

                return DbExcutor<VersionEntity>.ExcuteReader(query).Single();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool HasAlready(string versionNo)
        {
            try
            {
                var query = $@"SELECT * FROM {Table<VersionEntity>()}
                     WHERE {With<VersionEntity>(x => x.VersionNo)}='{versionNo}'";

                var v = DbExcutor<VersionEntity>.ExcuteReader(query);

                if (v.Count() > 0)
                    return true;

                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
