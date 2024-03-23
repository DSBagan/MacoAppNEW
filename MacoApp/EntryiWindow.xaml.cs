using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using TBMFurn;
using MySql.Data.MySqlClient;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MacoApp
{
    public partial class EntryiWindow : Window
    {
        static string path = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString()+"\\Furnapp.db";

        //Создаем файл блокировки, если приложение уже запущено
        string lockFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyAppLock.lock");

        //Создаем коллекцию лого
        private ObservableCollection<BitmapImage> backgroundsLogo = new ObservableCollection<BitmapImage>();

        public EntryiWindow()
        {
            InitializeComponent();

            backgroundsLogo.Add(new BitmapImage(new Uri("pack://application:,,,/images/maco.png")));
            backgroundsLogo.Add(new BitmapImage(new Uri("pack://application:,,,/images/roto-transformed.png")));
            backgroundsLogo.Add(new BitmapImage(new Uri("pack://application:,,,/images/Vorne-logo.png")));
            backgroundsLogo.Add(new BitmapImage(new Uri("pack://application:,,,/images/28.png")));
            backgroundsLogo.Add(new BitmapImage(new Uri("pack://application:,,,/images/akpen-logo.png")));
            backgroundsLogo.Add(new BitmapImage(new Uri("pack://application:,,,/images/Firmax1.png")));

            Loaded += EntryiWindow_Loaded;
        }

        private void EntryiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntOn.Visibility = Visibility.Collapsed;
            IntOff.Visibility = Visibility.Collapsed;
            ButtonEditor.Visibility = Visibility.Collapsed;

            for (int i = 0; i < backgroundsLogo.Count; i++)
            {
                Image img = new Image();
                img.Source = backgroundsLogo[i];
                stackPanelLogo.Children.Add(img);
            }
            


            UpgradeBD();
        }
        private async void UpgradeBD()
        {

                // Это первый запуск приложения
                // Создать файл блокировки
                File.Create(lockFilePath);

                if (Directory.Exists(@"X:\aTBMFURN\"))
                {
                    string[] files = Directory.GetFiles(@"X:\aTBMFURN\");
                    foreach (string file in files)
                    {
                        File.Delete(file);
                    }
                    // Удаление папки c сохраненными расчетами и всех ее подпапок и файлов
                }

                FileInfo fileInf = new FileInfo(path);
                if (fileInf.Exists)
                {
                    fileInf.Delete();
                }
                WebClient webClient = new WebClient();
                //Качаем БД с Google Drive

                //webClient.DownloadFile("https://drive.google.com/uc?export=download&id=18KBF6LMWrxoDqy8cUdEUaZYCXC_8SLPu", path);
                webClient.DownloadFile("https://drive.google.com/uc?export=download&id=11NtSrLJ481KCTRlefNl_KA5IvvwWG6-Q", path);
                webClient.Dispose();
            


            /*ProgressDialogWindow progressDialog = new ProgressDialogWindow();
            progressDialog.Show();

            await Task.Run(() =>
            {
                string connectionStringMySQL = "server=cz6.h.filess.io;user=BDFurnTBM_forcearmy;database=BDFurnTBM_forcearmy;port=3307;password=e5f1b53ca8619010c3c73fd972facf6e383871ed;";
                using MySqlConnection connectionMySQL = new MySqlConnection(connectionStringMySQL);
                connectionMySQL.Open();

                string connectionStringSQLite = "Data Source=Furnapp.db;";
                using SQLiteConnection connectionSQLite = new SQLiteConnection(connectionStringSQLite);
                connectionSQLite.Open();
                
                // Создание таблицы в базе данных SQLite
                string createTableQuery = "CREATE TABLE IF NOT EXISTS Elements (Id INTEGER PRIMARY KEY, Name_Furn TEXT, Title TEXT, Article TEXT, Quantity INTEGER, System TEXT, Side TEXT, FFH_before INTEGER, FFH_after INTEGER, FFB_before INTEGER, FFB_after INTEGER, Lower_loop TEXT, Micro_ventilation TEXT, Rotation TEXT, Framuga TEXT, Wood TEXT, Konst TEXT)";
                using SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connectionSQLite);
                createTableCommand.ExecuteNonQuery();

                // Извлечение данных из таблицы MySQL и вставка их в таблицу SQLite
                string selectQuery = "SELECT * FROM Elements";
                using MySqlCommand selectCommand = new MySqlCommand(selectQuery, connectionMySQL);
                using MySqlDataReader reader = selectCommand.ExecuteReader();
                int totalRows = 0;
                while (reader.Read())
                {
                    //int Id = Convert.ToInt32(reader["Id"]);
                    string nameFurn = reader["Name_Furn"].ToString();
                    string title = reader["Title"].ToString();
                    string article = reader["Article"].ToString();
                    int quantity = Convert.ToInt32(reader["Quantity"]);
                    string system = reader["System"].ToString();
                    string side = reader["Side"].ToString();
                    int ffhBefore = Convert.ToInt32(reader["FFH_before"]);
                    int ffhAfter = Convert.ToInt32(reader["FFH_after"]);
                    int ffbBefore = Convert.ToInt32(reader["FFB_before"]);
                    int ffbAfter = Convert.ToInt32(reader["FFB_after"]);
                    string lowerLoop = reader["Lower_loop"].ToString();
                    string microVentilation = reader["Micro_ventilation"].ToString();
                    string rotation = reader["Rotation"].ToString();
                    string framuga = reader["Framuga"].ToString();
                    string wood = reader["Wood"].ToString();
                    string konst = reader["Konst"].ToString();

                    string insertQuery = $"INSERT OR REPLACE INTO Elements (Name_Furn, Title, Article, Quantity, System, Side, FFH_before, FFH_after, FFB_before, FFB_after, Lower_loop, Micro_ventilation, Rotation, Framuga, Wood, Konst) VALUES ('{nameFurn}', '{title}', '{article}', {quantity}, '{system}', '{side}', '{ffhBefore}', '{ffhAfter}', '{ffbBefore}', '{ffbAfter}', '{lowerLoop}', '{microVentilation}', '{rotation}', '{framuga}', '{wood}', '{konst}')";
                    using SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connectionSQLite);
                    insertCommand.ExecuteNonQuery();
                    totalRows++;
                    System.Threading.Thread.Sleep(570); // Для имитации процесса
                    UpdateProgress(progressDialog, totalRows);
                }
                connectionMySQL.Close();
                connectionSQLite.Close();

                string connectionStringMySQLBox = "server=cz6.h.filess.io;user=BDFurnTBM_forcearmy;database=BDFurnTBM_forcearmy;port=3307;password=e5f1b53ca8619010c3c73fd972facf6e383871ed;";
                using MySqlConnection connectionMySQLBox = new MySqlConnection(connectionStringMySQL);
                connectionMySQL.Open();

                string connectionStringSQLiteBox = "Data Source=Furnapp.db;";
                using SQLiteConnection connectionSQLiteBox = new SQLiteConnection(connectionStringSQLite);
                connectionSQLite.Open();

                // Создание таблицы в базе данных SQLite
                string createTableQueryBox = "CREATE TABLE IF NOT EXISTS BoxeFirmaxTable (Id INTEGER PRIMARY KEY, Name TEXT, Article TEXT, System TEXT, Type_of_opening TEXT, Length TEXT, Height TEXT, Color TEXT, Railing TEXT, Inner_drawer TEXT, Washing TEXT)";
                using SQLiteCommand createTableCommandBox = new SQLiteCommand(createTableQueryBox, connectionSQLite);
                createTableCommandBox.ExecuteNonQuery();

                // Извлечение данных из таблицы MySQL и вставка их в таблицу SQLite
                string selectQueryBox = "SELECT * FROM BoxeFirmaxTable";
                using MySqlCommand selectCommandBox = new MySqlCommand(selectQueryBox, connectionMySQL);
                using MySqlDataReader readerBox = selectCommandBox.ExecuteReader();
                int totalRowsBox = 0;
                while (readerBox.Read())
                {
                    //int Id = Convert.ToInt32(reader["Id"]);
                    string name = readerBox["Name"].ToString();
                    string articleBox = readerBox["Article"].ToString();
                    string systemBox = readerBox["System"].ToString();
                    string Type_of_opening = readerBox["Type_of_opening"].ToString();
                    string lenght = readerBox["Length"].ToString();
                    string height = readerBox["Height"].ToString();
                    string color = readerBox["Color"].ToString();
                    string railing = readerBox["Railing"].ToString();
                    string inner_drawer = readerBox["Inner_drawer"].ToString();
                    string washing = readerBox["Washing"].ToString();

                    string insertQueryBox = $"INSERT OR REPLACE INTO BoxeFirmaxTable (Name, Article, System, Type_of_opening, Length, Height, Color, Railing, Inner_drawer, Washing) VALUES ('{name}', '{articleBox}', '{systemBox}', '{Type_of_opening}', '{lenght}', '{height}', '{color}', '{railing}', '{inner_drawer}', '{washing}')";
                    using SQLiteCommand insertCommandBox = new SQLiteCommand(insertQueryBox, connectionSQLite);
                    insertCommandBox.ExecuteNonQuery();
                    totalRowsBox++;
                    System.Threading.Thread.Sleep(168); // Для имитации процесса
                    UpdateProgress(progressDialog, totalRowsBox);
                }

                connectionMySQLBox.Close();
                connectionSQLiteBox.Close();
            });
            progressDialog.Close();*/
        }

        //Отображение прогрессбара во время обновления базы
        private void UpdateProgress(ProgressDialogWindow progressDialog, int value)
        {
            progressDialog.Dispatcher.Invoke(() =>
            {
                progressDialog.progressBar.Value = value;
                progressDialog.progressText.Text = $"Progress: {value}%";
            });
        }
        private void ButtonEditor_Click(object sender, RoutedEventArgs e)
        {
            WindowPassword windowPassword = new WindowPassword();
            windowPassword.Show();
            this.Close();
        }

        private void ButtonCalculation_Click(object sender, RoutedEventArgs e)
        {
            CalculationWindow calculationWindow = new CalculationWindow();
            calculationWindow.Show();
            this.Close();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //workGoogleDrive.DeleteFile("123");
        }

        private void ButtonBoxCalculation_Click(object sender, RoutedEventArgs e)
        {
            BoxCalculation boxCalculation = new BoxCalculation();
            boxCalculation.Show();
            this.Close();
        }

        private void ButtonFeedback_Click(object sender, RoutedEventArgs e)
        {
            FeedbackWindow feedbackWindow = new FeedbackWindow();
            feedbackWindow.Show();
            //this.Close();
        }
    }
}