namespace FC.Core
{
    using System.Collections.Generic;

    public interface IEssDataRepository : IEssRepository
    {
        //<summary>
        //Get full connection string information eg.Data Source = localhost\SQL;Initial Catalog = MyDb; User ID = sa; Password=********
        //</summary>
        //<remarks>
        //The change will never be effected after one time cofiguration made.
        //</remarks>
        Dictionary<string, DataConnection> DbConnection { get; }

        /// <summary>
        /// To set login user name as a db default audit field.
        /// </summary>
        string LoginName { get; set; }
    }
}
