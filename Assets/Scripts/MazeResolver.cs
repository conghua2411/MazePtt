using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeResolver
{
    public static List<Point> SolveMaze(Point start, Point end, List<Edge> mazeEdges, List<Point> pointGoThrough)
    {
        pointGoThrough.Add(start);

        if (start.Equals(end))
        {
            return pointGoThrough;
        }

        List<Edge> edgeHavePoint = FindEdgeHavePoint(start, mazeEdges);

        for (int i = 0; i < edgeHavePoint.Count; i++)
        {
            Point p = new Point(edgeHavePoint[i].GetOtherPoint(start));

            if (pointGoThrough.Contains(p))
            {
                continue;
            }

            List<Point> path = SolveMaze(p, end, mazeEdges, pointGoThrough);

            if (path.Count != 0 && path[path.Count-1].Equals(end))
            {
                return path;
            }
        }

        if (start != end)
        {
            pointGoThrough.Remove(start);
        }

        return pointGoThrough;
    }


    public static List<Edge> FindEdgeHavePoint(Point point, List<Edge> mazeEdges)
    {
        List<Edge> edges = new List<Edge>();
        for (int i = 0; i < mazeEdges.Count; i++)
        {
            if (mazeEdges[i].ContainPoint(point))
            {
                edges.Add(mazeEdges[i]);
            }
        }

        return edges;
    }
}
