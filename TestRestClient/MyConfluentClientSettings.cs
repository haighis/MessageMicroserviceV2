using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.RestClient;

namespace TestRestClient
{
    public class MyConfluentClientSettings : IConfluentClientSettings
    {
        public string KafkaBaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["Confluent.KafkaBaseUrl"];
            }
        }
    }
}
