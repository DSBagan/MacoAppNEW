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
        private readonly string _networkDbPath;
        private readonly string _resourcesDbPath;

        public event Action<string> StatusChanged;

        // Источник, откуда была загружена БД
        public enum DbSource
        {
            NetworkDrive,
            Resources,
            Local,
            Created
        }

        public DbSource CurrentSource { get; private set; } = DbSource.Local;

        public LocalCatalogDatabase()
        {
            _localDbPath = GetLocalDatabasePath();
            Directory.CreateDirectory(Path.GetDirectoryName(_localDbPath));

            // Путь к БД на сетевом диске
            _networkDbPath = @"R:\NOVOSIBIRSK\Обмен-филиалы\НСК расчет фурнитуры\БД\FurnApp.db";

            // Путь к БД в папке Resources
            _resourcesDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FurnApp.db");

            // Запускаем синхронизацию
            Task.Run(async () => await InitializeSyncAsync());
        }

        private string GetLocalDatabasePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "TBMFurn",
                "FurnApp.db"
            );
        }

        private async Task InitializeSyncAsync()
        {
            // 1. Пробуем загрузить с сетевого диска
            if (await TryLoadFromNetworkDriveAsync())
            {
                return;
            }

            // 2. Если сетевой диск недоступен, пробуем из Resources
            if (await TryLoadFromResourcesAsync())
            {
                return;
            }

            // 3. Если ничего не помогло, используем локальную БД или создаем новую
            await LoadFromLocalAsync();
        }

        /// <summary>
        /// Попытка загрузки с сетевого диска
        /// </summary>
        private async Task<bool> TryLoadFromNetworkDriveAsync()
        {
            try
            {
                StatusChanged?.Invoke("Проверка сетевого диска R:\\...");

                // Проверяем, доступен ли сетевой диск
                if (!Directory.Exists("R:\\"))
                {
                    StatusChanged?.Invoke("Сетевой диск R:\\ не найден");
                    return false;
                }

                // Проверяем, существует ли папка и файл
                string networkDir = Path.GetDirectoryName(_networkDbPath);
                if (!Directory.Exists(networkDir))
                {
                    StatusChanged?.Invoke($"Папка на сетевом диске не найдена: {networkDir}");
                    return false;
                }

                if (!File.Exists(_networkDbPath))
                {
                    StatusChanged?.Invoke("Файл БД не найден на сетевом диске");
                    return false;
                }

                // Получаем информацию о файле на сетевом диске
                var networkFileInfo = new FileInfo(_networkDbPath);
                var localFileInfo = new FileInfo(_localDbPath);

                // Проверяем, нужно ли обновлять локальную копию
                bool needCopy = false;

                if (!localFileInfo.Exists)
                {
                    needCopy = true;
                    StatusChanged?.Invoke($"Локальная БД не найдена, копирую с сетевого диска...");
                }
                else if (networkFileInfo.LastWriteTime > localFileInfo.LastWriteTime)
                {
                    needCopy = true;
                    StatusChanged?.Invoke($"Обнаружена новая версия БД на сетевом диске (от {networkFileInfo.LastWriteTime:dd.MM.yyyy HH:mm})");
                }
                else
                {
                    CurrentSource = DbSource.NetworkDrive;
                    StatusChanged?.Invoke($"БД актуальна (сетевой диск, версия от {localFileInfo.LastWriteTime:dd.MM.yyyy HH:mm})");
                }

                if (needCopy)
                {
                    // Копируем файл с сетевого диска
                    File.Copy(_networkDbPath, _localDbPath, true);
                    CurrentSource = DbSource.NetworkDrive;
                    StatusChanged?.Invoke($"БД скопирована с сетевого диска (размер: {networkFileInfo.Length / 1024} КБ)");
                }

                // Инициализируем БД (создаем таблицы, если нужно)
                InitializeDatabase();
                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Ошибка доступа к сетевому диску: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Попытка загрузки из папки Resources
        /// </summary>
        private async Task<bool> TryLoadFromResourcesAsync()
        {
            try
            {
                StatusChanged?.Invoke("Сетевой диск недоступен, проверяю встроенную БД...");

                if (!File.Exists(_resourcesDbPath))
                {
                    StatusChanged?.Invoke("Встроенная БД не найдена в папке Resources");
                    return false;
                }

                var resourcesFileInfo = new FileInfo(_resourcesDbPath);

                // Проверяем, что файл не пустой
                if (resourcesFileInfo.Length == 0)
                {
                    StatusChanged?.Invoke("Встроенная БД пуста");
                    return false;
                }

                var localFileInfo = new FileInfo(_localDbPath);

                bool needCopy = false;

                if (!localFileInfo.Exists)
                {
                    needCopy = true;
                    StatusChanged?.Invoke($"Локальная БД не найдена, копирую из встроенной...");
                }
                else if (resourcesFileInfo.LastWriteTime > localFileInfo.LastWriteTime)
                {
                    needCopy = true;
                    StatusChanged?.Invoke($"Обнаружена новая версия встроенной БД (от {resourcesFileInfo.LastWriteTime:dd.MM.yyyy HH:mm})");
                }
                else
                {
                    CurrentSource = DbSource.Resources;
                    StatusChanged?.Invoke($"БД актуальна (встроенная, версия от {localFileInfo.LastWriteTime:dd.MM.yyyy HH:mm})");
                }

                if (needCopy)
                {
                    // Копируем файл из Resources
                    File.Copy(_resourcesDbPath, _localDbPath, true);
                    CurrentSource = DbSource.Resources;
                    StatusChanged?.Invoke($"БД скопирована из встроенных ресурсов (размер: {resourcesFileInfo.Length / 1024} КБ)");
                }

                InitializeDatabase();
                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Ошибка доступа к встроенной БД: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Загрузка из локальной БД
        /// </summary>
        private async Task LoadFromLocalAsync()
        {
            StatusChanged?.Invoke("Сетевой диск и встроенная БД недоступны, использую локальную БД");

            if (!File.Exists(_localDbPath))
            {
                StatusChanged?.Invoke("Локальная БД не найдена, создаю новую...");
                InitializeDatabase();
                CurrentSource = DbSource.Created;
            }
            else
            {
                InitializeDatabase();
                CurrentSource = DbSource.Local;
                var fileInfo = new FileInfo(_localDbPath);
                StatusChanged?.Invoke($"Использую локальную БД (от {fileInfo.LastWriteTime:dd.MM.yyyy HH:mm})");
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
        /// Сохранение каталога (сохраняется локально + на сетевой диск)
        /// </summary>
        public async Task SaveAllCatalogAsync(Dictionary<string, CatalogItem> catalog)
        {
            // Сохраняем локально
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

            StatusChanged?.Invoke($"Сохранено {catalog.Count} записей локально");

            // Копируем на сетевой диск (если доступен)
            await CopyToNetworkDriveAsync();
        }

        /// <summary>
        /// Копирование БД на сетевой диск
        /// </summary>
        private async Task CopyToNetworkDriveAsync()
        {
            try
            {
                if (!Directory.Exists("R:\\"))
                {
                    StatusChanged?.Invoke("Сетевой диск R:\\ не найден, копирование не выполнено");
                    return;
                }

                string networkDir = Path.GetDirectoryName(_networkDbPath);
                if (!Directory.Exists(networkDir))
                {
                    Directory.CreateDirectory(networkDir);
                }

                File.Copy(_localDbPath, _networkDbPath, true);
                StatusChanged?.Invoke("БД скопирована на сетевой диск");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Не удалось скопировать БД на сетевой диск: {ex.Message}");
            }
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
        /// Получение источника БД для отображения в статус-баре
        /// </summary>
        public string GetSourceDescription()
        {
            switch (CurrentSource)
            {
                case DbSource.NetworkDrive:
                    return "🌐 Сетевой диск R:\\";
                case DbSource.Resources:
                    return "📦 Встроенная БД (Resources)";
                case DbSource.Local:
                    return "💻 Локальная копия";
                case DbSource.Created:
                    return "🆕 Создана новая БД";
                default:
                    return "❓ Неизвестно";
            }
        }

        public async Task ForceSyncAsync()
        {
            await InitializeSyncAsync();
        }
    }
}