namespace FC.Domain.Sales.Infrastructure
{
    using System.Collections.Generic;

    public class SalespersonRecursiveTree
    {
        public string SPSK { get; set; }

        public string ParentSK { get; set; }

        public string Name { get; set; }

        public int Depth { get; set; }

        public string Desingation { get; set; }

        public string PhotoPath { get; set; }

        public bool IsActive { get; set; }

        public int Rank { get; set; }

        public IEnumerable<SalespersonRecursiveTree> Items { get; set; }
    }
}
