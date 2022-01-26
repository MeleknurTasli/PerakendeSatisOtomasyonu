using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for nakit.xaml
    /// </summary>
    public partial class nakit : Window
    {
        decimal toplamtutar;
        string odemeturu;
        decimal kalantutar;
        decimal odenen = 0;
        public nakit(decimal _toplamtutar, decimal _kalantutar, string _odemeturu)
        {
            InitializeComponent();
            this.odemeturu = _odemeturu;
            if (odemeturu == "NAKIT")
            {
                this.toplamtutar = _toplamtutar;
                txttoplamtutar.Text = toplamtutar.ToString();
            }
            
            else if (odemeturu == "PARCALI") // eğer parçalı ödemede nakit eklenecekse ekranı değiştiriyor.
            {
                this.toplamtutar = _toplamtutar;
                this.kalantutar = _kalantutar;
                btnode.Content = "EKLE";
                btnode.Foreground = Brushes.White;
                btnode.FontSize = 30;
                txtparaustu.Visibility = Visibility.Hidden;
                tbparaustu.Visibility = Visibility.Hidden;
                tbtoplamtutar.Text = "Kalan Miktar:";
                txttoplamtutar.Text = _kalantutar.ToString();
                tbodenen.Text = "Ödenecek Miktar:";
            }
        }


        private void txtodenen_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(odemeturu == "NAKIT")
            {
                if (String.IsNullOrWhiteSpace(txtodenen.Text))
                {
                    txtparaustu.Text = "0";
                }
                else
                {
                    if (txtodenen.Text[0].ToString() == ",") { txtodenen.Text = "0,"+ txtodenen.Text.Replace(",", ""); }
                    decimal odenen = Convert.ToDecimal(txtodenen.Text);
                    if (odenen <= toplamtutar)
                    {
                        txtparaustu.Text = "0";
                    }
                    else
                    {
                        decimal d = odenen - toplamtutar;
                        txtparaustu.Text = d.ToString();
                    }
                }
            }
            else if (odemeturu == "PARCALI")
            {
                if (String.IsNullOrWhiteSpace(txtodenen.Text)) txtodenen.Text = "0";
                if (txtodenen.Text[0].ToString() == ",") { txtodenen.Text = "0," + txtodenen.Text.Replace(",",""); }
                odenen = Convert.ToDecimal(txtodenen.Text);
                if (odenen > kalantutar)
                {
                    txtodenen.Text = txtodenen.Text.Remove(txtodenen.Text.Length - 1, 1);
                    Carried.showMessage("Ödenen miktar tutardan daha fazla olamaz.");
                }
            }
        }

        private void txtodenen_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (txtodenen.Text.Contains(",") || txtodenen.Text == "")
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
            }
            else
            {
                Regex regex = new Regex("[^0-9,]+");
                e.Handled = regex.IsMatch(e.Text);
            }
        }

        private void odeme_Click(object sender, RoutedEventArgs e)
        {
            if (odemeturu == "PARCALI")
            {
                if (String.IsNullOrWhiteSpace(txtodenen.Text)) txtodenen.Text = "0";
                Odeme.parcaliodeme w = new Odeme.parcaliodeme(toplamtutar, kalantutar - odenen, toplamtutar - (kalantutar - odenen));
                w.VerticalAlignment = this.VerticalAlignment;
                w.HorizontalAlignment = this.HorizontalAlignment;
                if (Odeme.parcaliodeme.source == null) Odeme.parcaliodeme.source = new ObservableCollection<odeme>();
                Odeme.parcaliodeme.source.Add(new odeme() { odemeYontemi = "NAKİT", tutar = Convert.ToDecimal(txtodenen.Text) });
                this.Close();
                w.Show();
            }
            else if (odemeturu == "NAKIT")
            {
                this.Close();
            }
        }
    }
}
