using System;
using System.Collections.Generic;
using System.Linq;

namespace Kubernetes
{
    public class Controller : IController
    {
        private readonly Dictionary<string, Pod> pods = new Dictionary<string, Pod>();

        public void Deploy(Pod pod)
        {
            if (!pods.ContainsKey(pod.Id))
            {
                pods[pod.Id] = pod;
            }
            else
            {
                Upgrade(pod);
            }
        }

        public bool Contains(string podId)
        {
            return pods.ContainsKey(podId);
        }

        public int Size()
        {
            return pods.Count;
        }

        public Pod GetPod(string podId)
        {
            if (!pods.TryGetValue(podId, out Pod pod))
            {
                throw new ArgumentException("Pod not found");
            }
            return pod;
        }

        public void Uninstall(string podId)
        {
            if (!pods.Remove(podId))
            {
                throw new ArgumentException("Pod not found");
            }
        }

        public void Upgrade(Pod pod)
        {
            if (pods.ContainsKey(pod.Id))
            {
                pods[pod.Id] = pod;
            }
            else
            {
                Deploy(pod);
            }
        }

        public IEnumerable<Pod> GetPodsInNamespace(string @namespace)
        {
            return pods.Values.Where(p => p.Namespace == @namespace);
        }

        public IEnumerable<Pod> GetPodsBetweenPort(int lowerBound, int upperBound)
        {
            return pods.Values.Where(p => p.Port >= lowerBound && p.Port <= upperBound);
        }

        public IEnumerable<Pod> GetPodsOrderedByPortThenByName()
        {
            return pods.Values.OrderByDescending(p => p.Port).ThenBy(p => p.Namespace);
        }

    }
}