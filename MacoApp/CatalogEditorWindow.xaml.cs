using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
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

            CatalogGrid.ItemsSource = CatalogEntries;
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
                    CatalogGrid.Items.Refresh();
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

            if (MessageBox.Show($"Удалить {selectedItems.Count} записей?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var itemsToDelete = selectedItems.Cast<CatalogEntry>().ToList();
                foreach (var item in itemsToDelete)
                {
                    CatalogEntries.Remove(item);
                }
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

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtStatus.Text = "Статус: Сохранение в Supabase...";

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("apikey", supabaseKey);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {supabaseKey}");

                // 1. Сначала удаляем ВСЕ существующие записи
                TxtStatus.Text = "Статус: Удаление старых записей...";
                var deleteResponse = await httpClient.DeleteAsync($"{supabaseUrl}/rest/v1/catalog_replacements?old_article=not.is.null");

                if (!deleteResponse.IsSuccessStatusCode)
                {
                    var deleteError = await deleteResponse.Content.ReadAsStringAsync();
                    TxtStatus.Text = $"Статус: Ошибка удаления - {deleteResponse.StatusCode}";
                    MessageBox.Show($"Ошибка при удалении старых записей: {deleteResponse.StatusCode}\n{deleteError}");
                    return;
                }

                // 2. Добавляем новые записи по одной
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

                // 3. Обновляем локальный каталог
                OriginalCatalog.Clear();
                foreach (var entry in CatalogEntries)
                {
                    OriginalCatalog[entry.OldArticle] = new CatalogItem
                    {
                        ReplacementArticle = entry.NewArticle,
                        QuantityFactor = entry.Factor
                    };
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
}