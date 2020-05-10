using System;
using System.Collections.Generic;
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
            TimeSpan d = DateTime.Now - timestamp;
            message.time = d.Ticks;
            client.Publish(Resources.topicARN, JsonConvert.SerializeObject(message));
        }
    }
}
