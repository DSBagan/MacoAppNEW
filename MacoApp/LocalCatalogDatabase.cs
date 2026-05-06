using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TBMFurn
{
    public class LocalCatalogDatabase
    {
        private readonly string _connectionString;

        public LocalCatalogDatabase()
        {
            // Используем существующий Furnapp.db
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Furnapp.db");
            _connectionString = $"Data Source={dbPath}";

            // Создаем таблицу если её нет
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
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
        /// Получение всего каталога
        /// </summary>
        public async Task<Dictionary<string, CatalogItem>> GetAllCatalogAsync()
        {
            var catalog = new Dictionary<string, CatalogItem>();

            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection(_connectionString))
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
        /// Сохранение всего каталога (заменяет все существующие записи)
        /// </summary>
        public async Task SaveAllCatalogAsync(Dictionary<string, CatalogItem> catalog)
        {
            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        // Очищаем таблицу
                        var deleteCmd = connection.CreateCommand();
                        deleteCmd.CommandText = "DELETE FROM catalog_replacements";
                        deleteCmd.ExecuteNonQuery();

                        // Вставляем новые данные
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
        }

        /// <summary>
        /// Добавление или обновление одной записи
        /// </summary>
        public async Task UpsertCatalogEntryAsync(string oldArticle, string newArticle, decimal quantityFactor, bool isSeal = false, decimal shippingStandard = 0)
        {
            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection(_connectionString))
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
        /// Удаление записи
        /// </summary>
        public async Task DeleteCatalogEntryAsync(string oldArticle)
        {
            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM catalog_replacements WHERE old_article = $old_article";
                    command.Parameters.AddWithValue("$old_article", oldArticle);
                    command.ExecuteNonQuery();
                }
            });
        }

        /// <summary>
        /// Поиск по старому артикулу
        /// </summary>
        public async Task<CatalogItem> FindByOldArticleAsync(string oldArticle)
        {
            CatalogItem result = null;

            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT replacement_article, quantity_factor, is_seal, shipping_standard FROM catalog_replacements WHERE old_article = $old_article";
                    command.Parameters.AddWithValue("$old_article", oldArticle);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new CatalogItem
                            {
                                ReplacementArticle = reader.GetString(0),
                                QuantityFactor = (decimal)reader.GetDouble(1),
                                IsSeal = reader.GetInt32(2) == 1,
                                ShippingStandard = (decimal)reader.GetDouble(3)
                            };
                        }
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// Проверка, существует ли запись
        /// </summary>
        public async Task<bool> ExistsAsync(string oldArticle)
        {
            var exists = false;

            await Task.Run(() =>
            {
                using (var connection = new SqliteConnection(_connectionString))
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
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT COUNT(1) FROM catalog_replacements";

                    count = Convert.ToInt32(command.ExecuteScalar());
                }
            });

            return count;
        }
    }
}