using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TBMFurn
{
    public class GoogleDriveSync
    {
        private DriveService _driveService;
        private readonly string _applicationName = "TBMFurnApp";
        private string _folderId;
        private string _fileName;
        private bool _isInitialized;

        public event Action<string> StatusChanged;
        public event Action<bool> ConnectionStatusChanged;

        public bool IsConnected => _isInitialized;

        public GoogleDriveSync(string folderId, string fileName)
        {
            _folderId = folderId;
            _fileName = fileName;
        }

        public async Task<bool> InitializeAsync()
        {
            try
            {
                StatusChanged?.Invoke("Инициализация подключения к Google Drive...");

                string credPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "TBMFurn",
                    "token.json"
                );

                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(credPath));

                string credentialsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.json");

                if (!System.IO.File.Exists(credentialsPath))
                {
                    StatusChanged?.Invoke("Файл credentials.json не найден!");
                    ConnectionStatusChanged?.Invoke(false);
                    return false;
                }

                UserCredential credential;

                using (var stream = new System.IO.FileStream(credentialsPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { DriveService.Scope.DriveReadonly },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)
                    );
                }

                _driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = _applicationName
                });

                _isInitialized = true;
                ConnectionStatusChanged?.Invoke(true);
                StatusChanged?.Invoke("Подключение к Google Drive установлено");

                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Ошибка: {ex.Message}");
                ConnectionStatusChanged?.Invoke(false);
                return false;
            }
        }

        public async Task<bool> DownloadFileAsync(string localFilePath)
        {
            if (!_isInitialized)
            {
                StatusChanged?.Invoke("Google Drive не инициализирован");
                return false;
            }

            try
            {
                StatusChanged?.Invoke($"Поиск файла {_fileName} в Google Drive...");

                var listRequest = _driveService.Files.List();
                listRequest.Q = $"name='{_fileName}' and '{_folderId}' in parents and trashed=false";
                listRequest.Fields = "files(id, name, modifiedTime, size)";
                listRequest.PageSize = 1;

                var result = await listRequest.ExecuteAsync();

                if (result.Files == null || result.Files.Count == 0)
                {
                    StatusChanged?.Invoke($"Файл {_fileName} не найден в папке {_folderId}");
                    return false;
                }

                var remoteFile = result.Files[0];

                StatusChanged?.Invoke($"Скачивание файла {_fileName} (размер: {remoteFile.Size ?? 0} байт)...");

                string directory = System.IO.Path.GetDirectoryName(localFilePath);
                if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                var downloadRequest = _driveService.Files.Get(remoteFile.Id);
                using (var fileStream = new System.IO.FileStream(localFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    await downloadRequest.DownloadAsync(fileStream);
                }

                StatusChanged?.Invoke($"Файл скачан: {_fileName}");
                return true;
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Ошибка скачивания: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> FileExistsAsync()
        {
            if (!_isInitialized) return false;

            try
            {
                var listRequest = _driveService.Files.List();
                listRequest.Q = $"name='{_fileName}' and '{_folderId}' in parents and trashed=false";
                listRequest.Fields = "files(id)";
                listRequest.PageSize = 1;

                var result = await listRequest.ExecuteAsync();
                return result.Files != null && result.Files.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<GoogleDriveFileInfo> GetFileInfoAsync()
        {
            if (!_isInitialized) return null;

            try
            {
                var listRequest = _driveService.Files.List();
                listRequest.Q = $"name='{_fileName}' and '{_folderId}' in parents and trashed=false";
                listRequest.Fields = "files(id, name, modifiedTime, size)";
                listRequest.PageSize = 1;

                var result = await listRequest.ExecuteAsync();

                if (result.Files != null && result.Files.Count > 0)
                {
                    var file = result.Files[0];
                    return new GoogleDriveFileInfo
                    {
                        Id = file.Id,
                        Name = file.Name,
                        ModifiedTime = file.ModifiedTime ?? DateTime.MinValue,
                        Size = file.Size ?? 0
                    };
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public class GoogleDriveFileInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime ModifiedTime { get; set; }
        public long Size { get; set; }
    }
}