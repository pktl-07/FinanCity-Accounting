namespace FC.Domain.Sales.Infrastructure
{
    using FC.Core;
    using FC.Domain.Sales.DTO;
    using Ninject;
    using System.Collections.Generic;

    public class TreeBuilder
    {
        private TreeBuilderBase tree;
     
        public TreeBuilder() { }

        public TreeBuilder(string spsk,
            bool? downTree,
            IEnumerable<SalesPersonTireDTO> source)
        {
            if (downTree.HasValue)
            {
                if (downTree.Value == true)
                {
                    DI.Current.Rebind<DownTree>()
                        .To<DownTree>()
                        .WithConstructorArgument("spsk", spsk)
                        .WithConstructorArgument("source", source);

                    tree = DI.Current.Get<DownTree>();
                }
                else if (downTree.Value == false)
                {
                    DI.Current.Rebind<UpTree>()
                        .To<UpTree>()
                        .WithConstructorArgument("spsk", spsk)
                        .WithConstructorArgument("source", source);

                    tree = DI.Current.Get<UpTree>();
                }
            }
            else
            {
                DI.Current.Rebind<FullTree>()
                    .To<FullTree>()
                    .WithConstructorArgument("spsk", spsk)
                    .WithConstructorArgument("source", source);

                tree = DI.Current.Get<FullTree>();
            }
        }

        public TreeBuilderBase BuildTree()
        {
            return tree;
        }
    }
}
