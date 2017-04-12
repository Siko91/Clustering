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
        private const int repeat = 1;

        static void Main(string[] args)
        {
            Bitmap originalImage;
            ushort[] countsOfColorGroups;

            try
            {
                originalImage = Bitmap.FromFile(args[0]) as Bitmap;
                countsOfColorGroups = args.Skip(1).Select(arg => ushort.Parse(arg)).ToArray();
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
        static void Run(Bitmap originalImage, ushort countOfColorGroups, string resultFilename)
        {
            Log.Section("Running program with " + countOfColorGroups, () =>
            {
                var kmeans = new KMeans(countOfColorGroups);

                var pixelData = Enumerable.Range(0, originalImage.Height * originalImage.Width)
                    //.Where(i => i % 2 == 1)
                    .Select(i => new Point(i / originalImage.Height, i % originalImage.Height))
                    .Select(p =>
                    {
                        var c = originalImage.GetPixel(p.X, p.Y);
                        return new double[]
                        {
                            c.R, c.G, c.B,
                            p.X,
                            p.Y
                        };
                    });

                var results = new List<IDictionary<double[], List<double[]>>>();

                for (int i = 1; i <= repeat; i++)
                {
                    Log.Section("Running KMeans #" + i, () =>
                    {
                        results.Add(kmeans.Run(pixelData));
                    });
                }

                Log.Section("Measuring different runs", () =>
                {
                    results = results.OrderBy(r =>
                    {
                        var inacurracy = kmeans.CalculateInacurracy(r);
                        Log.Write("Inaccuracy calculated : " + inacurracy);
                        return inacurracy;
                    }).ToList();
                });

                Log.Section("Saving " + resultFilename, () =>
                {
                    int count = 1;
                    string fileNameOnly = Path.GetFileNameWithoutExtension(resultFilename);
                    string extension = Path.GetExtension(resultFilename);
                    string path = Path.GetDirectoryName(resultFilename);
                    string newFullPath = resultFilename;

                    while (File.Exists(newFullPath))
                    {
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                        newFullPath = Path.Combine(path, tempFileName + extension);
                    }

                    MergeToImage(originalImage.Size, results.First()).Save(newFullPath);
                });
            });
        }

        private static Bitmap MergeToImage(Size size, IDictionary<double[], List<double[]>> clusters)
        {
            var img = new Bitmap(size.Width, size.Height);
            foreach (var cl in clusters)
            {
                var color = Color.FromArgb((int)cl.Key[0], (int)cl.Key[1], (int)cl.Key[2]);
                foreach (var px in cl.Value)
                {
                    img.SetPixel((int)px[3], (int)px[4], color);

                    //// adding missing pixel
                    //var w = (int)((px[3] * px[4] + 1) / size.Height);
                    //var h = (int)((px[3] * px[4] + 1) % size.Height);
                    //img.SetPixel(w, h, color);
                }
            }

            return img;
        }
    }
}
