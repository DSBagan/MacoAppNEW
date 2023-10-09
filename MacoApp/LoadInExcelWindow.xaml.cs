using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TBMFurn
{
    /// <summary>
    /// Логика взаимодействия для LoadInExcelWindow.xaml
    /// </summary>
    public partial class LoadInExcelWindow : Window
    {
        List<ExcelEnter> excelEnter = new List<ExcelEnter>();
        public LoadInExcelWindow()
        {
            InitializeComponent();

        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
