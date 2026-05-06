using Supabase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

// Псевдоним для разрешения конфликта имен
using SupabaseClient = Supabase.Client;

namespace TBMFurn
{
    public class MessageItem
    {
        public string id { get; set; }
        public string text { get; set; }
        public string user_name { get; set; }
        public string user_contact { get; set; }
        public DateTime timestamp { get; set; }
        public string attachment_url { get; set; }
        public string attachment_name { get; set; }
        public int status { get; set; }
        public string comments { get; set; }
        public bool is_deleted { get; set; }
    }

    public class SupabaseHelper
    {
        private SupabaseClient _supabase;
        private HttpClient _httpClient;
        private string _supabaseUrl;
        private string _supabaseKey;

        // ВАШИ ДАННЫЕ
        private const string SUPABASE_URL = "https://kajvthlrnayyimrwnyqp.supabase.co";
        private const string SUPABASE_KEY = "sb_publishable_NZcAD8vZMM-j0QX-IQbusA_QlB3BRLF";

        public SupabaseHelper()
        {
            _supabaseUrl = SUPABASE_URL;
            _supabaseKey = SUPABASE_KEY;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_supabaseKey}");
        }

        public async Task Initialize()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== ПОДКЛЮЧЕНИЕ К SUPABASE ===");
                System.Diagnostics.Debug.WriteLine($"URL: {_supabaseUrl}");
                System.Diagnostics.Debug.WriteLine($"KEY: {_supabaseKey}");

                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = true
                };
                _supabase = new SupabaseClient(_supabaseUrl, _supabaseKey, options);
                await _supabase.InitializeAsync();

                System.Diagnostics.Debug.WriteLine("✅ Supabase инициализирован!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка инициализации: {ex.Message}");
            }
        }

        public async Task<List<MessageItem>> GetMessagesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_supabaseUrl}/rest/v1/messages?select=*&is_deleted=eq.false&order=timestamp.desc");
                var content = await response.Content.ReadAsStringAsync();

                var messages = JsonConvert.DeserializeObject<List<MessageItem>>(content);

                if (messages != null)
                {
                    foreach (var msg in messages)
                    {
                        // Проверяем, что комментарии в правильном формате
                        if (string.IsNullOrEmpty(msg.comments) || msg.comments == "null")
                        {
                            msg.comments = "[]";
                        }

                        // Пробуем распарсить комментарии для проверки
                        try
                        {
                            var test = JsonConvert.DeserializeObject(msg.comments);
                            System.Diagnostics.Debug.WriteLine($"Сообщение {msg.id}: комментарии = {msg.comments}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Ошибка парсинга комментариев для {msg.id}: {ex.Message}");
                            msg.comments = "[]";
                        }
                    }
                }

                return messages ?? new List<MessageItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка GetMessagesAsync: {ex.Message}");
                return new List<MessageItem>();
            }
        }

        public async Task<bool> SendMessageAsync(string text, string userName, string userContact,
                                          byte[] fileData = null, string fileName = null)
        {
            try
            {
                string attachmentUrl = null;
                string attachmentName = null;

                // Загружаем файл если есть
                // Загружаем файл если есть
                if (fileData != null && !string.IsNullOrEmpty(fileName))
                {
                    var fileId = Guid.NewGuid().ToString();
                    var extension = System.IO.Path.GetExtension(fileName);
                    var filePath = $"{fileId}{extension}";

                    System.Diagnostics.Debug.WriteLine($"Загрузка файла: {filePath}");
                    System.Diagnostics.Debug.WriteLine($"Размер файла: {fileData.Length} байт");

                    var storageUrl = _supabaseUrl.Replace(".supabase.co", ".storage.supabase.co");
                    var uploadUrl = $"{storageUrl}/storage/v1/object/attachments/{filePath}";

                    using var putContent = new ByteArrayContent(fileData);
                    putContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                    var putResponse = await _httpClient.PutAsync(uploadUrl, putContent);

                    if (putResponse.IsSuccessStatusCode)
                    {
                        attachmentUrl = filePath;
                        attachmentName = fileName;  // сохраняем исходное имя для отображения
                        System.Diagnostics.Debug.WriteLine($"✅ Файл загружен: {attachmentUrl}");
                    }
                    else
                    {
                        var error = await putResponse.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"❌ Ошибка загрузки: {putResponse.StatusCode} - {error}");
                    }
                }

                // Создаем сообщение
                var message = new
                {
                    text = text,
                    user_name = userName,
                    user_contact = userContact ?? "",
                    timestamp = DateTime.UtcNow,
                    attachment_url = attachmentUrl,
                    attachment_name = attachmentName,
                    status = 0,
                    comments = "[]",
                    is_deleted = false
                };

                var json = JsonConvert.SerializeObject(message);
                System.Diagnostics.Debug.WriteLine($"Отправка сообщения: {json}");

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_supabaseUrl}/rest/v1/messages", httpContent);

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Ответ сервера: {response.StatusCode} - {responseContent}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка в SendMessageAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        // Добавьте этот вспомогательный метод в класс SupabaseHelper
        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<bool> UpdateStatusAsync(string messageId, int status)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== ОБНОВЛЕНИЕ СТАТУСА ===");
                System.Diagnostics.Debug.WriteLine($"MessageId: {messageId}");
                System.Diagnostics.Debug.WriteLine($"Новый статус: {status}");

                var updateData = new { status = status };
                var json = JsonConvert.SerializeObject(updateData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{_supabaseUrl}/rest/v1/messages?id=eq.{messageId}";
                System.Diagnostics.Debug.WriteLine($"URL: {url}");

                var response = await _httpClient.PatchAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Ответ: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Тело: {responseContent}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка UpdateStatusAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddCommentAsync(string messageId, string commentText, string author)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== ДОБАВЛЕНИЕ КОММЕНТАРИЯ ===");
                System.Diagnostics.Debug.WriteLine($"MessageId: {messageId}");
                System.Diagnostics.Debug.WriteLine($"Текст: {commentText}");

                // Получаем текущее сообщение
                var getUrl = $"{_supabaseUrl}/rest/v1/messages?select=*&id=eq.{messageId}";
                var getResponse = await _httpClient.GetAsync(getUrl);
                var content = await getResponse.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Получено сообщение: {content}");

                var messages = JsonConvert.DeserializeObject<List<dynamic>>(content);
                if (messages == null || messages.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Сообщение не найдено!");
                    return false;
                }

                var message = messages[0];

                // Получаем существующие комментарии
                List<object> existingComments = new List<object>();

                // Пробуем получить comments разными способами
                if (message.comments != null && message.comments.ToString() != "[]" && message.comments.ToString() != "null")
                {
                    try
                    {
                        var commentsStr = message.comments.ToString();
                        System.Diagnostics.Debug.WriteLine($"Существующие комментарии: {commentsStr}");
                        existingComments = JsonConvert.DeserializeObject<List<object>>(commentsStr);
                        if (existingComments == null) existingComments = new List<object>();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Ошибка парсинга: {ex.Message}");
                        existingComments = new List<object>();
                    }
                }

                // Создаем новый комментарий
                var newComment = new
                {
                    id = Guid.NewGuid().ToString(),
                    text = commentText,
                    author = author,
                    timestamp = DateTime.UtcNow
                };

                existingComments.Add(newComment);

                // ВАЖНО: для JSONB поля нужно передавать JSON строку
                var commentsJson = JsonConvert.SerializeObject(existingComments);
                System.Diagnostics.Debug.WriteLine($"Сохраняем комментарии: {commentsJson}");

                // Обновляем сообщение - используем правильный синтаксис
                var updateUrl = $"{_supabaseUrl}/rest/v1/messages?id=eq.{messageId}";

                // Формируем данные для обновления
                var updateData = new
                {
                    comments = commentsJson  // Передаем как JSON строку
                };

                var updateJson = JsonConvert.SerializeObject(updateData);
                var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync(updateUrl, updateContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Ответ обновления: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Тело ответа: {responseContent}");

                // Проверяем, что комментарии действительно сохранились
                if (response.IsSuccessStatusCode)
                {
                    // Ждем немного и проверяем
                    await Task.Delay(500);

                    var checkResponse = await _httpClient.GetAsync($"{_supabaseUrl}/rest/v1/messages?select=comments&id=eq.{messageId}");
                    var checkContent = await checkResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Проверка после сохранения: {checkContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка AddCommentAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> DeleteMessageAsync(string messageId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== УДАЛЕНИЕ СООБЩЕНИЯ ===");
                System.Diagnostics.Debug.WriteLine($"MessageId: {messageId}");

                var updateData = new { is_deleted = true };
                var json = JsonConvert.SerializeObject(updateData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{_supabaseUrl}/rest/v1/messages?id=eq.{messageId}";
                System.Diagnostics.Debug.WriteLine($"URL: {url}");

                var response = await _httpClient.PatchAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Ответ: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Тело: {responseContent}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка DeleteMessageAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> VerifyAdminPasswordAsync(string password)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_supabaseUrl}/rest/v1/admin?select=password_hash");
                var content = await response.Content.ReadAsStringAsync();
                var admins = JsonConvert.DeserializeObject<List<dynamic>>(content);

                if (admins == null || admins.Count == 0)
                {
                    await CreateDefaultAdminAsync();
                    return password == "admin123";
                }

                return admins[0].password_hash.ToString() == password;
            }
            catch
            {
                return false;
            }
        }

        private async Task CreateDefaultAdminAsync()
        {
            try
            {
                var admin = new { password_hash = "admin123" };
                var json = JsonConvert.SerializeObject(admin);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync($"{_supabaseUrl}/rest/v1/admin", content);
                System.Diagnostics.Debug.WriteLine("✅ Админ по умолчанию создан");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка создания админа: {ex.Message}");
            }
        }

        public async Task<byte[]> DownloadFileAsync(string filePath)
        {
            try
            {
                var storageUrl = _supabaseUrl.Replace(".supabase.co", ".storage.supabase.co");
                // Для загрузки используем public URL
                var publicUrl = $"{storageUrl}/storage/v1/object/public/attachments/{filePath}";

                // Если filePath уже закодирован, не кодируем повторно
                System.Diagnostics.Debug.WriteLine($"Скачивание файла: {publicUrl}");

                var response = await _httpClient.GetAsync(publicUrl);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsByteArrayAsync();
                    System.Diagnostics.Debug.WriteLine($"✅ Файл скачан: {data.Length} байт");
                    return data;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Ошибка скачивания: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Ошибка DownloadFileAsync: {ex.Message}");
                return null;
            }
        }
        /*public async Task<Dictionary<string, CatalogItem>> GetCatalogAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== GetCatalogAsync START ===");

                // Создаем НОВЫЙ HttpClient для этого запроса
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("apikey", _supabaseKey);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_supabaseKey}");
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var url = $"{_supabaseUrl}/rest/v1/catalog_replacements?select=*";
                    System.Diagnostics.Debug.WriteLine($"URL: {url}");

                    var response = await client.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Content length: {content.Length}");
                    System.Diagnostics.Debug.WriteLine($"First 200 chars: {(content.Length > 200 ? content.Substring(0, 200) : content)}");

                    if (!response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error: {content}");
                        return new Dictionary<string, CatalogItem>();
                    }

                    if (string.IsNullOrWhiteSpace(content) || content == "[]")
                    {
                        System.Diagnostics.Debug.WriteLine("Empty catalog");
                        return new Dictionary<string, CatalogItem>();
                    }

                    // ВАЖНО: используем Newtonsoft.Json, но с правильными настройками
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

                    var items = JsonConvert.DeserializeObject<List<SupabaseCatalogItem>>(content, settings);

                    if (items == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Deserialization returned null");
                        return new Dictionary<string, CatalogItem>();
                    }

                    System.Diagnostics.Debug.WriteLine($"Deserialized {items.Count} items");

                    var catalog = new Dictionary<string, CatalogItem>();

                    foreach (var item in items)
                    {
                        if (!string.IsNullOrEmpty(item.old_article))
                        {
                            catalog[item.old_article] = new CatalogItem
                            {
                                ReplacementArticle = item.replacement_article ?? "",
                                QuantityFactor = item.quantity_factor
                            };
                            System.Diagnostics.Debug.WriteLine($"Added: {item.old_article} -> {item.replacement_article} (x{item.quantity_factor})");
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Returning catalog with {catalog.Count} items");
                    return catalog;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EXCEPTION in GetCatalogAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return new Dictionary<string, CatalogItem>();
            }
        }*/

    }
}