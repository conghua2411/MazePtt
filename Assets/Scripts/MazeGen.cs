using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Maze manager
/// Draw maze
/// </summary>
public class MazeGen : MonoBehaviour
{
    [SerializeField]
    private WallPool wallPool;

    [SerializeField]
    private Transform wallBorderParent;

    [SerializeField]
    private Transform pathParent;

    EdgeMazeGen edgeMazeGen = new EdgeMazeGen();

    public static int MAZE_SIZE = 8;

    [SerializeField]
    private int mazeSize = 8;

    [SerializeField]
    private GameObject startPointPb;

    [SerializeField]
    private GameObject endPointPb;

    private GameObject startGO;
    private GameObject endGO;

    [SerializeField]
    private bool useCoroutine = false;

    [SerializeField]
    private float drawSpeed = 0.2f;

    private bool isDrawing = false;

    private List<Edge> currentEdgeMaze;
    private Point start;
    private Point end;

    private bool solved = false;

    private void Update()
    {
        if (isDrawing)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.O))
        {
            GenNewMaze();
            solved = false;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.P))
        {
            SolveMaze();
        }
    }

    /// <summary>
    /// solve maze and draw path
    /// </summary>
    private void SolveMaze()
    {
        if (solved)
            return;

        solved = true;

        if (currentEdgeMaze != null)
        {
            List<Point> path = MazeResolver.SolveMaze(start, end, currentEdgeMaze, new List<Point>());

            if (useCoroutine)
            {
                isDrawing = true;
                StartCoroutine(DrawPathCoroutine(path));
            }
            else
            {
                DrawPath(path);
            }
        }
    }

    private IEnumerator DrawPathCoroutine(List<Point> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            GameObject go = wallPool.GetWall();

            if (pathParent != null)
            {
                go.transform.parent = pathParent;
            }

            /// position
            ///
            Vector3 pos = new Vector3((path[i].x + path[i + 1].x) / 2f - mazeSize / 2f + 0.5f, (path[i].y + path[i + 1].y) / 2f - mazeSize / 2f + 0.5f, 0);

            go.transform.position = pos;

            /// check rotation
            ///
            if (path[i].x == path[i + 1].x)
            {
                go.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (path[i].y == path[i + 1].y)
            {
                go.transform.rotation = Quaternion.Euler(0, 0, 90);
            }

            go.SetActive(true);

            yield return new WaitForSeconds(drawSpeed);
        }
        isDrawing = false;
    }

    private void DrawPath(List<Point> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            GameObject go = wallPool.GetWall();

            if (pathParent != null)
            {
                go.transform.parent = pathParent;
            }

            /// position
            ///
            Vector3 pos = new Vector3((path[i].x + path[i + 1].x) / 2f - mazeSize / 2f + 0.5f, (path[i].y + path[i + 1].y) / 2f - mazeSize / 2f + 0.5f, 0);

            go.transform.position = pos;

            /// check rotation
            ///
            if (path[i].x == path[i + 1].x)
            {
                go.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            if (path[i].y == path[i + 1].y)
            {
                go.transform.rotation = Quaternion.Euler(0, 0, 90);
            }

            go.SetActive(true);
        }
    }


    /// <summary>
    /// gen maze and draw maze
    /// </summary>
    private void GenNewMaze()
    {

        List<Edge> edges = edgeMazeGen.GenListEdge(mazeSize);

        //for (int i = 0; i < edges.Count; i++)
        //{
        //    MyDebug.Message(edges[i]);
        //}

        SetUpMaze(edges, mazeSize);

        Point startPoint = new Point(Random.Range(0, mazeSize), Random.Range(0, mazeSize));

        Point endPoint;

        do
        {
            endPoint = new Point(Random.Range(0, mazeSize), Random.Range(0, mazeSize));
        } while (startPoint.Equals(endPoint));

        /// back up maze for solving
        ///
        currentEdgeMaze = edges;
        start = startPoint;
        end = endPoint;

        if (startGO == null && endGO == null)
        {
            startGO = Instantiate(startPointPb, new Vector3(startPoint.x + 0.5f - mazeSize / 2f, startPoint.y + 0.5f - mazeSize / 2f, 0), Quaternion.identity);
            endGO = Instantiate(endPointPb, new Vector3(endPoint.x + 0.5f - mazeSize / 2f, endPoint.y + 0.5f - mazeSize / 2f, 0), Quaternion.identity);
        } 
        else
        {
            startGO.transform.position = new Vector3(startPoint.x + 0.5f - mazeSize / 2f, startPoint.y + 0.5f - mazeSize / 2f);
            endGO.transform.position = new Vector3(endPoint.x + 0.5f - mazeSize / 2f, endPoint.y + 0.5f - mazeSize / 2f);
        }

    }

    private void SetUpMaze(List<Edge> edges, int mazeSize)
    {
        if (wallPool != null)
        {
            wallPool.CleanPool();

            InstanceWallBorder(mazeSize);

            if (useCoroutine)
            {
                isDrawing = true;
                StartCoroutine(DrawMaze2(edges, mazeSize));
            } 
            else
            {
                DrawMaze(edges, mazeSize);
            }
        }
    }

    private IEnumerator DrawMaze2(List<Edge> edges, int mazeSize)
    {
        for (int i = 0; i < mazeSize; i++)
        {
            for (int j = 0; j < mazeSize; j++)
            {
                if (i < mazeSize - 1)
                {
                    /// right
                    Edge edgeRight = new Edge(new Point(i, j), new Point(i + 1, j));

                    if (!edges.Contains(edgeRight))
                    {
                        InstanceWall(i + 1 - mazeSize / 2f, j + 0.5f - mazeSize / 2f, true);
                        yield return new WaitForSeconds(drawSpeed);
                    }
                }

                if (j < mazeSize - 1)
                {
                    /// top
                    Edge edgeBottom = new Edge(new Point(i, j), new Point(i, j + 1));

                    if (!edges.Contains(edgeBottom))
                    {
                        InstanceWall(i + 0.5f - mazeSize / 2f, j + 1 - mazeSize / 2f, false);
                        yield return new WaitForSeconds(drawSpeed);
                    }
                }
            }
        }
        isDrawing = false;
    }

    private void DrawMaze(List<Edge> edges, int mazeSize)
    {
        for (int i = 0; i < mazeSize; i++)
        {
            for (int j = 0; j < mazeSize; j++)
            {
                if (i < mazeSize - 1)
                {
                    /// right
                    Edge edgeRight = new Edge(new Point(i, j), new Point(i + 1, j));

                    if (!edges.Contains(edgeRight))
                    {
                        InstanceWall(i + 1 - mazeSize / 2f, j + 0.5f - mazeSize / 2f, true);
                    }
                }

                if (j < mazeSize - 1)
                {
                    /// top
                    Edge edgeBottom = new Edge(new Point(i, j), new Point(i, j + 1));

                    if (!edges.Contains(edgeBottom))
                    {
                        InstanceWall(i + 0.5f - mazeSize / 2f, j + 1 - mazeSize / 2f, false);
                    }
                }
            }
        }
    }

    private void InstanceWallBorder(int mazeSize)
    {
        GameObject go;

        /// top
        for (int i = 0; i < mazeSize; i++)
        {
            go = wallPool.GetWall();
            go.transform.position = new Vector3(i + 0.5f - mazeSize / 2f, mazeSize - mazeSize / 2f, 0);
            go.transform.rotation = Quaternion.Euler(0, 0, 90);
            go.SetActive(true);
            go.transform.parent = wallBorderParent;

        }

        /// bottom
        for (int i = 0; i < mazeSize; i++)
        {
            go = wallPool.GetWall();
            go.transform.position = new Vector3(i + 0.5f - mazeSize / 2f, 0 - mazeSize / 2f, 0);
            go.transform.rotation = Quaternion.Euler(0, 0, 90);
            go.SetActive(true);
            go.transform.parent = wallBorderParent;

        }

        /// left
        for (int i = 0; i < mazeSize; i++)
        {
            go = wallPool.GetWall();
            go.transform.rotation = Quaternion.Euler(0, 0, 0);
            go.transform.position = new Vector3(0 - mazeSize / 2f, i + 0.5f - mazeSize / 2f, 0);
            go.SetActive(true);
            go.transform.parent = wallBorderParent;

        }

        /// right
        for (int i = 0; i < mazeSize; i++)
        {
            go = wallPool.GetWall();
            go.transform.rotation = Quaternion.Euler(0, 0, 0);
            go.transform.position = new Vector3(mazeSize / 2f, i + 0.5f - mazeSize / 2f, 0);
            go.SetActive(true);
            go.transform.parent = wallBorderParent;
        }
    }

    private void InstanceWall(float x, float y, bool isRight)
    {
        GameObject go = wallPool.GetWall();

        go.transform.position = new Vector3(x, y, 0);

        if (isRight)
        {
            go.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            go.transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        go.SetActive(true);
    }
}

