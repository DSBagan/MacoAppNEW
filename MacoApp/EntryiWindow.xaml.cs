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
using MaterialDesignThemes.Wpf;
using System.Text.Json;

namespace MacoApp
{
    public partial class EntryiWindow : Window
    {
        static string pathDelBD = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString() + "\\Furnapp.db";
        static string path2 = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString();
        static string path = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.ToString() + "\\Furnapp.db";

        private ObservableCollection<BitmapImage> backgroundsLogo = new ObservableCollection<BitmapImage>();
        Uri uri;

        private TaskbarIcon _notifyIcon;
        private PortalWindow _secondWindow1;
        private WindowAntipanic _secondWindow2;
        private BoxCalculation _secondWindow3;
        private CalculationWindow _secondWindow4;
        private CalculationWindowAlu _secondWindow5;
        private ExcelReplacer _secondWindow6;

        // Локальная БД
        private LocalCatalogDatabase _localDb;

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

        private async void EntryiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonEditor.Visibility = Visibility.Collapsed;

            System.Windows.Application.Current.MainWindow.ShowActivated = true;

            for (int i = 0; i < backgroundsLogo.Count; i++)
            {
                Image img = new Image();
                img.Source = backgroundsLogo[i];
                stackPanelLogo.Children.Add(img);
            }

            // Очистка временных файлов
            if (Directory.Exists(@"X:\aTBMFURN\"))
            {
                string[] files = Directory.GetFiles(@"X:\aTBMFURN\");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
            if (Directory.Exists(@"C:\aTBMFURN\"))
            {
                string[] files = Directory.GetFiles(@"C:\aTBMFURN\");
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }

            // Создаем экземпляр LocalCatalogDatabase
            _localDb = new LocalCatalogDatabase();
            _localDb.StatusChanged += OnDatabaseStatusChanged;

            // Инициализируем базу данных
            await InitializeDatabaseAsync();

            InitTasks();
        }

        private void OnDatabaseStatusChanged(string statusMessage)
        {
            // Обновляем статус в UI
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateStatus(statusMessage);
            }));
        }

        private async Task InitializeDatabaseAsync()
        {
            try
            {
                // Получаем каталог из локальной БД
                var catalog = await _localDb.GetAllCatalogAsync();

                // Обновляем статус с информацией об источнике
                string sourceInfo = _localDb.GetSourceDescription();
                UpdateStatus($"База данных загружена, записей: {catalog.Count} | Источник: {sourceInfo}", "#FF2E7D32");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Ошибка инициализации базы данных: {ex.Message}", "#FFD32F2F");
            }
        }

        /// <summary>
        /// Обновление статуса в статус-баре
        /// </summary>
        private void UpdateStatus(string message, string colorHex = "#FF6B6B6B")
        {
            // Выполняем в UI потоке
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (TxtStatus != null)
                {
                    TxtStatus.Text = message;
                    TxtStatus.Foreground = (Brush)new BrushConverter().ConvertFrom(colorHex);

                    // Меняем иконку MaterialDesign в зависимости от типа сообщения
                    if (TxtStatusIcon != null)
                    {
                        if (message.Contains("Ошибка") || message.Contains("не удалось"))
                        {
                            TxtStatusIcon.Kind = PackIconKind.Error;
                            TxtStatusIcon.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFD32F2F");
                        }
                        else if (message.Contains("офлайн") || message.Contains("нет подключения") || message.Contains("недоступен"))
                        {
                            TxtStatusIcon.Kind = PackIconKind.CloudOffOutline;
                            TxtStatusIcon.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFAA6F00");
                        }
                        else if (message.Contains("обновлена") || message.Contains("актуальна") || message.Contains("скопирована") || message.Contains("загружена"))
                        {
                            TxtStatusIcon.Kind = PackIconKind.CloudCheck;
                            TxtStatusIcon.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF2E7D32");
                        }
                        else if (message.Contains("Сетевой диск") || message.Contains("сетевого диска"))
                        {
                            TxtStatusIcon.Kind = PackIconKind.ServerNetwork;
                            TxtStatusIcon.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF1976D2");
                        }
                        else if (message.Contains("локальную") || message.Contains("Локальная"))
                        {
                            TxtStatusIcon.Kind = PackIconKind.Laptop;
                            TxtStatusIcon.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF6B6B6B");
                        }
                        else if (message.Contains("Создана"))
                        {
                            TxtStatusIcon.Kind = PackIconKind.DatabasePlus;
                            TxtStatusIcon.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF2E7D32");
                        }
                        else
                        {
                            TxtStatusIcon.Kind = PackIconKind.CloudSync;
                            TxtStatusIcon.Foreground = (Brush)new BrushConverter().ConvertFrom("#FF6B6B6B");
                        }
                    }
                }
            }));

            System.Diagnostics.Debug.WriteLine(message);
        }

        private void ExtractDatabaseFromResources(string targetPath)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "MacoApp.Resources.Furnapp.db";

                Stream resourceStream = null;
                try
                {
                    resourceStream = assembly.GetManifestResourceStream(resourceName);

                    if (resourceStream == null)
                    {
                        resourceName = assembly.GetManifestResourceNames()
                            .FirstOrDefault(name => name.EndsWith("Furnapp.db"));

                        if (resourceName == null)
                            throw new FileNotFoundException("Встроенная база данных не найдена в ресурсах");

                        resourceStream = assembly.GetManifestResourceStream(resourceName);
                    }

                    using (FileStream fileStream = new FileStream(targetPath, FileMode.Create))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }
                finally
                {
                    resourceStream?.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                CreateEmptyDatabase(targetPath);
            }
        }

        private void CreateEmptyDatabase(string dbPath)
        {
            try
            {
                SQLiteConnection.CreateFile(dbPath);

                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    string[] createTables =
                    {
                        "CREATE TABLE IF NOT EXISTS Fittings (Id INTEGER PRIMARY KEY, Name TEXT, Price REAL)",
                        "CREATE TABLE IF NOT EXISTS Categories (Id INTEGER PRIMARY KEY, Name TEXT)",
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
                // Error handling
            }
        }

        // Удаление старых версий программы
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

        // Отображение прогрессбара во время обновления базы
        private void UpdateProgress(ProgressDialogWindow progressDialog, int value)
        {
            progressDialog.Dispatcher.Invoke(() =>
            {
                progressDialog.progressBar.Value = value;
                progressDialog.progressText.Text = $"Progress: {value}%";
            });
        }

        // Сворачиваем в трей окно входа при выборе одного из калькуляторов
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

        private void ButtonCalculationAlu_Click(object sender, RoutedEventArgs e)
        {
            ShowCalculationWindowAlu();
        }

        private void ShowCalculationWindowAlu()
        {
            _secondWindow5 = new CalculationWindowAlu();
            _secondWindow5.Closed += Window_Closed;
            _secondWindow5.Show();
            this.Hide();
        }

        private void ButtonCalculation_Click(object sender, RoutedEventArgs e)
        {
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
        }

        private void ButtonAntipanic_Click(object sender, RoutedEventArgs e)
        {
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
            ShowPortalWindow();
        }

        private void ShowPortalWindow()
        {
            _secondWindow1 = new PortalWindow();
            _secondWindow1.Closed += Window_Closed;
            _secondWindow1.Show();
            this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _secondWindow6 = new ExcelReplacer();
            _secondWindow6.Closed += Window_Closed;
            _secondWindow6.Show();
            this.Hide();
        }

        private void ButtonChangelog_Click(object sender, RoutedEventArgs e)
        {
            ChangelogWindow changelogWindow = new ChangelogWindow();
            changelogWindow.Owner = this;
            changelogWindow.ShowDialog();
        }
    }
}