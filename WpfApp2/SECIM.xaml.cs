using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using BespokeFusion;
using System.Data;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Secim : Window
    {
        //public string girisBaglantiCPM;
        //public string girisBaglantiLocal;
        //public bool IsCPMconnected;
        //public bool IsAdmin;
        public Secim()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            //girisBaglantiCPM = _girisBaglantiCPM;
            //girisBaglantiLocal = _girisBaglantiLocal;
            //IsCPMconnected = _IsCPMconnected;
            //IsAdmin = _IsAdmin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Carried.IsAdmin == true)
            { btnKullaniciOlustur.Visibility = Visibility.Visible; btnKullaniciCıkar.Visibility = Visibility.Visible; }
            else
            { btnKullaniciOlustur.Visibility = Visibility.Hidden; btnKullaniciCıkar.Visibility = Visibility.Hidden; }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Secim_Satis w = new Secim_Satis();
            w.Show();
            this.Close();
            w.Width = this.ActualWidth;
            w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
        }
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            StokKartAc w = new StokKartAc();
            w.Show();
            this.Close();
            //w.Width = this.ActualWidth;
            //w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
        }
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            CariKartAc w = new CariKartAc("SECIM");
            w.Show();
            this.Close();
            //w.Width = this.ActualWidth;
            //w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
        }





        private void btnKullaniciOlustur_Click(object sender, RoutedEventArgs e)
        {
            kullaniciEklePopUp.IsOpen = true;
        }
        private void btnKullaniciCıkar_Click(object sender, RoutedEventArgs e)
        {
            kullaniciCıkarPopUp.IsOpen = true;
        }
        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            kullaniciEklePopUp.IsOpen = false;
        }
        private void btnKapat2_Click(object sender, RoutedEventArgs e)
        {
            kullaniciCıkarPopUp.IsOpen = false;
        }
        private void btnEkle_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection s = new SqlConnection(Carried.girisBaglantiLocal);
            s.Open();
            if (!string.IsNullOrWhiteSpace(txtSifre.Text) && !string.IsNullOrWhiteSpace(txtAd.Text))
            {
                SqlCommand command = new SqlCommand("SELECT COUNT(KULLANICIADI) FROM SMRTAPPKUL WHERE KULLANICIADI='" + txtAd.Text + "'", s);
                object sayi = command.ExecuteScalar();
                if ((int)sayi == 0)
                {
                    command = new SqlCommand("INSERT INTO SMRTAPPKUL(KULLANICIADI, SIFRE, ADMIN) VALUES('" + txtAd.Text + "','" + txtSifre.Text + "',0)", s);
                    command.ExecuteNonQuery();
                    durum.Text = "Kullanıcı eklendi.";
                }
                else durum.Text = "Bu kullanıcı adına sahip başka bir kullanıcı mevcuttur.";
            }
            else durum.Text = "Kullanıcı eklenemedi.";
        }
        private void btnCikar_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection s = new SqlConnection(Carried.girisBaglantiLocal);
            s.Open();
            if (!string.IsNullOrWhiteSpace(txtSifre2.Text) && !string.IsNullOrWhiteSpace(txtAd2.Text))
            {
                SqlCommand command = new SqlCommand("SELECT COUNT(KULLANICIADI) FROM SMRTAPPKUL WHERE KULLANICIADI='" + txtAd2.Text + "'", s);
                object sayi = command.ExecuteScalar();
                if ((int)sayi > 0) //Böyle bir kullanıcı adı varsa
                {
                    command = new SqlCommand("SELECT SIFRE  FROM SMRTAPPKUL WHERE KULLANICIADI='" + txtAd2.Text + "'", s);
                    object sifreobj = command.ExecuteScalar();
                    if (sifreobj != null)
                    {
                        string sifreStr = sifreobj.ToString();
                        if (sifreStr == txtSifre2.Text) //ve şifresi dogru girilmişse
                        {
                            command = new SqlCommand("SELECT ADMIN  FROM SMRTAPPKUL WHERE KULLANICIADI='" + txtAd2.Text + "' AND SIFRE='" + txtSifre2.Text + "'", s);
                            object adminobj = command.ExecuteScalar();
                            if ((bool)adminobj == false)
                            {
                                command = new SqlCommand("DELETE FROM SMRTAPPKUL WHERE KULLANICIADI='" + txtAd2.Text + "' AND SIFRE='" + txtSifre2.Text + "'", s);
                                command.ExecuteNonQuery(); //kullanıcıyı sil
                                command = new SqlCommand("DELETE FROM SMRTAPPKASAKULLANICI WHERE KULLANICIADI='" + txtAd2.Text + "'", s);
                                command.ExecuteNonQuery();
                                durum2.Text = "Kullanıcı silindi.";
                            }
                            else durum2.Text = "Admin silinemez.";
                        }
                        else durum2.Text = "Şifre doğru değil.";
                    }
                }
                else durum2.Text = "Bu kullanıcı adında bir kullanıcı mevcut değil.";
            }
            else durum2.Text = "Kullanıcı bulunamadı.";
        }
        private void kullaniciGor_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConn = new SqlConnection())
            {
                sqlConn.ConnectionString = Carried.girisBaglantiLocal;
                string queryString = "select * from SMRTAPPKUL";
                sqlConn.Open();

                DataTable table = new DataTable();
                SqlDataAdapter a = new SqlDataAdapter(queryString, sqlConn);
                a.Fill(table);

                this.DGkullanicilar.ItemsSource = table.DefaultView;

                foreach (DataGridColumn column in DGkullanicilar.Columns)
                {
                    column.IsReadOnly = true;
                }
            }
        }
        private void kullaniciGor2_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection sqlConn = new SqlConnection())
            {
                sqlConn.ConnectionString = Carried.girisBaglantiLocal;
                string queryString = "select * from SMRTAPPKUL";
                sqlConn.Open();

                DataTable table = new DataTable();
                SqlDataAdapter a = new SqlDataAdapter(queryString, sqlConn);
                a.Fill(table);

                this.DGkullanicilar2.ItemsSource = table.DefaultView;
                foreach (DataGridColumn column in DGkullanicilar2.Columns)
                {
                    column.IsReadOnly = true;
                }
            }
        }


        private void geri_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Width = this.ActualWidth;
            mw.Height = this.ActualHeight;
            mw.WindowState = this.WindowState;
            mw.VerticalAlignment = this.VerticalAlignment;
            mw.HorizontalAlignment = this.HorizontalAlignment;
            mw.Show();
            this.Close();
        }

        private void kullaniciEklePopUp_Closed(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void kullaniciEklePopUp_Opened(object sender, EventArgs e)
        {
            this.Opacity = 0.5;
        }

        
    }
}