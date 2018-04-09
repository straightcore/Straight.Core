using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using Straight.Core.Sample.RealEstateAgency.Model.Contracts.Extensions;
using NUnit.Framework;

namespace Straight.Core.Sample.RealEstateAgency.Model.Contracts.Tests
{
    [TestFixture]
    public class RealEstateAgencyModelConverterTests
    {
        [Test]
        public void Should_load_converter_without_exception_when_initiliaze_converter()
        {
            var converter = new RealEstateAgencyModelConverter();
        }

        [Test]
        public void Should_convert_phonedto_to_phone_when_usual_case()
        {
            var converter = new RealEstateAgencyModelConverter();

            var phoneDto = new PhoneDto()
                           {
                               CountryCode = "CountryCode",
                               Number = "Number"
                           };
            var phone = converter.ToModel<Phone>(phoneDto);
            Assert.That(phone.CountryCode, Is.EqualTo(phoneDto.CountryCode));
            Assert.That(phone.Number, Is.EqualTo(phoneDto.Number));
        }
    }
}
