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
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Secim_Satis.xaml
    /// </summary>
    public partial class Secim_Satis : Window
    {
        public Secim_Satis()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Satıs_Islemleri w = new Satıs_Islemleri("SATIS");
            w.Show();
            this.Close();
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
            //w.Width = this.ActualWidth;
            //w.Height = this.ActualHeight;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Satıs_Islemleri w = new Satıs_Islemleri("SATISTAN İADE");
            w.Show();
            this.Close();
            //w.Width = this.ActualWidth;
            //w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
        }

        private void Geri_Click(object sender, RoutedEventArgs e)
        {
            Secim w = new Secim();
            w.Show();
            this.Close();
            w.Width = this.ActualWidth;
            w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
        }
    }
}
