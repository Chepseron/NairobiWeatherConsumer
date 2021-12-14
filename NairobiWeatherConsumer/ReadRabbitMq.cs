using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NairobiWeatherConsumer
{
    class ReadRabbitMq
    {

        static void Main(string[] args)
        {
            try {
                var factory = new ConnectionFactory() { Uri = new Uri("amqp://Anonymous:  @localhost:15672/") };
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
                        Console.WriteLine(" [x] Received {0}", message);
                    };
                    channel.BasicConsume(queue: "Weather",
                                         autoAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
               
                
            }
        }
    }
 
}
