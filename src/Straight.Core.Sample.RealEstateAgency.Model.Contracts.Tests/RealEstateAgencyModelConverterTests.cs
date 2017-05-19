using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using Straight.Core.Sample.RealEstateAgency.Model.Contracts.Extensions;
using Xunit;

namespace Straight.Core.Sample.RealEstateAgency.Model.Contracts.Tests
{
    public class RealEstateAgencyModelConverterTests
    {
        [Fact]
        public void Should_load_converter_without_exception_when_initiliaze_converter()
        {
            var converter = new RealEstateAgencyModelConverter();
        }

        [Fact]
        public void Should_convert_phonedto_to_phone_when_usual_case()
        {
            var converter = new RealEstateAgencyModelConverter();

            var phoneDto = new PhoneDto()
                           {
                               CountryCode = "CountryCode",
                               Number = "Number"
                           };
            var phone = converter.ToModel<Phone>(phoneDto);
            Assert.Equal(phone.CountryCode, phoneDto.CountryCode);
            Assert.Equal(phone.Number, phoneDto.Number);
        }
    }
}
