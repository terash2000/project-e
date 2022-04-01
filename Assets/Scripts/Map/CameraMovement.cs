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
        rectOrigin = GetArea(Map.Instance.WidthScale);
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
            transform.position = Vector3.MoveTowards(transform.position, Scrolldirection, Input.GetAxis("Mouse ScrollWheel") * step);
            BoundCameraPos();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < maxZoom)
        {
            Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * step / 2;
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, maxZoom);
            transform.position = Vector3.MoveTowards(transform.position, Scrolldirection, Input.GetAxis("Mouse ScrollWheel") * step);
            BoundCameraPos();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, 0, transform.position.z), -Input.GetAxis("Mouse ScrollWheel") * step);
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
        Vector3 move = new Vector3(-pos.x * dragSpeed * Camera.main.orthographicSize / maxZoom, -pos.y * dragSpeed * Camera.main.orthographicSize / maxZoom, 0);
        dragOrigin = Input.mousePosition;
        transform.Translate(move, Space.World);
        if (InsideArea(GetArea()).Contains(false)) transform.Translate(-move, Space.World);
    }

    public Rect GetArea(float widthScale = 1)
    {
        Vector3 posTopLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 posBottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));
        return new Rect(posTopLeft.x, posTopLeft.y, (posBottomRight.x - posTopLeft.x) * widthScale, posBottomRight.y - posTopLeft.y);
    }

    public List<bool> InsideArea(Rect rect)
    {
        return new List<bool>() { rectOrigin.xMin <= rect.xMin, rectOrigin.xMax >= rect.xMax, rectOrigin.yMin <= rect.yMin, rectOrigin.yMax >= rect.yMax };
    }

    public void BoundCameraPos()
    {
        if (rectOrigin.width - GetArea().width < 0.002f || rectOrigin.height - GetArea().height < 0.002f) return;
        while (InsideArea(GetArea()).Contains(false))
        {
            List<bool> inside = InsideArea(GetArea());
            if (!inside[0] && !inside[1]) return;
            if (!inside[2] && !inside[3]) return;
            Vector3 pos = transform.position;
            if (!inside[0]) pos.x += 0.001f;
            if (!inside[1]) pos.x -= 0.001f;
            if (!inside[2]) pos.y += 0.001f;
            if (!inside[3]) pos.y -= 0.001f;
            transform.position = pos;
        }
    }
}
