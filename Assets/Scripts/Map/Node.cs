using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> Next;
    public List<Node> Prev;
    public int Layer;
    private SpriteRenderer _check;
    private SpriteOutline _outline;
    private SpriteRenderer _bg;

    public void Init(int layer)
    {
        Next = new List<Node>();
        Prev = new List<Node>();
        _check = transform.Find("Check").GetComponent<SpriteRenderer>();
        _check.enabled = false;
        _outline = GetComponent<SpriteOutline>();
        _outline.enabled = false;
        _bg = GetComponent<SpriteRenderer>();
        GetComponent<CircleCollider2D>().enabled = false;
        Layer = layer;
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
        Map.Instance.AddNodeToPath(this);
        ChangeScene();
    }

    public void OnClickable()
    {
        _outline.enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    public void OnPass()
    {
        _outline.enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        _check.enabled = true;
        _bg.color = Color.gray;
    }

    public void OnReset()
    {
        _outline.enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        _check.enabled = false;
    }

    protected virtual void ChangeScene() { }
}
