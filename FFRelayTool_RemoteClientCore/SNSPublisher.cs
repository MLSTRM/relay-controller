using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.SimpleNotificationService;
using FFRelayTool_Model;
using FFRelayTool_RemoteClient.Properties;
using Newtonsoft.Json;

namespace FFRelayTool_RemoteClient
{
    public class SNSPublisher
    {
        private AmazonSimpleNotificationServiceClient client;

        public SNSPublisher()
        {
            client = new AmazonSimpleNotificationServiceClient(Resources.accessKey, Resources.secretKey, RegionEndpoint.EUWest2);
        }

        public void publish(string team, DateTime timestamp)
        {
            OutboundMessage message = new OutboundMessage();
            message.team = team;
            TimeSpan d = DateTime.Now.ToUniversalTime() - timestamp;
            message.time = d.Ticks;
            var response = client.PublishAsync(Resources.topicARN, JsonConvert.SerializeObject(message)).Result;
            Debug.WriteLine(response.MessageId + " (" + response.HttpStatusCode + ")");
            Console.WriteLine(response.MessageId + " (" + response.HttpStatusCode + ")");
        }
    }
}
