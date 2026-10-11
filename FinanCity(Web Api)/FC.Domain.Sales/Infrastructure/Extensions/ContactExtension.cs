namespace FC.Domain.Sales.Entity
{
    using System.Linq;

    public static class ContactExtension
    {
        public static string StraightAddress(this ContactEntity entity)
        {
            if (entity == null)
                return null;

            var straightAddress = string.IsNullOrWhiteSpace(entity.BuildingHouseNo) ? "" : entity.BuildingHouseNo + " ";

            straightAddress += string.IsNullOrWhiteSpace(entity.Address1) ? "" : entity.Address1 + ", ";

            straightAddress += string.IsNullOrWhiteSpace(entity.Address2) ? "" : entity.Address2 + ", ";

            straightAddress += string.IsNullOrWhiteSpace(entity.FloorUnit) ? "" : entity.FloorUnit.Contains('#') ? entity.FloorUnit : "#" + entity.FloorUnit + ", ";

            straightAddress += entity.Country + " ";

            straightAddress += string.IsNullOrWhiteSpace(entity.PostalCode) ? string.Empty : entity.PostalCode;

            return straightAddress;
        }

        public static string SetFullAddress(this ContactEntity entity)
        {
            if (entity == null)
                return null;

            var fullAddress = "<p>";
            fullAddress += string.IsNullOrWhiteSpace(entity.BuildingHouseNo) ? "" : entity.BuildingHouseNo + " ";
            fullAddress += string.IsNullOrWhiteSpace(entity.Address1) ? "" : entity.Address1 + "<br>";
            fullAddress += string.IsNullOrWhiteSpace(entity.Address2) ? "" : entity.Address2 + "<br>";
            fullAddress += string.IsNullOrWhiteSpace(entity.FloorUnit) ? "" : "#" + entity.FloorUnit + "<br>";
            fullAddress += entity.Country + " ";
            fullAddress += string.IsNullOrWhiteSpace(entity.PostalCode) ? "" : entity.PostalCode;
            fullAddress += "</p>";

            return fullAddress;
        }
    }
}
