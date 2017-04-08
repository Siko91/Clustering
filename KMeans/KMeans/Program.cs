using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMeans
{
    class Program
    {
        static Random rnd = new Random();
        private const int repeat = 1;

        static void Main(string[] args)
        {
            Bitmap originalImage;
            int[] countsOfColorGroups;

            try
            {
                originalImage = Bitmap.FromFile(args[0]) as Bitmap;
                countsOfColorGroups = args.Skip(1).Select(arg => (int)ushort.Parse(arg)).ToArray();
            }
            catch (Exception)
            {
                Log.Section("Instructions", () =>
                {
                    Log.Write("First argument must be a valid path to the image");
                    Log.Write("From the second argument onward use positive numbers, representing the count of colors you want in each outputs");
                });
                throw;
            }

            foreach (var count in countsOfColorGroups)
                Run(originalImage, count, args[0] + "-" + count + ".jpeg");
        }

        // TODO
        /// <summary>
        ///  ============== Plan for the program ====================
        ///  read input
        ///  read image
        ///  call KMeans 3 times
        ///     ---KMeans logic---
        ///  select the result with the smallest total distance
        ///  ========================================================
        /// </summary>
        static void Run(Bitmap originalImage, int countOfColorGroups, string resultFilename)
        {
            Log.Section("Running program with " + countOfColorGroups, () =>
            {
                Tuple<Bitmap, double> bestResult = null;
                for (int i = 1; i <= repeat; i++)
                {
                    Log.Section("Running KMeans #" + i, () =>
                    {
                        var result = KMeans(originalImage, countOfColorGroups);

                        if (bestResult == null || result.Item2 < bestResult.Item2) // if new result has smaller average mistake
                        {
                            bestResult = result;
                        }
                    });
                }

                Log.Section("Saving " + resultFilename, () =>
                {
                    if (File.Exists(resultFilename))
                        File.Delete(resultFilename);
                    bestResult.Item1.Save(resultFilename);
                });
            });
        }

        /// <summary>
        ///  ================= Plan for the method ==================
        ///     place N different positions
        ///     group all pixels in one of the positions
        ///     calculate centers of clusters
        ///     while centers != chosen positions
        ///         move positions to centers
        ///         group all pixels in one of the positions
        ///         calculate centers of clusters
        ///     calculate sum of total distances of each cluster
        ///  ========================================================
        /// </summary>
        static Tuple<Bitmap, double> KMeans(Bitmap original, int countOfColorGroups)
        {
            Log.Write("Get Image Pixels");
            Dictionary<Point, Color> pixels = Enumerable.Range(0, original.Height * original.Width)
                .Select(i => new Point(i / original.Height, i % original.Height))
                .ToDictionary(p => p, p => original.GetPixel(p.X, p.Y));

            Log.Write("Select starting Group Pillars");
            Color[] pillars = ChooseRandomColors(original, countOfColorGroups);

            Log.Write("Group pixels");
            Dictionary<Point, Color>[] groups = GroupPixels(pixels, pillars);

            Log.Write("Find group centers");
            Color[] centers = groups.Select(g => GetCenter(g)).ToArray();

            while (!AreAllEqual(pillars, centers))
            {
                Log.Write("Optimizing groups");
                pillars = centers;
                groups = GroupPixels(pixels, pillars);
                centers = groups.Select(g => GetCenter(g)).ToArray();
            }
            
            var resultPixels = pillars
                .Zip(groups, (pl, g) => g.ToDictionary(px => px.Key, px => pl))
                .SelectMany(g => g);

            double averageMisstake = pillars
                .SelectMany((p, i) => groups[i].Values.Select(gp => GetAbsDistance(p, gp)))
                .Average();

            Log.Write("Done : averageMisstake=" + averageMisstake);
            return Tuple.Create(MergeToImage(original.Size, resultPixels), averageMisstake);
        }

        private static Color Invert(Color col)
        {
            return Color.FromArgb(255 - col.R, 255 - col.G, 255 - col.B);
        }

        private static Bitmap MergeToImage(Size size, IEnumerable<KeyValuePair<Point, Color>> pixels)
        {
            var img = new Bitmap(size.Width, size.Height);
            foreach (var p in pixels)
                img.SetPixel(p.Key.X, p.Key.Y, p.Value);

            return img;
        }

        private static Color GetCenter(Dictionary<Point, Color> group)
        {
            return Color.FromArgb(
                (int)group.Values.Average(c => c.R),
                (int)group.Values.Average(c => c.G),
                (int)group.Values.Average(c => c.B));
        }

        private static Dictionary<Point, Color>[] GroupPixels(Dictionary<Point, Color> pixels, Color[] pillars)
        {
            var results = new Dictionary<Point, Color>[pillars.Length];
            for (int i = 0; i < results.Length; i++)
                results[i] = new Dictionary<Point, Color>();

            foreach (var pixel in pixels)
            {
                var weights = pillars.Select(p => GetAbsDistance(pixel.Value, p)).ToList();
                var index = weights.IndexOf(weights.Min());
                results[index].Add(pixel.Key, pixel.Value);
            }
            return results;
        }

        private static Color[] ChooseRandomColors(Bitmap original, int count)
        {
            Color[] results = new Color[count];
            for (int i = 0; i < count; i++)
            {
                int x = rnd.Next(original.Width);
                int y = rnd.Next(original.Height);
                var selectedPixel = original.GetPixel(x, y);

                if (results.Take(i).Any(color => AreEqual(color, selectedPixel)))
                {
                    i--;
                    continue;
                    //retry
                }

                results[i] = selectedPixel;
            }
            return results;
        }

        private static bool AreEqual(Color c1, Color c2, int totalTlerance = 5)
        {
            return GetAbsDistance(c1, c2) < totalTlerance;
        }

        private static bool AreAllEqual(Color[] a1, Color[] a2, int totalTlerance = 5)
        {
            for (int i = 0; i < a1.Length; i++)
                if (!AreEqual(a1[i], a2[i]))
                    return false;

            return true;
        }

        private static double GetAbsDistance(Color c1, Color c2)
        {
            var distance = Math.Sqrt(
                Math.Pow(Math.Abs(c1.R - c2.R), 2) +
                Math.Pow(Math.Abs(c1.G - c2.G), 2) +
                Math.Pow(Math.Abs(c1.B - c2.B), 2));
            return Math.Abs(distance);
        }
    }
}
