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

namespace TBMFurn
{
    public partial class BoxCalculation : Window
    {
        DataTable table1 = new DataTable("Table1"); //Таблица для сохранения расчета
        DataTable table2 = new DataTable("Table2"); // Таблица для сохранения всех расчетов
        DataTable table3 = new DataTable("Table3");

        string LenghtText;
        string Railing;
        private ObservableCollection<BitmapImage> backgroundsFONBox = new ObservableCollection<BitmapImage>();

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
                              
                                /*else
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) });
                                }*/
                            }
                        }
                    }
                    connection.Close();
                }
            }
            catch
            {
                //MaterialMessageBox.ShowDialog("Одно или несколько полей пустое");
                return;
            }
        }

        private void SaveCalc_Click(object sender, RoutedEventArgs e)
        {
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

                                table1.Rows.Add(reader.GetValue(2).ToString(), reader.GetValue(1).ToString(), quantity);
                            }
                            // Получаем таблицы
                           SaveTable2();


                            LBListCalc.Items.Add("Newline. " + Type + ", " +
                                OpenClose + ", Глубина:" + Lenght + ", Высота:" + Height + ", " + Color+", Рейлинг: "+Railing);

                            
                        }
                    }
                    connection.Close();
                }
            }
            catch
            {
                //MaterialMessageBox.ShowDialog("Одно или несколько полей пустое");
                return;
            }
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
                if (CTlangth < 6)
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
                    }
                    catch
                    {
                        MaterialMessageBox.ShowDialog("Ошибка при сохранении файла!");
                    }
                }

                LBListCalc.Items.Clear();
                LBList.Items.Clear();
                SPSF.Children.Clear();
                //Count = 1;
            }
            /*ButtonSaveTxt.IsEnabled = true;
            SaveCalc.IsEnabled = false;
            TextBoxColvo.Text = "1";*/
        }

        private void TextBoxCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Code.Text.Length > 0)
            {
                // если в TextBox есть символы
                // Скрываем изображение стрелки
                LabelErrorСode.Visibility = Visibility.Hidden;
                LabelErrorСode.BeginAnimation(UIElement.OpacityProperty, null); // Остановка анимации
            }
            else
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
        // Обработка Сомбобокса Цвет
        private void ComboBoxColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxError();
            ComboBoxItem selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
            string content = selectedItem.Content.ToString();
            LabelColor.Content = content;
            if (content == "Серый")
            {
                LabelColor.BackColor = Color.LightGray; // устанавливаем цвет фона
                LabelColor.BorderStyle = BorderStyle.FixedSingle; // устанавливаем стиль границы
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
