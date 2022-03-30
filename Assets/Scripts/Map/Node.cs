using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public List<Node> Next;
    public List<Node> Prev;
    private SpriteRenderer check;
    private SpriteOutline outline;
    private SpriteRenderer bg;


    public void Init()
    {
        Next = new List<Node>();
        Prev = new List<Node>();
        check = transform.Find("Check").GetComponent<SpriteRenderer>();
        check.enabled = false;
        outline = GetComponent<SpriteOutline>();
        outline.enabled = false;
        //Debug.Log(outline);
        bg = GetComponent<SpriteRenderer>();
        GetComponent<CircleCollider2D>().enabled = false;
    }

    void OnMouseEnter()
    {
        transform.localScale = transform.localScale * 1.2f;
    }

    void OnMouseExit()
    {
        transform.localScale = transform.localScale / 1.2f;
    }

    void OnMouseDown()
    {
        Map.singleton.AddNodeToPath(this);
    }

    public void OnClickable()
    {
        //Debug.Log(outline);
        outline.enabled = true;
        //Debug.Log(outline.enabled);
        GetComponent<CircleCollider2D>().enabled = true;
    }

    public void OnPass()
    {
        outline.enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        check.enabled = true;
        bg.color = Color.gray;
    }

    public void OnReset()
    {
        outline.enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        check.enabled = false;
    }

}
