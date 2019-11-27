using CommunalPtSystem.src.utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace CommunalPtSystem.src.view_models
{
    class MainViewModel : INotifyCollectionChanged
    {

        public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };
        public void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
            }
        }
        public Filter filter { get; set; }

        private ObservableCollection<CommunalPaymentDocument> _paymentDocuments;

        public ObservableCollection<CommunalPaymentDocument> PaymentDocuments 
        { 
            get { return Filter(_paymentDocuments); } 
            set { _paymentDocuments = value; } 
        }

        private DBManager dbManager;
        public MainViewModel()
        {
            dbManager = DBManager.getInstance();

            filter = new Filter() { isEnabled = false };

            var documents = GetElements<CommunalPaymentDocument>();

            PaymentDocuments = new ObservableCollection<CommunalPaymentDocument>(documents);
        }

        public ObservableCollection<CommunalPaymentDocument> Filter(ObservableCollection<CommunalPaymentDocument> values)
        {
            if (!filter.isEnabled)
            {
                return values;
            }
            return new ObservableCollection<CommunalPaymentDocument>(filter.applyFilters(values.ToList()));
        }

        public T GetElementById<T>(int id, bool withChilds = true)
        {
            return dbManager.GetElementById<T>(id, withChilds);
        }

        public List<T> GetElements<T>(bool withChilds = true)
        {
            return dbManager.GetElements<T>(withChilds);
        } 

        public void UpdateOrCreateElement<T>(T element)
        {
            if (element.GetType() == typeof(CommunalPaymentDocument))
            {
                var newValue = (element as CommunalPaymentDocument);
                if (newValue.Id == 0)
                {
                    PaymentDocuments.Add(dbManager.CreateElement(element) as CommunalPaymentDocument);
                    return;
                }
                int i = PaymentDocuments.IndexOf(PaymentDocuments.FirstOrDefault(doc => doc.Id == newValue.Id));
                PaymentDocuments[i] = newValue;
            }

            dbManager.CreateOrUpdateElements(new List<T> { element });
        }

        public void UpdateOrCreateElements<T>(List<T> elements)
        {
            dbManager.CreateOrUpdateElements(elements);
        }

        public void DeleteElement<T>(T element)
        {
            dbManager.DeleteElement(element);
            if (element.GetType() == typeof(CommunalPaymentDocument))
            {
                _paymentDocuments.Remove((element as CommunalPaymentDocument));
            }
        }
    }
}
