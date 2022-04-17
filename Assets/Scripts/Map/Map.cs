using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviourSingleton<Map>
{
    public List<Wave> AllWaves;
    public List<DialogNode> AllRandomEvents;

    [SerializeField] private float _minGap;
    [SerializeField] private float _maxGap;
    [SerializeField] private int _numNode;
    [SerializeField] private int _numLayer;
    [SerializeField] private float _layerWidth;
    [SerializeField] private int _minNodePerLayer;
    [SerializeField] private int _maxNodePerLayer;
    [SerializeField] private GameObject _battleNodePrefab;
    [SerializeField] private GameObject _eventNodePrefab;
    [SerializeField] private GameObject _townNodePrefab;
    [SerializeField] private GameObject _edgePrefab;
    [SerializeField] private GameObject _completePopup;
    [SerializeField] private List<float> _weights;
    private List<Node> _nodes = new List<Node>();
    private List<LineRenderer> _edges = new List<LineRenderer>();
    private Node _curNode;
    private float _widthScale;
    private List<int> _addableLayers;

    public float WidthScale
    {
        get { return _widthScale; }
    }

    void Start()
    {
        SetMapSize();
        // load seed
        if (PlayerData.SeedJSON != null)
        {
            Random.state = JsonUtility.FromJson<Random.State>(PlayerData.SeedJSON);
        }
        else
        {
            PlayerData.SeedJSON = JsonUtility.ToJson(Random.state);
        }

        GenerateMap();

        if (PlayerData.Path != null && PlayerData.Path.Count > 0)
        {

            _curNode = _nodes[PlayerData.Path.Last()];
        }
        else
        {
            PlayerData.Path = new List<int>();
            AddNodeToPath(_nodes[0]);
        }
        ShowPath();
        // re-randomize
        Random.InitState(System.Environment.TickCount);
        if (PlayerData.Path != null && PlayerData.Path.Last() == _nodes.Count - 1)
        {
            _completePopup.SetActive(true);
        }
        SaveSystem.Save();
        CameraMovement.Instance.SetPosition(_curNode.transform.position);
    }


    public void GenerateMap()
    {
        InitAddableLayers();
        for (int i = 0; !AllLayersHaveEnoughNodes(); i++)
        {
            int layer = RandomLayer();
            GameObject node = CreateRandomNode(layer);
            node.GetComponent<Node>().Init(layer);
            node.transform.position = RandomPos(layer);
            node.transform.SetParent(gameObject.transform);
            _nodes.Add(node.GetComponent<Node>());
            SetAddableLayers(layer);
            _numNode = i + 1;
            /*if (_addableLayers.Count == 0)
            {
                break;
            }*/
        }
        _nodes = _nodes.OrderByDescending(x => x.gameObject.transform.position.x).ToList();
        _nodes[0].transform.position = new Vector3(_nodes[0].transform.position.x, 0, transform.position.z);
        _nodes.Last().transform.position = new Vector3(_nodes.Last().transform.position.x, 0, transform.position.z);

        for (int i = 1; i < _numNode; i++)
        {
            Node nextNode = FindNearestNode(_nodes[i], _nodes.GetRange(0, i));
            GameObject edge = Instantiate(_edgePrefab);
            edge.transform.SetParent(gameObject.transform);
            _edges.Add(edge.GetComponent<LineRenderer>());
            ConnectNodes(_nodes[i], nextNode);
            _edges.Last().SetPositions(new Vector3[] { _nodes[i].transform.position, nextNode.transform.position });
        }

        _nodes = _nodes.OrderBy(x => x.gameObject.transform.position.x).ToList();
        for (int i = 1; i < _numNode; i++)
        {
            if (_nodes[i].Prev.Count > 0) continue;
            Node prevNode = FindNearestNode(_nodes[i], _nodes.GetRange(0, i));
            GameObject edge = Instantiate(_edgePrefab);
            edge.transform.SetParent(gameObject.transform);
            _edges.Add(edge.GetComponent<LineRenderer>());
            ConnectNodes(prevNode, _nodes[i]);
            _edges.Last().SetPositions(new Vector3[] { prevNode.transform.position, _nodes[i].transform.position });
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


    public bool AllLayersHaveEnoughNodes()
    {
        for (int i = 0; i < _numLayer; i++)
        {
            int min = _minNodePerLayer;
            if (i == 0 || i == _numLayer - 1) min = 1;
            if (GetNodesInLayer(i).Count < min) return false;
        }
        return true;
    }

    public void InitAddableLayers()
    {
        _addableLayers = new List<int>();
        for (int i = 0; i < _numLayer; i++)
        {
            _addableLayers.Add(i);
        }
    }
    private GameObject CreateRandomNode(int layer)
    {
        float randf = Random.Range(0.0f, 1.0f);
        int rand = 0;
        foreach (float weight in _weights)
        {
            if (randf < weight) break;
            else
            {
                randf -= weight;
                rand++;
            }
        }
        if (layer == 0 || layer == _numLayer - 1) rand = 0;
        if (layer == _numLayer - 2) rand = 2;
        switch (rand)
        {
            case 0:
                return CreateBattleNode();
            case 1:
                return CreateEventNode();
            case 2:
                return CreateTownNode();
            default:
                return null;
        }
    }

    private GameObject CreateBattleNode()
    {
        GameObject battleNode = Instantiate(_battleNodePrefab);
        Wave wave = AllWaves[Random.Range(0, AllWaves.Count)];
        battleNode.GetComponent<BattleNode>().Wave = wave;
        return battleNode;
    }

    private GameObject CreateEventNode()
    {
        GameObject eventNode = Instantiate(_eventNodePrefab);
        DialogNode randomEvent = AllRandomEvents[Random.Range(0, AllRandomEvents.Count)];
        eventNode.GetComponent<EventNode>().RandomEvent = randomEvent;
        return eventNode;
    }

    private GameObject CreateTownNode()
    {
        GameObject townNode = Instantiate(_townNodePrefab);
        return townNode;
    }

    public void SetAddableLayers(int layer)
    {
        int max;
        if (layer == 0 || layer == _numLayer - 1) max = 1;
        else max = _maxNodePerLayer;
        if (GetNodesInLayer(layer).Count >= max) _addableLayers.Remove(layer);
    }
    public int RandomLayer()
    {
        int n = _addableLayers.Count;
        return _addableLayers[Random.Range(0, n)];
    }

    public List<Node> GetNodesInLayer(int layer)
    {
        List<Node> nodesInLayer = new List<Node>();
        foreach (Node node in _nodes)
        {
            if (node.Layer == layer) nodesInLayer.Add(node);
        }
        return nodesInLayer;
    }

    public Vector3 RandomPos(int layer)
    {
        float startX = GetXMin() + GetMapSize().x * (layer + 0.25f) / _numLayer;
        float x = Random.Range(startX, startX + 0.5f * GetMapSize().x / _numLayer);
        float y = Random.Range(-GetMapSize().y / 2, GetMapSize().y / 2 - 0.5f);
        foreach (Node node in _nodes)
        {
            if ((Mathf.Abs(x - node.transform.position.x) < _minGap && Mathf.Abs(y - node.transform.position.y) < _minGap))
            {
                return RandomPos(layer);
            }
        }
        foreach (Node node in GetNodesInLayer(layer))
        {
            if (Mathf.Abs(y - node.transform.position.y) < _minGap)
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
        if (a.Layer == 0 || a.Layer == _numLayer - 1 || b.Layer == 0 || b.Layer == _numLayer - 1) max = 1;
        return Mathf.Abs(a.Layer - b.Layer) <= max && a.Layer != b.Layer && !EdgesOverlap(a, b);
    }

    public bool EdgesOverlap(Node a, Node b)
    {
        foreach (LineRenderer edge in _edges)
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
        return _nodes.Where(x => x.Prev.Count == 0).ToList();
    }

    public void ShowPath()
    {
        int prev = -1;
        foreach (int i in PlayerData.Path)
        {
            if (prev >= 0)
            {
                LineRenderer line = FindEdge(_nodes[prev], _nodes[i]);
                line.material = line.gameObject.GetComponent<Edge>().DefaultMat;
                line.startWidth = 0.05f;
                line.endWidth = 0.05f;
                line.startColor = Color.green;
                line.endColor = Color.green;
            }
            _nodes[i].OnPass();
            prev = i;
        }
        foreach (Node node in _curNode.Next)
        {
            LineRenderer line = FindEdge(_curNode, node);
            line.material = line.gameObject.GetComponent<Edge>().DefaultMat;
            line.startWidth = 0.05f;
            line.endWidth = 0.05f;
            line.startColor = Color.cyan;
            line.endColor = Color.cyan;
            node.OnClickable();
        }
    }

    public void AddNodeToPath(Node node)
    {
        PlayerData.Path.Add(_nodes.IndexOf(node));
        _curNode = node;
    }

    public void ResetMap()
    {
        PlayerData.SeedJSON = null;
        PlayerData.Path = null;
    }

    public LineRenderer FindEdge(Node oriNode, Node desNode)
    {
        return _edges.Find(x => x.GetPosition(0).Equals(oriNode.transform.position) && x.GetPosition(1).Equals(desNode.transform.position));
    }

    public void ResetEdge(LineRenderer line)
    {
        Vector3[] pos = new Vector3[2];
        line.GetPositions(pos);
        _edges.Remove(line);
        GameObject.Destroy(line.gameObject);
        GameObject edge = Instantiate(_edgePrefab);
        edge.transform.SetParent(gameObject.transform);
        _edges.Add(edge.GetComponent<LineRenderer>());
        _edges.Last().SetPositions(pos);
    }

    public Vector2 GetMapSize()
    {
        return GetComponent<RectTransform>().sizeDelta;
    }

    public void SetMapSize()
    {
        Vector2 size = GetMapSize();
        float oriWidth = size.x;
        size.x = _numLayer * _layerWidth;
        _widthScale = size.x / oriWidth;
        GetComponent<RectTransform>().sizeDelta = size;
        Vector3 pos = transform.position;
        pos.x = (_numLayer * _layerWidth - oriWidth) / 2;
        GetComponent<RectTransform>().position = pos;
    }

}
