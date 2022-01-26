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
using System.Windows.Navigation;
using System.Windows.Shapes;
using IniDosyaIslemleri;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Media.Animation;
using BespokeFusion;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Net;

namespace WpfApp2
{
    static class Carried
    {
        public static string girisBaglantiCPM { get; set; }
        public static string girisBaglantiLocal { get; set; }
        public static int sirketNo { get; set; }
        public static bool IsCPMconnected { get; set; }
        public static bool IsAdmin { get; set; }
        public static string companiesInformationStr { get; set; }
        public static string transferCode { get; set; }
        public static string girenKullanici { get; set; }
        public static string kasaNo { get; set; }



        //public static List<List<string>> companiesInformation = new List<List<string>>(); //SMRTAPPDBNAME, companyno, companyname, servername, databasename

        public static void DosyaEncrypt()
        {
            string t1 = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
            t1 = IniDosyaIslemleri.IniDosyaIslemleri.Encrypt(t1);
            File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini", t1);
        }
        public static void DosyaDecrypt()
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini"))
            {
                string t = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                t = IniDosyaIslemleri.IniDosyaIslemleri.Decrypt(t);
                File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini", t);
            }
            else
            {
                File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini").Dispose();
                GrantAccess(System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
            }
        }
        public static void GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit |
                   InheritanceFlags.ContainerInherit,
                PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow));

            dInfo.SetAccessControl(dSecurity);
        }
        public static void showMessage(string mesaj)
        {
            CustomMaterialMessageBox msg = new CustomMaterialMessageBox
            {
                TxtMessage = { Text = mesaj,
                            TextAlignment=TextAlignment.Center,
                            VerticalAlignment=VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, TextWrapping = TextWrapping.Wrap ,
                            Foreground = Brushes.Black , FontFamily = new FontFamily("Arial"), FontSize = 25},
                TxtTitle = { Text = "UYARI", Foreground = Brushes.White },
                MainContentControl = { Background = Brushes.Turquoise },
                TitleBackgroundPanel = { Background = Brushes.Blue },
                BorderBrush = Brushes.Blue,
                BtnCancel = { Content = "TAMAM" },
                BtnOk = { Visibility = Visibility.Hidden }
            };
            msg.Show();
            //i1.Source = new BitmapImage(new Uri(@"/icons/hesapmakinesibeyaz.png", UriKind.Relative));
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        //private const string urlPattern = "http://rate-exchange-1.appspot.com/currency?from={0}&to={0}";
        private const string urlPattern = "https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/{0}/{1}.json";
        //usd, try gibi küçük harflerle yazılmalı
        public static decimal CurrencyConversion(decimal amount, string fromCurrency, string toCurrency)
        {
            string url = string.Format(urlPattern, fromCurrency, toCurrency);

            using (var wc = new WebClient())
            {
                var json = wc.DownloadString(url);

                Newtonsoft.Json.Linq.JToken token = Newtonsoft.Json.Linq.JObject.Parse(json);
                decimal exchangeRate = (decimal)token.SelectToken(toCurrency);

                return (amount * exchangeRate)/*.ToString()*/;
            }
        }
    }



}