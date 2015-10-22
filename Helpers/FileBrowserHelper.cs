using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Models;
using Newtonsoft.Json.Linq;

namespace RemoteFileBrowser
{
    public class FileBrowserHelper
    {
        ConnectionFactory factory;
        JsonSerializer js = new JsonSerializer();

        string ServerQueue = "ServerFileBrowserQueue";
        string clientQueue = "ClientFileBrowserQueue";
        int copyPercentage = 0;
        long jobSize = 0;
        long jobProgress = 0;
        string destination;

        public FileBrowserHelper()
        {

        }

        public void LoadServerListener()
        {
            new Thread(t =>
            {
                factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: ServerQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Debug.WriteLine(" [x] Received On Server: " + message);
                        Request r = JObject.Parse(message).ToObject<Request>();
                        ServerHandle(r);
                    };
                    channel.BasicConsume(queue: ServerQueue, noAck: true, consumer: consumer);

                    while (true) { }
                }
            }
                ).Start();
        }

        public void LoadClientListener(IClientCallback cb)
        {
            new Thread(t =>
            {
                factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: clientQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Debug.WriteLine(" [x] Received On Client: " + message);
                        Response r = JObject.Parse(message).ToObject<Response>();
                        cb.Handle(r);
                    };
                    channel.BasicConsume(queue: clientQueue, noAck: true, consumer: consumer);

                    while (true) { }
                }
            }
                ).Start();
        }

        private void ServerHandle(Request r)
        {
            if (r.RequestType == RequestEnum.ListOfDrives)
            {
                SendMessageToClient(GetListOfDrives());
            }
            else if (r.RequestType == RequestEnum.OpenFolder)
            {
                string dir = r.Data.ToString();
                SendMessageToClient(GetFilesAndFolders(dir));
            }
            else if (r.RequestType == RequestEnum.CopyData)
            {
                var j = ((JObject)r.Data).ToObject<CopyDataRequest>();
                SendMessageToClient(CopyData(j.Data as List<string>, j.Destination));
            }
        }

        public void SendMessageToServer(string message)
        {
            Random r = new Random();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: ServerQueue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: ServerQueue,
                                     basicProperties: null,
                                     body: body);
                Debug.WriteLine(" [x] Sent To Server:" + message);
            }
        }

        public void SendMessageToClient(string message)
        {
            Random r = new Random();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: clientQueue,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: clientQueue,
                                     basicProperties: null,
                                     body: body);
                Debug.WriteLine(" [x] Sent To  Client: " + message);
            }
        }

        public string GetListOfDrives()
        {
            var driveData = DriveInfo.GetDrives();
            List<DriveData> data = new List<DriveData>();
            foreach (var d in driveData)
            {
                try
                {
                    var dd = new DriveData()
                    {
                        DriveLetter = d.Name.Replace(@":\\", ""),
                        DriveName = d.VolumeLabel,
                        DriveType = d.DriveType.ToString(),
                        FreeSpace = d.AvailableFreeSpace,
                        UsedSpace = d.TotalSize - d.AvailableFreeSpace
                    };
                    data.Add(dd);
                }
                catch { }
            }

            return js.Serialize(new Response() { Data = data, id = 0, RequestType = RequestEnum.ListOfDrives });
        }

        public string GetFilesAndFolders(string dir)
        {
            try
            {
                var files = new List<string>(Directory.GetFiles(dir));
                var folders = new List<string>(Directory.GetDirectories(dir));

                var fData = new FolderData()
                {
                    Folders = folders,
                    Files = files
                };

                return js.Serialize(new Response() { Data = fData, id = 0, RequestType = RequestEnum.OpenFolder });
            }
            catch
            {
                return js.Serialize(new Response() { Data = new FolderData() { Folders = new List<string>(), Files = new List<string>() }, id = 0, RequestType = RequestEnum.OpenFolder });
            }
        }

        public string CopyData(List<string> data, string destination)
        {
            if (!Directory.Exists(destination)) { return ""; }
            jobSize = 0;

            foreach (var s in data)
            {
                if (File.Exists(s))
                {
                    jobSize += new FileInfo(s).Length;
                }
                else if (Directory.Exists(s))
                {
                    jobSize += GetTotalSizeOfDirectory(s);
                }
            }

            foreach (var s in data)
            {
                if (File.Exists(s))
                {
                    CopyFile(s, Path.Combine(destination, Path.GetFileName(s)));
                }
                else if (Directory.Exists(s))
                {
                    DirectoryCopy(s, Path.Combine(destination, Path.GetFileName(s)), true);
                }
            }

            return js.Serialize(new Response() { Data = "Success", id = 0, RequestType = RequestEnum.CopyData });
        }

        private void CopyFile(string src, string dest)
        {
            copyPercentage = 0;
            destination = dest;
            CustomFileCopier copier = new CustomFileCopier(src, dest);
            copier.OnProgressChanged += copier_OnProgressChanged;
            copier.OnComplete += copier_OnComplete;
            copier.Copy();
        }

        void copier_OnComplete(long bytes)
        {
            jobProgress += bytes;
        }

        void copier_OnProgressChanged(double Persentage, long progress, ref bool Cancel)
        {
            int perc = Convert.ToInt32(Persentage);
            if (perc != copyPercentage && perc % 5 == 0)
            {
                SendMessageToClient(js.Serialize(new Response()
                {
                    Data = new CopyDataUpdate() { Progress = perc, Destination = destination, jobTotal = jobSize, jobProgress = (jobProgress + progress) },
                    id = 0,
                    RequestType = RequestEnum.CopyUpdate
                }));
            }
        }

        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                CopyFile(file.FullName, temppath);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private long GetTotalSizeOfDirectory(string destination)
        {
            long size = 0;
            var files = Directory.GetFiles(destination, "", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                size += new FileInfo(f).Length;
            }

            return size;
        }
    }

    public delegate void ProgressChangeDelegate(double Persentage, long bytes, ref bool Cancel);
    public delegate void Completedelegate(long bytes);

    class CustomFileCopier
    {
        public CustomFileCopier(string Source, string Dest)
        {
            this.SourceFilePath = Source;
            this.DestFilePath = Dest;

            OnProgressChanged += delegate { };
            OnComplete += delegate { };
        }

        public void Copy()
        {
            byte[] buffer = new byte[1024 * 1024]; // 1MB buffer
            bool cancelFlag = false;

            using (FileStream source = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
            {
                long fileLength = source.Length;
                using (FileStream dest = new FileStream(DestFilePath, FileMode.CreateNew, FileAccess.Write))
                {
                    long totalBytes = 0;
                    int currentBlockSize = 0;

                    while ((currentBlockSize = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalBytes += currentBlockSize;
                        double persentage = (double)totalBytes * 100.0 / fileLength;

                        dest.Write(buffer, 0, currentBlockSize);

                        cancelFlag = false;
                        OnProgressChanged(persentage, totalBytes, ref cancelFlag);

                        if (cancelFlag == true)
                        {
                            // Delete dest file here
                            break;
                        }
                    }
                }
            }

            OnComplete(new FileInfo(SourceFilePath).Length);
        }

        public string SourceFilePath { get; set; }
        public string DestFilePath { get; set; }

        public event ProgressChangeDelegate OnProgressChanged;
        public event Completedelegate OnComplete;
    }
}
