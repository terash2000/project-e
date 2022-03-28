using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public List<Node> Next;
    public List<Node> Prev;

    void Start()
    {
        Next = new List<Node>();
        Prev = new List<Node>();
    }

}
