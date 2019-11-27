using CommunalPtSystem.src.models;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CommunalPtSystem
{
    [Table("CommunalPaymentDocuments")]
    public class CommunalPaymentDocument
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [OneToMany]
        public List<Service> Services { get; set; }

        [Ignore]
        public DateTime Date 
        { 
            get 
            {
                return DateInt == 0
                  ? DateTime.Today
                  : DateTime.ParseExact(DateInt.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
            }
            set { DateInt = int.Parse(value.ToString("yyyyMMdd")); } 
        }
        public int DateInt { get; set; }
        public double TotalSum { get; set; }

        [ForeignKey(typeof(Address))]
        public int AddressId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Address Address { get; set; }

        public override string ToString()
        {
            return $"ID: {Id} DateTime: {Date.ToString("dd.MM.yyyy")} AddressID: {Address.Id} Total Sum: {TotalSum}";
        }

        public CommunalPaymentDocument DeepCopy()
        {
            CommunalPaymentDocument document = new CommunalPaymentDocument()
            {
                Id = Id,
                AddressId = AddressId,
                Address = Address.DeepCopy(),
                Date = Date,
                DateInt = DateInt,
                Services = Services.ConvertAll(v => v.DeepCopy()),
                TotalSum = TotalSum
            };
            return document;
        }
    }
}
