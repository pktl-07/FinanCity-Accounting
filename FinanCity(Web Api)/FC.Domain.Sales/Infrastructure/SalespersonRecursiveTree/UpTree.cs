namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Core;
    using Ninject;
    using FC.Domain.Sales.DTO;
   
    public class UpTree : TreeBuilderBase
    {
        public UpTree(IEnumerable<SalesPersonTireDTO> source) : base("", source) { }

        public UpTree(string spsk,
            IEnumerable<SalesPersonTireDTO> source) : base(spsk, source)
        {
        }

        public override List<SalespersonRecursiveTree> Items
        {
            get
            {
                var items = base.TireContext.RecursiveJoin(element => element.ParentSK, element => element.SPSK,
                  (SalesPersonTireDTO element, int depth, IEnumerable<SalespersonRecursiveTree> children) => new SalespersonRecursiveTree
                  {
                      SPSK = element.SPSK,
                      Name = element.Name,
                      Items = children,
                      Depth = depth,
                      Rank = element.Rank,
                      IsActive = element.IsActive,
                      ParentSK = element.ParentSK,
                      Desingation = element.Designation,
                      PhotoPath = $"{DI.Current.Get<IEssAppRepository>().DomainUrl}/Content/photo-uploads/{element.SPSK}.png"
                  });

                var preResult = items.RecursiveSelect(x => x.Items)
                       .Where(x => x.SPSK == base.ParentNode.SPSK)
                       .FirstOrDefault();

                if (preResult == null)
                    return new List<SalespersonRecursiveTree>();

                //finally adding searching node on top.
                return new List<SalespersonRecursiveTree>
                {
                    new SalespersonRecursiveTree
                    {
                        SPSK =  base.ParentNode.SPSK,
                        Name =  base.ParentNode.Name,
                        Items = preResult.Items,
                        Rank = preResult.Rank,
                        ParentSK = preResult.ParentSK,
                        IsActive = preResult.IsActive,
                        Desingation = base.ParentNode.Designation,
                        PhotoPath = $"{DI.Current.Get<IEssAppRepository>().DomainUrl}/Content/photo-uploads/{base.ParentNode.SPSK}.png"
                    },
                };
            }
        }
    }
}
