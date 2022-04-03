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
    [SerializeField] private int numLayer;
    [SerializeField] private float layerWidth;
    [SerializeField] private int maxNodePerLayer;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject edgePrefab;
    private Node curNode;
    [HideInInspector] public float WidthScale;
    private List<int> addableLayers;

    void Start()
    {
        SetMapSize();
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
        InitAddableLayers();
        for (int i = 0; i < numNode || !AllLayersHaveNodes(); i++)
        {
            GameObject node = Instantiate(nodePrefab);
            int layer = RandomLayer();
            node.GetComponent<Node>().Init(layer);
            node.transform.position = RandomPos(layer);
            node.transform.SetParent(gameObject.transform);
            Nodes.Add(node.GetComponent<Node>());
            SetAddableLayers(layer);
            if (addableLayers.Count == 0)
            {
                numNode = i + 1;
                break;
            }
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

    public float GetXMin()
    {
        return transform.position.x - GetMapSize().x / 2;
    }

    public float GetXMax()
    {
        return transform.position.x + GetMapSize().x / 2;
    }


    public bool AllLayersHaveNodes()
    {
        for (int i = 0; i < numLayer; i++)
        {
            if (GetNodesInLayer(i).Count == 0) return false;
        }
        return true;
    }

    public void InitAddableLayers()
    {
        addableLayers = new List<int>();
        for (int i = 0; i < numLayer; i++)
        {
            addableLayers.Add(i);
        }
    }

    public void SetAddableLayers(int layer)
    {
        int max;
        if (layer == 0 || layer == numLayer - 1) max = 1;
        else max = maxNodePerLayer;
        if (GetNodesInLayer(layer).Count >= max) addableLayers.Remove(layer);
    }
    public int RandomLayer()
    {
        int n = addableLayers.Count;
        return addableLayers[Random.Range(0, n)];
    }

    public List<Node> GetNodesInLayer(int layer)
    {
        List<Node> nodesInLayer = new List<Node>();
        foreach (Node node in Nodes)
        {
            if (node.Layer == layer) nodesInLayer.Add(node);
        }
        return nodesInLayer;
    }

    public Vector3 RandomPos(int layer)
    {
        float startX = GetXMin() + GetMapSize().x * layer / numLayer;
        float x = Random.Range(startX, startX + GetMapSize().x / numLayer);
        float y = Random.Range(-GetMapSize().y / 2, GetMapSize().y / 2);
        foreach (Node node in Nodes)
        {
            if ((Mathf.Abs(x - node.transform.position.x) < minGap && Mathf.Abs(y - node.transform.position.y) < minGap) || Mathf.Abs(x - node.transform.position.x) < minGap / 2)
            {
                return RandomPos(layer);
            }
        }
        foreach (Node node in GetNodesInLayer(layer))
        {
            if (Mathf.Abs(y - node.transform.position.y) < minGap)
            {
                return RandomPos(layer);
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
            //float dis = Mathf.Abs(target.transform.position.y - node.transform.position.y);
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
        /*int max = Random.Range(1, 3);
        if (a.Layer == 0 || a.Layer == numLayer - 1 || b.Layer == 0 || b.Layer == numLayer - 1) max = 1;
        return Mathf.Abs(a.Layer - b.Layer) <= max && a.Layer != b.Layer;*/
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
            prev = i;
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

    public void SetMapSize()
    {
        Vector2 size = GetMapSize();
        float oriWidth = size.x;
        size.x = numLayer * layerWidth;
        WidthScale = size.x / oriWidth;
        GetComponent<RectTransform>().sizeDelta = size;
        Vector3 pos = transform.position;
        pos.x = (numLayer * layerWidth - oriWidth) / 2;
        GetComponent<RectTransform>().position = pos;
    }

}
