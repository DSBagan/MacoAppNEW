using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Microsoft.Win32;
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
        private LocalCatalogDatabase _localDb;

        public ExcelReplacer()
        {
            InitializeComponent();
            UserItems = new ObservableCollection<UserItem>();
            FinalItems = new ObservableCollection<FinalItem>();
            UserDataGrid.ItemsSource = UserItems;
            FinalDataGrid.ItemsSource = FinalItems;

            Catalog = new Dictionary<string, CatalogItem>();
            _localDb = new LocalCatalogDatabase();

            // Подписываемся на событие изменения коллекций
            UserItems.CollectionChanged += OnUserItemsCollectionChanged;
            FinalItems.CollectionChanged += (s, e) => UpdateRowNumbers();

            this.Loaded += async (s, e) => await LoadCatalogAsync();
        }

        private async Task LoadCatalogAsync()
        {
            try
            {
                if (TxtStatus != null)
                    TxtStatus.Text = "Загрузка каталога из локальной БД...";

                Catalog = await _localDb.GetAllCatalogAsync();

                // ОТЛАДКА
                System.Diagnostics.Debug.WriteLine($"=== CATALOG COUNT = {Catalog?.Count ?? 0} ===");

                // Показываем окно с результатом
                MessageBox.Show($"Загружено записей: {Catalog?.Count ?? 0}", "Отладка", MessageBoxButton.OK, MessageBoxImage.Information);

                if (TxtStatus != null)
                    TxtStatus.Text = $"Загружено {Catalog.Count} записей из каталога";

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
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
                    UpdateRowNumbers();
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

        /// <summary>
        /// Расчет количества для уплотнителя по заданным правилам
        /// </summary>
        /// <param name="originalQuantity">Исходное количество</param>
        /// <param name="shippingStandard">Норма отгрузки</param>
        /// <returns>Рассчитанное количество после применения правил</returns>
        private decimal CalculateSealQuantity(decimal originalQuantity, decimal shippingStandard)
        {
            if (shippingStandard <= 0)
                return originalQuantity; // Если норма не задана, возвращаем как есть

            decimal remainder = originalQuantity % shippingStandard;

            // Условие 1: Остаток более 2/3 нормы отгрузки
            if (remainder > (shippingStandard * 2 / 3))
            {
                decimal result = originalQuantity + (shippingStandard - remainder);
                System.Diagnostics.Debug.WriteLine($"Уплотнитель: остаток {remainder} > 2/3 нормы ({shippingStandard * 2 / 3}), округляем до {result}");
                return result;
            }
            // Условие 2: Остаток от 10% до 2/3 нормы
            else if (remainder > (shippingStandard * 0.1m))
            {
                decimal increase = originalQuantity * 0.1m;
                decimal result = originalQuantity + increase;

                // Округляем до 5, если исходное количество >= 20
                if (originalQuantity >= 20)
                {
                    decimal oldResult = result;
                    result = Math.Ceiling(result / 5) * 5;
                    System.Diagnostics.Debug.WriteLine($"Уплотнитель: остаток {remainder} между 10% и 2/3 нормы, увеличили на 10%: {oldResult} → округлили до 5: {result}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Уплотнитель: остаток {remainder} между 10% и 2/3 нормы, увеличили на 10%: {result} (округление до 5 не применяется, т.к. количество < 20)");
                }

                return result;
            }
            // Условие 3: Остаток менее 10% нормы
            else
            {
                decimal increase = originalQuantity * 0.1m;
                decimal result = originalQuantity + increase;

                // Округляем до 5, если исходное количество >= 20
                if (originalQuantity >= 20)
                {
                    decimal oldResult = result;
                    result = Math.Ceiling(result / 5) * 5;
                    System.Diagnostics.Debug.WriteLine($"Уплотнитель: остаток {remainder} < 10% нормы, увеличили на 10%: {oldResult} → округлили до 5: {result}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Уплотнитель: остаток {remainder} < 10% нормы, увеличили на 10%: {result} (округление до 5 не применяется, т.к. количество < 20)");
                }

                return result;
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

                    UpdateRowNumbers();
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
                bool isSeal = false;
                decimal shippingStandard = 0;

                if (Catalog.ContainsKey(item.Article))
                {
                    var catalogItem = Catalog[item.Article];
                    finalArticle = catalogItem.ReplacementArticle;
                    finalQuantity = item.Quantity * catalogItem.QuantityFactor;
                    isReplaced = true;
                    isSeal = catalogItem.IsSeal;
                    shippingStandard = catalogItem.ShippingStandard;

                    // Если это уплотнитель и есть норма отгрузки, применяем специальный расчет
                    if (isSeal && shippingStandard > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"=== ОБРАБОТКА УПЛОТНИТЕЛЯ ===");
                        System.Diagnostics.Debug.WriteLine($"Артикул: {item.Article}");
                        System.Diagnostics.Debug.WriteLine($"Исходное количество: {finalQuantity}");
                        System.Diagnostics.Debug.WriteLine($"Норма отгрузки: {shippingStandard}");

                        finalQuantity = CalculateSealQuantity(finalQuantity, shippingStandard);

                        System.Diagnostics.Debug.WriteLine($"Итоговое количество: {finalQuantity}");
                    }
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

            UpdateRowNumbers();

            MessageBox.Show($"Обработано {UserItems.Count} строк. Получено {FinalItems.Count} уникальных артикулов.");
        }

        private void OnUserItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateRowNumbers();
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
                return Path.Combine(driveXPath, $"export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            else
            {
                if (!Directory.Exists(driveCPath))
                    Directory.CreateDirectory(driveCPath);
                return Path.Combine(driveCPath, $"export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
        }

        private void SaveToExcel(string filePath)
        {
            try
            {
                if (!filePath.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    filePath = Path.ChangeExtension(filePath, ".xlsx");
                }

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Результат");

                for (int i = 0; i < FinalItems.Count; i++)
                {
                    IRow dataRow = sheet.CreateRow(i);
                    dataRow.CreateCell(0).SetCellValue(FinalItems[i].Article);
                    long roundedQuantity = (long)Math.Round(FinalItems[i].Quantity, 0);
                    dataRow.CreateCell(1).SetCellValue(roundedQuantity);
                }

                sheet.AutoSizeColumn(0);
                sheet.AutoSizeColumn(1);

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }

                MessageBox.Show($"Файл успешно сохранен: {filePath}\n\nЗаписей: {FinalItems.Count}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения Excel: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditCatalog_Click(object sender, RoutedEventArgs e)
        {
            var passwordDialog = new PasswordDialog();
            passwordDialog.Owner = this;

            if (passwordDialog.ShowDialog() == true && passwordDialog.IsPasswordCorrect)
            {
                CatalogEditorWindow editor = new CatalogEditorWindow(Catalog);
                editor.Owner = this;
                if (editor.ShowDialog() == true)
                {
                    _ = LoadCatalogAsync();
                    ProcessData();
                }
            }
        }

        private void UpdateRowNumbers()
        {
            for (int i = 0; i < UserItems.Count; i++)
            {
                UserItems[i].RowNumber = i + 1;
            }

            for (int i = 0; i < FinalItems.Count; i++)
            {
                FinalItems[i].RowNumber = i + 1;
            }
        }
    }

    // Модели данных
    public class UserItem : INotifyPropertyChanged
    {
        private decimal _quantity;
        private string _article;
        private int _rowNumber;

        public int RowNumber
        {
            get => _rowNumber;
            set { _rowNumber = value; OnPropertyChanged(nameof(RowNumber)); }
        }

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
        private int _rowNumber;

        public int RowNumber
        {
            get => _rowNumber;
            set { _rowNumber = value; OnPropertyChanged(nameof(RowNumber)); }
        }

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
        public bool IsSeal { get; set; } = false;
        public decimal ShippingStandard { get; set; } = 0;
    }
}