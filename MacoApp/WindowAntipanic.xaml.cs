using MacoApp;
using MaterialDesignMessageBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
using System.Windows.Markup;

namespace TBMFurn
{
    public partial class WindowAntipanic : Window
    {
        int NumberButton;
        int Quantity;
        DataTable table1 = new DataTable("Table1"); //Таблица для сохранения расчета
        string SavePathTXT = "";

        // Добавляем флаг для отслеживания состояния очистки
        private bool _isCleaningUp = false;

        // Сохраняем ссылки на анимации для остановки
        private DoubleAnimation _errorAnimation;

        public WindowAntipanic()
        {
            InitializeComponent();
            InitializeDataTable();

            LabelErrorСode.Visibility = Visibility.Hidden;
            ButtonSaveTxt.IsEnabled = false;

            // Подписываемся на событие закрытия окна
            this.Closed += OnWindowClosed;
        }

        private void InitializeDataTable()
        {
            table1.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table1.Columns.Add(new DataColumn("Название", typeof(string)));
            table1.Columns.Add(new DataColumn("Шт", typeof(int)));
        }

        private void ButtonAntipanic()
        {
            if (ComboboxDoor == null) return;

            try
            {
                if (TextBoxQuantity != null && !string.IsNullOrEmpty(TextBoxQuantity.Text))
                {
                    Quantity = Int32.Parse(TextBoxQuantity.Text);
                }
                else
                {
                    Quantity = 1;
                }

                if (ComboboxDoor.SelectedIndex == 0)
                {
                    if (NumberButton == 1)
                    {
                        ButtonSaveTxt.IsEnabled = true;
                        table1.Rows.Clear();
                        ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                        collection = new ObservableCollection<ClassList>();
                        GridListAntipanic.ItemsSource = collection;
                        collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт = (1 * Quantity) });
                        collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 3, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром", Шт = 1 * Quantity });
                        CopyCollectionToDataTable(collection);
                    }
                    else if (NumberButton == 2)
                    {
                        ButtonSaveTxt.IsEnabled = true;
                        table1.Rows.Clear();
                        ObservableCollection<ClassList> collection = null;
                        collection = new ObservableCollection<ClassList>();
                        GridListAntipanic.ItemsSource = collection;
                        collection.Add(new ClassList() { N = 1, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                        CopyCollectionToDataTable(collection);
                    }
                    else if (NumberButton == 3)
                    {
                        ButtonSaveTxt.IsEnabled = true;
                        table1.Rows.Clear();
                        ObservableCollection<ClassList> collection = null;
                        collection = new ObservableCollection<ClassList>();
                        GridListAntipanic.ItemsSource = collection;
                        collection.Add(new ClassList() { N = 1, Артикул = "ATP0504.06", Название = "Гарнитур нажимной, три точки запирания (горизонтальная и вертикальные)", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                        CopyCollectionToDataTable(collection);
                    }
                    else if (NumberButton == 4)
                    {
                        ButtonSaveTxt.IsEnabled = true;
                        table1.Rows.Clear();
                        ObservableCollection<ClassList> collection = null;
                        collection = new ObservableCollection<ClassList>();
                        GridListAntipanic.ItemsSource = collection;
                        collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 2 * Quantity });
                        collection.Add(new ClassList() { N = 3, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 4, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 5, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                        CopyCollectionToDataTable(collection);
                    }
                }
                else if (ComboboxDoor.SelectedIndex == 1)
                {
                    if (NumberButton == 1)
                    {
                        ButtonSaveTxt.IsEnabled = true;
                        table1.Rows.Clear();
                        ObservableCollection<ClassList> collection = null;
                        collection = new ObservableCollection<ClassList>();
                        GridListAntipanic.ItemsSource = collection;
                        collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 3, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 4, Артикул = "ATP4718", Название = "Планка ответная для антипаники ПВХ и деревянных дверей", Шт = 1 * Quantity });
                        CopyCollectionToDataTable(collection);
                    }
                    else if (NumberButton == 2)
                    {
                        ButtonSaveTxt.IsEnabled = true;
                        table1.Rows.Clear();
                        ObservableCollection<ClassList> collection = null;
                        collection = new ObservableCollection<ClassList>();
                        GridListAntipanic.ItemsSource = collection;
                        collection.Add(new ClassList() { N = 1, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 5, Артикул = "ATP4718", Название = "Планка ответная для антипаники ПВХ и деревянных дверей", Шт = 1 * Quantity });
                        CopyCollectionToDataTable(collection);
                    }
                    else if (NumberButton == 3)
                    {
                        ButtonSaveTxt.IsEnabled = true;
                        table1.Rows.Clear();
                        ObservableCollection<ClassList> collection = null;
                        collection = new ObservableCollection<ClassList>();
                        GridListAntipanic.ItemsSource = collection;
                        collection.Add(new ClassList() { N = 1, Артикул = "ATP0504.06", Название = "Гарнитур нажимной, три точки запирания (горизонтальная и вертикальные)", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 3, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 4, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 5, Артикул = "ATP4718", Название = "Планка ответная для антипаники ПВХ и деревянных дверей", Шт = 1 * Quantity });
                        CopyCollectionToDataTable(collection);
                    }
                    else if (NumberButton == 4)
                    {
                        ButtonSaveTxt.IsEnabled = true;
                        table1.Rows.Clear();
                        ObservableCollection<ClassList> collection = null;
                        collection = new ObservableCollection<ClassList>();
                        GridListAntipanic.ItemsSource = collection;
                        collection.Add(new ClassList() { N = 1, Артикул = "ATP0500.06", Название = "Гарнитур нажимной, одна точка запирания по горизонту", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 2, Артикул = "ATP0501.6029", Название = "Перекладина для антипаниковой ручки 1150 мм, зелен.", Шт = 2 * Quantity });
                        collection.Add(new ClassList() { N = 3, Артикул = "ATP0502.06", Название = "Гарнитур нажимной, две точки запирания по вертикали.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 4, Артикул = "ATP0503.06", Название = "Запоры вертикальные 2 штуки, высотой до 2640 мм", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 5, Артикул = "ATP0505.06", Название = "Ручка наружная для антипаники с профильным цилиндром, черн.", Шт = 1 * Quantity });
                        collection.Add(new ClassList() { N = 6, Артикул = "ATP4718", Название = "Планка ответная для антипаники ПВХ и деревянных дверей", Шт = 1 * Quantity });
                        CopyCollectionToDataTable(collection);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в ButtonAntipanic: {ex.Message}");
            }
        }

        public DataTable CopyCollectionToDataTable(ObservableCollection<ClassList> collection)
        {
            // Копирование данных из коллекции в DataTable
            foreach (ClassList item in collection)
            {
                DataRow row = table1.NewRow();
                row["Артикул"] = item.Артикул;
                row["Название"] = item.Название;
                row["Шт"] = item.Шт;
                table1.Rows.Add(row);
            }

            // Возврат DataTable
            return table1;
        }

        private void ButtonSaveTxt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем есть диск X и папка aTBMFURN, если нет- создаем
                Directory.CreateDirectory(@"X:\Подгрузка в КИС");
                SavePathTXT = @"X:\Подгрузка в КИС";
            }
            catch (System.Exception)
            {
                // Проверяем есть ли на диске C папка aTBMFURN, если нет- создаем
                Directory.CreateDirectory(@"C:\Подгрузка в КИС");
                MessageBox.Show("Нет доступа к диску X, файл будет сохранен в папку Подгрузка в КИС на диске C");
                SavePathTXT = @"C:\Подгрузка в КИС";
            }

            String date = DateTime.Now.ToString(" dd.MM.yyyy HH-mm-ss");

            int CTlangth = Code.Text.Length;
            if (CTlangth == 0)
            {
                // Показываем изображение стрелки и запускаем анимацию
                LabelErrorСode.Visibility = Visibility.Visible;
                _errorAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                LabelErrorСode.BeginAnimation(UIElement.OpacityProperty, _errorAnimation);
                return;
            }
            else if (CTlangth < 6)
            {
                for (int i = 0; i < 6 - CTlangth; i++)
                {
                    Code.Text = "0" + Code.Text;
                }
            }

            using (StreamWriter streamWriter = new StreamWriter(SavePathTXT + "Z" + Code.Text + " " + date + " Антипаника" + ".txt", false, Encoding.Default))
            {
                streamWriter.WriteLine("                    Шифр фирмы " + Code.Text);
                streamWriter.WriteLine("                    Фирма 123");
                streamWriter.WriteLine("                    Заявка №");
                streamWriter.WriteLine("                    Название");
                streamWriter.WriteLine("                    Дата заявки" + date);
                streamWriter.WriteLine("--------------------------------------------------------------------------------");
                streamWriter.WriteLine("    Артикул                       Название                      Кол.  Ед.изм.");
                streamWriter.WriteLine("--------------------------------------------------------------------------------");
                try
                {
                    foreach (DataRow row in table1.Rows)
                    {
                        string art = Convert.ToString(row["Артикул"]);
                        string nam = Convert.ToString(row["Название"]);
                        int qua = Convert.ToInt32(row["Шт"]);

                        if (art.Length < 16)
                        {
                            int b = 16 - art.Length;
                            for (int i = 0; i < b; i++)
                            {
                                art += " ";
                            }
                        }
                        string n = "                                                ";
                        streamWriter.WriteLine(art + n + qua);
                    }

                    streamWriter.WriteLine("--------------------------------------------------------------------------------");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("                    Заявку составил________________________");

                    streamWriter.Close();

                    MaterialMessageBox.ShowDialog("Файл успешно сохранен");

                    // Скрываем изображение стрелки
                    LabelErrorСode.Visibility = Visibility.Hidden;
                    LabelErrorСode.BeginAnimation(UIElement.OpacityProperty, null);

                    // Очищаем таблицу после сохранения
                    table1.Rows.Clear();
                    GridListAntipanic.ItemsSource = null;
                    ButtonSaveTxt.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    MaterialMessageBox.ShowDialog("Ошибка при сохранении файла!");
                    System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
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
            ButtonAntipanic();
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
            this.Close();
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            if (_isCleaningUp) return;
            _isCleaningUp = true;

            try
            {
                // Останавливаем анимацию ошибки
                if (_errorAnimation != null)
                {
                    LabelErrorСode.BeginAnimation(UIElement.OpacityProperty, null);
                    _errorAnimation = null;
                }

                // Отписываемся от событий
                this.Closed -= OnWindowClosed;

                // Отписываемся от событий кнопок
                ButtonAntipanic1.Click -= ButtonAntipanic1_Click;
                ButtonAntipanic2.Click -= ButtonAntipanic2_Click;
                ButtonAntipanic3.Click -= ButtonAntipanic3_Click;
                ButtonAntipanic4.Click -= ButtonAntipanic4_Click;
                ButtonSaveTxt.Click -= ButtonSaveTxt_Click;
                ButtonExit.Click -= ButtonExit_Click;

                // Отписываемся от событий ComboBox
                ComboboxDoor.SelectionChanged -= ComboBoxDoor_SelectionChanged;

                // Отписываемся от событий TextBox
                if (TextBoxQuantity != null)
                {
                    TextBoxQuantity.PreviewTextInput -= TextBox_PreviewTextInput;
                    TextBoxQuantity.PreviewKeyDown -= TextBox_PreviewKeyDown;
                }
                Code.PreviewTextInput -= TextBox_PreviewTextInput;
                Code.PreviewKeyDown -= TextBox_PreviewKeyDown;

                // Очищаем таблицу
                if (table1 != null)
                {
                    table1.Clear();
                    table1.Dispose();
                }

                // Очищаем GridList
                GridListAntipanic.ItemsSource = null;

                // Вызываем GC для принудительной сборки мусора
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при очистке ресурсов WindowAntipanic: {ex.Message}");
            }
        }
    }
}