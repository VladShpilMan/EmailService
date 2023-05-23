using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace EmailService
{
    public class Producer
    {
        private const string QueueName = "email_queue";

        public void SendEmail(EmailMessage email)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var message = JsonConvert.SerializeObject(email);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);
                Console.WriteLine("Email sent successfully.");
            }
        }
    }
}
