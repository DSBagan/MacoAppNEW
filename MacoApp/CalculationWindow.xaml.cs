﻿using Google.Apis.Drive.v3.Data;
using MaterialDesignColors;
using MaterialDesignMessageBox;
using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Office.Interop.Outlook;
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
using System.Windows.Shapes;

namespace MacoApp
{
    public partial class CalculationWindow : Window
    {
        SqlRequests sqlRequests = new SqlRequests();
        public ObservableCollection<ClassList> ClassLists { get; set; }

        DataTable table1 = new DataTable("Table1"); //Таблица для сохранения расчета
        DataTable table2 = new DataTable("Table2"); // Таблица для сохранения всех расчетов

        int Count = 1;
        string rotation;
        string rotationTwoArg;
        public int quantityBar = 0;

        public CalculationWindow()
        {
            InitializeComponent();
            Loaded += CalculationWindow_Loaded;
            SaveCalc.IsEnabled = false;
            table1.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table1.Columns.Add(new DataColumn("Название", typeof(string)));
            table1.Columns.Add(new DataColumn("Количество", typeof(int)));

            table2.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table2.Columns.Add(new DataColumn("Название", typeof(string)));
            table2.Columns.Add(new DataColumn("Количество", typeof(int)));
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e) // Обработчик радиобаттона
        {
            RadioButton pressed = (RadioButton)sender;
            if (pressed.Content.ToString() == "Поворот/откид.")
            {
                rotation = "Нет";
                rotationTwoArg = "Да/Нет";
            }
            else if (pressed.Content.ToString() == "Поворотная")
            {
                rotation = "Да";
                rotationTwoArg = "Да/Нет";
            }
        }
        private void CalculationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonSaveTxt.IsEnabled = false;
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => { TBDate.Text = DateTime.Now.ToString(); };
            timer.Start();
        }

        private void ButtonCalc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = 0;
                string queryString;
                string Furn = ComboBoxFurn.Text;
                int quantity = Int32.Parse(TextBoxColvo.Text);
                int quantitySrPr = Int32.Parse(TextBoxColvo.Text);
                int System = Int32.Parse(ComboBoxSystem.Text);
                string side = ComboBoxSide.Text;
                int FFH = Int32.Parse(TextBoxFFH.Text);
                int FFB = Int32.Parse(TextBoxFFB.Text);
                string Lower_loop = ComboBoxLL.Text;
                string Micro_ventilation = ComboBoxMv.Text;

                /*if (FFH < 601)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть менее 601 мм");
                    //MessageBox.Show("Высота не может быть менее 601 мм", "Не вышло");
                    return;
                }
                else if (FFB < 400)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть менее 400 мм");
                    return;
                }
                else if (FFH > 2350)
                {
                    MaterialMessageBox.ShowDialog("Высота не может быть более 2350 мм");
                    return;
                }
                else if (FFB > 1050)
                {
                    MaterialMessageBox.ShowDialog("Ширина не может быть ,более 1050 мм");
                    return;
                }
                if (quantity == 0)
                {
                    MaterialMessageBox.ShowDialog("Укажите корректное количество комплектов");
                    return;
                }*/

                queryString = $"Select * from Elements where (Name_Furn like '" + Furn + "') and(System  = 'Не имеет значения' or System  = " + System + ") and(Side like 'Не имеет значения' or Side like '" + side + "') " +
                    "and(Lower_loop like '" + Lower_loop + "' or Lower_loop like 'Нет') and(Micro_ventilation like '" + Micro_ventilation + "' or Micro_ventilation like 'Да/Нет')" +
                    "and(Rotation like '" + rotation + "' or Rotation like '" + rotationTwoArg + "') and(FFH_before = 0 or '" + FFH + "'>=FFH_before) and(FFH_after = 0 or '" + FFH + "' <= FFH_after)" +
                    " and(FFB_before = 0 or '" + FFB + "'>=FFB_before) and(FFB_after = 0 or '" + FFB + "' <= FFB_after)";
                quantityBar = sqlRequests.Que(rotation, Furn, FFH, FFB); //Вытаскиваем из класса количество ответных планок
                quantitySrPr = sqlRequests.QueSrPr(rotation, Furn, FFH); //Количество средних прижимов на поворотной створке

                using (var connection = new SqliteConnection("Data Source=Furnapp.db"))
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    collection = new ObservableCollection<ClassList>();
                    GridList.ItemsSource = collection;
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(queryString, connection);
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                count++;
                                string response_bars1 = "";
                                string response_bars2 = "";
                                string SrPr = "";
                                string SrPrN1 = "";
                                string SrPrN2 = "";
                                if (Furn == "Maco_Eco" || Furn == "Maco_MM")
                                {
                                    response_bars1 = "34623";
                                    response_bars2 = "34850";
                                    SrPr = "54783";
                                    SrPrN1 = "41342";
                                    SrPrN2 = "41339";
                                }
                                else if (Furn == "Vorne")
                                {
                                    response_bars1 = "V25070102";
                                    response_bars2 = "V26010102";
                                    SrPr = "V17010102";
                                    SrPrN1 = "V44020107";
                                    SrPrN2 = "V44030107";
                                }
                                //Записываем данные в объект класса и ,тем самым, передаём в таблицу
                                if (reader.GetValue(3).ToString() == response_bars1 || reader.GetValue(3).ToString() == response_bars2)
                                {
                                    collection.Add(new ClassList() { Id = count, Article = "" + reader.GetValue(3).ToString(), Name = "" + reader.GetValue(2).ToString(), Quantity = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantityBar });
                                }
                                else if (reader.GetValue(3).ToString() == SrPr || reader.GetValue(3).ToString() == SrPrN1 || reader.GetValue(3).ToString() == SrPrN2)
                                {
                                    collection.Add(new ClassList() { Id = count, Article = "" + reader.GetValue(3).ToString(), Name = "" + reader.GetValue(2).ToString(), Quantity = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantitySrPr });
                                }
                                else
                                {
                                    collection.Add(new ClassList() { Id = count, Article = "" + reader.GetValue(3).ToString(), Name = "" + reader.GetValue(2).ToString(), Quantity = (int.Parse(reader.GetValue(4).ToString()) * quantity) });
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                SaveCalc.IsEnabled = true;
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
                string queryStringCreateTable;
                string Furn = ComboBoxFurn.Text;
                int quantity = Int32.Parse(TextBoxColvo.Text);
                int quantitySrPr = Int32.Parse(TextBoxColvo.Text);
                int System = Int32.Parse(ComboBoxSystem.Text);
                string side = ComboBoxSide.Text;
                int FFH = Int32.Parse(TextBoxFFH.Text);
                int FFB = Int32.Parse(TextBoxFFB.Text);
                string Lower_loop = ComboBoxLL.Text;
                string Micro_ventilation = ComboBoxMv.Text;

                queryString = $"Select * from Elements where (Name_Furn like '" + Furn + "') and(System  = 'Не имеет значения' or System  = " + System + ") and(Side like 'Не имеет значения' or Side like '" + side + "') " +
                    "and(Lower_loop like '" + Lower_loop + "' or Lower_loop like 'Нет') and(Micro_ventilation like '" + Micro_ventilation + "' or Micro_ventilation like 'Да/Нет')" +
                    "and(Rotation like '" + rotation + "' or Rotation like '" + rotationTwoArg + "') and(FFH_before = 0 or '" + FFH + "'>=FFH_before) and(FFH_after = 0 or '" + FFH + "' <= FFH_after)" +
                    " and(FFB_before = 0 or '" + FFB + "'>=FFB_before) and(FFB_after = 0 or '" + FFB + "' <= FFB_after)";
                quantityBar = sqlRequests.Que(rotation, Furn, FFH, FFB); //Вытаскиваем из класса количество ответных планок               
                quantitySrPr = sqlRequests.QueSrPr(rotation, Furn, FFH); //Количество средних прижимов на поворотной створке

                using (var connection = new SqliteConnection("Data Source=Furnapp.db"))
                {
                    ObservableCollection<ClassList> collection = null; //Обнуляем коллекцию для нового расчета
                    table1.Rows.Clear();
                    collection = new ObservableCollection<ClassList>();
                    GridList.ItemsSource = collection;
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(queryString, connection);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows) // если есть данные
                        {
                            while (reader.Read())   // построчно считываем данные
                            {
                                count++;
                                string response_bars1 = "";
                                string response_bars2 = "";
                                string SrPr = "";
                                string SrPrN1 = "";
                                string SrPrN2 = "";
                                if (Furn == "Maco_Eco" || Furn == "Maco_MM")
                                {
                                    response_bars1 = "34623";
                                    response_bars2 = "34850";
                                    SrPr = "54783";
                                    SrPrN1 = "41342";
                                    SrPrN2 = "41339";
                                }
                                else if (Furn == "Vorne")
                                {
                                    response_bars1 = "V25070102";
                                    response_bars2 = "V26010102";
                                    SrPr = "V17010102";
                                    SrPrN1 = "V44020107";
                                    SrPrN2 = "V44030107";
                                }
                                //Записываем данные в объект класса и ,тем самым, передаём в таблицу
                                if (reader.GetValue(3).ToString() == response_bars1 || reader.GetValue(3).ToString() == response_bars2)
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantityBar);
                                }
                                else if (reader.GetValue(3).ToString() == SrPr || reader.GetValue(3).ToString() == SrPrN1 || reader.GetValue(3).ToString() == SrPrN2)
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantitySrPr );
                                }
                                else
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), (int.Parse(reader.GetValue(4).ToString()) * quantity));
                                }
                            }
                            // Получаем таблицы
                            SaveTable2();
                            LBListCalc.Items.Add("" + Count + ". " + Furn + ": " + TextBoxFFB.Text + "/" +
                                TextBoxFFH.Text + ", " + ComboBoxSystem.Text + "я, Откр. " + ComboBoxSide.Text + ", Ниж. петл.- " + ComboBoxLL.Text +
                                ", Микр.- " + ComboBoxMv.Text + " " + TextBoxColvo.Text + " шт.");
                        }
                    }
                    connection.Close();
                }
                SaveCalc.IsEnabled = true;
            }
            catch
            {
                //MaterialMessageBox.ShowDialog("Одно или несколько полей пустое");
                return;
            }
            ButtonSaveTxt.IsEnabled = true;
            SaveCalc.IsEnabled = false;
            TextBoxColvo.Text = "1";
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

        //ввод только цифр в текстбоксы
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            SaveCalc.IsEnabled = false;
            int val;
            if (!Int32.TryParse(e.Text, out val) && e.Text != "-")
            {
                e.Handled = true; // отклоняем ввод
            }
        }
        //ввод только цифр в текстбоксы (пробел тоже нам не нужен)
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            SaveCalc.IsEnabled = false;
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

        

        //Сохранение расчета в файл
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Code.Text == "")
            {
                MaterialMessageBox.ShowDialog("Введите шифр фирмы");
                return;

            }
            if (LBList.Items == null)
            {
                MaterialMessageBox.ShowDialog("Сначала произведите расчет, нечего сохранять.");
                return;
            }
            else
            {
                // Проверяем есть ли на диске C папка, если нет- создаем
                Directory.CreateDirectory(@"C:\FURNMACO\");
                String date = DateTime.Now.ToString(" dd.MM.yyyy HH-mm-ss");
                int CTlangth = Code.Text.Length;
                if (CTlangth < 6)
                {
                    for (int i = 0; i < 6 - CTlangth; i++)
                    {
                        Code.Text = "0" + Code.Text;
                    }
                }
                FileStream fs = new FileStream(@"C:\FURNMACO\" + "Z" + Code.Text + " " + date + ".txt", FileMode.Create); //Присваеваем имя файлу
                StreamWriter streamWriter = new StreamWriter(fs);

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

                        if (art.Length < 10)
                        {
                            for (int i = 0; i < 10 - art.Length; i++)
                            {
                                art += " ";
                            }
                        }
                        if (nam.Length < 55)
                        {
                            int a = 55 - nam.Length;
                            for (int i = 0; i < a; i++)
                            {
                                nam += " ";
                            }
                        }

                        streamWriter.WriteLine(art + nam + "   " + qua);
                    }

                    streamWriter.WriteLine("--------------------------------------------------------------------------------");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("                    Заявку составил________________________");


                    streamWriter.Close();
                    fs.Close();
                    table2.Rows.Clear();

                    MaterialMessageBox.ShowDialog("Файл успешно сохранен");
                }
                catch
                {
                    MaterialMessageBox.ShowDialog("Ошибка при сохранении файла!");
                }
                LBListCalc.Items.Clear();
                LBList.Items.Clear();
                Count = 1;
            }
            ButtonSaveTxt.IsEnabled = false;
        }
    }
}