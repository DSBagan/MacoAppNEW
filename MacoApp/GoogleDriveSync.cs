// using Google.Apis.Auth.OAuth2;
// using Google.Apis.Drive.v3;
// using Google.Apis.Drive.v3.Data;
// using Google.Apis.Services;
// using Google.Apis.Util.Store;
using System;
using System.Threading.Tasks;
// using System.IO;
// using System.Threading;
// using System.Threading.Tasks;

namespace TBMFurn
{
    // Класс GoogleDriveSync полностью закомментирован, так как функционал не используется
    // Оставлен для совместимости, чтобы не было ошибок в других файлах
    public class GoogleDriveSync
    {
        // private DriveService _driveService;
        // private readonly string _applicationName = "TBMFurnApp";
        // private string _folderId;
        // private string _fileName;
        // private bool _isInitialized;

        public event Action<string> StatusChanged;
        public event Action<bool> ConnectionStatusChanged;

        public bool IsConnected => false; // Всегда false

        public GoogleDriveSync(string folderId, string fileName)
        {
            // _folderId = folderId;
            // _fileName = fileName;
            StatusChanged?.Invoke("Google Drive синхронизация отключена");
        }

        public async Task<bool> InitializeAsync()
        {
            await Task.CompletedTask;
            StatusChanged?.Invoke("Google Drive синхронизация отключена");
            ConnectionStatusChanged?.Invoke(false);
            return false;
        }

        public async Task<bool> UploadFileAsync(string localFilePath)
        {
            await Task.CompletedTask;
            StatusChanged?.Invoke("Google Drive синхронизация отключена");
            return false;
        }

        public async Task<bool> DownloadFileAsync(string localFilePath)
        {
            await Task.CompletedTask;
            StatusChanged?.Invoke("Google Drive синхронизация отключена");
            return false;
        }

        public async Task<bool> FileExistsAsync()
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task<GoogleDriveFileInfo> GetFileInfoAsync()
        {
            await Task.CompletedTask;
            return null;
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