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

            string localDir = Path.GetDirectoryName(_localDbPath);
            if (!string.IsNullOrEmpty(localDir) && !Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            _networkDbPath = @"R:\NOVOSIBIRSK\Обмен-филиалы\НСК расчет фурнитуры\БД\Furnapp.db";
            _resourcesDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Furnapp.db");

            Task.Run(async () => await InitializeSyncAsync());
        }

        private string GetLocalDatabasePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Furnapp.db");
        }

        private async Task InitializeSyncAsync()
        {
            if (await TryLoadFromNetworkDriveAsync())
            {
                return;
            }

            if (await TryLoadFromResourcesAsync())
            {
                return;
            }

            await LoadFromLocalAsync();
        }

        private async Task<bool> TryLoadFromNetworkDriveAsync()
        {
            try
            {
                StatusChanged?.Invoke("Проверка сетевого диска R:\\...");

                if (!Directory.Exists("R:\\"))
                {
                    StatusChanged?.Invoke("Сетевой диск R:\\ не найден");
                    return false;
                }

                string networkDir = Path.GetDirectoryName(_networkDbPath);
                if (!Directory.Exists(networkDir))
                {
                    StatusChanged?.Invoke($"Папка на сетевом диске не найдена: {networkDir}");
                    return false;
                }

                if (!File.Exists(_networkDbPath))
                {
                    StatusChanged?.Invoke("Файл Furnapp.db не найден на сетевом диске");
                    return false;
                }

                var networkFileInfo = new FileInfo(_networkDbPath);
                var localFileInfo = new FileInfo(_localDbPath);

                bool needCopy = false;

                if (!localFileInfo.Exists)
                {
                    needCopy = true;
                    StatusChanged?.Invoke($"Локальная БД не найдена, копирую полный файл с сетевого диска...");
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
                    File.Copy(_networkDbPath, _localDbPath, true);

                    try
                    {
                        string resourcesDir = Path.GetDirectoryName(_resourcesDbPath);
                        if (!string.IsNullOrEmpty(resourcesDir) && !Directory.Exists(resourcesDir))
                        {
                            Directory.CreateDirectory(resourcesDir);
                        }
                        File.Copy(_networkDbPath, _resourcesDbPath, true);
                        StatusChanged?.Invoke($"БД скопирована с сетевого диска и обновлен резерв");
                    }
                    catch (Exception ex)
                    {
                        StatusChanged?.Invoke($"БД скопирована локально, но не удалось обновить резерв: {ex.Message}");
                    }

                    CurrentSource = DbSource.NetworkDrive;
                }

                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Ошибка доступа к сетевому диску: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> TryLoadFromResourcesAsync()
        {
            try
            {
                StatusChanged?.Invoke("Сетевой диск недоступен, проверяю резервную БД...");

                if (!File.Exists(_resourcesDbPath))
                {
                    StatusChanged?.Invoke("Резервная БД не найдена");
                    return false;
                }

                var resourcesFileInfo = new FileInfo(_resourcesDbPath);

                if (resourcesFileInfo.Length == 0)
                {
                    StatusChanged?.Invoke("Резервная БД пуста");
                    return false;
                }

                var localFileInfo = new FileInfo(_localDbPath);

                if (localFileInfo.Exists && localFileInfo.LastWriteTime > resourcesFileInfo.LastWriteTime)
                {
                    StatusChanged?.Invoke($"Локальная БД новее резервной, использую локальную");
                    CurrentSource = DbSource.Local;
                    return true;
                }

                File.Copy(_resourcesDbPath, _localDbPath, true);
                CurrentSource = DbSource.Resources;
                StatusChanged?.Invoke($"БД восстановлена из резервной копии (от {resourcesFileInfo.LastWriteTime:dd.MM.yyyy HH:mm})");

                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Ошибка доступа к резервной БД: {ex.Message}");
                return false;
            }
        }

        private async Task LoadFromLocalAsync()
        {
            if (File.Exists(_localDbPath))
            {
                var fileInfo = new FileInfo(_localDbPath);
                CurrentSource = DbSource.Local;
                StatusChanged?.Invoke($"Использую локальную БД (от {fileInfo.LastWriteTime:dd.MM.yyyy HH:mm})");
            }
            else
            {
                CurrentSource = DbSource.Created;
                StatusChanged?.Invoke("Локальная БД не найдена, будет создана при первом сохранении");
            }
        }

        /// <summary>
        /// Сохранение каталога замен (только таблица catalog_replacements)
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
                                INSERT OR REPLACE INTO catalog_replacements 
                                (old_article, replacement_article, quantity_factor, is_seal, shipping_standard, updated_at)
                                VALUES ($old_article, $replacement_article, $quantity_factor, $is_seal, $shipping_standard, $updated_at)
                            ";
                            insertCmd.Parameters.AddWithValue("$old_article", item.Key);
                            insertCmd.Parameters.AddWithValue("$replacement_article", item.Value.ReplacementArticle);
                            insertCmd.Parameters.AddWithValue("$quantity_factor", item.Value.QuantityFactor);
                            insertCmd.Parameters.AddWithValue("$is_seal", item.Value.IsSeal ? 1 : 0);
                            insertCmd.Parameters.AddWithValue("$shipping_standard", item.Value.ShippingStandard);
                            insertCmd.Parameters.AddWithValue("$updated_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            insertCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            });

            StatusChanged?.Invoke($"Сохранено {catalog.Count} записей в таблицу catalog_replacements");

            await CopyToNetworkDriveAsync();
        }

        /// <summary>
        /// Копирование БД на сетевой диск (при сохранении изменений)
        /// </summary>
        public async Task CopyToNetworkDriveAsync()
        {
            if (!File.Exists(_localDbPath))
            {
                StatusChanged?.Invoke("Локальная БД не найдена, копирование не выполнено");
                return;
            }

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

                try
                {
                    string resourcesDir = Path.GetDirectoryName(_resourcesDbPath);
                    if (!string.IsNullOrEmpty(resourcesDir) && !Directory.Exists(resourcesDir))
                    {
                        Directory.CreateDirectory(resourcesDir);
                    }
                    File.Copy(_localDbPath, _resourcesDbPath, true);
                    StatusChanged?.Invoke("Резервная копия БД обновлена");
                }
                catch (Exception ex)
                {
                    StatusChanged?.Invoke($"Не удалось обновить резерв: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Не удалось скопировать БД на сетевой диск: {ex.Message}");
            }
        }

        public string GetSourceDescription()
        {
            switch (CurrentSource)
            {
                case DbSource.NetworkDrive:
                    return "🌐 Сетевой диск R:\\";
                case DbSource.Resources:
                    return "📦 Резервная копия";
                case DbSource.Local:
                    return "💻 Локальная копия";
                case DbSource.Created:
                    return "🆕 Будет создана";
                default:
                    return "❓ Неизвестно";
            }
        }

        public async Task<Dictionary<string, CatalogItem>> GetAllCatalogAsync()
        {
            await Task.CompletedTask;
            return new Dictionary<string, CatalogItem>();
        }

        public async Task ForceSyncAsync()
        {
            await InitializeSyncAsync();
        }
    }
}