//using System;
//using System.Linq;
//using System.IO;
//using System.Text;
//using System.Collections;
//using System.Collections.Generic;

//class Solution
//{
//    private const char Wall = '#';
//    private const char Empty = '.';
//    private const char Mix = '+';
//    private static readonly CanID MixId = new (Mix, new(-1, -1));

//    private static bool IsCan(char c) =>
//        c != Wall && c != Empty && c != Mix;

//    private static void WriteResult(char[,] res, int width, int height)
//    {
//        for (int y = 0; y < height; y++)
//        {
//            var line = new StringBuilder();
//            for (int x = 0; x < width; x++)
//            {
//                line.Append(res[y, x]);
//            }
//            Console.WriteLine(line);
//        }
//    }

//    private static void PrintGrid(char[,] res, int width, int height)
//    {
//        for (int y = 0; y < height; y++)
//        {
//            var line = new StringBuilder();
//            for (int x = 0; x < width; x++)
//            {
//                line.Append(res[y, x]);
//            }
//            Console.Error.WriteLine(line);
//        }
//    }


//    static void Main(string[] args)
//    {
//        var width = int.Parse(Console.ReadLine());
//        var height = int.Parse(Console.ReadLine());

//        var grid = new char[height, width];
//        //var paintCans = new List<Node>(width * height);
//        var paintCans = new Dictionary<Int2, CanID>(width * height);

//        for (int y = 0; y < height; y++)
//        {
//            var line = Console.ReadLine()!;
//            for (int x = 0; x < width; x++)
//            {
//                var charValue = line[x];
//                grid[y, x] = charValue;
//                if (IsCan(charValue))
//                {
//                    var startPos = new Int2(x, y);
//                    paintCans.Add(startPos, new CanID(charValue, startPos));
//                }
//            }
//        }

//        while (true)
//        {
//            //Find all spaces that should be painted per can
//            //If spot is alredy painted, set it to mix
//            //Update grid

//            var paintedSpots = new Dictionary<Int2, CanID>(paintCans.Count * 4);

//            void TryAddSpot(Int2 spot, CanID paintCan)
//            {
//                if (paintedSpots.ContainsKey(spot))
//                {
//                    //One can cannot mix with itself
//                    if (!paintedSpots[spot].Equals(paintCan))
//                    {
//                        paintedSpots[spot] = MixId;
//                    }
//                }
//                else
//                {
//                    paintedSpots.Add(spot,paintCan);
//                }
//            }

//            //Populate painted spots
//            foreach (var paintCan in paintCans)
//            {
//                var up = new Int2(paintCan.Key.X, paintCan.Key.Y + 1);
//                var down = new Int2(paintCan.Key.X, paintCan.Key.Y - 1);
//                var left = new Int2(paintCan.Key.X - 1, paintCan.Key.Y);
//                var right = new Int2(paintCan.Key.X + 1, paintCan.Key.Y);

//                //Up
//                if (paintCan.Key.Y < height - 1)
//                {
//                    if (grid[up.Y, up.X] == Empty)
//                    {
//                        TryAddSpot(up, paintCan.Value);
//                    }
//                }
//                //Down
//                if (paintCan.Key.Y > 0)
//                {
//                    if (grid[down.Y, down.X] == Empty)
//                    {
//                        TryAddSpot(down, paintCan.Value);
//                    }
//                }
//                //Left
//                if (paintCan.Key.X > 0)
//                {
//                    if (grid[left.Y, left.X] == Empty)
//                    {
//                        TryAddSpot(left, paintCan.Value);
//                    }
//                }
//                //Right
//                if (paintCan.Key.X < width - 1)
//                {
//                    if (grid[right.Y, right.X] == Empty)
//                    {
//                        TryAddSpot(right, paintCan.Value);
//                    }
//                }
//            }

//            //Update grid & paint cans
//            paintCans.Clear();
//            var didGridChange = false;
//            foreach (var paintedSpot in paintedSpots)
//            {
//                if (grid[paintedSpot.Key.Y, paintedSpot.Key.X] != paintedSpot.Value.Appearance)
//                {
//                    didGridChange = true;
//                    grid[paintedSpot.Key.Y, paintedSpot.Key.X] = paintedSpot.Value.Appearance;
//                    paintCans.Add(paintedSpot.Key, paintedSpot.Value);
//                }
//            }

//            //PrintGrid(grid, width, height);
//            //Console.Error.WriteLine("----\n\n-----");

//            //If we've made no changes, we're done
//            if (!didGridChange)
//                break;
//        }

//        //Result
//        WriteResult(grid, width, height);
//    }

//    public readonly struct Int2 : IEquatable<Int2>
//    {
//        public readonly int X;
//        public readonly int Y;

//        public Int2(int x, int y)
//        {
//            X = x;
//            Y = y;
//        }

//        public override int GetHashCode()
//        {
//            return HashCode.Combine(X, Y);
//        }

//        public bool Equals(Int2 other)
//        {
//            return X == other.X && Y == other.Y;
//        }

//        public override bool Equals(object? obj)
//        {
//            return obj is Int2 other && Equals(other);
//        }
//    }

//    public readonly struct CanID : IEquatable<CanID>
//    {
//        public readonly char Appearance;
//        public readonly Int2 StartPosition;

//        public CanID(char appearance, Int2 startPosition)
//        {
//            Appearance = appearance;
//            StartPosition = startPosition;
//        }

//        public bool Equals(CanID other)
//        {
//            return Appearance == other.Appearance && StartPosition.Equals(other.StartPosition);
//        }

//        public override bool Equals(object? obj)
//        {
//            return obj is CanID other && Equals(other);
//        }

//        public override int GetHashCode()
//        {
//            return HashCode.Combine(Appearance, StartPosition);
//        }
//    }
//}  