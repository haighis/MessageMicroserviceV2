using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KafkaNet;
using KafkaNet.Model;

namespace KafkaMessageService
{
    public class KafkaClient
    {
        private BrokerRouter _brokerRouter;
        public KafkaClient(Uri[] hosts)
        {
            var options = new KafkaOptions(hosts);

            _brokerRouter = new BrokerRouter(options);
        }

        public BrokerRouter BrokerRouter
        {
            get { return _brokerRouter; }
        }
    }
}
