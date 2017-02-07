using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcGISApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MapViewModel mvm = this.Resources["MapViewModel"] as MapViewModel;

            mvm.CreateOpacity();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MapViewModel mvm = this.Resources["MapViewModel"] as MapViewModel;

            mvm.CreateCircle();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MapViewModel mvm = this.Resources["MapViewModel"] as MapViewModel;

            mvm.CreateSmallCircles();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MapViewModel mvm = this.Resources["MapViewModel"] as MapViewModel;

            mvm.CreateSmallCircle();
        }

        // Map initialization logic is contained in MapViewModel.cs
    }
}
