using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Navigation;
using System.Configuration;
using System.Security.Cryptography;
using BespokeFusion;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Interop;
using System.Xaml;
using System.Data;
using System.Windows.Media.Media3D;
using System.Windows.Controls.Primitives;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Text.RegularExpressions;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Satıs_Islemleri.xaml
    /// </summary>
    public partial class Satıs_Islemleri : Window
    {
        string tabloadi, value;
        SqlConnection s,sl;
        SqlCommand c1;

        public Satıs_Islemleri()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
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

        TabItem tabUserPage;
        Calculator cl;
        private void calculator_button_Click(object sender, RoutedEventArgs e)
        {
            if(sv.Visibility == Visibility.Hidden)
            {
                sv.Visibility = Visibility.Visible;
                Calc.Items.Clear();
                Calc.Visibility = Visibility.Hidden;
                i1.Source =  new BitmapImage(new Uri(@"/icons/hesapmakinesibeyaz.png", UriKind.Relative));
            }
            else
            {
                cl = new Calculator();
                tabUserPage = new TabItem { Content = cl };
                Calc.Items.Add(tabUserPage); 
                Calc.Items.Refresh();
                Calc.Visibility = Visibility.Visible;
                sv.Visibility = Visibility.Hidden;
                i1.Source = new BitmapImage(new Uri(@"\icons\çarpı beyaz.png", UriKind.Relative));
            }
        }

       
        private void txtBrkd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBrkd.Text))
            {
                if (txtBrkd.Text.StartsWith(" ")) { txtBrkd.Clear(); }
                
                try
                {
                    value = BarkodVSMalkodVSMalad();
                }
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
                if (value != null)
                {
                    try
                    {
                        Carried.DosyaDecrypt();
                        string a = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Satis Faturasi Evrak Tipi", "Baglanti.ini");
                        Carried.DosyaEncrypt();
                        if (!string.IsNullOrEmpty(a)) txt5.Text = a;
                        else Carried.showMessage("Parametreler ekranından evrak tipini belirtmeden kayıt işlemi yapamazsınız.");
                        UrunResminiGetir(value);  //resim(value); 
                        DepoVersiyonGetir(value);
                        StokBilgileriGetir(value);
                        EvrakBaslikGetir(value);
                        SatisHareketleriGetir(value);
                    }
                    catch (Exception ex)
                    {
                        if (Carried.IsCPMconnected == true)
                        {
                            bool netcon = Carried.CheckForInternetConnection();
                            Carried.IsCPMconnected = netcon == true ? true : false;
                            if (Carried.IsCPMconnected == false) { Carried.showMessage("İnternet bağlantısı bulunamadığı için local bağlantıya geçildi. İşlemi tekrar yapınız"); }
                            else Carried.showMessage(ex.Message);
                        }
                        if (ex.Message.Contains("String or binary data would be truncated")) Carried.showMessage("Girdiğiniz bazı değerler çok uzun olduğu için işlem yapılamadı.");
                        else Carried.showMessage(ex.Message);
                    }
                }
                else
                {
                    urunresmi.Source = new BitmapImage(new Uri(@"/icons/cpm-logo-yazili-beyaz_c@2x.png", UriKind.Relative));
                    this.depo_dataGrid.ItemsSource = null;
                }
            }
            else urunresmi.Source = new BitmapImage(new Uri(@"/icons/cpm-logo-yazili-beyaz_c@2x.png", UriKind.Relative));
        }
        private string BarkodVSMalkodVSMalad()
        {
            object o1, o2, o3;
            if (Carried.IsCPMconnected == true)
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                s.Open();
                string cmdBarkodMu = "select BARKOD1 from stkkrt where BARKOD1 = '" + txtBrkd.Text + "'";
                string cmdMalkodMu = "select MALKOD from stkkrt where MALKOD = '" + txtBrkd.Text + "'";
                c1 = new SqlCommand(cmdBarkodMu, s);
                o1 = c1.ExecuteScalar();
                c1 = new SqlCommand(cmdMalkodMu, s);
                o2 = c1.ExecuteScalar();
                c1 = new SqlCommand("select MALAD from stkkrt where MALAD = '" + txtBrkd.Text + "'", s);
                o3 = c1.ExecuteScalar();
            }
            else
            {
                s = new SqlConnection(Carried.girisBaglantiLocal);
                s.Open();
                string cmdBarkodMu = "select BARKOD1 from SMRTAPPSKRT where BARKOD1 = '" + txtBrkd.Text + "'";
                string cmdMalkodMu = "select MALKOD from SMRTAPPSKRT where MALKOD = '" + txtBrkd.Text + "'";
                c1 = new SqlCommand(cmdBarkodMu, s);
                o1 = c1.ExecuteScalar();
                c1 = new SqlCommand(cmdMalkodMu, s);
                o2 = c1.ExecuteScalar();
                c1 = new SqlCommand("select MALAD from SMRTAPPSKRT where MALAD = '" + txtBrkd.Text + "'", s);
                o3 = c1.ExecuteScalar();
            }

            if (o1 != null) return " where BARKOD1 = '" + o1.ToString() + "'";
            else if (o2 != null) return " where MALKOD = '" + o2.ToString() + "'";
            else if (o3 != null) return " where MALAD = '" + o3.ToString() + "'";
            else return null;
        }
        private void txtBrkd_KeyDown(object sender, KeyEventArgs e) //entera basınca malad araması yapıyor.
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtBrkd.Text))
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
                txtBrkd.Clear();
            }
        }
        private void ara_Click(object sender, RoutedEventArgs e) // ara butonuna basınca malad araması yapıyor.Carried.
        {
            if (!string.IsNullOrWhiteSpace(txtBrkd.Text))
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
            tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal ;
            s = new SqlConnection(constr);
            s.Open();
            string phrase = txtBrkd.Text.ToString();
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
        private void lbMalad_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtBrkd.Text = lbMalad.Items[lbMalad.SelectedIndex].ToString();
            Popup.IsOpen = false;
            lbMalad.Items.Clear();
        }
        private void lbMalad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtBrkd.Text = lbMalad.Items[lbMalad.SelectedIndex].ToString();
                Popup.IsOpen = false;
                lbMalad.Items.Clear();
            }
        }
        private void UrunResminiGetir(string value)//private ImageSource UrunResminiGetir(string value)
        {
            byte[] imgBytes = { };

            tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
            string tabloadi2 = Carried.IsCPMconnected == true ? "STKRES" : "SMRTAPPSRES";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();

            if (value.Contains("where BARKOD1 =")) value = " where " + tabloadi + ".BARKOD1 = '" + txtBrkd.Text + "'";
            else if (value.Contains("where MALKOD =")) value = " where " + tabloadi + ".MALKOD = '" + txtBrkd.Text + "'";
            else if (value.Contains("where MALAD ="))  value = " where " + tabloadi + ".MALAD = '" + txtBrkd.Text + "'";

            string komut = " select RESIM1 from " + tabloadi2 + " inner join " + tabloadi + " on " + tabloadi + ".MALKOD=" + tabloadi2 + ".MALKOD " + value;
            c1 = new SqlCommand(komut, s);
            SqlDataReader reader1 = c1.ExecuteReader();
            if (reader1 != null)
            {
                while (reader1.Read())
                {
                    imgBytes = (byte[])reader1["RESIM1"];
                }
                reader1.Close();
                if (imgBytes.Length > 0)
                {
                    BitmapImage bmpImage = new BitmapImage();
                    MemoryStream ms = new MemoryStream(imgBytes);
                    bmpImage.BeginInit();
                    bmpImage.StreamSource = new MemoryStream(ms.ToArray());
                    bmpImage.EndInit();
                    urunresmi.Source = bmpImage;
                    //return urunresmi.Source;
                }
                else urunresmi.Source = new BitmapImage(new Uri(@"/icons/cpm-logo-yazili-beyaz_c@2x.png", UriKind.Relative)); /*return null;*/
            }
            //else return null;
        }
        private void DepoVersiyonGetir(string value)
        {
            if (duzenle1.IsChecked == true) duzenle1.IsChecked = false;
            List<string> colsheader = new List<string>();
            foreach (CheckBox cb in grid_vd.Children)
            {
                if (cb.IsChecked == true)
                {
                    switch (cb.Content.ToString())
                    {
                        case "Ürün Kodu": colsheader.Add("MALKOD"); break;
                        case "Ürün Adı": colsheader.Add("MALAD"); break;
                        case "Depo Kodu": colsheader.Add("DEPOKOD"); break;
                        case "Depo Adı": colsheader.Add("DEPOAD"); break;
                        case "Versiyon No": colsheader.Add("VERSIYONNO"); break;
                        case "Seri No": colsheader.Add("SERINO"); break;
                        case "Stok Giriş": colsheader.Add("STOKGIRIS"); break;
                        case "Stok Çıkış": colsheader.Add("STOKCIKIS"); break;
                        case "Bakiye": colsheader.Add("BAKIYE"); break;
                    }
                }
            }

            if (Carried.IsCPMconnected == true)
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                s.Open();
                DataTable table = new DataTable();
                if (value.Contains("where MALKOD ="))
                {
                    value = " where VW_STKDRM.MALKOD = '" + txtBrkd.Text + "'";
                }
                SqlDataAdapter a = new SqlDataAdapter("select VW_STKDRM.MALKOD, STKKRT.MALAD, VW_STKDRM.VERSIYONNO, SERINO, VW_STKDRM.DEPOKOD, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, STOKGIRIS-STOKCIKIS AS BAKIYE FROM VW_STKDRM inner join stkkrt on stkkrt.malkod=VW_STKDRM.malkod inner join DEPKRT on VW_STKDRM.DEPOKOD = DEPKRT.DEPOKOD " + value, s);
                a.Fill(table);

                this.depo_dataGrid.ItemsSource = table.DefaultView;

                foreach (DataGridColumn column in depo_dataGrid.Columns)
                {
                    column.IsReadOnly = true;
                    if (!colsheader.Contains(column.Header.ToString()))
                    {
                        column.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                s = new SqlConnection(Carried.girisBaglantiLocal);
                s.Open();
                DataTable table = new DataTable();
                SqlDataAdapter a = new SqlDataAdapter("select * from smrtappdepo " + value, s);
                a.Fill(table);

                this.depo_dataGrid.ItemsSource = table.DefaultView;

                foreach (DataGridColumn column in depo_dataGrid.Columns)
                {
                    column.IsReadOnly = true;
                    if (!colsheader.Contains(column.Header.ToString()))
                    {
                        column.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        string malkod, malad, birim, stokkontroldurum, markaad, grupkod, mhssatiskod, ilkmalkod;
        private void StokBilgileriGetir(string value)
        {
            tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();

            grid_stok.Children.Clear();
            eklenecekyer.Children.Clear();

            if (btnDuzenle.IsChecked == true) btnDuzenle.IsChecked = false;
            
            //Grid _grid = new Grid();
            //_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            //_grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });

            foreach (CheckBox cb in grid_duzenle.Children)
            {
                if (cb.IsChecked == true)
                {
                    TextBox box = new TextBox() {
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalContentAlignment = HorizontalAlignment.Right,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        FontSize = 20,
                        MinHeight = 40,
                        MaxHeight = 40,
                        Height = 40,
                        //MaxWidth = eklenecekyer.ActualWidth/2,
                        Width = eklenecekyer.ActualWidth / 2,
                        Margin = new Thickness(3, 3, 3, 0),
                        Foreground = Brushes.White,
                        BorderThickness = new Thickness(0.5),
                        BorderBrush = Brushes.Transparent,
                        //IsEnabled = false,
                        Background = new SolidColorBrush(Color.FromArgb(0x66, 0x42, 0x7B, 0x8C))
                    };
                    TextBlock block = new TextBlock() {
                        Text = cb.Content.ToString()+":",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Foreground = Brushes.White,
                        FontSize = 18,
                        Margin = new Thickness(0, 3, 3, 3),
                        TextWrapping = TextWrapping.Wrap
                    };
                    box.SetValue(TextBoxBase.TemplateProperty, GetRoundedTextBoxTemplate());
                    eklenecekyer.Children.Add(block);
                    eklenecekyer.Children.Add(box);
                    switch (cb.Content)
                    {
                        case "Stok Kodu":
                            box.Text = new SqlCommand("select MALKOD from " + tabloadi + value,s).ExecuteScalar().ToString();
                            ilkmalkod = box.Text;
                            break;
                        case "Stok Adı":
                            box.Text = new SqlCommand("select MALAD from " + tabloadi + value, s).ExecuteScalar().ToString();
                            //malad = box.Text;
                            break;
                        case "Birim":
                            box.Text = new SqlCommand("select BIRIM from " + tabloadi + value, s).ExecuteScalar().ToString();
                            //birim = box.Text;
                            break;
                        case "Stok Kontrolü":
                            box.Text = new SqlCommand("select STOKKONTROLDURUM from " + tabloadi + value, s).ExecuteScalar().ToString();
                            //stokkontroldurum = box.Text;
                            break;
                        case "Marka Adı":
                            box.Text = new SqlCommand("select MARKAAD from " + tabloadi + value, s).ExecuteScalar().ToString();
                            //markaad = box.Text;
                            break;
                        case "Grup Kodu":
                            box.Text = new SqlCommand("select GRUPKOD from " + tabloadi + value, s).ExecuteScalar().ToString();
                            //grupkod = box.Text;
                            break;
                        case "Satış Muhasebe Kodu":
                            box.Text = new SqlCommand("select MHSSATISKOD from " + tabloadi + value, s).ExecuteScalar().ToString();
                            //mhssatiskod = box.Text;
                            break;
                        default: break;
                    }
                }
            }
        }
        private void EvrakBaslikGetir(string value)
        {
            DateTime now = DateTime.Now;
            txt7.Text = now.ToString("dd/MM/yyyy");
            txt11.Text = Carried.kasaNo;
            txt12.Text = Carried.girenKullanici;
            s = new SqlConnection(Carried.girisBaglantiLocal);
            s.Open();
            txt10.Text = new SqlCommand("select KASATANIMI from SMRTAPPKASA where KASANO='" + Carried.kasaNo + "'", s).ExecuteScalar().ToString();
            string evraktaki = new SqlCommand("select EVRAKNOTAKISI from SMRTAPPKASA where KASANO='" + Carried.kasaNo + "'", s).ExecuteScalar().ToString();
            int evrakserino = Convert.ToInt32(new SqlCommand("select EVRAKNOSERINO from SMRTAPPKASA where KASANO='" + Carried.kasaNo + "'", s).ExecuteScalar());
            string evrnokisa = evraktaki + evrakserino.ToString();
            txt6.Text = evraktaki + new String('0', (16 - evrnokisa.Length)) + evrakserino;
        }
        private void SatisHareketleriGetir(string value)
        {
            tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();
            sl = new SqlConnection(Carried.girisBaglantiLocal);
            sl.Open();
            //c1 = new SqlCommand("delete", s);
            string s1 = new SqlCommand("select MALKOD FROM " + tabloadi + value, s).ExecuteScalar().ToString();
            string s2 = new SqlCommand("select MALAD FROM " + tabloadi + value, s).ExecuteScalar().ToString();
            string s3 = new SqlCommand("select BIRIM FROM " + tabloadi + value, s).ExecuteScalar().ToString();
            decimal s4 = (decimal)new SqlCommand("select SATISFIYAT1 FROM " + tabloadi + value, s).ExecuteScalar();
            Single s5 = (Single)new SqlCommand("select KDVORAN FROM " + tabloadi + value, s).ExecuteScalar();
            string s6 = new SqlCommand("select DOVIZCINS FROM " + tabloadi + value, s).ExecuteScalar().ToString();
            c1 = new SqlCommand(); 
            c1.CommandText = @"INSERT INTO SATISHAR([MALKOD],[MALAD],[BIRIM],[SATISFIYAT1],[KDVORAN],[DOVIZCINS]) values(@MALKOD, @MALAD, @BIRIM,@SATISFIYAT1,@KDVORAN,@DOVIZCINS)";
            c1.Connection = sl;
            c1.Parameters.Add(new SqlParameter("@MALKOD", s1));
            c1.Parameters.Add(new SqlParameter("@MALAD", s2));
            c1.Parameters.Add(new SqlParameter("@BIRIM", s3));
            c1.Parameters.AddWithValue("@SATISFIYAT1", s4);
            c1.Parameters.AddWithValue("@KDVORAN", s5);
            c1.Parameters.Add(new SqlParameter("@DOVIZCINS", s6));
            c1.ExecuteNonQuery();
            populate();
        }
        private void populate()
        {
            //string constr = Carried.girisBaglantiLocal;
            //s = new SqlConnection(constr);
            //s.Open();
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Seçim", typeof(bool)));
            table.Columns.Add(new DataColumn("Miktar", typeof(string)));
            table.Columns.Add(new DataColumn("İskonto Tutar", typeof(string)));
            table.Columns.Add(new DataColumn("Toplam Tutar", typeof(string)));
            SqlDataAdapter a = new SqlDataAdapter("select MALKOD as 'Malzeme Kodu', MALAD as 'Malzeme Tanımı', BIRIM as Birim, SATISFIYAT1 as 'Birim Fiyatı', KDVORAN as KDV, DOVIZCINS as 'Döviz Cins' FROM SATISHAR", sl);
            a.Fill(table);
            this.stok_dataGrid.ItemsSource = table.DefaultView;
            int x = 0;
            foreach (DataGridColumn column in stok_dataGrid.Columns)
            {
                if (x == 0) { column.DisplayIndex = 0; x++; column.IsReadOnly = false; }
                else if (x == 1) { column.DisplayIndex = 2; x++; column.IsReadOnly = false; }
                else if (x == 2) { column.DisplayIndex = 5; x++; column.IsReadOnly = false; }
                else if (x == 3) { column.DisplayIndex = 7; x++; column.IsReadOnly = true; }
                else column.IsReadOnly = true;
            }
        }


        private void stkYeni_Click(object sender, RoutedEventArgs e)
        {
            StokKartAc w = new StokKartAc();
            w.Show();
            this.Close();
            w.Width = this.ActualWidth;
            w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
        }

        List<string> listetextblocktexts = new List<string>();
        private void StkDuzenle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
                string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
                s = new SqlConnection(constr);
                s.Open();
                //if(String.IsNullOrWhiteSpace(malkod)) { malkod = new SqlCommand("select MALKOD FROM "+ tabloadi + value, s).ExecuteScalar().ToString(); }
                //c1 = new SqlCommand("update " + tabloadi + " set MALKOD='" + malkod + "', MALAD='" + malad + "', BIRIM='" + birim + "', STOKKONTROLDURUM='" + stokkontroldurum + "', MARKAAD='" + markaad + "', GRUPKOD='" + grupkod + "', MHSSATISKOD='" + mhssatiskod + "'" + value, s);
                
                foreach(UIElement item in eklenecekyer.Children)
                {
                    if (item.GetType().Equals(typeof(TextBlock)))
                    {
                        TextBlock tb = (TextBlock)item;
                        listetextblocktexts.Add(tb.Text);
                    }
                }
                int i = 0;
                foreach (UIElement item in eklenecekyer.Children)
                {
                    if (item.GetType().Equals(typeof(TextBox)))
                    {
                        TextBox tbox = (TextBox)item;
                        switch (listetextblocktexts[i])
                        {
                            case "Stok Kodu":
                                malkod = tbox.Text;
                                break;
                            case "Stok Adı":
                                malad = tbox.Text;
                                break;
                            case "Birim":
                                birim = tbox.Text;
                                break;
                            case "Stok Kontrolü":
                                stokkontroldurum = tbox.Text;
                                break;
                            case "Marka Adı":
                                markaad = tbox.Text;
                                break;
                            case "Grup Kodu":
                                grupkod = tbox.Text;
                                break;
                            case "Satış Muhasebe Kodu":
                                mhssatiskod = tbox.Text;
                                break;
                            default: break;
                        }
                        i++;
                    }               
                }
                string cmmd = "update " + tabloadi + " set ";
                if (!String.IsNullOrWhiteSpace(malkod)) { cmmd += " MALKOD = '" + malkod + "',"; }
                if (!String.IsNullOrWhiteSpace(malad)) { cmmd += " MALAD = '" + malad + "',"; }
                if (!String.IsNullOrWhiteSpace(birim)) { cmmd += "  BIRIM = '" + birim + "',"; }
                if (!String.IsNullOrWhiteSpace(stokkontroldurum)) { cmmd += " STOKKONTROLDURUM ='" + stokkontroldurum + "',"; }
                if (!String.IsNullOrWhiteSpace(markaad)) { cmmd += " MARKAAD = '" + markaad + "',"; }
                if (!String.IsNullOrWhiteSpace(grupkod)) { cmmd += " GRUPKOD = '" + grupkod + "',"; }
                if (!String.IsNullOrWhiteSpace(mhssatiskod)) { cmmd += " MHSSATISKOD = '" + mhssatiskod + "',"; }
                cmmd += " DEGISTIRENTARIH = '" + DateTime.Now.Date + "',";
                cmmd += " DEGISTIRENSAAT = '" + DateTime.Now + "',";
                cmmd += " DEGISTIRENKULLANICI = '" + Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30)) + "',";
                cmmd += " DEGISTIRENKAYNAK = '" + System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30)) + "' ";
                c1 = new SqlCommand(cmmd + " where MALKOD = '" + ilkmalkod + "'", s);
                c1.ExecuteNonQuery();
                Carried.showMessage("Başarıyla güncellendi.");
            }
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


        private void txt1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
                string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
                string phrase = txt1.Text.ToString();
                string[] words = phrase.Split(' ');
                string cmdUnvanlar = " select UNVAN from " + tabloadi + " where UNVAN like '%";
                for (int i = 0; i < words.Length; i++)
                {
                    cmdUnvanlar += words[i] + "%'";
                    if (i != words.Length - 1) { cmdUnvanlar += " and UNVAN like '%"; }
                }
                try
                {
                    s = new SqlConnection(constr);
                    s.Open();
                    c1 = new SqlCommand(cmdUnvanlar, s);
                    SqlDataReader reader1 = c1.ExecuteReader();
                    ArrayList arrUnvanlar = new ArrayList();
                    if (reader1 != null)
                    {
                        try
                        {
                            while (reader1.Read())
                            {
                                arrUnvanlar.Add(reader1["UNVAN"].ToString());
                            }
                        }
                        finally { reader1.Close(); }
                    }
                    if (arrUnvanlar.Count != 0)
                    {
                        for (int i = 0; i < arrUnvanlar.Count; i++)
                            lbUnvan.Items.Add(arrUnvanlar[i]);
                        PopupUnvan.IsOpen = true;
                    }
                    else Carried.showMessage("Bu unvan mevcut değil.");
                }
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
        private void lbUnvan_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txt1.Text = lbUnvan.Items[lbUnvan.SelectedIndex].ToString();
            PopupUnvan.IsOpen = false;
            lbUnvan.Items.Clear();
            try
            {
                c1 = new SqlCommand("select HESAPKOD from " + tabloadi + " where UNVAN ='" + txt1.Text + "'", s);
                object o = c1.ExecuteScalar();
                c1 = new SqlCommand("select VERGIHESAPNO from " + tabloadi + " where UNVAN ='" + txt1.Text + "'", s);
                object o1 = c1.ExecuteScalar();
                c1 = new SqlCommand("select DOVIZCINS from " + tabloadi + " where UNVAN ='" + txt1.Text + "'", s);
                object o2 = c1.ExecuteScalar();
                if (o != null )
                {
                    txt2.Text = o.ToString();
                    txt3.Text = o1.ToString();
                    txt9.Text = o2.ToString();
                    if (string.IsNullOrEmpty(txt9.Text)) txt9.Text = "TL";
                    if (!string.IsNullOrWhiteSpace(txt3.Text))
                    {
                        c1 = new SqlCommand("select EFATURADURUM from " + tabloadi + " where VERGIHESAPNO ='" + txt3.Text + "'", s);
                        if(Convert.ToInt32(c1.ExecuteScalar().ToString()) == 0)
                        {
                            efat.Visibility = Visibility.Hidden;
                            ears.Visibility = Visibility.Visible;
                            Earsiv(); EIrsaliye();
                        }
                        else //1 yani efatura ise
                        {
                            efat.Visibility = Visibility.Visible;
                            ears.Visibility = Visibility.Hidden;
                            Efatura(); EIrsaliye();
                        }
                    }
                }
            }
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
        private void lbUnvan_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.Key == Key.Enter))
            {
                txt1.Text = lbUnvan.Items[lbUnvan.SelectedIndex].ToString();
                PopupUnvan.IsOpen = false;
                lbUnvan.Items.Clear();
                try
                {
                    c1 = new SqlCommand("select HESAPKOD from " + tabloadi + " where UNVAN ='" + txt1.Text + "'", s);
                    object o = c1.ExecuteScalar();
                    c1 = new SqlCommand("select VERGIHESAPNO from " + tabloadi + " where UNVAN ='" + txt1.Text + "'", s);
                    object o1 = c1.ExecuteScalar();
                    c1 = new SqlCommand("select DOVIZCINS from " + tabloadi + " where UNVAN ='" + txt1.Text + "'", s);
                    object o2 = c1.ExecuteScalar();
                    if (o != null)
                    {
                        txt2.Text = o.ToString();
                        txt3.Text = o1.ToString();
                        txt9.Text = o2.ToString();
                        if (string.IsNullOrEmpty(txt9.Text)) txt9.Text = "TL";
                        if (!string.IsNullOrWhiteSpace(txt3.Text))
                        {
                            c1 = new SqlCommand("select EFATURADURUM from " + tabloadi + " where VERGIHESAPNO ='" + txt3.Text + "'", s);
                            if (Convert.ToInt32(c1.ExecuteScalar().ToString()) == 0)
                            {
                                efat.Visibility = Visibility.Hidden;
                                ears.Visibility = Visibility.Visible;
                                Earsiv(); EIrsaliye();
                            }
                            else //1 yani efatura ise
                            {
                                efat.Visibility = Visibility.Visible;
                                ears.Visibility = Visibility.Hidden;
                                Efatura(); EIrsaliye();
                            }
                        }
                    }
                }
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
        private void txt2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
                string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
                try
                {
                    s = new SqlConnection(constr);
                    s.Open();
                    c1 = new SqlCommand("select VERGIHESAPNO from " + tabloadi + " where HESAPKOD ='" + txt2.Text + "'", s);
                    object o = c1.ExecuteScalar();
                    c1 = new SqlCommand("select UNVAN from " + tabloadi + " where HESAPKOD ='" + txt2.Text + "'", s);
                    object o1 = c1.ExecuteScalar();
                    c1 = new SqlCommand("select DOVIZCINS from " + tabloadi + " where HESAPKOD ='" + txt2.Text + "'", s);
                    object o2 = c1.ExecuteScalar();
                    if (o != null)
                    {
                        txt3.Text = o.ToString();
                        txt1.Text = o1.ToString();
                        txt9.Text = o2.ToString();
                        if (string.IsNullOrEmpty(txt9.Text)) txt9.Text = "TL";
                        if (!string.IsNullOrWhiteSpace(txt3.Text))
                        {
                            c1 = new SqlCommand("select EFATURADURUM from " + tabloadi + " where VERGIHESAPNO ='" + txt3.Text + "'", s);
                            if (Convert.ToInt32(c1.ExecuteScalar().ToString()) == 0)
                            {
                                efat.Visibility = Visibility.Hidden;
                                ears.Visibility = Visibility.Visible;
                                Earsiv(); EIrsaliye();
                            }
                            else //1 yani efatura ise
                            {
                                efat.Visibility = Visibility.Visible;
                                ears.Visibility = Visibility.Hidden;
                                Efatura(); EIrsaliye();
                            }
                        }
                    }
                    else Carried.showMessage("Bu cari kodu mevcut değil.");
                }
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
        //private void lbCarKod_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    txt2.Text = lbCarKod.Items[lbCarKod.SelectedIndex].ToString();
        //    PopupCarKod.IsOpen = false;
        //    lbCarKod.Items.Clear();
        //}
        //private void lbCarKod_KeyDown(object sender, KeyEventArgs e)
        //{
        //    txt2.Text = lbCarKod.Items[lbCarKod.SelectedIndex].ToString();
        //    PopupCarKod.IsOpen = false;
        //    lbCarKod.Items.Clear();
        //}
        private void txt3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if((txt3.Text.Length == 10 && isVergiNoValid(txt3.Text) == true) || (txt3.Text.Length == 11 && isKimlikNoValid(txt3.Text)))
                {
                    tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
                    string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
                    try
                    {
                        s = new SqlConnection(constr);
                        s.Open();
                        c1 = new SqlCommand("select HESAPKOD from " + tabloadi + " where VERGIHESAPNO ='" + txt3.Text + "'", s);
                        object o = c1.ExecuteScalar();
                        c1 = new SqlCommand("select UNVAN from " + tabloadi + " where VERGIHESAPNO ='" + txt3.Text + "'", s);
                        object o1 = c1.ExecuteScalar();
                        c1 = new SqlCommand("select DOVIZCINS from " + tabloadi + " where VERGIHESAPNO ='" + txt2.Text + "'", s);
                        object o2 = c1.ExecuteScalar();
                        if (o != null)
                        {
                            txt2.Text = o.ToString();
                            txt1.Text = o1.ToString();
                            txt9.Text = o2.ToString();
                            if (string.IsNullOrEmpty(txt9.Text)) txt9.Text = "TL";
                            if (!string.IsNullOrWhiteSpace(txt3.Text))
                            {
                                c1 = new SqlCommand("select EFATURADURUM from " + tabloadi + " where VERGIHESAPNO ='" + txt3.Text + "'", s);
                                if (Convert.ToInt32(c1.ExecuteScalar().ToString()) == 0)
                                {
                                    efat.Visibility = Visibility.Hidden;
                                    ears.Visibility = Visibility.Visible;
                                    Earsiv(); EIrsaliye();
                                }
                                else //1 yani efatura ise
                                {
                                    efat.Visibility = Visibility.Visible;
                                    ears.Visibility = Visibility.Hidden;
                                    Efatura(); EIrsaliye();
                                }
                            }
                        }
                        else
                        {
                            CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                            {
                                TxtMessage = { Text = "Vergi no bulunamamıştır. Yeni cari kart açmak ister misiniz?",
                                TextAlignment=TextAlignment.Center,
                                VerticalAlignment=VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, TextWrapping = TextWrapping.Wrap ,
                                Foreground = Brushes.Black , FontFamily = new FontFamily("Arial"), FontSize = 25},
                                TxtTitle = { Text = "UYARI", Foreground = Brushes.White },
                                MainContentControl = { Background = Brushes.Turquoise },
                                TitleBackgroundPanel = { Background = Brushes.Blue },
                                BorderBrush = Brushes.Blue,
                                BtnCancel = { Content = "HAYIR", Background = Brushes.Red, IsCancel = true },
                                BtnOk = { Content = "EVET", Background = Brushes.Green }
                            };
                            msg.Show();
                            MessageBoxResult results = msg.Result;
                            if (results == MessageBoxResult.OK)
                            {
                                Carried.DosyaDecrypt();
                                string hesapno = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Hesap No Ayari", "Baglanti.ini");
                                Carried.DosyaEncrypt();
                                if(!String.IsNullOrWhiteSpace(hesapno))
                                {
                                    CariKartAc w = new CariKartAc(txt3.Text, hesapno);
                                    w.Show();
                                    this.Close();
                                    //w.Width = this.ActualWidth;
                                    //w.Height = this.ActualHeight;
                                    w.WindowState = this.WindowState;
                                    w.VerticalAlignment = this.VerticalAlignment;
                                    w.HorizontalAlignment = this.HorizontalAlignment;
                                }
                                else Carried.showMessage("Parametreler kısmından hesap no tanımlanmadığı için bu işlem yapılamaz.");
                            }
                            else if (results == MessageBoxResult.Cancel) msg.Close();
                        }
                    }
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
                else Carried.showMessage("Vergi no ya hatalı girilmiştir ya da geçerli değildir.");
            }
        }
        public static bool isVergiNoValid(string no)
        {
            bool isValid = false;
            int numericValue, islemsonucu, toplam = 0, rakamdegilmi = 0;
            for (int i = 0; i < no.Length - 1; i++)
            {
                bool rakammi = int.TryParse(no[i].ToString(), out numericValue);
                if (rakammi == true)
                {
                    islemsonucu = (numericValue + 10 - (i + 1)) % 10;
                    if (islemsonucu != 9)
                        islemsonucu = (islemsonucu * (int)Math.Pow(2, (10 - (i + 1)))) % 9;
                    toplam += islemsonucu;
                }
                else { rakamdegilmi = 1; break; }
            }
            if(rakamdegilmi == 0)
            {
                islemsonucu = (10 - (toplam % 10)) % 10;
                if (Convert.ToInt32(no[no.Length - 1].ToString()) == islemsonucu) 
                    isValid = true;
            }
            return isValid;
        }
        public static bool isKimlikNoValid(string no)
        {
            bool isValid1 = false, isValid2 = false;
            int numericValue, tektoplam = 0, cifttoplam = 0, toplam = 0, rakamdegilmi = 0;
            for (int i = 0; i < no.Length - 1; i++)
            {
                bool rakammi = int.TryParse(no[i].ToString(), out numericValue);
                if (rakammi == true)
                {
                    toplam += numericValue;
                    if ((i + 1) % 2 != 0) tektoplam += numericValue;
                    else if ((i + 1) % 2 == 0 && (i + 1) != 10) cifttoplam += numericValue;
                }
                else { rakamdegilmi = 1; break; }
            }
            if (rakamdegilmi == 0)
            {
                if (Convert.ToInt32(no[no.Length - 1].ToString()) == toplam % 10)
                    isValid1 = true;
                if (Convert.ToInt32(no[no.Length - 2].ToString()) == (tektoplam * 7 - cifttoplam) % 10)
                    isValid2 = true;
            }
            return isValid1 && isValid2;
        }
        private void txt8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txt8.Text.StartsWith('*'))
                {
                    string stringNumber = txt8.Text.Remove(0, 1);
                    int numericValue;
                    bool isNumber = int.TryParse(stringNumber, out numericValue); //stringnumber sayı ise isNumber true ve numericvalue o sayı, değilse isNumber false ve numericvalue daima 0.
                    if (numericValue != 0) //*dan sonrası int değil değilse (yani int ise)
                    {
                        DateTime now = DateTime.Now;
                        try { txt8.Text = now.AddDays(numericValue).ToString(); }//toshorttimestring vb. de oluyor.
                        catch { Carried.showMessage("Girdiğiniz sayı çok büyük."); }
                    }
                }
            }
        }
        private void txt8_LostFocus(object sender, RoutedEventArgs e)
        {
            if (dp.IsFocused == false && dp.IsDropDownOpen == false) { cbdate.IsDropDownOpen = false; }
        }
        private void txt8_GotFocus(object sender, RoutedEventArgs e)
        {
            cbdate.IsDropDownOpen = true;
            cbdate.SelectedItem = null;
        }
        private void selected_date_changed(object sender, SelectionChangedEventArgs e)
        {
            txt8.Text = dp.SelectedDate.ToString();
        }
        private void Earsiv()
        {
            grid_ea.Children.Clear();
            tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            List<string> arrEmailler = new List<string>();
            try
            {
                if (!string.IsNullOrWhiteSpace(txt3.Text))
                {
                    s = new SqlConnection(constr);
                    s.Open();
                    c1 = new SqlCommand("SELECT EMAIL1, EMAIL2,EMAIL3,EMAIL4,EMAIL5 FROM " + tabloadi + " WHERE EFATURADURUM=0 and VERGIHESAPNO='" + txt3.Text + "'", s);
                    SqlDataReader reader = c1.ExecuteReader();
                    try
                    {
                        int count = reader.FieldCount;
                        while (reader.Read())
                        {
                            for (int i = 0; i < count; i++)
                            {
                                arrEmailler.Add(reader.GetValue(i).ToString());
                            }
                        }
                    }
                    finally { reader.Close(); }
                    arrEmailler.RemoveAll(x => string.IsNullOrEmpty(x));
                    for (int i = 0; i < arrEmailler.Count; i++)
                    {
                        RadioButton rb = new RadioButton() { Content = arrEmailler[i].ToString(), Foreground = Brushes.White, FontSize = 16 };
                        grid_ea.Children.Add(rb);
                        if (i == 0) { rb.IsChecked = true; }
                    }
                }
            }
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
        private void EIrsaliye()
        {
            grid_ei.Children.Clear();
            //if (efat.Visibility == Visibility.Visible)
            //{
            tabloadi = Carried.IsCPMconnected == true ? "EFAKUL" : "SMRTAPPETIKET";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            ArrayList arrEtiketler = new ArrayList();
            try
            {
                if (!string.IsNullOrWhiteSpace(txt3.Text))
                {
                    s = new SqlConnection(constr);
                    s.Open();
                    c1 = new SqlCommand("SELECT ETIKET FROM " + tabloadi + " WHERE ETIKETTIP=1 AND VERGIHESAPNO='" + txt3.Text + "'", s);
                    SqlDataReader reader1 = c1.ExecuteReader();
                    try
                    {
                        while (reader1.Read())
                        {
                            arrEtiketler.Add(reader1["ETIKET"].ToString());
                        }
                    }
                    finally { reader1.Close(); }
                    if (arrEtiketler.Count == 0)
                    {
                        eirs.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        eirs.Visibility = Visibility.Visible;
                        for (int i = 0; i < arrEtiketler.Count; i++)
                        {
                            RadioButton rb = new RadioButton() { Content = arrEtiketler[i].ToString(), Foreground = Brushes.White, FontSize = 16 };
                            grid_ei.Children.Add(rb);
                            if (i == 0) { rb.IsChecked = true; }
                        }
                    }
                }
            }
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
            //}
            //else
            //{
            //tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
            //string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            //List<string> arrEmailler = new List<string>();
            //try
            //{
            //    if (!string.IsNullOrWhiteSpace(txt3.Text))
            //    {
            //        s = new SqlConnection(constr);
            //        s.Open();
            //        c1 = new SqlCommand("SELECT EMAIL1, EMAIL2,EMAIL3,EMAIL4,EMAIL5 FROM " + tabloadi + " WHERE EFATURADURUM=0 VERGIHESAPNO='" + txt3.Text + "'", s);
            //        SqlDataReader reader = c1.ExecuteReader();
            //        try
            //        {
            //            int count = reader.FieldCount;
            //            while (reader.Read())
            //            {
            //                for (int i = 0; i < count; i++)
            //                {
            //                    arrEmailler.Add(reader.GetValue(i).ToString());
            //                }
            //            }
            //        }
            //        finally { reader.Close(); }
            //        arrEmailler.RemoveAll(x => string.IsNullOrEmpty(x));
            //        for (int i = 0; i < arrEmailler.Count; i++)
            //        {
            //            RadioButton rb = new RadioButton() { Content = arrEmailler[i].ToString(), Foreground = Brushes.White, FontSize = 16 };
            //            grid_ei.Children.Add(rb);
            //            if (i == 0) { rb.IsChecked = true; }
            //        }
            //    }
            //}
            //catch
            //{
            //    bool netcon = Carried.CheckForInternetConnection();
            //    Carried.IsCPMconnected = netcon == true ? true : false;
            //    Carried.showMessage("İnternet bağlantısı bulunamadığı için local bağlantıya geçildi.");
            //    txtBrkd.Clear();
            //}
            //}
        }
        private void Efatura()
        {
            grid_ef.Children.Clear();
            tabloadi = Carried.IsCPMconnected == true ? "EFAKUL" : "SMRTAPPETIKET";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            ArrayList arrEtiketler = new ArrayList();
            try
            {
                if (!string.IsNullOrWhiteSpace(txt3.Text))
                {
                    s = new SqlConnection(constr);
                    s.Open();
                    c1 = new SqlCommand("SELECT ETIKET FROM " + tabloadi + " WHERE ETIKETTIP=0 AND VERGIHESAPNO='" + txt3.Text + "'", s);
                    SqlDataReader reader1 = c1.ExecuteReader();
                    try
                    {
                        while (reader1.Read())
                        {
                            arrEtiketler.Add(reader1["ETIKET"].ToString());
                        }
                    }
                    finally { reader1.Close(); }
                    for (int i = 0; i < arrEtiketler.Count; i++)
                    {
                        RadioButton rb = new RadioButton() { Content = arrEtiketler[i].ToString(), Foreground = Brushes.White, FontSize = 16 };
                        grid_ef.Children.Add(rb);
                        if (i == 0) { rb.IsChecked = true; }
                    }

                }
            }
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


        private void duzenle1_Checked(object sender, RoutedEventArgs e)
        {
            depo_dataGrid.Visibility = Visibility.Hidden;
        }
        private void duzenle1_Unchecked(object sender, RoutedEventArgs e)
        {
            depo_dataGrid.Visibility = Visibility.Visible;
        }
        private void btnDuzenle_Unchecked(object sender, RoutedEventArgs e)
        {
            sv_stok.Visibility = Visibility.Visible;
            //btnDuzenle.Content = "DÜZENLE";
        }
        private void btnDuzenle_Checked(object sender, RoutedEventArgs e)
        {
            sv_stok.Visibility = Visibility.Hidden;
            //btnDuzenle.Content = "KAPAT";
        }
        private void efat_Checked(object sender, RoutedEventArgs e)
        {
            eirs.IsChecked = false;
            ears.IsChecked = false;
            r1.Height = new GridLength(0);
            r2.Height = new GridLength(0);
            r3.Height = new GridLength(0);
            r4.Height = new GridLength(0);
        }
        private void eirs_Checked(object sender, RoutedEventArgs e)
        {
            efat.IsChecked = false;
            ears.IsChecked = false;
            r1.Height = new GridLength(0);
            r2.Height = new GridLength(0);
            r3.Height = new GridLength(0);
            r4.Height = new GridLength(0);
        }
        private void ears_Checked(object sender, RoutedEventArgs e)
        {
            eirs.IsChecked = false;
            efat.IsChecked = false;
            r1.Height = new GridLength(0);
            r2.Height = new GridLength(0);
            r3.Height = new GridLength(0);
            r4.Height = new GridLength(0);
        }
        private void efat_Unchecked(object sender, RoutedEventArgs e)
        {
            r1.Height = new GridLength(1, GridUnitType.Star);
            r2.Height = new GridLength(1, GridUnitType.Star);
            r3.Height = new GridLength(1, GridUnitType.Star);
            r4.Height = new GridLength(1, GridUnitType.Star);
        }
        private void eirs_Unchecked(object sender, RoutedEventArgs e)
        {
            r1.Height = new GridLength(1, GridUnitType.Star);
            r2.Height = new GridLength(1, GridUnitType.Star);
            r3.Height = new GridLength(1, GridUnitType.Star);
            r4.Height = new GridLength(1, GridUnitType.Star);
        }
        private void ears_Unchecked(object sender, RoutedEventArgs e)
        {
            r1.Height = new GridLength(1, GridUnitType.Star);
            r2.Height = new GridLength(1, GridUnitType.Star);
            r3.Height = new GridLength(1, GridUnitType.Star);
            r4.Height = new GridLength(1, GridUnitType.Star);
        }

        
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }


        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            s = new SqlConnection(Carried.girisBaglantiLocal);
            s.Open();
            int evrakserino = Convert.ToInt32(new SqlCommand("select EVRAKNOSERINO from SMRTAPPKASA where KASANO='" + Carried.kasaNo + "'", s).ExecuteScalar());
            c1 = new SqlCommand("update SMRTAPPKASA set EVRAKNOSERINO='" + (evrakserino + 1).ToString() + "' where KASANO='" + Carried.kasaNo + "'", s);
            c1.ExecuteNonQuery();

            
        }


        private void evrakara_Click(object sender, RoutedEventArgs e)
        {
            if (txtevrakkod.Visibility == Visibility.Hidden) txtevrakkod.Visibility = Visibility.Visible;
            else if (txtevrakkod.Visibility == Visibility.Visible) txtevrakkod.Visibility = Visibility.Hidden;
        }




        private void txtmiktar_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private ControlTemplate GetRoundedTextBoxTemplate()
        {
            ControlTemplate template = new ControlTemplate(typeof(TextBoxBase));
            FrameworkElementFactory elemFactory = new FrameworkElementFactory(typeof(Border));
            elemFactory.Name = "Border";
            elemFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            elemFactory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
            elemFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(TextBox.BackgroundProperty));
            elemFactory.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(TextBox.BorderThicknessProperty));
            elemFactory.SetValue(Border.SnapsToDevicePixelsProperty, true);
            template.VisualTree = elemFactory;

            FrameworkElementFactory scrollViewerElementFactory = new FrameworkElementFactory(typeof(ScrollViewer));
            scrollViewerElementFactory.Name = "PART_ContentHost";
            elemFactory.AppendChild(scrollViewerElementFactory);


            Trigger IsMouseOverTrigger = new Trigger();
            IsMouseOverTrigger.Property = TextBox.IsMouseOverProperty;
            IsMouseOverTrigger.Value = true;

            Setter borderbrushSetter = new Setter();
            borderbrushSetter.Property = TextBox.BorderBrushProperty;
            borderbrushSetter.Value = new SolidColorBrush(Color.FromArgb(0xff, 0x7e, 0xb4, 0xea));

            Setter borderthicknessSetter = new Setter();
            borderthicknessSetter.Property = TextBox.BorderThicknessProperty;
            borderthicknessSetter.Value = new Thickness(1);

            //Setter backgroundSetter = new Setter();
            //backgroundSetter.TargetName = "Border";
            //backgroundSetter.Property = TextBox.BackgroundProperty;
            //backgroundSetter.Value = SystemColors.ControlBrush;

            //Setter foregroundSetter = new Setter();
            //foregroundSetter.Property = TextBox.ForegroundProperty;
            //foregroundSetter.Value = SystemColors.GrayTextBrush;

            //IsMouseOverTrigger.Setters.Add(backgroundSetter);
            //IsMouseOverTrigger.Setters.Add(foregroundSetter);
            IsMouseOverTrigger.Setters.Add(borderbrushSetter);
            IsMouseOverTrigger.Setters.Add(borderthicknessSetter);

            template.Triggers.Add(IsMouseOverTrigger);
            //Style style = new Style(typeof(TextBox));
            //style.Triggers.Add(IsMouseOverTrigger);
            return template;
        }

        #region 3D
        // The camera.
        private PerspectiveCamera TheCamera = null;
        // The camera controller.
        private SphericalCameraController CameraController = null;
        private void resim(string value)
        {
            //ImageSource a = UrunResminiGetir(value);
            ModelVisual3D visual3d = new ModelVisual3D();
            Model3DGroup group = new Model3DGroup();
            visual3d.Content = group;
            mainViewport.Children.Add(visual3d);

            DefineCamera(mainViewport);

            MeshGeometry3D brickMesh = new MeshGeometry3D();
            AddRectangle(brickMesh,
                new Point3D(-1, 2, -1),
                new Point3D(-1, 0, -1),
                new Point3D(+1, 0, -1),
                new Point3D(+1, 2, -1));
            ImageBrush brickBrush = new ImageBrush();
            ////////brickBrush.ImageSource = new BitmapImage(new Uri("70d73c59-4caf-4808-82f6-08d114c1ec15-removebg-preview.png", UriKind.Relative));
            //if (a != null) brickBrush.ImageSource = a;
             Material brickMaterial = new DiffuseMaterial(brickBrush);
            GeometryModel3D brickModel = new GeometryModel3D(brickMesh, brickMaterial);
            group.Children.Add(brickModel);
        }
        // Define the camera.
        private void DefineCamera(Viewport3D viewport)
        {
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            CameraController = new SphericalCameraController(TheCamera, viewport, this, mainGrid, mainGrid);
        }
        private void AddRectangle(MeshGeometry3D mesh, Point3D p1, Point3D p2, Point3D p3, Point3D p4)
        {
            int index = mesh.Positions.Count;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            mesh.Positions.Add(p4);

            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 1);
            mesh.TriangleIndices.Add(index + 2);

            mesh.TriangleIndices.Add(index);
            mesh.TriangleIndices.Add(index + 2);
            mesh.TriangleIndices.Add(index + 3);
        }

        #endregion

        
    }
}