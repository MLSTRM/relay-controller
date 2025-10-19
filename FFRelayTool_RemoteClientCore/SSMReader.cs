using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using FFRelayTool_Model;
using FFRelayTool_RemoteClient.Properties;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FFRelayTool_RemoteClient
{
    class SSMReader
    {
        private AmazonSimpleSystemsManagementClient client;

        public SSMReader()
        {
            client = new AmazonSimpleSystemsManagementClient(Resources.accessKey, Resources.secretKey, Amazon.RegionEndpoint.EUWest2);
        }

        public SSMStructure readParameter()
        {
            Amazon.SimpleSystemsManagement.Model.GetParameterRequest request = new Amazon.SimpleSystemsManagement.Model.GetParameterRequest();
            request.Name = Resources.SSMParam;
            request.WithDecryption = false;
            string result = client.GetParameterAsync(request).Result.Parameter.Value;
            SSMStructure structure = JsonConvert.DeserializeObject<SSMStructure>(result);
            return structure;
        }
    }
}
