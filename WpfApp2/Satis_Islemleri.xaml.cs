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
//using BespokeFusion;
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
using System.Globalization;
using Microsoft.Win32;
using System.Xml.Xsl;
using System.Xml;
using System.Xml.XPath;
using javax.swing.text.html;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB;
using Gat.Controls;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Satıs_Islemleri.xaml
    /// </summary>
    public partial class Satıs_Islemleri : Window
    {
        string tabloadi, value;
        SqlConnection s, sl;
        SqlCommand c1;
        string EvrakTip;
        public Satıs_Islemleri(string evraktip)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            EvrakTip = evraktip;
        }

       
        private void txtBrkd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBrkd.Text))
            {
                if (txtBrkd.Text.StartsWith(" ")) { txtBrkd.Clear(); }

                try
                {
                    value = BarkodVSMalkodVSMalad();
                    if (value != null)
                    {
                        Carried.DosyaDecrypt();
                        string evtip = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Satis Faturasi Evrak Tipi", "Baglanti.ini");
                        Carried.DosyaEncrypt();
                        if (!string.IsNullOrEmpty(evtip)) txt5.Text = evtip;
                        else { Carried.showMessage("Parametreler ekranından evrak tipini belirtmeden kayıt işlemi yapamazsınız."); return; }
                        UrunResminiGetir(value);  //resim(value); 
                        DepoVersiyonGetir(value);
                        StokBilgileriGetir(value);
                        EvrakBaslikGetir(value);
                        SatisHareketleriGetir(value);
                        txtBrkd.Clear();
                    }
                    else
                    {
                        urunresmi.Source = new BitmapImage(new Uri(@"/icons/cpm-logo-yazili-beyaz_c@2x.png", UriKind.Relative));
                        this.depo_dataGrid.ItemsSource = null;
                        foreach (UIElement item in eklenecekyer.Children)
                        {
                            if (item.GetType().Equals(typeof(TextBox)))
                            {
                                TextBox tbox = (TextBox)item;
                                tbox.Clear();
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

            if (o1 != null && !String.IsNullOrEmpty(o1.ToString())) return " where BARKOD1 = '" + o1.ToString() + "'";
            else if (o2 != null && !String.IsNullOrEmpty(o2.ToString())) return " where MALKOD = '" + o2.ToString() + "'";
            else if (o3 != null && !String.IsNullOrEmpty(o3.ToString())) return " where MALAD = '" + o3.ToString() + "'";
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
            lbMalad.Items.Clear();
            tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
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
            try
            {
                txtBrkd.Clear();
                txtBrkd.Text = lbMalad.Items[lbMalad.SelectedIndex].ToString();
                Popup.IsOpen = false;
                lbMalad.Items.Clear();
            }
            catch { return; }
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
            else if (value.Contains("where MALAD =")) value = " where " + tabloadi + ".MALAD = '" + txtBrkd.Text + "'";

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
                }
                else urunresmi.Source = new BitmapImage(new Uri(@"/icons/cpm-logo-yazili-beyaz_c@2x.png", UriKind.Relative)); 
            }
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
                SqlDataAdapter a = new SqlDataAdapter("select VW_STKDRM.MALKOD, STKKRT.MALAD, STKKRT.BARKOD1, VW_STKDRM.VERSIYONNO, SERINO, VW_STKDRM.DEPOKOD, DEPKRT.DEPOAD, STOKGIRIS, STOKCIKIS, STOKGIRIS-STOKCIKIS AS BAKIYE FROM VW_STKDRM inner join stkkrt on stkkrt.malkod=VW_STKDRM.malkod inner join DEPKRT on VW_STKDRM.DEPOKOD = DEPKRT.DEPOKOD " + value, s);
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
        string stokkontrol;
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
                    TextBox box = new TextBox()
                    {
                        TextAlignment = System.Windows.TextAlignment.Left,
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalContentAlignment = HorizontalAlignment.Right,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        FontSize = 15,
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
                    TextBlock block = new TextBlock()
                    {
                        Text = cb.Content.ToString() + ":",
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
                    stokkontrol = new SqlCommand("select STOKKONTROLDURUM from " + tabloadi + value, s).ExecuteScalar().ToString();
                    if(Convert.ToInt16(stokkontrol) == 0 && depo_dataGrid.Items.Count != 0) //stokkontroldurum 0 ise bakiyenin eksiye düşmesine izin yok, 1 ise var. (tabii depo varsa)
                    {
                        for (int j = 0; j < depo_dataGrid.Items.Count; j++)
                        {
                            DataRowView rowView = depo_dataGrid.Items[j] as DataRowView;
                            if (Convert.ToDecimal(rowView[9]) < 0)
                            {
                                rowView.BeginEdit();
                                rowView[9] = "0";// Convert.ToDecimal(0);
                                rowView.EndEdit();
                                depo_dataGrid.Items.Refresh();
                            }
                        }
                    }
                    switch (cb.Content)
                    {
                        case "Stok Kodu":
                            box.Text = new SqlCommand("select MALKOD from " + tabloadi + value, s).ExecuteScalar().ToString();
                            ilkmalkod = box.Text;
                            box.IsEnabled = false;
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
                            box.Text = stokkontrol;
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
        List<string> satısharmalkodlari = new List<string>();
        //int kdvdh;
        private void SatisHareketleriGetir(string value)
        {
            if (txtmiktar.Text == "0") return;
            tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();
            sl = new SqlConnection(Carried.girisBaglantiLocal);
            sl.Open();
            string s1 = new SqlCommand("select MALKOD FROM " + tabloadi + value, s).ExecuteScalar().ToString();
            if (!satısharmalkodlari.Contains(s1))
            {
                satısharmalkodlari.Add(s1);
                string s2 = new SqlCommand("select MALAD FROM " + tabloadi + value, s).ExecuteScalar().ToString();
                string s3 = new SqlCommand("select BIRIM FROM " + tabloadi + value, s).ExecuteScalar().ToString();
                decimal s4 = (decimal)new SqlCommand("select " + columnname + " FROM " + tabloadi + value, s).ExecuteScalar();
                Single s5 = (Single)new SqlCommand("select KDVORAN FROM " + tabloadi + value, s).ExecuteScalar();
                string s6 = new SqlCommand("select " + columnname + "DOVIZCINS FROM " + tabloadi + value, s).ExecuteScalar().ToString();
                string s7 = new SqlCommand("select MARKAAD FROM " + tabloadi + value, s).ExecuteScalar().ToString();
                Single s8 = (Single)new SqlCommand("select ISKONTOORAN FROM " + tabloadi + value, s).ExecuteScalar();
                Single kdvkesinti = (Single)new SqlCommand("select KDVKESINTIORAN FROM " + tabloadi + value, s).ExecuteScalar();
                //Single otvkesinti = (Single)new SqlCommand("select OTVKESINTIORAN FROM " + tabloadi + value, s).ExecuteScalar();
                //int kdvdh = Convert.ToInt16(new SqlCommand("select " + columnname + "KDVDH FROM " + tabloadi + value, s).ExecuteScalar());
                int otvtip = Convert.ToInt16(new SqlCommand("select OTVTIP FROM " + tabloadi + value, s).ExecuteScalar());
                decimal otvdeger = Convert.ToDecimal(new SqlCommand("select OTVDEGER FROM " + tabloadi + value, s).ExecuteScalar());
                c1 = new SqlCommand();
                c1.CommandText = @"INSERT INTO SATISHAR(SECIM, MIKTAR, ISKONTOORAN, ISKONTOTUTAR, TOPLAMTUTAR, [MALKOD],[MALAD],[BIRIM],[SATISFIYAT1],[KDVORAN], MARKAAD, [DOVIZCINS], OTVTIP, OTVDEGER, KDVKESINTIORAN, TOPLAMISKONTO) values(@SECIM, @MIKTAR, @ISKONTOORAN, @ISKONTOTUTAR, @TOPLAMTUTAR, @MALKOD, @MALAD, @BIRIM,@SATISFIYAT1,@KDVORAN, @MARKAAD, @DOVIZCINS, @OTVTIP, @OTVDEGER, @KDVKESINTIORAN, @TOPLAMISKONTO)";
                c1.Connection = sl;
                c1.Parameters.AddWithValue("@SECIM", 1);
                c1.Parameters.AddWithValue("@OTVTIP", otvtip);
                c1.Parameters.AddWithValue("@OTVDEGER", otvdeger);
                c1.Parameters.AddWithValue("@KDVKESINTIORAN", kdvkesinti);
                //c1.Parameters.AddWithValue("@OTVKESINTIORAN", otvkesinti);
                //c1.Parameters.AddWithValue("@kdvdh", kdvdh);
                int mikt;
                mikt = String.IsNullOrWhiteSpace(txtmiktar.Text) ? 1 : Convert.ToInt32(txtmiktar.Text);
                c1.Parameters.AddWithValue("@MIKTAR", mikt);
                decimal markaiskontoeklentisi = 0;
                if (!string.IsNullOrWhiteSpace(markaiskonto.Text) && markaadi.Count != 0 && markaadi.Contains(s7)) markaiskontoeklentisi = Convert.ToDecimal(markaiskonto.Text); //yeni ekleneceklerin markalarını kontrol edip eğer o markaya iskonto uygulanıyorsa iskontoyu ekliyor
                decimal isktutar = Convert.ToDecimal(s8) * s4 / 100;
                c1.Parameters.AddWithValue("@ISKONTOTUTAR", /* Convert.ToDecimal(mikt) * */ isktutar + markaiskontoeklentisi);
                if(s4 != 0) c1.Parameters.AddWithValue("@ISKONTOORAN", Convert.ToSingle(100*(isktutar + markaiskontoeklentisi)/s4));//markaiskontoeklentisi dahil yeni iskonto oranı
                else c1.Parameters.AddWithValue("@ISKONTOORAN", Convert.ToSingle(s8));//markaiskontoeklentisi dahil yeni iskonto oranı   s4 demişim daha önce
                c1.Parameters.AddWithValue("@TOPLAMISKONTO", isktutar + markaiskontoeklentisi);
                c1.Parameters.AddWithValue("@TOPLAMTUTAR", (double)mikt * (double)(s4 - isktutar));
                c1.Parameters.Add(new SqlParameter("@MALKOD", s1));
                c1.Parameters.Add(new SqlParameter("@MALAD", s2));
                c1.Parameters.Add(new SqlParameter("@BIRIM", s3));
                c1.Parameters.AddWithValue("@SATISFIYAT1", s4);
                c1.Parameters.AddWithValue("@KDVORAN", s5);
                c1.Parameters.Add(new SqlParameter("@DOVIZCINS", s6));
                c1.Parameters.Add(new SqlParameter("@MARKAAD", s7));
                c1.ExecuteNonQuery();
            }
            else
            {
                c1 = new SqlCommand("update SATISHAR set MIKTAR=MIKTAR+1 WHERE MALKOD='" + s1 + "'", s);
                c1.ExecuteNonQuery();
                c1 = new SqlCommand("update SATISHAR set TOPLAMTUTAR=(SATISFIYAT1-ISKONTOTUTAR)*MIKTAR WHERE MALKOD='" + s1 + "'", s);
                c1.ExecuteNonQuery();
            }
            populate();
            Hesap();
        }
        private void populate()
        {
            sl = new SqlConnection(Carried.girisBaglantiLocal);
            sl.Open();
            c1 = new SqlCommand("delete from SATISHAR WHERE SECIM = 0 OR MIKTAR=0", sl);
            c1.ExecuteNonQuery();
            DataTable table = new DataTable();
            //////////table.Columns.Add(new DataColumn("Seçim", typeof(bool)));
            //table.Columns.Add(new DataColumn("Miktar", typeof(string)));
            //table.Columns.Add(new DataColumn("İskonto Tutar", typeof(string)));
            //table.Columns.Add(new DataColumn("Toplam Tutar", typeof(string)));
            SqlDataAdapter a = new SqlDataAdapter("select SECIM as 'Seçim' , MIKTAR as 'Miktar', MALKOD as 'Malzeme Kodu', MALAD as 'Malzeme Tanımı', SATISFIYAT1 as 'Birim Fiyatı', ISKONTOORAN as 'İskonto Oranı', ISKONTOTUTAR as 'İskonto Tutar', TOPLAMTUTAR as 'Toplam Tutar' , BIRIM as Birim, KDVORAN as KDV, MARKAAD as 'Marka Adı', DOVIZCINS as 'Döviz Cins', KALEMISKONTOORAN2 as 'İskonto Oran 2', KALEMISKONTOORAN3 as 'İskonto Oran 3', KALEMISKONTOORAN4 as 'İskonto Oran 4', KALEMISKONTOORAN5 as 'İskonto Oran 5', TOPLAMISKONTO as 'Toplam İskonto' FROM SATISHAR", sl);
            a.Fill(table);
            this.stok_dataGrid.ItemsSource = table.DefaultView;
            int x = 0;
            foreach (DataGridColumn column in stok_dataGrid.Columns)
            {
                if (x == 0 || x == 1 || x == 4 || x == 5 || x == 6 || x == 12 || x == 13 || x == 14 || x == 15) { column.IsReadOnly = false; }
                else column.IsReadOnly = true;
                if (x == 0) { column.Visibility = Visibility.Hidden; }
                x++;
            }
            txtmiktar.Clear();
        }
        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            string constr = Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();
            c1 = new SqlCommand("delete from SATISHAR", s);
            c1.ExecuteNonQuery();
            populate();
            c1 = new SqlCommand("delete from SMRTAPPHSP", s);
            c1.ExecuteNonQuery();
            c1 = new SqlCommand("delete from FATEVR", s);
            c1.ExecuteNonQuery();

            satısharmalkodlari.Clear();
            markaadi.Clear();
            txt_kalemiskonto.Text = "";
            txt_aratoplam.Text = "";
            txt_genelnettutar.Text = "";
            txt_geneliskontotutar.Text = "";
            txt_geneltoplam.Text = "";
            txt_vergi.Text = "";
            txt_nettutar.Text = "";
            markaiskonto.Text = "";
            txtmiktar.Text = "";
            this.depo_dataGrid.ItemsSource = null;
            foreach (UIElement item in eklenecekyer.Children)
            {
                if (item.GetType().Equals(typeof(TextBox)))
                {
                    TextBox tbox = (TextBox)item;
                    tbox.Clear();
                }
            }
            Odeme.parcaliodeme.source = null;
        }
        private void Hesap()
        {
            Single kdvkesoran; decimal kdvkesinti = 0;//kdvkesinti satanın ödeyeceği tutar
            int otvtip; decimal otvdeger; decimal _otvtutar = 0, toplamotvtutar = 0; Single otvoran = 0; decimal eskinettutar;
            int kdvdh; string constr;  
            tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
            constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();
            decimal _geneltop=0, _nettutar=0 , _kdv = 0, _ara = 0,  _iskonto=0;
            decimal aratoplam = 0, nettutar = 0, vergi = 0, geneltoplam = 0, toplamkalemiskontotutar = 0, toplamtoplamtutar = 0;
            for (int i = 0; i < stok_dataGrid.Items.Count; i++)
            {
                DataRowView rowView = stok_dataGrid.Items[i] as DataRowView;
                otvdeger = (decimal)new SqlCommand("select OTVDEGER from SATISHAR where malkod='" + rowView[2] + "'", sl).ExecuteScalar();
                otvtip = Convert.ToInt16(new SqlCommand("select OTVTIP from SATISHAR where malkod='" + rowView[2] + "'", sl).ExecuteScalar());
                kdvkesoran = Convert.ToSingle(new SqlCommand("select KDVKESINTIORAN from SATISHAR where malkod='" + rowView[2] + "'", sl).ExecuteScalar());
                kdvdh = Convert.ToInt16(new SqlCommand("select " + columnname + "KDVDH FROM " + tabloadi + " where MALKOD='" + rowView[2] + "'", s).ExecuteScalar());
                if (otvdeger == 0) //if (rowView[0].ToString() == "True" )
                {
                    otvoran = 0; _otvtutar = 0;
                    if (kdvdh==0 && _switch.IsChecked == false)
                    { 
                        _ara = Convert.ToDecimal(rowView[1]) * Convert.ToDecimal(rowView[4]) * getCurrency(rowView[11].ToString().ToUpper());//hepsinin satış fiyatının miktarla çarpımı toplamı, herhangi bir iskonto, kdv vb. olmadan ve çarpı doviz oranı
                        aratoplam += _ara;
                        _iskonto = Convert.ToDecimal(rowView[16]) * Convert.ToDecimal(rowView[1]) * getCurrency(rowView[11].ToString().ToUpper());// iskonto tutarı * miktar * doviz
                        toplamkalemiskontotutar += _iskonto;
                        _kdv = (Convert.ToDecimal(rowView[9]) / Convert.ToDecimal(100)) * Convert.ToDecimal(rowView[1]) * (Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[16])) * getCurrency(rowView[11].ToString().ToUpper()); //her bir satırın kdv'si toplamı yani (kdv)*miktar*(satıs fiyatı-iskonto)*doviz
                        vergi += _kdv;
                        rowView.BeginEdit();
                        rowView[7] = Convert.ToDecimal(rowView[7]) * getCurrency(rowView[11].ToString().ToUpper());// YANİ İSKONTO DA DOLAR ÜZERİNDEN ALINIYOR. EN SONKİ TUTARI TL'YE ÇEVİRİYOR.
                        rowView.EndEdit();
                        stok_dataGrid.Items.Refresh();
                        //toplamtoplamtutar += Convert.ToDecimal(rowView[7]);

                        _nettutar = _ara - _iskonto; //Convert.ToDecimal(rowView[7]);
                        _geneltop = _nettutar + _kdv;
                    }
                    else if (kdvdh == 1 && _switch.IsChecked == false)
                    {
                        _ara = Convert.ToDecimal(rowView[1]) * Convert.ToDecimal(rowView[4]) / Convert.ToDecimal("1," + rowView[9].ToString()) * getCurrency(rowView[11].ToString().ToUpper());//miktar*kdvsiz satisfiyati
                        aratoplam += _ara;
                        
                        //_iskonto = Convert.ToDecimal(rowView[6]) * Convert.ToDecimal(rowView[1]) * getCurrency(rowView[11].ToString().ToUpper());// iskonto tutarı * miktar * doviz
                        if (Convert.ToDecimal(rowView[16]) != 0) _iskonto = Convert.ToDecimal(rowView[16]) / Convert.ToDecimal("1," + rowView[9].ToString()) * Convert.ToDecimal(rowView[1]) * getCurrency(rowView[11].ToString().ToUpper());// (iskonto tutarı/1.kdv) * miktar * doviz
                        else _iskonto = 0;
                        toplamkalemiskontotutar += _iskonto;
                       
                        _nettutar = _ara - _iskonto;

                        rowView.BeginEdit();
                        rowView[7] = Convert.ToDecimal(rowView[7]) * getCurrency(rowView[11].ToString().ToUpper());  
                        rowView.EndEdit();
                        stok_dataGrid.Items.Refresh();
                       
                        //kdv += Convert.ToDecimal(rowView[1]) * (Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[16]) - (Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[16])) / Convert.ToDecimal("1,"+rowView[9].ToString())) * getCurrency(rowView[11].ToString().ToUpper()); //her bir satırın kdv'si toplamı yani miktar*(satısfiyatı-(satısfiyatı/1.kdvoranı))*doviz  --->bu formüldeki satısfiyatı iskonto çıkmış hali
                        //_kdv = (Convert.ToDecimal(rowView[4]) * Convert.ToDecimal(rowView[1]) - _ara) * getCurrency(rowView[11].ToString().ToUpper());
                        _kdv = _nettutar * Convert.ToDecimal("0," + rowView[9].ToString());
                        vergi += _kdv;
                        
                        _geneltop = _nettutar + _kdv;
                        //toplamtoplamtutar += Convert.ToDecimal(rowView[7]);
                    }
                    else if (kdvdh == 0 && _switch.IsChecked == true)
                    {
                        _kdv = 0;
                        _ara = Convert.ToDecimal(rowView[1]) * Convert.ToDecimal(rowView[4]) * getCurrency(rowView[11].ToString().ToUpper());//hepsinin satış fiyatının miktarla çarpımı toplamı, herhangi bir iskonto, kdv vb. olmadan ve çarpı doviz oranı
                        aratoplam += _ara;
                        _iskonto = Convert.ToDecimal(rowView[16]) * Convert.ToDecimal(rowView[1]) * getCurrency(rowView[11].ToString().ToUpper());// iskonto tutarı * miktar * doviz
                        toplamkalemiskontotutar += _iskonto;
                        rowView.BeginEdit();
                        rowView[7] = Convert.ToDecimal(rowView[7]) * getCurrency(rowView[11].ToString().ToUpper());
                        rowView.EndEdit();
                        stok_dataGrid.Items.Refresh();
                        _nettutar = _ara - _iskonto; //Convert.ToDecimal(rowView[7]);
                        _geneltop = _nettutar;
                    }
                    else if (kdvdh == 1 && _switch.IsChecked == true)
                    {
                        _kdv = 0;
                        _ara = Convert.ToDecimal(rowView[1]) * Convert.ToDecimal(rowView[4]) / Convert.ToDecimal("1," + rowView[9].ToString()) * getCurrency(rowView[11].ToString().ToUpper());//miktar*kdvsiz satisfiyati
                        aratoplam += _ara;
                        _iskonto = Convert.ToDecimal(rowView[16]) * getCurrency(rowView[11].ToString().ToUpper());
                        toplamkalemiskontotutar += _iskonto;
                        rowView.BeginEdit();
                        rowView[7] = Convert.ToDecimal(rowView[7]) * getCurrency(rowView[11].ToString().ToUpper());
                        rowView.EndEdit();
                        stok_dataGrid.Items.Refresh();
                        _nettutar = _ara - _iskonto;
                        _geneltop = _nettutar;
                    }
                    nettutar += _nettutar;
                    geneltoplam += _geneltop;
                    if(kdvkesoran > 0)
                    {
                        kdvkesinti = ((decimal)kdvkesoran / 100) * _kdv;
                        _kdv = _kdv - kdvkesinti;
                        vergi = vergi - kdvkesinti;
                        geneltoplam = geneltoplam - kdvkesinti;
                    }
                    c1 = new SqlCommand("update SATISHAR set TOPLAMTUTAR = " + rowView[7].ToString().Replace(",", ".") + " , genel = " + _geneltop.ToString().Replace(",", ".") + ", kalemiskonto= " + _iskonto.ToString().Replace(",", ".") + ", ara = " + _ara.ToString().Replace(",", ".") + ", net = " + _nettutar.ToString().Replace(",", ".") + " WHERE MALKOD='" + rowView[2] + "'", sl);
                    c1.ExecuteNonQuery();
                    c1 = new SqlCommand("update SATISHAR set kdvtutar = " + _kdv.ToString().Replace(",", ".") + ", kdvdh = " + kdvdh + ", otvoran ="  + otvoran.ToString().Replace(",", ".") + ", otvtutar =" + _otvtutar.ToString().Replace(",", ".") + ", KDVKESINTI = " + kdvkesinti.ToString().Replace(",", ".") + " WHERE MALKOD='" + rowView[2] + "'", sl);
                    c1.ExecuteNonQuery();
                }
                else
                {
                    if (kdvdh == 0 && _switch.IsChecked == false)
                    {
                        _ara = Convert.ToDecimal(rowView[1]) * Convert.ToDecimal(rowView[4]) * getCurrency(rowView[11].ToString().ToUpper());//hepsinin satış fiyatının miktarla çarpımı toplamı, herhangi bir iskonto, kdv vb. olmadan ve çarpı doviz oranı
                        aratoplam += _ara;
                        _iskonto = Convert.ToDecimal(rowView[16]) * Convert.ToDecimal(rowView[1]) * getCurrency(rowView[11].ToString().ToUpper());// iskonto tutarı * miktar * doviz
                        toplamkalemiskontotutar += _iskonto;
                        _nettutar = _ara - _iskonto;
                        if (otvtip == 0)// otvdeger oransa
                        {
                            otvoran = Convert.ToSingle(otvdeger);
                            _otvtutar = _nettutar * Convert.ToDecimal("0," + otvoran.ToString());
                            _nettutar = _nettutar + _otvtutar;
                            _kdv = _nettutar * Convert.ToDecimal("0," + rowView[9].ToString());
                            vergi += _kdv + _otvtutar;
                            _geneltop = _nettutar + _kdv;
                        }
                        else//otvdeger tutarsa
                        {
                            eskinettutar = _nettutar;
                            _otvtutar = otvdeger * Convert.ToDecimal(rowView[1]);
                            _nettutar = _nettutar + _otvtutar;
                            otvoran = Convert.ToSingle((_nettutar / eskinettutar).ToString().Split(',').Last().Substring(0, 2));
                            _kdv = _nettutar * Convert.ToDecimal("0," + rowView[9].ToString());
                            vergi += _kdv + _otvtutar;
                            _geneltop = _nettutar + _kdv;
                        }
                        rowView.BeginEdit();
                        rowView[7] = Convert.ToDecimal(rowView[7]) * getCurrency(rowView[11].ToString().ToUpper());
                        rowView.EndEdit();
                        stok_dataGrid.Items.Refresh();
                    }
                    else if (kdvdh == 1 && _switch.IsChecked == false)
                    {
                        _ara = Convert.ToDecimal(rowView[1]) * Convert.ToDecimal(rowView[4]) / Convert.ToDecimal("1," + rowView[9].ToString()) * getCurrency(rowView[11].ToString().ToUpper());//miktar*kdvsiz satisfiyati
                        aratoplam += _ara;

                        if (Convert.ToDecimal(rowView[16]) != 0) _iskonto = Convert.ToDecimal(rowView[16]) / Convert.ToDecimal("1," + rowView[9].ToString()) * Convert.ToDecimal(rowView[1]) * getCurrency(rowView[11].ToString().ToUpper());// (iskonto tutarı/1.kdv) * miktar * doviz
                        else _iskonto = 0;
                        toplamkalemiskontotutar += _iskonto;

                        _nettutar = _ara - _iskonto;

                        if (otvtip == 0)// otvdeger oransa
                        {
                            otvoran = Convert.ToSingle(otvdeger);
                            _otvtutar = _nettutar * Convert.ToDecimal("0," + otvoran.ToString());
                            _nettutar = _nettutar + _otvtutar;
                            _kdv = _nettutar * Convert.ToDecimal("0," + rowView[9].ToString());
                            vergi += _kdv + _otvtutar;
                            _geneltop = _nettutar + _kdv;
                        }
                        else//otvdeger tutarsa
                        {
                            eskinettutar = _nettutar;
                            _otvtutar = otvdeger * Convert.ToDecimal(rowView[1]);
                            _nettutar = _nettutar + _otvtutar;
                            otvoran = Convert.ToSingle((_nettutar / eskinettutar).ToString().Split(',').Last().Substring(0, 2));
                            _kdv = _nettutar * Convert.ToDecimal("0," + rowView[9].ToString());
                            vergi += _kdv + _otvtutar;
                            _geneltop = _nettutar + _kdv;
                        }
                        
                        rowView.BeginEdit();
                        rowView[7] = Convert.ToDecimal(rowView[7]) * getCurrency(rowView[11].ToString().ToUpper());
                        rowView.EndEdit();
                        stok_dataGrid.Items.Refresh();
                    }
                    else if (kdvdh == 0 && _switch.IsChecked == true)
                    {
                        _kdv = 0; _otvtutar = 0; otvoran = 0;
                        _ara = Convert.ToDecimal(rowView[1]) * Convert.ToDecimal(rowView[4]) * getCurrency(rowView[11].ToString().ToUpper());//hepsinin satış fiyatının miktarla çarpımı ve çarpı doviz oranı
                        aratoplam += _ara;
                        _iskonto = Convert.ToDecimal(rowView[16]) * Convert.ToDecimal(rowView[1]) * getCurrency(rowView[11].ToString().ToUpper());// iskonto tutarı * miktar * doviz
                        toplamkalemiskontotutar += _iskonto;
                        rowView.BeginEdit();
                        rowView[7] = Convert.ToDecimal(rowView[7]) * getCurrency(rowView[11].ToString().ToUpper());
                        rowView.EndEdit();
                        stok_dataGrid.Items.Refresh();
                        _nettutar = _ara - _iskonto; //Convert.ToDecimal(rowView[7]);
                        _geneltop = _nettutar;
                    }
                    else if (kdvdh == 1 && _switch.IsChecked == true)
                    {
                        _kdv = 0; _otvtutar = 0; otvoran = 0;
                        _ara = Convert.ToDecimal(rowView[1]) * Convert.ToDecimal(rowView[4]) / Convert.ToDecimal("1," + rowView[9].ToString()) * getCurrency(rowView[11].ToString().ToUpper());//miktar*kdvsiz satisfiyati
                        aratoplam += _ara;
                        _iskonto = Convert.ToDecimal(rowView[16]) * getCurrency(rowView[11].ToString().ToUpper());
                        toplamkalemiskontotutar += _iskonto;
                        rowView.BeginEdit();
                        rowView[7] = Convert.ToDecimal(rowView[7]) * getCurrency(rowView[11].ToString().ToUpper());
                        rowView.EndEdit();
                        stok_dataGrid.Items.Refresh();
                        _nettutar = _ara - _iskonto;
                        _geneltop = _nettutar;
                    }
                    nettutar += _nettutar;
                    geneltoplam += _geneltop;
                    if (kdvkesoran > 0)
                    {
                        kdvkesinti = ((decimal)kdvkesoran / 100) * _kdv;
                        _kdv = _kdv - kdvkesinti;
                        vergi = vergi - kdvkesinti;
                        geneltoplam = geneltoplam - kdvkesinti;
                    }
                    c1 = new SqlCommand("update SATISHAR set genel = " + _geneltop.ToString().Replace(",", ".") + ", kalemiskonto = " + _iskonto.ToString().Replace(",", ".") + ", ara = " + _ara.ToString().Replace(",", ".") + ", net = " + _nettutar.ToString().Replace(",", ".") + " WHERE MALKOD='" + rowView[2] + "'", sl);
                    c1.ExecuteNonQuery();
                    c1 = new SqlCommand("update SATISHAR set kdvtutar = " + _kdv.ToString().Replace(",", ".") + ", kdvdh = " + kdvdh + ", otvoran =" + otvoran.ToString().Replace(",", ".") + ", otvtutar =" + _otvtutar.ToString().Replace(",", ".") + ", KDVKESINTI = " + kdvkesinti.ToString().Replace(",", ".") + " WHERE MALKOD='" + rowView[2] + "'", sl);
                    c1.ExecuteNonQuery();
                }
            }
            txt_aratoplam.Text = Math.Round(aratoplam, 2).ToString();
            txt_kalemiskonto.Text = Math.Round(toplamkalemiskontotutar, 2).ToString();
            txt_vergi.Text = Math.Round(vergi, 2).ToString();
            txt_nettutar.Text = Math.Round(nettutar, 2).ToString();  //MessageBox.Show(toplamtoplamtutar.ToString());
            txt_geneltoplam.Text = Math.Round(geneltoplam, 2).ToString();
            if (String.IsNullOrWhiteSpace(txt_geneliskontotutar.Text) || txt_geneliskontotutar.Text == "0") txt_genelnettutar.Text = Math.Round(geneltoplam, 2).ToString();
            else txt_genelnettutar.Text = Math.Round(geneltoplam - Convert.ToDecimal(txt_geneliskontotutar.Text), 2).ToString();
        }
        


        private void stkYeni_Click(object sender, RoutedEventArgs e)
        {
            StokKartAc w = new StokKartAc("SATIS");
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

                foreach (UIElement item in eklenecekyer.Children)
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
                            case "Stok Kodu:":
                                malkod = tbox.Text;
                                break;
                            case "Stok Adı:":
                                malad = tbox.Text;
                                break;
                            case "Birim:":
                                birim = tbox.Text;
                                break;
                            case "Stok Kontrolü:":
                                stokkontroldurum = tbox.Text;
                                if (Convert.ToInt16(stokkontroldurum) == 0) //stokkontroldurum 0 ise bakiyenin eksiye düşmesine izin ypk, 1 ise var.
                                {
                                    for(int j=0; j<depo_dataGrid.Items.Count; j++)
                                    {
                                        DataRowView rowView = depo_dataGrid.Items[j] as DataRowView;
                                        if (Convert.ToDecimal(rowView[9]) < 0)
                                        {
                                            rowView.BeginEdit();
                                            rowView[9] = "0";// Convert.ToDecimal(0);
                                            rowView.EndEdit();
                                            depo_dataGrid.Items.Refresh();
                                        }
                                    }
                                }
                                break;
                            case "Marka Adı:":
                                markaad = tbox.Text;
                                break;
                            case "Grup Kodu:":
                                grupkod = tbox.Text;
                                break;
                            case "Satış Muhasebe Kodu:":
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
                cmmd += " DEGISTIRENTARIH = @tarih,";
                cmmd += " DEGISTIRENSAAT = @saat,";
                cmmd += " DEGISTIRENKULLANICI = '" + Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30)) + "',";
                cmmd += " DEGISTIRENKAYNAK = '" + System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30)) + "' ";
                c1 = new SqlCommand(cmmd + " where MALKOD = '" + ilkmalkod + "'", s);
                c1.Parameters.Add("@tarih", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                c1.Parameters.Add("@saat", SqlDbType.SmallDateTime).Value = DateTime.Now;
                c1.ExecuteNonQuery();

                s = new SqlConnection(Carried.girisBaglantiLocal);
                s.Open();
                cmmd = "update SATISHAR set ";
                if (!String.IsNullOrWhiteSpace(malkod)) { cmmd += " MALKOD = '" + malkod + "',"; }
                if (!String.IsNullOrWhiteSpace(malad)) { cmmd += " MALAD = '" + malad + "',"; }
                if (!String.IsNullOrWhiteSpace(birim)) { cmmd += "  BIRIM = '" + birim + "',"; }
                if (!String.IsNullOrWhiteSpace(markaad)) { cmmd += " MARKAAD = '" + markaad + "',"; }
                cmmd += " SECIM = 1 ";
                c1 = new SqlCommand(cmmd + " where MALKOD = '" + ilkmalkod + "'", s);
                c1.ExecuteNonQuery();
                populate();
                Hesap();

                s = new SqlConnection(Carried.girisBaglantiLocal);
                s.Open();
                cmmd = "update SMRTAPPDEPO set ";
                if (!String.IsNullOrWhiteSpace(malad)) { cmmd += " MALAD = '" + malad + "',"; }
                cmmd += " MALKOD = '" + malkod + "'"; 
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
                    if (Carried.IsCPMconnected == false) { Carried.showMessage("İnternet bağlantısı bulunamadığı için local bağlantıya geçildi. İşlemi tekrar yapınız."); }
                    else Carried.showMessage(ex.Message);
                }
                else Carried.showMessage(ex.Message);
            }
        }


        
        #region ÜST KISIM
        private void vadeTarihiGetir()
        {
            tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
            c1 = new SqlCommand("select OPSIYON from " + tabloadi + " where HESAPKOD ='" + txt2.Text + "'", s);
            object o3 = c1.ExecuteScalar();
            if (o3 != null && Convert.ToInt16(o3) > 0)
            {
                txt8.Text = DateTime.Today.AddDays(Convert.ToInt16(o3)).ToShortDateString();
            }
            else if (o3 != null && Convert.ToInt16(o3) == 0)
            {
                txt8.Text = DateTime.Today.ToShortDateString();
            }
        }
        private void txt1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                lbUnvan.Items.Clear();
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
                tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
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
                        tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
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
                    vadeTarihiGetir();
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
            if ((e.Key == Key.Enter))
            {
                txt1.Text = lbUnvan.Items[lbUnvan.SelectedIndex].ToString();
                PopupUnvan.IsOpen = false;
                lbUnvan.Items.Clear();
                try
                {
                    tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
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
                            tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
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
                        vadeTarihiGetir();
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
                            tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
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
                        vadeTarihiGetir();
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
                if ((txt3.Text.Length == 10 && isVergiNoValid(txt3.Text) == true) || (txt3.Text.Length == 11 && isKimlikNoValid(txt3.Text)))
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
                        c1 = new SqlCommand("select DOVIZCINS from " + tabloadi + " where VERGIHESAPNO ='" + txt3.Text + "'", s);
                        object o2 = c1.ExecuteScalar();
                        if (o != null)
                        {
                            txt2.Text = o.ToString();
                            txt1.Text = o1.ToString();
                            txt9.Text = o2.ToString();
                            if (string.IsNullOrEmpty(txt9.Text)) txt9.Text = "TL";
                            if (!string.IsNullOrWhiteSpace(txt3.Text))
                            {
                                tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
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
                            vadeTarihiGetir();
                        }
                        else
                        {
                            //CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                            //{
                            //    TxtMessage = { Text = "Vergi no bulunamamıştır. Yeni cari kart açmak ister misiniz?",
                            //    TextAlignment=System.Windows.TextAlignment.Center,
                            //    VerticalAlignment=VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, TextWrapping = TextWrapping.Wrap ,
                            //    Foreground = Brushes.Black , FontFamily = new FontFamily("Arial"), FontSize = 25},
                            //    TxtTitle = { Text = "UYARI", Foreground = Brushes.White },
                            //    MainContentControl = { Background = Brushes.Turquoise },
                            //    TitleBackgroundPanel = { Background = Brushes.Blue },
                            //    BorderBrush = Brushes.Blue,
                            //    BtnCancel = { Content = "HAYIR", Background = Brushes.Red, IsCancel = true },
                            //    BtnOk = { Content = "EVET", Background = Brushes.Green }
                            //};
                            //msg.Show();
                            //MessageBoxResult results = msg.Result;
                            Gat.Controls.MessageBoxView messageBox = new Gat.Controls.MessageBoxView();
                            Gat.Controls.MessageBoxViewModel vm = (Gat.Controls.MessageBoxViewModel)messageBox.FindResource("ViewModel");

                            vm.Message = "";
                            vm.Ok = "TAMAM";
                            vm.Cancel = "HAYIR";
                            vm.OkVisibility = true;
                            vm.CancelVisibility = true;
                            vm.YesVisibility = false;
                            vm.NoVisibility = false;
                            vm.Image = new BitmapImage(new System.Uri(@"/icons/sc-modal-img-info.png", UriKind.Relative));
                            vm.Caption = "Test Message";

                            // Center functionality
                            vm.Position = MessageBoxPosition.CenterOwner;
                            vm.Owner = this;

                            //vm.Show();

                            Gat.Controls.MessageBoxResult result = vm.Show();

                            if (result == Gat.Controls.MessageBoxResult.Ok)
                            {
                                Carried.DosyaDecrypt();
                                string hesapno = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Hesap No Ayari", "Baglanti.ini");
                                Carried.DosyaEncrypt();
                                if (!String.IsNullOrWhiteSpace(hesapno))
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
                            //else if (result == Gat.Controls.MessageBoxResult.Cancel) vm.Close();
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
            if (rakamdegilmi == 0)
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
                        try { txt8.Text = now.AddDays(numericValue).ToShortDateString(); }
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
        private void txt7_LostFocus(object sender, RoutedEventArgs e)
        {
            if (dp1.IsFocused == false && dp1.IsDropDownOpen == false) { cbdate1.IsDropDownOpen = false; }
        }
        private void txt7_GotFocus(object sender, RoutedEventArgs e)
        {
            cbdate1.IsDropDownOpen = true;
            cbdate1.SelectedItem = null;
        }
        private void selected_date_changed1(object sender, SelectionChangedEventArgs e)
        {
            txt7.Text = dp1.SelectedDate.ToString();
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
        #endregion

        #region TOGGLEBUTTONLAR
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
        #endregion

        #region ALT KISIM EDİT
        private void markaiskonto_KeyDown(object sender, KeyEventArgs e)//markaları gösterir
        {
            if (e.Key == Key.Enter)
            {
                lbSatısHarMarkalar.Items.Clear();
                if (!string.IsNullOrWhiteSpace(markaiskonto.Text))
                {
                    try
                    {
                        for (int i = 0; i < stok_dataGrid.Items.Count; i++)
                        {
                            DataRowView rowView = stok_dataGrid.Items[i] as DataRowView;
                            if (!lbSatısHarMarkalar.Items.Contains(rowView[10].ToString()) && !String.IsNullOrWhiteSpace(rowView[10].ToString()))
                                lbSatısHarMarkalar.Items.Add(rowView[10].ToString());
                        }
                        PopupSatısHarMarkalar.IsOpen = true;
                    }

                    catch (Exception ex)
                    {
                        Carried.showMessage(ex.Message);
                    }
                }
            }
        }
        List<string> markaadi = new List<string>() { };
        private void lbSatısHarMarkalar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            markaadi.Add(lbSatısHarMarkalar.Items[lbSatısHarMarkalar.SelectedIndex].ToString());
            lbSatısHarMarkalar.Items.Clear();
            PopupSatısHarMarkalar.IsOpen = false;
            if (!String.IsNullOrWhiteSpace(markaiskonto.Text))
            {
                try
                {
                    for (int i = 0; i < stok_dataGrid.Items.Count; i++)
                    {
                        DataRowView rowView = (stok_dataGrid.Items[i] as DataRowView);
                        if (markaadi.Contains(rowView[10].ToString()))
                        {
                            rowView.BeginEdit();
                            decimal d = Convert.ToDecimal(rowView[6]) + Convert.ToDecimal(markaiskonto.Text);//iskonto oranı+girilen
                            rowView[6] = d;
                            if (Convert.ToDecimal(rowView[4]) != 0) rowView[5] = 100 * Convert.ToDecimal(rowView[6]) / Convert.ToDecimal(rowView[4]);
                            rowView.EndEdit();
                            stok_dataGrid.Items.Refresh();
                        }
                    }
                    SatısHarGuncelle();
                }
                catch (Exception ex)
                {
                    Carried.showMessage(ex.Message);
                }
            }
        }
        private void lbSatısHarMarkalar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                markaadi.Add(lbSatısHarMarkalar.Items[lbSatısHarMarkalar.SelectedIndex].ToString());
                lbSatısHarMarkalar.Items.Clear();
                PopupSatısHarMarkalar.IsOpen = false;
                if (!String.IsNullOrWhiteSpace(markaiskonto.Text))
                {
                    try
                    {
                        for (int i = 0; i < stok_dataGrid.Items.Count; i++)
                        {
                            DataRowView rowView = (stok_dataGrid.Items[i] as DataRowView);
                            if (markaadi.Contains(rowView[10].ToString()))
                            {
                                rowView.BeginEdit();
                                decimal d = Convert.ToDecimal(rowView[6]) + Convert.ToDecimal(markaiskonto.Text);//iskonto tutarı+girilen
                                rowView[6] = d;
                                if (Convert.ToDecimal(rowView[4]) != 0) rowView[5] = 100 * Convert.ToDecimal(rowView[6]) / Convert.ToDecimal(rowView[4]);
                                rowView.EndEdit();
                                stok_dataGrid.Items.Refresh();
                            }
                        }
                        SatısHarGuncelle();
                    }
                    catch (Exception ex)
                    {
                        Carried.showMessage(ex.Message);
                    }
                }
            }
        }
        private void txt_geneliskontotutar_TextChanged(object sender, TextChangedEventArgs e)//ya da enter basınca olsun burası??
        {
            if (!String.IsNullOrWhiteSpace(txt_geneliskontotutar.Text) && !String.IsNullOrWhiteSpace(txt_geneltoplam.Text))
            {
                if (txt_geneliskontotutar.Text.StartsWith("%"))
                {
                    string stringNumber = txt_geneliskontotutar.Text.Remove(0, 1);
                    int numericValue;
                    bool isNumber = int.TryParse(stringNumber, out numericValue);
                    if (isNumber == true) //%den sonrası int ise
                    {
                        txt_genelnettutar.Text = (Convert.ToDecimal(txt_geneltoplam.Text) - Convert.ToDecimal(txt_geneltoplam.Text) * Convert.ToDecimal(stringNumber) / Convert.ToDecimal(100)).ToString();
                    }
                    else txt_genelnettutar.Text = txt_geneltoplam.Text;
                }
                else
                {
                    int numericValue;
                    bool isNumber = int.TryParse(txt_geneliskontotutar.Text, out numericValue);
                    if (isNumber == true)
                        txt_genelnettutar.Text = (Convert.ToDecimal(txt_geneltoplam.Text) - Convert.ToDecimal(txt_geneliskontotutar.Text)).ToString();
                    else txt_genelnettutar.Text = txt_geneltoplam.Text;
                }
            }
            else if (String.IsNullOrWhiteSpace(txt_geneliskontotutar.Text) && !String.IsNullOrWhiteSpace(txt_geneltoplam.Text))
                txt_genelnettutar.Text = txt_geneltoplam.Text;
        }
        private void SatısHarGuncelle()
        {
            s = new SqlConnection(Carried.girisBaglantiLocal);
            s.Open();
            string cmmd;
            for (int i = 0; i < stok_dataGrid.Items.Count; i++)
            {
                DataRowView rowView = (stok_dataGrid.Items[i] as DataRowView);
                if (rowView[0].ToString() == "True")
                {
                    rowView.BeginEdit();
                    rowView[7] = Convert.ToDecimal(rowView[1]) * (Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[6])); //toplam tutar column hesaplama. miktar*(satısfiyatı-iskonto)
                    rowView.EndEdit();
                    stok_dataGrid.Items.Refresh();
                    cmmd = "update SATISHAR set " + columnname + " = " + rowView[4].ToString().Replace(",", ".") + ", ISKONTOORAN = " + Convert.ToSingle(rowView[5]) + ", ISKONTOTUTAR = " + rowView[6].ToString().Replace(",", ".") + ", TOPLAMTUTAR = " + rowView[7].ToString().Replace(",", ".") + ", MIKTAR = " + rowView[1] + " WHERE MALKOD = '" + rowView[2] + "'";
                    c1 = new SqlCommand(cmmd, s);
                    c1.ExecuteNonQuery();
                }
            }
            populate();
            Hesap();
        }
        private bool isManualEditCommit;
        private void stok_dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (!isManualEditCommit && e.EditAction == DataGridEditAction.Commit)
                {
                    isManualEditCommit = true;
                    DataGrid grid = (DataGrid)sender;
                    grid.CommitEdit(DataGridEditingUnit.Row, true);
                    isManualEditCommit = false;

                    s = new SqlConnection(Carried.girisBaglantiLocal);
                    s.Open();
                    string cmmd;
                    int colindex1 = 0;
                    if (stok_dataGrid.SelectedItems.Count == 1 && stok_dataGrid.CurrentCell.Column == null) colindex1 = colindex;
                    else if (stok_dataGrid.SelectedItems.Count == 1 && stok_dataGrid.CurrentCell.Column != null) colindex1 = stok_dataGrid.CurrentCell.Column.DisplayIndex;
                    for (int i = 0; i < stok_dataGrid.Items.Count; i++)
                    {
                        DataRowView rowView = stok_dataGrid.Items[i] as DataRowView;
                        if (rowView[0].ToString() == "True")
                        {
                            if (Convert.ToInt32(rowView[1]) == 0) satısharmalkodlari.Remove(rowView[2].ToString());

                            rowView.BeginEdit();
                            if (colindex1 == 6) 
                            {
                                if (Convert.ToDecimal(rowView[4]) != 0) rowView[5] = 100 * Convert.ToDecimal(rowView[6]) / Convert.ToDecimal(rowView[4]);
                                rowView[6] = Convert.ToDecimal(rowView[4]) * Convert.ToDecimal(rowView[5]) / 100;
                            }
                            else
                            {
                                rowView[6] = Convert.ToDecimal(rowView[4]) * Convert.ToDecimal(rowView[5]) / 100;
                                if (Convert.ToDecimal(rowView[4]) != 0) rowView[5] = 100 * Convert.ToDecimal(rowView[6]) / Convert.ToDecimal(rowView[4]);
                            }
                            //rowView[7] = Convert.ToDecimal(rowView[1]) * (Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[6])); //toplam tutar column hesaplama. miktar*(satısfiyatı-iskonto)
                            decimal kalansatfiyat = Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[6]); //satısfiyatı-iskonto1
                            decimal yeniiskonto, kalemiskontotoplami = Convert.ToDecimal(rowView[6]);
                            for (int j = 12; j<=15; j++)
                            {
                                yeniiskonto = kalansatfiyat * Convert.ToDecimal(rowView[j]) / 100;
                                kalansatfiyat -= yeniiskonto;
                                kalemiskontotoplami += yeniiskonto;
                                cmmd = "update SATISHAR set KALEMISKONTOORAN"+ (j - 10).ToString()+ "=" + rowView[j] + ", KALEMISKONTO"+ (j-10).ToString() + "=" + yeniiskonto.ToString().Replace(",", ".") + " WHERE MALKOD = '" + rowView[2] + "'";
                                c1 = new SqlCommand(cmmd, s);
                                c1.ExecuteNonQuery();
                            }
                            rowView[16] = kalemiskontotoplami;
                            rowView[7] = Convert.ToDecimal(rowView[1]) * (Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[16])); //toplam tutar column hesaplama. miktar*(satısfiyatı-toplamiskonto)
                            rowView.EndEdit();
                            int a = rowView[0].ToString() == "True" ? 1 : 0;
                            cmmd = "update SATISHAR set SECIM = " + a + ", " + columnname + " = " + rowView[4].ToString().Replace(",", ".") + ", ISKONTOORAN = " + Convert.ToSingle(rowView[5]) + ", ISKONTOTUTAR = " + rowView[6].ToString().Replace(",", ".") + ", TOPLAMTUTAR = " + rowView[7].ToString().Replace(",", ".") + ", MIKTAR = " + rowView[1] + ", TOPLAMISKONTO =" + rowView[16].ToString().Replace(",", ".") + " WHERE MALKOD = '" + rowView[2] + "'";
                            c1 = new SqlCommand(cmmd, s);
                            c1.ExecuteNonQuery();
                        }
                    }
                    populate();
                    Hesap();
                }
            }
            catch
            {
                Carried.showMessage("Girdilerinizi kontrol edin.");
                //return;
            }
        }
        private void stok_dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    DataGrid grid = (DataGrid)sender;
                    grid.CommitEdit(DataGridEditingUnit.Row, true);

                    s = new SqlConnection(Carried.girisBaglantiLocal);
                    s.Open();
                    string cmmd;
                    int colindex1 = 0;
                    if (stok_dataGrid.SelectedItems.Count == 1) colindex1 = stok_dataGrid.CurrentCell.Column.DisplayIndex;
                    for (int i = 0; i < stok_dataGrid.Items.Count; i++)
                    {
                        DataRowView rowView = stok_dataGrid.Items[i] as DataRowView;
                        if (rowView[0].ToString() == "True")
                        {
                            if (Convert.ToInt32(rowView[1]) == 0) satısharmalkodlari.Remove(rowView[2].ToString());
                            rowView.BeginEdit();
                            if (colindex1 == 6)
                            {
                                if (Convert.ToDecimal(rowView[4]) != 0) rowView[5] = 100 * Convert.ToDecimal(rowView[6]) / Convert.ToDecimal(rowView[4]);
                                rowView[6] = Convert.ToDecimal(rowView[4]) * Convert.ToDecimal(rowView[5]) / 100;
                            }
                            else
                            {
                                rowView[6] = Convert.ToDecimal(rowView[4]) * Convert.ToDecimal(rowView[5]) / 100;
                                if (Convert.ToDecimal(rowView[4]) != 0) rowView[5] = 100 * Convert.ToDecimal(rowView[6]) / Convert.ToDecimal(rowView[4]);
                            }
                            //rowView[7] = Convert.ToDecimal(rowView[1]) * (Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[6])); //toplam tutar column hesaplama. miktar*(satısfiyatı-iskonto)
                            decimal kalansatfiyat = Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[6]); //satısfiyatı-iskonto1
                            decimal yeniiskonto, kalemiskontotoplami = Convert.ToDecimal(rowView[6]);
                            for (int j = 12; j <= 15; j++)
                            {
                                yeniiskonto = kalansatfiyat * Convert.ToDecimal(rowView[j]) / 100;
                                kalansatfiyat -= yeniiskonto;
                                kalemiskontotoplami += yeniiskonto;
                                cmmd = "update SATISHAR set KALEMISKONTOORAN" + (j - 10).ToString() + "=" + rowView[j] + ", KALEMISKONTO" + (j - 10).ToString() + "=" + yeniiskonto.ToString().Replace(",", ".") + " WHERE MALKOD = '" + rowView[2] + "'";
                                c1 = new SqlCommand(cmmd, s);
                                c1.ExecuteNonQuery();
                            }
                            rowView[16] = kalemiskontotoplami;
                            rowView[7] = Convert.ToDecimal(rowView[1]) * (Convert.ToDecimal(rowView[4]) - Convert.ToDecimal(rowView[16])); //toplam tutar column hesaplama. miktar*(satısfiyatı-toplamiskonto)
                            rowView.EndEdit();
                            int a = rowView[0].ToString() == "True" ? 1 : 0;
                            cmmd = "update SATISHAR set SECIM = " + a + ", " + columnname + " = " + rowView[4].ToString().Replace(",", ".") + ", ISKONTOORAN = " + Convert.ToSingle(rowView[5]) + ", ISKONTOTUTAR = " + rowView[6].ToString().Replace(",", ".") + ", TOPLAMTUTAR = " + rowView[7].ToString().Replace(",", ".") + ", MIKTAR = " + rowView[1] + ", TOPLAMISKONTO =" + rowView[16].ToString().Replace(",", ".") + " WHERE MALKOD = '" + rowView[2] + "'";
                            c1 = new SqlCommand(cmmd, s);
                            c1.ExecuteNonQuery();
                        }
                    }
                    populate();
                    Hesap();
                }
                catch
                {
                    Carried.showMessage("Girdilerinizi kontrol edin.");
                }
            }
        }
        
        int rowIndex, colindex;
        private void stok_dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (stok_dataGrid.SelectedCells.Count != 0)
            {
                //DataGridCellInfo cell = stok_dataGrid.SelectedCells[0];
                //rowIndex = stok_dataGrid.Items.IndexOf(cell.Item);
                //colindex = cell.Column.DisplayIndex;
                colindex = stok_dataGrid.CurrentCell.Column.DisplayIndex;
                rowIndex = stok_dataGrid.SelectedIndex;
                PopupYaz.IsOpen = true;
            }
        }
        #endregion



        #region ORTADAKİ BUTONLAR
        string depokodparam;
        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                object o;
                depokodparam = (o = new SqlCommand("select DEPOKOD from SMRTAPPKUL where KULLANICIADI ='" + Carried.girenKullanici + "'", s).ExecuteScalar()) != null ? o.ToString() : "";
                if (depokodparam == "") { Carried.showMessage("Önce parametreler ekranından depo kodlarının tanımlanması gerekmektedir."); return; }


                if (!String.IsNullOrWhiteSpace(txt1.Text) && !String.IsNullOrWhiteSpace(txt7.Text) && !String.IsNullOrWhiteSpace(txt8.Text) && !String.IsNullOrWhiteSpace(txt6.Text) && !String.IsNullOrWhiteSpace(txt_aratoplam.Text))
                {
                    //FATEVR KAYIT (fatura için)
                    tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
                    string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
                    s = new SqlConnection(constr);
                    s.Open();
                    string adres = new SqlCommand("select FATURAADRES1 from " + tabloadi + " where  hesapkod='" + txt2.Text + "'", s).ExecuteScalar().ToString() + " " + new SqlCommand("select FATURAADRES2 from " + tabloadi + " where hesapkod='" + txt2.Text + "'", s).ExecuteScalar().ToString();
                    string vergidairesi = new SqlCommand("select VERGIDAIRE from " + tabloadi + " where hesapkod='" + txt2.Text + "'", s).ExecuteScalar().ToString();
                    string sirketvergidairesi = new SqlCommand("select VERGIDAIRE from SRKKRT where SIRKETNO='" + new String('0', (3 - Carried.sirketNo.ToString().Length)) + Carried.sirketNo.ToString() + "'", s).ExecuteScalar().ToString();
                    string sirketvergino = new SqlCommand("select VERGIHESAPNO from SRKKRT where SIRKETNO='" + new String('0', (3 - Carried.sirketNo.ToString().Length)) + Carried.sirketNo.ToString() + "'", s).ExecuteScalar().ToString();
                    string sirketadres = new SqlCommand("select ADRES1 from  SRKKRT where SIRKETNO='" + new String('0', (3 - Carried.sirketNo.ToString().Length)) + Carried.sirketNo.ToString() + "'", s).ExecuteScalar().ToString();
                    string sirketunvan = new SqlCommand("select SIRKETAD from SRKKRT  where SIRKETNO='" + new String('0', (3 - Carried.sirketNo.ToString().Length)) + Carried.sirketNo.ToString() + "'", s).ExecuteScalar().ToString();
                    if (String.IsNullOrEmpty(adres) || String.IsNullOrEmpty(vergidairesi)) { Carried.showMessage("Bu hesap bilgilerine sahip kişi cari tabloda bulunmamaktadır. Verileri eşleyin ya da cari kart açın. Ardından kayıt işlemini tekrar edin."); return; }

                    s = new SqlConnection(Carried.girisBaglantiLocal);
                    s.Open();
                    c1 = new SqlCommand();
                    c1.CommandText = "insert into FATEVR(UNVAN, ADRES, VERGINO, VERGIDAIRESI, SIRKETUNVAN, SIRKETADRES, SIRKETVERGINO, SIRKETVERGIDAIRESI, EVRAKTIP, EVRAKNO, EVRAKTARIH, VADETARIH, DOVIZCINSI) VALUES(@UNVAN, @ADRES, @VERGINO, @VERGIDAIRESI, @SIRKETUNVAN, @SIRKETADRES, @SIRKETVERGINO, @SIRKETVERGIDAIRESI, @EVRAKTIP, @EVRAKNO, @EVRAKTARIH, @VADETARIH, @DOVIZCINSI)";
                    c1.Connection = s;
                    c1.Parameters.Add(new SqlParameter("UNVAN", txt1.Text));
                    c1.Parameters.Add(new SqlParameter("ADRES", adres));
                    c1.Parameters.Add(new SqlParameter("VERGINO", txt3.Text));
                    c1.Parameters.Add(new SqlParameter("VERGIDAIRESI", vergidairesi));
                    c1.Parameters.Add(new SqlParameter("SIRKETUNVAN", sirketunvan));
                    c1.Parameters.Add(new SqlParameter("SIRKETADRES", sirketadres));
                    c1.Parameters.Add(new SqlParameter("SIRKETVERGINO", sirketvergino));
                    c1.Parameters.Add(new SqlParameter("SIRKETVERGIDAIRESI", sirketvergidairesi));
                    c1.Parameters.Add(new SqlParameter("EVRAKTIP", EvrakTip));
                    c1.Parameters.Add(new SqlParameter("EVRAKNO", txt6.Text));
                    c1.Parameters.Add(new SqlParameter("EVRAKTARIH", txt7.Text));
                    c1.Parameters.Add(new SqlParameter("VADETARIH", txt8.Text));
                    c1.Parameters.Add(new SqlParameter("DOVIZCINSI", txt9.Text));
                    c1.ExecuteNonQuery();

                    //satıshar sırano ekle fatura için
                    for (int i = 0; i < stok_dataGrid.Items.Count; i++)
                    {
                        c1 = new SqlCommand("UPDATE x SET x.SIRANO ="+ (i+1) + " FROM ( SELECT SIRANO, ROW_NUMBER() OVER (ORDER BY ID) AS rownum FROM SATISHAR ) x where rownum=" + (i + 1) , s);
                        c1.ExecuteNonQuery();
                    }

                    //evrak no GÜNCELLEME
                    //s = new SqlConnection(Carried.girisBaglantiLocal);
                    //s.Open();
                    int evrakserino = Convert.ToInt32(new SqlCommand("select EVRAKNOSERINO from SMRTAPPKASA where KASANO='" + Carried.kasaNo + "'", s).ExecuteScalar());
                    c1 = new SqlCommand("update SMRTAPPKASA set EVRAKNOSERINO='" + (evrakserino + 1).ToString() + "' where KASANO='" + Carried.kasaNo + "'", s);
                    c1.ExecuteNonQuery();

                    //SMRTAPPHSP KAYIT (fatura için)
                    c1 = new SqlCommand();
                    c1.CommandText = "insert into SMRTAPPHSP(ARATOPLAM, KALEMISKONTOTUTAR, NETTUTAR, KDV, GENELTOPLAM, GENELISKONTOTUTAR, GENELNETTUTAR) VALUES(@ARATOPLAM, @KALEMISKONTOTUTAR, @NETTUTAR, @KDV, @GENELTOPLAM, @GENELISKONTOTUTAR, @GENELNETTUTAR)";
                    c1.Connection = s;
                    c1.Parameters.Add(new SqlParameter("ARATOPLAM", Convert.ToDecimal(txt_aratoplam.Text)));
                    c1.Parameters.Add(new SqlParameter("KALEMISKONTOTUTAR", Convert.ToDecimal(txt_kalemiskonto.Text)));
                    c1.Parameters.Add(new SqlParameter("NETTUTAR", Convert.ToDecimal(txt_nettutar.Text)));
                    c1.Parameters.Add(new SqlParameter("KDV", Convert.ToDecimal(txt_vergi.Text)));
                    c1.Parameters.Add(new SqlParameter("GENELTOPLAM", Convert.ToDecimal(txt_geneltoplam.Text)));
                    if (!String.IsNullOrWhiteSpace(txt_geneliskontotutar.Text)) c1.Parameters.Add(new SqlParameter("GENELISKONTOTUTAR", Convert.ToDecimal(txt_geneliskontotutar.Text)));
                    else c1.Parameters.AddWithValue("@GENELISKONTOTUTAR", 0);
                    if (!String.IsNullOrWhiteSpace(txt_genelnettutar.Text)) c1.Parameters.Add(new SqlParameter("GENELNETTUTAR", Convert.ToDecimal(txt_genelnettutar.Text)));
                    else c1.Parameters.AddWithValue("@GENELNETTUTAR", 0);
                    c1.ExecuteNonQuery();
                    
                    VeritabanıKayıt();

                    Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\DXApplication3\\DXApplication3\\obj\\Debug\\DXApplication3.exe");
                }
                else Carried.showMessage("Gerekli alanlar girilmeden e-fatura oluşturulamaz.");
            }
            catch(Exception ex)
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
        private void VeritabanıKayıt()
        {
            s = new SqlConnection(Carried.girisBaglantiLocal);
            s.Open();
            for (int i = 0; i < stok_dataGrid.Items.Count; i++)//SMRTAPPDEPO
            {
                DataRowView rowView = (stok_dataGrid.Items[i] as DataRowView);
                c1 = new SqlCommand("update SMRTAPPDEPO set STOKCIKIS = " + rowView[1] + " , BAKIYE = STOKGIRIS-STOKCIKIS WHERE MALKOD = " + rowView[2], s); 
                c1.ExecuteNonQuery();
            }

            bool netcon = Carried.CheckForInternetConnection();
            if(Carried.IsCPMconnected == true && netcon == false)
            {
                Carried.IsCPMconnected = false;
                Carried.showMessage("İnternet bulunamadığı için local bağlantıya geçildi");
            }

            bool isover30k = Convert.ToDecimal(txt_genelnettutar.Text) > 30000;
            bool efatablokayitgerekmi;

            if (Carried.IsCPMconnected == false || netcon == false)//internet yoksa veya local seçili ise
            {
                int srktefa = Convert.ToInt16(new SqlCommand("select EFATURADURUM from SRKKRT", s).ExecuteScalar());
                efatablokayitgerekmi = isover30k || srktefa == 1;

                
                decimal kdvtutar, ara, genel, iskonto, net, otvdeger, otvtutar, kdvkesinti; Single otvoran, kdvkesintioran; int otvtip, kdvdh;
                decimal kalemiskonto1, kalemiskonto2, kalemiskonto3, kalemiskonto4, kalemiskonto5, toplamiskonto; Single kalemiskontooran1, kalemiskontooran2, kalemiskontooran3, kalemiskontooran4, kalemiskontooran5, toplamiskontooran;
                for (int i = 0; i < stok_dataGrid.Items.Count; i++)
                {
                    DataRowView rowView = stok_dataGrid.Items[i] as DataRowView;

                    #region EVRBAS
                    //EVRBAS
                    c1 = new SqlCommand("INSERT_SMRTAPP_EVRBAS", s);
                    c1.CommandType = CommandType.StoredProcedure;
                    c1.ExecuteNonQuery();
                    #endregion

                    #region STHVRG, STHVDH, SMRTAPPSHAR, EFAKLM
                    //STHVRG, STHVDF, SMRTAPPSHAR
                    kdvdh = Convert.ToInt16(new SqlCommand("select kdvdh from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kdvtutar = Convert.ToDecimal(new SqlCommand("select kdvtutar from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    ara = Convert.ToDecimal(new SqlCommand("select ara from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    genel = Convert.ToDecimal(new SqlCommand("select genel from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    iskonto = Convert.ToDecimal(new SqlCommand("select kalemiskonto from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    net = Convert.ToDecimal(new SqlCommand("select net from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    otvdeger = Convert.ToDecimal(new SqlCommand("select OTVDEGER from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    otvtutar = Convert.ToDecimal(new SqlCommand("select otvtutar from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kdvkesinti = Convert.ToDecimal(new SqlCommand("select KDVKESINTI from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    otvoran = Convert.ToSingle(new SqlCommand("select otvoran from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kdvkesintioran = Convert.ToSingle(new SqlCommand("select KDVKESINTIORAN from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    otvtip = Convert.ToInt16(new SqlCommand("select OTVTIP from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskontooran1 = Convert.ToSingle(new SqlCommand("select ISKONTOORAN from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskontooran2 = Convert.ToSingle(new SqlCommand("select KALEMISKONTOORAN2 from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskontooran3 = Convert.ToSingle(new SqlCommand("select KALEMISKONTOORAN3 from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskontooran4 = Convert.ToSingle(new SqlCommand("select KALEMISKONTOORAN4 from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskontooran5 = Convert.ToSingle(new SqlCommand("select KALEMISKONTOORAN5 from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskonto1 = Convert.ToDecimal(new SqlCommand("select ISKONTOTUTAR from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskonto2 = Convert.ToDecimal(new SqlCommand("select KALEMISKONTO2 from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskonto3 = Convert.ToDecimal(new SqlCommand("select KALEMISKONTO3 from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskonto4 = Convert.ToDecimal(new SqlCommand("select KALEMISKONTO4 from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    kalemiskonto5 = Convert.ToDecimal(new SqlCommand("select KALEMISKONTO5 from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    toplamiskonto = Convert.ToDecimal(new SqlCommand("select TOPLAMISKONTO from SATISHAR where malkod ='" + rowView[2] + "'", s).ExecuteScalar());
                    toplamiskontooran = Convert.ToSingle(((kalemiskonto1+kalemiskonto2+kalemiskonto3+kalemiskonto4+kalemiskonto5)*100)/Convert.ToDecimal(rowView[4]));

                    c1 = new SqlCommand("INSERT_STHVRG_STHVDF_SMRTAPPSHAR", s);
                    c1.CommandType = CommandType.StoredProcedure;
                    c1.Parameters.Add(new SqlParameter("@VERGIHESAPNO", txt3.Text));//efaklm
                    c1.Parameters.Add(new SqlParameter("@BIRIM", rowView[8].ToString()));//efaklm
                    c1.Parameters.Add(new SqlParameter("@MALAD", rowView[3].ToString()));//efaklm
                    c1.Parameters.AddWithValue("@ISKONTOORAN", toplamiskontooran);//efaklm
                    c1.Parameters.Add(new SqlParameter("@GIRENKULLANICI", Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30))));
                    c1.Parameters.Add("@GIRENTARIH", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                    c1.Parameters.Add("@GIRENSAAT", SqlDbType.SmallDateTime).Value = DateTime.Now;
                    c1.Parameters.Add(new SqlParameter("@GIRENKAYNAK", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30))));
                    c1.Parameters.Add(new SqlParameter("@GIRENSURUM", "CPMSMARTAPP"));
                    c1.Parameters.Add(new SqlParameter("@DEGISTIRENKULLANICI", Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30))));
                    c1.Parameters.Add("@DEGISTIRENTARIH", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                    c1.Parameters.Add("@DEGISTIRENSAAT", SqlDbType.SmallDateTime).Value = DateTime.Now;
                    c1.Parameters.Add(new SqlParameter("@DEGISTIRENKAYNAK", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30))));
                    c1.Parameters.Add(new SqlParameter("@DEGISTIRENSURUM", "CPMSMARTAPP"));
                    c1.Parameters.AddWithValue("@SIRKETNO", new String('0', (3 - Carried.sirketNo.ToString().Length)) + Carried.sirketNo.ToString());
                    c1.Parameters.AddWithValue("@EVRAKNO", txt6.Text);
                    c1.Parameters.AddWithValue("@EVRAKTIP", Convert.ToInt16(txt5.Text));
                    c1.Parameters.AddWithValue("@MALKOD", rowView[2].ToString());
                    c1.Parameters.AddWithValue("@HESAPKOD", txt2.Text);
                    c1.Parameters.AddWithValue("@SIRANO", i);
                    c1.Parameters.AddWithValue("@KAYITTUR", 1);
                    c1.Parameters.AddWithValue("@KAYITDURUM", 1);
                    c1.Parameters.Add(new SqlParameter("EVRAKTARIH", SqlDbType.SmallDateTime).Value = Convert.ToDateTime(txt7.Text));
                    c1.Parameters.Add(new SqlParameter("EVRAKHAZIRLAYAN", txt12.Text));
                    c1.Parameters.AddWithValue("@GIRISCIKIS", 1);
                    c1.Parameters.AddWithValue("@ISLEMTIP", 2);
                    c1.Parameters.AddWithValue("@EVRAKMIKTAR", Convert.ToInt32(rowView[1]));
                    c1.Parameters.AddWithValue("@MIKTAR", Convert.ToInt32(rowView[1]));//TEKRAR
                    c1.Parameters.AddWithValue("@EVRAKBIRIM", rowView[8].ToString());
                    c1.Parameters.AddWithValue("@FIYAT", Convert.ToDecimal(rowView[4]));
                    c1.Parameters.AddWithValue("@BIRIMFIYAT", Convert.ToDecimal(rowView[4]) * getCurrency_ConvertToLira(rowView[11].ToString()));
                    c1.Parameters.AddWithValue("@TUTAR", ara);
                    c1.Parameters.AddWithValue("@EVRAKTUTAR", ara);//TEKRAR
                    c1.Parameters.AddWithValue("@ISKONTO", iskonto);//???
                    c1.Parameters.AddWithValue("@OTVTIP", otvtip);
                    c1.Parameters.AddWithValue("@OTVDEGER", otvdeger);
                    c1.Parameters.AddWithValue("@OTVTUTAR", otvtutar);
                    c1.Parameters.AddWithValue("@KDV", kdvtutar);
                    c1.Parameters.AddWithValue("@KDVORAN", Convert.ToSingle(rowView[9]));
                    c1.Parameters.AddWithValue("@KDVDH", kdvdh);
                    c1.Parameters.AddWithValue("@KDVKESINTI", kdvkesinti);
                    c1.Parameters.AddWithValue("@KDVKESINTIORAN", kdvkesintioran);
                    c1.Parameters.AddWithValue("@VADETARIH", SqlDbType.SmallDateTime).Value = Convert.ToDateTime(txt8.Text);
                    c1.Parameters.AddWithValue("@DOVIZCINS", rowView[11].ToString());
                    c1.Parameters.AddWithValue("@FIYATDOVIZCINS", rowView[11].ToString());//TEKRAR
                    c1.Parameters.AddWithValue("@FIYATDOVIZKUR", getCurrency_ConvertToLira(rowView[11].ToString()));//???
                    c1.Parameters.AddWithValue("@FIYATDOVIZDURUM", 0);//???
                    c1.Parameters.AddWithValue("@EVRAKDOVIZCINS", rowView[11].ToString());//TEKRAR
                    c1.Parameters.AddWithValue("@EVRAKDOVIZKUR", getCurrency_ConvertToLira(rowView[11].ToString()));//???TEKRAR
                    c1.Parameters.AddWithValue("@EVRAKBIRIMFIYAT", Convert.ToDecimal(rowView[4]));
                    c1.Parameters.AddWithValue("@EVRAKISKONTO", iskonto);//???
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTOORAN1", kalemiskontooran1));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTOORAN2", kalemiskontooran2));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTOORAN3", kalemiskontooran3));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTOORAN4", kalemiskontooran4));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTOORAN5", kalemiskontooran5));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTO1", kalemiskonto1));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTO2", kalemiskonto2));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTO3", kalemiskonto3));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTO4", kalemiskonto4));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTO5", kalemiskonto5));
                    c1.Parameters.Add(new SqlParameter("@KALEMISKONTOTUTAR", iskonto));//???TEKRAR
                    c1.Parameters.Add(new SqlParameter("@EVRAKISKONTOTUTAR", iskonto * getCurrency_ConvertToLira(rowView[11].ToString())));//???
                    c1.Parameters.Add(new SqlParameter("@FIYATTUTAR", ara));//TEKRAR
                    c1.Parameters.Add(new SqlParameter("@FIYATISKONTO", iskonto));//???TEKRAR
                    c1.Parameters.Add(new SqlParameter("@FIYATKDV", kdvtutar));//TEKRAR
                    c1.Parameters.Add(new SqlParameter("@FIYATKDVKESINTI", kdvkesinti));//TEKRAR
                    c1.Parameters.Add(new SqlParameter("@FIYATOTV", otvtutar));//TEKRAR
                    c1.Parameters.Add(new SqlParameter("@EVRAKKDV", kdvtutar * getCurrency_ConvertToLira(rowView[11].ToString())));
                    c1.Parameters.Add(new SqlParameter("@EVRAKKDVKESINTI", kdvkesinti * getCurrency_ConvertToLira(rowView[11].ToString())));
                    c1.Parameters.Add(new SqlParameter("@EVRAKKDVFARK", (kdvtutar - kdvkesinti) * getCurrency_ConvertToLira(rowView[11].ToString())));
                    c1.Parameters.Add(new SqlParameter("@EVRAKOTV", otvtutar * getCurrency_ConvertToLira(rowView[11].ToString())));
                    c1.Parameters.Add(new SqlParameter("@SKOD1", ""));
                    c1.Parameters.Add(new SqlParameter("@SKOD2", ""));
                    c1.Parameters.Add(new SqlParameter("@SKOD3", ""));
                    c1.Parameters.Add(new SqlParameter("@SKOD4", ""));
                    c1.Parameters.Add(new SqlParameter("@SKOD5", ""));
                    c1.Parameters.Add(new SqlParameter("@BKOD1", 0));
                    c1.Parameters.Add(new SqlParameter("@BKOD2", 0));
                    c1.Parameters.Add(new SqlParameter("@BKOD3", 0));
                    c1.Parameters.Add(new SqlParameter("@BKOD4", 0));
                    c1.Parameters.Add(new SqlParameter("@BKOD5", 0));
                    if (_switch.IsChecked == true) 
                    {
                        if (Carried.transferCode != null && Carried.transferCode.Contains("BKOD")) c1.Parameters.Add(new SqlParameter("@" + Carried.transferCode, 1));
                        if (Carried.transferCode != null && Carried.transferCode.Contains("SKOD")) c1.Parameters.Add(new SqlParameter("@" + Carried.transferCode, "1")); //???
                    }
                    //SOR--->BİRİMNETHACİM VE NETHACİM
                    #region sil
                    //c1.CommandText = "insert into SMRTAPPSHAR(SIRKETNO,EVRAKTIP,EVRAKNO,HESAPKOD,SIRANO,KAYITTUR,KAYITDURUM,KARTTIP,MALTIP,MALKOD," +
                    //    "EVRAKTARIH,EVRAKHAZIRLAYAN,ISLEMTIP,GIRISCIKIS,EVRAKMIKTAR,EVRAKBIRIM,BIRIMAGIRLIK,MIKTAR,FIYAT,FIYATDOVIZCINS,FIYATDOVIZKUR,FIYATDOVIZDURUM," +
                    //    "BIRIMFIYAT,TUTAR,ISKONTO,KDV,KDVORAN,KDVDH,KDVKESINTI,KDVKESINTIORAN,BIRIMBRUTAGIRLIK,BRUTAGIRLIK,BIRIMNETAGIRLIK," +
                    //    "NETAGIRLIK,BIRIMBRUTHACIM,BRUTHACIM,BIRIMNETHACIM,NETHACIM,BIRIMKAPADET,KAPADET,KALEMISKONTOORAN1,KALEMISKONTO1," +
                    //    "KALEMISKONTOTUTAR,EVRAKISKONTOTUTAR,BKOD1,BKOD2,BKOD3,BKOD4,BKOD5,BKOD6,BKOD7,BKOD8,BKOD9,BKOD10,NKOD1,NKOD2,NKOD3,NKOD4,NKOD5," +
                    //    "NKOD6,NKOD7,NKOD8,NKOD9,NKOD10,PROJEKOD,DEPOKOD,ACIKLAMA1,IRSALIYETTARIH,SIPARISTARIH,TEKLIFTARIH,ALIMTIP,ALIMMIKTAR," +
                    //    "ALIMFIYAT,SEVKTARIH,SEVKHESAPKOD,KARSIHESAPKOD,TAHMINTESLIMTARIH,SONTESLIMTARIH,VADETARIH,KDVVADETARIH,DOVIZCINS,DOVIZDURUM,BANKA,DOVIZTIP," +
                    //    "DOVIZTARIH,KDVDOVIZTARIH,DOVIZKUR,KDVDOVIZKUR,DOVIZTUTAR,DOVIZISKONTO,DOVIZOTV,DOVIZKDV,DOVIZKDVKESINTI,DOVIZKALEMISKONTOTUTAR," +
                    //    "DOVIZEVRAKISKONTOTUTAR,CARIDOVIZCINS,CARIDOVIZKUR,CARIDOVIZTUTAR,CARIDOVIZISKONTO,CARIDOVIZOTV,CARIDOVIZKDV,CARIDOVIZKDVKESINTI," +
                    //    "FIYATTUTAR,FIYATISKONTO,FIYATKDV,FIYATKDVKESINTI,FIYATOTV,FIYATSABLONNO,FIYATSEKLI,FIYATTARIH,FIYATGRUPLA,FIYATYONTEM,EVRAKBIRIMFIYAT," +
                    //    "EVRAKDOVIZCINS,EVRAKDOVIZKUR,EVRAKTUTAR,EVRAKISKONTO,EVRAKKDV,EVRAKKDVKESINTI,EVRAKKDVFARK,EVRAKOTV,EVRAKOTVFARK,ISKONTOSABLONNO,ISKONTOSEKLI," +
                    //    "ISKONTOTARIH,ISKONTOGRUPLA,ISKONTOYONTEM,PROMOSYONSABLONNO,PROMOSYONSEKLI,PROMOSYONTARIH,PROMOSYONGRUPLA,PROMOSYONTIP,PROMOSYONSIRANO," +
                    //    "EVRAKSEKLI,ODEMESEKLI,OPSIYON,EVRAKDURUM,KULLANILANMIKTAR,FAZLASEVK,FAZLASEVKKAYNAK,SEVKMIKTAR,SEVKTIP,TARIH2,MIKTAR2,TUTAR2,EVRAKNO2,TARIH3," +
                    //    "MIKTAR3,TUTAR3,TARIH4,TARIH5,SIRANO2,MALIYETSIRANO,MHSENTTABLONO,MHSDURUM,MHSFISTARIH,MHSFISTIP,MHSFISNO,SONKAYNAKEVRAKTIP,SONKAYNAKEVRAKNO," +
                    //    "SONKAYNAKHESAPKOD,SONKAYNAKSIRANO,SONKAYNAKEVRAKTARIH,GIRENKULLANICI,GIRENTARIH,GIRENSAAT,GIRENKAYNAK,GIRENSURUM,DEGISTIRENKULLANICI,DEGISTIRENTARIH,DEGISTIRENSAAT,DEGISTIRENKAYNAK," +
                    //    "DEGISTIRENSURUM) VALUES()";

                    c1.Parameters.Add(new SqlParameter("SIRKETNO", new String('0', (3 - Carried.sirketNo.ToString().Length)) + Carried.sirketNo.ToString()));
                    c1.Parameters.Add(new SqlParameter("EVRAKNO", txt6.Text));
                    c1.Parameters.Add(new SqlParameter("EVRAKTIP", txt5.Text));
                    c1.Parameters.Add(new SqlParameter("HESAPKOD", txt2.Text));
                    c1.Parameters.AddWithValue("SIRANO", i);
                    c1.Parameters.AddWithValue("KAYITTUR",  1);
                    c1.Parameters.AddWithValue("KAYITDURUM", 1);
                    c1.Parameters.Add(new SqlParameter("MALKOD", rowView[2].ToString()));
                    c1.Parameters.Add(new SqlParameter("EVRAKTARIH", txt7.Text));
                    c1.Parameters.Add(new SqlParameter("EVRAKHAZIRLAYAN", txt12.Text));
                    c1.Parameters.Add(new SqlParameter("ISLEMTIP",  2));
                    c1.Parameters.Add(new SqlParameter("GIRISCIKIS",  1));
                    c1.Parameters.Add(new SqlParameter("EVRAKMIKTAR", Convert.ToInt32(rowView[1])));
                    c1.Parameters.Add(new SqlParameter("EVRAKBIRIM", rowView[8].ToString()));
                    //c1.Parameters.Add(new SqlParameter("BIRIMAGIRLIK",  ));//???
                    c1.Parameters.AddWithValue("MIKTAR", Convert.ToInt32(rowView[1]));
                    c1.Parameters.Add(new SqlParameter("FIYAT", Convert.ToDecimal(rowView[4])));
                    c1.Parameters.Add(new SqlParameter("FIYATDOVIZDURUM",  0));
                    c1.Parameters.Add(new SqlParameter("BIRIMFIYAT", Convert.ToDecimal(rowView[4])));
                    c1.Parameters.Add(new SqlParameter("TUTAR", ara));
                    c1.Parameters.Add(new SqlParameter("ISKONTO", iskonto));
                    c1.Parameters.Add(new SqlParameter("OTVTIP",  otvtip));
                    c1.Parameters.Add(new SqlParameter("OTVDEGER",  otvdeger));
                    c1.Parameters.Add(new SqlParameter("KDV",  kdvtutar));
                    c1.Parameters.Add(new SqlParameter("KDVORAN", Convert.ToSingle(rowView[9])));
                    c1.Parameters.Add(new SqlParameter("KDVDH",  kdvdh));
                    c1.Parameters.Add(new SqlParameter("KDVKESINTI", kdvkesinti));
                    c1.Parameters.Add(new SqlParameter("KDVKESINTIORAN", kdvkesintioran));
                    c1.Parameters.Add(new SqlParameter("EVRAKTUTAR", ara));
                    c1.Parameters.Add(new SqlParameter("VADETARIH", Convert.ToDateTime(txt8.Text)));
                    c1.Parameters.Add(new SqlParameter("OTV", otvtutar));
                    c1.Parameters.Add(new SqlParameter("FIYATDOVIZCINS", rowView[11].ToString()));
                    c1.Parameters.Add(new SqlParameter("FIYATDOVIZKUR", 1));
                    c1.Parameters.Add(new SqlParameter("DOVIZCINS", rowView[11].ToString()));
                    c1.Parameters.Add(new SqlParameter("EVRAKDOVIZKUR", 1));
                    c1.Parameters.Add(new SqlParameter("EVRAKDOVIZCINS", rowView[11].ToString()));
                    c1.Parameters.Add(new SqlParameter("EVRAKBIRIMFIYAT", rowView[4]));
                    c1.Parameters.Add(new SqlParameter("EVRAKISKONTO", rowView[6]));
                    #endregion
                    //c1.Parameters.Add(new SqlParameter("KALEMSN", ));***
                    //c1.Parameters.Add(new SqlParameter("EVRAKSN", ));***

                    //c1.Parameters.Add(new SqlParameter("EVRAKOTVFARK", ""));
                    //c1.Parameters.Add(new SqlParameter("BIRIMAGIRLIK", ));
                    //c1.Parameters.Add(new SqlParameter("FIYATSABLONNO",  ""));
                    //c1.Parameters.Add(new SqlParameter("FIYATSEKLI",  ""));
                    //c1.Parameters.Add(new SqlParameter("FIYATGRUPLA",  ""));
                    //c1.Parameters.Add(new SqlParameter("FIYATYONTEM",  ""));

                    //c1.Parameters.Add(new SqlParameter("TICARETDOSYATIP",  ));
                    c1.Parameters.Add(new SqlParameter("@DEPOKOD", depokodparam));
                    //c1.Parameters.Add(new SqlParameter("SURUMNO",  ));
                    //c1.Parameters.Add(new SqlParameter("IRSALIYETTARIH",  ));
                    //c1.Parameters.Add(new SqlParameter("SIPARISTARIH",  ));
                    //c1.Parameters.Add(new SqlParameter("TEKLIFTARIH",  ));
                    //c1.Parameters.Add(new SqlParameter("SEVKTARIH",  ));
                    //c1.Parameters.Add(new SqlParameter("@SEVKHESAPKOD", ""));//TEKRAR   txt2.Text
                    //c1.Parameters.Add(new SqlParameter("@KARSIHESAPKOD",  ""));//??????????????????????????????????
                    //c1.Parameters.Add(new SqlParameter("TAHMINTESLIMTARIH",  ));
                    //c1.Parameters.Add(new SqlParameter("@SONTESLIMTARIH",  ""));
                    //c1.Parameters.Add(new SqlParameter("@KDVVADETARIH", ""));
                    //c1.Parameters.Add(new SqlParameter("DOVIZDURUM",  ));
                    //c1.Parameters.Add(new SqlParameter("BANKA",  ));
                    //c1.Parameters.Add(new SqlParameter("DOVIZTIP",  ));
                    c1.Parameters.Add(new SqlParameter("@DOVIZTARIH", ""));//evrak tarihi
                    //c1.Parameters.Add(new SqlParameter("@KDVDOVIZTARIH",  ""));
                    c1.Parameters.Add(new SqlParameter("@DOVIZKUR", ""));
                    //c1.Parameters.Add(new SqlParameter("KDVDOVIZKUR",  ));
                    c1.Parameters.Add(new SqlParameter("@DOVIZTUTAR", ""));//evraktutar*doviztur
                    c1.Parameters.Add(new SqlParameter("@DOVIZISKONTO",  ""));//toplam iskontonun döviz hali
                    //c1.Parameters.Add(new SqlParameter("DOVIZOTV",  ));
                    c1.Parameters.Add(new SqlParameter("@DOVIZKDV",  "")); //toplam kdv'nin  döviz hali
                    //c1.Parameters.Add(new SqlParameter("DOVIZKDVKESINTI",  ));
                    c1.Parameters.Add(new SqlParameter("@DOVIZKALEMISKONTOTUTAR",  ""));
                    c1.Parameters.Add(new SqlParameter("@DOVIZEVRAKISKONTOTUTAR",  ""));
                    //c1.Parameters.Add(new SqlParameter("CARIDOVIZCINS",  ));
                    c1.Parameters.Add(new SqlParameter("@CARIDOVIZKUR",  ""));
                    c1.Parameters.Add(new SqlParameter("@CARIDOVIZTUTAR",  ""));
                    c1.Parameters.Add(new SqlParameter("@CARIDOVIZISKONTO",  ""));
                    //c1.Parameters.Add(new SqlParameter("CARIDOVIZOTV",  ));
                    c1.Parameters.Add(new SqlParameter("@CARIDOVIZKDV", "" ));
                    c1.Parameters.Add(new SqlParameter("@FIYATTARIH", ""));
                    //c1.Parameters.Add(new SqlParameter("CARIDOVIZKDVKESINTI",  ));
                    //c1.Parameters.Add(new SqlParameter("ISKONTOSABLONNO",  ));
                    //c1.Parameters.Add(new SqlParameter("ISKONTOSEKLI",  ));
                    c1.Parameters.Add(new SqlParameter("@ISKONTOTARIH", "" ));
                    //c1.Parameters.Add(new SqlParameter("ISKONTOGRUPLA",  ));
                    //c1.Parameters.Add(new SqlParameter("ISKONTOYONTEM",  ));
                    //c1.Parameters.Add(new SqlParameter("PROMOSYONSABLONNO",  ));
                    //c1.Parameters.Add(new SqlParameter("PROMOSYONSEKLI",  ));
                    c1.Parameters.Add(new SqlParameter("@PROMOSYONTARIH", ""));//????????????
                    //c1.Parameters.Add(new SqlParameter("PROMOSYONGRUPLA",  ));
                    //c1.Parameters.Add(new SqlParameter("PROMOSYONTIP",  ));
                    //c1.Parameters.Add(new SqlParameter("PROMOSYONSIRANO",  ));
                    c1.Parameters.Add(new SqlParameter("@EVRAKSEKLI",  ""));//????????????
                    c1.Parameters.Add(new SqlParameter("@LKOD3",  ""));
                    c1.Parameters.Add(new SqlParameter("@LKOD4",  ""));
                    //c1.Parameters.Add(new SqlParameter("ODEMESEKLI",  ));
                    //c1.Parameters.Add(new SqlParameter("OPSIYON",  ));
                    //c1.Parameters.Add(new SqlParameter("EVRAKDURUM",  ));
                    //c1.Parameters.Add(new SqlParameter("KULLANILANMIKTAR",  ));
                    //c1.Parameters.Add(new SqlParameter("FAZLASEVK",  ));
                    //c1.Parameters.Add(new SqlParameter("FAZLASEVKKAYNAK",  ));
                    //c1.Parameters.Add(new SqlParameter("SEVKMIKTAR",  ));
                    //c1.Parameters.Add(new SqlParameter("SEVKTIP",  ));
                    //c1.Parameters.Add(new SqlParameter("TARIH2",  ));
                    //c1.Parameters.Add(new SqlParameter("MIKTAR2",  ));
                    //c1.Parameters.Add(new SqlParameter("TUTAR2",  ));
                    //c1.Parameters.Add(new SqlParameter("EVRAKNO2",  ));
                    //c1.Parameters.Add(new SqlParameter("TARIH3",  ));
                    //c1.Parameters.Add(new SqlParameter("MIKTAR3",  ));
                    //c1.Parameters.Add(new SqlParameter("TUTAR3",  ));
                    //c1.Parameters.Add(new SqlParameter("TARIH4",  ));
                    //c1.Parameters.Add(new SqlParameter("TARIH5",  ));
                    //c1.Parameters.Add(new SqlParameter("SIRANO2",  ));
                    //c1.Parameters.Add(new SqlParameter("MALIYETSIRANO",  ));
                    //c1.Parameters.Add(new SqlParameter("MHSENTTABLONO",  ));
                    //c1.Parameters.Add(new SqlParameter("MHSDURUM",  ));
                    //c1.Parameters.Add(new SqlParameter("MHSFISTARIH",  ));
                    //c1.Parameters.Add(new SqlParameter("MHSFISTIP",  ));
                    //c1.Parameters.Add(new SqlParameter("MHSFISNO",  ));
                    //c1.Parameters.Add(new SqlParameter("SONKAYNAKEVRAKTIP",  ));
                    //c1.Parameters.Add(new SqlParameter("SONKAYNAKEVRAKNO",  ));
                    //c1.Parameters.Add(new SqlParameter("SONKAYNAKHESAPKOD",  ));
                    //c1.Parameters.Add(new SqlParameter("SONKAYNAKSIRANO",  ));
                    //c1.Parameters.Add(new SqlParameter("SONKAYNAKEVRAKTARIH",  ));
                    c1.ExecuteNonQuery();
                    #endregion

                    #region EFABAS
                    //EFABAS
                    int et = efat.Visibility == Visibility.Visible ? 1 : 0;
                    if (efatablokayitgerekmi == true)
                    {
                        c1 = new SqlCommand("INSERT_SMRTAPP_EFABAS", s);
                        c1.CommandType = CommandType.StoredProcedure;
                        c1.Parameters.AddWithValue("@ETICARET", et);
                        c1.Parameters.AddWithValue("@EARSIV", ears.Visibility == Visibility.Visible ? 1 : 0);
                        //if(et == 1) c1.Parameters.AddWithValue("@PKETIKET", );
                        //else c1.Parameters.AddWithValue("@PKETIKET", "");
                        c1.ExecuteNonQuery();
                    }
                    #endregion


                }
            }

            else if (netcon == true && Carried.IsCPMconnected == true)//internet varsa ve cpm şeçili ise
            {
                int srktefa = Convert.ToInt16(new SqlCommand("select EFATURADURUM from SRKKRT", s).ExecuteScalar());
                efatablokayitgerekmi = isover30k || srktefa == 1;
                //STKHAR
                decimal kdvtutar, ara, genel, iskonto, net, otvdeger, otvtutar, kdvkesinti; Single otvoran, kdvkesintioran; int otvtip; bool kdvdh;
                s = new SqlConnection(Carried.girisBaglantiCPM);
                s.Open();
                for (int i = 0; i < stok_dataGrid.Items.Count; i++)
                {
                    DataRowView rowView = (stok_dataGrid.Items[i] as DataRowView);
                    c1 = new SqlCommand();
                    c1.Connection = s;
                   
                }
            }

        }
        Encoding en;
        string strXLST = "";
        private void Goruntule_Click(object sender, RoutedEventArgs e)
        {
            string t1 = null;
            bool exist = File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt");
            if(exist) t1 = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdi.txt");
            if ( !exist || String.IsNullOrWhiteSpace(t1)) PopupBelge.IsOpen = true;
            else
                Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\reportReader.exe");

        }
        private void lbBelge_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string yaz = lbBelge.Items[lbBelge.SelectedIndex].ToString();
            File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdiFromIslem.txt").Dispose();
            Carried.GrantAccess(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdiFromIslem.txt");
            File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\DXApplication3\\DXApplication3\\bin\\Debug\\RaporAdiFromIslem.txt", yaz);
            PopupBelge.IsOpen = false;
            lbBelge.SelectedItem = null;
            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\reportReader\\reportReader\\bin\\Debug\\reportReader.exe");
        }
        private void Yeni_Click(object sender, RoutedEventArgs e)
        {
            string constr = Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();
            c1 = new SqlCommand("delete from SATISHAR", s);
            c1.ExecuteNonQuery();
            populate();
            c1 = new SqlCommand("delete from SMRTAPPHSP", s);
            c1.ExecuteNonQuery();
            c1 = new SqlCommand("delete from FATEVR", s);
            c1.ExecuteNonQuery();
            markaadi.Clear();
            txt_kalemiskonto.Text = "";
            txt_aratoplam.Text = "";
            txt_genelnettutar.Text = "";
            txt_geneliskontotutar.Text = "";
            txt_geneltoplam.Text = "";
            txt_genelnettutar.Text = "";
            txt_vergi.Text = "";
            txt_nettutar.Text = "";
            markaiskonto.Text = "";
            txtmiktar.Text = "";
            this.depo_dataGrid.ItemsSource = null;
            satısharmalkodlari.Clear();
            foreach (UIElement item in eklenecekyer.Children)
            {
                if (item.GetType().Equals(typeof(TextBox)))
                {
                    TextBox tbox = (TextBox)item;
                    tbox.Clear();
                }
            }
            txt1.Text = "";
            txt2.Text = "";
            txt3.Text = "";
            txt4.Text = "";
            txt5.Text = "";
            txt6.Text = "";
            txt7.Text = "";
            txt8.Text = "";
            txt9.Text = "";
            txt10.Text = "";
            txt11.Text = "";
            txt12.Text = "";
            txtmiktar.Text = "";
            txtBrkd.Text = "";
            Odeme.parcaliodeme.source = null;
        }
        private void Sil_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion



        private void txtevrakkod_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

            }
        }



        #region ÖDEME
        private void odeme_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txt_genelnettutar.Text) || !String.IsNullOrWhiteSpace(txt_geneltoplam.Text))
                PopupOdeme.IsOpen = true;
            else Carried.showMessage("Önce satış yapmanız gerekmektedir.");
        }
        private void nakit_Click(object sender, RoutedEventArgs e)
        {
            string text = String.IsNullOrWhiteSpace(txt_genelnettutar.Text) ? txt_geneltoplam.Text : txt_genelnettutar.Text;
            Odeme.nakit w = new Odeme.nakit(Convert.ToDecimal(text), Convert.ToDecimal(text), "NAKIT");
            PopupOdeme.IsOpen = false;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
            w.Show();
            w.Topmost = true;
        }
        private void kredi_Click(object sender, RoutedEventArgs e)
        {
            string text = String.IsNullOrWhiteSpace(txt_genelnettutar.Text) ? txt_geneltoplam.Text : txt_genelnettutar.Text;
            Odeme.kredi w = new Odeme.kredi(Convert.ToDecimal(text));
            PopupOdeme.IsOpen = false;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
            w.Show();
            w.Topmost = true;
        }
        private void parcali_Click(object sender, RoutedEventArgs e)
        {
            string text = String.IsNullOrWhiteSpace(txt_genelnettutar.Text) ? txt_geneltoplam.Text : txt_genelnettutar.Text;
            Odeme.parcaliodeme w = new Odeme.parcaliodeme(Convert.ToDecimal(text), Convert.ToDecimal(text), 0);
            PopupOdeme.IsOpen = false;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
            w.Show();
            w.Topmost = true;
        }
        #endregion



        private void ButtonTus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (!String.IsNullOrWhiteSpace(txtyazilan.Text))
            {
                int index1 = txtyazilan.SelectionStart;
                if (txtyazilan.SelectionLength > 0)
                {
                    txtyazilan.Text = txtyazilan.Text.Remove(index1, txtyazilan.SelectionLength);
                    txtyazilan.Text = txtyazilan.Text.Insert(index1, btn.Content.ToString());
                    txtyazilan.SelectionStart = index1 + 1;
                }
                else
                {
                    if (txtyazilan.Text != "0")
                    {
                        txtyazilan.Text = txtyazilan.Text.Insert(index1, btn.Content.ToString());//$"{txtyazilan.Text}{btn.Content}";
                        txtyazilan.SelectionStart = index1 + 1;
                    }
                    else
                    {
                        txtyazilan.Text = btn.Content.ToString();
                        txtyazilan.SelectionStart = 1;
                    }
                }
            }
            else { txtyazilan.Text = btn.Content.ToString(); txtyazilan.SelectionStart = 1; }
        }
        private void ButtonTus_Click_Sil(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtyazilan.Text))
            {
                if (txtyazilan.SelectionLength > 0)
                {
                    int index2 = txtyazilan.SelectionStart;
                    txtyazilan.Text = txtyazilan.Text.Remove(index2, txtyazilan.SelectionLength);
                    txtyazilan.SelectionStart = index2;
                }
                else
                {
                    if (txtyazilan.SelectionStart == 0) txtyazilan.SelectionStart = txtyazilan.Text.Length;
                    int index1 = txtyazilan.SelectionStart - 1;
                    txtyazilan.Text = txtyazilan.Text.Remove(index1, 1);
                    txtyazilan.SelectionStart = index1;
                }
            }
        }
        private void ButtonTus_Click_Tamam(object sender, RoutedEventArgs e)
        {
            //stok_dataGrid.CurrentCell = new DataGridCellInfo(stok_dataGrid.Items[rowIndex], stok_dataGrid.Columns[colindex]);
            DataRowView rowView = stok_dataGrid.Items[rowIndex] as DataRowView; 
            rowView.BeginEdit();
            if(!String.IsNullOrWhiteSpace(txtyazilan.Text)) rowView[colindex] = Convert.ToDecimal(txtyazilan.Text);
            rowView.EndEdit();
            stok_dataGrid.Items.Refresh();
            txtyazilan.Clear();

            stok_dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            s = new SqlConnection(Carried.girisBaglantiLocal);
            s.Open();
            string cmmd;
            for (int i = 0; i < stok_dataGrid.Items.Count; i++)
            {
                DataRowView rowView1 = stok_dataGrid.Items[i] as DataRowView;
                if (rowView1[0].ToString() == "True")
                {
                    if (Convert.ToInt32(rowView1[1]) == 0) satısharmalkodlari.Remove(rowView1[2].ToString());
                    rowView1.BeginEdit();
                    if (colindex == 6)
                    {
                        if (Convert.ToDecimal(rowView1[4]) != 0) rowView1[5] = 100 * Convert.ToDecimal(rowView1[6]) / Convert.ToDecimal(rowView1[4]);
                        rowView1[6] = Convert.ToDecimal(rowView1[4]) * Convert.ToDecimal(rowView1[5]) / 100;
                    }
                    else
                    {
                        rowView1[6] = Convert.ToDecimal(rowView1[4]) * Convert.ToDecimal(rowView1[5]) / 100;
                        if (Convert.ToDecimal(rowView1[4]) != 0) rowView1[5] = 100 * Convert.ToDecimal(rowView1[6]) / Convert.ToDecimal(rowView1[4]);
                    }
                    //rowView1[7] = Convert.ToDecimal(rowView1[1]) * (Convert.ToDecimal(rowView1[4]) - Convert.ToDecimal(rowView1[6])); //toplam tutar column hesaplama. miktar*(satısfiyatı-iskonto)
                    decimal kalansatfiyat = Convert.ToDecimal(rowView1[4]) - Convert.ToDecimal(rowView1[6]); //satısfiyatı-iskonto1
                    decimal yeniiskonto, kalemiskontotoplami = Convert.ToDecimal(rowView1[6]);
                    for (int j = 12; j <= 15; j++)
                    {
                        yeniiskonto = kalansatfiyat * Convert.ToDecimal(rowView1[j]) / 100;
                        kalansatfiyat -= yeniiskonto;
                        kalemiskontotoplami += yeniiskonto;
                        cmmd = "update SATISHAR set KALEMISKONTOORAN" + (j - 10).ToString() + "=" + rowView[j] + ", KALEMISKONTO" + (j - 10).ToString() + "=" + yeniiskonto.ToString().Replace(",", ".") + " WHERE MALKOD = '" + rowView[2] + "'";
                        c1 = new SqlCommand(cmmd, s);
                        c1.ExecuteNonQuery();
                    }
                    rowView1[16] = kalemiskontotoplami;
                    rowView1[7] = Convert.ToDecimal(rowView1[1]) * (Convert.ToDecimal(rowView1[4]) - Convert.ToDecimal(rowView1[16])); //toplam tutar column hesaplama. miktar*(satısfiyatı-toplamiskonto)
                    rowView1.EndEdit();
                    int a = rowView1[0].ToString() == "True" ? 1 : 0;
                    cmmd = "update SATISHAR set SECIM = " + a + ", " + columnname + " = " + rowView1[4].ToString().Replace(",", ".") + ", ISKONTOORAN = " + Convert.ToSingle(rowView1[5]) + ", ISKONTOTUTAR = " + rowView1[6].ToString().Replace(",", ".") + ", TOPLAMTUTAR = " + rowView1[7].ToString().Replace(",", ".") + ", MIKTAR = " + rowView1[1] + ", TOPLAMISKONTO =" + rowView1[16].ToString().Replace(",", ".") + " WHERE MALKOD = '" + rowView1[2] + "'";
                    c1 = new SqlCommand(cmmd, s);
                    c1.ExecuteNonQuery();
                }
            }
            populate();
            Hesap();
        }



        #region sabit
        decimal FromTryToEur = 0, FromTryToUsd = 0, FromUsdToTry = 0, FromEurToTry = 0, FromEurToUsd = 0, FromUsdToEur = 0;
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            bool netcon = Carried.CheckForInternetConnection();
            if (netcon == true)
            {
                try
                {
                    //FromTryToEur = Carried.CurrencyConversion(1, "try", "eur");
                    //FromTryToUsd = Carried.CurrencyConversion(1, "try", "usd");
                    //FromUsdToTry = Carried.CurrencyConversion(1, "usd", "try");
                    //FromEurToTry = Carried.CurrencyConversion(1, "eur", "try");
                    //FromEurToUsd = Carried.CurrencyConversion(1, "eur", "usd");
                    //FromUsdToEur = Carried.CurrencyConversion(1, "usd", "eur");
                    FromEurToTry = Carried.MerkezBankasi_CurrencyConversion("EUR");
                    FromUsdToTry = Carried.MerkezBankasi_CurrencyConversion("USD"); //MessageBox.Show(FromEurToTry.ToString() + "  " + FromUsdToTry.ToString());
                    FromTryToEur = 1/ FromEurToTry;
                    FromTryToUsd = 1 / FromUsdToTry;
                    FromEurToUsd = FromUsdToTry / FromEurToTry;
                    FromUsdToEur = FromEurToTry / FromUsdToTry;
                }
                catch (Exception ex)
                {
                    Carried.showMessage(ex.Message);
                }
            }
            else
            {
                try
                {
                    s = new SqlConnection(Carried.girisBaglantiLocal);
                    s.Open();
                    FromTryToEur = Convert.ToDecimal(new SqlCommand("select ORAN from SMRTAPPDVZ where SMRTAPPDVZ.[FROM] = 'TRY' AND SMRTAPPDVZ.[TO] = 'EUR' ORDER BY TARIH DESC", s).ExecuteScalar());
                    FromTryToUsd = Convert.ToDecimal(new SqlCommand("select ORAN from SMRTAPPDVZ where SMRTAPPDVZ.[FROM] = 'TRY' AND SMRTAPPDVZ.[TO] = 'USD' ORDER BY TARIH DESC", s).ExecuteScalar());
                    FromUsdToTry = Convert.ToDecimal(new SqlCommand("select ORAN from SMRTAPPDVZ where SMRTAPPDVZ.[FROM] = 'USD' AND SMRTAPPDVZ.[TO] = 'TRY' ORDER BY TARIH DESC", s).ExecuteScalar());
                    FromEurToTry = Convert.ToDecimal(new SqlCommand("select ORAN from SMRTAPPDVZ where SMRTAPPDVZ.[FROM] = 'EUR' AND SMRTAPPDVZ.[TO] = 'TRY' ORDER BY TARIH DESC", s).ExecuteScalar());
                    FromEurToUsd = Convert.ToDecimal(new SqlCommand("select ORAN from SMRTAPPDVZ where SMRTAPPDVZ.[FROM] = 'EUR' AND SMRTAPPDVZ.[TO] = 'USD' ORDER BY TARIH DESC", s).ExecuteScalar());
                    FromUsdToEur = Convert.ToDecimal(new SqlCommand("select ORAN from SMRTAPPDVZ where SMRTAPPDVZ.[FROM] = 'USD' AND SMRTAPPDVZ.[TO] = 'EUR' ORDER BY TARIH DESC", s).ExecuteScalar());
                    s.Close();
                }
                catch (Exception ex)
                {
                    Carried.showMessage(ex.Message);
                }
            }
        }
        private decimal getCurrency(string stkdvz)//stok para birimini cari para birimine çevirmek için
        {
            if (!String.IsNullOrWhiteSpace(txt9.Text))
            {
                string cardvz = txt9.Text;
                if ((stkdvz == "" || stkdvz == "TRY") && cardvz == "TL") return 1;
                else if ((stkdvz == "TRY" || stkdvz == "") && cardvz == "USD") return FromTryToUsd;
                else if ((stkdvz == "TRY" || stkdvz == "") && cardvz == "EUR") return FromTryToEur;
                else if (stkdvz == "EUR" && cardvz == "EUR") return 1;
                else if (stkdvz == "EUR" && cardvz == "USD") return FromEurToUsd;
                else if (stkdvz == "EUR" && cardvz == "TL") return FromEurToTry;
                else if (stkdvz == "USD" && cardvz == "USD") return 1;
                else if (stkdvz == "USD" && cardvz == "TL") return FromUsdToTry;
                else if (stkdvz == "USD" && cardvz == "EUR") return FromUsdToEur;
                else return 1;
            }
            else return 1;
        }
        private decimal getCurrency_ConvertToLira(string stkdvz)
        {
            if (stkdvz == "EUR") return FromEurToTry;
            else if (stkdvz == "USD") return FromUsdToTry;
            else if (stkdvz == "TRY" || stkdvz == "TL" || string.IsNullOrWhiteSpace(stkdvz)) return 1;
            else return 1;
        }
        private void txt9_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (stok_dataGrid.Items.Count != 0)
            {
                Hesap();
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string constr = Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();
            c1 = new SqlCommand("delete from SATISHAR", s);
            c1.ExecuteNonQuery();
            //c1 = new SqlCommand("delete from SMRTAPPHSP", s);
            //c1.ExecuteNonQuery();
            //c1 = new SqlCommand("delete from FATEVR", s);
            //c1.ExecuteNonQuery();
        }


        string columnname = "SATISFIYAT1";
        private void rb1_Checked(object sender, RoutedEventArgs e)
        {
            columnname = "SATISFIYAT1";
            secilenUrunSatısFiyatıDegistir();
        }
        private void rb2_Checked(object sender, RoutedEventArgs e)
        {
            columnname = "SATISFIYAT2";
            secilenUrunSatısFiyatıDegistir();
        }
        private void rb3_Checked(object sender, RoutedEventArgs e)
        {
            columnname = "SATISFIYAT3";
            secilenUrunSatısFiyatıDegistir();
        }
        private void rb4_Checked(object sender, RoutedEventArgs e)
        {
            columnname = "SATISFIYAT4";
            secilenUrunSatısFiyatıDegistir();
        }
        private void rb5_Checked(object sender, RoutedEventArgs e)
        {
            columnname = "SATISFIYAT5";
            secilenUrunSatısFiyatıDegistir();
        }
        private void secilenUrunSatısFiyatıDegistir()
        {
            if (stok_dataGrid.SelectedItems.Count == 0) return;
            else 
            {
                for(int i=0; i < stok_dataGrid.SelectedItems.Count; i++)
                {
                    tabloadi = Carried.IsCPMconnected == true ? "STKKRT" : "SMRTAPPSKRT";
                    string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
                    s = new SqlConnection(constr);
                    s.Open();
                    sl = new SqlConnection(Carried.girisBaglantiLocal);
                    sl.Open();
                    DataRowView row = (DataRowView)stok_dataGrid.SelectedItems[i];
                    c1 = new SqlCommand("select " + columnname + " from " + tabloadi + " WHERE MALKOD='" + row[2] + "'", s);
                    object o = c1.ExecuteScalar();
                    c1 = new SqlCommand("select " + columnname + "DOVIZCINS from " + tabloadi + " WHERE MALKOD='" + row[2] + "'", s);
                    object dc = c1.ExecuteScalar();
                    if (o != null)
                    {
                        c1 = new SqlCommand("update SATISHAR set DOVIZCINS = '" + dc.ToString() + "', SATISFIYAT1 = " + o.ToString().Replace(",", ".") + " WHERE MALKOD='" + row[2] + "'", sl);
                        c1.ExecuteNonQuery();
                        c1 = new SqlCommand("update SATISHAR set TOPLAMTUTAR=(SATISFIYAT1-TOPLAMISKONTO)*MIKTAR WHERE MALKOD='" + row[2] + "'", sl);
                        c1.ExecuteNonQuery();
                    }
                }
                populate();
                Hesap();
            }
        }
        private void selectall_Click(object sender, RoutedEventArgs e)
        {
            if(stok_dataGrid.SelectedItems.Count <= 1 ) stok_dataGrid.SelectAll();
            else stok_dataGrid.UnselectAll();
        }


        //private string Transform(string XMLPage, string XSLStylesheet, out string ErrorMessage)
        //{

        //    string result = "";
        //    ErrorMessage = "";
        //    try
        //    {
        //        // Reading XML
        //        TextReader textReader1 = new StringReader(XMLPage);
        //        XmlTextReader xmlTextReader1 = new XmlTextReader(textReader1);
        //        XPathDocument xPathDocument = new XPathDocument(xmlTextReader1);

        //        //Reading XSLT
        //        TextReader textReader2 = new StringReader(XSLStylesheet);
        //        XmlTextReader xmlTextReader2 = new XmlTextReader(textReader2);

        //        //Define XslCompiledTransform
        //        XslCompiledTransform xslt = new XslCompiledTransform();
        //        xslt.Load(xmlTextReader2);


        //        StringBuilder sb = new StringBuilder();
        //        TextWriter tw = new StringWriter(sb);

        //        xslt.Transform(xPathDocument, null, tw);

        //        result = sb.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage = ex.Message;
        //    }
        //    return result;
        //}
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
        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //((ToolTip)((FrameworkElement)sender).ToolTip).IsOpen = true;
            switch (((TextBlock)((FrameworkElement)sender)).Text)
            {
                case "Marka İskonto Tutar:":
                    tbl_tooltip.Text = "Buraya istediğiniz markaya uygulanmak üzere iskonto tutarını gireceksiniz.\nArdından enter tuşuna basacak ve markayı seçeceksiniz."; break;
                case "Genel İskonto Tutar:":
                    tbl_tooltip.Text = "Buraya girdiğiniz miktar kadar indirim uygulanacak.\nEğer gireceğiniz miktarın başına % işaretini koyarsanız girdiğiniz miktarın yüzdesi kadar indirim uygulanacaktır."; break;
            }
            PopupTooltip.IsOpen = true;
        }
        private void markaiskonto_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            if (markaiskonto.Text.Length == 0 && e.Text == "-")
            {
                e.Handled = regex.IsMatch(e.Text.Remove(0, 1));
            }
            else
            {
                e.Handled = regex.IsMatch(e.Text);
            }
        }
        private void txt_geneliskontotutar_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            if (markaiskonto.Text.Length == 0 && e.Text == "%")
            {
                e.Handled = regex.IsMatch(e.Text.Remove(0, 1));
            }
            else
            {
                e.Handled = regex.IsMatch(e.Text);
            }
        }
        private void stok_dataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = " " + e.Column.Header.ToString() + " ";

            //if (e.Column is DataGridCheckBoxColumn cb && !e.Column.IsReadOnly)
            //{
            //    var checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));
            //    checkboxFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            //    checkboxFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
            //    checkboxFactory.SetBinding(ToggleButton.IsCheckedProperty, new Binding(e.PropertyName) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

            //    e.Column = new DataGridTemplateColumn
            //    {
            //        Header = e.Column.Header,
            //        CellTemplate = new DataTemplate { VisualTree = checkboxFactory },
            //        SortMemberPath = e.Column.SortMemberPath
            //    };
            //}
        }
        //private void DataGridCell_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
            //if (stok_dataGrid.CurrentColumn.DisplayIndex == 0)//sender is DataGridCell cell && cell.GetType().Equals(typeof(DataGridCheckBoxColumn))/* && e.OriginalSource is UIElement source*/)
            //{
            //    DataRowView rowView = (stok_dataGrid.Items[stok_dataGrid.SelectedIndex] as DataRowView);
            //    stok_dataGrid.BeginInit();
            //    if (rowView[0].ToString() == "True") { CheckBox c = (CheckBox)rowView[0]; c.IsChecked = false; }
            //    else if (rowView[0].ToString() == "False") { CheckBox c = (CheckBox)rowView[0]; c.IsChecked = true; }
            //    stok_dataGrid.EndInit();
            //    populate(); Hesap();
            //}
            //{
            //    var actualSource = source is CheckBox ?
            //        (CheckBox)source : VisualExtensions.GetVisualParent<CheckBox>(source);

            //    if (actualSource != null)
            //    {
            //        ItemListDG.BeginEdit();

            //        var newSource = cell.GetVisualChild<CheckBox>();
            //        if (newSource != null)
            //        {
            //            newSource.IsChecked = !newSource.IsChecked;
            //        }
            //    }

            //}
        //}


        private void Geri_Click(object sender, RoutedEventArgs e)
        {
            Secim_Satis w = new Secim_Satis();
            w.Show();
            this.Close();
            //w.Width = this.ActualWidth;
            //w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
        }

        TabItem tabUserPage;
        Calculator cl;
        private void calculator_button_Click(object sender, RoutedEventArgs e)
        {
            if (sv.Visibility == Visibility.Hidden)
            {
                sv.Visibility = Visibility.Visible;
                Calc.Items.Clear();
                Calc.Visibility = Visibility.Hidden;
                i1.Source = new BitmapImage(new Uri(@"/icons/hesapmakinesibeyaz.png", UriKind.Relative));
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
        #endregion

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




//public static bool IsWindowOpen<T>(string name = "") where T : Window
//{
//    return string.IsNullOrEmpty(name)
//       ? Application.Current.Windows.OfType<T>().Any()
//       : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
//}






//XtraReport1 r = new XtraReport1();
//MsSqlConnectionParameters connectionParameters;
//Carried.DosyaDecrypt();
//if (IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Yetki", "Baglanti.ini") == "WINDOWS")
//{
//    string[] words = Carried.girisBaglantiLocal.Split(';');
//    connectionParameters = new MsSqlConnectionParameters()
//    {
//        ServerName = words[0].Substring(words[0].IndexOf("Data Source=") + "Data Source=".Length),
//        DatabaseName = words[1].Substring(words[1].IndexOf("Initial Catalog=") + "Initial Catalog=".Length),
//        UserName = null,
//        Password = null,
//        AuthorizationType = MsSqlAuthorizationType.Windows
//    };
//}
//else
//{
//    string[] words = Carried.girisBaglantiLocal.Split(';');
//    connectionParameters = new MsSqlConnectionParameters()
//    {

//        ServerName = words[0].Substring(words[0].IndexOf("Data Source=") + "Data Source=".Length),
//        DatabaseName = words[1].Substring(words[1].IndexOf("Initial Catalog=") + "Initial Catalog=".Length),
//        UserName = words[2].Substring(words[2].IndexOf("User ID =") + "User ID =".Length),
//        Password = words[3].Substring(words[3].IndexOf("Password=") + "Password=".Length),
//        AuthorizationType = MsSqlAuthorizationType.SqlServer
//    };
//}
//Carried.DosyaEncrypt();
//string connectionString = MSSqlConnectionProvider.GetConnectionString(connectionParameters.ServerName, connectionParameters.UserName, connectionParameters.Password, connectionParameters.DatabaseName);
//IDataLayer dataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema);
//r.DataSource = dataLayer;//new SqlDataSource(connectionParameters); //new SqlConnection(Carried.girisBaglantiLocal)
//SqlDataSource s = new SqlDataSource(connectionParameters);
//(r.DataSource as SqlDataSource).ConnectionParameters = connectionParameters;
////s.RebuildResultSchema();
//r.DataSource = s;
//r.CreateDocument();
//MemoryStream ms = new MemoryStream();
//PdfExportOptions options = new PdfExportOptions();
//options.ShowPrintDialogOnOpen = true;
//r.ExportToPdf(ms, options);
//ReportPrintTool tool = new ReportPrintTool(r);
//tool.ShowRibbonPreviewDialog();
//XtraReport r = new XtraReport();
//r.LoadLayout(System.AppDomain.CurrentDomain.BaseDirectory+"\\DXApplication3\\DXApplication3\\bin\\Debug\\Report1.repx");
//(r.DataSource as SqlDataSource).ConnectionParameters = connectionParameters;
//r.CreateDocument();
//MemoryStream ms = new MemoryStream();
//PdfExportOptions options = new PdfExportOptions();
//options.ShowPrintDialogOnOpen = true;
//r.ExportToPdf(ms, options);
//ReportPrintTool tool = new ReportPrintTool(r);
//tool.ShowRibbonPreviewDialog();


//Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\DXApplication3\\DXApplication3\\obj\\Debug\\DXApplication3.exe");
//OpenFileDialog openFileDialog1 = new OpenFileDialog();
//en = Encoding.UTF8;
//string xlst_path = System.AppDomain.CurrentDomain.BaseDirectory+ "general.xslt";
//if (!File.Exists(xlst_path))
//{
//    Carried.showMessage(xlst_path + " dosyası bulunamadı");
//    return;
//}
//else
//{
//    strXLST = File.ReadAllText(xlst_path, en);
//}

//OpenFileDialog ofd = new OpenFileDialog();
//bool? response = ofd.ShowDialog();
//if (response == true)
//{
//    string filepath = ofd.FileName;
//    try
//    {
//        if (!filepath.ToLower().EndsWith(".xml"))
//        {
//            Carried.showMessage("Sadece xml uzantılı dosyalar desteklenmektedir.");
//            return;
//        }
//        var sr = new StreamReader(filepath);
//        string strFaturaXML = sr.ReadToEnd();
//        string ErrMsg = "";
//        string strResult = Transform(strFaturaXML, strXLST, out ErrMsg);
//        if (ErrMsg != "")
//            Carried.showMessage(ErrMsg);
//        else
//        {
//            //webBrowser1.DocumentText = strResult;
//            //webBrowser1.NavigateToString(strResult);
//            System.Windows.Forms.WebBrowser webBrowser1 = new System.Windows.Forms.WebBrowser();
//            webBrowser1.Width = 850;
//            webBrowser1.Height = 800;
//            webBrowser1.DocumentText = strResult;
//            System.Windows.Forms.Form form = new System.Windows.Forms.Form();
//            form.Controls.Add(webBrowser1);
//            form.Width = 850;
//            form.Height = 800;
//            form.ShowDialog();
//            //PopupWb.IsOpen = true;
//        }
//    }
//    catch (Exception ex)
//    {
//        Carried.showMessage("Error message: " + ex.Message + Environment.NewLine + "Details:" + ex.StackTrace);
//    }
//}