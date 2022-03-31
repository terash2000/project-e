using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;
    public float dragSpeed;
    private Vector3 dragOrigin;
    private Rect rectOrigin;
    void Start()
    {
        rectOrigin = GetArea();
    }

    void Update()
    {
        Zoom();
        Drag();
    }
    void Zoom()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit point;
        Physics.Raycast(ray, out point, 1000);
        Vector3 Scrolldirection = ray.GetPoint(5);
        Scrolldirection.z = transform.position.z;
        float step = zoomSpeed * Time.deltaTime;
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > minZoom)
        {
            Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * step / 2;
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, minZoom);
            Vector3 oldPos = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, Scrolldirection, Input.GetAxis("Mouse ScrollWheel") * step);
            if (InsideArea(GetArea()).Contains(false))
            {
                if (InsideArea(GetArea())[0]) Scrolldirection.y = oldPos.y;
                else if (InsideArea(GetArea())[1]) Scrolldirection.x = oldPos.x;
                else Scrolldirection = oldPos;
                transform.position = Vector3.MoveTowards(transform.position, Scrolldirection, Input.GetAxis("Mouse ScrollWheel") * step);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < maxZoom)
        {
            Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * step / 2;
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, maxZoom);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 0, transform.position.z), -Input.GetAxis("Mouse ScrollWheel") * step);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 0, transform.position.z), -Input.GetAxis("Mouse ScrollWheel") * step);
        }
    }

    void Drag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }
        if (!Input.GetMouseButton(0)) return;
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);
        dragOrigin = Input.mousePosition;
        transform.Translate(move, Space.World);
        if (InsideArea(GetArea()).Contains(false)) transform.Translate(-move, Space.World);
    }

    public Rect GetArea()
    {
        Vector3 posTopLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 posBottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));
        return new Rect(posTopLeft.x, posTopLeft.y, posBottomRight.x - posTopLeft.x, posBottomRight.y - posTopLeft.y);
    }

    public List<bool> InsideArea(Rect rect)
    {
        Debug.Log(rect);
        return new List<bool>() { rectOrigin.xMin < rect.xMin && rectOrigin.xMax > rect.xMax, rectOrigin.yMin < rect.yMin && rectOrigin.yMax > rect.yMax };
    }
}
