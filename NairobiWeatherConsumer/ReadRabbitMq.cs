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
                //the factory declaration to match with the credentials used by Rabbit MQ (default being username:guest password:guest)
                var factory = new ConnectionFactory() { Uri = new Uri("amqp://guest:guest@localhost:5672/host") };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //ensure you are consuming from an exchange to match with the java application
                    channel.ExchangeDeclare("logs", ExchangeType.Fanout);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                  //      Console.WriteLine(" [x] Received {0}", message);

                        //log the message received on your log directory
                        WriteLog(message);
                    };
                    channel.BasicConsume(queue: "Weather",
                                         autoAck: true,
                                         consumer: consumer);

                }
            }
            catch(Exception ex)
            {
                //log your error ess
                WriteLog(ex.Message);
            }
        }

        public static void WriteLog(string content)
        {
            //this logs to the execution path of the application in this case the debug folder
            string appPath = System.AppContext.BaseDirectory;
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo fullDirPath = null;
            FileInfo fileInfo;

            string filePath = appPath+"Logs\\";
            filePath = filePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            fileInfo = new FileInfo(filePath);
            fullDirPath = new DirectoryInfo(fileInfo.DirectoryName);
            if (!fullDirPath.Exists) fullDirPath.Create();
            if (!fileInfo.Exists)
            {
                fileStream = fileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(filePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(content);
            log.Close();
        }


    }
 
}
