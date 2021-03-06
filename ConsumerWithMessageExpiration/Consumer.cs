﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Consumer
{
    class Consumer
    {
        static void Main(string[] args)
        {

            Guid consumerInstance = Guid.NewGuid();

            string logPath = "C:\\temp\\" ;

            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            FileStream logFile = File.Create(logPath + consumerInstance + ".txt");

            // Initialization of the connection to local RabbitMQ server.
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

            using (StreamWriter logWriter = new StreamWriter(logFile))
            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                // Connects to a queue in RabbitMQ named "task_queue",.
                channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
                // Configures the subscription queue for the current consumer instance.
                channel.BasicQos(prefetchSize: 0, prefetchCount: 20, global: false);

                Console.WriteLine($"{consumerInstance}: Waiting for messages.");
                logWriter.WriteLine($"{consumerInstance}: Log file created at: {logPath}");
                logWriter.WriteLine($"{consumerInstance}: Waiting for messages.");
                // Defined the handler which holds the event triggering when receiving the message from the queue.
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body;
                    object message = ByteArrayToObject(body);

                    Console.WriteLine($"{consumerInstance}: Received a message: {message} at: {DateTime.Now}");
                    logWriter.WriteLine($"{consumerInstance}: Received a message: {message} at: {DateTime.Now}");

                    ValueTuple<string, string> values = (ValueTuple<string, string>)message;

                    int primes = PrimeCheckerService.CountPrimes(long.Parse(values.Item1), long.Parse(values.Item2));

                    Console.WriteLine($"{consumerInstance}: Result from {message}: {primes}, finished at {DateTime.Now}");
                    logWriter.WriteLine($"{consumerInstance}: Result from {message}: {primes}, finished at {DateTime.Now}");

                    // Tells RabbitMQ that this message has been recieved, so it won't be re-issued.
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                // Registers the consumer.
                channel.BasicConsume(queue: "task_queue", consumer: consumer);
                
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return binForm.Deserialize(memStream);
        }
    }
}
