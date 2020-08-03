﻿using System.Collections.Generic;

namespace KubernetesClient.Models
{
    public class V1LabelSelector
    {
        public IList<V1LabelSelectorRequirement> MatchExpressions { get; set; }
        public IDictionary<string, string> MatchLabels { get; set; }
    }
}
