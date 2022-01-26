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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string dataSource = "";
        public string initialCatalog = "";
        public string userId = "";
        public string password = "";
        public string dbConnStr = "";

        public string localdataSource = "";
        public string localinitialCatalog = "";
        public string localuserId = "";
        public string localpassword = "";
        public string localdbConnStr = "";

        public string companiesInformation = "";

        public string girisBaglantiCPM = "";
        public string girisBaglantiLocal = "";
        public string kasaNo = "";
        public int sirketNo;
        public bool IsCPMconnected;
        public bool IsAdmin = false;

        public bool IsTransfer = false;
        public string transferCode;

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void showMessage(string mesaj)
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
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sifre.Focus();
            }
        }
        private void sifre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonAutomationPeer peer = new ButtonAutomationPeer(Giris);
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                invokeProv.Invoke();
            }
        }

        //public string lc;
        private void Giris_Click(object sender, RoutedEventArgs e)
        {
            Carried.DosyaDecrypt();
            girisBaglantiCPM = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("CPMconnection", "Baglanti.ini");
            girisBaglantiLocal = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
            companiesInformation = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("CompaniesInformation", "Baglanti.ini"); //listeye at ve listeyi carriedle.
            transferCode = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Transfer", "Baglanti.ini");
            sirketNo = Convert.ToInt32(IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("SirketNo", "Baglanti.ini"));
            kasaNo = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("KasaNo", "Baglanti.ini");
            Carried.DosyaEncrypt();

            //girisBaglantiCPM = Properties.Settings.Default["CPMBaglantiStr"].ToString();
            //girisBaglantiLocal = Properties.Settings.Default["LocalBaglantiStr"].ToString();

            IsCPMconnected = cpmdb.IsChecked == true ? true : false;
            bool netcon = Carried.CheckForInternetConnection();
            if (netcon == false && IsCPMconnected == true)
            {
                Carried.showMessage("İnternet bağlantısı bulunamadığı için local bağlantıya geçildi.");
                IsCPMconnected = false;
            }

            if (IsCPMconnected == true)
            {
                try
                {
                    SqlConnection s1 = new SqlConnection(girisBaglantiCPM);
                    s1.Open();
                }
                catch 
                { 
                    Carried.showMessage("Yapmadıysanız CPM bağlantısını yapıp, ardından şirket seçin."); 
                    return; 
                }
            }

            //try
            //{
            //    SqlConnection s2 = new SqlConnection(girisBaglantiLocal);
            //    s2.Open();
            //}
            //catch { showMessage("Yapmadıysanız CPM bağlantısını yapıp, ardından şirket seçin. Eğer yaptıysanız hatalı yapmışsınız. Yeniden yapınız."); return; }

            try
            {
                SqlConnection s = new SqlConnection(girisBaglantiLocal);
                s.Open();
                SqlCommand command = new SqlCommand("SELECT SIFRE FROM SMRTAPPKUL WHERE KULLANICIADI='" + kullaniciAdi.Text + "'", s);
                object sifreobj = command.ExecuteScalar();
                if (sifreobj != null)
                {
                    string sifreStr = sifreobj.ToString();
                    if (sifreStr == sifre.Password)
                    {
                        command = new SqlCommand("SELECT ADMIN FROM SMRTAPPKUL WHERE KULLANICIADI='" + kullaniciAdi.Text + "' AND SIFRE='" + sifre.Password + "'", s);
                        IsAdmin = (bool)command.ExecuteScalar();
                        object o = new SqlCommand("select KULLANICIADI FROM SMRTAPPKASAKULLANICI WHERE KASANO='" + kasaNo + "'", s).ExecuteScalar();
                        if (o != null && kullaniciAdi.Text == o.ToString()) girisYonlendir();
                        else Carried.showMessage("Kasa mevcut değil ya da kullanıcı kasa uyuşmazlığı var.");
                    }
                    else showMessage("Şifre yanlış girilmiştir.");
                }
                else showMessage("Bu kullanıcı adında bir kullanıcı yok.");
            }
            catch { showMessage("Önce bağlantıları yapınız."); }
        }

        public void girisYonlendir()
        {
            Secim secim = new Secim();
            Carried.girisBaglantiCPM = girisBaglantiCPM;
            Carried.girisBaglantiLocal = girisBaglantiLocal;
            Carried.IsCPMconnected = IsCPMconnected;
            Carried.IsAdmin = IsAdmin;
            Carried.sirketNo = sirketNo;
            Carried.kasaNo = kasaNo;
            Carried.girenKullanici = kullaniciAdi.Text;
            Carried.companiesInformationStr = companiesInformation;
            if (transferCode != null)
            {
                if (transferCode == "None") transferCode = null;
                Carried.transferCode = transferCode;
            }
            secim.Width = this.ActualWidth;
            secim.Height = this.ActualHeight;
            secim.WindowState = this.WindowState;
            secim.VerticalAlignment = this.VerticalAlignment;
            secim.HorizontalAlignment = this.HorizontalAlignment;
            secim.Show();
            this.Close();
        }


        private void cpm_Selected(object sender, RoutedEventArgs e)
        {
            adminPopup.IsOpen = true;
            //cpmPopup.IsOpen = true;
        }

        
        private void kapat_Click(object sender, RoutedEventArgs e)
        {
            cpmPopup.IsOpen = false;
            LV.SelectedItem = null;
        }
        private void cpmPopup_Closed(object sender, EventArgs e)
        {
            this.Opacity = 1;
            LV.SelectedItem = null;
        }
        private void cpmPopup_Opened(object sender, EventArgs e)
        {
            this.Opacity = 0.5;
            LV.SelectedItem = null;
        }
        private void v1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                v3.Focus();
            }
        }
        private void v3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                v4.Focus();
            }
        }

        //private void v4_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        comboBoxVeriTabani.Items.Clear();
        //        try
        //        {
        //            //List<String> databases = new List<String>();
        //            SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();
        //            connection.DataSource = v1.Text;
        //            connection.IntegratedSecurity = false;
        //            connection.UserID = v3.Text;
        //            connection.Password = v4.Password;
        //            String strConn = connection.ToString();
        //            SqlConnection sqlConn = new SqlConnection(strConn);
        //            sqlConn.Open();
        //            SqlCommand command1 = new SqlCommand("SELECT name FROM master.sys.databases", sqlConn);
        //            SqlDataReader reader1 = command1.ExecuteReader();
        //            ArrayList arrlist = new ArrayList();
        //            try
        //            {
        //                while (reader1.Read())
        //                {
        //                    arrlist.Add(reader1["name"].ToString());
        //                }
        //                reader1.Close();
        //            }
        //            finally
        //            {
        //                sqlConn.Close();
        //            }

        //            for (int i = 0; i < arrlist.Count; i++)
        //            {
        //                comboBoxVeriTabani.Items.Add(arrlist[i]);
        //            }
        //        }
        //        catch
        //        {
        //            CustomMaterialMessageBox msg = new CustomMaterialMessageBox
        //            {
        //                TxtMessage = { Text = "Bağlantı sağlanamadı. Yanlış veriler giriyor olabilirsiniz.",
        //                    TextAlignment=TextAlignment.Center,
        //                    VerticalAlignment=VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, TextWrapping = TextWrapping.Wrap ,
        //                    Foreground = Brushes.Black , FontFamily = new FontFamily("Arial"), FontSize = 25},
        //                TxtTitle = { Text = "UYARI", Foreground = Brushes.White },
        //                MainContentControl = { Background = Brushes.Turquoise },
        //                TitleBackgroundPanel = { Background = Brushes.Blue },
        //                BorderBrush = Brushes.Blue,
        //                BtnCancel = { Content = "TAMAM" },
        //                BtnOk = { Visibility = Visibility.Hidden }
        //            };
        //            msg.Show();
        //        }
        //    }
        //}
        private void cpmPopup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                cpmPopup.IsOpen = false;
                LV.SelectedItem = null;
            }
        }





        private void local_Selected(object sender, RoutedEventArgs e)
        {
            localPopup.IsOpen = true;
        }
        private void windowsYetki_Checked(object sender, RoutedEventArgs e)
        {
            v3_1.IsEnabled = false;
            v4_1.IsEnabled = false;
            comboBoxVeriTabani1.Items.Clear();
            if (!string.IsNullOrWhiteSpace(v1_1.Text))
            {
                try
                {
                    SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();
                    connection.DataSource = v1_1.Text;
                    connection.IntegratedSecurity = true;
                    String strConn = connection.ToString();
                    SqlConnection sqlConn = new SqlConnection(strConn);
                    sqlConn.Open();
                    SqlCommand command1 = new SqlCommand("SELECT name FROM master.sys.databases", sqlConn);
                    SqlDataReader reader1 = command1.ExecuteReader();
                    ArrayList arrlist = new ArrayList();
                    try
                    {
                        while (reader1.Read())
                        {
                            arrlist.Add(reader1["name"].ToString());
                        }
                        reader1.Close();
                    }
                    finally
                    {
                        sqlConn.Close();
                    }

                    for (int i = 0; i < arrlist.Count; i++)
                    {
                        comboBoxVeriTabani1.Items.Add(arrlist[i]);
                    }
                }
                catch
                {
                    CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                    {
                        TxtMessage = { Text = "Bağlantı sağlanamadı. Yanlış veriler giriyor olabilirsiniz.",
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
                }
            }
        }
        private void windowsYetki_Unchecked(object sender, RoutedEventArgs e)
        {
            v3_1.IsEnabled = true;
            v4_1.IsEnabled = true;
        }
        private void sqlYetki_Checked(object sender, RoutedEventArgs e)
        {
            v3_1.IsEnabled = true;
            v4_1.IsEnabled = true;
            v3_1.Focus();
        }
        private void sqlYetki_Unchecked(object sender, RoutedEventArgs e)
        {
            v3_1.IsEnabled = false;
            v4_1.IsEnabled = false;
        }
        private void kapat1_Click(object sender, RoutedEventArgs e)
        {
            localPopup.IsOpen = false;
            LV.SelectedItem = null;
        }
        private void v3_1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                v4_1.Focus();
        }
        private void localPopup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                localPopup.IsOpen = false;
                LV.SelectedItem = null;
            }
        }
        private void tamam1_Click(object sender, RoutedEventArgs e)
        {

            if (sqlYetki.IsChecked == true)
            {

                localdataSource = v1_1.Text;
                localuserId = v3_1.Text;
                localpassword = v4_1.Password;
                if (comboBoxVeriTabani1.SelectedIndex == -1)
                {
                    localinitialCatalog = "";
                }
                else
                {
                    localinitialCatalog = comboBoxVeriTabani1.SelectedItem.ToString();
                }

                if (!String.IsNullOrEmpty(localinitialCatalog))
                {
                    try
                    {
                        localdbConnStr = "Data Source=" + localdataSource + ";Initial Catalog=" + localinitialCatalog +
                        "; User ID=" + localuserId + ";Password=" + localpassword + ";Persist Security Info=True";
                        SqlConnection s = new SqlConnection(localdbConnStr);
                        s.Open();
                        if (s != null)
                        {
                            localPopup.IsOpen = false;
                            LV.SelectedItem = null;
                            Carried.DosyaDecrypt();
                            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Data Source", localdataSource, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("User ID", localuserId, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Password", localpassword, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Yetki", "SQL", System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("LocalConnection", localdbConnStr, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                            Carried.DosyaEncrypt();
                            //Properties.Settings.Default["LocalBaglantiStr"] = localdbConnStr;
                        }
                    }
                    catch
                    {
                        showMessage("Veritabanı bağlantısı sağlanamadı. Yanlış veriler giriyor olabilirsiniz.");
                    }

                }
                else
                {
                    showMessage("Bir veri tabanı seçiniz.");
                }
            }
            else if (windowsYetki.IsChecked == true)
            {
                localdataSource = v1_1.Text;
                if (comboBoxVeriTabani1.SelectedIndex == -1)
                {
                    localinitialCatalog = "";
                }
                else
                {
                    localinitialCatalog = comboBoxVeriTabani1.SelectedItem.ToString();
                }

                if (!String.IsNullOrEmpty(localinitialCatalog))
                {
                    try
                    {
                        localdbConnStr = "Data Source=" + localdataSource + ";Initial Catalog=" + localinitialCatalog +
                        ";Integrated Security=True";
                        SqlConnection s = new SqlConnection(localdbConnStr);
                        s.Open();
                        if (s != null)
                        {
                            localPopup.IsOpen = false;
                            LV.SelectedItem = null;
                            Carried.DosyaDecrypt();
                            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Data Source", localdataSource, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Yetki", "WINDOWS", System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("LocalConnection", localdbConnStr, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                            Carried.DosyaEncrypt();
                            //Properties.Settings.Default["LocalBaglantiStr"] = localdbConnStr;
                        }
                    }
                    catch { showMessage("Veritabanı bağlantısı sağlanamadı."); }
                }
                else showMessage("Bir veri tabanı seçin.");
            }
        }
        private void v4_1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                comboBoxVeriTabani1.Items.Clear();
                if (sqlYetki.IsChecked == true)
                {
                    try
                    {
                        SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();
                        connection.DataSource = v1_1.Text;
                        connection.IntegratedSecurity = false;
                        connection.UserID = v3_1.Text;
                        connection.Password = v4_1.Password;
                        String strConn = connection.ToString();
                        SqlConnection sqlConn = new SqlConnection(strConn);
                        sqlConn.Open();
                        SqlCommand command1 = new SqlCommand("SELECT name FROM master.sys.databases", sqlConn);
                        SqlDataReader reader1 = command1.ExecuteReader();
                        ArrayList arrlist = new ArrayList();
                        try
                        {
                            while (reader1.Read())
                            {
                                arrlist.Add(reader1["name"].ToString());
                            }
                            reader1.Close();
                        }
                        finally
                        {
                            sqlConn.Close();
                        }

                        for (int i = 0; i < arrlist.Count; i++)
                        {
                            comboBoxVeriTabani1.Items.Add(arrlist[i]);
                        }
                    }
                    catch
                    {
                        CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                        {
                            TxtMessage = { Text = "Bağlantı sağlanamadı. Yanlış veriler giriyor olabilirsiniz.",
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
                    }
                }
            }
        }
        private void v1_1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                v3_1.Focus();
        }
        private void localPopup_Closed(object sender, EventArgs e)
        {
            this.Opacity = 1;
            LV.SelectedItem = null;
        }
        private void localPopup_Opened(object sender, EventArgs e)
        {
            this.Opacity = 0.5;
            LV.SelectedItem = null;
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Properties.Settings.Default.Save();
        }




        SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();
        private void v4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    connection.DataSource = v1.Text;
                    connection.IntegratedSecurity = false;
                    connection.UserID = v3.Text;
                    connection.Password = v4.Password;
                    connection.InitialCatalog = secVT.Text;
                    String strConn = connection.ToString();

                    using (SqlConnection sqlConn = new SqlConnection(strConn))
                    {
                        sqlConn.Open();
                        string queryString = "select companyno, companyname, servername, databasename from SECCMP order by companyno";
                        DataTable table = new DataTable();
                        table.Columns.Add(new DataColumn("Seçilen", typeof(bool)));
                        table.Columns.Add(new DataColumn("Local Veri Tabanı", typeof(string)));
                        SqlDataAdapter a = new SqlDataAdapter(queryString, sqlConn);
                        a.Fill(table);
                        this.cpmSec.ItemsSource = table.DefaultView;
                        int x = 0;
                        foreach (DataGridColumn column in cpmSec.Columns)
                        {
                            if (x == 0) { x++; }
                            else if (x == 1) { column.DisplayIndex = 5; column.Width = 80; x++; }
                            else
                            {
                                column.IsReadOnly = true;
                            }
                        }
                        //List<Company> liste = new List<Company>();
                        //SqlCommand command = new SqlCommand("select count(companyname) from SECCMP", sqlConn);
                        //object sayi = command.ExecuteScalar();
                        //for (int i = 0; i < (int)sayi; i++) Company c = new Company();
                    }
                }
                catch { showMessage("Bağlantı sağlanamadı. Yanlış veriler giriyor olabilirsiniz."); }
            }
        }

        private void ileri_Click(object sender, RoutedEventArgs e)
        {
            List<List<string>> list2 = new List<List<string>>();
            Carried.DosyaDecrypt();
            string con = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
            Carried.DosyaEncrypt();
            SqlConnection sqllocal, sqlcpm; SqlCommand c;
            try
            {
                sqllocal = new SqlConnection(con);
                sqllocal.Open();
            }
            catch { showMessage("Local bağlantıyı kontrol edin."); return; }

            for (int i = 0; i < cpmSec.Items.Count; i++)
            {
                List<string> list1 = new List<string>();
                int x = 0;
                var item = cpmSec.Items[i];
                var mycheckbox = cpmSec.Columns[0].GetCellContent(item) as CheckBox;
                if (mycheckbox.IsChecked == true)
                {
                    foreach (DataGridColumn column in cpmSec.Columns)
                    {
                        if (x == 0) { x++; }
                        else
                        {
                            TextBlock cellContent = column.GetCellContent(cpmSec.Items[i]) as TextBlock;
                            if (x == 1 && string.IsNullOrWhiteSpace(cellContent.Text))
                            {
                                showMessage("Local veri tabanını yazın.");
                                return;
                            }
                            list1.Add(cellContent.Text);//localdbname, companyno, companyname, servername, cpmdbname
                        }
                    }
                    list2.Add(list1);
                }
                //else
                //{
                //    int y = 0; TextBlock cellContent = null;
                //    foreach (DataGridColumn column in cpmSec.Columns)
                //    {
                //        y++;
                //        if (y == 4) { cellContent = column.GetCellContent(cpmSec.Items[i]) as TextBlock; }
                //    }
                //    c = new SqlCommand("if exists (select companyname='"+ cellContent.Text +"') begin delete from smrtappcon where companyname='"+ cellContent.Text + "' end", sqllocal);
                //    c.ExecuteNonQuery();
                //}
            }

            if (list2.Count == 0) { showMessage("İlerlemeden önce şirket seçin."); }
            else
            {
                cpmPopup.IsOpen = false;
                LV.SelectedItem = null;
                //List<string> cpmBaglantilari = new List<string>();
                SqlConnectionStringBuilder connection2 = new SqlConnectionStringBuilder();
                SqlConnectionStringBuilder connectionlocal = new SqlConnectionStringBuilder();
                Carried.DosyaDecrypt();
                List<string> conlist = new List<string>();
                foreach (List<string> liste in list2)
                {
                    connection2.DataSource = liste[3];
                    connection2.IntegratedSecurity = false;
                    connection2.UserID = v3.Text;
                    connection2.Password = v4.Password;
                    connection2.InitialCatalog = liste[4];
                    String strConn = connection2.ToString();

                    connectionlocal.DataSource = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Data Source", "Baglanti.ini");
                    if (IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Yetki", "Baglanti.ini") == "SQL")
                    {
                        connectionlocal.UserID = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("User ID", "Baglanti.ini");
                        connectionlocal.Password = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Password", "Baglanti.ini");
                        connectionlocal.PersistSecurityInfo = true;
                    }
                    else
                    {
                        connectionlocal.IntegratedSecurity = true;
                    }
                    connectionlocal.InitialCatalog = liste[0];
                    String strConnLocal = connectionlocal.ToString();
                    conlist.Add(strConnLocal);
                    c = new SqlCommand("if not exists (select companyname from SMRTAPPCON where companyname = '" + liste[2] + "') begin insert into SMRTAPPCON values('" + liste[2] + "','" + strConn + "','" + strConnLocal + "'," + Convert.ToInt32(liste[1]) + ") end", sqllocal);
                    c.ExecuteNonQuery();
                }

                for (int i = 0; i < conlist.Count; i++)
                {
                    sqllocal = new SqlConnection(conlist[i]);
                    sqllocal.Open();
                    c1 = new SqlCommand("DELETE FROM SMRTAPPCON", sqllocal);
                    c1.ExecuteNonQuery();
                    foreach (List<string> liste in list2)
                    {
                        connection2.DataSource = liste[3];
                        connection2.IntegratedSecurity = false;
                        connection2.UserID = v3.Text;
                        connection2.Password = v4.Password;
                        connection2.InitialCatalog = liste[4];
                        String strConn = connection2.ToString();

                        connectionlocal.DataSource = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Data Source", "Baglanti.ini");
                        if (IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Yetki", "Baglanti.ini") == "SQL")
                        {
                            connectionlocal.UserID = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("User ID", "Baglanti.ini");
                            connectionlocal.Password = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("Password", "Baglanti.ini");
                            connectionlocal.PersistSecurityInfo = true;
                        }
                        else
                        {
                            connectionlocal.IntegratedSecurity = true;
                        }
                        connectionlocal.InitialCatalog = liste[0];
                        String strConnLocal = connectionlocal.ToString();
                        c = new SqlCommand("if not exists (select companyname from SMRTAPPCON where companyname = '" + liste[2] + "') begin insert into SMRTAPPCON values('" + liste[2] + "','" + strConn + "','" + strConnLocal + "',"+ Convert.ToInt32(liste[1]) + ") end", sqllocal);
                        c.ExecuteNonQuery();

                    }
                    CreateTableSMRTAPPSC(list2, sqllocal);
                }

                //string olusturulanString = "";
                //for (int i = 0; i < cpmBaglantilari.Count; i++)
                //{
                //    if (i == cpmBaglantilari.Count - 1) { olusturulanString += cpmBaglantilari[i]; }
                //    else olusturulanString += cpmBaglantilari[i] + ":";
                //    //MessageBox.Show(cpmBaglantilari[i]);
                //}
                //IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("CPMconnection", olusturulanString, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");

                string olusturulanString2 = "";
                foreach (List<string> liste in list2)
                {
                    for (int i = 0; i < liste.Count; i++)
                    {
                        if (i == liste.Count - 1) { olusturulanString2 += liste[i] + ";"; }
                        else olusturulanString2 += liste[i] + ",";
                    }
                }
                IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("CompaniesInformation", olusturulanString2, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");

                Carried.DosyaEncrypt();

                cpmPopupKul.IsOpen = true;
            }
        }

        private void secilenGor_Click(object sender, RoutedEventArgs e)
        {
            List<string> secilengorliste = new List<string>();
            for (int i = 0; i < cpmSec.Items.Count; i++)
            {
                var item = cpmSec.Items[i];
                var mycheckbox = cpmSec.Columns[0].GetCellContent(item) as CheckBox;
                if (mycheckbox.IsChecked == true) //her bir checkbox ile işaretlenen satirin 4. columndaki verisini al.
                {
                    TextBlock cellContent = cpmSec.Columns[2].GetCellContent(cpmSec.Items[i]) as TextBlock;
                    secilengorliste.Add(cellContent.Text);
                }
            }
            string sirketler = "";
            foreach (string item in secilengorliste)
            {
                sirketler += item + "\n";
            }
            showMessage("Seçilen Şirketler:\n" + sirketler);
        }




        private void cpmPopupKul_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                cpmPopupKul.IsOpen = false;
                LV.SelectedItem = null;
            }
        }
        private void kapat2_Click(object sender, RoutedEventArgs e)
        {
            cpmPopupKul.IsOpen = false;
            LV.SelectedItem = null;
        }
        private void cpmPopupKul_Opened(object sender, EventArgs e)
        {
            try
            {
                String strConn = connection.ToString();

                using (SqlConnection sqlConn = new SqlConnection(strConn))
                {
                    int x = 0;
                    sqlConn.Open();
                    string queryString = "select Username, Fullname, secusr.[DESCRIPTION] from SECUSR where USERTYPE = 0 order by USERNAME";
                    DataTable table = new DataTable();
                    table.Columns.Add(new DataColumn("Seçilen", typeof(bool)));
                    table.Columns.Add(new DataColumn("Password", typeof(string)));
                    SqlDataAdapter a = new SqlDataAdapter(queryString, sqlConn);
                    a.Fill(table);
                    this.cpmKul.ItemsSource = table.DefaultView;
                    foreach (DataGridColumn column in cpmKul.Columns)
                    {
                        if (x == 0) { x++; }
                        else if (x == 1) { column.DisplayIndex = 4; column.Width = 100; x++; }
                        else
                        {
                            column.IsReadOnly = true;
                        }
                    }
                }
            }
            catch { showMessage("Bağlantıyı kontrol edin."); }
        }
        private void geri_Click(object sender, RoutedEventArgs e)
        {
            cpmPopupKul.IsOpen = false;
            cpmPopup.IsOpen = true;
        }
        private void tamam_Click(object sender, RoutedEventArgs e)
        {
            LV.SelectedItem = null;
            List<List<string>> list2 = new List<List<string>>();
            for (int i = 0; i < cpmKul.Items.Count; i++)
            {
                List<string> list1 = new List<string>();
                var item = cpmKul.Items[i];
                var mycheckbox = cpmKul.Columns[0].GetCellContent(item) as CheckBox;
                if (mycheckbox.IsChecked == true)
                {
                    int x = 0;
                    foreach (DataGridColumn column in cpmKul.Columns)
                    {
                        if (x == 0 || x == 3 || x == 4) { x++; }//1 PASSWORD 2 USERNAME
                        else
                        {
                            TextBlock cellContent = column.GetCellContent(cpmKul.Items[i]) as TextBlock;
                            list1.Add(cellContent.Text);
                            x++;
                        }
                    }
                    list2.Add(list1);
                }
            }
            if (list2.Count == 0)
            {
                //showMessage("İlerlemeden önce kullanıcı seçin."); 
                cpmPopupKul.IsOpen = false;
                LV.SelectedItem = null;
            }
            else
            {
                Carried.DosyaDecrypt();
                string con = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
                Carried.DosyaEncrypt();
                try
                {
                    SqlConnection s = new SqlConnection(con);
                    s.Open();
                    SqlCommand c1 = new SqlCommand("select LOCALCONNECTIONSTRING from smrtappcon", s);
                    SqlDataReader reader1 = c1.ExecuteReader();
                    ArrayList arrlist = new ArrayList();
                    if (reader1 != null)
                    {
                        while (reader1.Read())
                        {
                            arrlist.Add(reader1["LOCALCONNECTIONSTRING"].ToString());
                        }
                        reader1.Close();
                    }
                    if (arrlist.Count != 0)
                    {
                        for (int i = 0; i < arrlist.Count; i++)
                        {
                            s = new SqlConnection(arrlist[i].ToString());
                            s.Open();
                            foreach (List<string> liste in list2)
                            {
                                if (!string.IsNullOrWhiteSpace(liste[0]))
                                {
                                    c1 = new SqlCommand("IF EXISTS (SELECT * FROM SMRTappkul WHERE KULLANICIADI = '" + liste[1] + "') BEGIN DELETE FROM SMRTAPPKUL WHERE KULLANICIADI='" + liste[1] + "' END INSERT INTO SMRTAPPKUL VALUES('" + liste[1] + "','" + liste[0] + "',0) ", s); //password,kullanıcıadı
                                    c1.ExecuteNonQuery();
                                }
                                else
                                {
                                    showMessage(liste[1] + "adlı kullanıcı için şifre girmeyi unuttunuz."); //bunu selection changed de yapmayı dene. 
                                    return;
                                }
                            }
                        }
                    }

                    cpmPopupKul.IsOpen = false;
                    LV.SelectedItem = null;
                }
                catch { showMessage("Local bağlantıyı kontrol edin."); }

            }
        }





        SqlConnection sql; SqlCommand c1, c2;
        private void sifreDegistir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(kullaniciAdi.Text))
            {
                showMessage("Kullanıcı adı giriniz.");
            }
            else
            {
                Carried.DosyaDecrypt();
                string con = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
                Carried.DosyaEncrypt();
                try
                {
                    sql = new SqlConnection(con);
                    sql.Open();
                    c1 = new SqlCommand("select KULLANICIADI from SMRTAPPKUL where KULLANICIADI='" + kullaniciAdi.Text + "'", sql);
                    object objvarmi = c1.ExecuteScalar();
                    if (objvarmi != null) sdadminPopup.IsOpen = true;
                    else { showMessage("Bu kullanıcı adında bir kullanıcı yok."); }
                }
                catch { showMessage("Local bağlantıyı kontrol edin."); }
            }
        }


        private void sdadminPopup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                sdadminPopup.IsOpen = false;
            }
        }
        private void sifreDegistirPopup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                sifreDegistirPopup.IsOpen = false;
            }
        }
        private void sdadminkapat_Click(object sender, RoutedEventArgs e)
        {
            sdadminPopup.IsOpen = false;
        }
        private void sdkapat_Click(object sender, RoutedEventArgs e)
        {
            sifreDegistirPopup.IsOpen = false;
        }

        private void btnsifreDegistir_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(yeniSifre.Password) && !string.IsNullOrWhiteSpace(yeniSifreTekrar.Password))
            {
                if (yeniSifre.Password == yeniSifreTekrar.Password)
                {
                    c1 = new SqlCommand("update SMRTAPPKUL set SIFRE = '" + yeniSifre.Password + "' where KULLANICIADI='" + kullaniciAdi.Text + "'", sql);
                    c1.ExecuteNonQuery();
                    showMessage("Şifre değiştirildi.");
                    sifreDegistirPopup.IsOpen = false;
                }
                else showMessage("Şifreler aynı değil.");
            }
            else showMessage("Yeni şifre giriniz.");
        }

        private void sdileri_Click(object sender, RoutedEventArgs e)
        {
            c1 = new SqlCommand("SELECT SIFRE FROM SMRTAPPKUL WHERE ADMIN=1", sql);
            c2 = new SqlCommand("SELECT KULLANICIADI FROM SMRTAPPKUL WHERE ADMIN=1", sql);
            object objadminSifre = c1.ExecuteScalar();
            object objadminKullaniciAdi = c2.ExecuteScalar();
            if (objadminSifre != null && objadminKullaniciAdi != null)
            {
                string stradminSifre = objadminSifre.ToString();
                string stradminKullaniciAdi = objadminKullaniciAdi.ToString();
                if (stradminSifre == adminSifre.Password || stradminKullaniciAdi == adminKullaniciAdi.Text)
                {
                    sdadminPopup.IsOpen = false;
                    sifreDegistirPopup.IsOpen = true;
                }
                else showMessage("Admin bilgileri yanlış.");
            }

        }



        private void adminkapat_Click(object sender, RoutedEventArgs e)
        {
            adminPopup.IsOpen = false;
        }
        private void adminileri_Click(object sender, RoutedEventArgs e)
        {
            Carried.DosyaDecrypt();
            string con = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
            Carried.DosyaEncrypt();
            if (LV.SelectedIndex == 5)
            {
                try
                {
                    tabKasa.Items.Clear();
                    sql = new SqlConnection(con);
                    sql.Open();
                    c1 = new SqlCommand("SELECT SIFRE FROM SMRTAPPKUL WHERE ADMIN=1", sql);
                    c2 = new SqlCommand("SELECT KULLANICIADI FROM SMRTAPPKUL WHERE ADMIN=1", sql);
                    object objadminSifre = c1.ExecuteScalar();
                    object objadminKullaniciAdi = c2.ExecuteScalar();
                    if (objadminSifre != null && objadminKullaniciAdi != null)
                    {
                        string stradminSifre = objadminSifre.ToString();
                        string stradminKullaniciAdi = objadminKullaniciAdi.ToString();
                        if (stradminSifre == adminSifre2.Password && stradminKullaniciAdi == adminKullaniciAdi2.Text)
                        {
                            adminPopup.IsOpen = false;
                            kasaPopup.IsOpen = true;
                            Kasa kasa = new Kasa();
                            TabItem tabUserPage = new TabItem { Content = kasa };
                            tabKasa.Items.Add(tabUserPage);
                            tabKasa.Items.Refresh();
                            LV.SelectedItem = null;
                        }
                        else showMessage("Admin bilgileri yanlış.");
                    }
                }
                catch { showMessage("Local bağlantıyı kontrol edin."); }
            }
            else if (LV.SelectedIndex == 6) 
            {
                try
                {
                    sql = new SqlConnection(con);
                    sql.Open();
                    c1 = new SqlCommand("SELECT SIFRE FROM SMRTAPPKUL WHERE ADMIN=1", sql);
                    c2 = new SqlCommand("SELECT KULLANICIADI FROM SMRTAPPKUL WHERE ADMIN=1", sql);
                    object objadminSifre = c1.ExecuteScalar();
                    object objadminKullaniciAdi = c2.ExecuteScalar();
                    if (objadminSifre != null && objadminKullaniciAdi != null)
                    {
                        string stradminSifre = objadminSifre.ToString();
                        string stradminKullaniciAdi = objadminKullaniciAdi.ToString();
                        if (stradminSifre == adminSifre2.Password && stradminKullaniciAdi == adminKullaniciAdi2.Text)
                        {
                            adminPopup.IsOpen = false;
                            ParametreEkrani p = new ParametreEkrani();
                            p.Width = this.ActualWidth;
                            p.Height = this.ActualHeight;
                            p.WindowState = this.WindowState;
                            p.VerticalAlignment = this.VerticalAlignment;
                            p.HorizontalAlignment = this.HorizontalAlignment;
                            p.Show();
                            this.Close();
                        }
                        else showMessage("Admin bilgileri yanlış.");
                    }
                }
                catch { showMessage("Local bağlantıyı kontrol edin."); }
            }
            else if (LV.SelectedIndex == 1)
            {
                try
                {
                    sql = new SqlConnection(con);
                    sql.Open();
                    c1 = new SqlCommand("SELECT SIFRE FROM SMRTAPPKUL WHERE ADMIN=1", sql);
                    c2 = new SqlCommand("SELECT KULLANICIADI FROM SMRTAPPKUL WHERE ADMIN=1", sql);
                    object objadminSifre = c1.ExecuteScalar();
                    object objadminKullaniciAdi = c2.ExecuteScalar();
                    if (objadminSifre != null && objadminKullaniciAdi != null)
                    {
                        string stradminSifre = objadminSifre.ToString();
                        string stradminKullaniciAdi = objadminKullaniciAdi.ToString();
                        if (stradminSifre == adminSifre2.Password && stradminKullaniciAdi == adminKullaniciAdi2.Text)
                        {
                            adminPopup.IsOpen = false;
                            cpmPopup.IsOpen = true;
                        }
                        else showMessage("Admin bilgileri yanlış.");
                    }
                }
                catch { showMessage("Local bağlantıyı kontrol edin."); }
            }
        }




        private void sirketSec_Selected(object sender, RoutedEventArgs e)
        {
            Carried.DosyaDecrypt();
            string con = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
            Carried.DosyaEncrypt();
            SqlConnection sqllocal, sqlcpm;
            try
            {
                sqllocal = new SqlConnection(con);
                sqllocal.Open();
            }
            catch { showMessage("Local bağlantıyı kontrol edin."); return; }

            SqlCommand command1 = new SqlCommand("select COMPANYNAME from smrtappcon", sqllocal);
            SqlDataReader reader1 = command1.ExecuteReader();
            ArrayList sirketler = new ArrayList();
            try
            {
                while (reader1.Read())
                {
                    sirketler.Add(reader1["COMPANYNAME"].ToString());
                }
            }
            finally { reader1.Close(); }

            if (sirketler.Count > 0)
            {
                listbox.Items.Clear();
                for (int i = 0; i < sirketler.Count; i++)
                {
                    listbox.Items.Add(sirketler[i]);  //(new RadioButton() { Content = sirketler[i], VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Foreground = Brushes.White });
                }
                sirketlerPopup.IsOpen = true;
            }
            else showMessage("Önce cpm bağlantısından şirket seçiniz.");
        }

        private void listbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string sirketadi = listbox.Items[listbox.SelectedIndex].ToString();
            Carried.DosyaDecrypt();
            string con = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
            Carried.DosyaEncrypt();
            SqlConnection sqllocal, sqlcpm;
            try
            {
                sqllocal = new SqlConnection(con);
                sqllocal.Open();
            }
            catch { showMessage("Local bağlantıyı kontrol edin."); return; }
            SqlCommand command1 = new SqlCommand("select CONNECTIONSTRING from smrtappcon WHERE COMPANYNAME='" + sirketadi + "'", sqllocal);
            string cs = command1.ExecuteScalar().ToString();
            command1 = new SqlCommand("select LOCALCONNECTIONSTRING from smrtappcon WHERE COMPANYNAME='" + sirketadi + "'", sqllocal);
            string lcs = command1.ExecuteScalar().ToString();
            command1 = new SqlCommand("select COMPANYNO from smrtappcon WHERE COMPANYNAME='" + sirketadi + "'", sqllocal);
            string sirketno = command1.ExecuteScalar().ToString();

            Carried.DosyaDecrypt();
            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("CPMconnection", cs, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("LocalConnection", lcs, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("SirketSecildiMi", "True", System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
            IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("SirketNo", sirketno, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
            Carried.DosyaEncrypt();
            sirketlerPopup.IsOpen = false;
        }

        private void sirketlerPopup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                sirketlerPopup.IsOpen = false;
            }
        }




        private void verileriGetir_Selected(object sender, RoutedEventArgs e)
        {
            ButtonAutomationPeer peer = new ButtonAutomationPeer(verileriGetir);
            IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProv.Invoke();
        }

        private void verileriGetir_Click(object sender, RoutedEventArgs e)
        {
            if (Carried.CheckForInternetConnection() == true)
            {
                if (IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("SirketSecildiMi", "Baglanti.ini") != null)
                {
                    CustomMaterialMessageBox msg = new CustomMaterialMessageBox
                    {
                        TxtMessage = { Text = "Bu işlem uzun zaman alabilir. Yapmak istediğinize emin misiniz?",
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
                    if (results == MessageBoxResult.Cancel) LV.SelectedItem = -1;
                    else if (results == MessageBoxResult.OK)
                    {
                        LV.SelectedItem = -1;
                        progressPopup.IsOpen = true;
                        progressBar.Value = 1;
                        TablolariGetir.Baglan();
                        progressBar.Value += 10;
                        TablolariGetir.CreateSMRTAPPSKRT();
                        progressBar.Value += 25;
                        TablolariGetir.CreateSMRTAPPCKRT();
                        progressBar.Value += 25;
                        TablolariGetir.CreateSMRTAPPSRES();
                        progressBar.Value += 20;
                        //TablolariGetir.SMRTAPPBAS();
                        //progressBar.Value += 20;
                        TablolariGetir.CreateSMRTAPPDEPO();
                        progressBar.Value += 20;
                        TablolariGetir.CreateSMRTAPPETIKET();
                        progressBar.Value += 10;
                        TablolariGetir.CreateSRKKRT();
                        progressBar.Value += 10;

                        if (progressBar.Value >= 121)
                        {
                            showMessage("Tamamlandı."); 
                            progressPopup.IsOpen = false;
                        }
                    }
                }
                else Carried.showMessage("Önce şirket seçin.");
            }
            else { Carried.showMessage("İnternet bağlantınız yok.\nİnternet bağlantısı olmadan bu işlemi yapamazsınız."); }
        }

        private void transfer_Selected(object sender, RoutedEventArgs e)
        {
            cbtransfer.Items.Clear();
            cbtransfer.Items.Add("None");
            cbtransfer.Items.Add("SKOD1");
            cbtransfer.Items.Add("SKOD2");
            cbtransfer.Items.Add("SKOD3");
            cbtransfer.Items.Add("SKOD4");
            cbtransfer.Items.Add("SKOD5");
            cbtransfer.Items.Add("BKOD1");
            cbtransfer.Items.Add("BKOD2");
            cbtransfer.Items.Add("BKOD3");
            cbtransfer.Items.Add("BKOD4");
            cbtransfer.Items.Add("BKOD5");
            Popup_transfer.IsOpen = true;
            LV.SelectedItem = null;
        }

        private void kapat3_Click(object sender, RoutedEventArgs e)
        {
            Popup_transfer.IsOpen = false;
            LV.SelectedItem = null;
        }

        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            Popup_transfer.IsOpen = false;
            if (cbtransfer.SelectedItem != null)
            {
                Carried.DosyaDecrypt();
                IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("Transfer", cbtransfer.SelectedItem.ToString(), System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                Carried.DosyaEncrypt();
            }
        }

        private void parametreEkrani_Selected(object sender, RoutedEventArgs e)
        {
            adminPopup.IsOpen = true;
        }
        private void kasa_Selected(object sender, RoutedEventArgs e)
        {
            adminPopup.IsOpen = true;
        }

        private void kapatkasa_Click(object sender, RoutedEventArgs e)
        {
            kasaPopup.IsOpen = false;
        }




        public static decimal FromTryToEur = 0, FromTryToUsd = 0, FromUsdToTry = 0, FromEurToTry = 0, FromEurToUsd = 0, FromUsdToEur = 0;
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)//internetten döviz kurlarını alıp, localdeki tabloya yazıyor
        {
            try
            {
                FromTryToEur = Carried.CurrencyConversion(1, "try", "eur");
                FromTryToUsd = Carried.CurrencyConversion(1, "try", "usd");
                FromUsdToTry = Carried.CurrencyConversion(1, "usd", "try");
                FromEurToTry = Carried.CurrencyConversion(1, "eur", "try");
                FromEurToUsd = Carried.CurrencyConversion(1, "eur", "usd");
                FromUsdToEur = Carried.CurrencyConversion(1, "usd", "eur");
                Carried.DosyaDecrypt();
                string con = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
                Carried.DosyaEncrypt();
                if (!String.IsNullOrWhiteSpace(con))
                {
                    SqlConnection s = new SqlConnection(con);
                    s.Open();
                    SqlCommand c = new SqlCommand("insert into SMRTAPPDVZ([FROM],[TO],[ORAN],[TARIH]) VALUES('TRY', 'EUR', " + FromTryToEur.ToString().Replace(",", ".") + ", @tarih)", s);
                    c.Parameters.Add("@tarih", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                    c.ExecuteNonQuery();
                    c = new SqlCommand("insert into SMRTAPPDVZ([FROM],[TO],[ORAN],[TARIH]) VALUES('TRY', 'USD', " + FromTryToUsd.ToString().Replace(",", ".") + ", @tarih)", s);
                    c.Parameters.Add("@tarih", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                    c.ExecuteNonQuery();
                    c = new SqlCommand("insert into SMRTAPPDVZ([FROM],[TO],[ORAN],[TARIH]) VALUES('USD', 'TRY', " + FromUsdToTry.ToString().Replace(",", ".") + ", @tarih)", s);
                    c.Parameters.Add("@tarih", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                    c.ExecuteNonQuery();
                    c = new SqlCommand("insert into SMRTAPPDVZ([FROM],[TO],[ORAN],[TARIH]) VALUES('EUR', 'TRY', " + FromEurToTry.ToString().Replace(",", ".") + ", @tarih)", s);
                    c.Parameters.Add("@tarih", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                    c.ExecuteNonQuery();
                    c = new SqlCommand("insert into SMRTAPPDVZ([FROM],[TO],[ORAN],[TARIH]) VALUES('EUR', 'USD', " + FromEurToUsd.ToString().Replace(",", ".") + ", @tarih)", s);
                    c.Parameters.Add("@tarih", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                    c.ExecuteNonQuery();
                    c = new SqlCommand("insert into SMRTAPPDVZ([FROM],[TO],[ORAN],[TARIH]) VALUES('USD', 'EUR', " + FromUsdToEur.ToString().Replace(",", ".") + ", @tarih)", s);
                    c.Parameters.Add("@tarih", SqlDbType.SmallDateTime).Value = DateTime.Now.Date;
                    c.ExecuteNonQuery();
                }
            }
            catch
            {
                bool netcon = Carried.CheckForInternetConnection();
                if (netcon == false) { Carried.showMessage("İnternet bağlantısı olmadığı için son döviz kurları alınamadı."); }
            }
        }










        //sor ve sor burada da apiye ihtiyaç var mı?
        public void CreateTableSMRTAPPSC(List<List<string>> liste, SqlConnection sqllocal)
        {
            //Carried.DosyaDecrypt();
            //string con = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
            //Carried.DosyaEncrypt();
            SqlConnection sqlcpm;
            //try
            //{
            //    sqllocal = new SqlConnection(con);
            //    sqllocal.Open();
            //}
            //catch { showMessage("Local bağlantıyı kontrol edin."); return; }
            c1 = new SqlCommand("DELETE FROM SMRTAPPSC", sqllocal);
            c1.ExecuteNonQuery();
            try
            {
                SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder();
                connection.DataSource = v1.Text;
                connection.IntegratedSecurity = false;
                connection.UserID = v3.Text;
                connection.Password = v4.Password;
                connection.InitialCatalog = secVT.Text;
                String strConn = connection.ToString();
                sqlcpm = new SqlConnection(strConn);
                sqlcpm.Open();
            }
            catch { showMessage("Bağlantı sağlanamadı. Yanlış veriler giriyor olabilirsiniz."); return; }

            int id;
            //string smrtappdbname = "CPMSMARTAPP";

            //int rows = (int)new SqlCommand("select COUNT(*) from SECCMP", sqlcpm).ExecuteScalar();
            for (int i = 0; i < liste.Count; i++)
            {
                c1 = new SqlCommand("SELECT ID FROM SMRTAPPSC WHERE ID=(SELECT max(ID) FROM SMRTAPPSC)", sqllocal);
                object id_obj = c1.ExecuteScalar();
                if (id_obj == null) { id = 1; }
                else
                {
                    id = (int)id_obj + 1;
                }
                string companyno = (string)new SqlCommand("WITH CTE AS (SELECT COMPANYNO, ROW_NUMBER() OVER (ORDER BY COMPANYNO) AS 'row' FROM SECCMP) SELECT COMPANYNO FROM CTE WHERE row =" + (i + 1).ToString(), sqlcpm).ExecuteScalar();
                string companyname = (string)new SqlCommand("WITH CTE AS (SELECT COMPANYNAME, ROW_NUMBER() OVER (ORDER BY COMPANYNO) AS 'row' FROM SECCMP) SELECT COMPANYNAME FROM CTE WHERE row =" + (i + 1).ToString(), sqlcpm).ExecuteScalar();
                string servername = (string)new SqlCommand("WITH CTE AS (SELECT SERVERNAME, ROW_NUMBER() OVER (ORDER BY COMPANYNO) AS 'row' FROM SECCMP) SELECT SERVERNAME FROM CTE WHERE row =" + (i + 1).ToString(), sqlcpm).ExecuteScalar();
                string cpmdbname = (string)new SqlCommand("WITH CTE AS (SELECT DATABASENAME, ROW_NUMBER() OVER (ORDER BY COMPANYNO) AS 'row' FROM SECCMP) SELECT DATABASENAME FROM CTE WHERE row =" + (i + 1).ToString(), sqlcpm).ExecuteScalar();
                short status = (short)new SqlCommand("WITH CTE AS (SELECT STATUS, ROW_NUMBER() OVER (ORDER BY COMPANYNO) AS 'row' FROM SECCMP) SELECT STATUS FROM CTE WHERE row =" + (i + 1).ToString(), sqlcpm).ExecuteScalar();
                string supportcode = (string)new SqlCommand("WITH CTE AS (SELECT SUPPORTCODE, ROW_NUMBER() OVER (ORDER BY COMPANYNO) AS 'row' FROM SECCMP) SELECT SUPPORTCODE FROM CTE WHERE row =" + (i + 1).ToString(), sqlcpm).ExecuteScalar();
                c1 = new SqlCommand("if exists (select * from smrtappsc where COMPANYNO = '" + companyno + "') BEGIN DELETE FROM SMRTAPPSC WHERE COMPANYNO='" + companyno + "' END insert into smrtappsc(ID, companyno, companyname, servername, smrtappdbname, cpmdbname, status, supportcode) " +
                    "values(" + id.ToString() + "," + companyno + ",'" + companyname + "','" + servername + "','" + liste[i][0] + "','" + cpmdbname + "'," + status.ToString() + ",'" + supportcode + "')", sqllocal);
                c1.ExecuteNonQuery();
            }
        }
    }




}




//class Company
//{
//    public string companyname { get; set; }
//    public string servername { get; set; }
//    public string databasename { get; set; }

//    public Company(string _companyname, string _servername, string _databasename)
//    {
//        companyname = _companyname;
//        servername = _servername;
//        databasename = _databasename;
//    }
//}







//Dictionary<int, List<string>> dict = new Dictionary<int, List<string>>();
//Dictionary<int, List<List<string>>> dict2 = new Dictionary<int, List<List<string>>>();
//private void Row_Selected(object sender, RoutedEventArgs e)
//{
//    //int currentRowIndex = cpmSec.Items.IndexOf(cpmSec.CurrentItem);
//    //showMessage(currentRowIndex.ToString() + " index numaralı şirket seçildi.");

//    //foreach (DataGridColumn column in cpmSec.Columns)
//    //{
//    //    if (column.GetCellContent(cpmSec.Items[currentRowIndex]) is TextBlock)
//    //    {
//    //        TextBlock cellContent = column.GetCellContent(cpmSec.Items[currentRowIndex]) as TextBlock;
//    //        list.Add(cellContent.Text);
//    //        //MessageBox.Show(cellContent.Text);
//    //    }
//    //}
//    //if (!dict.ContainsKey(currentRowIndex))
//    //{
//    //    dict.Add(currentRowIndex, list);
//    //    //list2.Add(list);
//    //    //dict2.Add(currentRowIndex, list2);
//    //}

//    //DataGridRow row = (DataGridRow)cpmSec.ItemContainerGenerator.ContainerFromIndex(currentRowIndex); 
//    //row.Background = new SolidColorBrush(Colors.SpringGreen);

//    ////String strConn = connection.ToString();
//    ////using (SqlConnection sqlConn = new SqlConnection(strConn))
//    ////{
//    ////    sqlConn.Open();
//    ////    string queryString = "WITH CTE AS (SELECT companyno, ROW_NUMBER() OVER (ORDER BY companyno) AS 'row' FROM SECCMP ) SELECT * FROM CTE WHERE row="+(currentRowIndex+1);
//    ////    SqlCommand command = new SqlCommand(queryString, sqlConn);
//    ////    string companyno = command.ExecuteScalar().ToString();
//    ////    companynumbers.Add(companyno);
//    ////}
//}
//private void Row_DoubleClick(object sender, RoutedEventArgs e)
//{
//    //int currentRowIndex = cpmSec.Items.IndexOf(cpmSec.CurrentItem);
//    //showMessage(currentRowIndex.ToString()+" index numaralı şirket seçilmekten vazgeçildi.");
//    //if (dict.ContainsKey(currentRowIndex))
//    //{
//    //    dict.Remove(currentRowIndex);
//    //    //dict2.Remove(currentRowIndex);
//    //}
//    //DataGridRow row = (DataGridRow)cpmSec.ItemContainerGenerator.ContainerFromIndex(currentRowIndex);
//    //row.Background = new SolidColorBrush(Colors.White);
//}




//private void button1_Click(object sender, RoutedEventArgs e)
//{
//    Window1 w = new Window1();
//    w.Show();
//    this.Close();
//    w.Width = this.ActualWidth;
//    w.Height = this.ActualHeight;
//    w.WindowState = this.WindowState;
//    w.VerticalAlignment = this.VerticalAlignment;
//    w.HorizontalAlignment = this.HorizontalAlignment;
//}

//private void Giris_Click(object sender, RoutedEventArgs e)
//{
//    girisEkran.Visibility = Visibility.Hidden;
//}

//private void button2_Click(object sender, RoutedEventArgs e)
//{
//    string sourceConnStr = "Data Source=SMARTSRV\\CPM;Initial Catalog=CPM_UYG;Persist Security Info=True;User ID=Sa;Password=Nova1881";
//    string destinationConnStr = "Data Source=DESKTOP-6PSD2C4; Initial Catalog=CPMSMARTAPP; Integrated Security=True";
//    ////using (SqlConnection sourceConnection = new SqlConnection(sourceConnStr))
//    ////{
//    ////sourceConnection.Open();
//    //ArrayList arrlist = new ArrayList();
//    //// Get data from the source table as a SqlDataReader.
//    ////SqlCommand commandSourceData = new SqlCommand("SELECT EVRAKNO FROM EVRBAS", sourceConnection);
//    ////SqlDataReader reader = commandSourceData.ExecuteReader();
//    //if (reader != null)
//    //{
//    //    while (reader.Read())
//    //    {
//    //        arrlist.Add(reader["EVRAKNO"].ToString());
//    //    }
//    //    reader.Close();
//    //}
//    ////using (SqlConnection destinationConnection = new SqlConnection(destinationConnStr))
//    ////{
//    //    destinationConnection.Open();
//    //    for (int i=0; i<arrlist.Count; i++)
//    //    {
//    //        SqlCommand commandDestinationData = new SqlCommand("INSERT INTO SMRTAPPBAS (ID) VALUES (" + i + ")", destinationConnection);
//    //        commandDestinationData.ExecuteNonQuery();

//    //        commandDestinationData = new SqlCommand("INSERT INTO SMRTAPPBAS (EVRAKNO) VALUES ('"+ arrlist[i].ToString() +"') WHERE ID = "+ i.ToString(), destinationConnection);
//    //        commandDestinationData.ExecuteNonQuery();

//    //        commandDestinationData = new SqlCommand("INSERT INTO SMRTAPPBAS (EVRAKNO) VALUES ('" + arrlist[i].ToString() + "')", destinationConnection);
//    //        commandDestinationData.ExecuteNonQuery();

//    //    }
//    // Set up the bulk copy object.
//    // Note that the column positions in the source
//    // data reader match the column positions in
//    // the destination table so there is no need to
//    // map columns.
//    ////using (SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection))
//    ////{
//    ////    bulkCopy.DestinationTableName = "SMRTAPPBAS";
//    ////    bulkCopy.ColumnMappings.Add("EVRAKNO", "EVRAKNO");
//    ////    try
//    ////    {
//    ////        bulkCopy.WriteToServer(reader);
//    ////    }
//    ////    //catch (Exception ex)
//    ////    //{
//    ////    //    Console.WriteLine(ex.Message);
//    ////    //}
//    ////    finally
//    ////    {
//    ////        reader.Close();
//    ////    }
//    ////}

//    // Perform a final count on the destination
//    // table to see how many rows were added.
//    //long countEnd = System.Convert.ToInt32(
//    //    commandRowCount.ExecuteScalar());
//    //Console.WriteLine("Ending row count = {0}", countEnd);
//    //Console.WriteLine("{0} rows were added.", countEnd - countStart);
//    //Console.WriteLine("Press Enter to finish.");
//    //Console.ReadLine();
//    ////    }
//    ////}

//    SqlConnection sourceConnection = new SqlConnection(sourceConnStr);
//    SqlConnection destinationConnection = new SqlConnection(destinationConnStr);
//    sourceConnection.Open();
//    destinationConnection.Open();

//    //var common = from c in dt.AsEnumerable()
//    //             join x in dt2.AsEnumerable() on c.Field<string>("ID") equals x.Field<string>("ID")
//    //             select new object[] { c["ID"], c["First Name"], x["Birthday"] };
//    //DataTable dt3 = new DataTable();
//    //dt3.Columns.Add("ID");
//    //dt3.Columns.Add("Name");
//    //dt3.Columns.Add("Birthdate");
//    //foreach (var item in common)
//    //    dt3.LoadDataRow(item.ToArray(), true);

//    /*
//    var dt = new DataTable();
//    dt.Columns.Add("EVRAKNO");
//    dt.Columns.Add("KARSIUNVAN");
//    dt.Columns.Add("REFNO");
//    dt.Rows.Add("TR000000626", "unvan1", "r1");
//    dt.Rows.Add("TR000000627", "unvan2", "r2");
//    dt.Rows.Add("TR000000628", "unvan3", "r3");

//    string cmdstrsayi = "select count(*) from ( select evrbas.[ID],[EVRAKNO],[HESAPTIP],evrbas.[HESAPKOD],[UNVAN],[EFATURADURUM],[EARSIV],[PKETIKET],evrbas.[DOVIZCINS]," +
//        "[EVRAKDOVIZCINS],[EVRAKTARIH],evrbas.[ACIKLAMA1],[KARSIHESAPKOD],[_MIKTAR],[_EVRAKDURUM],EVRBAS.[KAYITDURUM] from EVRBAS" +
//        " inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD inner join EFABAS on evrbas.EVRAKSN = EFABAS.EVRAKSN ) as sayi";
//    SqlCommand commandSourceDataMiktar = new SqlCommand(cmdstrsayi, sourceConnection);
//    int rows = Convert.ToInt32(commandSourceDataMiktar.ExecuteScalar());

//    //string cmdstr = "select evrbas.[ID],[EVRAKNO],[HESAPTIP],evrbas.[HESAPKOD],[UNVAN],[EFATURADURUM],[EARSIV],[PKETIKET],evrbas.[DOVIZCINS]," +
//    //    "[EVRAKDOVIZCINS],[EVRAKTARIH],evrbas.[ACIKLAMA1],[KARSIHESAPKOD],[_MIKTAR],[_EVRAKDURUM],EVRBAS.[KAYITDURUM] from EVRBAS" +
//    //    " inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD inner join EFABAS on evrbas.EVRAKSN = EFABAS.EVRAKSN";

//    for (int i = 1; i <= rows; i++)
//    {
//        SqlCommand c = new SqlCommand("WITH CTE AS (SELECT EVRBAS.ID, ROW_NUMBER() OVER (ORDER BY EVRBAS.EVRAKNO) AS 'row' FROM EFABAS inner join EVRBAS ON EVRBAS.EVRAKSN=EFABAS.EVRAKSN inner join CARKRT on evrbas.HESAPKOD = CARKRT.HESAPKOD) SELECT * FROM CTE WHERE row="+i.ToString(), sourceConnection); 
//        int s1 = Convert.ToInt32(c.ExecuteScalar()); MessageBox.Show(s1.ToString());
//        c = new SqlCommand("", sourceConnection);
//        string s2 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s3 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s4 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s5 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s6 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s7 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s8 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s9 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s10 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s11 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s12 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s13 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s14 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s15 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s16 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s17 = c.ExecuteScalar().ToString();
//        c = new SqlCommand("", sourceConnection);
//        string s18 = c.ExecuteScalar().ToString();
//        string komut = "insert into SMRTAPPBAS (ID, EVRAKNO, HESAPTIP, HESAPKOD, UNVAN, EFATURADURUM, EARSIV, PKETIKET, DOVIZCINS, EVRAKDOVIZCINS, EVRAKTARIH, ACIKLAMA,KARSIHESAPKOD,MIKTAR,EVRAKDURUM, KAYITDURUM ) " + //KARSIUNVAN, REFNO yok
//            " values (" + s1 + "," + s2 + "," + s3 + "," + s4 + "," + s5 + "," + s6 + "," + s7 + "," + s8 + "," + s9 + "," + s10 + "," + s11 + "," + s12 + "," + s13 + "," + s14 + "," + s15 + "," + s16 + "," + s17 + "," + s18 + ")";
//        SqlCommand commandDestinationData = new SqlCommand(komut, destinationConnection);
//        commandDestinationData.ExecuteNonQuery();
//    }
//    */


//    //SqlDataReader reader = commandSourceData.ExecuteReader();
//    //while (reader.Read())
//    //{
//    //    for (int i = 0; i<rows; i++)
//    //    {
//    //        MessageBox.Show(reader[i].ToString());

//    //    }
//    //}

//    //string komut = "insert into SMRTAPPBAS (ID, EVRAKNO, HESAPTIP, HESAPKOD, UNVAN, EFATURADURUM, EARSIV, PKETIKET, DOVIZCINS, EVRAKDOVIZCINS, EVRAKTARIH, ACIKLAMA,KARSIHESAPKOD,KARSIUNVAN,REFNO,MIKTAR,EVRAKDURUM, KAYITDURUM ) " +
//    //    " values (" + reader[i].ToString() + ")";
//    //SqlCommand commandDestinationData = new SqlCommand(komut, destinationConnection);
//    //commandDestinationData.ExecuteNonQuery();


//    //    SqlDataReader reader = commandSourceData.ExecuteReader();
//    //    var dt1 = new DataTable();
//    //    dt1.Columns.Add("ID");
//    //    dt1.Columns.Add("EVRAKNO");
//    //    dt1.Columns.Add("HESAPTIP");
//    //    dt1.Columns.Add("HESAPKOD");
//    //    dt1.Columns.Add("UNVAN");
//    //    dt1.Columns.Add("EFATURADURUM");
//    //    dt1.Columns.Add("EARSIVDURUM");
//    //    dt1.Columns.Add("PKETIKET");
//    //    dt1.Columns.Add("DOVIZCINS");
//    //    dt1.Columns.Add("EVRAKDOVIZCINS");
//    //    dt1.Columns.Add("EVRAKTARIH");
//    //    dt1.Columns.Add("ACIKLAMA");
//    //    dt1.Columns.Add("KARSIHESAPKOD");
//    //    dt1.Columns.Add("KARSIUNVAN");
//    //    dt1.Columns.Add("REFNO");
//    //    dt1.Columns.Add("MIKTAR");
//    //    dt1.Columns.Add("EVRAKDURUM");
//    //    dt1.Columns.Add("KAYITDURUM");

//    //    while (reader.Read())
//    //    {
//    //        dt1.Rows.Add(reader["EVRAKNO"]);
//    //    }
//    //    reader.Close();
//}