using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommunalPtSystem.src.templates
{
    public partial class ServiceTypeDetailedWindow : Window
    {
        public ServiceType serviceType { get; set; }
        public ServiceTypeDetailedWindow(ServiceType serviceType)
        {
            InitializeComponent();
            this.serviceType = serviceType.DeepCopy();
            DataContext = this.serviceType;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
