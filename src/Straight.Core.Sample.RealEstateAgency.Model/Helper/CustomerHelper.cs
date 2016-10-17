using System;
using System.Text.RegularExpressions;
using Straight.Core.Extensions.Guard;

namespace Straight.Core.Sample.RealEstateAgency.Model.Helper
{
    public static class CustomerHelper
    {
        private static readonly Regex EmailReagex =
            new Regex(
                @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

        private static readonly TimeSpan AgeRef;

        static CustomerHelper()
        {
            AgeRef = new TimeSpan(new DateTime(121, 1, 1).Ticks);
        }

        public static void CheckMandatoryCustomer(
            string firstName,
            string lastName,
            DateTime birthday,
            string email,
            Phone phone,
            Phone cellPhone)
        {
            firstName.CheckIfArgumentIsNullOrEmpty("FirstName");
            lastName.CheckIfArgumentIsNullOrEmpty("LastName");
            email.CheckIfArgumentIsNullOrEmpty("email");
            IsValidEmail(email);
            IsValidPhone(phone, cellPhone);
            IsValidBirthday(birthday);
        }

        private static void IsValidBirthday(DateTime birthday)
        {
            var diff = DateTime.UtcNow - birthday.ToUniversalTime();
            if (diff > AgeRef)
                throw new ArgumentException("Age cannot be greatest to 120 years.");
        }

        private static void IsValidPhone(Phone phone, Phone cellPhone)
        {
            if ((phone == null) && (cellPhone == null))
                throw new ArgumentException("Phone or/and CellPhone are mandatory.");
            if (phone != null)
                PhoneHelper.CheckMandatory(phone.Number, phone.CountryCode);
            if (cellPhone != null)
                PhoneHelper.CheckMandatory(cellPhone.Number, cellPhone.CountryCode);
        }

        private static void IsValidEmail(string email)
        {
            if (!EmailReagex.IsMatch(email))
                throw new ArgumentException("Email format is not valid.");
        }
    }
}