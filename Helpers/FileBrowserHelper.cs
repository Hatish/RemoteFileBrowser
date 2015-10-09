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
    }
}
