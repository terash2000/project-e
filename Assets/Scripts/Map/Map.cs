using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviourSingleton<Map>
{
    public List<Wave> allWaves;
    public List<DialogNode> allRandomEvents;
    public List<Node> Nodes;
    public List<LineRenderer> Edges;
    [SerializeField] private float minGap;
    [SerializeField] private float maxGap;
    [SerializeField] private int numNode;
    [SerializeField] private int numLayer;
    [SerializeField] private float layerWidth;
    [SerializeField] private int maxNodePerLayer;
    [SerializeField] private GameObject battleNodePrefab;
    [SerializeField] private GameObject eventNodePrefab;
    [SerializeField] private GameObject edgePrefab;
    [SerializeField] private GameObject completePopup;
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
        if (PlayerData.path != null && PlayerData.path.Last() == Nodes.Count - 1)
        {
            completePopup.SetActive(true);
        }
        SaveSystem.Save();
    }


    public void GenerateMap()
    {
        InitAddableLayers();
        for (int i = 0; i < numNode || !AllLayersHaveNodes(); i++)
        {
            GameObject node = CreateRandomNode();
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
            Node nextNode = FindNearestNode(Nodes[i], Nodes.GetRange(0, i));
            GameObject edge = Instantiate(edgePrefab);
            edge.transform.SetParent(gameObject.transform);
            Edges.Add(edge.GetComponent<LineRenderer>());
            ConnectNodes(Nodes[i], nextNode);
            Edges.Last().SetPositions(new Vector3[] { Nodes[i].transform.position, nextNode.transform.position });
        }

        Nodes = Nodes.OrderBy(x => x.gameObject.transform.position.x).ToList();
        for (int i = 1; i < numNode; i++)
        {
            if (Nodes[i].Prev.Count > 0) continue;
            Node prevNode = FindNearestNode(Nodes[i], Nodes.GetRange(0, i));
            GameObject edge = Instantiate(edgePrefab);
            edge.transform.SetParent(gameObject.transform);
            Edges.Add(edge.GetComponent<LineRenderer>());
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
    private GameObject CreateRandomNode()
    {
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0:
                return CreateBattleNode();
            case 1:
                return CreateEventNode();
            default:
                return null;
        }
    }

    private GameObject CreateBattleNode()
    {
        GameObject battleNode = Instantiate(battleNodePrefab);
        Wave wave = allWaves[Random.Range(0, allWaves.Count)];
        battleNode.GetComponent<BattleNode>().wave = wave;
        return battleNode;
    }

    private GameObject CreateEventNode()
    {
        GameObject eventNode = Instantiate(eventNodePrefab);
        DialogNode randomEvent = allRandomEvents[Random.Range(0, allRandomEvents.Count)];
        eventNode.GetComponent<EventNode>().randomEvent = randomEvent;
        return eventNode;
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
        float startX = GetXMin() + GetMapSize().x * (layer + 0.25f) / numLayer;
        float x = Random.Range(startX, startX + 0.5f * GetMapSize().x / numLayer);
        float y = Random.Range(-GetMapSize().y / 2, GetMapSize().y / 2 - 0.5f);
        foreach (Node node in Nodes)
        {
            if ((Mathf.Abs(x - node.transform.position.x) < minGap && Mathf.Abs(y - node.transform.position.y) < minGap))
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
        //List<Node> nearestNodes = nodes.Where(x => Connectable(x, target)).ToList().OrderBy(x => Mathf.Abs(target.transform.position.y - x.transform.position.y)).ToList();
        List<Node> nearestNodes = nodes.Where(x => Connectable(x, target)).ToList().OrderBy(x => Vector3.Distance(target.transform.position, x.transform.position)).ToList();
        if (nearestNodes.Count == 0) return nodes[Random.Range(0, nodes.Count)];

        //return nearestNodes[Mathf.Min(nearestNodes.Count - 1, Random.Range(0, 2))];
        return nearestNodes[0];
    }

    public bool Connectable(Node a, Node b)
    {
        //return Mathf.Abs(a.transform.position.x - b.transform.position.x) > minGap / 2;
        int max = Random.Range(1, 2);
        if (a.Layer == 0 || a.Layer == numLayer - 1 || b.Layer == 0 || b.Layer == numLayer - 1) max = 1;
        return Mathf.Abs(a.Layer - b.Layer) <= max && a.Layer != b.Layer && !EdgesOverlap(a, b);
    }

    public bool EdgesOverlap(Node a, Node b)
    {
        foreach (LineRenderer edge in Edges)
        {
            if (EdgeOverlap(a.transform.position, b.transform.position, edge.GetPosition(0), edge.GetPosition(1))) return true;
        }
        return false;
    }
    public bool EdgeOverlap(Vector3 a1, Vector3 b1, Vector3 a2, Vector3 b2)
    {
        if (a1 == a2 || a1 == b2 || b1 == a2 || b1 == b2) return false;
        float m = (b1.y - a1.y) / (b1.x - a1.x);
        float b = -m * a1.x + a1.y;
        bool overlap = (m * a2.x + b - a2.y) * (m * b2.x + b - b2.y) < 0;
        m = (b2.y - a2.y) / (b2.x - a2.x);
        b = -m * a2.x + a2.y;
        return overlap && (m * a1.x + b - a1.y) * (m * b1.x + b - b1.y) < 0;
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
    }

    public void ResetMap()
    {
        PlayerData.seedJSON = null;
        PlayerData.path = null;
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
