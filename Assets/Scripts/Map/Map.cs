using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public List<Node> Nodes;
    public List<LineRenderer> Edges;
    [SerializeField] private float minGap;
    [SerializeField] private float maxGap;
    [SerializeField] private int numNode;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject edgePrefab;

    void Start()
    {
        Random.State originalState = Random.state;

        // load seed
        if (PlayerData.seedJSON != null)
        {
            Random.state = JsonUtility.FromJson<Random.State>(PlayerData.seedJSON);
        }

        GenerateMap();
        Random.state = originalState;
    }

    public void GenerateMap()
    {
        for (int i = 0; i < numNode; i++)
        {
            GameObject node = Instantiate(nodePrefab);
            node.transform.position = RandomPos();
            node.transform.parent = gameObject.transform;
            Nodes.Add(node.GetComponent<Node>());
        }
        Nodes = Nodes.OrderByDescending(x => x.gameObject.transform.position.x).ToList();

        for (int i = 1; i < numNode; i++)
        {
            GameObject edge = Instantiate(edgePrefab);
            edge.transform.parent = gameObject.transform;
            Edges.Add(edge.GetComponent<LineRenderer>());
            Node nextNode = FindNearestNode(Nodes[i], Nodes.GetRange(0, i));
            ConnectNodes(Nodes[i], nextNode);
            Edges.Last().SetPositions(new Vector3[] { Nodes[i].transform.position, nextNode.transform.position });
        }

        Nodes = Nodes.OrderBy(x => x.gameObject.transform.position.x).ToList();

        for (int i = 3; i < numNode; i++)
        {
            if (Nodes[i].Prev.Count > 0) continue;
            GameObject edge = Instantiate(edgePrefab);
            edge.transform.parent = gameObject.transform;
            Edges.Add(edge.GetComponent<LineRenderer>());
            Node prevNode = FindNearestNode(Nodes[i], Nodes.GetRange(0, i));
            ConnectNodes(prevNode, Nodes[i]);
            Edges.Last().SetPositions(new Vector3[] { prevNode.transform.position, Nodes[i].transform.position });
        }
    }

    public Vector3 RandomPos()
    {
        float x = Random.Range(-GetComponent<RectTransform>().sizeDelta.x / 2, GetComponent<RectTransform>().sizeDelta.x / 2);
        float y = Random.Range(-GetComponent<RectTransform>().sizeDelta.y / 2, GetComponent<RectTransform>().sizeDelta.y / 2);
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
        return Mathf.Abs(a.transform.position.x - b.transform.position.x) > minGap;
    }

}
