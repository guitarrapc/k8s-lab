﻿using System.Collections.Generic;

namespace KubernetesClient.Models
{
    public class V1Podaffinity
    {
        public IList<V1WeightedPodAffinityTerm> preferredDuringSchedulingIgnoredDuringExecution { get; set; }
        public IList<V1PodAffinityTerm> requiredDuringSchedulingIgnoredDuringExecution { get; set; }
    }
}
