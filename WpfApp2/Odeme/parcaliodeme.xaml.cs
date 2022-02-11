using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfApp2.Odeme
{
    /// <summary>
    /// Interaction logic for parcaliodeme.xaml
    /// </summary>
    public partial class parcaliodeme : Window
    {
        decimal toplamtutar;
        decimal odenen;
        decimal kalantutar;
        public static ObservableCollection<odeme> source;
        public parcaliodeme(decimal _toplamtutar, decimal _kalantutar, decimal _odenen)
        {
            InitializeComponent();
            this.toplamtutar = _toplamtutar;
            txtodenecektutar.Text = toplamtutar.ToString();
            this.odenen = _odenen;
            txtodenen.Text = odenen.ToString();
            this.kalantutar = _kalantutar;
            txtkalan.Text = kalantutar.ToString();
            if (kalantutar == 0) odemeekle.IsEnabled = false;
        }

        private void odemeekle_Click(object sender, RoutedEventArgs e)
        {
            PopupParcaliOdemeEkle.IsOpen = true;
        }

        private void nakit1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Odeme.nakit w = new Odeme.nakit(toplamtutar, kalantutar, "PARCALI");
            PopupParcaliOdemeEkle.IsOpen = false;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
            w.Show();
        }

        private void kredi1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Odeme.kredi w = new Odeme.kredi(Convert.ToDecimal(txtkalan.Text));
            PopupParcaliOdemeEkle.IsOpen = false;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
            w.Show();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            odeme_dataGrid.ItemsSource = source;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //source = null;
        }

        private void odemeyikapat_Click(object sender, RoutedEventArgs e)
        {
            source = null;
        }
    }

    public class odeme
    {
        public string odemeYontemi { get; set; }
        public decimal tutar { get; set; }
    }
}
