using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Extensions
{
    public static class ToModelExtensions
    {
        public static Phone ToModel(this PhoneDto phoneDto)
        {
            return phoneDto == null ? null : new Phone {CountryCode = phoneDto.CountryCode, Number = phoneDto.Number};
        }

        public static Customer ToModel(this CustomerDto dto)
        {
            return new Customer(dto.Id)
            {
                Phone = dto.Phone.ToModel(),
                CellPhone = dto.CellPhone.ToModel(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Gender = dto.Gender.ToMappedEnum<Gender>(),
                Birthday = dto.Birthday,
                AdditionalAddress = dto.Address.AdditionalAddress,
                StreetNumber = dto.Address.StreetNumber,
                PostalCode = dto.Address.PostalCode,
                City = dto.Address.City,
                Street = dto.Address.Street
            };
        }
    }
}