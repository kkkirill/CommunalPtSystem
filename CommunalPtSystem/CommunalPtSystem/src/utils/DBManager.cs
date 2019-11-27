using SQLiteDBEngine = System.Data.SQLite;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using SQLiteNetExtensions.Extensions;
using CommunalPtSystem.src.models;
using System.Linq;

namespace CommunalPtSystem.src.utils
{
    class DBManager
    {
        private static DBManager dBManager;
        private static readonly object syncRoot = new object();

        private SQLiteConnection db;

        private Dictionary<Type, int> typeToInt;

        public DBManager()
        {
            if (!File.Exists(App.DatabasePath)) {
                SQLiteDBEngine.SQLiteConnection.CreateFile(App.DatabasePath);
            }

            typeToInt = new Dictionary<Type, int>
            {
                {typeof(Address), 0},
                {typeof(CommunalPaymentDocument), 1},
                {typeof(ServiceType), 2},
                {typeof(Service), 3},
            };

            db = new SQLiteConnection(App.DatabasePath);

            db.CreateTable<ServiceType>();
            db.CreateTable<Service>();
            db.CreateTable<Address>();
            db.CreateTable<CommunalPaymentDocument>();

            db.DeleteAll<CommunalPaymentDocument>();
            db.DeleteAll<ServiceType>();
            db.DeleteAll<Service>();
            db.DeleteAll<Address>();

            //------------------ Creating temp data ------------------

            initValues();
        }

        private void initValues()
        {
            List<DateTime> times = new List<DateTime>()
            {
                new DateTime(2019, 11, 20),
                new DateTime(2019, 11, 10),
                new DateTime(2019, 5, 2),
                new DateTime(2019, 1, 15),
                new DateTime(2019, 5, 15),
            };
            List<Address> addresses = new List<Address>()
            {
                new Address()
                {
                    City = "Minsk",
                    Street = "Dzerzhinskiy prospect",
                    HouseNumber = 123,
                    FlatNumber = 210
                },
                new Address()
                {
                    City = "Brest",
                    Street = "Zubacheva street",
                    HouseNumber = 15,
                    FlatNumber = 120
                },
                new Address()
                {
                    City = "Pinsk",
                    Street = "Olkhovskyh street",
                    HouseNumber = 10,
                    FlatNumber = 22
                },
                new Address()
                {
                    City = "Minsk",
                    Street = "Moscowskaya street",
                    HouseNumber = 18,
                    FlatNumber = 149
                },
                new Address()
                {
                    City = "Min",
                    Street = "Bobruiskaya street",
                    HouseNumber = 123,
                    FlatNumber = 22
                },
            };
            List<CommunalPaymentDocument> paymentDocuments = new List<CommunalPaymentDocument>()
            {
                new CommunalPaymentDocument()
                {
                    Date = times[0],
                    TotalSum = 400
                },
                new CommunalPaymentDocument()
                {
                    Date = times[1],
                    TotalSum = 120
                },
                new CommunalPaymentDocument()
                {
                    Date = times[2],
                    TotalSum = 120
                },
                new CommunalPaymentDocument()
                {
                    Date = times[3],
                    TotalSum = 120
                },
                new CommunalPaymentDocument()
                {
                    Date = times[4],
                    TotalSum = 120
                }
            };
            List<ServiceType> serviceTypes = new List<ServiceType>
            {
                new ServiceType()
                {
                    Id = 1,
                    Name = "Maintenance",
                    Units = "m2"
                },
                new ServiceType()
                {
                    Id = 2,
                    Name = "Major Overhaul",
                    Units = "m2"
                },
                new ServiceType()
                {
                    Id = 3,
                    Name = "Hot Water Supply",
                    Units = "Gcal"
                },
                new ServiceType()
                {
                    Id = 4,
                    Name = "Heating",
                    Units = "Gcal"
                },
                new ServiceType()
                {
                    Id = 5,
                    Name = "Municipal Waste Management",
                    Units = "m3"
                },
            };
            db.InsertAll(serviceTypes);
            List<List<Service>> services = new List<List<Service>>()
            {
                new List<Service>()
                {
                    new Service()
                    {
                        Amount = 120,
                        Rate = 0.1287,
                    },
                    new Service()
                    {
                        Amount = 50.1,
                        Rate = 0.151,
                    },
                    new Service()
                    {
                        Amount = 19.25,
                        Rate = 18.531,
                    }
                },
                new List<Service>()
                {
                    new Service()
                    {
                        Amount = 100,
                        Rate = 0.0887,
                    },
                    new Service()
                    {
                        Amount = 47,
                        Rate = 0.112,
                    },
                    new Service()
                    {
                        Amount = 13.5,
                        Rate = 26.531,
                    }
                },
                new List<Service>()
                {
                    new Service()
                    {
                        Amount = 108,
                        Rate = 0.0587,
                    },
                    new Service()
                    {
                        Amount = 62,
                        Rate = 0.099,
                    },
                    new Service()
                    {
                        Amount = 19.9,
                        Rate = 22.181,
                    }
                },
                new List<Service>()
                {
                    new Service()
                    {
                        Amount = 113,
                        Rate = 0.131,
                    },
                    new Service()
                    {
                        Amount = 70.7,
                        Rate = 0.129,
                    },
                    new Service()
                    {
                        Amount = 20,
                        Rate = 19,
                    }
                },
                new List<Service>()
                {
                    new Service()
                    {
                        Amount = 60,
                        Rate = 0.131,
                    },
                    new Service()
                    {
                        Amount = 100.7,
                        Rate = 0.199,
                    },
                    new Service()
                    {
                        Amount = 30,
                        Rate = 13,
                    }
                }
            };
            
            services.ForEach(e => {
                int i = 0;
                e.ForEach(e =>
                    {
                        e.ServiceTypeId = serviceTypes[i].Id;
                        e.ServiceType = serviceTypes[i++];
                        db.UpdateWithChildren(e);
                    }); 
            });
            db.InsertAll(addresses);
            db.InsertAll(paymentDocuments);
            services.ForEach(e => db.InsertAll(e));
            int i = 0;
            paymentDocuments.ForEach(e =>
            {
                e.Address = addresses[i];
                e.Services = services[i++];
                db.UpdateWithChildren(e);
            });
        }

        public static DBManager getInstance()
        {
            lock (syncRoot)
            {
                if (dBManager == null)
                {
                    dBManager = new DBManager();
                }
                return dBManager;
            }
        }

        public T CreateElement<T>(T e)
        {
            db.InsertOrReplaceWithChildren(e, recursive: true);
            return e;
        }

        // Bulk Create or Update elements

        public void CreateOrUpdateElements<T>(List<T> elements)
        {
            db.InsertOrReplaceAllWithChildren(elements);
        }

        // Get multiple elements

        public List<T> GetElements<T>(bool withChilds = false)
        {
            List<T> values = null;
            switch (typeToInt[typeof(T)])
            {
                case 0:
                    //addresses
                    values = (withChilds
                                ? db.GetAllWithChildren<Address>()
                                : db.Table<Address>().ToList())
                                .Select(p => (T)Convert.ChangeType(p, typeof(T)))
                                .ToList();
                    break;
                case 1:
                    //communal payment documents
                    values = (withChilds
                                ? db.GetAllWithChildren<CommunalPaymentDocument>(recursive: true)
                                : db.Table<CommunalPaymentDocument>().ToList())
                                .Select(p => (T)Convert.ChangeType(p, typeof(T)))
                                .ToList();
                    break;
                case 2:
                    //service types
                    values = (withChilds
                                ? db.GetAllWithChildren<ServiceType>()
                                : db.Table<ServiceType>().ToList())
                                .Select(p => (T)Convert.ChangeType(p, typeof(T)))
                                .ToList();
                    break;
                case 3:
                    //services
                    values = (withChilds
                                ? db.GetAllWithChildren<Service>()
                                : db.Table<Service>().ToList())
                                .Select(p => (T)Convert.ChangeType(p, typeof(T)))
                                .ToList();
                    break;
            }
            return values;
        }

        //public List<CommunalPaymentDocument> GetCommunalPaymentDocuments(bool withChilds = false)
        //{
        //    return withChilds ? db.GetAllWithChildren<CommunalPaymentDocument>(recursive: true) 
        //                      : db.Table<CommunalPaymentDocument>().ToList();
        //}

        //public List<Address> GetAddresses(bool withChilds = false)
        //{
        //    return withChilds ? db.GetAllWithChildren<Address>()
        //                      : db.Table<Address>().ToList();
        //}

        //public List<ServiceType> GetServiceTypes(bool withChilds = false)
        //{
        //    return withChilds ? db.GetAllWithChildren<ServiceType>()
        //                      : db.Table<ServiceType>().ToList();
        //}

        //public List<Service> GetServices(bool withChilds = false)
        //{
        //    return withChilds ? db.GetAllWithChildren<Service>()
        //                      : db.Table<Service>().ToList();
        //}


        /// Get element by id

        public T GetElementById<T>(int id, bool withChilds = true)
        {
            T value;
            switch (typeToInt[typeof(T)])
            {
                case 0:
                    //addresse
                    var address = withChilds ? db.GetWithChildren<Service>(id) : db.Get<Service>(id);
                    value = (T)Convert.ChangeType(address, typeof(T));
                    break;
                case 1:
                    //communal payment document
                    var document = withChilds ? db.GetWithChildren<CommunalPaymentDocument>(id) : db.Get<CommunalPaymentDocument>(id);
                    value = (T)Convert.ChangeType(document, typeof(T));
                    break;
                case 2:
                    //service type
                    var serviceType = withChilds ? db.GetWithChildren<ServiceType>(id) : db.Get<ServiceType>(id);
                    value = (T)Convert.ChangeType(serviceType, typeof(T));
                    break;
                case 3:
                    //service
                    var service = withChilds ? db.GetWithChildren<Service>(id) : db.Get<Service>(id);
                    value = (T)Convert.ChangeType(service, typeof(T));
                    break;
                default:
                    value = (T)Convert.ChangeType(null, typeof(T));
                    break;
            }
            return value;
        }

        /// Delete element
        
        public void DeleteElement<T>(T element)
        {
            db.Delete(element);
        }

        //public CommunalPaymentDocument GetCommunalPaymentDocumentsById(int id, bool withChilds = true)
        //{
        //    return withChilds ? db.GetWithChildren<CommunalPaymentDocument>(id)
        //                      : db.Get<CommunalPaymentDocument>(id);
        //}

        //public Address GetAddressById(int id, bool withChilds = true)
        //{
        //    return withChilds ? db.GetWithChildren<Address>(id)
        //                      : db.Get<Address>(id);
        //}

        //public ServiceType GetServiceTypeById(int id, bool withChilds = true)
        //{
        //    return withChilds ? db.GetWithChildren<ServiceType>(id)
        //                      : db.Get<ServiceType>(id);
        //}

        //public Service GetServiceById(int id, bool withChilds = true)
        //{
        //    return withChilds ? db.GetWithChildren<Service>(id)
        //                      : db.Get<Service>(id);
        //}

    }
}
