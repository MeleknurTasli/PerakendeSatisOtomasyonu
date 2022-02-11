using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
//using BespokeFusion;
using System.Security.AccessControl;
using System.Security.Principal;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for SatısIslemleri.xaml
    /// </summary>
    public partial class SatısIslemleri : Window
    {
        SqlConnection s;
        public SatısIslemleri()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void txtEvrakTarih_GotFocus(object sender, RoutedEventArgs e)
        {
            cb.IsDropDownOpen = true;
            cb.SelectedItem = null;
        }

        private void cb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (dp.IsFocused == false && dp.IsDropDownOpen == false) { cb.IsDropDownOpen = false; }
        }


        private void selected_date_changed(object sender, SelectionChangedEventArgs e)
        {
            txtEvrakTarih.Text = dp.SelectedDate.ToString();
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

        private void txtDovizCins_GotFocus(object sender, RoutedEventArgs e)
        {
            cb_dovizcins.Items.Clear();
            cb_dovizcins.Items.Add("USD");
            cb_dovizcins.Items.Add("EUR");
            cb_dovizcins.Items.Add("TL");
            cb_dovizcins.IsDropDownOpen = true;
        }

        private void cb_dovizcins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_dovizcins.SelectedItem != null) txtDovizCins.Text = cb_dovizcins.SelectedItem.ToString();
        }

        private void cb_evrakdovizcins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_evrakdovizcins.SelectedItem != null) txtEvrakDovizCins.Text = cb_evrakdovizcins.SelectedItem.ToString();
        }

        private void txtEvrakDovizCins_GotFocus(object sender, RoutedEventArgs e)
        {
            cb_evrakdovizcins.Items.Clear();
            cb_evrakdovizcins.Items.Add("USD");
            cb_evrakdovizcins.Items.Add("EUR");
            cb_evrakdovizcins.Items.Add("TL");
            cb_evrakdovizcins.IsDropDownOpen = true;
        }





        private void txtBrkd_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBrkd.Text))
            {
                if (txtBrkd.Text.StartsWith(" ")) { txtBrkd.Clear(); }
                string value = BarkodVSMalkodVSMalad();
                if (value != null)
                {
                    depoGetir(value);
                    stkkrtTabloGetir(value);
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
                SqlCommand c1 = new SqlCommand(cmdBarkodMu, s);
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
                SqlCommand c1 = new SqlCommand(cmdBarkodMu, s);
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
                string phrase = txtBrkd.Text.ToString();
                string[] words = phrase.Split(' ');
                string cmdMalAdlari = " select MALAD from STKKRT where MALAD like '%";
                for (int i = 0; i < words.Length; i++)
                {
                    cmdMalAdlari += words[i] + "%'";
                    if (i != words.Length - 1) { cmdMalAdlari += " and MALAD like '%"; }
                }
                SqlCommand c1 = new SqlCommand(cmdMalAdlari, s);
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
                else Carried.showMessage("Veri bulunamadı.");
            }
        }
        private void lbMalad_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtBrkd.Text = lbMalad.Items[lbMalad.SelectedIndex].ToString();
            Popup.IsOpen = false;
            lbMalad.Items.Clear();
        }
      
        
        private void depoGetir(string value)
        {
            listdepo.Items.Clear();
            string cmdDepoAdları = " select DEPKRT.DEPOAD from DEPKRT " +
               "inner join VW_STKDRM on VW_STKDRM.DEPOKOD=DEPKRT.DEPOKOD " +
               "where VW_STKDRM.MALKOD=(select malkod from stkkrt " + value + ")";
            SqlCommand c1 = new SqlCommand(cmdDepoAdları, s);
            SqlDataReader reader1 = c1.ExecuteReader();
            ArrayList arrlist = new ArrayList();
            if (reader1 != null)
            {
                try
                {
                    while (reader1.Read())
                    {
                        arrlist.Add(reader1["DEPOAD"].ToString());
                    }
                }
                finally { reader1.Close(); }
            }

            int intMiktar = 0;
            if (arrlist.Count != 0)
            {
                for (int i = 0; i < arrlist.Count; i++)
                {
                    string cmdMiktar3 = "select (STOKGIRIS - STOKCIKIS) FROM VW_STKDRM " +
                                           "inner join depkrt on VW_STKDRM.DEPOKOD = DEPKRT.DEPOKOD " +
                                           "where VW_STKDRM.MALKOD = (select malkod from stkkrt " + value  +
                                           " and DEPOAD = '" + arrlist[i] + "')";
                    c1 = new SqlCommand(cmdMiktar3, s);
                    object objMiktar3 = c1.ExecuteScalar();

                    StackPanel sp1 = new StackPanel() { Orientation = Orientation.Horizontal };
                    TextBlock lb1 = new TextBlock()
                    {
                        Width = gbdepo.ActualWidth - 90,
                        FontSize = 20,
                        FontFamily = new FontFamily("Arial"),
                        Foreground = Brushes.White,
                        Text = arrlist[i].ToString(),
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Left,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    TextBox tb1 = new TextBox()
                    {
                        Width = 40,
                        TextAlignment = TextAlignment.Center,
                        IsEnabled = false,
                        FontSize = 20,
                        TextWrapping = TextWrapping.Wrap,
                        FontFamily = new FontFamily("Arial"),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalContentAlignment = HorizontalAlignment.Center
                    };

                    if (objMiktar3 != null)
                    {
                        intMiktar = Convert.ToInt32(objMiktar3);
                        if (intMiktar > 0)
                        {
                            tb1.Text = intMiktar.ToString();
                            sp1.Children.Add(lb1);
                            sp1.Children.Add(tb1);
                            listdepo.Items.Add(sp1);
                        }
                    }
                }
            }
        }
        private void stkkrtTabloGetir(string value)
        {
            //sl = new SqlConnection(Carried.girisBaglantiLocal);
            //sl.Open();
            DataTable table = new DataTable();
            SqlDataAdapter a = new SqlDataAdapter("select * from stkkrt " + value, s);
            a.Fill(table);

            this.c_dataGrid.ItemsSource = table.DefaultView;

            SqlCommand c1 = new SqlCommand();
        }




        //Asagısı datagridi yan çevirmek için
        private void c_dataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new RotateTransform(90));
            foreach (DataGridColumn dataGridColumn in c_dataGrid.Columns)
            {
                if (dataGridColumn is DataGridTextColumn)
                {
                    DataGridTextColumn dataGridTextColumn = dataGridColumn as DataGridTextColumn;

                    Style style = new Style(dataGridTextColumn.ElementStyle.TargetType, dataGridTextColumn.ElementStyle.BasedOn);
                    style.Setters.Add(new Setter(TextBlock.MarginProperty, new Thickness(0, 2, 0, 2)));
                    style.Setters.Add(new Setter(TextBlock.LayoutTransformProperty, transformGroup));
                    style.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));

                    Style editingStyle = new Style(dataGridTextColumn.EditingElementStyle.TargetType, dataGridTextColumn.EditingElementStyle.BasedOn);
                    editingStyle.Setters.Add(new Setter(TextBox.MarginProperty, new Thickness(0, 2, 0, 2)));
                    editingStyle.Setters.Add(new Setter(TextBox.LayoutTransformProperty, transformGroup));
                    editingStyle.Setters.Add(new Setter(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Center));

                    dataGridTextColumn.ElementStyle = style;
                    dataGridTextColumn.EditingElementStyle = editingStyle;
                }
            }
            List<DataGridColumn> dataGridColumns = new List<DataGridColumn>();
            foreach (DataGridColumn dataGridColumn in c_dataGrid.Columns)
            {
                dataGridColumns.Add(dataGridColumn);
            }
            c_dataGrid.Columns.Clear();
            dataGridColumns.Reverse();
            foreach (DataGridColumn dataGridColumn in dataGridColumns)
            {
                c_dataGrid.Columns.Add(dataGridColumn);
            }
        }

        private void c_dataGridScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            // Add MouseWheel support for the datagrid scrollviewer.
            c_dataGrid.AddHandler(MouseWheelEvent, new RoutedEventHandler(DataGridMouseWheelHorizontal), true);
        }

        private void DataGridMouseWheelHorizontal(object sender, RoutedEventArgs e)
        {
            MouseWheelEventArgs eargs = (MouseWheelEventArgs)e;
            double x = (double)eargs.Delta;
            double y = c_dataGridScrollViewer.VerticalOffset;
            c_dataGridScrollViewer.ScrollToVerticalOffset(y - x);
        }






    }
}