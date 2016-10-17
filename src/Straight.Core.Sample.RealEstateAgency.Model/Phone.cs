using System;

namespace Straight.Core.Sample.RealEstateAgency.Model
{
    public sealed class Phone : ICloneable
    {
        public string Number { get; set; }

        public string CountryCode { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}