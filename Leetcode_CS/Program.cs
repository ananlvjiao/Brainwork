using Leetcode_CS.Puzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leetcode_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            var maze = new Maze(5, 5);
            maze.InitMaze();
            var start = new Point(0,4);
            var end = new Point(3, 2);//new Point(4,4); //new Point(3,2);
            //var mazeSolver = new MazeSolver(maze, start, end);
            //mazeSolver.FindMinPath();
            //Console.ReadLine();

            var newSolver = new MazeSimpleSolver();
            var res = newSolver.SolveMaze(maze.Values, new int[] { start.X, start.Y }, new int[] { end.X, end.Y });
        }
    }
}
