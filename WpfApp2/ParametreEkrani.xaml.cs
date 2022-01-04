using System;
using System.Collections.Generic;
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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for ParametreEkrani.xaml
    /// </summary>
    public partial class ParametreEkrani : Window
    {
        public ParametreEkrani()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Geri_Click(object sender, RoutedEventArgs e)
        {
            MainWindow w = new MainWindow();
            w.Width = this.ActualWidth;
            w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
            w.Show();
            this.Close();
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            grid1.Visibility = Visibility.Visible;
        }
        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt1.Text) &&
            string.IsNullOrWhiteSpace(txt2.Text) &&
            string.IsNullOrWhiteSpace(txt3.Text) &&
            string.IsNullOrWhiteSpace(txt4.Text) &&
            string.IsNullOrWhiteSpace(txt5.Text) && 
            string.IsNullOrWhiteSpace(txt6.Text) && 
            string.IsNullOrWhiteSpace(txt7.Text) && 
            string.IsNullOrWhiteSpace(txt8.Text))
            {
                Carried.showMessage("Bir şeyler giriniz.");
            }
            else 
            {
                Carried.DosyaDecrypt();
                if (!string.IsNullOrWhiteSpace(txt1.Text)) IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Satis Faturasi Evrak Tipi", txt1.Text, System.AppDomain.CurrentDomain.BaseDirectory + "Baglanti.ini");
                if (!string.IsNullOrWhiteSpace(txt2.Text)) IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Alim Faturasi Evrak Tipi", txt2.Text, System.AppDomain.CurrentDomain.BaseDirectory + "Baglanti.ini");
                if (!string.IsNullOrWhiteSpace(txt3.Text)) IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Satistan Iade Evrak Tipi", txt3.Text, System.AppDomain.CurrentDomain.BaseDirectory + "Baglanti.ini");
                if (!string.IsNullOrWhiteSpace(txt4.Text)) IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Alimdan Iade Evrak Tipi", txt4.Text, System.AppDomain.CurrentDomain.BaseDirectory + "Baglanti.ini");
                if (!string.IsNullOrWhiteSpace(txt5.Text)) IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Hesap Tipleri", txt5.Text, System.AppDomain.CurrentDomain.BaseDirectory + "Baglanti.ini");
                if (!string.IsNullOrWhiteSpace(txt6.Text) && !string.IsNullOrWhiteSpace(txt7.Text) && !string.IsNullOrWhiteSpace(txt8.Text))
                {

                    IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Hesap No Ayari", txt6.Text + "." + txt7.Text + "." + txt8.Text, System.AppDomain.CurrentDomain.BaseDirectory + "Baglanti.ini");
                }
                Carried.DosyaEncrypt();
                Carried.showMessage("BAŞARIYLA KAYDEDİLDİ.");
                grid1.Visibility = Visibility.Hidden;
            }
        }
        private void Geri1_Click(object sender, RoutedEventArgs e)
        {
            grid1.Visibility = Visibility.Hidden;
        }

        private void txt6_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        
    }
}
