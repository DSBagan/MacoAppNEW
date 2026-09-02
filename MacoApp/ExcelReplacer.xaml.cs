using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

                /*// Показываем окно с результатом
                MessageBox.Show($"Загружено записей: {Catalog?.Count ?? 0}", "Отладка", MessageBoxButton.OK, MessageBoxImage.Information);
                */
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
                    //MessageBox.Show($"Загружено {UserItems.Count} строк из буфера обмена");
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
        /// Расчет количества для уплотнителя по заданным правилам
        /// </summary>
        private decimal CalculateSealQuantity(decimal originalQuantity, decimal shippingStandard, string article = "")
        {
            // ИСКЛЮЧЕНИЕ ДЛЯ КОНКРЕТНОГО АРТИКУЛА
            if (article == "ALM770071-02/1" || article == "ALM770071-02")
            {
                decimal result = Math.Ceiling(originalQuantity / 35) * 35;
                System.Diagnostics.Debug.WriteLine($"ИСКЛЮЧЕНИЕ: Артикул {article} → округлен до {result} (кратно 35)");
                return result;
            }

            if (shippingStandard <= 0)
                return originalQuantity;

            // Если количество больше нормы отгрузки
            if (originalQuantity > shippingStandard)
            {
                // Разбиваем на целые нормы и остаток
                decimal fullStandards = Math.Floor(originalQuantity / shippingStandard);
                decimal remainder = originalQuantity % shippingStandard;

                decimal result = 0;

                // Целые нормы оставляем без изменений
                result += fullStandards * shippingStandard;

                // К остатку применяем правила
                if (remainder > 0)
                {
                    // Условие 1: Остаток более 2/3 нормы отгрузки
                    if (remainder > (shippingStandard * 2 / 3))
                    {
                        result += shippingStandard;
                        System.Diagnostics.Debug.WriteLine($"Уплотнитель: остаток {remainder} > 2/3 нормы, добавляем полную норму");
                    }
                    // Условие 2: Остаток от 10% до 2/3 нормы
                    else if (remainder > (shippingStandard * 0.1m))
                    {
                        decimal increase = remainder * 0.1m;
                        decimal newRemainder = remainder + increase;

                        if (originalQuantity >= 20)
                        {
                            newRemainder = Math.Ceiling(newRemainder / 5) * 5;
                        }
                        result += newRemainder;
                        System.Diagnostics.Debug.WriteLine($"Уплотнитель: остаток {remainder} между 10% и 2/3 нормы, увеличили на 10% → {newRemainder}");
                    }
                    // Условие 3: Остаток менее 10% нормы
                    else
                    {
                        decimal increase = remainder * 0.1m;
                        decimal newRemainder = remainder + increase;

                        if (originalQuantity >= 20)
                        {
                            newRemainder = Math.Ceiling(newRemainder / 5) * 5;
                        }
                        result += newRemainder;
                        System.Diagnostics.Debug.WriteLine($"Уплотнитель: остаток {remainder} < 10% нормы, увеличили на 10% → {newRemainder}");
                    }
                }

                return result;
            }
            else
            {
                // Исходная логика для количества <= нормы
                decimal remainder = originalQuantity % shippingStandard;

                if (remainder > (shippingStandard * 2 / 3))
                {
                    return originalQuantity + (shippingStandard - remainder);
                }
                else if (remainder > (shippingStandard * 0.1m))
                {
                    decimal increase = originalQuantity * 0.1m;
                    decimal result = originalQuantity + increase;

                    if (originalQuantity >= 20)
                    {
                        result = Math.Ceiling(result / 5) * 5;
                    }
                    return result;
                }
                else
                {
                    decimal increase = originalQuantity * 0.1m;
                    decimal result = originalQuantity + increase;

                    if (originalQuantity >= 20)
                    {
                        result = Math.Ceiling(result / 5) * 5;
                    }
                    return result;
                }
            }
        }

        /// Расчет количества для крепежа (округление до сотни вверх)
        /// </summary>
        private decimal CalculateFastenerQuantity(decimal originalQuantity)
        {
            if (originalQuantity <= 0)
                return 0;

            decimal result = Math.Ceiling(originalQuantity / 100) * 100;
            System.Diagnostics.Debug.WriteLine($"Крепеж: {originalQuantity} → округлено до сотен: {result}");
            return result;
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

            // Собираем информацию о дубликатах ДО группировки
            var duplicateGroups = UserItems
                .GroupBy(x => x.Article)
                .Where(g => g.Count() > 1)
                .Select(g => new
                {
                    Article = g.Key,
                    Count = g.Count(),
                    TotalQuantity = g.Sum(x => x.Quantity),
                    OriginalQuantities = string.Join(", ", g.Select(x => x.Quantity.ToString("N0")))
                })
                .ToList();

            var summedItems = UserItems
                .GroupBy(x => x.Article)
                .Select(g => new UserItem { Article = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            var processedItems = new Dictionary<string, FinalItem>();

            foreach (var item in summedItems)
            {
                string finalArticle = item.Article;
                decimal finalQuantity = item.Quantity;
                bool isReplaced = false;
                string replacementInfo = "Нет";

                // Сохраняем свойства исходного артикула (если они есть)
                bool isSeal = false;
                bool isFastener = false;
                decimal shippingStandard = 0;
                decimal originalQuantity = item.Quantity;

                if (Catalog.ContainsKey(item.Article))
                {
                    var catalogItem = Catalog[item.Article];
                    finalArticle = catalogItem.ReplacementArticle;
                    finalQuantity = item.Quantity * catalogItem.QuantityFactor;
                    isReplaced = true;

                    // Сохраняем свойства ИСХОДНОГО артикула
                    isSeal = catalogItem.IsSeal;
                    isFastener = catalogItem.IsFastener;
                    shippingStandard = catalogItem.ShippingStandard;

                    System.Diagnostics.Debug.WriteLine($"\n--- Обработка артикула: {item.Article} → замена: {finalArticle}");
                    System.Diagnostics.Debug.WriteLine($"  Исходное количество: {finalQuantity}");
                    System.Diagnostics.Debug.WriteLine($"  IsSeal: {isSeal}, IsFastener: {isFastener}, Норма: {shippingStandard}");
                }
                else
                {
                    // Если артикул не найден в каталоге, проверяем его свойства (вдруг он есть как артикул-замена?)
                    if (Catalog.ContainsKey(finalArticle))
                    {
                        var catalogItem = Catalog[finalArticle];
                        isSeal = catalogItem.IsSeal;
                        isFastener = catalogItem.IsFastener;
                        shippingStandard = catalogItem.ShippingStandard;
                        System.Diagnostics.Debug.WriteLine($"\n--- Обработка артикула: {finalArticle}");
                        System.Diagnostics.Debug.WriteLine($"  IsSeal: {isSeal}, IsFastener: {isFastener}, Норма: {shippingStandard}");
                    }
                }

                // Применяем расчеты на основе сохраненных свойств
                if (isFastener)
                {
                    finalQuantity = CalculateFastenerQuantity(finalQuantity);
                    System.Diagnostics.Debug.WriteLine($"  Артикул (или его замена) является крепежом, применено округление: {finalQuantity}");
                    replacementInfo = $"Крепеж: {originalQuantity} → {finalQuantity}";
                }
                else if (isSeal && shippingStandard > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"  === ПРИМЕНЕНИЕ ПРАВИЛ УПЛОТНИТЕЛЯ ===");
                    System.Diagnostics.Debug.WriteLine($"  Артикул: {item.Article} (замена: {finalArticle})");
                    System.Diagnostics.Debug.WriteLine($"  Количество до расчета: {finalQuantity}");
                    System.Diagnostics.Debug.WriteLine($"  Норма отгрузки: {shippingStandard}");

                    finalQuantity = CalculateSealQuantity(finalQuantity, shippingStandard, item.Article);

                    replacementInfo = $"В заявке: {originalQuantity}, округлил до: {finalQuantity}";

                    System.Diagnostics.Debug.WriteLine($"  Итоговое количество: {finalQuantity}");
                }
                else if (isReplaced)
                {
                    replacementInfo = "Да";
                }

                finalQuantity = Math.Round(finalQuantity, 0);

                if (processedItems.ContainsKey(finalArticle))
                {
                    processedItems[finalArticle].Quantity += finalQuantity;
                    processedItems[finalArticle].IsReplaced = processedItems[finalArticle].IsReplaced || isReplaced;
                    if (isReplaced && processedItems[finalArticle].ReplacementInfo == "Нет")
                    {
                        processedItems[finalArticle].ReplacementInfo = replacementInfo;
                    }
                }
                else
                {
                    processedItems[finalArticle] = new FinalItem
                    {
                        Article = finalArticle,
                        Quantity = finalQuantity,
                        IsReplaced = isReplaced,
                        ReplacementInfo = replacementInfo
                    };
                }
            }

            FinalItems.Clear();
            foreach (var item in processedItems.Values)
            {
                FinalItems.Add(item);
            }

            UpdateRowNumbers();

            // Формируем сообщение о дубликатах
            string duplicateMessage = "";
            if (duplicateGroups.Count > 0)
            {
                duplicateMessage = "\n\n=== Объединенные дубликаты ===\n";
                foreach (var dup in duplicateGroups)
                {
                    duplicateMessage += $"Артикул: {dup.Article}\n";
                    duplicateMessage += $"  Вхождений: {dup.Count}\n";
                    duplicateMessage += $"  Суммарно: {dup.TotalQuantity:N0}\n";
                    duplicateMessage += $"  Значения: {dup.OriginalQuantities}\n\n";
                }
            }

            MessageBox.Show($"Обработано {UserItems.Count} строк. Получено {FinalItems.Count} уникальных артикулов.{duplicateMessage}");
        }

        private void BtnSaveTXT_Click(object sender, RoutedEventArgs e)
        {
            if (FinalItems.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения. Сначала загрузите данные и обработайте их.");
                return;
            }

            // Диалог для ввода кода фирмы
            string firmCode = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите шифр фирмы (например: 005677):",
                "Шифр фирмы",
                "005677",
                -1, -1);

            if (string.IsNullOrWhiteSpace(firmCode))
            {
                return; // Пользователь отменил ввод
            }

            string savePath = GetSavePathTXT(firmCode);
            SaveToTXT(savePath, firmCode);
        }

        private string GetSavePathTXT(string firmCode)
        {
            string driveXPath = @"X:\Подгрузка в КИС";
            string driveCPath = @"C:\Подгрузка в КИС";
            string date = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");

            bool isDriveXAvailable = false;
            try
            {
                isDriveXAvailable = Directory.Exists("X:\\");
            }
            catch { }

            string basePath = isDriveXAvailable ? driveXPath : driveCPath;

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            return Path.Combine(basePath, $"Z{firmCode}  {date}.txt");
        }

        private void SaveToTXT(string filePath, string firmCode)
        {
            try
            {
                string date = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");

                using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.Default))
                {
                    // Заголовок
                    streamWriter.WriteLine($"Шифр фирмы {firmCode}");
                    streamWriter.WriteLine("                    Фирма 123");
                    streamWriter.WriteLine("                    Заявка №");
                    streamWriter.WriteLine("                    Название");
                    streamWriter.WriteLine($"                    Дата заявки {date}");

                    streamWriter.WriteLine("--------------------------------------------------------------------------------");
                    streamWriter.WriteLine("    Артикул                       Название                      Кол.  Ед.изм.");
                    streamWriter.WriteLine("--------------------------------------------------------------------------------");

                    // Находим максимальную длину артикула
                    int maxArtLength = 16;
                    foreach (var item in FinalItems)
                    {
                        if (item.Article.Length > maxArtLength)
                            maxArtLength = item.Article.Length;
                    }
                    // Добавляем небольшой запас
                    maxArtLength += 2;

                    foreach (var item in FinalItems)
                    {
                        string art = item.Article;
                        int quantity = (int)Math.Round(item.Quantity, 0);

                        // Выравниваем до максимальной длины
                        art = art.PadRight(maxArtLength);

                        // Формируем отступы
                        string spaces = new string(' ', 48);
                        streamWriter.WriteLine(art + spaces + quantity);
                    }

                    streamWriter.WriteLine("--------------------------------------------------------------------------------");
                    streamWriter.WriteLine();
                    streamWriter.WriteLine("                    Заявку составил________________________");
                }

                MessageBox.Show($"Файл успешно сохранен:\n{filePath}\n\nЗаписей: {FinalItems.Count}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения файла: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        // Новая кнопка для сохранения в .xls
        private void BtnSaveExcelForKIS_Click(object sender, RoutedEventArgs e)
        {
            if (FinalItems.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения. Сначала загрузите данные и обработайте их.");
                return;
            }

            string savePath = GetSavePathForKIS();
            SaveToExcelOldFormat(savePath);
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

        // Новый метод для получения пути сохранения для КИС (.xls)
        private string GetSavePathForKIS()
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

        // Новый метод для сохранения в формате .xls
        private void SaveToExcelOldFormat(string filePath)
        {
            try
            {
                if (!filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    filePath = Path.ChangeExtension(filePath, ".xls");
                }

                IWorkbook workbook = new HSSFWorkbook();
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

                MessageBox.Show($"Файл успешно сохранен в формате .xls:\n{filePath}\n\nЗаписей: {FinalItems.Count}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения Excel (.xls): {ex.Message}", "Ошибка",
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
        private string _replacementInfo; // Новое свойство для информации о замене

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

        public string ReplacementInfo
        {
            get => _replacementInfo;
            set { _replacementInfo = value; OnPropertyChanged(nameof(ReplacementInfo)); }
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
        public bool IsFastener { get; set; } = false;
    }
}