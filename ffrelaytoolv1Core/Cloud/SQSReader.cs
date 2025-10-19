using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using FFRelayTool_Model;
using ffrelaytoolv1.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var request = new ReceiveMessageRequest(Resources.inboundQueue);
            // request.WaitTimeSeconds = 2;
            ReceiveMessageResponse r = client.ReceiveMessageAsync(request).Result;
            Debug.WriteLine("Raw response contains " + r.Messages.Count + " messages with status code " + r.HttpStatusCode);
            Console.WriteLine("Raw response contains " + r.Messages.Count + " messages with status code " + r.HttpStatusCode);
            return r.Messages.Select(m => {
                var deleted = client.DeleteMessageAsync(Resources.inboundQueue, m.ReceiptHandle).Result;
                Debug.WriteLine($"deleted message with id {m.MessageId} from queue");
                Console.WriteLine($"deleted message with id {m.MessageId} from queue");
                return JsonConvert.DeserializeObject<OutboundMessage>(m.Body);
                }).ToList();
        }
    }
}
