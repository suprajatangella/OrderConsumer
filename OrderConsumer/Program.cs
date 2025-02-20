using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using OrderConsumer;
using Newtonsoft.Json;

internal class Program
{
    static void Main()
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri("amqps://qmxuhgri:iCVIIpjCErxVOlx8f6tekdFAUeksHAL5@dog.lmq.cloudamqp.com/qmxuhgri")
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "orderQueue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Thread.Sleep(5000); // Simulate order processing
                Order order = JsonConvert.DeserializeObject<Order>(message); // Convert JSON to Order Object
                
                Console.WriteLine($" [✔] Received Order: OrderID={order.OrderID}, Product={order.Product}, Quantity={order.Quantity}");
            };

            channel.BasicConsume(queue: "orderQueue",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Waiting for orders...");
            Console.ReadLine();
        }
    }
}

