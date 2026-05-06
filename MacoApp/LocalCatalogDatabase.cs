using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TBMFurn
{
    public class LocalCatalogDatabase
    {
        private readonly string _localDbPath;
        private GoogleDriveSync _driveSync;
        private readonly string _folderId;
        private readonly string _fileName;

        public event Action<string> StatusChanged;
        public bool IsGoogleDriveAvailable => _driveSync?.IsConnected ?? false;

        public LocalCatalogDatabase()
        {
            _localDbPath = GetLocalDatabasePath();
            Directory.CreateDirectory(Path.GetDirectoryName(_localDbPath));

            // Читаем настройки из конфигурации (если есть)
            var folderId = GetGoogleDriveFolderId();
            _fileName = "Furnapp.db";

            if (!string.IsNullOrEmpty(folderId))
            {
                _folderId = folderId;
                _driveSync = new GoogleDriveSync(_folderId, _fileName);
                _driveSync.StatusChanged += (msg) => StatusChanged?.Invoke(msg);

                // Запускаем синхронизацию при создании
                Task.Run(async () => await InitializeAndSyncAsync());
            }
            else
            {
                StatusChanged?.Invoke("Google Drive не настроен. Использую локальную БД.");
                InitializeDatabase();
            }
        }

        private string GetLocalDatabasePath()
        {
            // Путь к базе данных в папке приложения
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Furnapp.db");
        }

        private string GetGoogleDriveFolderId()
        {
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

                if (File.Exists(configPath))
                {
                    string jsonContent = File.ReadAllText(configPath);
                    using (var doc = System.Text.Json.JsonDocument.Parse(jsonContent))
                    {
                        var root = doc.RootElement;
                        if (root.TryGetProperty("GoogleDrive", out var googleDrive))
                        {
                            if (googleDrive.TryGetProperty("FolderId", out var folderId))
                            {
                                return folderId.GetString() ?? "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка чтения конфигурации: {ex.Message}");
            }

            return "";
        }

        private async Task InitializeAndSyncAsync()
        {
            var connected = await _driveSync.InitializeAsync();
            if (connected)
            {
                await SyncFromCloudAsync();
            }
            else
            {
                InitializeDatabase();
            }
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection($"Data Source={_localDbPath}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS catalog_replacements (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        old_article TEXT NOT NULL UNIQUE,
                        replacement_article TEXT NOT NULL,
                        quantity_factor REAL NOT NULL DEFAULT 1,
                        is_seal INTEGER NOT NULL DEFAULT 0,
                        shipping_standard REAL NOT NULL DEFAULT 0,
                        created_at TEXT,
                        updated_at TEXT
                    );
                    
                    CREATE INDEX IF NOT EXISTS idx_old_article ON catalog_replacements(old_article);
                ";
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Синхронизация из Google Drive
        /// </summary>
        public async Task<bool> SyncFromCloudAsync()
        {
            if (_driveSync == null || !_driveSync.IsConnected) return false;

            try
            {
                var cloudFileExists = await _driveSync.FileExistsAsync();

                if (!cloudFileExists)
                {
                    StatusChanged?.Invoke("Файл в Google Drive не найден. Использую локальную версию.");
                    return false;
                }

                var cloudFileInfo = await _driveSync.GetFileInfoAsync();

                StatusChanged?.Invoke($"Скачивание файла из Google Drive (от {cloudFileInfo.ModifiedTime:dd.MM.yyyy HH:mm})...");

                var success = await _driveSync.DownloadFileAsync(_localDbPath);

                if (success)
                {
                    StatusChanged?.Invoke("База данных обновлена из Google Drive");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Ошибка синхронизации: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Сохранение каталога
        /// </summary>
        public async Task SaveAllCatalogAsync(Dictionary<string, CatalogItem> catalog)
        {
            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection($"Data Source={_localDbPath}"))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        var deleteCmd = connection.CreateCommand();
                        deleteCmd.CommandText = "DELETE FROM catalog_replacements";
                        deleteCmd.ExecuteNonQuery();

                        foreach (var item in catalog)
                        {
                            var insertCmd = connection.CreateCommand();
                            insertCmd.CommandText = @"
                                INSERT INTO catalog_replacements 
                                (old_article, replacement_article, quantity_factor, is_seal, shipping_standard, created_at, updated_at)
                                VALUES ($old_article, $replacement_article, $quantity_factor, $is_seal, $shipping_standard, $created_at, $updated_at)
                            ";
                            insertCmd.Parameters.AddWithValue("$old_article", item.Key);
                            insertCmd.Parameters.AddWithValue("$replacement_article", item.Value.ReplacementArticle);
                            insertCmd.Parameters.AddWithValue("$quantity_factor", item.Value.QuantityFactor);
                            insertCmd.Parameters.AddWithValue("$is_seal", item.Value.IsSeal ? 1 : 0);
                            insertCmd.Parameters.AddWithValue("$shipping_standard", item.Value.ShippingStandard);
                            insertCmd.Parameters.AddWithValue("$created_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            insertCmd.Parameters.AddWithValue("$updated_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            insertCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            });

            StatusChanged?.Invoke($"Сохранено {catalog.Count} записей");
        }

        /// <summary>
        /// Загрузка каталога
        /// </summary>
        public async Task<Dictionary<string, CatalogItem>> GetAllCatalogAsync()
        {
            var catalog = new Dictionary<string, CatalogItem>();

            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection($"Data Source={_localDbPath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT old_article, replacement_article, quantity_factor, is_seal, shipping_standard FROM catalog_replacements";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var oldArticle = reader.GetString(0);
                            catalog[oldArticle] = new CatalogItem
                            {
                                ReplacementArticle = reader.GetString(1),
                                QuantityFactor = (decimal)reader.GetDouble(2),
                                IsSeal = reader.GetInt32(3) == 1,
                                ShippingStandard = (decimal)reader.GetDouble(4)
                            };
                        }
                    }
                }
            });

            return catalog;
        }

        /// <summary>
        /// Добавление или обновление одной записи
        /// </summary>
        public async Task UpsertCatalogEntryAsync(string oldArticle, string newArticle, decimal quantityFactor, bool isSeal = false, decimal shippingStandard = 0)
        {
            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection($"Data Source={_localDbPath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"
                        INSERT INTO catalog_replacements 
                        (old_article, replacement_article, quantity_factor, is_seal, shipping_standard, updated_at)
                        VALUES ($old_article, $replacement_article, $quantity_factor, $is_seal, $shipping_standard, $updated_at)
                        ON CONFLICT(old_article) DO UPDATE SET
                            replacement_article = excluded.replacement_article,
                            quantity_factor = excluded.quantity_factor,
                            is_seal = excluded.is_seal,
                            shipping_standard = excluded.shipping_standard,
                            updated_at = excluded.updated_at
                    ";

                    command.Parameters.AddWithValue("$old_article", oldArticle);
                    command.Parameters.AddWithValue("$replacement_article", newArticle);
                    command.Parameters.AddWithValue("$quantity_factor", quantityFactor);
                    command.Parameters.AddWithValue("$is_seal", isSeal ? 1 : 0);
                    command.Parameters.AddWithValue("$shipping_standard", shippingStandard);
                    command.Parameters.AddWithValue("$updated_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    command.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// Проверка, существует ли запись
        /// </summary>
        public async Task<bool> ExistsAsync(string oldArticle)
        {
            var exists = false;

            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection($"Data Source={_localDbPath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT COUNT(1) FROM catalog_replacements WHERE old_article = $old_article";
                    command.Parameters.AddWithValue("$old_article", oldArticle);

                    exists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            });

            return exists;
        }

        /// <summary>
        /// Получение количества записей
        /// </summary>
        public async Task<int> GetCountAsync()
        {
            var count = 0;

            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection($"Data Source={_localDbPath}"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT COUNT(1) FROM catalog_replacements";

                    count = Convert.ToInt32(command.ExecuteScalar());
                }
            });

            return count;
        }

        /// <summary>
        /// Принудительная синхронизация
        /// </summary>
        public async Task ForceSyncAsync()
        {
            if (_driveSync != null)
            {
                await _driveSync.InitializeAsync();
                await SyncFromCloudAsync();
            }
        }
    }
}