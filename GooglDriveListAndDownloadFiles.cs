using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Json;
using Google.Apis.Services;
using Google.Apis.Logging;
using Newtonsoft.Json;

namespace GoogleDriveAccessMartin
{
    // Class to demonstrate use-case of drive's download file.
    public class GooglDriveListAndDownloadFiles
    {
        public string GoogleApiKey { get; set; } = "";

        /// <summary>
        /// Download a Document file in PDF format.
        /// </summary>
        /// <param name="fileId">file ID of any workspace document format file.</param>
        /// <returns>byte array stream if successful, null otherwise.</returns>
        public static MemoryStream DriveDownloadFile(string fileId)
        {
            try
            {
             
                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer
                {
                    //HttpClientInitializer = credential,
                    ApplicationName = "Drive API Snippets",
                    ApiKey = "AIzaSyBOovfc5suZCRPIAUBNNw7hQpHYFDRmAOo"
                });

                var request = service.Files.Get(fileId);
                var stream = new MemoryStream();

                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                    progress =>
                    {
                        switch (progress.Status)
                        {
                            case DownloadStatus.Downloading:
                                {
                                    Console.WriteLine(progress.BytesDownloaded);
                                    break;
                                }
                            case DownloadStatus.Completed:
                                {
                                    Console.WriteLine("Download complete.");
                                    break;
                                }
                            case DownloadStatus.Failed:
                                {
                                    Console.WriteLine("Download failed.");
                                    Console.WriteLine($"Error: {progress.Exception}");
                                    break;
                                }
                        }
                    };
                request.Download(stream);

                return stream;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
              
            }
            return null;
        }


        public static List<GoogleDriveFiles> DriveListDriveFiles(string FolderId)
        {
            try
            {
               

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer
                {
                    //HttpClientInitializer = credential,
                    ApplicationName = "Drive API Snippets",
                    ApiKey = "AIzaSyBOovfc5suZCRPIAUBNNw7hQpHYFDRmAOo"
                });

                var FileListRequest = service.Files.List();

                FileListRequest.Q = $"'{FolderId}' in parents and trashed = false";
                Console.WriteLine(FileListRequest.Q);

                FileListRequest.Fields = "nextPageToken, files(id, name, size, version, trashed, createdTime)";

                IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files.Where(c => c.Trashed == false).ToList();
                List<GoogleDriveFiles> FileList = new List<GoogleDriveFiles>();

                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        GoogleDriveFiles File = new GoogleDriveFiles
                        {
                            Id = file.Id,
                            Name = file.Name,
                            Size = file.Size,
                            Version = file.Version,
                            CreatedTime = file.CreatedTimeRaw
                        };
                        FileList.Add(File);
                    }
                }
                Console.WriteLine(JsonConvert.SerializeObject(FileList, Formatting.Indented));
                return FileList;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
           
            }
            return new List<GoogleDriveFiles>();
        }


    }

    public class GoogleDriveFiles
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public long? Size { get; set; }
        public long? Version { get; set; }
        public string? CreatedTime { get; set; }
    }
}