using Google.Apis.Drive.v3.Data;
using MaterialDesignColors;
using MaterialDesignMessageBox;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using System.Runtime.Serialization;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Media.Animation;
using System.Drawing;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using System.Reflection.Emit;
using TBMFurn;
using MySql.Data.MySqlClient;
using System.Collections;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Key = System.Windows.Input.Key;
using MacoApp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;
using System.Reflection.Metadata;
using System.Diagnostics.Metrics;
using System.Windows.Documents;
using static Google.Protobuf.Reflection.FieldDescriptorProto.Types;
using System.Windows.Media.Effects;

namespace TBMFurn
{
    public partial class BoxCalculation : Window
    {
        DataTable table1 = new DataTable("Table1"); //Таблица для сохранения расчета
        DataTable table2 = new DataTable("Table2"); // Таблица для сохранения всех расчетов
        DataTable table3 = new DataTable("Table3");

        string Railing;
        private ObservableCollection<BitmapImage> backgroundsFONBox = new ObservableCollection<BitmapImage>();

        private bool isPanelExpanded = false;

        int Count = 1;

        string textBox1Value;
        string textBox2Value;
        string textBox3Value;
        string textBox4Value;
        string textBox5Value;
        string textBox6Value;
        string textBox7Value;
        string textBox8Value;

        public BoxCalculation()
        {
            InitializeComponent();

            SaveCalc.IsEnabled = false;
            ButtonSaveTxt.IsEnabled = false;

            table1.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table1.Columns.Add(new DataColumn("Название", typeof(string)));
            table1.Columns.Add(new DataColumn("Количество", typeof(int)));

            table2.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table2.Columns.Add(new DataColumn("Название", typeof(string)));
            table2.Columns.Add(new DataColumn("Количество", typeof(int)));

            table3.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table3.Columns.Add(new DataColumn("Название", typeof(string)));
            table3.Columns.Add(new DataColumn("Количество", typeof(int)));

            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Ящик под мойку Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик 135 Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик 199 Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик 135 Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик 199 Цвет.png")));

            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик белый.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик 135 Белый.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик 135 Квадрат.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик 135 Квадрат Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик 199 Белый.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик 199 Квадрат.png")));           
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Обычный ящик 199 Квадрат Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик Белый.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик 135 Квадрат.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик 135 Квадрат Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик 199 Квадрат.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик 199 Квадрат Цвет.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик 135 Белый.png")));
            backgroundsFONBox.Add(new BitmapImage(new Uri("pack://application:,,,/images/Внутренний ящик 199 Белый.png")));

            DataGridImage.HeadersVisibility = DataGridHeadersVisibility.None;

            LabelErrorType.Visibility = Visibility.Hidden;
            LabelErrorOpenClose.Visibility = Visibility.Hidden;
            LabelErrorHeight.Visibility = Visibility.Hidden;
            LabelErrorLenght.Visibility = Visibility.Hidden;
            LabelErrorRailing.Visibility = Visibility.Hidden;
            LabelErrorColor.Visibility = Visibility.Hidden;
            LabelErrorСode.Visibility = Visibility.Hidden;

            ButtonColor.Background = Brushes.White; // устанавливаем Цвет Label

            int Count = 1;
        }

        private void ButtonCalc_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxError();
            ButtonSaveTxt.IsEnabled = true;
            try
            {
                int count = 0;
                string queryString;
                string Type = ComboBoxType.Text;
                string Washing;
                if (Type == "Под мойку")
                {
                    Washing = "Да";
                }
                else
                {
                    Washing = "Нет";
                }
                string OpenClose = ComboBoxOpenClose.Text;
                if (OpenClose == "Soft Close")
                {
                    OpenClose = "SC";
                }
                else if (OpenClose == "Push to Open")
                {
                    OpenClose = "PTO";
                }
                string Inner_drawer;
                if (Type == "Внутренний")
                {
                    Inner_drawer = "Да";
                }
                else
                {
                    Inner_drawer = "Нет";
                }
                string Lenght = ComboBoxLenght.Text;
                string Height = ComboBoxHeight.Text;
                string Color = ComboBoxColor.Text;
                Railing = ComboBoxRailing.Text;
                int quantity = Int32.Parse(TextBoxColvo.Text);

                //queryString = $"Select * from BoxeFirmaxTAble";
                
                queryString = $"Select * from BoxeFirmaxTAble where (System like 'Newline') and(Type_of_opening  like 'Не имеет значения' or Type_of_opening  like '" + OpenClose + "') " +
                    "and(Length  like 'Не имеет значения' or Length like '" + Lenght + "') and(Height  like 'Не имеет значения' or Height like '" + Height + "')" + 
                    "and(Color  like 'Не имеет значения' or Color like '" + Color + "') and(Railing  like 'Не имеет значения' or Railing like '" + Railing + "')" +
                    "and(Inner_drawer  like 'Да/Нет' or Inner_drawer like '" + Inner_drawer + "') and(Washing  like 'Да/Нет' or Washing like '" + Washing + "')";


                using (var connection = new SqliteConnection("Data Source=Furnapp.db"))
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridListBox.ItemsSource = collection;
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(queryString, connection);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                count++;
                                
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(2).ToString(), Название = "" + reader.GetValue(1).ToString(), Шт = quantity });
                              
                            }
                        }
                    }
                    connection.Close();

                    // Скрываем кнопку "сохранить" если не выбран один из параметров
                    if (ComboBoxColor.SelectedItem == null|| ComboBoxType.SelectedItem == null || ComboBoxOpenClose.SelectedItem == null || ComboBoxHeight.SelectedItem == null || ComboBoxColor.SelectedItem == null || ComboBoxLenght.SelectedItem == null || (ComboBoxRailing.Visibility == Visibility.Visible && ComboBoxRailing.SelectedItem == null))
                    {
                        SaveCalc.IsEnabled = false;
                    }
                    else 
                    {
                        SaveCalc.IsEnabled = true;
                    }
                    //SaveCalc.IsEnabled = true;
                }
            }
            catch
            {
                MaterialMessageBox.ShowDialog("Что-то не так, я не знаю что...");
                return;
            }
        }

        private void SaveCalc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = 0;
                string queryString;
                string System = "Newline";
                string Type = ComboBoxType.Text;
                string Washing;
                if (Type == "Под мойку")
                {
                    Washing = "Да";
                }
                else
                {
                    Washing = "Нет";
                }
                string OpenClose = ComboBoxOpenClose.Text;
                if (OpenClose == "Soft Close")
                {
                    OpenClose = "SC";
                }
                else if (OpenClose == "Push to Open")
                {
                    OpenClose = "PTO";
                }
                string Inner_drawer;
                if (Type == "Внутренний")
                {
                    Inner_drawer = "Да";
                }
                else
                {
                    Inner_drawer = "Нет";
                }
                string Lenght = ComboBoxLenght.Text;
                string Height = ComboBoxHeight.Text;
                string Color = ComboBoxColor.Text;
                Railing = ComboBoxRailing.Text;
                int quantity = Int32.Parse(TextBoxColvo.Text);

                queryString = $"Select * from BoxeFirmaxTAble where (System like '" + System + "') and(Type_of_opening  like 'Не имеет значения' or Type_of_opening  like '" + OpenClose + "') " +
                    "and(Length  like 'Не имеет значения' or Length like '" + Lenght + "') and(Height  like 'Не имеет значения' or Height like '" + Height + "')" +
                    "and(Color  like 'Не имеет значения' or Color like '" + Color + "') and(Railing  like 'Не имеет значения' or Railing like '" + Railing + "')" +
                    "and(Inner_drawer  like 'Да/Нет' or Inner_drawer like '" + Inner_drawer + "') and(Washing  like 'Да/Нет' or Washing like '" + Washing + "')";


                using (var connection = new SqliteConnection("Data Source=Furnapp.db"))
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    table1.Rows.Clear();
                    collection = new ObservableCollection<ClassList>();
                    GridListBox.ItemsSource = collection;
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(queryString, connection);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                count++;

                                table1.Rows.Add(reader.GetValue(2).ToString(), reader.GetValue(1).ToString(), quantity);
                            }
                            // Получаем таблицы
                           SaveTable2();


                            LBListCalc.Items.Add("Newline. " + Type + ", " +
                                OpenClose + ", Глубина:" + Lenght + ", Высота:" + Height + ", " + Color+", Рейлинг: " + Railing);

                            // Создаем элементы управления для новой строки стекпанели
                            TextBlock textBlockSystem = new TextBlock();
                            TextBlock textBlockSystem_ = new TextBlock();
                            textBlockSystem_.Text = ". Тип: ";
                            TextBlock textBlockType = new TextBlock();
                            TextBlock textBlockType_ = new TextBlock();
                            textBlockType_.Text = ", Откр./закр.: ";
                            TextBlock textBlockOpenClose = new TextBlock();
                            TextBlock textBlockOpenClose_ = new TextBlock();
                            textBlockOpenClose_.Text = ", Размер: ";
                            TextBlock textBlockLenght = new TextBlock();
                            TextBlock textBlockLenght_ = new TextBlock();
                            textBlockLenght_.Text = "/";
                            TextBlock textBlockHeight = new TextBlock();
                            TextBlock textBlockHeight_ = new TextBlock();
                            textBlockHeight_.Text = ", Цвет: ";
                            TextBlock textBlockColor = new TextBlock();
                            TextBlock textBlockColor_ = new TextBlock();
                            textBlockColor_.Text = ", Рейлинг: ";
                            TextBlock textBlockRailing = new TextBlock();
                            TextBlock textBlockRailing_ = new TextBlock();
                            textBlockRailing_.Text = ". Шт: ";
                            TextBlock textBlockQuantity = new TextBlock();
                            TextBlock textBlockQuantity_ = new TextBlock();
                            textBlockQuantity_.Text = "   ";

                            Button deleteButton = new Button();
                            //deleteButton.Width = 10;
                            deleteButton.Height = 15;
                            deleteButton.Click += DeleteButton_Click;
                            deleteButton.Content = " Удалить  ";
                            Color paleRedColor = (Color)ColorConverter.ConvertFromString("#FFFFF5F5");
                            SolidColorBrush brush = new SolidColorBrush(paleRedColor);
                            deleteButton.Background = brush;
                            deleteButton.HorizontalContentAlignment = HorizontalAlignment.Left;
                            deleteButton.VerticalContentAlignment = VerticalAlignment.Top;
                            deleteButton.FontSize = 10;
                            deleteButton.Padding = new Thickness(0);
                            deleteButton.Foreground = Brushes.Black;

                            // Задаем значения для TextBlock элементов
                            textBlockSystem.Text = System;
                            textBlockType.Text = "" + Type;
                            textBlockOpenClose.Text = "" + OpenClose;
                            textBlockLenght.Text = Lenght;
                            textBlockHeight.Text = Height;
                            textBlockColor.Text = Color;
                            textBlockRailing.Text = Railing;
                            textBlockQuantity.Text = "" + quantity;

                            // Создаем новую строку с использованием созданных элементов
                            StackPanel newStackPanel = new StackPanel();
                            newStackPanel.Orientation = Orientation.Horizontal;
                            newStackPanel.HorizontalAlignment = HorizontalAlignment.Right;

                            newStackPanel.Children.Add(textBlockSystem);
                            newStackPanel.Children.Add(textBlockSystem_);
                            newStackPanel.Children.Add(textBlockType);
                            newStackPanel.Children.Add(textBlockType_);
                            newStackPanel.Children.Add(textBlockOpenClose);
                            newStackPanel.Children.Add(textBlockOpenClose_);
                            newStackPanel.Children.Add(textBlockLenght);
                            newStackPanel.Children.Add(textBlockLenght_);
                            newStackPanel.Children.Add(textBlockHeight);
                            newStackPanel.Children.Add(textBlockHeight_);
                            newStackPanel.Children.Add(textBlockColor);
                            newStackPanel.Children.Add(textBlockColor_);
                            newStackPanel.Children.Add(textBlockRailing);
                            newStackPanel.Children.Add(textBlockRailing_);
                            newStackPanel.Children.Add(textBlockQuantity);
                            newStackPanel.Children.Add(textBlockQuantity_);
                            newStackPanel.Children.Add(deleteButton);
                            // Добавляем новую строку в StackPanel
                            SPSF.Children.Add(newStackPanel);

                            //массив текстблоков для окрашивания
                            TextBlock[] textblockArray = new TextBlock[] { textBlockSystem, textBlockType, textBlockOpenClose, textBlockLenght, textBlockHeight, textBlockColor,
                                textBlockRailing, textBlockQuantity};
                            ColorTextblock(textblockArray);
                        }
                    }
                    connection.Close();
                    SaveCalc.IsEnabled = false;

                }
            }
            catch
            {
                MaterialMessageBox.ShowDialog("Что-то пошло не так...");
                return;
            }
        }

        //Окрашиваем Текстбокс
        public void ColorTextblock(TextBlock[] textblockArray)
        {
            foreach (var TextBlock in textblockArray)
            {
                // Получаем текст из текстблока
                string text = TextBlock.Text;


                /*
                // Устанавливаем цвет в зависимости от текста
                if (text == "Да")
                {
                    TextBlock.Foreground = Brushes.Green;
                }
                else if (text == "Нет")
                {
                    TextBlock.Foreground = Brushes.Red;
                }
                else
                {
                    TextBlock.Foreground = Brushes.Blue;
                }
                */

                TextBlock.Foreground = Brushes.Green;
            }
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем кнопку, на которую было нажато
            Button deleteButton = (Button)sender;
            // Получаем родительский StackPanel
            StackPanel parentStackPanel = (StackPanel)deleteButton.Parent;

            textBox1Value = ((TextBlock)parentStackPanel.Children[0]).Text;
            textBox2Value = ((TextBlock)parentStackPanel.Children[2]).Text;
            textBox3Value = ((TextBlock)parentStackPanel.Children[4]).Text;
            textBox4Value = ((TextBlock)parentStackPanel.Children[6]).Text;
            textBox5Value = ((TextBlock)parentStackPanel.Children[8]).Text;
            textBox6Value = ((TextBlock)parentStackPanel.Children[10]).Text;
            textBox7Value = ((TextBlock)parentStackPanel.Children[12]).Text;
            textBox8Value = ((TextBlock)parentStackPanel.Children[14]).Text;


            // Получаем значения TextBlock элементов в строке

            try
            {
                int count = 0;
                string queryString;
                string Type = textBox2Value;
                string Washing;
                if (Type == "Под мойку")
                {
                    Washing = "Да";
                }
                else
                {
                    Washing = "Нет";
                }
                string OpenClose = textBox3Value;
                if (OpenClose == "Soft Close")
                {
                    OpenClose = "SC";
                }
                else if (OpenClose == "Push to Open")
                {
                    OpenClose = "PTO";
                }
                string Inner_drawer;
                if (Type == "Внутренний")
                {
                    Inner_drawer = "Да";
                }
                else
                {
                    Inner_drawer = "Нет";
                }
                string Lenght = textBox4Value;
                string Height = textBox5Value;
                string Color = textBox6Value;
                Railing = textBox7Value;
                int quantity = Int32.Parse(textBox8Value);


                queryString = $"Select * from BoxeFirmaxTAble where (System like 'Newline') and(Type_of_opening  like 'Не имеет значения' or Type_of_opening  like '" + OpenClose + "') " +
                    "and(Length  like 'Не имеет значения' or Length like '" + Lenght + "') and(Height  like 'Не имеет значения' or Height like '" + Height + "')" +
                    "and(Color  like 'Не имеет значения' or Color like '" + Color + "') and(Railing  like 'Не имеет значения' or Railing like '" + Railing + "')" +
                    "and(Inner_drawer  like 'Да/Нет' or Inner_drawer like '" + Inner_drawer + "') and(Washing  like 'Да/Нет' or Washing like '" + Washing + "')";


                using (var connection = new SqliteConnection("Data Source=Furnapp.db"))
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    table1.Rows.Clear();
                    collection = new ObservableCollection<ClassList>();
                    GridListBox.ItemsSource = collection;
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(queryString, connection);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                count++;

                                table3.Rows.Add(reader.GetValue(2).ToString(), reader.GetValue(1).ToString(), quantity);

                            }
                            DeletedTable2();
                            table3.Clear();
                        }
                    }
                    connection.Close();
                }
                SaveCalc.IsEnabled = true;
            }
            catch
            {
                MaterialMessageBox.ShowDialog("При удалении возникла ошибка, не знаю какая, надо разбираться");
                return;
            }

            // Удаляем строку из StackPanel
            SPSF.Children.Remove(parentStackPanel);
        }



        public void SaveTable2() // Сохранение каждого нового расчета в таблицу 2 для дальнейшего сохранения
        {
            // Итерируем по строкам таблицы 1
            foreach (DataRow row1 in table1.Rows)
            {
                // Получаем значение первой колонки текущей строки
                string value1 = row1[0].ToString();

                // Итерируем по строкам таблицы 2
                DataRow row2 = table2.Rows.Cast<DataRow>().FirstOrDefault(r => r[0].ToString() == value1);

                // Если есть строка в таблице 2 с таким же значением первой колонки
                if (row2 != null)
                {
                    // Складываем значения третьих колонок
                    int sum = Convert.ToInt32(row1[2]) + Convert.ToInt32(row2[2]);

                    // Присваиваем второй таблице значение суммы 
                    row2[2] = sum;
                }
                else // Если нет строки в таблице 2
                {
                    // Добавляем новую строку в таблицу 2 со всеми значениями из таблицы 1
                    table2.Rows.Add(row1.ItemArray);
                }
            }
        }

        public void DeletedTable2() //Удаление расчета из таблицы 2 при удалении его из списка расчетов
        {
            foreach (DataRow row3 in table3.Rows)
            {
                // Получаем значение первой колонки текущей строки
                string value1 = row3[0].ToString();

                // Итерируем по строкам таблицы 2
                DataRow row2 = table2.Rows.Cast<DataRow>().FirstOrDefault(r => r[0].ToString() == value1);

                // Если есть строка в таблице 2 с таким же значением первой колонки
                if (row2 != null)
                {
                    // Отнимаем от второй таблицы значения количества третьей
                    int sum = Convert.ToInt32(row2[2]) - Convert.ToInt32(row3[2]);

                    // Если значение sum равно нулю, то удаляем строку из таблицы 2
                    if (sum == 0)
                    {
                        table2.Rows.Remove(row2);
                    }
                    else
                    {
                        // Присваиваем второй таблице значение суммы 
                        row2[2] = sum;
                    }
                }
            }
        }

        private void ButtonSaveTxt_Click(object sender, RoutedEventArgs e)
        {
            if (LBList.Items == null)
            {
                MaterialMessageBox.ShowDialog("Сначала произведите расчет, нечего сохранять.");
                return;
            }
            else
            {
                // Проверяем есть ли на диске C папка, если нет- создаем
                Directory.CreateDirectory(@"C:\aTBMFURN\");
                String date = DateTime.Now.ToString(" dd.MM.yyyy HH-mm-ss");
                int CTlangth = Code.Text.Length;
                if (CTlangth == 0)
                {
                    // Показываем изображение стрелки и запускаем анимацию
                    LabelErrorСode.Visibility = Visibility.Visible;
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    LabelErrorСode.BeginAnimation(UIElement.OpacityProperty, animation);
                    return;
                }
                else if (CTlangth < 6)
                {
                    for (int i = 0; i < 6 - CTlangth; i++)
                    {
                        Code.Text = "0" + Code.Text;
                    }
                }
                using (StreamWriter streamWriter = new StreamWriter(@"C:\aTBMFURN\" + "Z" + Code.Text + " " + date + ".txt", false, Encoding.Default))
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
                        foreach (DataRow row in table2.Rows)
                        {
                            string art = Convert.ToString(row["Артикул"]);
                            string nam = Convert.ToString(row["Название"]);
                            int qua = Convert.ToInt32(row["Количество"]);

                            if (art.Length < 16)
                            {
                                int b = 16 - art.Length;
                                for (int i = 0; i < b; i++)
                                {
                                    art += " ";
                                }
                            }
                            string n = "                                                ";
                            streamWriter.WriteLine(art + /*nam + "   " */n + qua);
                        }

                        streamWriter.WriteLine("--------------------------------------------------------------------------------");
                        streamWriter.WriteLine();
                        streamWriter.WriteLine("                    Заявку составил________________________");


                        streamWriter.Close();
                        table2.Rows.Clear();

                        MaterialMessageBox.ShowDialog("Файл успешно сохранен");
                        // если в TextBox есть символы
                        // Скрываем изображение стрелки
                        LabelErrorСode.Visibility = Visibility.Hidden;
                        LabelErrorСode.BeginAnimation(UIElement.OpacityProperty, null); // Остановка анимации
                    }
                    catch
                    {
                        MaterialMessageBox.ShowDialog("Ошибка при сохранении файла!");
                    }
                }
                LBListCalc.Items.Clear();
                LBList.Items.Clear();
                SPSF.Children.Clear();
                Count = 1;
            }
            ButtonSaveTxt.IsEnabled = false;
        }

        //Изменение размера списка расчетов
        private void ButtonSpisokName_Click(object sender, RoutedEventArgs e)
        {
            if (isPanelExpanded)
            {
                // если StackPanel уже раскрыт, то уменьшаем его размеры до исходных
                //SPSF.Width = 500; // возвращает ширину элемента к исходному значению
                SVSP.Height = 70;
                isPanelExpanded = false;
                ButtonSpisokName.Content = "▲";
            }
            else
            {
                // если StackPanel свернут, то увеличиваем его размеры на определенную величину
                //SPSF.Width = 600; // увеличиваем ширину
                isPanelExpanded = true;
                SVSP.Height = 400;

                ButtonSpisokName.Content = "▼";

            }
        }

        // Ниже ряд обработчиков Combobox
        private void ComboBoxOpenClose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxError();
        }
        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxError();
            //Выбираем фоном соответствующий ящик 
            int index = ComboBoxType.SelectedIndex;  
            if (index >= 0 && index < backgroundsFONBox.Count)
            {
                BitmapImage image = backgroundsFONBox[index];
                ImageBrush brush = new ImageBrush(image);
                DataGridImage.Background = brush;
            }
            if (ComboBoxType.SelectedIndex == 0)
            {
                ComboBoxOpenClose.Items.Clear();
                ComboBoxOpenClose.Items.Add("Soft Close");
                ComboBoxOpenClose.Items.Add("Push to Open");
                ComboBoxLenght.Items.Clear();
                ComboBoxLenght.Items.Add("300");
                ComboBoxLenght.Items.Add("350");
                ComboBoxLenght.Items.Add("400");
                ComboBoxLenght.Items.Add("450");
                ComboBoxLenght.Items.Add("500");
                ComboBoxColor.Items.Clear();
                ComboBoxColor.Items.Add("Серый");
                ComboBoxColor.Items.Add("Белый");


                if (ComboBoxHeight.SelectedIndex == 0)
                {
                    BitmapImage image = backgroundsFONBox[0];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxHeight.SelectedIndex == 1)
                {
                    BitmapImage image = backgroundsFONBox[3];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxHeight.SelectedIndex == 2)
                {
                    BitmapImage image = backgroundsFONBox[4];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
            }
            else if (ComboBoxType.SelectedIndex == 1)
            {
                ComboBoxOpenClose.Items.Clear();
                ComboBoxOpenClose.Items.Add("Soft Close");
                ComboBoxLenght.Items.Clear();
                ComboBoxLenght.Items.Add("300");
                ComboBoxLenght.Items.Add("350");
                ComboBoxLenght.Items.Add("400");
                ComboBoxLenght.Items.Add("450");
                ComboBoxLenght.Items.Add("500");
                ComboBoxColor.Items.Clear();
                ComboBoxColor.Items.Add("Серый");
                ComboBoxColor.Items.Add("Белый");

                if (ComboBoxHeight.SelectedIndex == 1)
                {
                    BitmapImage image = backgroundsFONBox[0];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxHeight.SelectedIndex == 1)
                {
                    BitmapImage image = backgroundsFONBox[5];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxHeight.SelectedIndex == 2)
                {
                    BitmapImage image = backgroundsFONBox[6];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
            }
            else if (ComboBoxType.SelectedIndex == 2)
            {
                ComboBoxOpenClose.Items.Clear();
                ComboBoxOpenClose.Items.Add("Soft Close");
                ComboBoxOpenClose.Items.Add("Push to Open");
                ComboBoxLenght.Items.Clear();
                ComboBoxLenght.Items.Add("450");
                ComboBoxLenght.Items.Add("500");
                ComboBoxColor.Items.Clear();
                ComboBoxColor.Items.Add("Серый");

                if (ComboBoxHeight.SelectedIndex == 0)
                {
                    BitmapImage image = backgroundsFONBox[2];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxHeight.SelectedIndex == 1)
                {
                    BitmapImage image = backgroundsFONBox[2];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxHeight.SelectedIndex == 2)
                {
                    BitmapImage image = backgroundsFONBox[2];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
            }
        }

        private void ComboBoxHeight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxError();
            ComboBoxItem selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
            string content = selectedItem.Content.ToString();
            TextBlockHeight.Text = content;
            if (ComboBoxHeight.SelectedIndex == 0)
            {
                ComboBoxRailing.Items.Clear();
                ComboBoxRailing.Visibility = Visibility.Collapsed;
                TextBlockRailing.Visibility = Visibility.Collapsed;
                LabelErrorRailing.Visibility = Visibility.Collapsed;
                Railing = "Нет";

                if (ComboBoxType.SelectedIndex == 0)
                {
                    BitmapImage image = backgroundsFONBox[0];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxType.SelectedIndex == 1)
                {
                    BitmapImage image = backgroundsFONBox[1];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxType.SelectedIndex == 2)
                {
                    BitmapImage image = backgroundsFONBox[2];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
            }
            else if (ComboBoxHeight.SelectedIndex == 1)
            {
                ComboBoxRailing.Items.Clear();
                ComboBoxRailing.Items.Add("Круглый");
                ComboBoxRailing.Items.Add("Квадратный");
                ComboBoxRailing.Visibility = Visibility.Visible;
                TextBlockRailing.Visibility = Visibility.Visible;

                if (ComboBoxType.SelectedIndex == 0)
                {
                    BitmapImage image = backgroundsFONBox[3];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxType.SelectedIndex == 1)
                {
                    BitmapImage image = backgroundsFONBox[5];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxType.SelectedIndex == 2)
                {
                    BitmapImage image = backgroundsFONBox[2];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }

            }
            else if (ComboBoxHeight.SelectedIndex == 2)
            {
                ComboBoxRailing.Items.Clear();
                ComboBoxRailing.Items.Add("Круглый");
                ComboBoxRailing.Items.Add("Квадратный");
                ComboBoxRailing.Visibility = Visibility.Visible;
                TextBlockRailing.Visibility = Visibility.Visible;

                if (ComboBoxType.SelectedIndex == 0)
                {
                    BitmapImage image = backgroundsFONBox[4];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxType.SelectedIndex == 1)
                {
                    BitmapImage image = backgroundsFONBox[6];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
                else if (ComboBoxType.SelectedIndex == 2)
                {
                    BitmapImage image = backgroundsFONBox[2];
                    ImageBrush brush = new ImageBrush(image);
                    DataGridImage.Background = brush;
                }
            }
        }
        // Обработка Комбобокса Цвет
        private void ComboBoxColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxError();
            if (ComboBoxColor.SelectedIndex != null) 
            {
                /*ComboBoxItem selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
                string content = selectedItem.Content.ToString();
                ButtonColor.Content = content;*/


                if (ComboBoxColor.SelectedIndex == 0)
                {
                    ButtonColor.Content = "Серый";
                    ButtonColor.Background = Brushes.LightGray; // устанавливаем цвет фона          
                }
                else if (ComboBoxColor.SelectedIndex == 1)
                {
                    ButtonColor.Content = "Белый";
                    ButtonColor.Background = Brushes.White;
                }
            }
                

            if (ComboBoxColor.SelectedIndex == 0) //Серый
            {
                if (ComboBoxType.SelectedIndex == 0) //Обычный
                {
                    if (ComboBoxHeight.SelectedIndex == 0) //84мм
                    {
                        BitmapImage image = backgroundsFONBox[0];
                        ImageBrush brush = new ImageBrush(image);
                        DataGridImage.Background = brush;
                    }
                    else if (ComboBoxHeight.SelectedIndex == 1) //135мм
                    {
                        if (ComboBoxRailing.SelectedIndex == 0) //Круглый рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[3];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxRailing.SelectedIndex == 1) //Квадратный рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[10];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                    else if (ComboBoxHeight.SelectedIndex == 2) //199мм
                    {
                        if (ComboBoxRailing.SelectedIndex == 0) //Круглый рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[4];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxRailing.SelectedIndex == 1) //Квадратный рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[13];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                }
                if (ComboBoxType.SelectedIndex == 1) //Внутренний
                {
                    if (ComboBoxHeight.SelectedIndex == 0) //84мм
                    {
                        BitmapImage image = backgroundsFONBox[1];
                        ImageBrush brush = new ImageBrush(image);
                        DataGridImage.Background = brush;
                    }
                    else if (ComboBoxHeight.SelectedIndex == 1) //135мм
                    {
                        if (ComboBoxRailing.SelectedIndex == 0) //Круглый рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[5];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxRailing.SelectedIndex == 1) //Квадратный рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[16];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                    else if (ComboBoxHeight.SelectedIndex == 2) //199мм
                    {
                        if (ComboBoxRailing.SelectedIndex == 0) //Круглый рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[6];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxRailing.SelectedIndex == 1) //Квадратный рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[18];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                }
            }
            /// Белый цвет*************************************************
            else if (ComboBoxColor.SelectedIndex == 1) //Белый
            {
                if (ComboBoxType.SelectedIndex == 0) //Обычный
                {
                    if (ComboBoxHeight.SelectedIndex == 0) //84мм
                    {
                        BitmapImage image = backgroundsFONBox[7];
                        ImageBrush brush = new ImageBrush(image);
                        DataGridImage.Background = brush;
                        Railing = "Нет";
                    }
                    else if (ComboBoxHeight.SelectedIndex == 1) //135мм
                    {
                        if (ComboBoxRailing.SelectedIndex == 0) //Круглый рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[8];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxRailing.SelectedIndex == 1) //Квадратный рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[9];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                    else if (ComboBoxHeight.SelectedIndex == 2) //199мм
                    {
                        if (ComboBoxRailing.SelectedIndex == 0) //Круглый рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[11];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxRailing.SelectedIndex == 1) //Квадратный рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[12];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                }
                if (ComboBoxType.SelectedIndex == 1) //Внутренний
                {
                    if (ComboBoxHeight.SelectedIndex == 0) //84мм
                    {
                        BitmapImage image = backgroundsFONBox[14];
                        ImageBrush brush = new ImageBrush(image);
                        DataGridImage.Background = brush;
                        Railing = "Нет";
                    }
                    else if (ComboBoxHeight.SelectedIndex == 1) //135мм
                    {
                        if (ComboBoxRailing.SelectedIndex == 0) //Круглый рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[19];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxRailing.SelectedIndex == 1) //Квадратный рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[15];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                    else if (ComboBoxHeight.SelectedIndex == 2) //199мм
                    {
                        if (ComboBoxRailing.SelectedIndex == 0) //Круглый рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[20];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxRailing.SelectedIndex == 1) //Квадратный рейлинг
                        {
                            BitmapImage image = backgroundsFONBox[17];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                }
            }
        }
        //Обработка комбобокс рейлинг
        private void ComboBoxRailing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxError();
            if (ComboBoxRailing.SelectedIndex == 0) //Круглый рейлинг
            {
                if (ComboBoxType.SelectedIndex == 0) //Обычный
                {
                    if (ComboBoxHeight.SelectedIndex == 1) //135мм
                    {
                        if (ComboBoxColor.SelectedIndex == 0) //Серый
                        {
                            BitmapImage image = backgroundsFONBox[3];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxColor.SelectedIndex == 1) //Белый
                        {
                            BitmapImage image = backgroundsFONBox[8];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                    else if (ComboBoxHeight.SelectedIndex == 2) //199мм
                    {
                        if (ComboBoxColor.SelectedIndex == 0) //Серый
                        {
                            BitmapImage image = backgroundsFONBox[4];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxColor.SelectedIndex == 1) //Белый
                        {
                            BitmapImage image = backgroundsFONBox[11];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                }
                if (ComboBoxType.SelectedIndex == 1) //Внутренний
                {
                    if (ComboBoxHeight.SelectedIndex == 1) //135мм
                    {
                        if (ComboBoxColor.SelectedIndex == 0) //Серый
                        {
                            BitmapImage image = backgroundsFONBox[5];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxColor.SelectedIndex == 1) //Белый
                        {
                            BitmapImage image = backgroundsFONBox[19];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                    else if (ComboBoxHeight.SelectedIndex == 2) //199мм
                    {
                        if (ComboBoxColor.SelectedIndex == 0) //Серый
                        {
                            BitmapImage image = backgroundsFONBox[6];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxColor.SelectedIndex == 1) //Белый
                        {
                            BitmapImage image = backgroundsFONBox[20];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                }
            }
            /// Квадратный рейлинг*************************************************
            else if (ComboBoxRailing.SelectedIndex == 1) //
            {
                if (ComboBoxType.SelectedIndex == 0) //Обычный
                {
                    if (ComboBoxHeight.SelectedIndex == 1) //135мм
                    {
                        if (ComboBoxColor.SelectedIndex == 0) //Серый
                        {
                            BitmapImage image = backgroundsFONBox[10];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxColor.SelectedIndex == 1) //Белый
                        {
                            BitmapImage image = backgroundsFONBox[9];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                    else if (ComboBoxHeight.SelectedIndex == 2) //199мм
                    {
                        if (ComboBoxColor.SelectedIndex == 0) //Серый
                        {
                            BitmapImage image = backgroundsFONBox[13];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxColor.SelectedIndex == 1) //Белый
                        {
                            BitmapImage image = backgroundsFONBox[12];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                }
                if (ComboBoxType.SelectedIndex == 1) //Внутренний
                {
                    if (ComboBoxHeight.SelectedIndex == 1) //135мм
                    {
                        if (ComboBoxColor.SelectedIndex == 0) //Серый
                        {
                            BitmapImage image = backgroundsFONBox[16];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxColor.SelectedIndex == 1) //Белый
                        {
                            BitmapImage image = backgroundsFONBox[15];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                    else if (ComboBoxHeight.SelectedIndex == 2) //199мм
                    {
                        if (ComboBoxColor.SelectedIndex == 0) //Серый
                        {
                            BitmapImage image = backgroundsFONBox[18];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                        else if (ComboBoxColor.SelectedIndex == 1) //Белый
                        {
                            BitmapImage image = backgroundsFONBox[17];
                            ImageBrush brush = new ImageBrush(image);
                            DataGridImage.Background = brush;
                        }
                    }
                }
            }
        }

        private void ComboBoxLenght_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxError();
            ComboBox comboBox = sender as ComboBox;

            if (comboBox.SelectedItem != null)
            {
                // Получаем выбранное значение
                string selectedValue = comboBox.SelectedItem.ToString();

                // Тут можно транслировать значение или выполнять другие действия
                TextBlockLenght.Text = selectedValue;
            }
        }

        //Обработчик пустых Комбобокс********************************
        private void ComboBoxError()
        {
            if (ComboBoxColor.SelectedItem == null)
            {
                // Показываем изображение стрелки и запускаем анимацию
                LabelErrorColor.Visibility = Visibility.Visible;
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                LabelErrorColor.BeginAnimation(UIElement.OpacityProperty, animation);
            }
            else
            {
                // Скрываем изображение стрелки
                LabelErrorColor.Visibility = Visibility.Hidden;
                LabelErrorColor.BeginAnimation(UIElement.OpacityProperty, null); // Остановка анимации
            }

            if (ComboBoxType.SelectedItem == null)
            {
                // Показываем изображение стрелки и запускаем анимацию
                LabelErrorType.Visibility = Visibility.Visible;
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                LabelErrorType.BeginAnimation(UIElement.OpacityProperty, animation);
            }
            else
            {
                // Скрываем изображение стрелки
                LabelErrorType.Visibility = Visibility.Hidden;
                LabelErrorType.BeginAnimation(UIElement.OpacityProperty, null); // Остановка анимации
            }
            if (ComboBoxOpenClose.SelectedItem == null)
            {
                // Показываем изображение стрелки и запускаем анимацию
                LabelErrorOpenClose.Visibility = Visibility.Visible;
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                LabelErrorOpenClose.BeginAnimation(UIElement.OpacityProperty, animation);
            }
            else
            {
                // Скрываем изображение стрелки
                LabelErrorOpenClose.Visibility = Visibility.Hidden;
                LabelErrorOpenClose.BeginAnimation(UIElement.OpacityProperty, null); // Остановка анимации
            }
            if (ComboBoxHeight.SelectedItem == null)
            {
                // Показываем изображение стрелки и запускаем анимацию
                LabelErrorHeight.Visibility = Visibility.Visible;
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                LabelErrorHeight.BeginAnimation(UIElement.OpacityProperty, animation);
                
            }
            else
            {
                // Скрываем изображение стрелки
                LabelErrorHeight.Visibility = Visibility.Hidden;
                LabelErrorHeight.BeginAnimation(UIElement.OpacityProperty, null); // Остановка анимации
            }
            if (ComboBoxLenght.SelectedItem == null)
            {
                // Показываем изображение стрелки и запускаем анимацию
                LabelErrorLenght.Visibility = Visibility.Visible;
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                LabelErrorLenght.BeginAnimation(UIElement.OpacityProperty, animation);
            }
            else
            {
                // Скрываем изображение стрелки
                LabelErrorLenght.Visibility = Visibility.Hidden;
                LabelErrorLenght.BeginAnimation(UIElement.OpacityProperty, null); // Остановка анимации
            }

            if (ComboBoxHeight.SelectedIndex == 0)
            {
                ComboBoxRailing.Items.Clear();
                ComboBoxRailing.Visibility = Visibility.Collapsed;
                TextBlockRailing.Visibility = Visibility.Collapsed;
                LabelErrorRailing.Visibility = Visibility.Collapsed;
                LabelErrorRailing.BeginAnimation(UIElement.OpacityProperty, null);
                Railing = "Нет";
            }
            else 
            {
                if (ComboBoxRailing.SelectedItem == null)
                {
                    // Показываем изображение стрелки и запускаем анимацию
                    LabelErrorRailing.Visibility = Visibility.Visible;
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        From = 1,
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    LabelErrorRailing.BeginAnimation(UIElement.OpacityProperty, animation);
                }
                else
                {
                    // Скрываем изображение стрелки
                    LabelErrorRailing.Visibility = Visibility.Hidden;
                    LabelErrorRailing.BeginAnimation(UIElement.OpacityProperty, null); // Остановка анимации
                }
            }
            
        }

        //ввод только цифр в текстбоксы****************************************
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //SaveCalc.IsEnabled = false;
            int val;
            if (!Int32.TryParse(e.Text, out val) && e.Text != "-")
            {
                e.Handled = true; // отклоняем ввод
            }
        }
        //ввод только цифр в текстбоксы (пробел тоже нам не нужен)**********************************
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
