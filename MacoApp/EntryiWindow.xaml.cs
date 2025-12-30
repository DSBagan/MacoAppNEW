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

            this.Title = "Калькулятор фурнитуры";

            Loaded += EntryiWindow_Loaded;
        }


        private void EntryiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntOn.Visibility = Visibility.Collapsed;
            IntOff.Visibility = Visibility.Collapsed;
            ButtonEditor.Visibility = Visibility.Collapsed;

            System.Windows.Application.Current.MainWindow.ShowActivated = true;

            for (int i = 0; i < backgroundsLogo.Count; i++)
            {
                Image img = new Image();
                img.Source = backgroundsLogo[i];
                stackPanelLogo.Children.Add(img);
            }
            //UpgradeBD();

            //CopyBD();
            
            InitializeDatabase(); // Инициализируем базу данных (без интернета!)
            InitTasks(); //Запуск метода удаления старых версий после обновления
        }

        /*private async void UpgradeBD()
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
                     webClient.DownloadFile("https://drive.google.com/file/d/1grOTFs196K1H6Z7P4uZkIepzJi4IcR1z/view?usp=sharing", path);
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
        }*/

        private void InitializeDatabase()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Furnapp.db");

            // Если база данных уже существует и не пуста - используем её
            if (File.Exists(dbPath) && new FileInfo(dbPath).Length > 0)
            {
                path = dbPath;
                return;
            }

            // Если базы нет - извлекаем из ресурсов
            ExtractDatabaseFromResources(dbPath);
            path = dbPath;
        }

        private void ExtractDatabaseFromResources(string targetPath)
        {
            try
            {
                // Получаем встроенный ресурс
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "MacoApp.Resources.Furnapp.db";

                Stream resourceStream = null;
                try
                {
                    // Пытаемся получить ресурс по ожидаемому имени
                    resourceStream = assembly.GetManifestResourceStream(resourceName);

                    if (resourceStream == null)
                    {
                        // Попробуем найти ресурс с другим именем
                        resourceName = assembly.GetManifestResourceNames()
                            .FirstOrDefault(name => name.EndsWith("Furnapp.db"));

                        if (resourceName == null)
                            throw new FileNotFoundException("Встроенная база данных не найдена в ресурсах");

                        resourceStream = assembly.GetManifestResourceStream(resourceName);
                    }

                    // Сохраняем на диск
                    using (FileStream fileStream = new FileStream(targetPath, FileMode.Create))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
                finally
                {
                    // Закрываем stream вручную, так как он не в using блоке
                    resourceStream?.Dispose();
                }

                MessageBox.Show("База данных успешно инициализирована!", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // Создаем пустую базу как запасной вариант
                CreateEmptyDatabase(targetPath);
            }
        }

        private void CreateEmptyDatabase(string dbPath)
        {
            try
            {
                // Создаем минимальную структуру базы данных SQLite
                SQLiteConnection.CreateFile(dbPath);

                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    // Создаем базовые таблицы (пример)
                    string[] createTables =
                    {
                "CREATE TABLE IF NOT EXISTS Fittings (Id INTEGER PRIMARY KEY, Name TEXT, Price REAL)",
                "CREATE TABLE IF NOT EXISTS Categories (Id INTEGER PRIMARY KEY, Name TEXT)",
                // Добавьте другие необходимые таблицы
            };

                    foreach (var sql in createTables)
                    {
                        using (var command = new SQLiteCommand(sql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось создать базу данных: {ex.Message}", "Критическая ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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