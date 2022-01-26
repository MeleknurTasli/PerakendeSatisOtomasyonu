using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Kasa.xaml
    /// </summary>
    public partial class Kasa : UserControl
    {
        SqlConnection s, s1;
        SqlCommand c;
        string loc, cpm, kasaNo;
        int sirketno;

        public Kasa()
        {
            InitializeComponent();
        }

        private void rbmanuel_Checked(object sender, RoutedEventArgs e)
        {
            cb2.Visibility = Visibility.Hidden;
            tb2.Clear();
            tb2.Visibility = Visibility.Visible;
        }

        private void rbcpm_Checked(object sender, RoutedEventArgs e)
        {
            cb2.SelectedItem = null;
            cb2.Visibility = Visibility.Visible;
            tb2.Visibility = Visibility.Hidden;

            try
            {
                s = new SqlConnection(Carried.girisBaglantiCPM);
                s.Open();
                c = new SqlCommand("select SUBEAD FROM SRKSUB WHERE SIRKETNO = '" + new String('0', (3 - Carried.sirketNo.ToString().Length)) + "'");
                SqlDataReader reader1 = c.ExecuteReader();
                ArrayList arrlist = new ArrayList();
                while (reader1.Read())
                {
                    arrlist.Add(reader1["SUBEAD"].ToString());
                }
                reader1.Close();
                for (int i = 0; i < arrlist.Count; i++)
                {
                    cb2.Items.Add(arrlist[i]);
                }
            }
            catch
            {
                bool netcon = Carried.CheckForInternetConnection();
                if (netcon == false) Carried.showMessage("Bu işlemi internetsiz gerçekleştiremezsiniz.");
            }




        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Carried.DosyaDecrypt();
                loc = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("LocalConnection", "Baglanti.ini");
                cpm = IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("CPMconnection", "Baglanti.ini");
                sirketno =  Convert.ToInt32(IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("SirketNo", "Baglanti.ini"));
                kasaNo =  IniDosyaIslemleri.IniDosyaIslemleri.GetValueFromIniFile("KasaNo", "Baglanti.ini");
                Carried.DosyaEncrypt();
                Carried.girisBaglantiLocal = loc;
                Carried.girisBaglantiCPM = cpm;
                if(!string.IsNullOrEmpty(kasaNo)) tb1.Text = kasaNo;
                s = new SqlConnection(Carried.girisBaglantiLocal);
                s.Open();
                DataTable table = new DataTable();
                table.Columns.Add(new DataColumn("Seçilen", typeof(bool)));
                SqlDataAdapter a = new SqlDataAdapter("select KULLANICIADI as Kullanıcı from SMRTAPPKUL", s);
                a.Fill(table);
                this.KUL_dataGrid.ItemsSource = table.DefaultView;
                int x = 0;
                foreach (DataGridColumn column in KUL_dataGrid.Columns)
                {
                    if (x == 0) { x++; }
                    else
                    {
                        column.IsReadOnly = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Carried.showMessage("Kullanıcı belirleme hatası.\n" + ex.Message);
            }
        }

        private void tb1_PreviewTextInput(object sender, TextCompositionEventArgs e) //TURKCE KARAKTERLERI KISITLA
        {
            Regex regex = new Regex("^[a-zA-Z0-9]+$"); //TURKCE KARAKTERLERI KISITLA
            if (!regex.IsMatch(e.Text))
                e.Handled = true;
            base.OnPreviewTextInput(e);
        }

        private void tb5_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        
        private void tamamkasa_Click(object sender, RoutedEventArgs e)
        {
            ArrayList arraylist = new ArrayList();
            string tanim = rbmanuel.IsChecked == true ? tb2.Text : rbcpm.IsChecked == true && cb2.SelectedItem != null ? cb2.SelectedItem.ToString() + " - " + tb1.Text : null;
            bool kontrol = tanim != null &&!string.IsNullOrWhiteSpace(tb1.Text) && !string.IsNullOrWhiteSpace(tb3.Text) && !string.IsNullOrWhiteSpace(tb4.Text) && !string.IsNullOrWhiteSpace(tb5.Text);
            if (kontrol)
            {
                //seçilen kullanıcıları arrayliste koyma
                for (int i = 0; i < KUL_dataGrid.Items.Count; i++)
                {
                    var item = KUL_dataGrid.Items[i];
                    var mycheckbox = KUL_dataGrid.Columns[0].GetCellContent(item) as CheckBox;
                    if (mycheckbox.IsChecked == true)
                    {
                        int x = 0;
                        foreach (DataGridColumn column in KUL_dataGrid.Columns)
                        {
                            if (x == 0) { x++; }//0 cb 1 USERNAME
                            else
                            {
                                TextBlock cellContent = column.GetCellContent(KUL_dataGrid.Items[i]) as TextBlock;
                                arraylist.Add(cellContent.Text);
                            }
                        }
                    }
                }

                //SMRTAPPKASA tablosu oluşturuluyor NOT:VARSA KASA NO UPDATE EDİLİYOR
                //c = new SqlCommand("IF EXISTS(SELECT * FROM SMRTAPPKASA WHERE KASANO = '" + tb1.Text + "') OR EXISTS(SELECT * FROM SMRTAPPKASA WHERE EVRAKNOTAKISI = '"+ tb4.Text +"') OR EXISTS(SELECT * FROM SMRTAPPKASA WHERE KASAHESAPKODU = '" + tb3.Text +"') BEGIN DELETE FROM SMRTAPPKASA WHERE KASANO = '" + tb1.Text + "' END INSERT INTO SMRTAPPKASA VALUES('" + tb1.Text + "','" + tanim + "','" + tb3.Text + "','" + tb4.Text + "','" + tb5.Text +"') ", s);
                //c.ExecuteNonQuery();
                c = new SqlCommand("IF EXISTS(SELECT * FROM SMRTAPPKASA WHERE KASANO = '" + tb1.Text + "') BEGIN DELETE FROM SMRTAPPKASA WHERE KASANO = '" + tb1.Text + "' END ", s);
                c.ExecuteNonQuery();
                //c = new SqlCommand("IF EXISTS(SELECT * FROM SMRTAPPKASA WHERE EVRAKNOTAKISI = '" + tb4.Text + "') BEGIN DELETE FROM SMRTAPPKASA WHERE EVRAKNOTAKISI = '" + tb4.Text + "' END ", s);
                //c.ExecuteNonQuery();
                //c = new SqlCommand("IF EXISTS(SELECT * FROM SMRTAPPKASA WHERE KASAHESAPKODU = '" + tb3.Text + "') BEGIN DELETE FROM SMRTAPPKASA WHERE KASAHESAPKODU = '" + tb3.Text + "' END ", s);
                //c.ExecuteNonQuery();
                c = new SqlCommand("INSERT INTO SMRTAPPKASA VALUES('" + tb1.Text + "','" + tanim + "','" + tb3.Text + "','" + tb4.Text + "','" + Convert.ToInt32(tb5.Text) + "')", s);
                c.ExecuteNonQuery();

                //Bu kasanın kasa nosu kaydediliyor.
                Carried.DosyaDecrypt();
                IniDosyaIslemleri.IniDosyaIslemleri.WriteINI("KasaNo", tb1.Text, System.AppDomain.CurrentDomain.BaseDirectory + "\\Baglanti.ini");
                Carried.DosyaEncrypt();

                //SMRTAPPKASAKULLANICI tablosu oluşturma, kullanıcı adları ve kasaları
                if (arraylist.Count != 0)
                {
                    for (int i = 0; i < arraylist.Count; i++)
                    {
                        c = new SqlCommand("IF NOT EXISTS (SELECT * FROM SMRTAPPKASAKULLANICI WHERE KULLANICIADI='" + arraylist[i] + "') BEGIN INSERT INTO SMRTAPPKASAKULLANICI VALUES('" + arraylist[i] + "','" + tb1.Text + "') END", s);
                        c.ExecuteNonQuery();
                    }
                }
                else Carried.showMessage("Seçim yapınız.");
            }
            else Carried.showMessage("Tüm veriler doldurulmalıdır.");
        }

        private void tb1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)//enter basınca eğer girilen kasa no mevcutsa bilgileri gelecek.
            {
                kasaBilgileriGetir();
            }
        }

        private void hesaptip_Click(object sender, RoutedEventArgs e)
        {
            //if (cb3.IsDropDownOpen == false)
            //{
                cb3.ItemsSource = null;
                try
                {
                    string tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
                    string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
                    using (s = new SqlConnection(constr))
                    {
                        s.Open();
                        SqlDataAdapter ProjectTableTableAdapter = new SqlDataAdapter("SELECT HESAPKOD FROM " + tabloadi + " where HESAPTIP=4", s);
                        DataSet ds = new DataSet();
                        ProjectTableTableAdapter.Fill(ds);

                        cb3.DisplayMemberPath = "HESAPKOD";
                        cb3.SelectedValuePath = "HESAPKOD";
                        cb3.ItemsSource = ds.Tables[0].DefaultView;
                        cb3.IsDropDownOpen = true;
                    }
                }
                catch (Exception ex)
                {
                    Carried.showMessage(ex.ToString());
                }
            //}
        }

        private void cb3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb3.SelectedItem != null)
            {
                tb3.Text = cb3.SelectedValue.ToString();
            }
        }

        private void kasaBilgileriGetir()
        {
            string tabloadi = Carried.IsCPMconnected == true ? "CARKRT" : "SMRTAPPCKRT";
            string constr = Carried.IsCPMconnected == true ? Carried.girisBaglantiCPM : Carried.girisBaglantiLocal;
            s = new SqlConnection(constr);
            s.Open();
            c = new SqlCommand("select count(kasano) from smrtappkasa where kasano='" + tb1.Text + "'",s);
            if ((int)c.ExecuteScalar() > 0)
            {
                tb2.Visibility = Visibility.Visible;
                tb2.Text = new SqlCommand("select KASATANIMI from smrtappkasa where kasano='" + tb1.Text + "'",s).ExecuteScalar().ToString();
                tb3.Text = new SqlCommand("select KASAHESAPKODU from smrtappkasa where kasano='" + tb1.Text + "'",s).ExecuteScalar().ToString();
                tb4.Text = new SqlCommand("select EVRAKNOTAKISI from smrtappkasa where kasano='" + tb1.Text + "'",s).ExecuteScalar().ToString();
                tb5.Text = new SqlCommand("select EVRAKNOSERINO from smrtappkasa where kasano='" + tb1.Text + "'",s).ExecuteScalar().ToString();
                int i = 0;
                foreach (DataRowView dr in KUL_dataGrid.ItemsSource)
                {
                    if (0 < (int)new SqlCommand("select count(KULLANICIADI) from SMRTAPPKASAKULLANICI where KULLANICIADI='" + dr[1].ToString() + "' AND KASANO='" + tb1.Text + "'",s).ExecuteScalar())
                    {
                        var item = KUL_dataGrid.Items[i];
                        var mycheckbox = KUL_dataGrid.Columns[0].GetCellContent(item) as CheckBox;
                        mycheckbox.IsChecked = true;
                    }
                    i++;
                }

            }
        }

        //public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        //{
        //    var itemsSource = grid.ItemsSource as IEnumerable;
        //    if (null == itemsSource) yield return null;
        //    foreach (var item in itemsSource)
        //    {
        //        var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
        //        if (null != row) yield return row;
        //    }
        //}



    }
}
