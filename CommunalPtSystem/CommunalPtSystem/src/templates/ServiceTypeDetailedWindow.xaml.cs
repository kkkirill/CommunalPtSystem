using System.Windows;

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
