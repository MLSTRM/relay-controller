using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
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
            Amazon.SimpleSystemsManagement.Model.GetParametersRequest paramR = new Amazon.SimpleSystemsManagement.Model.GetParametersRequest();
            paramR.Names = new List<string>(){ "*"};
            GetParametersResponse parameters = client.GetParameters(paramR);
            string result = client.GetParameter(request).Parameter.Value;
            SSMStructure structure = JsonConvert.DeserializeObject<SSMStructure>(result);
            return structure;
        }
    }

    public class SSMStructure
    {
        public SSMTeam[] teams;
        public long timestamp;

        public class SSMTeam
        {
            public string name;
            public string activeSplit;
            public string color;
        }
    }
}
