﻿using System.Collections.Generic;

namespace KubernetesClient.Models
{
    public class V1Nodeaffinity
    {
        public IList<V1PreferredSchedulingTerm> preferredDuringSchedulingIgnoredDuringExecution { get; set; }
        public V1NodeSelector requiredDuringSchedulingIgnoredDuringExecution { get; set; }
    }
}
