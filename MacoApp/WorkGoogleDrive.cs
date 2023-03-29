﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using static Google.Apis.Drive.v3.DriveService;

namespace MacoApp
{
    public class WorkGoogleDrive
    {
        //Аутентификация Гугл Диска
        private static DriveService GetService()
        {
            var tokenResponse = new TokenResponse
            {
                AccessToken = "ya29.a0AeTM1ifY58F3vrV3puisAguUu4ArJi8cjpSb6CUDts9DQllF5XLXS3stk5KbHjDo2YKERLZGlOMJn_eIfAXlD0wpAnqOQU_GjzgbB-MD_-4VSQSKiyA__KyMGp3AgYud_5tXS9TBIxtLDg1FtTh3UQzfwctkaCgYKAR4SARMSFQHWtWOmap4NBUVNxTNF8LNKxEP7-Q0163",
                RefreshToken = "1//04Uljd6FtWeazCgYIARAAGAQSNwF-L9IrRoiTw5FPc8ldtNNVPU0m0iSqWUAFM1SSlx5kmMfsr6PpQL-xEVYkVAKW8wITvt6GbMk",
            };

            var applicationName = "Web client 3";
            var username = "denischetb@gmail.com";

            var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = "895054086876-0v6aqauh0sns7kbqjc1g8blp3pc42j8d.apps.googleusercontent.com",
                    ClientSecret = "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"
                },
                Scopes = new[] { Scope.Drive },
                DataStore = new FileDataStore(applicationName)
            });

            var credential = new UserCredential(apiCodeFlow, username, tokenResponse);

            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
            return service;
        }

        //Загрузка файла
        public string UploadFile(Stream file, string fileName, string fileMime, string folder, string fileDescription)
        {
            DriveService service = GetService();

            var driveFile = new Google.Apis.Drive.v3.Data.File();
            driveFile.Name = fileName;
            driveFile.Description = fileDescription;
            driveFile.MimeType = fileMime;
            driveFile.Parents = new string[] { folder };

            var request = service.Files.Create(driveFile, file, fileMime);
            request.Fields = "id";

            var response = request.Upload();
            if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
            {
                throw response.Exception;
            }
            return request.ResponseBody.Id;
        }

        //Обновление файла
        public string UpdateFile(Stream file, string fileId, string fileMime)
        {
            DriveService service = GetService();

            var updateFile = new Google.Apis.Drive.v3.Data.File();

            var request = service.Files.Update(updateFile, fileId, file, fileMime);
            request.Fields = "id";

            var response = request.Upload();
            if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
            {
                throw response.Exception;
            }
            return request.ResponseBody.Id;
        }



        //Удаление файла
        public void DeleteFile(string fileId)
        {
            var service = GetService();
            var command = service.Files.Delete(fileId);
            var result = command.Execute();
        }

        //Создание папки
        public string CreateFolder(string parent, string folderName)
        {
            var service = GetService();
            var driveFolder = new Google.Apis.Drive.v3.Data.File();
            driveFolder.Name = folderName;
            driveFolder.MimeType = "application/vnd.google-apps.folder";
            driveFolder.Parents = new string[] { parent };
            var command = service.Files.Create(driveFolder);
            var file = command.Execute();
            return file.Id;
        }


        //Перечисление файлов в папке
        public IEnumerable<Google.Apis.Drive.v3.Data.File> GetFiles(string folder)
        {
            var service = GetService();

            var fileList = service.Files.List();
            fileList.Q = $"mimeType!='application/vnd.google-apps.folder' and '{folder}' in parents";
            fileList.Fields = "nextPageToken, files(id, name, size, mimeType)";

            var result = new List<Google.Apis.Drive.v3.Data.File>();
            string pageToken = null;
            do
            {
                fileList.PageToken = pageToken;
                var filesResult = fileList.Execute();
                var files = filesResult.Files;
                pageToken = filesResult.NextPageToken;
                result.AddRange(files);
                MessageBox.Show("" + result);
            } while (pageToken != null);
            return result;
        }

    }
}
