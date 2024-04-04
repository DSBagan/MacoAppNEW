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
    public partial class WindowAntipanic : Window
    {
        int NumberButton;
        //int Quantity;

        public WindowAntipanic()
        {
            InitializeComponent();
            
        }

        private void ButtonAntipanic()
        {
            if (!string.IsNullOrEmpty(TextBoxQuantity.Text) && TextBoxQuantity.Text.All(char.IsDigit))
            {
                int Quantity = Int32.Parse(TextBoxQuantity.Text);
            }
            else
            {
                return;
            }

            if (ComboboxDoor.SelectedIndex == 0)
            {
                if (NumberButton == 1)
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridListAntipanic.ItemsSource = collection;
                    collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт = (1 * Quantity) });
                    collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 3, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром", Шт = 1 * Quantity });
                }
                else if (NumberButton == 2)
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridListAntipanic.ItemsSource = collection;
                    collection.Add(new ClassList() { N = 1, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                }
                else if (NumberButton == 3)
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridListAntipanic.ItemsSource = collection;
                    collection.Add(new ClassList() { N = 1, Артикул = "ATP0504.06", Название = "Гарнитур нажимной, три точки запирания (горизонтальная и вертикальные)", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                }
                else if (NumberButton == 4)
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridListAntipanic.ItemsSource = collection;
                    collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 2 * Quantity });
                    collection.Add(new ClassList() { N = 3, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 4, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 5, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                }
            }
            if (ComboboxDoor.SelectedIndex == 1)
            {
                if (NumberButton == 1)
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridListAntipanic.ItemsSource = collection;
                    collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 3, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 4, Артикул = "ATP4718", Название = "Планка ответная для антипаники ПВХ и деревянных дверей", Шт = 1 * Quantity });
                }
                else if (NumberButton == 2)
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridListAntipanic.ItemsSource = collection;
                    collection.Add(new ClassList() { N = 1, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 5, Артикул = "ATP4718", Название = "Планка ответная для антипаники ПВХ и деревянных дверей", Шт = 1 * Quantity });
                }
                else if (NumberButton == 3)
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridListAntipanic.ItemsSource = collection;
                    collection.Add(new ClassList() { N = 1, Артикул = "ATP0504.06", Название = "Гарнитур нажимной, три точки запирания (горизонтальная и вертикальные)", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 5, Артикул = "ATP4718", Название = "Планка ответная для антипаники ПВХ и деревянных дверей", Шт = 1 * Quantity });
                }
                else if (NumberButton == 4)
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridListAntipanic.ItemsSource = collection;
                    collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 2 * Quantity });
                    collection.Add(new ClassList() { N = 3, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 4, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 5, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                    collection.Add(new ClassList() { N = 6, Артикул = "ATP4718", Название = "Планка ответная для антипаники ПВХ и деревянных дверей", Шт = 1 * Quantity });
                }
            }
        }

        public void ButtonAntipanic1_Click(object sender, RoutedEventArgs e)
        {
            ButtonAntipanic1.BorderBrush = Brushes.Red;
            ButtonAntipanic2.BorderBrush = Brushes.White;
            ButtonAntipanic3.BorderBrush = Brushes.White;
            ButtonAntipanic4.BorderBrush = Brushes.White;

            NumberButton = 1;
            ButtonAntipanic();
        }

        public void ButtonAntipanic2_Click(object sender, RoutedEventArgs e)
        {
            ButtonAntipanic1.BorderBrush = Brushes.White;
            ButtonAntipanic2.BorderBrush = Brushes.Red;
            ButtonAntipanic3.BorderBrush = Brushes.White;
            ButtonAntipanic4.BorderBrush = Brushes.White;

            NumberButton = 2;
            ButtonAntipanic();
        }

        public void ButtonAntipanic3_Click(object sender, RoutedEventArgs e)
        {
            ButtonAntipanic1.BorderBrush = Brushes.White;
            ButtonAntipanic2.BorderBrush = Brushes.White;
            ButtonAntipanic3.BorderBrush = Brushes.Red;
            ButtonAntipanic4.BorderBrush = Brushes.White;

            NumberButton = 3;
            ButtonAntipanic();
        }

        public void ButtonAntipanic4_Click(object sender, RoutedEventArgs e)
        {
            ButtonAntipanic1.BorderBrush = Brushes.White;
            ButtonAntipanic2.BorderBrush = Brushes.White;
            ButtonAntipanic3.BorderBrush = Brushes.White;
            ButtonAntipanic4.BorderBrush = Brushes.Red;

            NumberButton = 4;
            ButtonAntipanic();
        }

        private void ComboBoxDoor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboboxDoor.SelectedIndex == 0)
            {
                ButtonAntipanic();
            }
            else if(ComboboxDoor.SelectedIndex == 1) 
            {
                ButtonAntipanic();
            }
                
        }

        //ввод только цифр в текстбоксы
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val) && e.Text != "-")
            {
                e.Handled = true; // отклоняем ввод
            }
            
        }
        //ввод только цифр в текстбоксы (пробел тоже нам не нужен)
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true; // если пробел, отклоняем ввод
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