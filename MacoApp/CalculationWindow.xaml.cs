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

namespace MacoApp
{
    public partial class CalculationWindow : Window
    {
        SqlRequests sqlRequests = new SqlRequests();
        ClassError classError = new ClassError();
        public ObservableCollection<ClassList> ClassLists { get; set; }
        public SerializationInfo BaseUri { get; private set; }

        static string path = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();

        DataTable table1 = new DataTable("Table1"); //Таблица для сохранения расчета
        DataTable table2 = new DataTable("Table2"); // Таблица для сохранения всех расчетов

        private ObservableCollection<BitmapImage> backgroundsFON = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> backgroundsButtons = new ObservableCollection<BitmapImage>();


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

            ButtonFram.Visibility = Visibility.Hidden;

            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/MacoFon.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/MacoFon.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/vorneFon.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/RotoFon.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/internikaFon.png")));

            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/P_OL.png")));
            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/PL.png")));
            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/P_OP.png")));
            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/PP.png")));
        }

        private void CalculationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonSaveTxt.IsEnabled = false;
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => { TBDate.Text = DateTime.Now.ToString(); };
            timer.Start();
            ButtonP_O.BorderBrush = Brushes.Red;
            rotation = "Нет";
            rotationTwoArg = "Да/Нет";
            
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
                string System = ComboBoxSystem.Text;
                string side = ComboBoxSide.Text;
                int FFH = Int32.Parse(TextBoxFFH.Text);
                int FFB = Int32.Parse(TextBoxFFB.Text);
                string Lower_loop = ComboBoxLL.Text;
                string Micro_ventilation = ComboBoxMv.Text;

                if (classError.Err(Furn, FFH, FFB, quantity, rotation) == 1)
                {
                    return;
                }

                

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
                                string response_bars3 = "";
                                string SrPr = "";
                                string SrPrN1 = "";
                                string SrPrN2 = "";
                                string SrPrRama = "";
                                if (Furn == "Maco_Eco" || Furn == "Maco_MM")
                                {
                                    response_bars1 = "34623";
                                    response_bars2 = "34850";
                                    response_bars3 = "34780";
                                    SrPr = "54783";
                                    SrPrN1 = "41342";
                                    SrPrN2 = "41339";
                                }
                                else if (Furn == "Vorne")
                                {
                                    response_bars1 = "V25070102";
                                    response_bars2 = "V26010102";
                                    response_bars3 = "V25010102";
                                    SrPr = "V17010102";
                                    SrPrN1 = "V44020107";
                                    SrPrN2 = "V44030107";
                                }
                                else if (Furn == "Roto_NT")
                                {
                                    response_bars1 = "338070";
                                    response_bars2 = "260367";
                                    SrPr = "281639";
                                    SrPrRama = "281638";
                                    SrPrN1 = "208598";
                                    SrPrN2 = "208600";
                                }
                                else if (Furn == "Internika")
                                {
                                    response_bars1 = "1077591";
                                    response_bars2 = "1077246";
                                    response_bars3 = "1099378";
                                    SrPr = "1080572";
                                    SrPrN1 = "1080573";
                                    SrPrN2 = "1080574";
                                }

                                //Записываем данные в объект класса и ,тем самым, передаём в таблицу
                                if (reader.GetValue(3).ToString() == response_bars1 || reader.GetValue(3).ToString() == response_bars2 || reader.GetValue(3).ToString() == response_bars3)
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantityBar });
                                }
                                else if (reader.GetValue(3).ToString() == SrPr || reader.GetValue(3).ToString() == SrPrN1 || reader.GetValue(3).ToString() == SrPrN2|| reader.GetValue(3).ToString() == SrPrRama)
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantitySrPr });
                                }
                                else
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) });
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
                string System = ComboBoxSystem.Text;
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
                                string response_bars3 = "";
                                string SrPr = "";
                                string SrPrN1 = "";
                                string SrPrN2 = "";
                                string SrPrRama = "";
                                if (Furn == "Maco_Eco" || Furn == "Maco_MM")
                                {
                                    response_bars1 = "34623";
                                    response_bars2 = "34850";
                                    response_bars3 = "34780";
                                    SrPr = "54783";
                                    SrPrN1 = "41342";
                                    SrPrN2 = "41339";
                                }
                                else if (Furn == "Vorne")
                                {
                                    response_bars1 = "V25070102";
                                    response_bars2 = "V26010102";
                                    response_bars3 = "V25010102";
                                    SrPr = "V17010102";
                                    SrPrN1 = "V44020107";
                                    SrPrN2 = "V44030107";
                                }
                                else if (Furn == "Roto_NT")
                                {
                                    response_bars1 = "338070";
                                    response_bars2 = "260367";
                                    SrPr = "281639";
                                    SrPrRama = "281638";
                                    SrPrN1 = "208598";
                                    SrPrN2 = "208600";
                                }
                                else if (Furn == "Internika")
                                {
                                    response_bars1 = "1077591";
                                    response_bars2 = "1077246";
                                    response_bars3 = "1099378";
                                    SrPr = "1080572";
                                    SrPrN1 = "1080573";
                                    SrPrN2 = "1080574";
                                }
                                //Записываем данные в объект класса и ,тем самым, передаём в таблицу
                                if (reader.GetValue(3).ToString() == response_bars1 || reader.GetValue(3).ToString() == response_bars2 || reader.GetValue(3).ToString() == response_bars3)
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
        //***************************************************************************************************************
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
                            /*if (nam.Length < 55)
                            {
                                int a = 55 - nam.Length;
                                for (int i = 0; i < a; i++)
                                {
                                    nam += " ";
                                }
                            }*/
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
                Count = 1;
            }
            ButtonSaveTxt.IsEnabled = false;
        }

        private void ButtonP_O_Click(object sender, RoutedEventArgs e)
        {
            ButtonP_O.BorderBrush = Brushes.Red;
            ButtonP.BorderBrush = Brushes.White;
            ButtonFram.BorderBrush = Brushes.White;

            rotation = "Нет";
            rotationTwoArg = "Да/Нет";
        }

        private void ButtonP_Click(object sender, RoutedEventArgs e)
        {
            ButtonP.BorderBrush = Brushes.Red;
            ButtonP_O.BorderBrush = Brushes.White;
            ButtonFram.BorderBrush = Brushes.White;
            rotation = "Да";
            rotationTwoArg = "Да/Нет";
        }

        private void ButtonFram_Click(object sender, RoutedEventArgs e)
        {
            ButtonP_O.BorderBrush = Brushes.White;
            ButtonP.BorderBrush = Brushes.White;
            ButtonFram.BorderBrush = Brushes.Red;
        }

        //Фон для GridList*******************************************
        private void ComboBoxFurn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ComboBoxFurn.SelectedIndex;
            if (index >= 0 && index < backgroundsFON.Count)
            {
                BitmapImage image = backgroundsFON[index];
                ImageBrush brush = new ImageBrush(image);
                GridList.Background = brush;
            }
            if (index == 0)
            {
                ComboBoxMv.Visibility = Visibility.Visible;
            }
            if (index == 1)
            {
                ComboBoxMv.Visibility = Visibility.Visible;
            }
            if (index == 2)
            {
                ComboBoxMv.Visibility = Visibility.Visible;
            }
            if (index == 3)
            {
                ComboBoxMv.Visibility = Visibility.Hidden;
            }
            if (index == 4)
            {
                ComboBoxMv.Visibility = Visibility.Visible;
            }
        }

        //Фон для кнопок поворота************************************
        private void ComboBoxSide_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ButtonP_O != null && ButtonP != null)
            {
                if (ComboBoxSide.SelectedIndex == 0)
                {
                    BitmapImage image = backgroundsButtons[0];
                    ImageBrush brush = new ImageBrush(image);
                    ButtonP_O.Background = brush;

                    BitmapImage image1 = backgroundsButtons[1];
                    ImageBrush brush1 = new ImageBrush(image1);
                    ButtonP.Background = brush1;
                    
                }
                if (ComboBoxSide.SelectedIndex == 1)
                {
                    BitmapImage image = backgroundsButtons[2];
                    ImageBrush brush = new ImageBrush(image);
                    ButtonP_O.Background = brush;

                    BitmapImage image1 = backgroundsButtons[3];
                    ImageBrush brush1 = new ImageBrush(image1);
                    ButtonP.Background = brush1;
                }
            }
        }

        private void ComboBoxSystem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxFurn != null)
            {
                if (ComboBoxFurn.SelectedIndex == 3)
                {
                    MaterialMessageBox.ShowDialog("Roto можно посчитать только на 9 или 13 системе");
                    return;
                }
            }
                
        }
    }
}
