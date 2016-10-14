using System;

namespace Straight.Core.RealEstateAgency.Model
{
    public sealed class Customer : IIdentifiable, ICloneable
    {
        public Customer()
            : this(Guid.NewGuid())
        {
        }

        public Customer(Guid id)
        {
            Id = id;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public Phone Phone { get; set; }
        public Phone CellPhone { get; set; }

        public string AdditionalAddress { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        public object Clone()
        {
            var clone = MemberwiseClone() as Customer;
            clone.Phone = Phone?.Clone() as Phone;
            clone.CellPhone = CellPhone?.Clone() as Phone;
            return clone;
        }

        public Guid Id { get; }
    }

    public enum Gender
    {
        Mr,
        Mrs,
        Miss
    }
}