using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Publisher
{
    class Publisher
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                while (stopwatch.Elapsed < TimeSpan.FromMinutes(10))
                {
                    for (long i = 1; i < 10000000; i += 200000)
                    {
                        for (int t = 0; t < 10; t++)

                        {
                            Console.WriteLine($"Creating request at: {DateTime.Now} for Count Primes: 1, 10");
                            channel.BasicPublish(exchange: "",
                                             routingKey: "task_queue",
                                             basicProperties: properties,
                                             body: GetMessage(("1", "10")));
                        }

                        Console.WriteLine($"Creating request at: {DateTime.Now} for Count Primes: 1, {i}");
                        channel.BasicPublish(exchange: "",
                                         routingKey: "task_queue",
                                         basicProperties: properties,
                                         body: GetMessage(("1", i.ToString())));

                    }
                }

                stopwatch.Stop();

                Console.WriteLine(" [x] Sent");
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static byte[] GetMessage(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
