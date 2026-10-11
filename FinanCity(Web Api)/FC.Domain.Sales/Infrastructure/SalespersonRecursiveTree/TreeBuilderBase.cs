namespace FC.Domain.Sales.Infrastructure
{
    using System.Linq;
    using System.Collections.Generic;
    using FC.Domain.Sales.DTO;
  
    public abstract class TreeBuilderBase
    {
       public TreeBuilderBase(string spsk,
            IEnumerable<SalesPersonTireDTO> source)
        {
            _parentNode = source.Where(x => x.SPSK == spsk).SingleOrDefault()
                ?? new SalesPersonTireDTO(); // parent node

            // Recursive Join does not allow Parent is null cos of filter parent SK
            _tireContext = source.Select(x => new SalesPersonTireDTO
            {
                SPSK = x.SPSK,
                Name = x.Name,
                Rank = x.Rank,
                GroupNum = x.GroupNum,
                Designation = x.Designation,
                Level = x.Level,
                IsActive = x.IsActive,
                ParentSK = x.ParentSK ?? "",
            })
            .Where(x => x.GroupNum == _parentNode.GroupNum)
            .ToList();
        }

        private List<SalesPersonTireDTO> _tireContext;
        public List<SalesPersonTireDTO> TireContext
        {
            get
            {
                return _tireContext;
            }
        }

        private SalesPersonTireDTO _parentNode;
        public SalesPersonTireDTO ParentNode
        {
            get
            {
                return _parentNode;
            }
        }

        public abstract List<SalespersonRecursiveTree> Items { get; }
    }
}
