using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KafkaMessageService;
using KafkaNet;
using KafkaNet.Common;
using KafkaNet.Model;
using KafkaNet.Protocol;
using KafkaMessage = KafkaNet.Protocol.Message;

using ServiceStack;

namespace TestKafka
{
    class Program
    {
        static void Main(string[] args)
        {
            string input;

            Console.WriteLine("Enter send to send the message bar or quit to exit.");

            while ((input = Console.ReadLine()) != null)
            {
                var cmd = input;
                switch (cmd)
                {
                    case "quit":
                        return; 
                    case "produce":
                        // Send to backend which will write to the console
                        Produce2();
                        break;
                    case "consume":
                        Consume2();
                        break;
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static void Produce2()
        {
            //string message = string.Format("{0} - {1}", DateTime.UtcNow.ToString("O"), Guid.NewGuid());

            var svc = new KakfaProducerService<Test>("john", new [] { new Uri("http://logsvrdev:9092") });
            svc.Send(new Test("asaaaadf","aaaa"));
            //Console.WriteLine("In after send message " + message);
        }

        private static void Consume2()
        {
            var svc = new KafkaConsumerService<Test>("john", "ConsumerGroup-MultiPartition-1", new[] { new Uri("http://logsvrdev:9092") });
            var list = svc.Consume();

            foreach (var item in list)
            {
                Console.WriteLine("v " + item.Title);
            }
        }
    }

    public class Test
    {
        public Test(string title, string desc)
        {
            this.Title = title;
            this.Desc = desc;
        }

        public string Title { get; set; }
        public string Desc { get; set; }
    }
}
