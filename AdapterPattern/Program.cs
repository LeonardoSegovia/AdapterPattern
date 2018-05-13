using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace AdapterPattern
{
    public class Point
    {
        private int _x;
        private int _y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X
        {
            get => _x;
            set => _x = value;
        }

        public int Y
        {
            get => _y;
            set => _y = value;
        }

        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
        }

        protected bool Equals(Point other)
        {
            return _x == other._x && _y == other._y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_x * 397) ^ _y;
            }
        }
    }

    public class Line
    {

        public Point Start;
        public Point End;

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        protected bool Equals(Line other)
        {
            return Equals(Start, other.Start) && Equals(End, other.End);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Line)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Start != null ? Start.GetHashCode() : 0) * 397) ^ (End != null ? End.GetHashCode() : 0);
            }
        }
    }

    public class VectorObject : Collection<Line>
    {

    }

    public class VectorRectangle : VectorObject
    {
        public VectorRectangle(int x, int y, int width, int height)
        {
            Add(new Line(new Point(x, y), new Point(x + width, y)));
            Add(new Line(new Point(x + width, y), new Point(x + width, y + height)));
            Add(new Line(new Point(x, y), new Point(x, y + height)));
            Add(new Line(new Point(x, y + height), new Point(x + width, y + height)));
        }
    }

    public class LineToPointAdapter : IEnumerable<Point>
    {
        private static Dictionary<int, List<Point>> _cache = new Dictionary<int, List<Point>>();
        private static int _count;

        public LineToPointAdapter(Line line)
        {
            var hash = line.GetHashCode();

            if (_cache.ContainsKey(hash)) return;

            Console.WriteLine($"{++_count}: Generating points for line [{line.Start.X},{line.Start.Y}]-[{line.End.X},{line.End.Y}]");

            var points = new List<Point>();

            var left = Math.Min(line.Start.X, line.End.X);
            var right = Math.Max(line.Start.X, line.End.X);
            var top = Math.Min(line.Start.Y, line.End.Y);
            var bottom = Math.Max(line.Start.Y, line.End.Y);
            var dx = right - left;
            var dy = line.End.Y - line.Start.Y;

            if (dx == 0)
            {
                for (var y = top; y <= bottom; ++y)
                {
                    points.Add(new Point(left, y));
                }
            }
            else if (dy == 0)
            {
                for (var x = left; x <= right; ++x)
                {
                    points.Add(new Point(x, top));
                }

            }

            _cache.Add(hash, points);
        }

        public IEnumerator<Point> GetEnumerator()
        {
            return _cache.Values.SelectMany(x => x).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    class Program
    {
        private static readonly List<VectorObject> VectorObjects = new List<VectorObject>
        {
            new VectorRectangle(1, 1, 10, 10),
            new VectorRectangle(3, 3, 6, 6)
        };

        public static void DrawPoint(Point p)
        {
            Console.Write(".");
        }

        private static void Draw()
        {
            foreach (var vo in VectorObjects)
            {
                LineToPointAdapter adapter;

                foreach (var line in vo)
                {
                    adapter = new LineToPointAdapter(line);

                    adapter.ToList().ForEach(DrawPoint);
                }
            }
        }

        static void Main(string[] args)
        {
            Draw();
            Draw();

            Console.ReadKey();
        }
    }
}

