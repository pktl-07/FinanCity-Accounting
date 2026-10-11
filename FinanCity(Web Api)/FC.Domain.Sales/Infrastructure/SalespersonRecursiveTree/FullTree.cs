namespace FC.Domain.Sales.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using FC.Core;
    using FC.Domain.Sales.DTO;
    using Ninject;

    public class FullTree : TreeBuilderBase
    {
        public FullTree(IEnumerable<SalesPersonTireDTO> source)
            : base("", source) 
        { 
        }

        public FullTree(string spsk, IEnumerable<SalesPersonTireDTO> source)
            : base(spsk, source)
        {
        }

        public override List<SalespersonRecursiveTree> Items
        {
            get
            {
                var items = base.TireContext.RecursiveJoin(element => element.SPSK,
                    element => element.ParentSK,
                    (SalesPersonTireDTO element, int depth, IEnumerable<SalespersonRecursiveTree> children) => new SalespersonRecursiveTree
                    {
                        SPSK = element.SPSK,
                        Name = element.Name,
                        Items = children,
                        Depth = depth,
                        IsActive = element.IsActive,
                        ParentSK = element.ParentSK,
                        Desingation = element.Designation,
                        PhotoPath = $"{DI.Current.Get<IEssAppRepository>().DomainUrl}/Content/photo-uploads/{element.SPSK}.png"
                    });

                return items.ToList();
            }
        }
    }
}
