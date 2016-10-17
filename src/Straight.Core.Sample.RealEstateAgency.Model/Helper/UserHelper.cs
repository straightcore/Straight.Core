using Straight.Core.Extensions.Guard;

namespace Straight.Core.Sample.RealEstateAgency.Model.Helper
{
    public static class UserHelper
    {
        public static void CheckMandatoryUser(string firstname, string lastname, string username)
        {
            firstname.CheckIfArgumentIsNullOrEmpty("Creator.FirstName");
            lastname.CheckIfArgumentIsNullOrEmpty("Creator.LastName");
            username.CheckIfArgumentIsNullOrEmpty("Creator.UserName");
        }
    }
}