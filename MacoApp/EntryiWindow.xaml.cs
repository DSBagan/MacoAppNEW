using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
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
using System.Net.NetworkInformation;
using Hardcodet.Wpf.TaskbarNotification;

namespace MacoApp
{
    public partial class EntryiWindow : Window
    {
        static string pathDelBD = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString() + "\\Furnapp.db";
        //Путь к БД
        static string path2 = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();

        static string path = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString() + "\\Furnapp.db";

        //Создаем коллекцию лого
        private ObservableCollection<BitmapImage> backgroundsLogo = new ObservableCollection<BitmapImage>();
        Uri uri;

        private TaskbarIcon _notifyIcon;
        private PortalWindow _secondWindow1;
        private WindowAntipanic _secondWindow2;
        private BoxCalculation _secondWindow3;
        private CalculationWindow _secondWindow4;

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

            //CopyBD();

            InitTasks(); //Запуск метода удаления старых версий после обновления
        }

        private async void UpgradeBD()
        {
            if (Directory.Exists(@"X:\aTBMFURN\"))
            {
                string[] files = Directory.GetFiles(@"X:\aTBMFURN\");
                foreach (string file in files)
                {
                    // Удаление папки c сохраненными расчетами и всех ее подпапок и файлов
                    File.Delete(file);
                } 
            }
            if (Directory.Exists(@"C:\aTBMFURN\"))
            {
                string[] files = Directory.GetFiles(@"C:\aTBMFURN\");
                foreach (string file in files)
                {
                    // Удаление папки c сохраненными расчетами и всех ее подпапок и файлов
                    File.Delete(file);
                }
            }
            try
            {
                // Проверка доступности хотя бы одного известного хоста
                Ping ping = new Ping();
                PingReply replyGoogle = ping.Send("www.google.com", 1000); // Пингуем google.com
                PingReply replyYandex = ping.Send("www.yandex.ru", 1000); // Пингуем yandex.ru
                PingReply replyMail = ping.Send("www.mail.ru", 1000); // Пингуем mail.ru
                PingReply replyWikipedia = ping.Send("www.wikipedia.org", 1000); // Пингуем wikipedia.org

                if ((replyGoogle.Status == IPStatus.Success) || (replyYandex.Status == IPStatus.Success) || (replyMail.Status == IPStatus.Success) || (replyWikipedia.Status == IPStatus.Success))
                 {
                     //string pathBD = @"SaveDB\Furnapp.db";
                     FileInfo fileInf = new FileInfo(path);
                     if (fileInf.Exists)
                     {
                         fileInf.Delete();
                         //File.Copy(pathBD, path2); //Копируем в новую папку БД. чтобы оттуда скопировать в Google Drive
                     }
                     else
                     {
                         //File.Copy(pathBD, path2); //Копируем в новую папку БД. чтобы оттуда скопировать в Google Drive
                     }

                     //CopyBD();

                     //Качаем БД с Google Drive
                     WebClient webClient = new WebClient();
                     webClient.DownloadFile("https://drive.google.com/uc?export=download&id=1AcuvhDkjaS2aRt_qlPPu4piQ2Xx8j21D", path);
                     webClient.Dispose();
                 }
                 else 
                 {
                     return;
                 }
            }
            catch
            {
                
            }
            
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
                string createTableQuery = "CREATE TABLE IF NOT EXISTS Elements (Id INTEGER PRIMARY KEY, Name_Furn TEXT, Title TEXT, Article TEXT, Quantity INTEGER, System TEXT, Side TEXT, FFH_before INTEGER, FFH_after INTEGER, FFB_before INTEGER, FFB_after INTEGER, Lower_loop TEXT, Micro_ventilation TEXT, Rotation TEXT, Framuga TEXT, Wood TEXT, Konst TEXT, Shtulp TEXT, Color TEXT)";
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
                    string shtulp = reader["Shtulp"].ToString();
                    string color = reader["Color"].ToString();

                    string insertQuery = $"INSERT OR REPLACE INTO Elements (Name_Furn, Title, Article, Quantity, System, Side, FFH_before, FFH_after, FFB_before, FFB_after, Lower_loop, Micro_ventilation, Rotation, Framuga, Wood, Konst, Shtulp, Color) VALUES ('{nameFurn}', '{title}', '{article}', {quantity}, '{system}', '{side}', '{ffhBefore}', '{ffhAfter}', '{ffbBefore}', '{ffbAfter}', '{lowerLoop}', '{microVentilation}', '{rotation}', '{framuga}', '{wood}', '{konst}', '{shtulp}', '{color}')";
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

        private async void InitTasks()
        {
            try
            {
                await Task.Run(() => CleanOldVersions());
            }
            catch (Exception ex)
            {
                //Error handling
            }
        }


        //Копирование базы данных из папки в корень проекта
        private void CopyBD()
        {
            /*// Создать объект DirectoryInfo для папки Save
            DirectoryInfo saveDBDirectory = new DirectoryInfo("Save");

            // Получить путь к файлу в папке Save
            string filePath = Path.Combine(saveDBDirectory.FullName, "Furnapp.db");

            try
            {
                FileInfo fileInf = new FileInfo(filePath);
                if (fileInf.Exists)
                {
                    fileInf.Delete();
                    File.Copy(filePath, path2); //Копируем в новую папку БД.
                }
                else
                {
                    fileInf.CopyTo(path2, true); //Копируем в новую папку БД.
                }
            }
            catch 
            {
                return;
            }*/

            // Создать объект DirectoryInfo для папки Save

            // Получить путь к файлу в папке Save


            // Получить путь к исполняемому файлу приложения
            string appPath = Assembly.GetExecutingAssembly().Location;

            // Удалить имя исполняемого файла, чтобы получить путь к рабочему каталогу
            string workingDirectoryPath = Path.GetDirectoryName(appPath);

            // Перейти на три ступени выше по каталогам
            string parentDirectoryPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(workingDirectoryPath)));

            // Определить относительный путь к папке Save
            string saveDBRelativePath = @"Save";

            // Объединить пути
            string saveDBPath = Path.Combine(parentDirectoryPath, saveDBRelativePath);

            // Определить относительный путь к нужному файлу в папке Save
            string relativeFilePath = @"Furnapp.db";

            // Объединить пути
            string filePath = Path.Combine(saveDBPath, relativeFilePath);

            try
            {
                FileInfo fileInf = new FileInfo(pathDelBD);

                if (fileInf.Exists)
                {
                    fileInf.Delete();
                    File.Copy(filePath, pathDelBD); //Копируем в новую папку БД.
                }
                else
                {
                    File.Copy(filePath, pathDelBD); //Копируем в новую папку БД.
                }
            }
            catch
            {
                return;
            }
        }


        //Удаление старых версий программы
        public static void CleanOldVersions()
        {

            string path = AppDomain.CurrentDomain.BaseDirectory;
            int lastSlash = path.LastIndexOf(@"\");
            path = path.Substring(0, lastSlash);
            lastSlash = path.LastIndexOf(@"\");
            path = path.Substring(0, lastSlash);

            var dirInfo = new DirectoryInfo(path);

            var directories = dirInfo.EnumerateDirectories()
                                        .OrderByDescending(d => d.CreationTime)
                                        .ToList();

            List<string> DeletedAppIDs = new List<string>();

            foreach (DirectoryInfo subDirInfo in directories)
            {

                int first_ = subDirInfo.Name.IndexOf("_");
                if (first_ < 0) continue;
                string appID = subDirInfo.Name.Substring(first_ + 1, 21);

                if (DeletedAppIDs.Contains(appID)) continue;

                var subdirectories = subDirInfo.Parent.EnumerateDirectories()
                                            .Where(d => d.Name.Contains(appID))
                                            .OrderByDescending(d => d.CreationTime)
                                            .ToList();

                bool isNewest = true;
                foreach (DirectoryInfo subDirName in subdirectories)
                {
                    if (isNewest)
                    {
                        isNewest = false;
                    }
                    else
                    {
                        try
                        {
                            SetAttributesToNormal(subDirName);
                            subDirName.Delete(true);

                            if (!DeletedAppIDs.Contains(appID))
                            {
                                DeletedAppIDs.Add(appID);
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            
                        }
                    }
                }
            }
        }

        private static void SetAttributesToNormal(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
                SetAttributesToNormal(subDir);
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
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


        //Сворачиваем в трей окно входа при выборе одного из калькуляторов

        private void Window_Closed(object sender, System.EventArgs e)
        {
            this.Show();
        }



        private void ButtonEditor_Click(object sender, RoutedEventArgs e)
        {
            WindowPassword windowPassword = new WindowPassword();
            windowPassword.Show();
            this.Close();
        }

        private void ButtonCalculation_Click(object sender, RoutedEventArgs e)
        {
            /*CalculationWindow calculationWindow = new CalculationWindow();
            calculationWindow.Show();
            this.Close();*/
            ShowCalculationWindow();
        }
        private void ShowCalculationWindow()
        {
            _secondWindow4 = new CalculationWindow();
            _secondWindow4.Closed += Window_Closed;
            _secondWindow4.Show();
            this.Hide();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonBoxCalculation_Click(object sender, RoutedEventArgs e)
        {
            /*BoxCalculation boxCalculation = new BoxCalculation();
            boxCalculation.Show();
            this.Close();*/
            ShowBoxCalculation();
        }
        private void ShowBoxCalculation()
        {
            _secondWindow3 = new BoxCalculation();
            _secondWindow3.Closed += Window_Closed;
            _secondWindow3.Show();
            this.Hide();
        }

        private void ButtonFeedback_Click(object sender, RoutedEventArgs e)
        {
            FeedbackWindow feedbackWindow = new FeedbackWindow();
            feedbackWindow.Show();
            //this.Close();
        }

        private void ButtonAntipanic_Click(object sender, RoutedEventArgs e)
        {
            /*WindowAntipanic windowAntipanic = new WindowAntipanic();
            windowAntipanic.Show();
            this.Close();*/
            ShowWindowAntipanic();
        }
        private void ShowWindowAntipanic()
        {
            _secondWindow2 = new WindowAntipanic();
            _secondWindow2.Closed += Window_Closed;
            _secondWindow2.Show();
            this.Hide();
        }

        private void ButtonPortalCalculation_Click(object sender, RoutedEventArgs e)
        {
            /*PortalWindow portalWindow = new PortalWindow();
            portalWindow.Show();
            this.Close();*/
            ShowPortalWindow();
        }
        private void ShowPortalWindow()
        {
            _secondWindow1 = new PortalWindow();
            _secondWindow1.Closed += Window_Closed;
            _secondWindow1.Show();
            this.Hide();
        }

    }
}