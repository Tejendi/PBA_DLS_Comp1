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
            // Initialization of the connection to local RabbitMQ server.
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                // Connects to a queue in RabbitMQ named "task_queue", 
                channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                // Using the stopwatch, ensures that the application will run for 10 minutes while we're testing.
                while (stopwatch.Elapsed < TimeSpan.FromMinutes(10))
                {
                    // Makes sure the requests increase the workload required.
                    for (long i = 1; i < 10000000; i += 200000)
                    {
                        // For each of the large requests made to the message queue, we generate and send 10 smaller requests to simulate variations in requests
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
