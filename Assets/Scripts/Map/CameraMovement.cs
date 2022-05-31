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
    private Vector2 _velocity;

    void Update()
    {
        if (!Input.GetMouseButton(0) && _velocity != Vector2.zero) Spring();
        else if (Time.timeScale > 0f)
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
            _velocity = new Vector2(DragSpeed, DragSpeed);
            return;
        }
        if (!Input.GetMouseButton(0)) return;
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
        Vector3 move = new Vector3(-pos.x * DragSpeed * Camera.main.orthographicSize / MaxZoom, -pos.y * DragSpeed * Camera.main.orthographicSize / MaxZoom, 0);
        _dragOrigin = Input.mousePosition;

        Rect currentRect = GetArea();

        if (currentRect.xMin + move.x < _rectOrigin.xMin) move.x = RubberDelta(move.x, currentRect.width);
        if (currentRect.yMin + move.y < _rectOrigin.yMin) move.y = RubberDelta(move.y, currentRect.height);
        if (currentRect.xMax + move.x > _rectOrigin.xMax) move.x = RubberDelta(move.x, currentRect.width);
        if (currentRect.yMax + move.y > _rectOrigin.yMax) move.y = RubberDelta(move.y, currentRect.height);

        transform.Translate(move, Space.World);
    }

    private float RubberDelta(float overStretching, float viewSize)
    {
        return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
    }

    public Rect GetArea(float widthExtend = 0)
    {
        Vector3 posTopLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 posBottomRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.nearClipPlane));
        return new Rect(posTopLeft.x, posTopLeft.y, posBottomRight.x - posTopLeft.x + widthExtend, posBottomRight.y - posTopLeft.y);
    }

    public List<bool> InsideArea(Rect rect)
    {
        return new List<bool>() { _rectOrigin.xMin <= rect.xMin, _rectOrigin.xMax >= rect.xMax, _rectOrigin.yMin <= rect.yMin, _rectOrigin.yMax >= rect.yMax };
    }

    public List<float> GetOffset(Rect rect)
    {
        return new List<float>() { _rectOrigin.xMin - rect.xMin, _rectOrigin.xMax - rect.xMax, _rectOrigin.yMin - rect.yMin, _rectOrigin.yMax - rect.yMax };
    }

    public void BoundCameraPos()
    {
        List<bool> inside = InsideArea(GetArea());
        Vector3 pos = transform.position;
        float offsetX = 0;
        float offsetY = 0;
        for (int i = 0; i < 4; i++)
        {
            if (!inside[i] && i < 2) offsetX = GetOffset(GetArea())[i];
            else if (!inside[i]) offsetY = GetOffset(GetArea())[i];
        }
        if (!inside[0] || !inside[1]) pos.x = pos.x + offsetX;
        if (!inside[2] || !inside[3]) pos.y = pos.y + offsetY;
        transform.position = pos;
    }

    public void Spring()
    {
        Vector2 speed = _velocity;
        float smoothTime = 0.1f;
        float deltaTime = Time.unscaledDeltaTime;
        List<bool> inside = InsideArea(GetArea());
        Vector3 pos = transform.position;
        float offsetX = 0;
        float offsetY = 0;
        for (int i = 0; i < 4; i++)
        {
            if (!inside[i] && i < 2) offsetX = GetOffset(GetArea())[i];
            else if (!inside[i]) offsetY = GetOffset(GetArea())[i];
        }
        if (offsetX < 0 && speed.x > 0) speed.x = -speed.x;
        if (offsetY < 0 && speed.y > 0) speed.y = -speed.y;
        if (!inside[0] || !inside[1]) pos.x = Mathf.SmoothDamp(pos.x, pos.x + offsetX, ref speed.x, smoothTime, Mathf.Infinity, deltaTime);
        else speed.x = 0;
        if (!inside[2] || !inside[3]) pos.y = Mathf.SmoothDamp(pos.y, pos.y + offsetY, ref speed.y, smoothTime, Mathf.Infinity, deltaTime);
        else speed.y = 0;
        transform.position = pos;
        if (Mathf.Abs(speed.x) < 1)
            speed.x = 0;
        if (Mathf.Abs(speed.y) < 1)
            speed.y = 0;
        _velocity = speed;
        if (_velocity == Vector2.zero) BoundCameraPos();
    }

    public void SetPosition(Vector3 pos)
    {
        _rectOrigin = GetArea(Map.Instance.WidthExtend);
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
