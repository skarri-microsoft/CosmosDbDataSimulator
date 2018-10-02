using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;


namespace Common.SdkExtensions
{
    public class ServiceBusClientExtension
    {
        private static IQueueClient queueClient;

        public static void InitClient(ServiceBusConfig config)
        {
            queueClient = new QueueClient(config.ConnectionString, config.QueueName);
        }

        public static async Task SendMessagesAsync(string message)
        {
            if (queueClient == null)
            {
                throw new Exception("please initiate the queue client before sending the message.");
            }
            try
            {
                    // Send the message to the queue.
                    await queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(message)));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
