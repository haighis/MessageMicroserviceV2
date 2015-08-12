using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.RestClient;
using Confluent.RestClient.Model;
using Microsoft.Hadoop.Avro;

namespace TestRestClient
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
                        break;
                    case "produce":
                        Produce();
                        break;
                    case "consume":
                        Consume();
                        break;
                }
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static async void Produce()
        {
            IConfluentClient client = new ConfluentClient(new MyConfluentClientSettings());

            var records = new[]
            {
                new AvroRecord<string, Person>
                {
                    PartitionId = Convert.ToInt32(0),
                    Value = new Person { Name = Guid.NewGuid().ToString("N"), Age = 25 }
                },
                new AvroRecord<string, Person>
                {
                    Value = new Person { Name = Guid.NewGuid().ToString("N"), Age = 26 }
                }
            };

            var recordSet = new AvroRecordSet<string, Person>(records)
            {
                //Creating schema using "Microsoft.Hadoop.Avro" - https://www.nuget.org/packages/Microsoft.Hadoop.Avro/
                ValueSchema = AvroSerializer.Create<Person>().ReaderSchema.ToString()
            };

            await client.PublishAsAvroAsync("TestTopic", recordSet);
        }

        private async static void Consume()
        {
            IConfluentClient client = new ConfluentClient(new MyConfluentClientSettings());

            var request = new CreateConsumerRequest
            {
                // Confluent API will create a new InstanceId if not supplied
                InstanceId = "TestConsumerInstance",
                MessageFormat = MessageFormat.Avro
            };

            ConfluentResponse<ConsumerInstance> response = await client.CreateConsumerAsync("TestConsumerGroup", request);
            ConsumerInstance consumerInstance = response.Payload;

            ConfluentResponse<List<AvroMessage<string, Person>>> response2 = await client.ConsumeAsAvroAsync<string, Person>(consumerInstance, "TestTopic");

            foreach (AvroMessage<string, Person> message in response2.Payload)
            {
                Person person = message.Value;
                Console.WriteLine("Name: {0}, Age: {1}", person.Name, person.Age);
            }

            await client.CommitOffsetAsync(consumerInstance);
        }
    }
}
