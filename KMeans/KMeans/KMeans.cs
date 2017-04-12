using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMeans
{
    public class KMeans
    {
        private static Random rnd = new Random();

        public KMeans(ushort k)
        {
            K = k;
        }

        private int K { get; set; }

        public IDictionary<double[], List<double[]>> Run(IEnumerable<double[]> data)
        {
            List<double[]> nodes = VerifyAndListifyNodes(data);
            var clusters = new Dictionary<double[], List<double[]>>();
            InitializePillars(nodes, clusters);
            clusters.First().Value.AddRange(nodes);

            bool optimized = false;
            while (!optimized)
            {
                Log.Write("Adjusting...");
                optimized = AdjustClusters(clusters);
            }

            return clusters;
        }

        public double CalculateInacurracy(IDictionary<double[], List<double[]>> clusters)
        {
            return clusters.Sum(cl => cl.Value.Sum(n => GetAbsDistance(cl.Key, n)));
        }

        private bool AdjustClusters(Dictionary<double[], List<double[]>> clusters)
        {
            var nodesToAdd = new Dictionary<double[], double[]>();
            foreach (var cluster in clusters)
            {
                var indexesToRemove = new List<int>();
                for (int i = 0; i < cluster.Value.Count; i++)
                {
                    var node = cluster.Value[i];
                    var closestNeighbour = clusters.Keys.OrderBy(cl => GetAbsDistance(cl, node)).First();
                    if (closestNeighbour != cluster.Key)
                    {
                        nodesToAdd.Add(node, closestNeighbour);
                        indexesToRemove.Add(i);
                    }
                }
                foreach (var i in indexesToRemove.OrderByDescending(i => i))
                    cluster.Value.RemoveAt(i);
            }

            foreach (var node in nodesToAdd)
                clusters[node.Value].Add(node.Key);

            double largestOptimizationDistance = 0;
            foreach (var cluster in clusters)
            {
                var oldCeter = (double[]) cluster.Key.Clone();
                for (int i = 0; i < cluster.Key.Length; i++)
                    cluster.Key[i] = cluster.Value.Average(node => node[i]);

                largestOptimizationDistance = Math.Max(largestOptimizationDistance, GetAbsDistance(oldCeter, cluster.Key));
            }
            
            bool optimized = (false == nodesToAdd.Any()) || largestOptimizationDistance < 5;
            // 'optimized' means no adjustments were made (or they were too small). 
            // That means we have decent clusters

            return optimized;
        }
        
        private double GetAbsDistance(double[] centerValues, double[] node)
        {
            double sumOfSquares = centerValues.Select((v, i) => Math.Pow(v - node[i], 2)).Sum();
            return Math.Abs(Math.Sqrt(sumOfSquares));
        }

        private void InitializePillars(List<double[]> nodes, Dictionary<double[], List<double[]>> clusters)
        {
            var randomNumbers = new List<int>();
            while (randomNumbers.Count < K)
            {
                var r = rnd.Next(nodes.Count);
                if (!randomNumbers.Contains(r))
                    randomNumbers.Add(r);
            }
            for (int i = 0; i < randomNumbers.Count; i++)
            {
                clusters.Add(nodes[randomNumbers[i]], new List<double[]>());
            }
        }

        private List<double[]> VerifyAndListifyNodes(IEnumerable<double[]> data)
        {
            if (data == null)
                throw new ArgumentException("data is null");
            List<double[]> nodes = data.ToList();
            if (!nodes.Any())
                throw new ArgumentException("Data is empty");
            return nodes;
        }
    }
}
