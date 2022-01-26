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

namespace WpfApp2.Odeme
{
    /// <summary>
    /// Interaction logic for kredi.xaml
    /// </summary>
    public partial class kredi : Window
    {
        decimal toplamtutar;
        public kredi(decimal _toplamtutar)
        {
            InitializeComponent();
            this.toplamtutar = _toplamtutar;
        }
    }
}
