using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TBMFurn;

namespace TBMFurn
{
    // Конвертер для преобразования bool в "Да"/"Нет"
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Да" : "Нет";
            }
            return "Нет";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ExcelReplacer : Window
    {
        private ObservableCollection<UserItem> UserItems { get; set; }
        private ObservableCollection<FinalItem> FinalItems { get; set; }
        private Dictionary<string, CatalogItem> Catalog { get; set; }

        // Настройки Supabase - ЗАМЕНИТЕ НА СВОИ!
        private const string SUPABASE_URL = "https://kajvthlrnayyimrwnyqp.supabase.co";
        private const string SUPABASE_API_KEY = "sb_publishable_NZcAD8vZMM-j0QX-IQbusA_QlB3BRLF";

        private static readonly HttpClient httpClient = new HttpClient();

        public ExcelReplacer()
        {
            InitializeComponent();
            UserItems = new ObservableCollection<UserItem>();
            FinalItems = new ObservableCollection<FinalItem>();
            UserDataGrid.ItemsSource = UserItems;
            FinalDataGrid.ItemsSource = FinalItems;

            InitializeSupabase();
        }

        private async void InitializeSupabase()
        {
            try
            {
                await LoadCatalogFromSupabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД: {ex.Message}\nИспользую локальный каталог",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadLocalCatalog();
            }
        }

        private async Task LoadCatalogFromSupabase()
        {
            try
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("apikey", SUPABASE_API_KEY);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {SUPABASE_API_KEY}");

                var response = await httpClient.GetAsync($"{SUPABASE_URL}/rest/v1/catalog_replacements?select=*");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var items = JsonSerializer.Deserialize<List<SupabaseCatalogItem>>(json);

                    Catalog = new Dictionary<string, CatalogItem>();
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            Catalog[item.old_article] = new CatalogItem
                            {
                                ReplacementArticle = item.replacement_article,
                                QuantityFactor = item.quantity_factor
                            };
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Загружено {Catalog.Count} записей из БД");
                    MessageBox.Show($"Загружено {Catalog.Count} записей из БД", "Просто нажми OK",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {response.StatusCode} - {error}");
                    throw new Exception($"HTTP Error: {response.StatusCode} - {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки каталога: {ex.Message}");
                throw;
            }
        }

        private void LoadLocalCatalog()
        {
            string catalogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ExcelComparer", "catalog.json");

            try
            {
                if (File.Exists(catalogPath))
                {
                    string json = File.ReadAllText(catalogPath);
                    Catalog = JsonSerializer.Deserialize<Dictionary<string, CatalogItem>>(json) ?? new Dictionary<string, CatalogItem>();
                }
                else
                {
                    Catalog = new Dictionary<string, CatalogItem>();
                    // Пример для демонстрации
                    Catalog["OLD001"] = new CatalogItem { ReplacementArticle = "NEW001", QuantityFactor = 1.5m };
                    Catalog["OLD002"] = new CatalogItem { ReplacementArticle = "NEW002", QuantityFactor = 2.0m };
                    Catalog["641125"] = new CatalogItem { ReplacementArticle = "641125NEW", QuantityFactor = 1.0m };
                    SaveLocalCatalog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки локального каталога: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Catalog = new Dictionary<string, CatalogItem>();
            }
        }

        private void SaveLocalCatalog()
        {
            try
            {
                string catalogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ExcelComparer", "catalog.json");
                string directory = Path.GetDirectoryName(catalogPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string json = JsonSerializer.Serialize(Catalog, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(catalogPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения локального каталога: {ex.Message}");
            }
        }

        private void BtnLoadExcel_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                LoadFromExcel(openFileDialog.FileName);
            }
        }

        private void BtnPasteFromClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string clipboardText = Clipboard.GetText();
                if (string.IsNullOrWhiteSpace(clipboardText))
                {
                    MessageBox.Show("Буфер обмена пуст");
                    return;
                }

                var lines = clipboardText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                UserItems.Clear();

                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        if (decimal.TryParse(parts[0].Trim(), out decimal quantity))
                        {
                            UserItems.Add(new UserItem
                            {
                                Quantity = quantity,
                                Article = parts[1].Trim()
                            });
                        }
                    }
                }

                if (UserItems.Count > 0)
                {
                    ProcessData();
                    MessageBox.Show($"Загружено {UserItems.Count} строк из буфера обмена");
                }
                else
                {
                    MessageBox.Show("Не удалось распознать данные в буфере обмена. Убедитесь, что это таблица с двумя колонками.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка вставки из буфера: {ex.Message}");
            }
        }

        private void LoadFromExcel(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = null;

                    if (filePath.EndsWith(".xls"))
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    else
                    {
                        workbook = new XSSFWorkbook(fs);
                    }

                    var sheet = workbook.GetSheetAt(0);
                    if (sheet.LastRowNum == 0)
                    {
                        MessageBox.Show("Excel файл пуст");
                        return;
                    }

                    UserItems.Clear();

                    for (int row = 1; row <= sheet.LastRowNum; row++)
                    {
                        var dataRow = sheet.GetRow(row);
                        if (dataRow != null)
                        {
                            var quantityCell = dataRow.GetCell(0);
                            var articleCell = dataRow.GetCell(1);

                            if (quantityCell != null && articleCell != null)
                            {
                                string quantityStr = quantityCell.ToString();
                                if (decimal.TryParse(quantityStr, out decimal quantity))
                                {
                                    UserItems.Add(new UserItem
                                    {
                                        Quantity = quantity,
                                        Article = articleCell.ToString().Trim()
                                    });
                                }
                            }
                        }
                    }

                    ProcessData();
                    MessageBox.Show($"Загружено {UserItems.Count} строк из Excel");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки Excel: {ex.Message}");
            }
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            ProcessData();
        }

        private void ProcessData()
        {
            if (UserItems.Count == 0)
            {
                MessageBox.Show("Нет данных для обработки");
                return;
            }

            // Сначала суммируем дубликаты из пользовательского списка
            var summedItems = UserItems
                .GroupBy(x => x.Article)
                .Select(g => new UserItem { Article = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            // Применяем замены из каталога
            var processedItems = new Dictionary<string, FinalItem>();

            foreach (var item in summedItems)
            {
                string finalArticle = item.Article;
                decimal finalQuantity = item.Quantity;
                bool isReplaced = false;

                if (Catalog.ContainsKey(item.Article))
                {
                    var catalogItem = Catalog[item.Article];
                    finalArticle = catalogItem.ReplacementArticle;
                    finalQuantity = item.Quantity * catalogItem.QuantityFactor;
                    isReplaced = true;
                }

                // Округляем до целого числа
                finalQuantity = Math.Round(finalQuantity, 0);

                if (processedItems.ContainsKey(finalArticle))
                {
                    processedItems[finalArticle].Quantity += finalQuantity;
                    processedItems[finalArticle].IsReplaced = processedItems[finalArticle].IsReplaced || isReplaced;
                }
                else
                {
                    processedItems[finalArticle] = new FinalItem
                    {
                        Article = finalArticle,
                        Quantity = finalQuantity,
                        IsReplaced = isReplaced
                    };
                }
            }

            FinalItems.Clear();
            foreach (var item in processedItems.Values)
            {
                FinalItems.Add(item);
            }

            MessageBox.Show($"Обработано {UserItems.Count} строк. Получено {FinalItems.Count} уникальных артикулов.");
        }

        private void BtnSaveExcel_Click(object sender, RoutedEventArgs e)
        {
            if (FinalItems.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения. Сначала загрузите данные и обработайте их.");
                return;
            }

            string savePath = GetSavePath();
            SaveToExcel(savePath);
        }

        private string GetSavePath()
        {
            string driveXPath = @"X:\Подгрузка в КИС";
            string driveCPath = @"C:\Подгрузка в КИС";

            bool isDriveXAvailable = false;
            try
            {
                isDriveXAvailable = Directory.Exists("X:\\");
            }
            catch { }

            if (isDriveXAvailable)
            {
                if (!Directory.Exists(driveXPath))
                    Directory.CreateDirectory(driveXPath);
                return Path.Combine(driveXPath, $"export_{DateTime.Now:yyyyMMdd_HHmmss}.xls");
            }
            else
            {
                if (!Directory.Exists(driveCPath))
                    Directory.CreateDirectory(driveCPath);
                return Path.Combine(driveCPath, $"export_{DateTime.Now:yyyyMMdd_HHmmss}.xls");
            }
        }

        private void SaveToExcel(string filePath)
        {
            try
            {
                // Создаем новую книгу Excel в формате .xls
                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Результат");

                // Данные - без заголовков, только артикул и количество
                for (int i = 0; i < FinalItems.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i);

                    // Колонка A: Артикул
                    dataRow.CreateCell(0).SetCellValue(FinalItems[i].Article);

                    // Колонка B: Количество (округляем до целого)
                    long roundedQuantity = (long)Math.Round(FinalItems[i].Quantity, 0);
                    dataRow.CreateCell(1).SetCellValue(roundedQuantity);
                }

                // Автоматически подгоняем ширину колонок
                for (int i = 0; i < 2; i++)
                {
                    sheet.AutoSizeColumn(i);
                }

                // Сохраняем файл
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }

                MessageBox.Show($"Файл успешно сохранен: {filePath}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения Excel: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditCatalog_Click(object sender, RoutedEventArgs e)
        {
            var passwordDialog = new PasswordDialog();
            passwordDialog.Owner = this;

            if (passwordDialog.ShowDialog() == true && passwordDialog.IsPasswordCorrect)
            {
                CatalogEditorWindow editor = new CatalogEditorWindow(Catalog, SUPABASE_URL, SUPABASE_API_KEY);
                editor.Owner = this;
                if (editor.ShowDialog() == true)
                {
                    _ = LoadCatalogFromSupabase();
                    ProcessData();
                }
            }
        }
    }

    // Модели данных
    public class UserItem : INotifyPropertyChanged
    {
        private decimal _quantity;
        private string _article;

        public decimal Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(nameof(Quantity)); }
        }

        public string Article
        {
            get => _article;
            set { _article = value; OnPropertyChanged(nameof(Article)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class FinalItem : INotifyPropertyChanged
    {
        private decimal _quantity;
        private string _article;
        private bool _isReplaced;

        public decimal Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(nameof(Quantity)); }
        }

        public string Article
        {
            get => _article;
            set { _article = value; OnPropertyChanged(nameof(Article)); }
        }

        public bool IsReplaced
        {
            get => _isReplaced;
            set { _isReplaced = value; OnPropertyChanged(nameof(IsReplaced)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class CatalogItem
    {
        public string ReplacementArticle { get; set; }
        public decimal QuantityFactor { get; set; }
    }

    // Модель для Supabase
    public class SupabaseCatalogItem
    {
        public int id { get; set; }
        public string old_article { get; set; }
        public string replacement_article { get; set; }
        public decimal quantity_factor { get; set; }
    }
}