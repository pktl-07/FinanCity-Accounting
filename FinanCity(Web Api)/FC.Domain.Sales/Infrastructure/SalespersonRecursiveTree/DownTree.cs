namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Core;
    using FC.Domain.Sales.DTO;
    using Ninject;

    public class DownTree : TreeBuilderBase
    {
        #region Ctor

        public DownTree(IEnumerable<SalesPersonTireDTO> source) : base("", source) { }

        public DownTree(string spsk, IEnumerable<SalesPersonTireDTO> source) : base(spsk, source)
        {
        }

        #endregion

        #region Methods

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
                        IsActive = element.IsActive,
                        Items = children,
                        Depth = depth,
                        Rank = element.Rank,
                        ParentSK = element.ParentSK,
                        Desingation = element.Designation,
                        PhotoPath = $"{DI.Current.Get<IEssAppRepository>().DomainUrl}/Content/photo-uploads/{element.SPSK}.png"
                    });

                var preResult = items.RecursiveSelect(x => x.Items)
                    .Where(x => x.SPSK == base.ParentNode.SPSK).FirstOrDefault();

                if (preResult == null)
                    return new List<SalespersonRecursiveTree>();

                //finally adding searching node on top.
                return new List<SalespersonRecursiveTree>
                {
                    new SalespersonRecursiveTree
                    {
                        SPSK =  base.ParentNode.SPSK,
                        IsActive = preResult.IsActive,
                        Name =  base.ParentNode.Name,
                        Items = preResult.Items,
                        Rank = preResult.Rank,
                        ParentSK = preResult.ParentSK,
                        Desingation = base.ParentNode.Designation,
                        PhotoPath = $"{DI.Current.Get<IEssAppRepository>().DomainUrl}/Content/photo-uploads/{base.ParentNode.SPSK}.png"
                    },
                }; 
            }
        }

        #endregion
    }
}
