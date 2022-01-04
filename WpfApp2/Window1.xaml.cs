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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Deneme : Window
    {
        public Deneme()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void tb_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string str = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Sales;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            int id;
            using (SqlConnection connection = new SqlConnection(str))
            {
                // Create a SqlCommand, and identify it as a stored procedure.
                using (SqlCommand sqlCommand = new SqlCommand("Sales.uspNewCustomer", connection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    // Add input parameter for the stored procedure and specify what to use as its value.
                    sqlCommand.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 40));
                    sqlCommand.Parameters["@CustomerName"].Value = tb.Text;

                    // Add the output parameter.
                    sqlCommand.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int));
                    sqlCommand.Parameters["@CustomerID"].Direction = ParameterDirection.Output;

                    try
                    {
                        connection.Open();

                        // Run the stored procedure.
                        sqlCommand.ExecuteNonQuery();

                        // Customer ID is an IDENTITY value from the database.
                        id = (int)sqlCommand.Parameters["@CustomerID"].Value;

                        // Put the Customer ID value into the read-only text box.
                        this.tb1.Text = Convert.ToString(id);
                    }
                    //catch
                    //{
                    //    MessageBox.Show("Customer ID was not returned. Account could not be created.");
                    //}
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}













//TabItem _tabUserPage;
//private void BtnUser1_Click(object sender, RoutedEventArgs e)
//{
//    MainTab.Items.Clear();    
//    var userControls = new UserControl1();
//    _tabUserPage = new TabItem { Content = userControls };
//    MainTab.Items.Add(_tabUserPage); // Add User Controls    
//    MainTab.Items.Refresh();
//}