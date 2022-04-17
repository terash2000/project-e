using UnityEngine;

public class BackgroundShake : MonoBehaviourSingleton<BackgroundShake>
{
    private const float SHAKE_AMOUNT = 2f;

    private Transform _transform;
    private float _shakeDuration = 0f;
    private Vector3 _originalPos;

    void Awake()
    {
        if (_transform == null)
        {
            _transform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        _originalPos = _transform.localPosition;
    }

    void Update()
    {
        if (_shakeDuration > 0)
        {
            _transform.localPosition = _originalPos + Random.insideUnitSphere * SHAKE_AMOUNT;
            _shakeDuration -= Time.deltaTime;
        }
        else
        {
            _transform.localPosition = _originalPos;
        }
    }

    public void Shake(float shakeDuration = 0.5f)
    {
        _shakeDuration = shakeDuration;
    }
}
