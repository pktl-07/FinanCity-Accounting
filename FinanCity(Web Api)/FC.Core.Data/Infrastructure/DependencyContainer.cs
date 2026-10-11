namespace FC.Core.Data.Infrastructure
{
    using Ninject;
    using FC.Core.Data.Infrastructure.Implementation;
    using FC.Core.Data.SQL;
    using FC.Core.Data.SQL.Fluent;

    public class DependencyContainer : FC.Core.DI
    {
        public override void RegisterService(IKernel kernal)
        {
            kernal.Bind<IColumnMetadata>().To<ColumnMetadata>();
            kernal.Bind<IDbMetadata>().To<DbMetadata>();
            kernal.Bind<Select>().ToConstructor((x) => new Select(QueryType.Select));
            kernal.Bind<Insert>().ToConstructor((x) => new Insert(QueryType.Insert));
            kernal.Bind<Update>().ToConstructor((x) => new Update(QueryType.Update));
            kernal.Bind<Delete>().ToConstructor((x) => new Delete(QueryType.Delete));
            kernal.Bind<ISelect>().To<SelectBuilder>();
            kernal.Bind<IInsert>().To<InsertBuilder>();
            kernal.Bind<IUpdate>().To<UpdateBuilder>();
            kernal.Bind<IDelete>().To<DeleteBuilder>();
        }
    }
}
