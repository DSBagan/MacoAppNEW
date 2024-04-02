using MacoApp;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TBMFurn
{
    /// <summary>
    /// Логика взаимодействия для WindowAntipanic.xaml
    /// </summary>
    public partial class WindowAntipanic : Window
    {
        ObservableCollection<ClassList> collection = new ObservableCollection<ClassList> ();

        public WindowAntipanic()
        {
            InitializeComponent();
        }

        private void ButtonAntipanic1_Click(object sender, RoutedEventArgs e)
        {
            ButtonAntipanic1.BorderBrush = Brushes.Red;
            ButtonAntipanic2.BorderBrush = Brushes.White;
            ButtonAntipanic3.BorderBrush = Brushes.White;
            ButtonAntipanic4.BorderBrush = Brushes.White;

            ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
            collection = new ObservableCollection<ClassList>();
            GridListAntipanic.ItemsSource = collection;
            collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт =1});
            collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 });
            collection.Add(new ClassList() { N = 3, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром", Шт = 1 });
        }

        private void ButtonAntipanic2_Click(object sender, RoutedEventArgs e)
        {
            ButtonAntipanic1.BorderBrush = Brushes.White;
            ButtonAntipanic2.BorderBrush = Brushes.Red;
            ButtonAntipanic3.BorderBrush = Brushes.White;
            ButtonAntipanic4.BorderBrush = Brushes.White;
            ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
            collection = new ObservableCollection<ClassList>();
            GridListAntipanic.ItemsSource = collection;
            collection.Add(new ClassList() { N = 1, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 });
            collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 });
            collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 });
            collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 });
        }

        private void ButtonAntipanic3_Click(object sender, RoutedEventArgs e)
        {
            ButtonAntipanic1.BorderBrush = Brushes.White;
            ButtonAntipanic2.BorderBrush = Brushes.White;
            ButtonAntipanic3.BorderBrush = Brushes.Red;
            ButtonAntipanic4.BorderBrush = Brushes.White;
            ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
            collection = new ObservableCollection<ClassList>();
            GridListAntipanic.ItemsSource = collection;
            collection.Add(new ClassList() { N = 1, Артикул = "ATP0504.06", Название = "Гарнитур нажимной, три точки запирания (горизонтальная и вертикальные)", Шт = 1 });
            collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 });
            collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 });
            collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 });
            
        }

        private void ButtonAntipanic4_Click(object sender, RoutedEventArgs e)
        {
            ButtonAntipanic1.BorderBrush = Brushes.White;
            ButtonAntipanic2.BorderBrush = Brushes.White;
            ButtonAntipanic3.BorderBrush = Brushes.White;
            ButtonAntipanic4.BorderBrush = Brushes.Red;
            ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
            collection = new ObservableCollection<ClassList>();
            GridListAntipanic.ItemsSource = collection;
            collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт = 1 });
            collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 2 });
            collection.Add(new ClassList() { N = 3, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 });
            collection.Add(new ClassList() { N = 4, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 });
            collection.Add(new ClassList() { N = 5, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 });
        }

        private int ComboBoxDoor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboboxDoor.SelectedIndex == 1)
            {
                collection.Add(new ClassList() { N = 5, Артикул = "ATP4718", Название = "Планка ответная для антипаники ПВХ и деревянных дверей", Шт = 1 });
            }
        }

                private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            EntryiWindow entryiWindow = new EntryiWindow();
            entryiWindow.Show();
            this.Close();
        }
    }
}
