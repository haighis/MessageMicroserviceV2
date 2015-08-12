using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using KafkaNet;
using KafkaNet.Common;
using KafkaNet.Model;
using KafkaNet.Protocol;
using ServiceStack;
using KafkaMessage = KafkaNet.Protocol.Message;

namespace KafkaMessageService
{
    public class KakfaProducerService1<T> where T : class
    {
        private readonly string _topic;
        private readonly Uri[] _hosts;
        private Producer _producer;
        private KafkaClient _kafkaClient;

        public KakfaProducerService1(string topic, Uri[] hosts)
        {
            _kafkaClient = new KafkaClient(hosts);
            _topic = topic;
        }

        public void Send(T message)
        {
            _producer = new Producer(_kafkaClient.BrokerRouter);
            _producer.SendMessageAsync(_topic, new[] { new KafkaMessage(message.ToJson()) });
            //Console.WriteLine("in after send to producer");
        }
    }

    public class KafkaConsumerService1<T> where T : class
    {
        private BrokerRoute _brokerRoute;
        private Consumer _consumer;
        private readonly string _topic;
        private readonly string _consumerGroup;
        private readonly Uri[] _hosts;
        private KafkaClient _kafkaClient;

        public KafkaConsumerService1(string topic, string consumerGroup, Uri[] hosts)
        {
            _kafkaClient = new KafkaClient(hosts);
            _topic = topic;
            _hosts = hosts;
            _consumerGroup = consumerGroup;
        }

        public async Task<List<T>> Consume()
        {
            _brokerRoute = _kafkaClient.BrokerRouter.SelectBrokerRoute(_topic);
            List<Topic> topics = _kafkaClient.BrokerRouter.GetTopicMetadata(_topic);
            OffsetPosition[] offsetPositions = OffsetFetchRequest(topics.First().Partitions.Select(p => new OffsetFetch
            {
                Topic = _topic,
                PartitionId = p.PartitionId
            })).Select(o => new OffsetPosition { Offset = o.Offset + 1, PartitionId = o.PartitionId }).ToArray();

            var list = await StartConsumer(offsetPositions);

            return list;
        }

        private IEnumerable<OffsetFetchResponse> OffsetFetchRequest(IEnumerable<OffsetFetch> offsetFetches)
        {
            var request = new OffsetFetchRequest
            {
                ConsumerGroup = _consumerGroup,
                Topics = offsetFetches.ToList()
            };

            return _brokerRoute.Connection.SendAsync(request).Result;
        }

        private void OffsetCommitRequest(int partitionId, long offset, string metadata = null)
        {
            var request = new OffsetCommitRequest
            {
                ConsumerGroup = _consumerGroup,
                OffsetCommits = new List<OffsetCommit>
                {
                    new OffsetCommit
                    {
                        PartitionId = partitionId,
                        Topic = _topic,
                        Offset = offset,
                        Metadata = metadata
                    }
                }
            };

            _brokerRoute.Connection.SendAsync(request);
        }

        private async Task<List<T>> StartConsumer(params OffsetPosition[] offsetPositions)
        {
            _consumer = new Consumer(new ConsumerOptions(_topic, _kafkaClient.BrokerRouter),offsetPositions);
            List<T> list = new List<T>();

            foreach (var data in _consumer.Consume())
            {
                list.Add(data.Value.ToUtf8String().FromJson<T>());
                //Console.ForegroundColor = ConsoleColor.Magenta;
                //Console.WriteLine(@"Response: P{0},O{1} : {2}", data.Meta.PartitionId, data.Meta.Offset, data.Value.ToUtf8String());
                //Console.ForegroundColor = ConsoleColor.White;

             //   var item = data.Value.ToUtf8String().FromJson<T>();
                //OffsetCommitRequest(data.Meta.PartitionId, data.Meta.Offset);
            }

            return list;
        }
    }
    public class KakfaProducerService<T> where T : class
    {
        private readonly string _topic;
        private readonly Uri[] _hosts;
        private Producer _producer;
        private KafkaClient _kafkaClient;

        public KakfaProducerService(string topic, Uri[] hosts)
        {
            _kafkaClient = new KafkaClient(hosts);
            _topic = topic;
        }

        public void Send(T message)
        {
            _producer = new Producer(_kafkaClient.BrokerRouter);
            _producer.SendMessageAsync(_topic, new[] {new KafkaMessage(message.ToJson())});
        }
    }

    public class KafkaConsumerService<T> where T : class
    {
        private BrokerRoute _brokerRoute;
        private Consumer _consumer;
        private readonly string _topic;
        private readonly string _consumerGroup;
        private readonly Uri[] _hosts;
        private KafkaClient _kafkaClient;

        public KafkaConsumerService(string topic, string consumerGroup, Uri[] hosts)
        {
            _kafkaClient = new KafkaClient(hosts);
            _topic = topic;
            _hosts = hosts;
            _consumerGroup = consumerGroup;
        }

        public List<T> Consume()
        {
            _brokerRoute = _kafkaClient.BrokerRouter.SelectBrokerRoute(_topic);
            List<Topic> topics = _kafkaClient.BrokerRouter.GetTopicMetadata(_topic);
            OffsetPosition[] offsetPositions = OffsetFetchRequest(topics.First().Partitions.Select(p => new OffsetFetch
            {
                Topic = _topic,
                PartitionId = p.PartitionId
            })).Select(o => new OffsetPosition { Offset = o.Offset + 1, PartitionId = o.PartitionId }).ToArray();

            //var list = StartConsumer(offsetPositions);
            _consumer = new Consumer(new ConsumerOptions(_topic, _kafkaClient.BrokerRouter), offsetPositions);
            List<T> list = new List<T>();

            var count = 0;

            foreach (var data in _consumer.Consume())
            {
                //Console.ForegroundColor = ConsoleColor.Magenta;
                //Console.WriteLine(@"Response: P{0},O{1} : {2}", data.Meta.PartitionId, data.Meta.Offset, data.Value.ToUtf8String());
                //Console.ForegroundColor = ConsoleColor.White;
                count++;

                var item = data.Value.ToUtf8String().FromJson<T>();
                list.Add(item);

                if (count > 2)
                {
                    break;
                }
                
                OffsetCommitRequest(data.Meta.PartitionId, data.Meta.Offset);
            }

            return list;
        }

        private IEnumerable<OffsetFetchResponse> OffsetFetchRequest(IEnumerable<OffsetFetch> offsetFetches)
        {
            var request = new OffsetFetchRequest
            {
                ConsumerGroup = _consumerGroup,
                Topics = offsetFetches.ToList()
            };

            return _brokerRoute.Connection.SendAsync(request).Result;
        }

        private void OffsetCommitRequest(int partitionId, long offset, string metadata = null)
        {
            var request = new OffsetCommitRequest
            {
                ConsumerGroup = _consumerGroup,
                OffsetCommits = new List<OffsetCommit>
                {
                    new OffsetCommit
                    {
                        PartitionId = partitionId,
                        Topic = _topic,
                        Offset = offset,
                        Metadata = metadata
                    }
                }
            };

            _brokerRoute.Connection.SendAsync(request);
        }

        private List<T> StartConsumer(params OffsetPosition[] offsetPositions)
        {
            _consumer = new Consumer(new ConsumerOptions(_topic, _kafkaClient.BrokerRouter), offsetPositions);
            List<T> list = new List<T>();

            foreach (var data in _consumer.Consume())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(@"Response: P{0},O{1} : {2}", data.Meta.PartitionId, data.Meta.Offset, data.Value.ToUtf8String());
                Console.ForegroundColor = ConsoleColor.White;

                var item = data.Value.ToUtf8String().FromJson<T>();
                list.Add(item);

               // OffsetCommitRequest(data.Meta.PartitionId, data.Meta.Offset);
            }

            return list;
        }
    }
}
