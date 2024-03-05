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

namespace TBMFurn
{
    public partial class BoxCalculation : Window
    {
        DataTable table1 = new DataTable("Table1"); //Таблица для сохранения расчета
        DataTable table2 = new DataTable("Table2"); // Таблица для сохранения всех расчетов
        DataTable table3 = new DataTable("Table3");

        string LenghtText;

        private ObservableCollection<BitmapImage> backgroundsFONBox = new ObservableCollection<BitmapImage>();

        public BoxCalculation()
        {
            InitializeComponent();

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

            ComboBoxLenght.SelectionChanged += ComboBoxLenght_SelectionChanged;

            DataGridImage.HeadersVisibility = DataGridHeadersVisibility.None;
        }

        private void ButtonCalc_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxType.Text == "")
            {
                MessageBox.Show("Выбери тип Ящика");
                return;
            }
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
                string Railing = ComboBoxRailing.Text;
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

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        // Ниже ряд обработчиков Combobox
        private void ComboBoxType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
            if (ComboBoxHeight.SelectedIndex == 0)
            {
                ComboBoxRailing.Items.Clear();
                ComboBoxRailing.Visibility = Visibility.Collapsed;
                TextBlockRailing.Visibility = Visibility.Collapsed;
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
            TextBlockLenght.Text = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
        }


        //ввод только цифр в текстбоксы
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //SaveCalc.IsEnabled = false;
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
