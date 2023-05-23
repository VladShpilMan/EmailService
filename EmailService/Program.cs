using EmailService;

public class Program
{
    public static void Main()
    {
        var producer = new Producer();
        var consumer = new Consumer();

        var email = new EmailMessage
        {
            Recipient = "vladik.lanets@gmail.com",
            Subject = "Hello!",
            Body = "Some text"
        };

        producer.SendEmail(email);

        consumer.StartConsuming();
    }
}
