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
//using Microsoft.Data.SqlClient;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for CariKartAc.xaml
    /// </summary>
    public partial class CariKartAc : ThemedWindow
    {
        int hesaptip;
        string tabloadi, hesapno="", vkn="";
        string ilAdi, ilceAdi, mahalleSemt, koy, caddeSokak, disKapiNo, icKapiNo, beldeBucak, ad, soyad;
        SqlConnection s, sl;
        SqlCommand c, c1;
        string path;
        public CariKartAc(string path)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ApiHelper.InitializeClient();
            s = new SqlConnection(Carried.girisBaglantiCPM);
            sl = new SqlConnection(Carried.girisBaglantiLocal);
            txt24.IsEnabled = true;
            this.path = path;
            l_fatil.Height = 0;
            l_fatilce.Height = 0;
            txt22.Height = 0;
            txt23.Height = 0;
        }
        public CariKartAc(string vkn, string hesapno)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.vkn = vkn;
            gridcarikartara.Visibility = Visibility.Hidden;
            this.hesapno = hesapno;
            ApiHelper.InitializeClient();
            s = new SqlConnection(Carried.girisBaglantiCPM);
            sl = new SqlConnection(Carried.girisBaglantiLocal);
        }


        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if(hesapno != "") txt3.Text = HesapkodUret(hesapno, false);
            if (vkn != "")
            {
                txt24.Text = vkn;
                if (vkn.Length == 10) txt27.Text = "2";
                else if (vkn.Length == 11)  txt27.Text = "1";
                efaBilgileriGetir(vkn);
                if(Carried.CheckForInternetConnection()==true) await GetVKNinfo();
                else
                {
                    txt34.IsEnabled = true;
                    txt38.IsEnabled = true;
                    txt4.IsEnabled = true;
                    txt22.IsEnabled = true;
                    txt23.IsEnabled = true;
                    txt18.IsEnabled = true;
                    txt20.IsEnabled = true;
                    txt16.IsEnabled = true;
                    txt26.IsEnabled = true;
                }
            }
        }
        private async Task GetVKNinfo()
        {
            var info = await VKNProcessor.LoadVKNinfo(vkn);
            txt4.Text = info.unvan;
            ad = info.ad;
            soyad = info.soyad;
            if (vkn.Length == 11 && ad != null && soyad != null)
            {
                txt34.Text = ad;
                txt38.Text = soyad;
                txt4.Text = ad + " " + soyad;
            }
            if (info.adresBilgileri.Count != 0) 
            { 
            info.adresBilgileri[0].TryGetValue("ilAdi", out ilAdi);
            info.adresBilgileri[0].TryGetValue("ilceAdi", out ilceAdi);
            info.adresBilgileri[0].TryGetValue("mahalleSemt", out mahalleSemt);
            info.adresBilgileri[0].TryGetValue("koy", out koy);
            info.adresBilgileri[0].TryGetValue("caddeSokak", out caddeSokak);
            info.adresBilgileri[0].TryGetValue("disKapiNo", out disKapiNo);
            info.adresBilgileri[0].TryGetValue("icKapiNo", out icKapiNo);
            info.adresBilgileri[0].TryGetValue("beldeBucak", out beldeBucak);
            txt22.Text = ilAdi;
            txt23.Text = ilceAdi;
            txt18.Text = disKapiNo;
            txt20.Text = icKapiNo; 
            string adres = mahalleSemt + " " + koy + " " + caddeSokak + " Bina No:" + disKapiNo + " Daire No:" + icKapiNo;
            if (adres.Length <= 49) txt16.Text = adres;
            else { txt16.Text = adres.Substring(0,49);  txt17.Text = adres.Substring(adres.Length - (adres.Length - 49)); }
            }
            txt26.Text = GetUntilOrEmpty(info.vergiDairesiAdi, " VERGİ DAİRESİ MÜD.");
        }
        private string GetUntilOrEmpty(string text, string stopAt)
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }
            return String.Empty;
        }
        private string HesapkodUret(string hesapno, bool birsonrakimi)  //HesapkodUret(int hesaptip)
        {
            //aşağıda otomatik olarak hesapkod üretme islemi yapılmıştır. 
            //birsonrakimi true ise örneğin o hesaptipinde en büyük hesapkod=100A.0001.9999 diyelim, yeni hesap kod 100A.0002.0001 olacak
            //birsonrakimi true ise başka bir örnek: son hesapkod=100.0001.0010 yeni hesapkod= 100.0001.0011 olacak
            string strsayi = "";
            //Carried.DosyaDecrypt();
            //string hesapno = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Hesap No Ayarı", "Baglanti.ini");
            //Carried.DosyaEncrypt();
            string[] hesapnoarray = hesapno.Split('.');
            int sayi = Convert.ToInt32(hesapnoarray[2]);
            if (sayi != 9999)
            {
                if (birsonrakimi == true) sayi = sayi + 1;
                strsayi = sayi.ToString();
                strsayi = new String('0', (4 - strsayi.Length)) + strsayi;
                strsayi = "." + new String('0', (4 - hesapnoarray[1].Length)) + hesapnoarray[1] + "." + strsayi; 
            }
            else
            {
                sayi = Convert.ToInt32(hesapnoarray[1]);
                sayi = sayi + 1;
                strsayi = sayi.ToString();
                strsayi = new String('0', (4 - strsayi.Length)) + strsayi;
                strsayi = "." + strsayi + ".0001";
            }
            return hesapnoarray[0] + strsayi;

            //    //aşağıda otomatik olarak hesapkod üretme ilemi yapılmıştır. Seçilen hesaptipin en büyük hesapnosunu alıp 1 arttırıyor.
            //    //Örneğin o hesaptipinde en büyük hesapkod=320.01.999 diyelim, yeni hesap kod 320.02.001 olacak
            //    //başka bir örnek: son hesapkod=100.01.010 yeni hesapkod= 100.01.011 olacak
            //    tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
            //    string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            //    s = new SqlConnection(constr);
            //    s.Open();
            //    object objhesapkod = new SqlCommand("select hesapkod from " + tabloadi + " where HESAPTIP='" + hesaptip + "' ORDER BY HESAPKOD DESC", s).ExecuteScalar();
            //    if (objhesapkod != null)
            //    {
            //        string strsayi, hesapkod = objhesapkod.ToString();
            //        string[] numbers = hesapkod.Split('.');
            //        int sayi = Convert.ToInt32(numbers[numbers.Length - 1]);
            //        if (sayi != 999)
            //        {
            //            sayi = sayi + 1;
            //            strsayi = sayi.ToString();
            //            strsayi = new String('0', (3 - strsayi.Length)) + strsayi;
            //            strsayi = numbers[0] + "." + numbers[1] + "." + strsayi;
            //            return strsayi;
            //        }
            //        else
            //        {
            //            sayi = Convert.ToInt32(numbers[numbers.Length - 2]);
            //            sayi = sayi + 1;
            //            strsayi = sayi.ToString();
            //            strsayi = new String('0', (2 - strsayi.Length)) + strsayi;
            //            strsayi = numbers[0] + "." + strsayi + ".001";
            //            return strsayi;
            //        }
            //    }
            //    else
            //    {
            //        return "";
            //    }
        }
        private void efaBilgileriGetir(string vkn)
        {
            txt15.IsEnabled = false;
            txt35.IsEnabled = false;
            SqlConnection s1 = Carried.IsCPMconnected == true && Carried.CheckForInternetConnection() == true ?  s : sl;
            s1.Open();
            tabloadi = Carried.IsCPMconnected == true && Carried.CheckForInternetConnection() == true ? "efakul" : "SMRTAPPETIKET";
            int etip = (int) new SqlCommand("select COUNT(ETIKETTIP) from "+tabloadi+" WHERE VERGIHESAPNO='"+vkn+"'", s1).ExecuteScalar();
            if (etip == 0) 
            { 
                txt13.Text = "0"; txt36.Text = "0"; txt37.Text = "1"; 

            }
            else if (etip == 1) 
            {
                txt13.Text = "1"; txt36.Text = "0"; txt37.Text = "0";
                object etiket = new SqlCommand("select ETIKET from  " + tabloadi + "  WHERE ETIKETTIP=0 AND VERGIHESAPNO='" + vkn + "'", s1).ExecuteScalar();
                if(etiket != null) txt15.Text = etiket.ToString();
            }
            else if (etip == 2) 
            { 
                txt13.Text = "1"; txt36.Text = "1"; txt37.Text = "0";
                object etiket1 = new SqlCommand("select ETIKET from  " + tabloadi + "  WHERE ETIKETTIP=0 AND VERGIHESAPNO='" + vkn + "'", s1).ExecuteScalar();
                object etiket2 = new SqlCommand("select ETIKET from  " + tabloadi + "  WHERE ETIKETTIP=1 AND VERGIHESAPNO='" + vkn + "'", s1).ExecuteScalar();
                if (etiket1 != null) txt15.Text = etiket1.ToString();
                else txt15.IsEnabled = true;
                if (etiket2 != null) txt35.Text = etiket2.ToString();
                else txt35.IsEnabled = true;
            }
        }


        private void Geri_Click(object sender, RoutedEventArgs e)
        {
            Window w;
            if (!String.IsNullOrEmpty(path)) w = new Secim();
            else w = new Satıs_Islemleri("SATIS");
            w.Show();
            this.Close();
            //w.Width = this.ActualWidth;
            //w.Height = this.ActualHeight;
            w.WindowState = this.WindowState;
            w.VerticalAlignment = this.VerticalAlignment;
            w.HorizontalAlignment = this.HorizontalAlignment;
        }


        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txt2.Text) && !String.IsNullOrWhiteSpace(txt3.Text) && !String.IsNullOrWhiteSpace(txt4.Text) && !String.IsNullOrWhiteSpace(txt12.Text) && !String.IsNullOrWhiteSpace(txt24.Text) && !String.IsNullOrWhiteSpace(txt25.Text) && !String.IsNullOrWhiteSpace(txt26.Text) && !String.IsNullOrWhiteSpace(txt27.Text) && !String.IsNullOrWhiteSpace(txt13.Text) && !String.IsNullOrWhiteSpace(txt36.Text) && !String.IsNullOrWhiteSpace(txt37.Text) && !String.IsNullOrWhiteSpace(txt2.Text))
            {
                if (!Regex.IsMatch(txt25.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                {
                    Carried.showMessage("Geçerli bir e-posta adresi girin.");
                    return;
                }

                //if (String.IsNullOrWhiteSpace(txt10.Text)) txt10.Text = "0";
                //if (String.IsNullOrWhiteSpace(txt11.Text)) txt11.Text = "0";
                if (String.IsNullOrWhiteSpace(txt6.Text)) txt6.Text = "0";
                if (String.IsNullOrWhiteSpace(txt29.Text)) txt29.Text = "0";

                //string komut = @"if not exists (select * from SMRTAPPBAS where EVRAKNO = '" + s2 + "') begin insert into SMRTAPPBAS (ID, EVRAKNO, HESAPTIP, HESAPKOD, UNVAN, EFATURADURUM, EARSIVDURUM, PKETIKET, DOVIZCINS, EVRAKDOVIZCINS, EVRAKTARIH, ACIKLAMA, KARSIHESAPKOD, MIKTAR, EVRAKDURUM, KAYITDURUM) values(@ID, @EVRAKNO, @HESAPTIP, @HESAPKOD, @UNVAN, @EFATURADURUM, @EARSIVDURUM, @PKETIKET, @DOVIZCINS, @EVRAKDOVIZCINS, @EVRAKTARIH, @ACIKLAMA, @KARSIHESAPKOD, @MIKTAR, @EVRAKDURUM, @KAYITDURUM) end "; /**/   //KARSIUNVAN, REFNO yok
                //commandDestinationData.Parameters.Add(new SqlParameter("EVRAKNO", s2));
                List<string> list = new List<string>() { "SMRTAPPCKRT" };
                if (Carried.IsCPMconnected == true && Carried.CheckForInternetConnection() == true) list.Add("CARKRT");
                try
                {
                    foreach (string tabload in list)
                    {
                        string komut = @"insert into " + tabload + " (SIRKETNO,HESAPKOD,KAYITTUR,KAYITDURUM,KARTTIP,HESAPTIP,UNVAN,KISITIP,ULKEKOD,VERGIDAIRE,VERGIHESAPNO,KISIUNVAN,KISIAD,KISISOYAD,KISIUYRUK,KISIPASAPORTTARIH," +
                            "FATURAUNVAN,FATURAADRES1,FATURAADRES2,FATURAADRES4,FATURAADRES5,FATURAADRESBINANO,FATURAADRESBINAAD,FATURAADRESDAIRENO,YETKILI1,YETKILI2,YETKILI3," +
                            "TELEFON1,TELEFON2,OPSIYONTIP,OPSIYON,ODEMEGUN,ISKONTOORAN,DOVIZBANKA,DOVIZTIP,DOVIZCINS,BKOD1,BKOD2,BKOD3,BKOD4,BKOD5,BKOD6,BKOD7,BKOD8,BKOD9,NKOD1," +
                            "NKOD2,NKOD3,NKOD4,NKOD5,NKOD6,NKOD7,NKOD8,NKOD9,TARIH1,TARIH2,TARIH3,TARIH4,TARIH5,ACIKLAMA3,ACIKLAMA4,ACIKLAMA5,MUHASEBEKOD1,ACIKHESAPLIMIT," +
                            "DOVIZACIKHESAPLIMIT,KREDILIMIT,DOVIZKREDILIMIT,BORCLUKREDILIMIT,MUTABAKATTARIH,MUTABAKATBAKIYE,DOVIZMUTABAKATBAKIYE,VADEFARKMUTABAKATTARIH," +
                            "KONTROLEVRAKTIP,FIRMATIP,TAKVIMOZEL,BLOKE,BABSTIP,EFATURADURUM,EFATURASENARYO,EFATURAPKETIKET,EFATURAYUKLEMETIP,EARSIVKAGITNUSHA,EIRSALIYEDURUM," +
                            "EIRSALIYEPKETIKET,GIRENKULLANICI,GIRENTARIH,GIRENSAAT,GIRENKAYNAK,GIRENSURUM,DEGISTIRENKULLANICI,DEGISTIRENTARIH,DEGISTIRENSAAT,DEGISTIRENKAYNAK," +
                            "DEGISTIRENSURUM,EMAIL1,EMAIL2,EMAIL3,EMAIL4,EMAIL5) " +
                            " VALUES(@SIRKETNO,@HESAPKOD,@KAYITTUR,@KAYITDURUM,@KARTTIP,@HESAPTIP,@UNVAN,@KISITIP,@ULKEKOD,@VERGIDAIRE,@VERGIHESAPNO,@KISIUNVAN,@KISIAD,@KISISOYAD,@KISIUYRUK,@KISIPASAPORTTARIH,@FATURAUNVAN,@FATURAADRES1,@FATURAADRES2,@FATURAADRES4,@FATURAADRES5,@FATURAADRESBINANO,@FATURAADRESBINAAD,@FATURAADRESDAIRENO,@YETKILI1,@YETKILI2,@YETKILI3,@TELEFON1,@TELEFON2,@OPSIYONTIP,@OPSIYON,@ODEMEGUN,@ISKONTOORAN,@DOVIZBANKA,@DOVIZTIP,@DOVIZCINS,@BKOD1,@BKOD2,@BKOD3,@BKOD4,@BKOD5,@BKOD6,@BKOD7,@BKOD8,@BKOD9,@NKOD1,@NKOD2,@NKOD3,@NKOD4,@NKOD5,@NKOD6,@NKOD7,@NKOD8,@NKOD9,@TARIH1,@TARIH2,@TARIH3,@TARIH4,@TARIH5,@ACIKLAMA3,@ACIKLAMA4,@ACIKLAMA5,@MUHASEBEKOD1,@ACIKHESAPLIMIT,@DOVIZACIKHESAPLIMIT,@KREDILIMIT,@DOVIZKREDILIMIT,@BORCLUKREDILIMIT,@MUTABAKATTARIH,@MUTABAKATBAKIYE,@DOVIZMUTABAKATBAKIYE,@VADEFARKMUTABAKATTARIH,@KONTROLEVRAKTIP,@FIRMATIP,@TAKVIMOZEL,@BLOKE,@BABSTIP,@EFATURADURUM,@EFATURASENARYO,@EFATURAPKETIKET,@EFATURAYUKLEMETIP,@EARSIVKAGITNUSHA,@EIRSALIYEDURUM,@EIRSALIYEPKETIKET,@GIRENKULLANICI,@GIRENTARIH,@GIRENSAAT,@GIRENKAYNAK,@GIRENSURUM,@DEGISTIRENKULLANICI,@DEGISTIRENTARIH,@DEGISTIRENSAAT,@DEGISTIRENKAYNAK,@DEGISTIRENSURUM,@EMAIL1,@EMAIL2,@EMAIL3,@EMAIL4,@EMAIL5) ";
                        c = new SqlCommand(); c.CommandText = komut;
                        if (tabload == "CARKRT") { c.Connection = s; s.Open(); }
                        else if (tabload == "SMRTAPPCKRT") { c.Connection = sl; sl.Open(); }
                        #region PARAMETRELER
                        c.Parameters.Add(new SqlParameter("SIRKETNO", new String('0', (3 - Carried.sirketNo.ToString().Length)) + Carried.sirketNo.ToString()));
                        c.Parameters.Add(new SqlParameter("HESAPKOD", txt3.Text));
                        c.Parameters.Add(new SqlParameter("KAYITTUR", 1));
                        c.Parameters.Add(new SqlParameter("KAYITDURUM", 1));
                        c.Parameters.Add(new SqlParameter("KARTTIP", 1));
                        c.Parameters.Add(new SqlParameter("HESAPTIP", hesaptip));
                        c.Parameters.Add(new SqlParameter("UNVAN", txt4.Text.Substring(0, Math.Min(txt4.Text.Length, 100))));
                        c.Parameters.Add(new SqlParameter("KISITIP", Convert.ToInt32(txt27.Text)));
                        c.Parameters.Add(new SqlParameter("ULKEKOD", "052"));
                        c.Parameters.Add(new SqlParameter("VERGIDAIRE", txt26.Text));
                        c.Parameters.Add(new SqlParameter("VERGIHESAPNO", txt24.Text));
                        c.Parameters.Add(new SqlParameter("KISIUNVAN", ""));
                        c.Parameters.Add(new SqlParameter("KISIAD", txt34.Text));
                        c.Parameters.Add(new SqlParameter("KISISOYAD", txt38.Text));
                        c.Parameters.Add(new SqlParameter("KISIUYRUK", ""));
                        c.Parameters.Add(new SqlParameter("KISIPASAPORTTARIH", ""));
                        c.Parameters.Add(new SqlParameter("FATURAUNVAN", txt4.Text));
                        c.Parameters.Add(new SqlParameter("FATURAADRES1", txt16.Text));
                        c.Parameters.Add(new SqlParameter("FATURAADRES2", txt17.Text));
                        c.Parameters.Add(new SqlParameter("FATURAADRES4", txt23.Text));
                        c.Parameters.Add(new SqlParameter("FATURAADRES5", txt22.Text));//İL KODU??????????????
                        c.Parameters.Add(new SqlParameter("FATURAADRESBINANO", txt18.Text));
                        c.Parameters.Add(new SqlParameter("FATURAADRESBINAAD", txt19.Text));
                        c.Parameters.Add(new SqlParameter("FATURAADRESDAIRENO", txt20.Text));
                        c.Parameters.Add(new SqlParameter("YETKILI1", txt34.Text + " " + txt38.Text));
                        c.Parameters.Add(new SqlParameter("YETKILI2", ""));
                        c.Parameters.Add(new SqlParameter("YETKILI3", ""));
                        c.Parameters.Add(new SqlParameter("TELEFON1", ""));
                        c.Parameters.Add(new SqlParameter("TELEFON2", ""));
                        c.Parameters.AddWithValue("@OPSIYONTIP", 0); //Convert.ToInt32(txt10.Text)
                        c.Parameters.AddWithValue("@OPSIYON", 0); //Convert.ToInt32(txt11.Text)
                        c.Parameters.Add(new SqlParameter("ODEMEGUN", Convert.ToInt32(txt29.Text)));
                        c.Parameters.Add(new SqlParameter("ISKONTOORAN", Convert.ToInt32(txt6.Text)));
                        c.Parameters.AddWithValue("@DOVIZBANKA", 0);
                        c.Parameters.AddWithValue("@DOVIZTIP", 0);
                        c.Parameters.Add(new SqlParameter("DOVIZCINS", txt12.Text));
                        c.Parameters.AddWithValue("@BKOD1", 0);
                        c.Parameters.AddWithValue("@BKOD2", 0);
                        c.Parameters.AddWithValue("@BKOD3", 0);
                        c.Parameters.AddWithValue("@BKOD4", 0);
                        c.Parameters.AddWithValue("@BKOD5", 0);
                        c.Parameters.AddWithValue("@BKOD6", 0);
                        c.Parameters.AddWithValue("@BKOD7", 0);
                        c.Parameters.AddWithValue("@BKOD8", 0);
                        c.Parameters.AddWithValue("@BKOD9", 0);
                        c.Parameters.AddWithValue("@NKOD1", 0);
                        c.Parameters.AddWithValue("@NKOD2", 0);
                        c.Parameters.AddWithValue("@NKOD3", 0);
                        c.Parameters.AddWithValue("@NKOD4", 0);
                        c.Parameters.AddWithValue("@NKOD5", 0);
                        c.Parameters.AddWithValue("@NKOD6", 0);
                        c.Parameters.AddWithValue("@NKOD7", 0);
                        c.Parameters.AddWithValue("@NKOD8", 0);
                        c.Parameters.AddWithValue("@NKOD9", 0);
                        c.Parameters.AddWithValue("@TARIH1", 0);
                        c.Parameters.AddWithValue("@TARIH2", 0);
                        c.Parameters.AddWithValue("@TARIH3", 0);
                        c.Parameters.AddWithValue("@TARIH4", 0);
                        c.Parameters.AddWithValue("@TARIH5", 0);
                        c.Parameters.Add(new SqlParameter("ACIKLAMA3", ""));
                        c.Parameters.Add(new SqlParameter("ACIKLAMA4", ""));
                        c.Parameters.Add(new SqlParameter("ACIKLAMA5", ""));
                        c.Parameters.Add(new SqlParameter("MUHASEBEKOD1", "")); //MUHASEBE KODU CPM'DE KENDİ OLUŞUYOR.
                        c.Parameters.AddWithValue("@ACIKHESAPLIMIT", 0);
                        c.Parameters.AddWithValue("@DOVIZACIKHESAPLIMIT", 0);
                        c.Parameters.AddWithValue("@KREDILIMIT", 0);
                        c.Parameters.AddWithValue("@DOVIZKREDILIMIT", 0);
                        c.Parameters.AddWithValue("@BORCLUKREDILIMIT", 0);
                        c.Parameters.AddWithValue("@MUTABAKATTARIH", 0);
                        c.Parameters.AddWithValue("@MUTABAKATBAKIYE", 0);
                        c.Parameters.AddWithValue("@DOVIZMUTABAKATBAKIYE", 0);
                        c.Parameters.AddWithValue("@VADEFARKMUTABAKATTARIH", 0);
                        c.Parameters.AddWithValue("@KONTROLEVRAKTIP", 0);
                        c.Parameters.AddWithValue("@FIRMATIP", 0);
                        c.Parameters.AddWithValue("@TAKVIMOZEL", 0);
                        c.Parameters.AddWithValue("@BLOKE", 0);
                        c.Parameters.AddWithValue("@BABSTIP", 0);
                        c.Parameters.Add(new SqlParameter("EFATURADURUM", txt13.Text));
                        c.Parameters.Add(new SqlParameter("EFATURASENARYO", txt14.Text));
                        c.Parameters.Add(new SqlParameter("EFATURAPKETIKET", txt15.Text));
                        c.Parameters.AddWithValue("@EFATURAYUKLEMETIP", 0);
                        c.Parameters.AddWithValue("@EARSIVKAGITNUSHA", 0);
                        c.Parameters.Add(new SqlParameter("EIRSALIYEDURUM", txt36.Text));
                        c.Parameters.Add(new SqlParameter("EIRSALIYEPKETIKET", txt35.Text));
                        //string date1 = DateTime.Now.ToString("dd/MM/yyyy");
                        //string hourMinute = DateTime.Now.ToString("HH:mm:ss");
                        c.Parameters.Add(new SqlParameter("GIRENKULLANICI", Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30))));
                        c.Parameters.Add("@GIRENTARIH", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                        c.Parameters.Add("@GIRENSAAT", SqlDbType.SmallDateTime).Value = DateTime.Now;
                        c.Parameters.Add(new SqlParameter("GIRENKAYNAK", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30))));
                        c.Parameters.Add(new SqlParameter("GIRENSURUM", ""));
                        c.Parameters.Add(new SqlParameter("DEGISTIRENKULLANICI", Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30))));
                        c.Parameters.Add("@DEGISTIRENTARIH", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                        c.Parameters.Add("@DEGISTIRENSAAT", SqlDbType.SmallDateTime).Value = DateTime.Now;
                        c.Parameters.Add(new SqlParameter("DEGISTIRENKAYNAK", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30))));
                        c.Parameters.Add(new SqlParameter("DEGISTIRENSURUM", ""));
                        c.Parameters.Add(new SqlParameter("EMAIL1", txt25.Text));
                        c.Parameters.Add(new SqlParameter("EMAIL2", ""));
                        c.Parameters.Add(new SqlParameter("EMAIL3", ""));
                        c.Parameters.Add(new SqlParameter("EMAIL4", ""));
                        c.Parameters.Add(new SqlParameter("EMAIL5", ""));
                        #endregion
                        c.ExecuteNonQuery();
                    }
                    Carried.DosyaDecrypt();
                    if (!string.IsNullOrWhiteSpace(txt3.Text))
                        IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Hesap No Ayari", HesapkodUret(txt3.Text, true), System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                    Carried.DosyaEncrypt();
                    Carried.showMessage("BAŞARIYLA KAYDEDİLDİ.");
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
                    else
                    {
                        int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                        Carried.showMessage(ex.Message); // + "\n" + line.ToString());
                    }

                }
            }
            else
            {
                Carried.showMessage("Gerekli bilgileri doldurmadan kayıt yapılamaz.");
            }

        }

        
        private async void txt24_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                vkn = txt24.Text;
                Carried.DosyaDecrypt();
                hesapno = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Hesap No Ayari", "Baglanti.ini");
                Carried.DosyaEncrypt();
                txt3.Text = HesapkodUret(hesapno, false);
                if (vkn.Length == 10) txt27.Text = "2";
                else if (vkn.Length == 11) txt27.Text = "1";
                efaBilgileriGetir(vkn);
                await GetVKNinfo();
            }
        }

        //private void selected_date_changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    txt29.Text = dp.SelectedDate.ToString();
        //}


        private void cb4_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cb4.SelectedItem != null)
            {
                if (cb4.SelectedItem.ToString().Contains("TL")) txt12.Text = "TL";
                if (cb4.SelectedItem.ToString().Contains("USD")) txt12.Text = "USD";
                if (cb4.SelectedItem.ToString().Contains("EURO")) txt12.Text = "EUR";
            }
            cb4.IsDropDownOpen = false;
        }
        private void txt12_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (cb4.IsDropDownOpen == true) cb4.IsDropDownOpen = false;
            else
            {
                cb4.Items.Clear();
                cb4.Items.Add(new ComboBoxItem() { Content = "TL", Foreground = Brushes.White });
                cb4.Items.Add(new ComboBoxItem() { Content = "USD", Foreground = Brushes.White });
                cb4.Items.Add(new ComboBoxItem() { Content = "EURO", Foreground = Brushes.White });
                cb4.IsDropDownOpen = true;
            }
        }

        

        List<string> comboboxitemslist = new List<string>();
        Dictionary<int, string> comboboxitems = new Dictionary<int, string>();
        private void txt2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            comboboxitems.Clear();
            comboboxitemslist.Clear();
            string phrase = "";
            if (cb2.IsDropDownOpen == true) cb2.IsDropDownOpen = false;
            else
            {
                cb2.Items.Clear();
                Carried.DosyaDecrypt();
                phrase = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Hesap Tipleri", "Baglanti.ini");
                Carried.DosyaEncrypt();
                if (!string.IsNullOrWhiteSpace(phrase))
                {
                    int[] n = (int[])Enum.GetValues(typeof(HesapTipleri));
                    string[] numbers2 = phrase.Split(',');
                    for (int i = 0; i < numbers2.Length; i++)
                    {
                        int no = Convert.ToInt32(numbers2[i].ToString());
                        if (n.Contains(no))
                        {
                            string enumname = Enum.GetName(typeof(HesapTipleri), no);
                            var cbi = new ComboBoxItem() { Content = enumname, Foreground = Brushes.White };
                            cb2.Items.Add(cbi);
                            comboboxitems.Add(no, enumname);
                            comboboxitemslist.Add(enumname);
                        }
                    }
                    cb2.IsDropDownOpen = true;
                }
            }
        }
        private void cb2_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cb2.SelectedItem != null && comboboxitemslist.Count != 0)
            {
                for (int i = 0; i < comboboxitemslist.Count; i++)
                {
                    if (cb2.SelectedItem.ToString().Contains(comboboxitemslist[i])) txt2.Text = comboboxitemslist[i];
                }
            }
            cb2.IsDropDownOpen = false;
            var myKey = comboboxitems.FirstOrDefault(x => x.Value == txt2.Text).Key;
            hesaptip = Convert.ToInt32(myKey.ToString());
            //txt3.Text = HesapkodUret(Convert.ToInt32(myKey.ToString()));
        }


        private void txt11_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void duzenle1_Checked(object sender, RoutedEventArgs e)
        {
            layoutgroup1.Visibility = Visibility.Hidden;
            layoutgroup2.Visibility = Visibility.Hidden;
            layoutgroup3.Visibility = Visibility.Hidden;
        }
        private void duzenle1_Unchecked(object sender, RoutedEventArgs e)
        {
            layoutgroup1.Visibility = Visibility.Visible;
            layoutgroup2.Visibility = Visibility.Visible;
            layoutgroup3.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ara_Click(object sender, RoutedEventArgs e)//ARA BUTONUNA BASINCA UNVANLAR GELİYOR
        {
            lbUnvan.Items.Clear();
            tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            string phrase = txtcariara.Text.ToString();
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
        private void txtcariara_KeyDown(object sender, KeyEventArgs e)//ENTER BASINCA UNVANLAR GELİYOR
        {
            if (e.Key == Key.Enter)
            {
                lbUnvan.Items.Clear();
                tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
                string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
                string phrase = txtcariara.Text.ToString();
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
            try
            {
                txtcariara.Clear();
                txtcariara.Text = lbUnvan.Items[lbUnvan.SelectedIndex].ToString();
                PopupUnvan.IsOpen = false;
                lbUnvan.Items.Clear();
            }
            catch { return; }
        }
        private string VergiNoVSHesapKodVSUnvan()
        {
            object o1, o2, o3;
            if (Carried.IsCPMconnected == true)
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                s.Open();
                c1 = new SqlCommand("select VERGIHESAPNO from CARKRT where VERGIHESAPNO = '" + txtcariara.Text + "'", s);
                o1 = c1.ExecuteScalar();
                c1 = new SqlCommand("select HESAPKOD from CARKRT where HESAPKOD = '" + txtcariara.Text + "'", s);
                o2 = c1.ExecuteScalar();
                c1 = new SqlCommand("select UNVAN from CARKRT where UNVAN = '" + txtcariara.Text + "'", s);
                o3 = c1.ExecuteScalar();
            }
            else
            {
                s = new SqlConnection(Carried.girisBaglantiLocal);
                s.Open();
                c1 = new SqlCommand("select VERGIHESAPNO from SMRTAPPCKRT where VERGIHESAPNO = '" + txtcariara.Text + "'", s);
                o1 = c1.ExecuteScalar();
                c1 = new SqlCommand("select HESAPKOD from SMRTAPPCKRT where HESAPKOD = '" + txtcariara.Text + "'", s);
                o2 = c1.ExecuteScalar();
                c1 = new SqlCommand("select UNVAN from SMRTAPPCKRT where UNVAN = '" + txtcariara.Text + "'", s);
                o3 = c1.ExecuteScalar();
            }

            if (o1 != null && !String.IsNullOrEmpty(o1.ToString())) return " where VERGIHESAPNO = '" + o1.ToString() + "'";
            else if (o2 != null && !String.IsNullOrEmpty(o2.ToString())) return " where HESAPKOD = '" + o2.ToString() + "'";
            else if (o3 != null && !String.IsNullOrEmpty(o3.ToString())) return " where UNVAN = '" + o3.ToString() + "'";
            else return null;
        }
        string value;
        private void txtcariara_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtcariara.Text))
            {
                if (txtcariara.Text.StartsWith(" ")) { txtcariara.Clear(); }

                try
                {
                    value = VergiNoVSHesapKodVSUnvan();
                    if(value != null)
                    {
                        try
                        {
                            veriGetir(value);
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
        private void veriGetir(string value)
        {
            string tablename; object o;
            if(Carried.IsCPMconnected == true)
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                tablename = "CARKRT";
            }
            else
            {
                s = new SqlConnection(Carried.girisBaglantiLocal);
                tablename = "SMRTAPPCKRT";
            }
            s.Open();
            
            txt1.Text = (o = new SqlCommand("select KAYITDURUM from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            int ht = (o = new SqlCommand("select HESAPTIP from " + tablename + " " + value, s).ExecuteScalar()) != null ? Convert.ToInt16(o) : 0;       
            txt2.Text = Enum.GetName(typeof(HesapTipleri), ht);       
            txt3.Text = (o = new SqlCommand("select HESAPKOD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt4.Text = (o = new SqlCommand("select UNVAN from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt5.Text = (o = new SqlCommand("select UNVAN2 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt6.Text = (o = new SqlCommand("select ISKONTOORAN from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt12.Text = (o = new SqlCommand("select DOVIZCINS from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : ""; 
                  
            txt24.Text = (o = new SqlCommand("select VERGIHESAPNO from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt25.Text = (o = new SqlCommand("select EMAIL1 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt26.Text = (o = new SqlCommand("select VERGIDAIRE from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt27.Text = (o = new SqlCommand("select KISITIP from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            txt28.Text = (o = new SqlCommand("select ODEMEIBAN from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt29.Text = (o = new SqlCommand("select ODEMEGUN from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt30.Text = (o = new SqlCommand("select KREDILIMIT from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt33.Text = (o = new SqlCommand("select ODEMEBANKAKOD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt34.Text = (o = new SqlCommand("select KISIAD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";       
            txt38.Text = (o = new SqlCommand("select KISISOYAD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            
            txt13.Text = (o = new SqlCommand("select EFATURADURUM from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            txt36.Text = (o = new SqlCommand("select EIRSALIYEDURUM from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            txt37.Text = txt13.Text == "1" ? "0" : "1";   
            txt14.Text = (o = new SqlCommand("select EFATURASENARYO from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            txt15.Text = (o = new SqlCommand("select EFATURAPKETIKET from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            txt35.Text = (o = new SqlCommand("select EIRSALIYEPKETIKET from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            txt16.Text = (o = new SqlCommand("select FATURAADRES1 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            txt17.Text = (o = new SqlCommand("select FATURAADRES2 from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            txt18.Text = (o = new SqlCommand("select FATURAADRESBINANO from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            txt19.Text = (o = new SqlCommand("select FATURAADRESBINAAD from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            txt20.Text = (o = new SqlCommand("select FATURAADRESDAIRENO from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";
            //txt22.Clear();
            //txt23.Clear();
            //txt22.IsEnabled = false;
            //txt23.IsEnabled = false;
            ////txt22.Text = (o = new SqlCommand("select  from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
            ////txt23.Text = (o = new SqlCommand("select  from " + tablename + " " + value, s).ExecuteScalar()) != null ? o.ToString() : "";   
        }
        private void Duzenle_Click(object sender, RoutedEventArgs e)
        {
            s = new SqlConnection(Carried.girisBaglantiLocal);
            Duzenle_("SMRTAPPCKRT", s);

            if (Carried.IsCPMconnected == true)
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                Duzenle_("CARKRT", s);
            }
        }

        private void Duzenle_(string tablename, SqlConnection s)
        {
            s.Open();
            string ekle = " KAYITDURUM=@KAYITDURUM, HESAPTIP=@HESAPTIP, UNVAN = @UNVAN, UNVAN2=@UNVAN2, ISKONTOORAN=@ISKONTOORAN, DOVIZCINS=@DOVIZCINS, EMAIL1=@EMAIL1," +
                " VERGIDAIRE=@VERGIDAIRE, KISITIP=@KISITIP, ODEMEIBAN=@ODEMEIBAN, ODEMEGUN=@ODEMEGUN, KREDILIMIT=@KREDILIMIT, ODEMEBANKAKOD=@ODEMEBANKAKOD, KISIAD=@KISIAD, KISISOYAD=@KISISOYAD, EFATURADURUM=@EFATURADURUM, EIRSALIYEDURUM=@EIRSALIYEDURUM, EFATURASENARYO=@EFATURASENARYO, EFATURAPKETIKET=@EFATURAPKETIKET, EIRSALIYEPKETIKET=@EIRSALIYEPKETIKET," +
                " FATURAADRES1=@FATURAADRES1, FATURAADRES2=@FATURAADRES, FATURAADRESBINANO=@FATURAADRESBINANO, FATURAADRESBINAAD=@FATURAADRESBINAAD, FATURAADRESDAIRENO=@FATURAADRESDAIRENO, " +
                "DEGISTIRENKULLANICI=@DEGISTIRENKULLANICI, DEGISTIRENTARIH=@DEGISTIRENTARIH, DEGISTIRENSAAT=@DEGISTIRENSAAT, DEGISTIRENKAYNAK=@DEGISTIRENKAYNAK, DEGISTIRENSURUM=@DEGISTIRENSURUM ";
            c1 = new SqlCommand("UPDATE " + tablename + " SET " + ekle + " WHERE VERGIHESAPNO='" + txt24.Text + "'", s);
            c1.Parameters.AddWithValue("@KAYITDURUM", Convert.ToInt16(txt1.Text));
            Dictionary<string, int> enumdict = new Dictionary<string, int>() { { "Genel", 0 }, {"Müşteri" , 1 }, {"Tedarikçi" , 2 }, {"Kasa" , 3 }, {"Banka" , 4 },
                {"GiderYeri" , 5 }, {"SevkYeri" , 6 }, {"BankaKrediKart" , 7 }, {"ZimmetYeri" , 8 }, {"ÜretimHattı" , 9 }, {"Avans" , 10 }, {"DiğerAlacaklılar" , 11 }, {"OrtaklarCariHesap" , 12 }, {"Bayi" , 98 }, {"Potansiyel" , 99 } };
            var myKey = enumdict.FirstOrDefault(x => x.Key == txt2.Text).Value;
            c1.Parameters.AddWithValue("@HESAPTIP", Convert.ToInt16(myKey.ToString()));
            c1.Parameters.AddWithValue("@UNVAN", txt4.Text);
            c1.Parameters.AddWithValue("@UNVAN2", txt5.Text);
            c1.Parameters.AddWithValue("@ISKONTOORAN", Convert.ToDecimal(txt6.Text));
            c1.Parameters.AddWithValue("@DOVIZCINS", txt12.Text);
            c1.Parameters.AddWithValue("@EMAIL1", txt25.Text);
            c1.Parameters.AddWithValue("@VERGIDAIRE", txt26.Text);
            c1.Parameters.AddWithValue("@KISITIP", Convert.ToInt16(txt27.Text));
            c1.Parameters.AddWithValue("@ODEMEIBAN", txt28.Text);
            c1.Parameters.AddWithValue("@ODEMEGUN", Convert.ToInt16(txt29.Text));
            c1.Parameters.AddWithValue("@KREDILIMIT", Convert.ToDecimal(txt30.Text));
            c1.Parameters.AddWithValue("@ODEMEBANKAKOD", txt33.Text);
            c1.Parameters.AddWithValue("@KISIAD", txt34.Text);
            c1.Parameters.AddWithValue("@KISISOYAD", txt38.Text);
            c1.Parameters.AddWithValue("@EFATURADURUM", Convert.ToInt16(txt13.Text));
            c1.Parameters.AddWithValue("@EIRSALIYEDURUM", Convert.ToInt16(txt36.Text));
            c1.Parameters.AddWithValue("@EFATURASENARYO", txt14.Text);
            c1.Parameters.AddWithValue("@EFATURAPKETIKET", txt15.Text);
            c1.Parameters.AddWithValue("@EIRSALIYEPKETIKET", txt35.Text);
            c1.Parameters.AddWithValue("@FATURAADRES1", txt16.Text);
            c1.Parameters.AddWithValue("@FATURAADRES2", txt17.Text);
            c1.Parameters.AddWithValue("@FATURAADRESBINANO", txt18.Text);
            c1.Parameters.AddWithValue("@FATURAADRESBINAAD", txt19.Text);
            c1.Parameters.AddWithValue("@FATURAADRESDAIRENO", txt20.Text);
            c1.Parameters.Add(new SqlParameter("@DEGISTIRENKULLANICI", Carried.girenKullanici.Substring(0, Math.Min(Carried.girenKullanici.Length, 30))));
            c1.Parameters.Add("@DEGISTIRENTARIH", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
            c1.Parameters.Add("@DEGISTIRENSAAT", SqlDbType.SmallDateTime).Value = DateTime.Now;
            c1.Parameters.Add(new SqlParameter("@DEGISTIRENKAYNAK", System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(0, Math.Min(System.Security.Principal.WindowsIdentity.GetCurrent().Name.Length, 30))));
            c1.Parameters.Add(new SqlParameter("@DEGISTIRENSURUM", ""));
            c1.ExecuteNonQuery();
        }

    }

    enum HesapTipleri
    {
        Genel = 0,
        Müşteri = 1,
        Tedarikçi = 2,
        Kasa = 3,
        Banka = 4,
        GiderYeri = 5,
        SevkYeri = 6,
        BankaKrediKart = 7,
        ZimmetYeri = 8,
        ÜretimHattı = 9,
        Avans = 10,
        DiğerAlacaklılar = 11,
        OrtaklarCariHesap = 12,
        Bayi = 98,
        Potansiyel = 99
    }
}




//Asagısı datagridi yan çevirmek için
//private void c_dataGrid_AutoGeneratedColumns(object sender, EventArgs e)
//{
//    TransformGroup transformGroup = new TransformGroup();
//    transformGroup.Children.Add(new RotateTransform(90));
//    foreach (DataGridColumn dataGridColumn in c_dataGrid.Columns)
//    {
//        if (dataGridColumn is DataGridTextColumn)
//        {
//            DataGridTextColumn dataGridTextColumn = dataGridColumn as DataGridTextColumn;

//            Style style = new Style(dataGridTextColumn.ElementStyle.TargetType, dataGridTextColumn.ElementStyle.BasedOn);
//            style.Setters.Add(new Setter(TextBlock.MarginProperty, new Thickness(0, 2, 0, 2)));
//            style.Setters.Add(new Setter(TextBlock.LayoutTransformProperty, transformGroup));
//            style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));

//            Style editingStyle = new Style(dataGridTextColumn.EditingElementStyle.TargetType, dataGridTextColumn.EditingElementStyle.BasedOn);
//            editingStyle.Setters.Add(new Setter(TextBox.MarginProperty, new Thickness(0, 2, 0, 2)));
//            editingStyle.Setters.Add(new Setter(TextBox.LayoutTransformProperty, transformGroup));
//            editingStyle.Setters.Add(new Setter(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Center));

//            dataGridTextColumn.ElementStyle = style;
//            dataGridTextColumn.EditingElementStyle = editingStyle;
//        }
//    }
//    List<DataGridColumn> dataGridColumns = new List<DataGridColumn>();
//    foreach (DataGridColumn dataGridColumn in c_dataGrid.Columns)
//    {
//        dataGridColumns.Add(dataGridColumn);
//    }
//    c_dataGrid.Columns.Clear();
//    dataGridColumns.Reverse();
//    foreach (DataGridColumn dataGridColumn in dataGridColumns)
//    {
//        c_dataGrid.Columns.Add(dataGridColumn);
//    }
//}

//private void c_dataGridScrollViewer_Loaded(object sender, RoutedEventArgs e)
//{
//    // Add MouseWheel support for the datagrid scrollviewer.
//    c_dataGrid.AddHandler(MouseWheelEvent, new RoutedEventHandler(DataGridMouseWheelHorizontal), true);
//}

//private void DataGridMouseWheelHorizontal(object sender, RoutedEventArgs e)
//{
//    MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
//    double x = (double)eargs.Delta;
//    double y = c_dataGridScrollViewer.VerticalOffset;
//    c_dataGridScrollViewer.ScrollToVerticalOffset(y - x);
//}
////////////////////////////////
//private void Grid_Loaded(object sender, RoutedEventArgs e)
//{
    ////datagride columns ekle
    //DataGridTextColumn textColumn = new DataGridTextColumn();
    //textColumn.Header = "Kayıt Durumu";
    ////textColumn.Binding = new Binding("KAYITDURUM");
    //c_dataGrid.Columns.Add(textColumn);
    //DataGridTextColumn textColumn1 = new DataGridTextColumn();
    //textColumn.Header = "Hesap Tipi";
    ////textColumn.Binding = new Binding("HESAPTIP);
    //c_dataGrid.Columns.Add(textColumn1);
    //DataGridTextColumn textColumn2 = new DataGridTextColumn();
    //textColumn.Header = "Hesap Kodu";
    ////textColumn.Binding = new Binding("HESAPKOD");
    //c_dataGrid.Columns.Add(textColumn2);
    //DataGridTextColumn textColumn3 = new DataGridTextColumn();
    //textColumn.Header = "Unvanı";
    ////textColumn.Binding = new Binding("UNVAN");
    //c_dataGrid.Columns.Add(textColumn3);
    //DataGridTextColumn textColumn4 = new DataGridTextColumn();
    //textColumn.Header = "Unvanı 2";
    ////textColumn.Binding = new Binding("UNVAN2");
    //c_dataGrid.Columns.Add(textColumn4);
    //DataGridTextColumn textColumn5 = new DataGridTextColumn();
    //textColumn.Header = "İskonto Oranı";
    ////textColumn.Binding = new Binding("UNVAN");
    //c_dataGrid.Columns.Add(textColumn5);
    //DataGridTextColumn textColumn6 = new DataGridTextColumn();
    //textColumn.Header = "Program Set Kodu";
    ////textColumn.Binding = new Binding("UNVAN");
    //c_dataGrid.Columns.Add(textColumn6);
    //DataGridTextColumn textColumn7 = new DataGridTextColumn();
    //textColumn.Header = "Versiyon Bilgisi";
    ////textColumn.Binding = new Binding("UNVAN");
    //c_dataGrid.Columns.Add(textColumn7);
    //DataGridTextColumn textColumn8 = new DataGridTextColumn();
    //textColumn.Header = "Program Bilgisi";
    ////textColumn.Binding = new Binding("UNVAN");
    //c_dataGrid.Columns.Add(textColumn8);
    //DataGridTextColumn textColumn9 = new DataGridTextColumn();
    //textColumn.Header = "Opsiyon Tipi";
    ////textColumn.Binding = new Binding("UNVAN");
    //c_dataGrid.Columns.Add(textColumn9);
    //DataGridTextColumn textColumn10 = new DataGridTextColumn();
    //textColumn.Header = "Opsiyon";
    ////textColumn.Binding = new Binding("UNVAN");
    //c_dataGrid.Columns.Add(textColumn10);
    //DataGridTextColumn textColumn11 = new DataGridTextColumn();
    //textColumn.Header = "Döviz Cinsi";
    ////textColumn.Binding = new Binding("UNVAN");
    //c_dataGrid.Columns.Add(textColumn11);

    //c_dataGrid.Items.Add(new());
//}