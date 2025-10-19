using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using FFRelayTool_Model;
using ffrelaytoolv1.Properties;
using Newtonsoft.Json;

namespace ffrelaytoolv1.Cloud
{
    class SSMUpdater
    {
        private AmazonSimpleSystemsManagementClient client;

        public SSMUpdater()
        {
            client = new AmazonSimpleSystemsManagementClient(Resources.accessKey, Resources.secretKey, RegionEndpoint.EUWest2);
        }

        public void updateValue(SSMStructure structure)
        {
            PutParameterRequest request = new PutParameterRequest();
            request.Overwrite = true;
            request.Name = Resources.ssmParameter;
            request.Value = JsonConvert.SerializeObject(structure);
            client.PutParameterAsync(request).Wait();
        }
    }
}
