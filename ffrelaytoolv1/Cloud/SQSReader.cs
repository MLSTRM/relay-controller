using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using FFRelayTool_Model;
using ffrelaytoolv1.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ffrelaytoolv1.Cloud
{
    class SQSReader
    {
        private AmazonSQSClient client;

        public SQSReader()
        {
            client = new AmazonSQSClient(Resources.accessKey, Resources.secretKey, RegionEndpoint.EUWest2);
        }

        public List<OutboundMessage> consume()
        {
            ReceiveMessageResponse r = client.ReceiveMessage(Resources.inboundQueue);
            return r.Messages.Select(m => {
                client.DeleteMessage(Resources.inboundQueue, m.ReceiptHandle);
                return JsonConvert.DeserializeObject<OutboundMessage>(m.Body);
                }).ToList();
        }
    }
}
