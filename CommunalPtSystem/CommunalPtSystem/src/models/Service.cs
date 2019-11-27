using SQLite;
using SQLiteNetExtensions.Attributes;

namespace CommunalPtSystem.src.models
{
    [Table("Services")]

    public class Service
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }


        public double Rate { get; set; }

        public double Amount { get; set; }

        [ForeignKey(typeof(ServiceType))]
        public int ServiceTypeId { get; set; }
        
        private ServiceType _serviceType;

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public ServiceType ServiceType
        {
            get { return _serviceType; }
            set
            {
                _serviceType = value;
                ServiceTypeId = value?.Id ?? 0;
            }
        }


        [ForeignKey(typeof(CommunalPaymentDocument))]
        public int CommunalPaymentDocumentId { get; set; }

        public override string ToString()
        {
            return $"ID: {Id} ServiceTypeID: {ServiceTypeId} Rate: {Rate} Amount: {Amount}";
        }

        public Service DeepCopy()
        {
            Service service = new Service()
            {
                Id = Id,
                Amount = Amount,
                Rate = Rate,
                CommunalPaymentDocumentId = CommunalPaymentDocumentId,
                ServiceTypeId = ServiceTypeId,
                ServiceType = ServiceType.DeepCopy()
            };
            return service;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
