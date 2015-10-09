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
        public FileBrowserHelper()
        {
            
        }

        public void LoadListener()
        {
            new Thread(t =>
            {
                factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "RemoteFileBrowserQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Debug.WriteLine(" [x] Received: " + message);
                        Request r = JObject.Parse(message).ToObject<Request>();
                        Handle(r);
                    };
                    channel.BasicConsume(queue: "RemoteFileBrowserQueue", noAck: true, consumer: consumer);

                    while (true) { }
                }
            }
                ).Start();
        }

        private void Handle(Request r)
        {
            if (r.RequestType == RequestEnum.ListOfDrives)
            {
                SendMessage(GetListOfDrives());
            }
        }

        public void SendMessage(string message)
        {
            Random r = new Random();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "RemoteFileBrowserQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "RemoteFileBrowserQueue",
                                     basicProperties: null,
                                     body: body);
                Debug.WriteLine(" [x] Sent " + message);
            }
        }

        public string GetListOfDrives()
        {
            var drives = DriveInfo.GetDrives();
            var ret = new ListOfDrives();
            ret.Drives = new List<Drive>();

            foreach (var d in drives)
            {
                try
                {
                    Drive d1 = new Drive()
                    {
                        DriveLetter = d.Name.Replace(@":\\", ""),
                        DriveName = d.VolumeLabel,
                        DriveType = d.DriveType.ToString(),
                        FreeSpace = d.AvailableFreeSpace,
                        UsedSpace = d.TotalSize - d.AvailableFreeSpace
                    };
                    ret.Drives.Add(d1);
                }
                catch { }
            }

            JsonSerializer js = new JsonSerializer();
            return js.Serialize(ret);
        }
    }
}
