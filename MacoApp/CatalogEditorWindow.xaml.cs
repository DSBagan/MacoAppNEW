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
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using TBMFurn;

namespace TBMFurn
{
    public partial class CatalogEditorWindow : Window
    {
        private ObservableCollection<CatalogEntry> CatalogEntries { get; set; }
        private Dictionary<string, CatalogItem> OriginalCatalog { get; set; }
        private LocalCatalogDatabase _localDb;
        private ICollectionView view;

        public CatalogEditorWindow(Dictionary<string, CatalogItem> catalog)
        {
            InitializeComponent();
            OriginalCatalog = catalog;
            _localDb = new LocalCatalogDatabase();
            CatalogEntries = new ObservableCollection<CatalogEntry>();

            foreach (var item in catalog)
            {
                CatalogEntries.Add(new CatalogEntry
                {
                    OldArticle = item.Key,
                    NewArticle = item.Value.ReplacementArticle,
                    Factor = item.Value.QuantityFactor,
                    IsSeal = item.Value.IsSeal,
                    ShippingStandard = item.Value.ShippingStandard
                });
            }

            CatalogGrid.ItemsSource = CatalogEntries;
            view = CollectionViewSource.GetDefaultView(CatalogEntries);
            view.Filter = FilterPredicate;

            UpdateSearchResultCount();
        }

        private bool FilterPredicate(object item)
        {
            if (item == null)
                return false;

            if (item.GetType().Name == "NamedObject")
                return false;

            if (!(item is CatalogEntry))
                return false;

            var catalogItem = item as CatalogEntry;
            string searchText = TxtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchText))
                return true;

            return (catalogItem.OldArticle != null && catalogItem.OldArticle.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0) ||
                   (catalogItem.NewArticle != null && catalogItem.NewArticle.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
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
                        bool isSeal = false;
                        decimal shippingStandard = 0;

                        if (parts.Length >= 3)
                        {
                            decimal.TryParse(parts[2].Trim(), out factor);
                        }
                        if (parts.Length >= 4)
                        {
                            bool.TryParse(parts[3].Trim(), out isSeal);
                        }
                        if (parts.Length >= 5)
                        {
                            decimal.TryParse(parts[4].Trim(), out shippingStandard);
                        }

                        var existing = CatalogEntries.FirstOrDefault(x => x.OldArticle == oldArticle);
                        if (existing != null)
                        {
                            existing.NewArticle = newArticle;
                            existing.Factor = factor;
                            existing.IsSeal = isSeal;
                            existing.ShippingStandard = shippingStandard;
                        }
                        else
                        {
                            CatalogEntries.Add(new CatalogEntry
                            {
                                OldArticle = oldArticle,
                                NewArticle = newArticle,
                                Factor = factor,
                                IsSeal = isSeal,
                                ShippingStandard = shippingStandard
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

        private async void BtnRestoreFromBackup_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
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
                                    Factor = entry.Factor,
                                    IsSeal = entry.IsSeal,
                                    ShippingStandard = entry.ShippingStandard
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
                TxtStatus.Text = "Сохранение...";
                BtnSave.IsEnabled = false;

                var catalogToSave = new Dictionary<string, CatalogItem>();
                foreach (var entry in CatalogEntries)
                {
                    if (!string.IsNullOrWhiteSpace(entry.OldArticle) && !string.IsNullOrWhiteSpace(entry.NewArticle))
                    {
                        catalogToSave[entry.OldArticle] = new CatalogItem
                        {
                            ReplacementArticle = entry.NewArticle,
                            QuantityFactor = entry.Factor,
                            IsSeal = entry.IsSeal,
                            ShippingStandard = entry.ShippingStandard
                        };
                    }
                }

                await _localDb.SaveAllCatalogAsync(catalogToSave);

                OriginalCatalog.Clear();
                foreach (var item in catalogToSave)
                {
                    OriginalCatalog[item.Key] = item.Value;
                }

                TxtStatus.Text = $"Сохранено {catalogToSave.Count} записей";
                MessageBox.Show($"Каталог сохранен!\nСохранено: {catalogToSave.Count} записей",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                TxtStatus.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnSave.IsEnabled = true;
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
            }

            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }

            string fileName = $"catalog_backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string backupPath = Path.Combine(backupFolder, fileName);

            var backupData = new BackupData
            {
                BackupDate = DateTime.Now,
                Version = "1.0",
                Entries = CatalogEntries.Select(entry => new BackupEntry
                {
                    OldArticle = entry.OldArticle,
                    NewArticle = entry.NewArticle,
                    Factor = entry.Factor,
                    IsSeal = entry.IsSeal,
                    ShippingStandard = entry.ShippingStandard
                }).ToList()
            };

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
        private bool _isSeal = false;
        private decimal _shippingStandard = 0;

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

        public bool IsSeal
        {
            get => _isSeal;
            set { _isSeal = value; OnPropertyChanged(nameof(IsSeal)); }
        }

        public decimal ShippingStandard
        {
            get => _shippingStandard;
            set { _shippingStandard = value; OnPropertyChanged(nameof(ShippingStandard)); }
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
        public bool IsSeal { get; set; }
        public decimal ShippingStandard { get; set; }
    }
}