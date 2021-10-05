using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QuanLy.Utilities
{
    public static class GoogleUtilities
    {

        public static string[] Scopes = { Google.Apis.Drive.v3.DriveService.Scope.Drive };

        public static string CreateFolder(string FolderName, string folderid, int type)
        {
            try
            {
                DriveService service = GetService();
                Google.Apis.Drive.v3.Data.File FileMetaData = new Google.Apis.Drive.v3.Data.File();
                //folder
                string typederive = "application/vnd.google-apps.folder";
                if (type == 1)//doc
                    typederive = "application/vnd.google-apps.document";

                if (string.IsNullOrEmpty(folderid))
                {
                    FileMetaData.Name = FolderName;
                    FileMetaData.MimeType = typederive;
                }
                else
                {
                    FileMetaData = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = FolderName,
                        MimeType = typederive,
                        Parents = new List<string>
                    {
                        folderid
                    }
                    };
                }

                Google.Apis.Drive.v3.FilesResource.CreateRequest request;
                request = service.Files.Create(FileMetaData);
                request.Fields = "id";
                var file = request.Execute();
                //Console.WriteLine("Folder ID: " + file.Id);
                return file.Id;
            }
            catch
            {
                return "";
            }
        }

        public static string CreateFolder(string FolderName)
        {
            Google.Apis.Drive.v3.DriveService service = GetService();

            Google.Apis.Drive.v3.Data.File FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.Name = FolderName;
            FileMetaData.MimeType = "application/vnd.google-apps.folder";

            Google.Apis.Drive.v3.FilesResource.CreateRequest request;

            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();
            if (file != null) return file.Id;
            return string.Empty;
        }


        private static DriveService GetService()
        {
            var clientId = "1007075612519-41ghti6ogrjng65iqr6djnrlob1ddf2f.apps.googleusercontent.com";// From https://console.developers.google.com
            var clientSecret = "x9pmHIYGd0qWL0tF0fZEBOor";// From https://console.developers.google.com

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            }, Scopes,
              Environment.UserName, CancellationToken.None
              , new FileDataStore(@"E:\\MonaMedia\\QuanLyCore\\QuanLy.Core\\QuanLy.Core\\verify", true)
              ).Result;
            //Once consent is recieved, your token will be stored locally on the AppData directory, so that next time you wont be prompted for consent.   

            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "QuanLyMonaAPI",
            });
            return service;
        }
    }
}
