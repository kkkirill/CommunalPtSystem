using CommunalPtSystem.src.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommunalPtSystem.src.templates
{
    public partial class PaymentDetailedWindow : Window
    {
        public bool IsReadOnly { get; set; }
        public bool NotIsReadOnly { get { return !IsReadOnly; } }

        public List<ServiceType> ServiceTypes { get; set; }

        public CommunalPaymentDocument document { get; set; }

        public PaymentDetailedWindow(CommunalPaymentDocument document, List<ServiceType> serviceTypes, bool isReadOnly)
        {
            InitializeComponent();
            this.document = document?.DeepCopy()
                            ?? new CommunalPaymentDocument() { 
                                Address = new Address(), 
                                Services = new List<Service>(), 
                                Date = DateTime.Now 
                            };
            ServiceTypes = serviceTypes;
            IsReadOnly = isReadOnly;
            DataContext = this.document;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^\d+\.\d+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var res = isValid();
            if (!res.isValid)
            {
                (Owner as MainWindow).ShowMessageBoxError(res.msg);
                return;
            }
            DialogResult = true;
            document.TotalSum = CalculateTotalSum();
        }
        
        private (string msg, bool isValid) isValid()
        {
            var kek = string.IsNullOrEmpty("0");
            var isAddressEmpty = document.Address.GetType()
                    .GetProperties()
                    .Select(pi =>
                    {
                        var v = pi.GetValue(document.Address);
                        return v?.GetType() == typeof(int) ? ((int)v == 0 && !pi.Name.Contains("Id") ? "" : "12") : (string)v;
                    })
                    .Any(value => string.IsNullOrEmpty(value));
            var isServisesEmpty = CalculateTotalSum() == 0;
            if (isAddressEmpty)
            {
                return (msg: "Fill address!", isValid: false);
            }
            if (isServisesEmpty)
            {
                return (msg: "Add any service or fix their cost!", isValid: false);
            }
            if (isAnyRepeatedServiceType())
            {
                return (msg: "Remove repeated service names!", isValid: false);
            }
            return (msg: "OK", true);
        }

        private bool isAnyRepeatedServiceType()
        {
            var serviceIds = document.Services?.Select(s => s.ServiceType.Id);
            return serviceIds?.Distinct().Count() != serviceIds?.Count();
        }

        private double CalculateTotalSum()
        {
            return Math.Round(document.Services?.Sum(e => e.Rate * e.Amount) ?? 0, 2);
        }

        private ServiceTypeDetailedWindow CreateDetailedWindow(ServiceType dataContext = null)
        {
            var serviceTypeDetailedWindow = new ServiceTypeDetailedWindow(dataContext);
            serviceTypeDetailedWindow.Owner = this;
            
            return serviceTypeDetailedWindow;
        }

        private void AddServiceTypeContextMenuClick(object sender, RoutedEventArgs e)
        {
            var serviceTypeDetailedWindow = CreateDetailedWindow();
            serviceTypeDetailedWindow.ShowDialog();

            if (!serviceTypeDetailedWindow.DialogResult ?? false)
            {
                return;
            }

            ServiceTypes.Add(serviceTypeDetailedWindow.serviceType);
        }

        private void EditServiceTypeContextMenuClick(object sender, RoutedEventArgs e)
        {
            var comboBox = (ComboBox)((ContextMenu)((FrameworkElement)sender).Parent).PlacementTarget;
            var selectedItem = comboBox.SelectedItem as ServiceType;
            var serviceTypeDetailedWindow = CreateDetailedWindow(selectedItem);

            serviceTypeDetailedWindow.ShowDialog();

            if (!serviceTypeDetailedWindow.DialogResult ?? false)
            {
                return;
            }
            
            selectedItem = serviceTypeDetailedWindow.serviceType;
            int i = ServiceTypes.FindIndex(st => st.Id == selectedItem.Id);
            ServiceTypes[i] = selectedItem;

            comboBox.SelectedIndex = -1;
            comboBox.Items.Refresh();
            comboBox.SelectedIndex = i;
        }

        private void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var document = DataContext as CommunalPaymentDocument;
            var services = Services.ItemsSource as List<Service>;
            var firstServiceType = ServiceTypes?.Count != 0 ? ServiceTypes[0] : null;

            var newService = new Service()
            {
                CommunalPaymentDocumentId = document.Id,
                Amount = 0,
                Rate = 0,
                ServiceTypeId = firstServiceType?.Id ?? 0,
                ServiceType = firstServiceType
            };

            (services ??= new List<Service>()).Add(newService);

            //(document.Services ??= new List<Service>()).Add(newService);

            Services.ItemsSource = services;
            Services.Items.Refresh();           
        }

        private void RemoveServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var target = Services.SelectedItem as Service;
            var services = Services.ItemsSource as List<Service>;

            if (target == null)
            {
                (Owner as MainWindow).ShowMessageBoxError("You should select item before!");
                return;
            }

            services.Remove(target);
            Services.Items.Refresh();
        }

        private void OnMouseButtonLeftClick(object sender, MouseButtonEventArgs e)
        {
            var target = sender as ListViewItem;
            target.IsSelected = true;
        }
    }
}
