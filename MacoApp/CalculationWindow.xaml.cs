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

namespace MacoApp
{
    public partial class CalculationWindow : Window
    {
        SqlRequests sqlRequests = new SqlRequests();
        ClassError classError = new ClassError();
        public ObservableCollection<ClassList> ClassLists { get; set; }
        public SerializationInfo BaseUri { get; private set; }
        private bool isExpanded = false;

        static string path = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();

        DataTable table1 = new DataTable("Table1"); //Таблица для сохранения расчета
        DataTable table2 = new DataTable("Table2"); // Таблица для сохранения всех расчетов
        DataTable table3 = new DataTable("Table3");

        private ObservableCollection<BitmapImage> backgroundsFON = new ObservableCollection<BitmapImage>();
        private ObservableCollection<BitmapImage> backgroundsButtons = new ObservableCollection<BitmapImage>();
        //Список фрамужных артикулов
        List<string> ArticleFram1 = new List<string> { "52480","52486","52487","94491","42083","42084","V12010102",
            "V12020102","V13030102","V45010107","230177","227354","230252","230205","INT1003.07","1077266","1077266","52486","52486","52321",
            "52321","264015","230651","264007","1090505","1090506","V16030102", "1084761", "1107269", "1086935", "1107281", "ELM6000100", "ELM4080101"};
        List<string> ArticleFram2 = new List<string> { "101548", "V05010102", "482823", "1099150", };
        //Список ответных планок
        List<string> response_bars = new List<string> { "34623", "34850", "34780", "V25070102", "V26010102", "V25010102", "338070", "260367", "1077591",
            "1077246", "1099378", "1082704", "ELM0020E00", "ELM0050E00"};
        //Список средних прижимов
        List<string> SrPr = new List<string> { "54783", "41342", "41339", "V17010102", "V44020107", "V44030107", "281639", "281638", "208598", "208600",
        "1080572", "1080573", "1080574", "ELM6000400", "ELM6000500", "41582", "41583", "40756", "40757", "43568", "43569", "42135", "42136", "362185", "362186",
        "41340", "41343", "208602", "208604", "229858", "229863", "1092264", "1092265", "V44030108"};
        //Список штульповых дублирующихся артикулов
        List<string> ListStulp = new List<string> { "52480", "52486", "52487", "94491", "42083", "42084", "52483", "52478", "41742", "41743", "42087", "52479", "52484",
        "42186", "42189", "42192", "42195", "42208", "42187", "42190", "42193", "42196", "42209", "43566", "43567", "43760", "43761", "43940", "42048", "42057", "42095", 
            "42099", "42107", "41752", "41762", "42093", "42097", "42105", "41760", "41763", "42094", "42098", "42106", "42049", "42058", "42096", "42103", "42108"};
        List<string> ListStulpOtv = new List<string> { "34610"};

        int Count = 1;
        string rotation;
        string rotationTwoArg;
        string framuga;
        string framugaTwoArg;
        string konst;
        string konstTwoArg;
        string shtulp;
        string shtulpTwoArg;
        string shtulpTreeArg;
        public int quantityBar = 0;
        private bool isPanelExpanded = false;
        string wood;

        string textBox1Value;
        string textBox2Value;
        string textBox3Value;
        string textBox4Value;
        string textBox5Value;
        string textBox6Value;
        string textBox7Value;
        string textBox8Value;
        string textBox9Value;
        string textBox10Value;
        string textBox11Value;
        string textBox12Value;
        string textBox13Value;
        string textBox14Value;
        string textBox15Value;

        string CountSP;

        

        public CalculationWindow()
        {
            InitializeComponent();
            Loaded += CalculationWindow_Loaded;
            StartTextAnimation(); // Запускаем анимацию текстблока обратной связи
            SaveCalc.IsEnabled = false;
            table1.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table1.Columns.Add(new DataColumn("Название", typeof(string)));
            table1.Columns.Add(new DataColumn("Количество", typeof(int)));

            table2.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table2.Columns.Add(new DataColumn("Название", typeof(string)));
            table2.Columns.Add(new DataColumn("Количество", typeof(int)));

            table3.Columns.Add(new DataColumn("Артикул", typeof(string)));
            table3.Columns.Add(new DataColumn("Название", typeof(string)));
            table3.Columns.Add(new DataColumn("Количество", typeof(int)));

            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/MacoFon.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/MacoFonMM.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/vorneFon.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/RotoFon.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/RotoFonNX.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/internikaFon.png")));
            backgroundsFON.Add(new BitmapImage(new Uri("pack://application:,,,/images/AkpenFon.png")));

            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/P_OL.png")));
            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/PL.png")));
            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/P_OP.png")));
            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/PP.png")));
            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/Stulp_L.png")));
            backgroundsButtons.Add(new BitmapImage(new Uri("pack://application:,,,/images/Stulp_P.png")));

            LabelErrorСode.Visibility = Visibility.Hidden;
            ButtonColor.Background = Brushes.White;

            TBSR.Visibility = Visibility.Hidden;
            TBShablonRama.Visibility = Visibility.Hidden;
            TBSS.Visibility = Visibility.Hidden;
            TBShablonStvorka.Visibility = Visibility.Hidden;
            TextBlockShtulp.Visibility = Visibility.Collapsed;
            ComboBoxShtulp.Visibility = Visibility.Collapsed;
        }

        private void CalculationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonSaveTxt.IsEnabled = false;
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            //timer.Tick += (o, t) => { TBDate.Text = DateTime.Now.ToString(); };
            timer.Start();
            ButtonP_O.BorderBrush = Brushes.Red;
            rotation = "Нет";
            rotationTwoArg = "Да/Нет";
            framuga = "Нет";
            framugaTwoArg = "Да/Нет";
            konst = "Нет";
            konstTwoArg = "Да/Нет";
            shtulp = "Нет";
            shtulpTwoArg = "Да/Нет";
            shtulpTreeArg = "";

        }

        private void ButtonCalc_Click(object sender, RoutedEventArgs e)
        {

            TBSR.Visibility = Visibility.Visible;
            TBShablonRama.Visibility = Visibility.Visible;
            TBSS.Visibility = Visibility.Visible;
            TBShablonStvorka.Visibility = Visibility.Visible;

            try
            {
                int count = 0;
                string queryString;
                string Furn = ComboBoxFurn.Text;
                int quantity = Int32.Parse(TextBoxColvo.Text);
                int quantitySrPr = Int32.Parse(TextBoxColvo.Text);
                int quantityShtulp = Int32.Parse(TextBoxColvo.Text);
                string System = ComboBoxSystem.Text;
                string side = ComboBoxSide.Text;
                int FFH = Int32.Parse(TextBoxFFH.Text);
                int FFB = Int32.Parse(TextBoxFFB.Text);
                string Lower_loop = ComboBoxLL.Text;
                string Micro_ventilation = ComboBoxMv.Text;
                string wood;
                string color = ComboBoxColor.Text;
                
                if (ComboBoxSystem.Text == "Дерево")
                {
                    wood = "Да";
                }
                else
                {
                    wood = "Нет";
                }

                if (classError.Err(Furn, FFH, FFB, quantity, rotation, framuga, konst, shtulp, shtulpTreeArg) == 1)
                {
                    return;
                }

                queryString = $"Select * from Elements where (Name_Furn like '" + Furn + "') and(System  = 'Не имеет значения' or System  = '" + System + "') and(Side like 'Не имеет значения' or Side like '" + side + "') " +
                    "and(Lower_loop like '" + Lower_loop + "' or Lower_loop like 'Нет') and(Micro_ventilation like '" + Micro_ventilation + "' or Micro_ventilation like 'Да/Нет')" +
                    "and(Rotation like '" + rotation + "' or Rotation like '" + rotationTwoArg + "') and(FFH_before = 0 or '" + FFH + "'>=FFH_before) and(FFH_after = 0 or '" + FFH + "' <= FFH_after)" +
                    " and(FFB_before = 0 or '" + FFB + "'>=FFB_before) and(FFB_after = 0 or '" + FFB + "' <= FFB_after) and(Framuga like '" + framuga + "' or Framuga like '" + framugaTwoArg + "') and(Wood  = 'Да/Нет' or Wood  = '" + wood + "') " +
                    "and(Konst like '" + konst + "' or Konst like '" + konstTwoArg + "') and(Shtulp like '"+ shtulpTwoArg +"' or Shtulp  like '" + shtulp + "' or Shtulp = '"+ shtulpTreeArg +"') and(Color  = 'Не имеет значения' or Color  = '" + color + "')";
                quantityBar = sqlRequests.Que(rotation, framuga, Furn, FFH, FFB); //Вытаскиваем из класса количество ответных планок
                quantitySrPr = sqlRequests.QueSrPr(rotation, Furn, FFH); //Количество средних прижимов на поворотной створке
                quantityShtulp = sqlRequests.QueShtup(shtulp, Furn, FFH, shtulpTreeArg);  //Количество штульповых ответных планок 

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
                                //Записываем данные в объект класса и ,тем самым, передаём в таблицу
                                if (response_bars.Contains(reader.GetValue(3).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantityBar });
                                }
                                else if (SrPr.Contains(reader.GetValue(3).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantitySrPr });
                                }
                                else if (ListStulpOtv.Contains(reader.GetValue(3).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * quantityShtulp });
                                }
                                else if (framuga == "Да" && ArticleFram1.Contains(reader.GetValue(3).ToString()) && FFH <= 1600)
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * 2 });
                                }
                                else if (framuga == "Да" && ArticleFram1.Contains(reader.GetValue(3).ToString()) && FFH >= 1601 && FFH <= 2400)
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * 3 });
                                }
                                else if (framuga == "Да" && ArticleFram2.Contains(reader.GetValue(3).ToString()) && FFH >= 1101)
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * 2 });
                                }
                                else if (shtulp=="Да" && ListStulp.Contains(reader.GetValue(3).ToString()))
                                {
                                    collection.Add(new ClassList() { N = count, Артикул = "" + reader.GetValue(3).ToString(), Название = "" + reader.GetValue(2).ToString(), Шт = (int.Parse(reader.GetValue(4).ToString()) * quantity) * 2});
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
                int quantityShtulp = Int32.Parse(TextBoxColvo.Text);
                string System = ComboBoxSystem.Text;
                string side = ComboBoxSide.Text;
                int FFH = Int32.Parse(TextBoxFFH.Text);
                int FFB = Int32.Parse(TextBoxFFB.Text);
                string Lower_loop = ComboBoxLL.Text;
                string Micro_ventilation = ComboBoxMv.Text;
                string color = ComboBoxColor.Text;

                if (ComboBoxSystem.Text == "Дерево")
                {
                    wood = "Да";
                }
                else
                {
                    wood = "Нет";
                }

                queryString = $"Select * from Elements where (Name_Furn like '" + Furn + "') and(System  = 'Не имеет значения' or System  = '" + System + "') and(Side like 'Не имеет значения' or Side like '" + side + "') " +
                    "and(Lower_loop like '" + Lower_loop + "' or Lower_loop like 'Нет') and(Micro_ventilation like '" + Micro_ventilation + "' or Micro_ventilation like 'Да/Нет')" +
                    "and(Rotation like '" + rotation + "' or Rotation like '" + rotationTwoArg + "') and(FFH_before = 0 or '" + FFH + "'>=FFH_before) and(FFH_after = 0 or '" + FFH + "' <= FFH_after)" +
                    " and(FFB_before = 0 or '" + FFB + "'>=FFB_before) and(FFB_after = 0 or '" + FFB + "' <= FFB_after) and(Framuga like '" + framuga + "' or Framuga like '" + framugaTwoArg + "') and(Wood  = 'Да/Нет' or Wood  = '" + wood + "') " +
                    "and(Konst like '" + konst + "' or Konst like '" + konstTwoArg + "') and(Shtulp like '" + shtulpTwoArg + "' or Shtulp  like '" + shtulp + "' or Shtulp = '" + shtulpTreeArg + "') and(Color  = 'Не имеет значения' or Color  = '" + color + "')";
                quantityBar = sqlRequests.Que(rotation, framuga, Furn, FFH, FFB); //Вытаскиваем из класса количество ответных планок               
                quantitySrPr = sqlRequests.QueSrPr(rotation, Furn, FFH); //Количество средних прижимов на поворотной створке
                quantityShtulp = sqlRequests.QueShtup(shtulp, Furn, FFH, shtulpTreeArg);  //Количество штульповых ответных планок 

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
                                //Записываем данные в объект класса и ,тем самым, передаём в таблицу
                                if (response_bars.Contains(reader.GetValue(3).ToString()))
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * quantityBar);
                                }
                                else if (ListStulpOtv.Contains(reader.GetValue(3).ToString()))
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * quantityShtulp);
                                }
                                else if (SrPr.Contains(reader.GetValue(3).ToString()))
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * quantitySrPr);
                                }
                                else if (framuga == "Да" && ArticleFram1.Contains(reader.GetValue(3).ToString()) && FFH <= 1600)
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * 2);
                                }
                                else if (framuga == "Да" && ArticleFram1.Contains(reader.GetValue(3).ToString()) && FFH >= 1601 && FFH <= 2400)
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * 3);
                                }
                                else if (framuga == "Да" && ArticleFram2.Contains(reader.GetValue(3).ToString()) && FFH >= 1101)
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * 2);
                                }
                                else if (shtulp == "Да" && ListStulp.Contains(reader.GetValue(3).ToString()))
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * 2);
                                }
                                else
                                {
                                    table1.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity);
                                }
                            }


                            // Получаем таблицы
                            SaveTable2();
                            LBListCalc.Items.Add("" + Count + ". " + Furn + ": " + TextBoxFFB.Text + "/" +
                                TextBoxFFH.Text + ", " + ComboBoxSystem.Text + "я, Откр. " + ComboBoxSide.Text + ", Ниж. петл.- " + ComboBoxLL.Text +
                                ", Микр.- " + ComboBoxMv.Text + " " + TextBoxColvo.Text + " шт.");

                            // Создаем элементы управления для новой строки стекпанели
                            TextBlock textBlockFurn = new TextBlock();
                            TextBlock textBlockFFBN = new TextBlock();
                            textBlockFFBN.Text = " ";
                            TextBlock textBlockFFB = new TextBlock();
                            TextBlock textBlockFFHN = new TextBlock();
                            textBlockFFHN.Text = "/";
                            TextBlock textBlockFFH = new TextBlock();
                            TextBlock textBlockSystemN = new TextBlock();
                            textBlockSystemN.Text = " Сист.: ";
                            TextBlock textBlockSystem = new TextBlock();
                            TextBlock textBlockSideN = new TextBlock();
                            textBlockSideN.Text = " Сторона: ";
                            TextBlock textBlockSide = new TextBlock();
                            TextBlock textBlockLLN = new TextBlock();
                            textBlockLLN.Text = " Н. петл.: ";
                            TextBlock textBlockLL = new TextBlock();
                            TextBlock textBlockMicrVN = new TextBlock();
                            textBlockMicrVN.Text = " Микр.: ";
                            TextBlock textBlockMicrV = new TextBlock();

                            TextBlock textBlockKonstN = new TextBlock();
                            textBlockKonstN.Text = " Ручка: ";
                            TextBlock textBlockKonst = new TextBlock();

                            TextBlock textBlockKonstN1 = new TextBlock();
                            textBlockKonstN1.Text = "      ";
                            TextBlock textBlockKolVoN = new TextBlock();
                            textBlockKolVoN.Text = " шт.  ";
                            TextBlock textBlockKolVo = new TextBlock();

                            TextBlock textBlockwood = new TextBlock();
                            textBlockwood.Text = wood;
                            textBlockwood.Width = 0;
                            textBlockwood.Height = 0;
                            TextBlock textBlockrotationTwoArg = new TextBlock();
                            textBlockrotationTwoArg.Text = rotationTwoArg;
                            textBlockrotationTwoArg.Width = 0;
                            textBlockrotationTwoArg.Height = 0;
                            TextBlock textBlockframugaTwoArg = new TextBlock();
                            textBlockframugaTwoArg.Text = framugaTwoArg;
                            textBlockframugaTwoArg.Width = 0;
                            textBlockframugaTwoArg.Height = 0;
                            TextBlock textBlockkonstTwoArg = new TextBlock();
                            textBlockkonstTwoArg.Text = konstTwoArg;
                            textBlockkonstTwoArg.Width = 0;
                            textBlockkonstTwoArg.Height = 0;
                            TextBlock Rotat = new TextBlock();
                            textBlockkonstTwoArg.Text = rotation;
                            textBlockkonstTwoArg.Width = 0;
                            textBlockkonstTwoArg.Height = 0;
                            TextBlock Framuga = new TextBlock();
                            textBlockkonstTwoArg.Text = framuga;
                            textBlockkonstTwoArg.Width = 0;
                            textBlockkonstTwoArg.Height = 0;

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
                            textBlockFurn.Text = Furn;
                            textBlockFFB.Text = "" + FFB;
                            textBlockFFH.Text = "" + FFH;
                            textBlockSystem.Text = System;
                            textBlockSide.Text = side;
                            textBlockLL.Text = Lower_loop;
                            textBlockMicrV.Text = Micro_ventilation;
                            textBlockKonst.Text = konst;
                            textBlockKolVo.Text = "" + quantity;
                            textBlockwood.Text = wood;
                            textBlockrotationTwoArg.Text = rotationTwoArg;
                            textBlockframugaTwoArg.Text = framugaTwoArg;
                            textBlockkonstTwoArg.Text = konstTwoArg;
                            Rotat.Text = rotation;
                            Rotat.Width = 0;
                            Rotat.Height = 0;
                            Framuga.Text = framuga;
                            Framuga.Width = 0;
                            Framuga.Height = 0;

                            // Создаем новую строку с использованием созданных элементов
                            StackPanel newStackPanel = new StackPanel();
                            newStackPanel.Orientation = Orientation.Horizontal;
                            newStackPanel.HorizontalAlignment = HorizontalAlignment.Right;

                            newStackPanel.Children.Add(textBlockFurn);
                            newStackPanel.Children.Add(textBlockFFBN);
                            newStackPanel.Children.Add(textBlockFFB);
                            newStackPanel.Children.Add(textBlockFFHN);
                            newStackPanel.Children.Add(textBlockFFH);
                            newStackPanel.Children.Add(textBlockSystemN);
                            newStackPanel.Children.Add(textBlockSystem);
                            newStackPanel.Children.Add(textBlockSideN);
                            newStackPanel.Children.Add(textBlockSide);
                            newStackPanel.Children.Add(textBlockLLN);
                            newStackPanel.Children.Add(textBlockLL);
                            newStackPanel.Children.Add(textBlockMicrVN);
                            newStackPanel.Children.Add(textBlockMicrV);
                            newStackPanel.Children.Add(textBlockKonstN);
                            newStackPanel.Children.Add(textBlockKonst);
                            newStackPanel.Children.Add(textBlockKonstN1);
                            newStackPanel.Children.Add(textBlockKolVo);
                            newStackPanel.Children.Add(textBlockKolVoN);
                            newStackPanel.Children.Add(textBlockwood);
                            newStackPanel.Children.Add(textBlockrotationTwoArg);
                            newStackPanel.Children.Add(textBlockframugaTwoArg);
                            newStackPanel.Children.Add(textBlockkonstTwoArg);
                            newStackPanel.Children.Add(Rotat);
                            newStackPanel.Children.Add(Framuga);
                            newStackPanel.Children.Add(deleteButton);
                            // Добавляем новую строку в StackPanel
                            SPSF.Children.Add(newStackPanel);
                            //массив текстблоков для окрашивания
                            TextBlock[] textblockArray = new TextBlock[] { textBlockFurn, textBlockFFB, textBlockFFH, textBlockSystem, textBlockSide, textBlockLL, textBlockMicrV, textBlockKonst,
                                textBlockKolVo, textBlockwood};
                            ColorTextblock(textblockArray);
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

        //Окрашиваем Текстбокс
        public void ColorTextblock(TextBlock[] textblockArray)
        {
            foreach (var TextBlock in textblockArray)
            {
                // Получаем текст из текстблока
                string text = TextBlock.Text;

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

        

        //Сохранение расчета в файл
        //***************************************************************************************************************
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*if (Code.Text == "")
            {
                MaterialMessageBox.ShowDialog("Введите шифр фирмы");
                return;

            }*/

            if (LBList.Items == null)
            {
                MaterialMessageBox.ShowDialog("Сначала произведите расчет, нечего сохранять.");
                return;
            }
            else
            {
                try
                {
                    // Проверяем есть диск X b папка aTBMFURN, если нет- создаем
                    Directory.CreateDirectory(@"C:\aTBMFURN\");
                }
                catch (System.Exception)
                {
                    MaterialMessageBox.ShowDialog("Нет доступа к диску X");
                    return;
                }
                
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

        private void ButtonP_O_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxMv.IsEnabled = true;
            TextBlockMv.IsEnabled = true;
            ButtonP_O.BorderBrush = Brushes.Red;
            ButtonP.BorderBrush = Brushes.White;
            ButtonFram.BorderBrush = Brushes.White;
            ButtonStulp.BorderBrush = Brushes.White;
            TextBlockShtulp.Visibility = Visibility.Collapsed;
            ComboBoxShtulp.Visibility = Visibility.Collapsed;

            rotation = "Нет";
            rotationTwoArg = "Да/Нет";
            framuga = "Нет";
            framugaTwoArg = "Да/Нет";
            shtulp = "Нет";
            shtulpTwoArg = "Да/Нет";
            shtulpTreeArg = "";

            ComboBoxLL.IsEnabled = true;
            TextBlockLL.IsEnabled = true;
            ComboBoxSide.IsEnabled = true;
            TextBlockSide.IsEnabled = true;
            ComboBoxKonst.IsEnabled = true;

            // Получаем доступ к объекту TextBox
            TextBox myTextBox = this.MainGrid.Children.OfType<TextBox>().ElementAt(2);
            // Устанавливаем значение свойства Grid.Column
            Grid.SetRow(myTextBox, 3);
            TextBox myTextBox1 = this.MainGrid.Children.OfType<TextBox>().ElementAt(1);
            Grid.SetRow(myTextBox1, 2);
        }

        private void ButtonP_Click(object sender, RoutedEventArgs e)
        {
            ButtonP.BorderBrush = Brushes.Red;
            ButtonP_O.BorderBrush = Brushes.White;
            ButtonFram.BorderBrush = Brushes.White;
            ButtonStulp.BorderBrush = Brushes.White;
            rotation = "Да";
            rotationTwoArg = "Да/Нет";
            framuga = "Нет";
            framugaTwoArg = "Да/Нет";
            konst = "Нет";
            shtulp = "Нет";
            shtulpTwoArg = "Да/Нет";
            shtulpTreeArg = "";

            //konstTwoArg = "Да/Нет";
            ComboBoxMv.IsEnabled = false;
            TextBlockMv.IsEnabled = false;
            ComboBoxKonst.IsEnabled = false;
            ComboBoxLL.IsEnabled = true;
            TextBlockLL.IsEnabled = true;
            ComboBoxSide.IsEnabled = true;
            TextBlockSide.IsEnabled = true;
            TextBlockShtulp.Visibility = Visibility.Collapsed;
            ComboBoxShtulp.Visibility = Visibility.Collapsed;

            // Получаем доступ к объекту TextBox
            TextBox myTextBox = this.MainGrid.Children.OfType<TextBox>().ElementAt(2);
            // Устанавливаем значение свойства Grid.Column
            Grid.SetRow(myTextBox, 3);
            TextBox myTextBox1 = this.MainGrid.Children.OfType<TextBox>().ElementAt(1);
            Grid.SetRow(myTextBox1, 2);
        }

        private void ButtonFram_Click(object sender, RoutedEventArgs e)
        {
            ButtonP_O.BorderBrush = Brushes.White;
            ButtonP.BorderBrush = Brushes.White;
            ButtonFram.BorderBrush = Brushes.Red;
            ButtonStulp.BorderBrush = Brushes.White;
            rotation = "Нет";
            rotationTwoArg = "Да/Нет";
            framuga = "Да";
            framugaTwoArg = "Да/Нет";
            konst = "Нет";
            shtulp = "Нет";
            shtulpTwoArg = "Да/Нет";
            shtulpTreeArg = "";
            //konstTwoArg = "Да/Нет";
            ComboBoxMv.IsEnabled = false;
            TextBlockMv.IsEnabled = false;
            ComboBoxLL.IsEnabled = false;
            TextBlockLL.IsEnabled = false;
            ComboBoxSide.IsEnabled = false;
            TextBlockSide.IsEnabled = false;
            ComboBoxKonst.IsEnabled = false;
            TextBlockShtulp.Visibility = Visibility.Collapsed;
            ComboBoxShtulp.Visibility = Visibility.Collapsed;

            // Получаем доступ к объекту TextBox
            TextBox myTextBox = this.MainGrid.Children.OfType<TextBox>().ElementAt(2);
            // Устанавливаем значение свойства Grid.Column
            Grid.SetRow(myTextBox, 2);
            TextBox myTextBox1 = this.MainGrid.Children.OfType<TextBox>().ElementAt(1);
            Grid.SetRow(myTextBox1, 3);
        }

        private void ButtonStulp_Click(object sender, RoutedEventArgs e)
        {
            ButtonP_O.BorderBrush = Brushes.White;
            ButtonP.BorderBrush = Brushes.White;
            ButtonFram.BorderBrush = Brushes.White;
            ButtonStulp.BorderBrush = Brushes.Red;
            rotation = "Нет";
            rotationTwoArg = "Да/Нет";
            framuga = "Нет";
            framugaTwoArg = "Да/Нет";
            konst = "Нет";
            shtulp = "Да";
            shtulpTwoArg = "Да/Нет";
            if (ComboBoxShtulp.Text == "Шпингалеты")
            {
                shtulpTreeArg = "Шпингалет";
            }
            else if (ComboBoxShtulp.Text == "Штульп. запор")
            {
                shtulpTreeArg = "Запор";
            }

            //konstTwoArg = "Да/Нет";
            ComboBoxMv.IsEnabled = true;
            TextBlockMv.IsEnabled = true;
            ComboBoxLL.IsEnabled = true;
            TextBlockLL.IsEnabled = true;
            ComboBoxSide.IsEnabled = true;
            TextBlockSide.IsEnabled = true;
            ComboBoxKonst.IsEnabled = false;
            TextBlockShtulp.Visibility = Visibility.Visible;
            ComboBoxShtulp.Visibility = Visibility.Visible;

            // Получаем доступ к объекту TextBox
            TextBox myTextBox = this.MainGrid.Children.OfType<TextBox>().ElementAt(2);
            // Устанавливаем значение свойства Grid.Column
            Grid.SetRow(myTextBox, 3);
            TextBox myTextBox1 = this.MainGrid.Children.OfType<TextBox>().ElementAt(1);
            Grid.SetRow(myTextBox1, 2);
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
            //Скрываем микровентиляцию на поворотных створках и фрамугах
            if (rotation == "Да" || framuga == "Да")
            {
                ComboBoxMv.IsEnabled = false;
                TextBlockMv.IsEnabled = false;
                ComboBoxKonst.IsEnabled = false;
            }
            else
            {
                ComboBoxMv.IsEnabled = true;
                TextBlockMv.IsEnabled = true;
                ComboBoxKonst.IsEnabled = true;
            }

            try
            {
                if (index == 0)
                {
                    TBShablonRama.Text = "21958";
                    TBShablonStvorka.Text = "21564";
                }
                if (index == 1)
                {
                    TBShablonRama.Text = "21958";
                    TBShablonStvorka.Text = "21564";
                }
                if (index == 2)
                {
                    TBShablonRama.Text = "V50040714";
                    TBShablonStvorka.Text = "V50030114N";
                }
                if (index == 3)
                {
                    TBShablonRama.Text = "Нет";
                    TBShablonStvorka.Text = "Нет";
                }
                if (index == 4)
                {
                    TBShablonRama.Text = "Нет";
                    TBShablonStvorka.Text = "Нет";
                }
                if (index == 5)
                {
                    TBShablonRama.Text = "1080391";
                    TBShablonStvorka.Text = "1080369";
                }
                if (index == 6)
                {
                    TBShablonRama.Text = "ELM0030200";
                    TBShablonStvorka.Text = "ELM0030100";
                }
            }
            catch (System.Exception)
            {

                return;
            }
            
        }

        //Анимация текстблока обратной связи
        private void StartTextAnimation()
        {
            string[] texts = new string[] {"Есть предложение по улучшению программы? Напиши \u2192", "Нашел ошибку? Напиши \u2192", "Есть неточность? Напиши \u2192", "Обязательно проверь расчет после подгрузки в КиС!!!" }; // Замените на свои тексты
            int currentIndex = 0;

            // Создаем таймер, который будет вызывать смену текста каждые 20 секунд
            System.Windows.Threading.DispatcherTimer textTimer = new System.Windows.Threading.DispatcherTimer();
            textTimer.Interval = TimeSpan.FromSeconds(30);
            textTimer.Tick += (sender, e) =>
            {
                var fadeOut = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
                fadeOut.Completed += (s, a) =>
                {
                    TextBlockFeedback.Text = texts[currentIndex];
                    var fadeIn = new DoubleAnimation(1, (Duration)TimeSpan.FromSeconds(1));
                    TextBlockFeedback.BeginAnimation(UIElement.OpacityProperty, fadeIn);
                };
                TextBlockFeedback.BeginAnimation(UIElement.OpacityProperty, fadeOut);

                currentIndex = (currentIndex + 1) % texts.Length;
            };

            textTimer.Start();
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

                    BitmapImage image2 = backgroundsButtons[4];
                    ImageBrush brush2 = new ImageBrush(image2);
                    ButtonStulp.Background = brush2;

                }
                if (ComboBoxSide.SelectedIndex == 1)
                {
                    BitmapImage image = backgroundsButtons[2];
                    ImageBrush brush = new ImageBrush(image);
                    ButtonP_O.Background = brush;

                    BitmapImage image1 = backgroundsButtons[3];
                    ImageBrush brush1 = new ImageBrush(image1);
                    ButtonP.Background = brush1;

                    BitmapImage image2 = backgroundsButtons[5];
                    ImageBrush brush2 = new ImageBrush(image2);
                    ButtonStulp.Background = brush2;
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

        private void ComboBoxShtulp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxShtulp != null)
            {
                if (ComboBoxShtulp.SelectedIndex == 0)
                {
                    shtulpTreeArg = "Шпингалет";
                }
                else if (ComboBoxShtulp.SelectedIndex == 1)
                {
                    shtulpTreeArg = "Запор";
                }
            }
        }

        private void ComboBoxKonst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxKonst.SelectedIndex == 0)
            {
                konst = "Да";
            }
            else
            {
                konst = "Нет";
            }
        }
        private void ComboBoxColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxColor.SelectedIndex >= 0)
                {
                    if (ComboBoxColor.SelectedIndex == 0)
                    {
                        ButtonColor.Background = Brushes.White;
                    }
                    else if (ComboBoxColor.SelectedIndex == 1)
                    {
                        ButtonColor.Background = Brushes.Chocolate;
                    }
                    else if (ComboBoxColor.SelectedIndex == 2)
                    {
                        ButtonColor.Background = Brushes.Brown;
                    }
                    else if (ComboBoxColor.SelectedIndex == 3)
                    {
                        ButtonColor.Background = Brushes.Gray;
                    }
                    else if (ComboBoxColor.SelectedIndex == 4)
                    {
                        ButtonColor.Background = Brushes.DarkGray;
                    }
                    else if (ComboBoxColor.SelectedIndex == 5)
                    {
                        ButtonColor.Background = Brushes.Black;
                    }
                    else if (ComboBoxColor.SelectedIndex == 6)
                    {
                        ButtonColor.Background = Brushes.Yellow;
                    }
                }
            }
            catch (System.Exception)
            {
                return;
            }
            

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
            textBox9Value = ((TextBlock)parentStackPanel.Children[16]).Text;
            textBox10Value = ((TextBlock)parentStackPanel.Children[18]).Text;
            textBox11Value = ((TextBlock)parentStackPanel.Children[19]).Text;
            textBox12Value = ((TextBlock)parentStackPanel.Children[20]).Text;
            textBox13Value = ((TextBlock)parentStackPanel.Children[21]).Text;
            textBox14Value = ((TextBlock)parentStackPanel.Children[22]).Text;
            textBox15Value = ((TextBlock)parentStackPanel.Children[23]).Text;

            // Получаем значения TextBlock элементов в строке

            try
            {
                string queryString;
                string queryStringCreateTable;
                string Furn = textBox1Value;
                int quantity = int.Parse(textBox9Value);
                int quantitySrPr = int.Parse(textBox9Value);
                string System = textBox4Value;
                string side = textBox5Value;
                int FFH = int.Parse(textBox3Value);
                int FFB = int.Parse(textBox2Value);
                string Lower_loop = textBox6Value;
                string Micro_ventilation = textBox7Value;
                string FramugaTwoArg = textBox12Value;
                string KonstTwoArg = textBox13Value;
                string RotationTwoArg = textBox11Value;
                string Wood = textBox10Value;
                string Rot = textBox14Value;
                string Fr = textBox15Value;
                string Konst = textBox8Value;

                queryString = $"Select * from Elements where (Name_Furn like '" + Furn + "') and(System  = 'Не имеет значения' or System  = '" + System + "') and(Side like 'Не имеет значения' or Side like '" + side + "') " +
                    "and(Lower_loop like '" + Lower_loop + "' or Lower_loop like 'Нет') and(Micro_ventilation like '" + Micro_ventilation + "' or Micro_ventilation like 'Да/Нет')" +
                    "and(Rotation like '" + Rot + "' or Rotation like '" + RotationTwoArg + "') and(FFH_before = 0 or '" + FFH + "'>=FFH_before) and(FFH_after = 0 or '" + FFH + "' <= FFH_after)" +
                    " and(FFB_before = 0 or '" + FFB + "'>=FFB_before) and(FFB_after = 0 or '" + FFB + "' <= FFB_after) and(Framuga like '" + Fr + "' or Framuga like '" + FramugaTwoArg + "') and(Wood  = 'Да/Нет' or Wood  = '" + Wood + "') and(Konst like '" + Konst + "' or Konst like '" + KonstTwoArg + "')";
                quantityBar = sqlRequests.Que(Rot, Fr, Furn, FFH, FFB); //Вытаскиваем из класса количество ответных планок               
                quantitySrPr = sqlRequests.QueSrPr(Rot, Furn, FFH); //Количество средних прижимов на поворотной створке

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
                                //Записываем данные в объект класса и ,тем самым, передаём в таблицу
                                if (response_bars.Contains(reader.GetValue(3).ToString()))
                                {
                                    table3.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * quantityBar);
                                }
                                else if (SrPr.Contains(reader.GetValue(3).ToString()))
                                {
                                    table3.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * quantitySrPr);
                                }
                                else if (framuga == "Да" && ArticleFram1.Contains(reader.GetValue(3).ToString()) && FFH <= 1600)
                                {
                                    table3.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * 2);
                                }
                                else if (framuga == "Да" && ArticleFram1.Contains(reader.GetValue(3).ToString()) && FFH >= 1601 && FFH <= 2400)
                                {
                                    table3.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * 3);
                                }
                                else if (framuga == "Да" && ArticleFram2.Contains(reader.GetValue(3).ToString()) && FFH >= 1101)
                                {
                                    table3.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity * 2);
                                }
                                else
                                {
                                    table3.Rows.Add(reader.GetValue(3).ToString(), reader.GetValue(2).ToString(), int.Parse(reader.GetValue(4).ToString()) * quantity);
                                }
                            }
                            //Удаляем из таблицы
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
                //MaterialMessageBox.ShowDialog("Одно или несколько полей пустое");
                return;
            }

            // Удаляем строку из StackPanel
            SPSF.Children.Remove(parentStackPanel);
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
        private void ButtonFeedback_Click(object sender, RoutedEventArgs e)
        {
            FeedbackWindow feedbackWindow = new FeedbackWindow();
            feedbackWindow.Show();
        }

        
    }
}
