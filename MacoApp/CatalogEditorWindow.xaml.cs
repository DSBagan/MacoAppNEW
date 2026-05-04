using ICSharpCode.SharpZipLib.Zip;
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
using System.Windows.Controls;
using System.Windows.Data;
using TBMFurn;

namespace TBMFurn
{
    public partial class CatalogEditorWindow : Window
    {
        private ObservableCollection<CatalogEntry> CatalogEntries { get; set; }
        private Dictionary<string, CatalogItem> OriginalCatalog { get; set; }
        private string supabaseUrl;
        private string supabaseKey;
        private static readonly HttpClient httpClient = new HttpClient();
        private ICollectionView view;

        public CatalogEditorWindow(Dictionary<string, CatalogItem> catalog, string url, string key)
        {
            InitializeComponent();
            OriginalCatalog = catalog;
            supabaseUrl = url;
            supabaseKey = key;
            CatalogEntries = new ObservableCollection<CatalogEntry>();

            foreach (var item in catalog)
            {
                CatalogEntries.Add(new CatalogEntry
                {
                    OldArticle = item.Key,
                    NewArticle = item.Value.ReplacementArticle,
                    Factor = item.Value.QuantityFactor
                });
            }

            // Настраиваем привязку для поиска
            CatalogGrid.ItemsSource = CatalogEntries;
            view = CollectionViewSource.GetDefaultView(CatalogEntries);
            view.Filter = FilterPredicate;

            UpdateSearchResultCount();
        }

        private bool FilterPredicate(object item)
        {
            // Пропускаем null
            if (item == null)
                return false;

            // Проверяем тип объекта - это ключевое исправление!
            if (item.GetType().Name == "NamedObject")
                return false;

            // Проверяем, что это наш тип
            if (!(item is CatalogEntry))
                return false;

            var catalogItem = item as CatalogEntry;
            string searchText = TxtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
                return true;

            // Поиск по старому артикулу и артикулу замене (без учета регистра)
            return (catalogItem.OldArticle != null && catalogItem.OldArticle.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (catalogItem.NewArticle != null && catalogItem.NewArticle.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Обновляем фильтр при изменении текста поиска
            view?.Refresh();
            UpdateSearchResultCount();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Text = "";
            TxtSearch.Focus();
        }

        private void UpdateSearchResultCount()
        {
            int totalCount = CatalogEntries.Count;
            int filteredCount = 0;

            if (view != null)
            {
                try
                {
                    filteredCount = view.Cast<object>().Count(x => x is CatalogEntry);
                }
                catch
                {
                    filteredCount = totalCount;
                }
            }

            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                TxtSearchResult.Text = $"Всего записей: {totalCount}";
            }
            else
            {
                TxtSearchResult.Text = $"Найдено: {filteredCount} из {totalCount}";
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
                int addedCount = 0;

                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        string oldArticle = parts[0].Trim();
                        string newArticle = parts[1].Trim();
                        decimal factor = 1;

                        if (parts.Length >= 3)
                        {
                            decimal.TryParse(parts[2].Trim(), out factor);
                        }

                        var existing = CatalogEntries.FirstOrDefault(x => x.OldArticle == oldArticle);
                        if (existing != null)
                        {
                            existing.NewArticle = newArticle;
                            existing.Factor = factor;
                        }
                        else
                        {
                            CatalogEntries.Add(new CatalogEntry
                            {
                                OldArticle = oldArticle,
                                NewArticle = newArticle,
                                Factor = factor
                            });
                        }
                        addedCount++;
                    }
                }

                if (addedCount > 0)
                {
                    MessageBox.Show($"Добавлено/обновлено {addedCount} записей");
                    view?.Refresh();
                    UpdateSearchResultCount();
                    TxtStatus.Text = $"Статус: Добавлено {addedCount} записей (не сохранено в БД)";
                }
                else
                {
                    MessageBox.Show("Не удалось распознать данные. Формат: Артикул [Tab] Артикул_замена [Tab] Коэффициент");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка вставки из буфера: {ex.Message}");
            }
        }

        private void BtnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = CatalogGrid.SelectedItems;
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Выберите строки для удаления");
                return;
            }

            // Фильтруем только реальные CatalogEntry объекты
            var itemsToDelete = selectedItems.Cast<object>()
                .Where(x => x is CatalogEntry)
                .Cast<CatalogEntry>()
                .ToList();

            if (itemsToDelete.Count == 0)
            {
                MessageBox.Show("Выберите существующие записи для удаления");
                return;
            }

            if (MessageBox.Show($"Удалить {itemsToDelete.Count} записей?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var item in itemsToDelete)
                {
                    CatalogEntries.Remove(item);
                }
                view?.Refresh();
                UpdateSearchResultCount();
                TxtStatus.Text = $"Статус: Удалено {itemsToDelete.Count} записей (не сохранено в БД)";
            }
        }

        private async void BtnRefreshFromDB_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataFromSupabase();
        }

        private async Task LoadDataFromSupabase()
        {
            try
            {
                TxtStatus.Text = "Статус: Загрузка из Supabase...";

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");

                var response = await httpClient.GetAsync($"{supabaseUrl}/rest/v1/catalog_replacements?select=*");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var items = JsonSerializer.Deserialize<System.Collections.Generic.List<SupabaseCatalogItem>>(json);

                    CatalogEntries.Clear();
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            CatalogEntries.Add(new CatalogEntry
                            {
                                OldArticle = item.old_article,
                                NewArticle = item.replacement_article,
                                Factor = item.quantity_factor
                            });
                        }
                    }

                    view?.Refresh();
                    UpdateSearchResultCount();
                    TxtStatus.Text = $"Статус: Загружено {CatalogEntries.Count} записей";
                    MessageBox.Show($"Загружено {CatalogEntries.Count} записей из Supabase");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TxtStatus.Text = $"Статус: Ошибка - {response.StatusCode}";
                    MessageBox.Show($"Ошибка загрузки: {response.StatusCode}\n{error}");
                }
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"Статус: Ошибка - {ex.Message}";
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private async void BtnRestoreFromBackup_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Title = "Выберите файл бэкапа";
            openFileDialog.Filter = "JSON files (*.json)|*.json";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string json = File.ReadAllText(openFileDialog.FileName);
                    var backupData = JsonSerializer.Deserialize<BackupData>(json);

                    if (backupData?.Entries != null && backupData.Entries.Any())
                    {
                        if (MessageBox.Show($"Восстановить {backupData.Entries.Count} записей из бэкапа от {backupData.BackupDate:dd.MM.yyyy HH:mm}?\nТекущие данные будут заменены!",
                            "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            CatalogEntries.Clear();
                            foreach (var entry in backupData.Entries)
                            {
                                CatalogEntries.Add(new CatalogEntry
                                {
                                    OldArticle = entry.OldArticle,
                                    NewArticle = entry.NewArticle,
                                    Factor = entry.Factor
                                });
                            }

                            view?.Refresh();
                            UpdateSearchResultCount();
                            TxtStatus.Text = $"Статус: Восстановлено {CatalogEntries.Count} записей (не сохранено в БД)";
                            MessageBox.Show($"Восстановлено {CatalogEntries.Count} записей!\nНе забудьте сохранить изменения в БД.",
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Файл бэкапа не содержит данных или имеет неверный формат.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка восстановления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtStatus.Text = "Статус: Сохранение в Supabase...";

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");

                // Удаляем все существующие записи
                TxtStatus.Text = "Статус: Удаление старых записей...";
                var deleteResponse = await httpClient.DeleteAsync($"{supabaseUrl}/rest/v1/catalog_replacements?old_article=not.is.null");

                if (!deleteResponse.IsSuccessStatusCode)
                {
                    var deleteError = await deleteResponse.Content.ReadAsStringAsync();
                    TxtStatus.Text = $"Статус: Ошибка удаления - {deleteResponse.StatusCode}";
                    MessageBox.Show($"Ошибка при удалении старых записей: {deleteResponse.StatusCode}\n{deleteError}");
                    return;
                }

                // Добавляем новые записи
                int savedCount = 0;
                foreach (var entry in CatalogEntries.Where(x => !string.IsNullOrWhiteSpace(x.OldArticle) && !string.IsNullOrWhiteSpace(x.NewArticle)))
                {
                    var newItem = new
                    {
                        old_article = entry.OldArticle,
                        replacement_article = entry.NewArticle,
                        quantity_factor = entry.Factor
                    };

                    var json = JsonSerializer.Serialize(newItem);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var insertResponse = await httpClient.PostAsync($"{supabaseUrl}/rest/v1/catalog_replacements", content);

                    if (insertResponse.IsSuccessStatusCode)
                    {
                        savedCount++;
                    }
                    else
                    {
                        var error = await insertResponse.Content.ReadAsStringAsync();
                        TxtStatus.Text = $"Статус: Ошибка при сохранении {entry.OldArticle}";
                        MessageBox.Show($"Ошибка при сохранении {entry.OldArticle}: {insertResponse.StatusCode}\n{error}");
                        return;
                    }
                }

                // Обновляем локальный каталог
                OriginalCatalog.Clear();
                foreach (var entry in CatalogEntries)
                {
                    if (!string.IsNullOrWhiteSpace(entry.OldArticle) && !string.IsNullOrWhiteSpace(entry.NewArticle))
                    {
                        OriginalCatalog[entry.OldArticle] = new CatalogItem
                        {
                            ReplacementArticle = entry.NewArticle,
                            QuantityFactor = entry.Factor
                        };
                    }
                }

                TxtStatus.Text = $"Статус: Сохранено {savedCount} записей";
                MessageBox.Show($"Каталог успешно сохранен в Supabase!\nСохранено записей: {savedCount}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"Статус: Ошибка - {ex.Message}";
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateBackup();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания бэкапа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateBackup()
        {
            // Определяем путь для бэкапа
            string backupFolder;
            bool isDriveXAvailable = false;

            try
            {
                isDriveXAvailable = Directory.Exists("X:\\");
            }
            catch { }

            if (isDriveXAvailable)
            {
                backupFolder = @"X:\Резерв БД FurnApp";
            }
            else
            {
                backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Резерв БД FurnApp");
                // Или на диск C:
                // backupFolder = @"C:\Резерв БД FurnApp";
            }

            // Создаем папку если не существует
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            // Формируем имя файла с датой и временем
            string fileName = $"catalog_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string backupPath = Path.Combine(backupFolder, fileName);

            // Создаем объект для бэкапа
            var backupData = new BackupData
            {
                BackupDate = DateTime.Now,
                Version = "1.0",
                Entries = CatalogEntries.Select(entry => new BackupEntry
                {
                    OldArticle = entry.OldArticle,
                    NewArticle = entry.NewArticle,
                    Factor = entry.Factor
                }).ToList()
            };

            // Сохраняем в JSON
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(backupData, options);
            File.WriteAllText(backupPath, json, Encoding.UTF8);

            TxtStatus.Text = $"Статус: Бэкап создан - {fileName}";
            MessageBox.Show($"Бэкап успешно создан!\n\nПуть: {backupPath}\n\nЗаписей: {CatalogEntries.Count}",
                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class CatalogEntry : INotifyPropertyChanged
    {
        private string _oldArticle;
        private string _newArticle;
        private decimal _factor = 1;

        public string OldArticle
        {
            get => _oldArticle;
            set { _oldArticle = value; OnPropertyChanged(nameof(OldArticle)); }
        }

        public string NewArticle
        {
            get => _newArticle;
            set { _newArticle = value; OnPropertyChanged(nameof(NewArticle)); }
        }

        public decimal Factor
        {
            get => _factor;
            set { _factor = value; OnPropertyChanged(nameof(Factor)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
    public class BackupData
    {
        public DateTime BackupDate { get; set; }
        public string Version { get; set; }
        public List<BackupEntry> Entries { get; set; }
    }

    public class BackupEntry
    {
        public string OldArticle { get; set; }
        public string NewArticle { get; set; }
        public decimal Factor { get; set; }
    }
}