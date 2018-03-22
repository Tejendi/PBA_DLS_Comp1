﻿using RabbitMQ.Client;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Publisher
{
    class Publisher
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "task_queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                
                
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                for (long i = 10000000; i < 1000000000; i += 5000000)
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
                                     body: GetMessage(("1",i.ToString())));

                }

                Console.WriteLine(" [x] Sent");
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static byte[] GetMessage(Object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
