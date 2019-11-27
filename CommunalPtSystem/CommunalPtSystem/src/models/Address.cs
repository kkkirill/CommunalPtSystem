using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunalPtSystem
{
    [Table("Addresses")]
    public class Address
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string City { get; set; }
        public string Street { get; set; }
        public int HouseNumber { get; set; }
        public int FlatNumber { get; set; }

        [ForeignKey(typeof(CommunalPaymentDocument))]
        public int CommunalPaymentDocumentId { get; set; }

        public override string ToString()
        {
            return $"{City}, {Street} {HouseNumber}-{FlatNumber}";
        }

        public Address DeepCopy()
        {
            Address address = new Address()
            {
                Id = Id,
                City = City,
                Street = Street,
                HouseNumber = HouseNumber,
                FlatNumber = FlatNumber,
                CommunalPaymentDocumentId = CommunalPaymentDocumentId
            };

            return address;
        }
    }
}
