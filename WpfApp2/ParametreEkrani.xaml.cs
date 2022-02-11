using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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

        /// <summary>
        /// SATIŞ PARAMETRELERİ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            grid1.Visibility = Visibility.Visible;
        }
        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            int[] newArr = new int[3];
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
        //////////


        //public static List<string> columnnames;
        //public static List<List<string>> rowstr;
        //private void Diger_Click(object sender, RoutedEventArgs e)
        //{
        //    grid2.Visibility = Visibility.Visible;
        //}

        //private void Kaydet_diger_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void Geri1_diger_Click(object sender, RoutedEventArgs e)
        //{
        //    grid2.Visibility = Visibility.Hidden;
        //}

        /// <summary>
        /// BELGE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fatduz_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\DXApplication3.exe");
        }

        //private void earsivduz_Click(object sender, RoutedEventArgs e)
        //{
        //    Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\DXApplication3.exe");
        //}

        private void fatura_Checked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt"))
            {
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt", String.Empty);
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt", "EFATURA");
            }
            else
            {
                File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt").Dispose();
                Carried.GrantAccess(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt");
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt", "EFATURA");
            }
        }
        private void earsiv_Checked(object sender, RoutedEventArgs e)
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt"))
            {
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt", String.Empty);//dosyada yazılanları siliyor
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt", "EARSIV");
            }
            else
            {
                File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt").Dispose();
                Carried.GrantAccess(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt");
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt", "EARSIV");
            }
        }
        private void fis_Checked(object sender, RoutedEventArgs e)
        {

            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt"))
            {
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt", String.Empty);
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt", "FIS");
            }
            else
            {
                File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt").Dispose();
                Carried.GrantAccess(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt");
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt", "FIS");
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt"))
            {
                string t1 = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt");
                if (t1 == "EFATURA") fatura.IsChecked = true; 
                else if (t1 == "EARSIV") earsiv.IsChecked = true;
                else if (t1 == "FIS") fis.IsChecked = true;
            }
        }
        /////////////////
        


        /// <summary>
        /// STOK PARAMETRELERİ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStk_Click(object sender, RoutedEventArgs e)
        {
            grid2.Visibility = Visibility.Visible;
        }

        //private static ObservableCollection<KullanıcıDepo> source;
        SqlCommand c1; SqlConnection sl;
        private void grid2_Loaded(object sender, RoutedEventArgs e)
        {
            Carried.DosyaDecrypt();
            string lc = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
            Carried.DosyaEncrypt();
            if (lc != null)
            {
                sl = new SqlConnection(lc);
                sl.Open();
                DataTable table = new DataTable();
                SqlDataAdapter a = new SqlDataAdapter("select KULLANICIADI as 'Kullanıcı Adı' , DEPOKOD as 'Depo Kodu' from SMRTAPPKUL", sl);
                a.Fill(table);
                this.depokullanici_dataGrid.ItemsSource = table.DefaultView;
                int x = 0;
                foreach (DataGridColumn column in depokullanici_dataGrid.Columns)
                {
                    if (x == 0) { column.IsReadOnly = true; column.Width = 200; }
                    else { column.IsReadOnly = false; column.Width = 250; }
                    x++;
                }
            }
            else { Carried.showMessage("Önce local bağlantıyı sağlayınız."); grid2.Visibility = Visibility.Hidden; }
        }
        private void Geri1_diger_Click(object sender, RoutedEventArgs e)
        {
            grid2.Visibility = Visibility.Hidden;
        }
        private void Kaydet_diger_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < depokullanici_dataGrid.Items.Count; i++)
            {
                DataRowView rowView = depokullanici_dataGrid.Items[i] as DataRowView;
                c1 = new SqlCommand("update smrtappkul set DEPOKOD = '" + rowView[1].ToString() + "' WHERE KULLANICIADI='" + rowView[0].ToString() + "'", sl);
                c1.ExecuteNonQuery();
                grid2.Visibility = Visibility.Hidden;
            }
        }
        ///////////////////////
    }

    //class KullanıcıDepo
    //{
    //    public string KullanıcıAdi { get; set; }
    //    public decimal DepoKod { get; set; }
    //}
}
