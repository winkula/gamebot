using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace GameBot.Game.Tetris.Extraction
{
    internal class Data
    {
        public int Count { get; set; }
        public List<int> Numbers { get; set; }

        public Data()
        {
            Numbers = new List<int>();
        }
    }

    public class NumberExtractor
    {
        private List<Bitmap> images = new List<Bitmap>();
        private IEnumerable<Point> relevantPixels = new List<Point> { new Point(1, 1), new Point(1, 2), new Point(1, 3), new Point(1, 4), new Point(1, 5), new Point(2, 2), new Point(2, 3), new Point(2, 4), new Point(2, 5), new Point(3, 3), new Point(3, 4), new Point(3, 5), new Point(4, 2), new Point(4, 5), new Point(5, 2), new Point(5, 5), new Point(6, 2), new Point(6, 3), new Point(6, 4), new Point(6, 5) };
        private List<int> numberMasks = new List<int>();
        private int[] lookup = new int[1048576 + 1];

        public NumberExtractor()
        {
            Run();
        }

        private void Run(bool verbose = false)
        {
            Build();

            for (int i = 0; i < 1048576 + 1; i++)
            {
                lookup[i] = GetBestMatchingNumber(i);
            }

            if (verbose)
            {
                File.WriteAllText(@"C:\users\winkler\desktop\lookup.txt", string.Join(",", lookup));

                Console.WriteLine("fin");
                Console.Read();
            }
        }

        void Build()
        {
            for (int i = 0; i < 10; i++)
            {
                var bm = Load(i);
                images.Add(bm);
                numberMasks.Add(GetMask(bm));

                Console.WriteLine(images[i]);
            }
            //Analyze(images);
        }

        int GetBestMatchingNumber(int mask)
        {
            int best = -1;
            int minDifference = int.MaxValue;
            for (int i = 0; i < 10; i++)
            {
                var difference = GetDifference(mask, numberMasks[i]);
                if (difference < minDifference)
                {
                    best = i;
                    minDifference = difference;
                }
            }
            //Console.WriteLine($"Min difference: {minDifference}");
            return best;
        }

        Bitmap Load(int num)
        {
            return (Bitmap)Image.FromFile(string.Format(@"C:\Users\Winkler\OneDrive\89_Projekt 2\004_Material\Tetris\Numbers\{0}.png", num));
        }

        int GetDifference(int mask, int numberMask)
        {
            return GetBitSwapCount(mask, numberMask);
        }

        int GetBitSwapCount(int x, int y)
        {
            int count = 0;
            for (int z = x ^ y; z != 0; z = z >> 1)
            {
                count += z & 1;
            }
            return count;
        }

        int GetMask(Bitmap image)
        {
            int mask = 0;

            int i = 0;
            foreach (var pixel in relevantPixels)
            {
                bool black = image.GetPixel(pixel.X, pixel.Y).GetBrightness() < 0.5;
                if (black)
                {
                    mask |= (1 << i);
                }
                i++;
            }

            return mask;
        }

        void Analyze(List<Bitmap> images)
        {
            Data[,] data = new Data[8, 8];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    data[x, y] = new Data();

                    for (int i = 0; i < images.Count; i++)
                    {
                        var bitmap = images[i];

                        int black = 1 - (int)bitmap.GetPixel(x, y).GetBrightness();
                        data[x, y].Count += black;
                        if (black == 1)
                        {
                            data[x, y].Numbers.Add(i);
                        }
                    }
                }
            }

            var res = new Bitmap(8, 8);
            var res2 = new Bitmap(8, 8);
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (data[x, y].Count != 0 && data[x, y].Count != 10)
                    {
                        //Console.Write($"({x},{y}) : {data[x, y],-6}");
                        string nums = string.Join(",", data[x, y].Numbers);
                        Console.WriteLine($"{x}\t{y}\t{data[x, y].Count,-5}\t{nums,-10}");

                        int abw = 5 - Math.Abs(5 - data[x, y].Count);
                        int alpha = abw * 51;
                        res.SetPixel(x, y, Color.FromArgb(alpha, Color.Black));


                        if (data[x, y].Count > 2 && data[x, y].Count < 8)
                        {
                            res2.SetPixel(x, y, Color.Red);
                            Debug.Write($"new Point({x}, {y}),");
                        }
                    }
                }
                //Console.WriteLine();
            }
            res.Save(@"C:\users\winkler\desktop\res.png");
            res2.Save(@"C:\users\winkler\desktop\res2.png");
        }
    }
}

