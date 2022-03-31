using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviourSingleton<Map>
{
    public List<Node> Nodes;
    public List<LineRenderer> Edges;
    [SerializeField] private float minGap;
    [SerializeField] private float maxGap;
    [SerializeField] private int numNode;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject edgePrefab;
    private Node curNode;

    void Start()
    {
        // load seed
        if (PlayerData.seedJSON != null)
        {
            Random.state = JsonUtility.FromJson<Random.State>(PlayerData.seedJSON);
        }
        else
        {
            PlayerData.seedJSON = JsonUtility.ToJson(Random.state);
        }

        GenerateMap();

        if (PlayerData.path != null && PlayerData.path.Count > 0)
        {
            curNode = Nodes[PlayerData.path.Last()];
        }
        else
        {
            PlayerData.path = new List<int>();
            AddNodeToPath(Nodes[0]);
        }
        ShowPath();
        // re-randomize
        Random.InitState(System.Environment.TickCount);

        SaveSystem.Save();
    }

    public void GenerateMap()
    {
        for (int i = 0; i < numNode; i++)
        {
            GameObject node = Instantiate(nodePrefab);
            node.GetComponent<Node>().Init();
            node.transform.position = RandomPos();
            node.transform.SetParent(gameObject.transform);
            Nodes.Add(node.GetComponent<Node>());
        }
        Nodes = Nodes.OrderByDescending(x => x.gameObject.transform.position.x).ToList();
        Nodes[0].transform.position = new Vector3(Nodes[0].transform.position.x, 0, transform.position.z);
        Nodes.Last().transform.position = new Vector3(Nodes.Last().transform.position.x, 0, transform.position.z);

        for (int i = 1; i < numNode; i++)
        {
            GameObject edge = Instantiate(edgePrefab);
            edge.transform.SetParent(gameObject.transform);
            Edges.Add(edge.GetComponent<LineRenderer>());
            Node nextNode = FindNearestNode(Nodes[i], Nodes.GetRange(0, i));
            ConnectNodes(Nodes[i], nextNode);
            Edges.Last().SetPositions(new Vector3[] { Nodes[i].transform.position, nextNode.transform.position });
        }

        Nodes = Nodes.OrderBy(x => x.gameObject.transform.position.x).ToList();

        for (int i = 1; i < numNode; i++)
        {
            if (Nodes[i].Prev.Count > 0) continue;
            GameObject edge = Instantiate(edgePrefab);
            edge.transform.SetParent(gameObject.transform);
            Edges.Add(edge.GetComponent<LineRenderer>());
            Node prevNode = FindNearestNode(Nodes[i], Nodes.GetRange(0, i));
            ConnectNodes(prevNode, Nodes[i]);
            Edges.Last().SetPositions(new Vector3[] { prevNode.transform.position, Nodes[i].transform.position });
        }
    }

    public Vector3 RandomPos()
    {
        float x = Random.Range(-GetMapSize().x / 2, GetMapSize().x / 2);
        float y = Random.Range(-GetMapSize().y / 2, GetMapSize().y / 2);
        foreach (Node node in Nodes)
        {
            if (Mathf.Abs(x - node.transform.position.x) < minGap && Mathf.Abs(y - node.transform.position.y) < minGap)
            {
                return RandomPos();
            }
        }
        return new Vector3(x, y, transform.position.z);
    }

    public void ConnectNodes(Node oriNode, Node desNode)
    {
        oriNode.Next.Add(desNode);
        desNode.Prev.Add(oriNode);

    }

    public Node FindNearestNode(Node target, List<Node> nodes)
    {
        float min = Mathf.Infinity;
        Node nearestNode = null;
        foreach (Node node in nodes.Where(x => Connectable(x, target)).ToList())
        {
            float dis = Vector3.Distance(target.transform.position, node.transform.position);
            if (min > dis)
            {
                min = dis;
                nearestNode = node;
            }
        }
        if (nearestNode == null) return nodes[Random.Range(0, nodes.Count)];
        return nearestNode;
    }

    public bool Connectable(Node a, Node b)
    {
        return Mathf.Abs(a.transform.position.x - b.transform.position.x) > minGap / 2;
    }

    public List<Node> FindStartNodes()
    {
        return Nodes.Where(x => x.Prev.Count == 0).ToList();
    }

    public void ShowPath()
    {
        int prev = -1;
        foreach (int i in PlayerData.path)
        {
            if (prev >= 0)
            {
                LineRenderer line = FindEdge(Nodes[prev], Nodes[i]);
                line.startColor = Color.green;
                line.endColor = Color.green;
            }
            Nodes[i].OnPass();
        }
        foreach (Node node in curNode.Next)
        {
            LineRenderer line = FindEdge(curNode, node);
            line.startColor = Color.cyan;
            line.endColor = Color.cyan;
            node.OnClickable();
        }
    }

    public void ShowUpdatePath()
    {
        curNode.OnPass();
        foreach (Node node in curNode.Next)
        {
            LineRenderer line = FindEdge(curNode, node);
            line.startColor = Color.cyan;
            line.endColor = Color.cyan;
            node.OnClickable();
        }
    }

    public void AddNodeToPath(Node node)
    {
        //if (FindStartNodes().Contains(node)) 
        PlayerData.path.Add(Nodes.IndexOf(node));
        UpdateOldPath(node);
        curNode = node;
        ShowUpdatePath();
        SaveSystem.Save();
    }

    public void UpdateOldPath(Node newCurNode)
    {
        List<Node> removeClickableNodes;
        if (curNode == null) removeClickableNodes = FindStartNodes();
        else
        {
            removeClickableNodes = curNode.Next;
            LineRenderer line = FindEdge(curNode, newCurNode);
            line.startColor = Color.green;
            line.endColor = Color.green;
        }
        removeClickableNodes.Remove(newCurNode);
        foreach (Node node in removeClickableNodes)
        {
            node.OnReset();
            if (curNode != null) ResetEdge(FindEdge(curNode, node));
        }
    }

    public LineRenderer FindEdge(Node oriNode, Node desNode)
    {
        return Edges.Find(x => x.GetPosition(0).Equals(oriNode.transform.position) && x.GetPosition(1).Equals(desNode.transform.position));
    }

    public void ResetEdge(LineRenderer line)
    {
        Vector3[] pos = new Vector3[2];
        line.GetPositions(pos);
        Edges.Remove(line);
        GameObject.Destroy(line.gameObject);
        GameObject edge = Instantiate(edgePrefab);
        edge.transform.SetParent(gameObject.transform);
        Edges.Add(edge.GetComponent<LineRenderer>());
        Edges.Last().SetPositions(pos);
    }

    public Vector2 GetMapSize()
    {
        return GetComponent<RectTransform>().sizeDelta;
    }
}
