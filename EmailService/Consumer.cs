using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Net.Mail;
using System.Text;
using Newtonsoft.Json;

namespace EmailService
{
    public class Consumer
    {
        private const string QueueName = "email_queue";

        public void StartConsuming()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var email = JsonConvert.DeserializeObject<EmailMessage>(message);
                    SendEmail(email);

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

                Console.WriteLine("Consumer started. Press any key to exit.");
                Console.ReadKey();
            }
        }

        private void SendEmail(EmailMessage email)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("someusername", "somepassword");
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("vladislav.lanets@gmail.com"),
                    Subject = email.Subject,
                    Body = email.Body
                };
                mailMessage.To.Add(email.Recipient);

                smtpClient.Send(mailMessage);
                Console.WriteLine("Email sent to: " + email.Recipient);
            }
        }
    }
}
