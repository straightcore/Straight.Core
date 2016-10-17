using Straight.Core.Extensions.Guard;

namespace Straight.Core.Sample.RealEstateAgency.Model.Helper
{
    public static class AddressHelper
    {
        public static void CheckMandatory(string street, string city, string postalCode)
        {
            street.CheckIfArgumentIsNullOrEmpty("street");
            city.CheckIfArgumentIsNullOrEmpty("city");
            postalCode.CheckIfArgumentIsNullOrEmpty("postalCode");
        }
    }
}