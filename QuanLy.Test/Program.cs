using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Threading;

namespace QuanLy.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string folderId = CreateFolder("test_2109", string.Empty, 1);
            Console.WriteLine("FolderId: " + folderId);

        }
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
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private const string PathToServiceAccountKeyFile = @"E:\\MonaMedia\\QuanLyCore\\QuanLy.Core\\QuanLy.Test\\service_account.json";
        private const string ServiceAccountEmail = "drivedemo@norse-lens-326709.iam.gserviceaccount.com";
        private const string DirectoryId = "10krlloIS2i_2u_ewkdv3_1NqcpmWSL1w";

        private static DriveService GetService()
        {

            var credential = GoogleCredential.FromFile(PathToServiceAccountKeyFile)
                .CreateScoped(DriveService.ScopeConstants.Drive);

            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            return service;
        }

    }
}
