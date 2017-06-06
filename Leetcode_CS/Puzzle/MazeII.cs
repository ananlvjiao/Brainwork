using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leetcode_CS.Puzzle
{
    public class MazeSolver
    {
        public Maze Maze;
        public Point StartPoint;
        public Point EndPoint;

        public MazeSolver(Maze maze, Point start, Point end)
        {
            Maze = maze;
            StartPoint = start;
            EndPoint = end;
        }

        public IList<IList<Point>> Paths = new List<IList<Point>>();

        //get next and update cur direction
        public Locator GetNextLocatorI(Stack<Locator> locators, Locator locator)
        {
            var point = new Point();
            //right, down, left, up
            while(locator.CurDir < 4)
            {
                switch (locator.CurDir)
                {
                    case 0: // go right
                        point.X = locator.Location.X;
                        point.Y = locator.Location.Y + 1;
                        break;
                    case 1: // go down
                        point.X = locator.Location.X + 1;
                        point.Y = locator.Location.Y;
                        break;
                    case 2: // go left
                        point.X = locator.Location.X;
                        point.Y = locator.Location.Y - 1;
                        break;
                    case 3: // go up
                        point.X = locator.Location.X - 1;
                        point.Y = locator.Location.Y;
                        break;
                    default: break;
                }
                if (Maze.IsAccessible(point))
                {
                    if (!locators.Select(l => l.Location).ToList().Contains(point))
                    {
                        var next = new Locator(point);
                        return next;
                    }
                }
                locator.CurDir++;
            }
            return null;
        }

        //get rolling next
        public Locator GetNextLocator(Stack<Locator> locators, Locator locator)
        {
            //right, down, left, up
            while(locator.CurDir < 4)
            {
                var nextPoint = Maze.RollingTowardsDirection(locator.Location, locator.CurDir, EndPoint);
                if (nextPoint != null)
                {
                    if (!locators.Select(l => l.Location).ToList().Contains(nextPoint))
                    {
                        var next = new Locator(nextPoint);
                        return next;
                    }
                }
                locator.CurDir++;
            }
            return null;
        }

        public void FindPath(Stack<Locator> locators)
        {
            if (locators.Count == 0)
                return;
            var top = locators.Peek();
            var next = GetNextLocator(locators, top);
            //if no next or Path contains Node or IsWall, go back
            if (next == null)
            {
                //go back one
                locators.Pop();
            }
            else if (next.Location.Equals(EndPoint))
            {
                //export this path; add lastNode and next
                var path = locators.Select(l => l.Location).Reverse().ToList();
                path.Add(EndPoint);
                Paths.Add(path);
                //go back to last node, move to next dir
                locators.Pop();
            }
            else 
            {
                top.CurDir++;
                locators.Push(next);
            }
            FindPath(locators);
        }

        public void FindMinPath()
        {
            if (StartPoint.Equals(EndPoint))
            {
                Console.WriteLine("Same points");
                return;
            }
            var initLocators = new Stack<Locator>();
            initLocators.Push(new Locator(StartPoint));
            FindPath(initLocators);
            int min = Int32.MaxValue;
            var minPath = new List<Point>();
            foreach (var path in Paths)
            {
                PrintPath(path);
                if (path.Count < min)
                {
                    minPath = path as List<Point>;
                    min = path.Count;
                }
            }
            if (minPath.Count != 0)
            {
                Console.WriteLine($"Min path length: {minPath.Count}");
                PrintPath(minPath);
            }
            else
            {
                Console.WriteLine($"No solution");
            }
        }

        private void PrintPath(IList<Point> path)
        {
            foreach (var point in path)
            {
                if (point.Equals(EndPoint))
                {
                    Console.Write("Destination!");
                    Console.WriteLine();
                }
                else
                {
                    Console.Write($"[{point.X}, {point.Y}] => ");
                }
            }
        }
    }

    public class Locator
    {
        public Point Location { get; set; }
        public int CurDir { get; set; }

        public Locator(Point point)
        {
            Location = point;
            CurDir = 0;
        }

        public override bool Equals(Object obj)
        {
            var node = obj as Locator;
            return node.Location == this.Location;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class Maze
    {
        public int Width { get; }
        public int Height { get; }

        public int[,] Values { get; set; }

        public Maze(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Values = new int[height, width];//create an uniform array
        }

        public void InitMaze()
        {
            this.Values= new int[,]{ 
                { 0, 0, 1, 0, 0 }, 
                { 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 1, 0 },
                { 1, 1, 0, 1, 1 }, 
                { 0, 0, 0, 0, 0 } };
        }

        public bool IsAccessible(Point point)
        {
            if (point.X < 0 || point.Y < 0 || point.X >= Height || point.Y >= Width)
                return false;
            return Values[point.X,point.Y] == 0;
        }

        public Point RollingTowardsDirection(Point start, int direction, Point end)
        {
            var next = new Point();
            var curr = new Point(); 
            next.X = start.X;
            next.Y = start.Y;
            bool bypass = false;
            while (IsAccessible(next))
            {
                if (next.Equals(end))
                { bypass = true; }
                curr.X = next.X;
                curr.Y = next.Y;
                switch (direction)
                {
                    case 0: // go right
                        next.Y++;
                        break;
                    case 1: // go down
                        next.X++;
                        break;
                    case 2: // go left
                        next.Y--;
                        break;
                    case 3: // go up
                        next.X--;
                        break;
                    default: break;
                }
            }
            if (bypass && !end.Equals(curr)) return null;
            else if (curr.Equals(start)) return null;
            else return curr;
        }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point() { X = -1; Y = -1; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool IsInited()
        {
            return X >= 0 && Y >= 0;
        }

        public override bool Equals(object obj)
        {
            var point = obj as Point;
            return point.X == this.X && point.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class MazeSimpleSolver
    {
        public int SolveMaze(int[,] map, int[] start, int[] end) {
            //init path[]
            int[,] path = new int[map.GetLength(0), map.GetLength(1)];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    path[i,j] = Int32.MaxValue;
                }
            }
            path[start[0], start[1]] = 0;
            dfs(map, path, new Point(start[0], start[1]));
            return path[end[0], end[1]] == Int32.MaxValue? -1: path[end[0], end[1]];
        }

        public void dfs(int[,] map, int[,] path, Point start)
        {
            //travel existing node
            // right
            for (int j = 1; j <= map.GetLength(1) - start.Y - 1; j++)
            {
                if (map[start.X, start.Y + j] == 1) break;
                //check next one and next next one
                if (start.Y + j + 1 == map.GetLength(1) || map[start.X, start.Y + j + 1] == 1)
                {
                    if ( path[start.X, start.Y] + j > path[start.X, start.Y + j]) break;
                    path[start.X, start.Y + j] = path[start.X, start.Y] + j;
                    dfs(map, path, new Point(start.X, start.Y + j));
                }
            }
            //down
            for (int i = 1; i <= map.GetLength(0) - start.X -1; i++)
            {
                if (map[start.X + i, start.Y] == 1) break;
                if (start.X + i + 1 == map.GetLength(0) || map[start.X + i + 1 , start.Y] == 1)
                {
                    if (path[start.X, start.Y] + i > path[start.X + i, start.Y]) break;
                    path[start.X + i, start.Y] = path[start.X, start.Y] + i;
                    dfs(map, path, new Point(start.X + i, start.Y));
                }
            }
            //left
            for (int m = 1; m <= start.Y; m++)
            {
                if (map[start.X, start.Y - m] == 1) break;
                if (start.Y - m == 0 || map[start.X,start.Y - m - 1] == 1)
                {
                    if (path[start.X, start.Y] + m > path[start.X, start.Y - m]) break; ;
                    path[start.X,start.Y -m ] = path[start.X,start.Y] + m;
                    dfs(map, path, new Point(start.X, start.Y  - m));
                }
            }
            //up
            for (int n = 1; n <= start.X; n++)
            {
                if (map[start.X - n, start.Y] == 1) break;
                if (start.X - n == 0 || map[start.X - n - 1,start.Y] == 1)
                {
                    if (path[start.X - n, start.Y] + n > path[start.X - n, start.Y]) break;
                    path[start.X - n, start.Y] = path[start.X, start.Y] + n;
                    dfs(map, path, new Point(start.X -n, start.Y));
                }
            }

        }

        
    }
}
