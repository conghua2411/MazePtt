using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Maze generation
/// Use Growing binary tree
/// </summary>
public class EdgeMazeGen
{
    int RandomInMax(int max)
    {
        return Random.Range(0, max);
    }

    Point RandomPointInMaze(int mazeSize)
    {
        return new Point(RandomInMax(mazeSize), RandomInMax(mazeSize));
    }

    bool IsInMaze(Point p, int mazeSize)
    {
        if (p.x < 0 || p.x >= mazeSize)
            return false;
        if (p.y < 0 || p.y >= mazeSize)
            return false;
        return true;
    }

    List<Point> PosGoNext(Point curPoint, List<Point> visitedPoint, int mazeSize)
    {
        List<Point> DirCanGo = new List<Point>
            {
                new Point(-1, 0),       /// left
                new Point(1, 0),        /// right
                new Point(0, -1),       /// top
                new Point(0, 1),        /// bottom
            };

        List<Point> goNext = new List<Point>();

        /// top
        /// 
        foreach (Point item in DirCanGo)
        {
            Point target = item + curPoint;
            if (IsInMaze(target, mazeSize) && !visitedPoint.Contains(target))
            {
                goNext.Add(target);
            }
        }

        if (goNext.Count <= 2)
        {
            return goNext;
        }

        /// get random 2 point from goNext
        List<Point> goNextRandom = new List<Point>();

        Point p1 = goNext[RandomInMax(goNext.Count)];

        goNextRandom.Add(p1);

        goNext.Remove(p1);

        goNextRandom.Add(goNext[RandomInMax(goNext.Count)]);

        return goNextRandom;
    }

    public List<Edge> GenListEdge(int mazeSize)
    {
        List<Edge> listEdge = new List<Edge>();

        List<Point> visitedPoint = new List<Point>();

        Point startPoint = RandomPointInMaze(mazeSize);

        visitedPoint.Add(startPoint);

        List<Point> checkedPoint = new List<Point>();

        checkedPoint.Add(startPoint);

        while (visitedPoint.Count != 0)
        {
            int randomIndex = RandomInMax(visitedPoint.Count);
            Point cur = visitedPoint[randomIndex];

            /// get list pos can go
            /// 
            List<Point> posCanGo = PosGoNext(cur, checkedPoint, mazeSize);

            if (posCanGo.Count == 0)
            {
                //visitedPoint.RemoveAt(cur);
            }
            else
            {
                for (int i = 0; i < posCanGo.Count; i++)
                {
                    listEdge.Add(new Edge(cur, posCanGo[i]));
                }

                visitedPoint.AddRange(posCanGo);
                checkedPoint.AddRange(posCanGo);
            }

            visitedPoint.Remove(cur);

        }

        return listEdge;
    }
}

public class Point
{
    public int x;
    public int y;

    public Point()
    {
        this.x = 0;
        this.y = 0;
    }

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Point(Point p)
    {
        x = p.x;
        y = p.y;
    }

    public override bool Equals(object obj)
    {
        var point = (Point)obj;
        return point != null &&
               x == point.x &&
               y == point.y;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 31 + x.GetHashCode();
        hash = hash * 31 + y.GetHashCode();
        return hash;
    }

    public static Point Clone(Point p)
    {
        return new Point(p.x, p.y);
    }

    public override string ToString()
    {
        return string.Format("{0} - {1}", x, y);
    }

    public static Point operator +(Point a, Point b)
    => new Point(a.x + b.x, a.y + b.y);
}

public class Edge
{
    Point start;
    Point end;

    public Edge(Point start, Point end)
    {
        this.start = start;
        this.end = end;
    }

    public override bool Equals(object obj)
    {
        Edge other = (Edge)obj;
        return other != null && ((this.start.Equals(other.start) && this.end.Equals(other.end)) || ((this.end.Equals(other.start) && this.start.Equals(other.end))));
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 31 + start.GetHashCode();
        hash = hash * 31 + end.GetHashCode();
        return hash;
    }

    public override string ToString()
    {
        return $"Edge: ({start}) - ({end})";
    }

    public bool ContainPoint(Point p)
    {
        if (start.Equals(p) || end.Equals(p))
        {
            return true;
        }

        return false;
    }

    public Point GetOtherPoint(Point p)
    {
        /// maze don't have point(-1, -1)
        if (!ContainPoint(p))
            return new Point(-1, -1);

        if (p.Equals(start))
            return end;

        return start;
    }
}
