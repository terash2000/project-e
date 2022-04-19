using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviourSingleton<CameraMovement>
{
    public float ZoomSpeed;
    public float MinZoom;
    public float MaxZoom;
    public float DragSpeed;
    private Vector3 _dragOrigin;
    private Rect _rectOrigin;

    void Update()
    {
        if (Time.timeScale > 0f)
        {
            Zoom();
            Drag();
        }
    }
    void Zoom()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit point;
        Physics.Raycast(ray, out point, 1000);
        Vector3 Scrolldirection = ray.GetPoint(5);
        Scrolldirection.z = transform.position.z;
        float step = ZoomSpeed * Time.deltaTime;
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > MinZoom)
        {
            Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * step / 2;
            Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, MinZoom);
            transform.position = Vector3.MoveTowards(transform.position, Scrolldirection, Input.GetAxis("Mouse ScrollWheel") * step);
            BoundCameraPos();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < MaxZoom)
        {
            Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * step / 2;
            Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, MaxZoom);
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
            _dragOrigin = Input.mousePosition;
            return;
        }
        if (!Input.GetMouseButton(0)) return;
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
        Vector3 move = new Vector3(-pos.x * DragSpeed * Camera.main.orthographicSize / MaxZoom, -pos.y * DragSpeed * Camera.main.orthographicSize / MaxZoom, 0);
        _dragOrigin = Input.mousePosition;

        Rect currentRect = GetArea();

        if (currentRect.xMin + move.x < _rectOrigin.xMin) move.x = _rectOrigin.xMin - currentRect.xMin;
        if (currentRect.yMin + move.y < _rectOrigin.yMin) move.y = _rectOrigin.yMin - currentRect.yMin;
        if (currentRect.xMax + move.x > _rectOrigin.xMax) move.x = _rectOrigin.xMax - currentRect.xMax;
        if (currentRect.yMax + move.y > _rectOrigin.yMax) move.y = _rectOrigin.yMax - currentRect.yMax;

        transform.Translate(move, Space.World);
    }

    public Rect GetArea(float widthScale = 1)
    {
        Vector3 posTopLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 posBottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));
        return new Rect(posTopLeft.x, posTopLeft.y, (posBottomRight.x - posTopLeft.x) * widthScale, posBottomRight.y - posTopLeft.y);
    }

    public List<bool> InsideArea(Rect rect)
    {
        return new List<bool>() { _rectOrigin.xMin <= rect.xMin, _rectOrigin.xMax >= rect.xMax, _rectOrigin.yMin <= rect.yMin, _rectOrigin.yMax >= rect.yMax };
    }

    public void BoundCameraPos()
    {
        if (_rectOrigin.width - GetArea().width < 0.002f || _rectOrigin.height - GetArea().height < 0.002f) return;
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

    public void SetPosition(Vector3 pos)
    {
        _rectOrigin = GetArea(Map.Instance.WidthScale);
        float x = pos.x;
        if (_rectOrigin.xMin > x - GetArea().width / 2)
        {
            x = _rectOrigin.xMin + GetArea().width / 2;
        }
        else if (_rectOrigin.xMax < x + GetArea().width / 2)
        {
            x = _rectOrigin.xMax - GetArea().width / 2;
        }
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
