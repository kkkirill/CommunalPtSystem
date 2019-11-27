using SQLite;

namespace CommunalPtSystem
{
    [Table("ServiceTypes")]
    public class ServiceType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Units { get; set; }

        public bool Equals(ServiceType other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Id == other.Id;
        }

        public sealed override bool Equals(object obj)
        {
            var otherMyItem = obj as ServiceType;
            if (ReferenceEquals(otherMyItem, null)) return false;
            return otherMyItem.Equals(this);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(ServiceType myItem1, ServiceType myItem2)
        {
            return Equals(myItem1, myItem2);
        }

        public static bool operator !=(ServiceType myItem1, ServiceType myItem2)
        {
            return !(myItem1 == myItem2);
        }

        public override string ToString()
        {
            return $"ID: {Id} Name: {Name} Units: {Units}";
        }

        public ServiceType DeepCopy()
        {
            ServiceType serviceType = new ServiceType()
            {
                Id = Id,
                Name = Name,
                Units = Units
            };
            return serviceType;
        }

    }
}
