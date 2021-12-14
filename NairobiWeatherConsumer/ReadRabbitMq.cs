using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.IO;
using System.Reflection;

namespace NairobiWeatherConsumer
{
    class ReadRabbitMq
    {

        private static string m_exePath = string.Empty;
        static void Main(string[] args)
        {
            try {
                var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@localhost:5672/host") };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "Weather",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                  //      Console.WriteLine(" [x] Received {0}", message);
                        WriteLog(message);
                    };
                    channel.BasicConsume(queue: "Weather",
                                         autoAck: true,
                                         consumer: consumer);

                }
            }
            catch(Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        public static void WriteLog(string content)
        {
            //this logs to the execution path of the application in this case the debug folder
            string appPath = System.AppContext.BaseDirectory;
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = appPath+"Logs\\";
            logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(content);
            log.Close();
        }


    }
 
}
