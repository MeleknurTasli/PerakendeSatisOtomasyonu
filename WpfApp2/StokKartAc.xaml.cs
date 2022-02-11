using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
using DevExpress.Data;
using DevExpress.Xpf.Core;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for StokKartAc.xaml
    /// </summary>
    public partial class StokKartAc : ThemedWindow
    {
        string path = null;
        public StokKartAc()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        public StokKartAc(string path)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.path = path;
        }
        private void duzenle1_Checked(object sender, RoutedEventArgs e)
        {
            gr1.Visibility = Visibility.Hidden;
            gr2.Visibility = Visibility.Hidden;
            gr3.Visibility = Visibility.Hidden;
            gr4.Visibility = Visibility.Hidden;
            gr5.Visibility = Visibility.Hidden;
            gr6.Visibility = Visibility.Hidden;
        }
        private void duzenle1_Unchecked(object sender, RoutedEventArgs e)
        {
            gr1.Visibility = Visibility.Visible;
            gr2.Visibility = Visibility.Visible;
            gr3.Visibility = Visibility.Visible;
            gr4.Visibility = Visibility.Visible;
            gr5.Visibility = Visibility.Visible;
            gr6.Visibility = Visibility.Visible;
        }
        private void txt11_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void txt1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.Contains(",") || textBox.Text == "")
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
        private void Geri_Click(object sender, RoutedEventArgs e)
        {
            Window w = new Secim();
            if (String.IsNullOrEmpty(path)) w = new Secim();
            else w = new Satıs_Islemleri("SATIS");
            w.Show();
            this.Close();
            //w.Width = this.ActualWidth;
            //w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
        }


        private void Check()
        {
            if (string.IsNullOrWhiteSpace(txtsf1.Text)) txtsf1.Text = "0";
            if (string.IsNullOrWhiteSpace(txtsf2.Text)) txtsf2.Text = "0";
            if (string.IsNullOrWhiteSpace(txtsf3.Text)) txtsf3.Text = "0";
            if (string.IsNullOrWhiteSpace(txtsf4.Text)) txtsf4.Text = "0";
            if (string.IsNullOrWhiteSpace(txtsf5.Text)) txtsf5.Text = "0";
            if (string.IsNullOrWhiteSpace(txtaf1.Text)) txtaf1.Text = "0";
            if (string.IsNullOrWhiteSpace(txtaf2.Text)) txtaf2.Text = "0";
            if (string.IsNullOrWhiteSpace(txtaf3.Text)) txtaf3.Text = "0";
            if (string.IsNullOrWhiteSpace(txtaf4.Text)) txtaf4.Text = "0";
            if (string.IsNullOrWhiteSpace(txtaf5.Text)) txtaf5.Text = "0";
            if (string.IsNullOrWhiteSpace(txtkdvorani.Text)) txtkdvorani.Text = "0";
            if (string.IsNullOrWhiteSpace(txtkdvkesintioran.Text)) txtkdvkesintioran.Text = "0";
            if (string.IsNullOrWhiteSpace(txtotvdegeri.Text)) txtotvdegeri.Text = "0";
            if (string.IsNullOrWhiteSpace(txtotvkesintioran.Text)) txtotvkesintioran.Text = "0";

            if (cb4.SelectedIndex == -1) cb4.SelectedIndex = 0;
            if (cbaf1doviz.SelectedIndex == -1) cbaf1doviz.SelectedIndex = 2;
            if (cbaf2doviz.SelectedIndex == -1) cbaf2doviz.SelectedIndex = 2;
            if (cbaf3doviz.SelectedIndex == -1) cbaf3doviz.SelectedIndex = 2;
            if (cbaf4doviz.SelectedIndex == -1) cbaf4doviz.SelectedIndex = 2;
            if (cbaf5doviz.SelectedIndex == -1) cbaf5doviz.SelectedIndex = 2;
            if (cbsf1doviz.SelectedIndex == -1) cbsf1doviz.SelectedIndex = 2;
            if (cbsf2doviz.SelectedIndex == -1) cbsf2doviz.SelectedIndex = 2;
            if (cbsf2doviz.SelectedIndex == -1) cbsf3doviz.SelectedIndex = 2;
            if (cbsf2doviz.SelectedIndex == -1) cbsf4doviz.SelectedIndex = 2;
            if (cbsf2doviz.SelectedIndex == -1) cbsf5doviz.SelectedIndex = 2;
            if (cbMalTipi.SelectedIndex == -1) cbMalTipi.SelectedIndex = 0;
            if (cbKartTipi.SelectedIndex == -1) cbKartTipi.SelectedIndex = 0;
            if (cb_kdvdahilharic_.SelectedIndex == -1) cbKartTipi.SelectedIndex = 0;
            if (cb_SF1DH.SelectedIndex == -1) cb_SF1DH.SelectedIndex = 0;
            if (cb_SF2DH.SelectedIndex == -1) cb_SF2DH.SelectedIndex = 0;
            if (cb_SF3DH.SelectedIndex == -1) cb_SF3DH.SelectedIndex = 0;
            if (cb_SF4DH.SelectedIndex == -1) cb_SF4DH.SelectedIndex = 0;
            if (cb_SF5DH.SelectedIndex == -1) cb_SF5DH.SelectedIndex = 0;
            if (cb_AF1DH.SelectedIndex == -1) cb_AF1DH.SelectedIndex = 0;
            if (cb_AF2DH.SelectedIndex == -1) cb_AF2DH.SelectedIndex = 0;
            if (cb_AF3DH.SelectedIndex == -1) cb_AF3DH.SelectedIndex = 0;
            if (cb_AF4DH.SelectedIndex == -1) cb_AF4DH.SelectedIndex = 0;
            if (cb_AF5DH.SelectedIndex == -1) cb_AF5DH.SelectedIndex = 0;
            //CB1????????????OTOMASYON???????????


        }
        private void Duzenle_Click(object sender, RoutedEventArgs e)
        {
            s = new SqlConnection(Carried.girisBaglantiLocal);
            Duzenle_("SMRTAPPSKRT", s);

            if (Carried.IsCPMconnected == true)
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                Duzenle_("STKKRT", s);
            }
        }
        private void Duzenle_(string tablename, SqlConnection s /*, TextBox t*/)
        {
            Check();
            s.Open();
            string ekle = " DEGISTIRENKULLANICI=@DEGISTIRENKULLANICI, DEGISTIRENTARIH=@DEGISTIRENTARIH, DEGISTIRENSAAT=@DEGISTIRENSAAT, DEGISTIRENKAYNAK=@DEGISTIRENKAYNAK, DEGISTIRENSURUM=@DEGISTIRENSURUM,  " +
                "GRUPKOD=@GRUPKOD, MALTIP=@MALTIP, KARTTIP=@KARTTIP, MALKOD=@MALKOD, MALAD=@MALAD, BIRIM=@BIRIM, DOVIZCINS=@DOVIZCINS, STOKKONTROLDURUM=@STOKKONTROLDURUM, MARKAAD=@MARKAAD, MODELAD=@MODELAD, URETICIKOD=@URETICIKOD, URETICIMALKOD=@URETICIMALKOD, " +
                "SATISEVRAKSEKLI=@SATISEVRAKSEKLI, SATISFIYATSEKLI=@SATISFIYATSEKLI, SATISEVRAKBIRIM=@SATISEVRAKBIRIM, SATISFIYAT1=@SATISFIYAT1, SATISFIYAT2=@SATISFIYAT2,SATISFIYAT3=@SATISFIYAT3,SATISFIYAT4=@SATISFIYAT4,SATISFIYAT5=@SATISFIYAT5, SATISFIYAT1DOVIZCINS=@SATISFIYAT1DOVIZCINS, SATISFIYAT2DOVIZCINS=@SATISFIYAT2DOVIZCINS, SATISFIYAT3DOVIZCINS=@SATISFIYAT3DOVIZCINS, SATISFIYAT4DOVIZCINS=@SATISFIYAT4DOVIZCINS, SATISFIYAT5DOVIZCINS=@SATISFIYAT5DOVIZCINS, SATISFIYAT1KDVDH=@SATISFIYAT1KDVDH, SATISFIYAT2KDVDH=@SATISFIYAT2KDVDH, SATISFIYAT3KDVDH=@SATISFIYAT3KDVDH, SATISFIYAT4KDVDH=@SATISFIYAT4KDVDH, SATISFIYAT5KDVDH=@SATISFIYAT5KDVDH, " +
                "ALIMEVRAKSEKLI=@ALIMEVRAKSEKLI, ALIMFIYATSEKLI=@ALIMFIYATSEKLI, ALIMEVRAKBIRIM=@ALIMEVRAKBIRIM, ALISFIYAT1=@ALISFIYAT1, ALISFIYAT2=@ALISFIYAT2,ALISFIYAT3=@ALISFIYAT3,ALISFIYAT4=@ALISFIYAT4,ALISFIYAT5=@ALISFIYAT5, ALISFIYAT1DOVIZCINS=@ALISFIYAT1DOVIZCINS, ALISFIYAT2DOVIZCINS=@ALISFIYAT2DOVIZCINS, ALISFIYAT3DOVIZCINS=@ALISFIYAT3DOVIZCINS, ALISFIYAT4DOVIZCINS=@ALISFIYAT4DOVIZCINS, ALISFIYAT5DOVIZCINS=@ALISFIYAT5DOVIZCINS, ALISFIYAT1KDVDH=@ALISFIYAT1KDVDH, ALISFIYAT2KDVDH=@ALISFIYAT2KDVDH, ALISFIYAT3KDVDH=@ALISFIYAT3KDVDH, ALISFIYAT4KDVDH=@ALISFIYAT4KDVDH, ALISFIYAT5KDVDH=@ALISFIYAT5KDVDH, " +
                "GUVENLIKKOD=@GUVENLIKKOD, OZELKOD=@OZELKOD, TIPKOD=@TIPKOD, KDVORAN=@KDVORAN, kdvdh=@kdvdh, KDVKESINTIORAN=@KDVKESINTIORAN, OTVTIP=@OTVTIP, OTVDEGER=@OTVDEGER, OTVKESINTIORAN=@OTVKESINTIORAN ";
            c1 = new SqlCommand("UPDATE " + tablename + " SET " + ekle + " " + value, s);
            c1.Parameters.Add(new SqlParameter("@DEGISTIRENKULLANICI", Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30))));
            c1.Parameters.Add("@DEGISTIRENTARIH", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
            c1.Parameters.Add("@DEGISTIRENSAAT", SqlDbType.SmallDateTime).Value = DateTime.Now;
            c1.Parameters.Add(new SqlParameter("@DEGISTIRENKAYNAK", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30))));
            c1.Parameters.Add(new SqlParameter("@DEGISTIRENSURUM", ""));
            c1.Parameters.AddWithValue("@GRUPKOD", txt3.Text);
            c1.Parameters.AddWithValue("@MALTIP", Convert.ToInt16(cbMalTipi.SelectedIndex));
            c1.Parameters.AddWithValue("@KARTTIP", Convert.ToInt16(cbKartTipi.SelectedIndex));
            c1.Parameters.AddWithValue("@MALKOD", txt5.Text);
            c1.Parameters.AddWithValue("@MALAD", txt6.Text);
            c1.Parameters.AddWithValue("@BIRIM", txt7.Text);
            c1.Parameters.AddWithValue("@DOVIZCINS", cb4.SelectedValue.ToString());
            if(cbStokKontrol.SelectedValue.ToString().Contains("DÜŞEBİLİR")) c1.Parameters.AddWithValue("@STOKKONTROLDURUM", 1);
            else c1.Parameters.AddWithValue("@STOKKONTROLDURUM", 0);
            c1.Parameters.AddWithValue("@MARKAAD", txt4.Text);
            c1.Parameters.AddWithValue("@MODELAD", txt8.Text);
            c1.Parameters.AddWithValue("@URETICIKOD", txt9.Text);
            c1.Parameters.AddWithValue("@URETICIMALKOD", txt10.Text);
            c1.Parameters.AddWithValue("@SATISEVRAKSEKLI", txtsatevrsekli.Text);
            c1.Parameters.AddWithValue("@SATISEVRAKBIRIM", txtsatbirim.Text);
            c1.Parameters.AddWithValue("@SATISFIYATSEKLI", txtsatfytsekli.Text);
            c1.Parameters.AddWithValue("@ALIMEVRAKSEKLI", txtalevrsekli.Text);
            c1.Parameters.AddWithValue("@ALIMEVRAKBIRIM", txtalbirim.Text);
            c1.Parameters.AddWithValue("@ALIMFIYATSEKLI", txtalfytsekli.Text);
            c1.Parameters.AddWithValue("@SATISFIYAT1", Convert.ToDecimal(txtsf1.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT2", Convert.ToDecimal(txtsf2.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT3", Convert.ToDecimal(txtsf3.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT4", Convert.ToDecimal(txtsf4.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT5", Convert.ToDecimal(txtsf5.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT1", Convert.ToDecimal(txtaf1.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT2", Convert.ToDecimal(txtaf2.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT3", Convert.ToDecimal(txtaf3.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT4", Convert.ToDecimal(txtaf4.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT5", Convert.ToDecimal(txtaf5.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT1DOVIZCINS", cbsf1doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT2DOVIZCINS", cbsf2doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT3DOVIZCINS", cbsf3doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT4DOVIZCINS", cbsf4doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT5DOVIZCINS", cbsf5doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT1DOVIZCINS", cbaf1doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT2DOVIZCINS", cbaf2doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT3DOVIZCINS", cbaf3doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT4DOVIZCINS", cbaf4doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT5DOVIZCINS", cbaf5doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT1KDVDH", Convert.ToInt16(cb_SF1DH.SelectedIndex));
            c1.Parameters.AddWithValue("@SATISFIYAT2KDVDH", Convert.ToInt16(cb_SF2DH.SelectedIndex));
            c1.Parameters.AddWithValue("@SATISFIYAT3KDVDH", Convert.ToInt16(cb_SF3DH.SelectedIndex));
            c1.Parameters.AddWithValue("@SATISFIYAT4KDVDH", Convert.ToInt16(cb_SF4DH.SelectedIndex));
            c1.Parameters.AddWithValue("@SATISFIYAT5KDVDH", Convert.ToInt16(cb_SF5DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT1KDVDH", Convert.ToInt16(cb_AF1DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT2KDVDH", Convert.ToInt16(cb_AF2DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT3KDVDH", Convert.ToInt16(cb_AF3DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT4KDVDH", Convert.ToInt16(cb_AF4DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT5KDVDH", Convert.ToInt16(cb_AF5DH.SelectedIndex));
            c1.Parameters.AddWithValue("@GUVENLIKKOD", txtguvenlikkodu.Text);
            c1.Parameters.AddWithValue("@OZELKOD", txtozelkodu.Text);
            c1.Parameters.AddWithValue("@TIPKOD", txttipkodu.Text);
            c1.Parameters.AddWithValue("@KDVORAN", Convert.ToSingle(txtkdvorani.Text));
            c1.Parameters.AddWithValue("@kdvdh", Convert.ToInt16(cb_kdvdahilharic_.SelectedIndex));
            c1.Parameters.AddWithValue("@KDVKESINTIORAN", Convert.ToSingle(txtkdvkesintioran.Text));
            if(cbOtvtip.SelectedValue.ToString().Contains("TUTAR")) c1.Parameters.AddWithValue("@OTVTIP", 2);
            else c1.Parameters.AddWithValue("@OTVTIP", 0);
            c1.Parameters.AddWithValue("@OTVDEGER", Convert.ToDecimal(txtotvdegeri.Text));
            c1.Parameters.AddWithValue("@OTVKESINTIORAN", Convert.ToSingle(txtotvkesintioran.Text));
            c1.ExecuteNonQuery();
        }
        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            s = new SqlConnection(Carried.girisBaglantiLocal);
            Kayit("SMRTAPPSKRT", s);

            if (Carried.IsCPMconnected == true)
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                Kayit("STKKRT", s);
            }
        }
        private void Kayit(string tablename, SqlConnection s)
        {
            Check();
            s.Open();
            string ekle = " @DEGISTIRENKULLANICI, @DEGISTIRENTARIH, @DEGISTIRENSAAT, @DEGISTIRENKAYNAK, @DEGISTIRENSURUM, @GIRENKULLANICI, @GIRENTARIH, @GIRENSAAT, @GIRENKAYNAK, @GIRENSURUM, " +
                "@GRUPKOD, @MALTIP, @KARTTIP, @MALKOD, @MALAD, @BIRIM, @DOVIZCINS, @STOKKONTROLDURUM, @MARKAAD, @MODELAD, @URETICIKOD, @URETICIMALKOD, " +
                "@SATISEVRAKSEKLI, @SATISFIYATSEKLI, @SATISEVRAKBIRIM, @SATISFIYAT1, @SATISFIYAT2, @SATISFIYAT3, @SATISFIYAT4, @SATISFIYAT5, @SATISFIYAT1DOVIZCINS, @SATISFIYAT2DOVIZCINS, @SATISFIYAT3DOVIZCINS, @SATISFIYAT4DOVIZCINS, @SATISFIYAT5DOVIZCINS, @SATISFIYAT1KDVDH, @SATISFIYAT2KDVDH, @SATISFIYAT3KDVDH, @SATISFIYAT4KDVDH, @SATISFIYAT5KDVDH, " +
                "@ALIMEVRAKSEKLI, @ALIMFIYATSEKLI, @ALIMEVRAKBIRIM, @ALISFIYAT1, @ALISFIYAT2, @ALISFIYAT3, @ALISFIYAT4, @ALISFIYAT5, @ALISFIYAT1DOVIZCINS, @ALISFIYAT2DOVIZCINS, @ALISFIYAT3DOVIZCINS, @ALISFIYAT4DOVIZCINS, @ALISFIYAT5DOVIZCINS, @ALISFIYAT1KDVDH, @ALISFIYAT2KDVDH, @ALISFIYAT3KDVDH, @ALISFIYAT4KDVDH, @ALISFIYAT5KDVDH, " +
                "@GUVENLIKKOD, @OZELKOD, @TIPKOD, @KDVORAN, @kdvdh, @KDVKESINTIORAN, @OTVTIP, @OTVDEGER, @OTVKESINTIORAN ";
           
            c1 = new SqlCommand("INSERT INTO " + tablename + " (DEGISTIRENKULLANICI, DEGISTIRENTARIH, DEGISTIRENSAAT, DEGISTIRENKAYNAK, DEGISTIRENSURUM, GIRENKULLANICI, GIRENTARIH, GIRENSAAT, GIRENKAYNAK, GIRENSURUM, " +
                "GRUPKOD, MALTIP, KARTTIP, MALKOD, MALAD, BIRIM, DOVIZCINS, STOKKONTROLDURUM, MARKAAD, MODELAD, URETICIKOD, URETICIMALKOD, " +
                "SATISEVRAKSEKLI, SATISFIYATSEKLI, SATISEVRAKBIRIM, SATISFIYAT1, SATISFIYAT2,SATISFIYAT3,SATISFIYAT4,SATISFIYAT5, SATISFIYAT1DOVIZCINS, SATISFIYAT2DOVIZCINS, SATISFIYAT3DOVIZCINS, SATISFIYAT4DOVIZCINS, SATISFIYAT5DOVIZCINS, SATISFIYAT1KDVDH, SATISFIYAT2KDVDH, SATISFIYAT3KDVDH, SATISFIYAT4KDVDH, SATISFIYAT5KDVDH, " +
                "ALIMEVRAKSEKLI, ALIMFIYATSEKLI, ALIMEVRAKBIRIM, ALISFIYAT1, ALISFIYAT2,ALISFIYAT3,ALISFIYAT4,ALISFIYAT5, ALISFIYAT1DOVIZCINS, ALISFIYAT2DOVIZCINS, ALISFIYAT3DOVIZCINS, ALISFIYAT4DOVIZCINS, ALISFIYAT5DOVIZCINS, ALISFIYAT1KDVDH, ALISFIYAT2KDVDH, ALISFIYAT3KDVDH, ALISFIYAT4KDVDH, ALISFIYAT5KDVDH, " +
                "GUVENLIKKOD, OZELKOD, TIPKOD, KDVORAN, kdvdh, KDVKESINTIORAN, OTVTIP, OTVDEGER, OTVKESINTIORAN) VALUES (" + ekle + ")", s);
            c1.Parameters.Add(new SqlParameter("@GIRENKULLANICI", Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30))));
            c1.Parameters.Add("@GIRENTARIH", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
            c1.Parameters.Add("@GIRENSAAT", SqlDbType.SmallDateTime).Value = DateTime.Now;
            c1.Parameters.Add(new SqlParameter("@GIRENKAYNAK", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30))));
            c1.Parameters.Add(new SqlParameter("@GIRENSURUM", ""));
            c1.Parameters.Add(new SqlParameter("@DEGISTIRENKULLANICI", Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30))));
            c1.Parameters.Add("@DEGISTIRENTARIH", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
            c1.Parameters.Add("@DEGISTIRENSAAT", SqlDbType.SmallDateTime).Value = DateTime.Now;
            c1.Parameters.Add(new SqlParameter("@DEGISTIRENKAYNAK", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30))));
            c1.Parameters.Add(new SqlParameter("@DEGISTIRENSURUM", ""));
            c1.Parameters.AddWithValue("@GRUPKOD", txt3.Text);
            c1.Parameters.AddWithValue("@MALTIP", Convert.ToInt16(cbMalTipi.SelectedIndex));
            c1.Parameters.AddWithValue("@KARTTIP", Convert.ToInt16(cbKartTipi.SelectedIndex));
            c1.Parameters.AddWithValue("@MALKOD", txt5.Text);
            c1.Parameters.AddWithValue("@MALAD", txt6.Text);
            c1.Parameters.AddWithValue("@BIRIM", txt7.Text);
            c1.Parameters.AddWithValue("@DOVIZCINS", cb4.SelectedValue.ToString());
            if (cbStokKontrol.SelectedValue.ToString().Contains("DÜŞMESİN")) c1.Parameters.AddWithValue("@STOKKONTROLDURUM", 0);
            else c1.Parameters.AddWithValue("@STOKKONTROLDURUM", 1);
            c1.Parameters.AddWithValue("@MARKAAD", txt4.Text);
            c1.Parameters.AddWithValue("@MODELAD", txt8.Text);
            c1.Parameters.AddWithValue("@URETICIKOD", txt9.Text);
            c1.Parameters.AddWithValue("@URETICIMALKOD", txt10.Text);
            c1.Parameters.AddWithValue("@SATISEVRAKSEKLI", txtsatevrsekli.Text);
            c1.Parameters.AddWithValue("@SATISEVRAKBIRIM", txtsatbirim.Text);
            c1.Parameters.AddWithValue("@SATISFIYATSEKLI", txtsatfytsekli.Text);
            c1.Parameters.AddWithValue("@ALIMEVRAKSEKLI", txtalevrsekli.Text);
            c1.Parameters.AddWithValue("@ALIMEVRAKBIRIM", txtalbirim.Text);
            c1.Parameters.AddWithValue("@ALIMFIYATSEKLI", txtalfytsekli.Text);
            c1.Parameters.AddWithValue("@SATISFIYAT1", Convert.ToDecimal(txtsf1.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT2", Convert.ToDecimal(txtsf2.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT3", Convert.ToDecimal(txtsf3.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT4", Convert.ToDecimal(txtsf4.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT5", Convert.ToDecimal(txtsf5.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT1", Convert.ToDecimal(txtaf1.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT2", Convert.ToDecimal(txtaf2.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT3", Convert.ToDecimal(txtaf3.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT4", Convert.ToDecimal(txtaf4.Text));
            c1.Parameters.AddWithValue("@ALISFIYAT5", Convert.ToDecimal(txtaf5.Text));
            c1.Parameters.AddWithValue("@SATISFIYAT1DOVIZCINS", cbsf1doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT2DOVIZCINS", cbsf2doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT3DOVIZCINS", cbsf3doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT4DOVIZCINS", cbsf4doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT5DOVIZCINS", cbsf5doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT1DOVIZCINS", cbaf1doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT2DOVIZCINS", cbaf2doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT3DOVIZCINS", cbaf3doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT4DOVIZCINS", cbaf4doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@ALISFIYAT5DOVIZCINS", cbaf5doviz.SelectedValue.ToString());
            c1.Parameters.AddWithValue("@SATISFIYAT1KDVDH", Convert.ToInt16(cb_SF1DH.SelectedIndex));
            c1.Parameters.AddWithValue("@SATISFIYAT2KDVDH", Convert.ToInt16(cb_SF2DH.SelectedIndex));
            c1.Parameters.AddWithValue("@SATISFIYAT3KDVDH", Convert.ToInt16(cb_SF3DH.SelectedIndex));
            c1.Parameters.AddWithValue("@SATISFIYAT4KDVDH", Convert.ToInt16(cb_SF4DH.SelectedIndex));
            c1.Parameters.AddWithValue("@SATISFIYAT5KDVDH", Convert.ToInt16(cb_SF5DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT1KDVDH", Convert.ToInt16(cb_AF1DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT2KDVDH", Convert.ToInt16(cb_AF2DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT3KDVDH", Convert.ToInt16(cb_AF3DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT4KDVDH", Convert.ToInt16(cb_AF4DH.SelectedIndex));
            c1.Parameters.AddWithValue("@ALISFIYAT5KDVDH", Convert.ToInt16(cb_AF5DH.SelectedIndex));
            c1.Parameters.AddWithValue("@GUVENLIKKOD", txtguvenlikkodu.Text);
            c1.Parameters.AddWithValue("@OZELKOD", txtozelkodu.Text);
            c1.Parameters.AddWithValue("@TIPKOD", txttipkodu.Text);
            c1.Parameters.AddWithValue("@KDVORAN", Convert.ToSingle(txtkdvorani.Text));
            c1.Parameters.AddWithValue("@kdvdh", Convert.ToInt16(cb_kdvdahilharic_.SelectedIndex));
            c1.Parameters.AddWithValue("@KDVKESINTIORAN", Convert.ToSingle(txtkdvkesintioran.Text));
            if (cbOtvtip.SelectedValue.ToString().Contains("TUTAR")) c1.Parameters.AddWithValue("@OTVTIP", 2);
            else c1.Parameters.AddWithValue("@OTVTIP", 0);
            c1.Parameters.AddWithValue("@OTVDEGER", Convert.ToDecimal(txtotvdegeri.Text));
            c1.Parameters.AddWithValue("@OTVKESINTIORAN", Convert.ToSingle(txtotvkesintioran.Text));
            c1.ExecuteNonQuery();
        }



        private void veriGetir(string value)//DÜZELT
        {
            string tablename; object o;
            if (Carried.IsCPMconnected == true)
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                tablename = "STKKRT";
            }
            else
            {
                s = new SqlConnection(Carried.girisBaglantiLocal);
                tablename = "SMRTAPPSKRT";
            }
            s.Open();

            //DataTable table = new DataTable();
            //SqlDataAdapter a = new SqlDataAdapter("select * FROM " + tablename + " " + value, s);
            //a.Fill(table);
            //DataView dataView = new DataView();
            //dataView = table.DefaultView;
            //Binding bindMyColumn1 = new Binding();
            //bindMyColumn1.Source = dataView;
            //bindMyColumn1.Path = new PropertyPath("[0][MALAD]");
            //txt6.SetBinding(TextBox.TextProperty, bindMyColumn1);
           
            txt2.Text = (o = new SqlCommand("select KAYITDURUM from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txt3.Text = (o = new SqlCommand("select GRUPKOD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            cbKartTipi.SelectedIndex = (o = new SqlCommand("select MALTIP from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            cbMalTipi.SelectedIndex = (o = new SqlCommand("select KARTTIP from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            txt5.Text = (o = new SqlCommand("select MALKOD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txt6.Text = (o = new SqlCommand("select MALAD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txt7.Text = (o = new SqlCommand("select BIRIM from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            cb4.SelectedIndex = (o = new SqlCommand("select DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cbStokKontrol.SelectedIndex = (o = new SqlCommand("select STOKKONTROLDURUM from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            txt4.Text = (o = new SqlCommand("select MARKAAD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txt8.Text = (o = new SqlCommand("select MODELAD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txt9.Text = (o = new SqlCommand("select URETICIKOD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txt10.Text = (o = new SqlCommand("select URETICIMALKOD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";

            txtsatevrsekli.Text = (o = new SqlCommand("select SATISEVRAKSEKLI from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txtsatfytsekli.Text = (o = new SqlCommand("select SATISFIYATSEKLI from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txtsatbirim.Text = (o = new SqlCommand("select SATISEVRAKBIRIM from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txtsf1.Text = (o = new SqlCommand("select SATISFIYAT1 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            txtsf2.Text = (o = new SqlCommand("select SATISFIYAT2 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            txtsf3.Text = (o = new SqlCommand("select SATISFIYAT3 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            txtsf4.Text = (o = new SqlCommand("select SATISFIYAT4 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            txtsf5.Text = (o = new SqlCommand("select SATISFIYAT5 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            cbsf1doviz.SelectedIndex = (o = new SqlCommand("select SATISFIYAT1DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cbsf2doviz.SelectedIndex = (o = new SqlCommand("select SATISFIYAT2DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cbsf3doviz.SelectedIndex = (o = new SqlCommand("select SATISFIYAT3DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cbsf4doviz.SelectedIndex = (o = new SqlCommand("select SATISFIYAT4DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cbsf5doviz.SelectedIndex = (o = new SqlCommand("select SATISFIYAT5DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cb_SF1DH.SelectedIndex = (o = new SqlCommand("select SATISFIYAT1KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            cb_SF2DH.SelectedIndex = (o = new SqlCommand("select SATISFIYAT2KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            cb_SF3DH.SelectedIndex = (o = new SqlCommand("select SATISFIYAT3KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            cb_SF4DH.SelectedIndex = (o = new SqlCommand("select SATISFIYAT4KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            cb_SF5DH.SelectedIndex = (o = new SqlCommand("select SATISFIYAT5KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;

            txtalevrsekli.Text = (o = new SqlCommand("select ALIMEVRAKSEKLI from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txtalfytsekli.Text = (o = new SqlCommand("select ALIMFIYATSEKLI from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txtalbirim.Text = (o = new SqlCommand("select ALIMEVRAKBIRIM from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txtaf1.Text = (o = new SqlCommand("select ALISFIYAT1 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            txtaf2.Text = (o = new SqlCommand("select ALISFIYAT2 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            txtaf3.Text = (o = new SqlCommand("select ALISFIYAT3 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            txtaf4.Text = (o = new SqlCommand("select ALISFIYAT4 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            txtaf5.Text = (o = new SqlCommand("select ALISFIYAT5 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            cbaf1doviz.SelectedIndex = (o = new SqlCommand("select ALISFIYAT1DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cbaf2doviz.SelectedIndex = (o = new SqlCommand("select ALISFIYAT2DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cbaf3doviz.SelectedIndex = (o = new SqlCommand("select ALISFIYAT3DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cbaf4doviz.SelectedIndex = (o = new SqlCommand("select ALISFIYAT4DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cbaf5doviz.SelectedIndex = (o = new SqlCommand("select ALISFIYAT5DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? getdovizcins(o.ToString()) : -1;
            cb_AF1DH.SelectedIndex = (o = new SqlCommand("select ALISFIYAT1KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            cb_AF2DH.SelectedIndex = (o = new SqlCommand("select ALISFIYAT2KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            cb_AF3DH.SelectedIndex = (o = new SqlCommand("select ALISFIYAT3KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            cb_AF4DH.SelectedIndex = (o = new SqlCommand("select ALISFIYAT4KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            cb_AF5DH.SelectedIndex = (o = new SqlCommand("select ALISFIYAT5KDVDH from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            
            txtguvenlikkodu.Text = (o = new SqlCommand("select GUVENLIKKOD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txtozelkodu.Text = (o = new SqlCommand("select OZELKOD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txttipkodu.Text = (o = new SqlCommand("select TIPKOD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txtkdvorani_.Text = (o = new SqlCommand("select KDVORAN from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            cb_kdvdahilharic_.SelectedIndex = (o = new SqlCommand("select kdvdh from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : -1;
            txtkdvkesintioran.Text = (o = new SqlCommand("select KDVKESINTIORAN from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            cbOtvtip.SelectedIndex = (o = new SqlCommand("select OTVTIP from " + tablename + " " + value, s).ExecuteScalar()) != null ? getotvtip(Convert.ToInt32(o)) : -1;
            txtotvdegeri.Text = (o = new SqlCommand("select OTVDEGER from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";
            txtotvkesintioran.Text = (o = new SqlCommand("select OTVKESINTIORAN from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "0";



        }
        private int getdovizcins(string a)
        {
            if(a.Contains("USD")) return 0;
            else if(a.Contains("EUR")) return 1;
            else /*if(a.Contains("TRY"))*/ return 2;
        }
        private int getotvtip(int a)
        {
            if (a == 0) return 0;
            else return 1;   //1 YA DA 2 YANİ TUTAR
        }
        


        /// <summary>
        /// Stok Getir
        /// </summary>
        string value, tabloadi; SqlCommand c1; SqlConnection s;
        private void txtstokara_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty( txtstokara.Text))
            {
                if ( txtstokara.Text.StartsWith(" ")) {  txtstokara.Clear(); }

                try
                {
                    value = BarkodVSMalkodVSMalad();
                    if (value != null) veriGetir(value);
                }
                catch (Exception ex)
                {
                    if (Carried.IsCPMconnected == true)
                    {
                        bool netcon = Carried.CheckForInternetConnection();
                        Carried.IsCPMconnected = netcon == true ? true : false;
                        if (netcon == false) { Carried.showMessage("İnternet bağlantısı bulunamadığı için local bağlantıya geçildi. İşlemi tekrar yapınız"); return; }
                    }
                    if (ex.Message.Contains("String or binary data would be truncated")) Carried.showMessage("Girdiğiniz bazı değerler çok uzun olduğu için işlem yapılamadı.");
                    else Carried.showMessage(ex.Message);
                }
            }
        }
        private string BarkodVSMalkodVSMalad()
        {
            object o1, o2, o3;
            if (Carried.IsCPMconnected == true)
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                s.Open();
                string cmdBarkodMu = "select BARKOD1 from stkkrt where BARKOD1 = '" +  txtstokara.Text + "'";
                string cmdMalkodMu = "select MALKOD from stkkrt where MALKOD = '" +  txtstokara.Text + "'";
                c1 = new SqlCommand(cmdBarkodMu, s);
                o1 = c1.ExecuteScalar();
                c1 = new SqlCommand(cmdMalkodMu, s);
                o2 = c1.ExecuteScalar();
                c1 = new SqlCommand("select MALAD from stkkrt where MALAD = '" +  txtstokara.Text + "'", s);
                o3 = c1.ExecuteScalar();
            }
            else
            {
                s = new SqlConnection(Carried.girisBaglantiLocal);
                s.Open();
                string cmdBarkodMu = "select BARKOD1 from SMRTAPPSKRT where BARKOD1 = '" +  txtstokara.Text + "'";
                string cmdMalkodMu = "select MALKOD from SMRTAPPSKRT where MALKOD = '" +  txtstokara.Text + "'";
                c1 = new SqlCommand(cmdBarkodMu, s);
                o1 = c1.ExecuteScalar();
                c1 = new SqlCommand(cmdMalkodMu, s);
                o2 = c1.ExecuteScalar();
                c1 = new SqlCommand("select MALAD from SMRTAPPSKRT where MALAD = '" +  txtstokara.Text + "'", s);
                o3 = c1.ExecuteScalar();
            }

            if (o1 != null && !String.IsNullOrEmpty(o1.ToString())) return " where BARKOD1 = '" + o1.ToString() + "'";
            else if (o2 != null && !String.IsNullOrEmpty(o2.ToString())) return " where MALKOD = '" + o2.ToString() + "'";
            else if (o3 != null && !String.IsNullOrEmpty(o3.ToString())) return " where MALAD = '" + o3.ToString() + "'";
            else return null;
        }
        private void lbMalad_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                txtstokara.Clear();
                txtstokara.Text = lbMalad.Items[lbMalad.SelectedIndex].ToString();
                Popup.IsOpen = false;
                lbMalad.Items.Clear();
            }
            catch { return; }
        }
        private void lbMalad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtstokara.Text = lbMalad.Items[lbMalad.SelectedIndex].ToString();
                Popup.IsOpen = false;
                lbMalad.Items.Clear();
            }
        }
        private void txtstokara_KeyDown(object sender, KeyEventArgs e) //entera basınca malad araması yapıyor.
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace( txtstokara.Text))
                {
                    try { MalAdGetir(); }
                    catch (Exception ex)
                    {
                        if (Carried.IsCPMconnected == true)
                        {
                            bool netcon = Carried.CheckForInternetConnection();
                            Carried.IsCPMconnected = netcon == true ? true : false;
                            if (Carried.IsCPMconnected == false) { Carried.showMessage("İnternet bağlantısı bulunamadığı için local bağlantıya geçildi. İşlemi tekrar yapınız"); }
                            else Carried.showMessage(ex.Message);
                        }
                        else Carried.showMessage(ex.Message);
                    }
                }
            }
            if (e.Key == Key.Escape)
            {
                 txtstokara.Clear();
            }
        }
        private void ara_Click(object sender, RoutedEventArgs e) // ara butonuna basınca malad araması yapıyor.Carried.
        {
            if (!string.IsNullOrWhiteSpace( txtstokara.Text))
            {
                try { MalAdGetir(); }
                catch (Exception ex)
                {
                    if (Carried.IsCPMconnected == true)
                    {
                        bool netcon = Carried.CheckForInternetConnection();
                        Carried.IsCPMconnected = netcon == true ? true : false;
                        if (Carried.IsCPMconnected == false) { Carried.showMessage("İnternet bağlantısı bulunamadığı için local bağlantıya geçildi. İşlemi tekrar yapınız"); }
                        else Carried.showMessage(ex.Message);
                    }
                    else Carried.showMessage(ex.Message);
                }
            }
        }
        private void MalAdGetir()
        {
            lbMalad.Items.Clear();
            tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();
            string phrase =  txtstokara.Text.ToString();
            string[] words = phrase.Split(' ');
            string cmdMalAdlari = " select MALAD from " + tabloadi + " where MALAD like '%";
            for (int i = 0; i < words.Length; i++)
            {
                cmdMalAdlari += words[i] + "%'";
                if (i != words.Length - 1) { cmdMalAdlari += " and MALAD like '%"; }
            }
            c1 = new SqlCommand(cmdMalAdlari, s);
            SqlDataReader reader1 = c1.ExecuteReader();
            ArrayList arrMalAdlari = new ArrayList();
            if (reader1 != null)
            {
                try
                {
                    while (reader1.Read())
                    {
                        arrMalAdlari.Add(reader1["MALAD"].ToString());
                    }
                }
                finally { reader1.Close(); }
            }
            if (arrMalAdlari.Count != 0)
            {
                for (int i = 0; i < arrMalAdlari.Count; i++)
                    lbMalad.Items.Add(arrMalAdlari[i]);
                Popup.IsOpen = true;
            }
            else Carried.showMessage("Bu ürün adına sahip bir ürün bulunamadı.");
        }
    }
}


//private int getstokkontrol(string a)
//{
//    if (a == "0") return 0;
//    else /*if (a == "0")*/ return 1;
//}
//private int getdahilharic(string a)
//{
//    if (a.Contains("HARİÇ")) return 0;
//    else return 1;   //DAHİL
//}
//private int getmaltip(string a)
//{
//    if (a.Contains("STOK")) return 0;
//    else if (a.Contains("DEMİRBAŞ")) return 1;
//    else if (a.Contains("HİZMET")) return 2;
//    else return 3;   //YEDEK PARÇA
//}
//private int getkarttip(string a)
//{
//    if (a.Contains("TİCARİ")) return 0;
//    else if (a.Contains("DAHİLİ")) return 1;
//    else if (a.Contains("SERVİS")) return 2;
//    else return 3;   //ÜRETİM
//}
//private void txt_TextChanged(object sender, TextChangedEventArgs e)
//{
//    TextBox textBox = (TextBox)sender;
//    s = new SqlConnection(Carried.girisBaglantiLocal);
//    Duzenle_("SMRTAPPSKRT", s, textBox);

//    if (Carried.IsCPMconnected == true)
//    {
//        s = new SqlConnection(Carried.girisBaglantiCPM);
//        Duzenle_("STKKRT", s, textBox);
//    }
//}


////////s.Open();
////////int numericValue = 0;
////////decimal numericValue2 = 0;
////////Single numericValue3 = 0;
////////bool rakammi = int.TryParse(t.Text, out numericValue);
////////bool decimalmı = decimal.TryParse(t.Text, out numericValue2);
////////bool singlemı = Single.TryParse(t.Text, out numericValue3);
////////if(rakammi==true) c1 = new SqlCommand("update " + tablename + " set " + t.Name + "=" + t.Text + " " + value, s);
////////else if(decimalmı==true) c1 = new SqlCommand("update " + tablename + " set " + t.Name + "=" + Convert.ToDecimal(t.Text.Replace(",",".")) + " " + value, s);
////////else if(singlemı==true) c1 = new SqlCommand("update " + tablename + " set " + t.Name + "=" + Convert.ToSingle(t.Text.Replace(",", ".")) + " " + value, s);
////////else  c1 = new SqlCommand("update " + tablename + " set " + t.Name + "='" + t.Text + "' " + value, s);
////////c1.ExecuteNonQuery();
////////c1= new SqlCommand("update " + tablename + " set DEGISTIRENKULLANICI=@DEGISTIRENKULLANICI, DEGISTIRENTARIH=@DEGISTIRENTARIH, DEGISTIRENSAAT=@DEGISTIRENSAAT, DEGISTIRENKAYNAK=@DEGISTIRENKAYNAK, DEGISTIRENSURUM=@DEGISTIRENSURUM  " + value, s);
////////c1.Parameters.Add(new SqlParameter("@DEGISTIRENKULLANICI", Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30))));
////////c1.Parameters.Add("@DEGISTIRENTARIH", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
////////c1.Parameters.Add("@DEGISTIRENSAAT", SqlDbType.SmallDateTime).Value = DateTime.Now;
////////c1.Parameters.Add(new SqlParameter("@DEGISTIRENKAYNAK", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30))));
////////c1.Parameters.Add(new SqlParameter("@DEGISTIRENSURUM", ""));
////////c1.ExecuteNonQuery();

//////s.Open();
//////DataTable table = new DataTable();
//////SqlDataAdapter a = new SqlDataAdapter("select * FROM " + tablename, s);
//////a.Fill(table);
////////this.stok_dataGrid.ItemsSource = table.DefaultView;
//////Binding bindMyColumn = new Binding();
//////bindMyColumn.Source = table;
//////bindMyColumn.Path = new PropertyPath("Rows[0][MALAD]");
//////txt6.SetBinding(TextBox.TextProperty, bindMyColumn);

////s.Open();
////DataTable table = new DataTable();
////SqlDataAdapter a = new SqlDataAdapter("select * FROM " + tablename, s);
////a.Fill(table);
////DataView dataView = new DataView();
////dataView = table.DefaultView;
////Binding bindMyColumn1 = new Binding();
////bindMyColumn1.Source = dataView;
////bindMyColumn1.Path = new PropertyPath("[0][MALAD]");
////txt6.SetBinding(TextBox.TextProperty, bindMyColumn1);





//private void changeBackground(object sender, MouseEventArgs e)
//{
//    TreeViewItem t = (TreeViewItem)sender;
//    t.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#CCE8FF"));
//    e.Handled = false;
//}

