using CommunalPtSystem.src.view_models;
using CommunalPtSystem.src.templates;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CommunalPtSystem.src.utils;
using System;

namespace CommunalPtSystem
{
    public partial class MainWindow : Window
    {
        MainViewModel MainViewModel;
        Filter filter;
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel = new MainViewModel();
            filter = new Filter();
            DataContext = MainViewModel;
            MainViewModel.filter = filter;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private PaymentDetailedWindow CreateDetailedWindow(bool isReadOnly, CommunalPaymentDocument dataContext = null)
        {
            var serviceTypes = MainViewModel.GetElements<ServiceType>();
            var paymentDetailedWindow = new PaymentDetailedWindow(dataContext, serviceTypes, isReadOnly);
            
            paymentDetailedWindow.Owner = this;

            return paymentDetailedWindow;
        }

        public void ShowMessageBoxError(string msg)
        {
            System.Media.SystemSounds.Asterisk.Play();
            Xceed.Wpf.Toolkit.MessageBox.Show(this, msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        private void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            CommunalPaymentDocument target = PaymentsList.SelectedItem as CommunalPaymentDocument;
            if (target == null)
            {
                ShowMessageBoxError("You should select item before!");
                return;
            }
            PaymentDetailedWindow paymentDetailedWindow = CreateDetailedWindow(true, target);


            paymentDetailedWindow.ShowDialog();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            PaymentDetailedWindow paymentDetailedWindow = CreateDetailedWindow(false);

            paymentDetailedWindow.ShowDialog();

            if (!paymentDetailedWindow.DialogResult ?? false)
            {
                return;
            }

            MainViewModel.UpdateOrCreateElements(paymentDetailedWindow.ServiceTypes);
            MainViewModel.UpdateOrCreateElement(paymentDetailedWindow.document);
            PaymentsList.ItemsSource = MainViewModel.PaymentDocuments;
            PaymentsList.Items.Refresh();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            CommunalPaymentDocument target = PaymentsList.SelectedItem as CommunalPaymentDocument;
            if (target == null)
            {
                ShowMessageBoxError("You should select item before!");
                return;
            }
            PaymentDetailedWindow paymentDetailedWindow = CreateDetailedWindow(false, target);

            paymentDetailedWindow.ShowDialog();

            if (!paymentDetailedWindow.DialogResult ?? false)
            {
                return;
            }

            target = paymentDetailedWindow.document;

            MainViewModel.UpdateOrCreateElements(paymentDetailedWindow.ServiceTypes);
            MainViewModel.UpdateOrCreateElement(target);
            PaymentsList.ItemsSource = MainViewModel.PaymentDocuments;
            PaymentsList.Items.Refresh();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            CommunalPaymentDocument target = PaymentsList.SelectedItem as CommunalPaymentDocument;
            if (target == null)
            {
                ShowMessageBoxError("You should select item before!");
                return;
            }

            MainViewModel.DeleteElement(target);
            PaymentsList.ItemsSource = MainViewModel.PaymentDocuments;
            PaymentsList.Items.Refresh();
        }

        private void ClearFields()
        {
            MinDateFilter.SelectedDate = null;
            MinDateFilter.DisplayDate = DateTime.Today;
            MaxDateFilter.SelectedDate = null;
            MaxDateFilter.DisplayDate = DateTime.Today;
            MinSumFilter.Text = "";
            MaxSumFilter.Text = "";
            CityFilter.Text = "";
            StreetFilter.Text = "";
            HouseNumberFilter.Text = "";
            FlatNumberFilter.Text = "";
        }

        private void saveFilterOptions(bool isEnabled)
        {
            filter.isEnabled = true;
            var startDate = MinDateFilter.SelectedDate ?? DateTime.Now;
            filter.startTime = new Tuple<bool, DateTime>(MinDateFilter.SelectedDate != null ? true : false, startDate);
            var endDate = MaxDateFilter.SelectedDate ?? DateTime.Now;
            filter.endTime = new Tuple<bool, DateTime>(MaxDateFilter.SelectedDate != null ? true : false, endDate);
            var minSumFilter = MinSumFilter.Text;
            if (minSumFilter.Length == 0)
            {
                filter.minSumValue = new Tuple<bool, int>(false, 0);
            }
            else
            {
                filter.minSumValue = new Tuple<bool, int>(minSumFilter != "0" ? true : false, int.Parse(minSumFilter));
            }
            var maxSumFilter = MaxSumFilter.Text;
            if (maxSumFilter.Length == 0)
            {
                filter.maxSumValue = new Tuple<bool, int>(false, 0);
            }
            else
            {
                filter.maxSumValue = new Tuple<bool, int>(maxSumFilter != "0" ? true : false, int.Parse(maxSumFilter));
            }
            var city = CityFilter.Text;
            filter.city = new Tuple<bool, string>(city.Length != 0 ? true : false, city);
            var street = StreetFilter.Text;
            filter.street = new Tuple<bool, string>(street.Length != 0 ? true : false, street);
            var houseNumber = HouseNumberFilter.Text;
            filter.houseNumber = new Tuple<bool, string>(houseNumber.Length != 0 && houseNumber != "0" ? true : false, houseNumber);
            var flatNumber = FlatNumberFilter.Text;
            filter.flatNumber = new Tuple<bool, string>(flatNumber.Length != 0 && flatNumber != "0" ? true : false, flatNumber);
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            saveFilterOptions(true);
            PaymentsList.ItemsSource = MainViewModel.PaymentDocuments;
            PaymentsList.Items.Refresh();
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            saveFilterOptions(false);
            PaymentsList.ItemsSource = MainViewModel.PaymentDocuments;
            PaymentsList.Items.Refresh();
        }
    }
}
