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

namespace MacoApp
{
    public partial class EntryiWindow : Window
    {
        static string path = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString()+"\\Furnapp.db";
        

        public EntryiWindow()
        {
            InitializeComponent();       
            Loaded += EntryiWindow_Loaded;
            
        }
        

        private void EntryiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntOn.Visibility = Visibility.Collapsed;
            IntOff.Visibility = Visibility.Collapsed;
            ButtonEditor.Visibility = Visibility.Collapsed;
            ButtonExcel.Visibility = Visibility.Collapsed;
            UpgradeBD();

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

        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            LoadInExcelWindow loadExcelWindow = new LoadInExcelWindow();
            loadExcelWindow.Show();
            this.Close();
        }

        private void UpgradeBD()
        {
            if (Directory.Exists(@"X:\aTBMFURN\"))
            {
                string[] files = Directory.GetFiles(@"X:\aTBMFURN\");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                // Удаление папки и всех ее подпапок и файлов
            }

            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                fileInf.Delete();
            }
            //WebClient webClient = new WebClient();
            //Качаем БД с Google Drive

            //webClient.DownloadFile("https://drive.google.com/uc?export=download&id=18KBF6LMWrxoDqy8cUdEUaZYCXC_8SLPu", path);
            //webClient.Dispose();


            // Строки подключения к MySQL и SQLite
            //string mysqlConnectionString = "server=cz6.h.filess.io;user=BDFurnTBM_forcearmy;database=BDFurnTBM_forcearmy;port=3307;password=e5f1b53ca8619010c3c73fd972facf6e383871ed";
            //string sqliteConnectionString = "Data Source=Furnapp.db;";

            string connectionStringMySQL = "server=cz6.h.filess.io;user=BDFurnTBM_forcearmy;database=BDFurnTBM_forcearmy;port=3307;password=e5f1b53ca8619010c3c73fd972facf6e383871ed;";
            using MySqlConnection connectionMySQL = new MySqlConnection(connectionStringMySQL);
            connectionMySQL.Open();

            string connectionStringSQLite = "Data Source=Furnapp.db;";
            using SQLiteConnection connectionSQLite = new SQLiteConnection(connectionStringSQLite);
            connectionSQLite.Open();

            // Создание таблицы в базе данных SQLite
            string createTableQuery = "CREATE TABLE IF NOT EXISTS Elements (Id INTEGER PRIMARY KEY, Name_Furn TEXT, Title TEXT, Article TEXT, Quantity INTEGER, System TEXT, Side TEXT, FFH_before TEXT, FFH_after TEXT, FFB_before TEXT, FFB_after TEXT, Lower_loop TEXT, Micro_ventilation TEXT, Rotation TEXT, Framuga TEXT, Wood TEXT, Konst TEXT)";
            using SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connectionSQLite);
            createTableCommand.ExecuteNonQuery();

            // Извлечение данных из таблицы MySQL и вставка их в таблицу SQLite
            string selectQuery = "SELECT * FROM Elements";
            using MySqlCommand selectCommand = new MySqlCommand(selectQuery, connectionMySQL);
            using MySqlDataReader reader = selectCommand.ExecuteReader();

            while (reader.Read())
            {
                //int Id = Convert.ToInt32(reader["Id"]);
                string nameFurn = reader["Name_Furn"].ToString();
                string title = reader["Title"].ToString();
                string article = reader["Article"].ToString();
                int quantity = Convert.ToInt32(reader["Quantity"]);
                string system = reader["System"].ToString();
                string side = reader["Side"].ToString();
                string ffhBefore = reader["FFH_before"].ToString();
                string ffhAfter = reader["FFH_after"].ToString();
                string ffbBefore = reader["FFB_before"].ToString();
                string ffbAfter = reader["FFB_after"].ToString();
                string lowerLoop = reader["Lower_loop"].ToString();
                string microVentilation = reader["Micro_ventilation"].ToString();
                string rotation = reader["Rotation"].ToString();
                string framuga = reader["Framuga"].ToString();
                string wood = reader["Wood"].ToString();
                string konst = reader["Konst"].ToString();

                string insertQuery = $"INSERT INTO Elements (Name_Furn, Title, Article, Quantity, System, Side, FFH_before, FFH_after, FFB_before, FFB_after, Lower_loop, Micro_ventilation, Rotation, Framuga, Wood, Konst) VALUES ('{nameFurn}', '{title}', '{article}', {quantity}, '{system}', '{side}', '{ffhBefore}', '{ffhAfter}', '{ffbBefore}', '{ffbAfter}', '{lowerLoop}', '{microVentilation}', '{rotation}', '{framuga}', '{wood}', '{konst}')";
                using SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connectionSQLite);
                insertCommand.ExecuteNonQuery();
            }
            connectionMySQL.Close();
            connectionSQLite.Close();
        }
    }
}