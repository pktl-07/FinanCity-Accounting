namespace FC.Core.Data
{
    using Ninject;
    using Newtonsoft.Json;
    using FC.Core.Data.Infrastructure;
    using FC.Core.Data.Infrastructure.Implementation;
   
    /// <summary>
    /// All entity base class.
    ///<remarks>Customizing Db fields by not removing inheritance or adding properties in the interfaces. </remarks>
    /// </summary>
    public abstract class EntityBase : IEntity, ILoginUser, IKey, IRecordStatus, IAuto, ICreatedDate, ICreatedTime, IModifiedUser, IModifiedDate, IModifiedTime , IReferenIndexing
    {
        public EntityBase(string conn)
        {
            var dbrepo = DependencyContainer.Current.Get<IEssDataRepository>();
            _metaContext = DependencyContainer.Current.Get<IDbMetadata>();
            (_metaContext as DbMetadata).KEY = conn;
            (_metaContext as DbMetadata).ConnectionString = dbrepo.DbConnection[conn].ConnectionString;
            (_metaContext as DbMetadata).Provider = dbrepo.DbConnection[conn].Provider;
        }

        /// <summary>
        /// Primary key.
        /// </summary>
        [Column(Name = "SysKey")]
        public string SysKey { get; set; } 

        /// <summary>
        /// Login User
        /// </summary>
        [Column(Name = "LoginUser")]
        [JsonIgnore]
        public string LoginUser { get; set; } = DependencyContainer.Current.Get<IEssDataRepository>().LoginName ?? "phyokyaw.tl@gmail.com";

        /// <summary>
        /// Status of record.
        /// </summary>
        [Column(Name = "RecordStatus")]
        public int RecordStatus { get; set; }

        /// <summary>
        /// Db auto identity.
        /// </summary>
        [Column(Name = "Auto")]
        public int Auto { get; set; }

        /// <summary>
        /// Identity name of user who created record.
        /// </summary>
        //[Column]
        //public string CreatedUser { get; internal set; }

        /// <summary>
        /// Record created date.
        /// </summary>
        [Column(IsDate = true,Name = "CreatedDate")]
        public int? CreatedDate { get; set; }

        /// <summary>
        /// Record created time.
        /// </summary>
        [Column(Name = "CreatedTime")]
        public string CreatedTime { get; set; }

        /// <summary>
        /// Identity name of user who last updated record.
        /// </summary>
        [Column(Name = "ModifiedUser")]
        [JsonIgnore]
        public string ModifiedUser { get; set; }

        /// <summary>
        /// Last record modified date.
        /// </summary>
        [Column(IsDate = true)]
        public int? ModifiedDate { get; set; }

        /// <summary>
        /// Last record modified time
        /// </summary>
        [Column(Name = "ModifiedTime")]
        public string ModifiedTime { get; set; }

        [Ignore]
        private IDbMetadata _metaContext;

        /// <summary>
        /// Get entity metadata such as its connection string or provider.
        /// </summary>
        /// <returns></returns>
        [Ignore]
        public IDbMetadata GetMetadata()
        {
            return _metaContext;
        }

        /// <summary>
        /// Indexing Ref Id
        /// </summary>
        [Column(Name = "R1")]
        public virtual string R1 { get; set; } = "TBA";

        /// <summary>
        /// Indexing Prefix
        /// </summary>
        [Column(Name = "Prefix")]
        public virtual string Prefix { get; set; }
        
        /// <summary>
        /// Indexing Postfix
        /// </summary>
        [Column(Name = "Postfix")]
        public virtual string Postfix { get; set; }
        
        /// <summary>
        /// Indexing VoucherId
        /// </summary>
        [Column(Name = "VoucherId")]
        public virtual int VoucherId { get; set; }

        /// <summary>
        /// Table Name can be identified unless you use the TableAtrribute. 
        /// </summary>
        [Ignore]
        public void SetTableName(string table)
        {
            _metaContext.TableName = table;
        }
    }
}
