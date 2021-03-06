﻿using System.Text;

namespace KubernetesClient.Models
{
    public class V1DeploymentList
    {
        public string ApiVersion { get; set; }
        public V1Deployment[] Items { get; set; }
        public string Kind { get; set; }
        public V1ListMeta Metadata { get; set; }
    }
}
