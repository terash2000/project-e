using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BlockPopup : MonoBehaviour
{
    private const float DISAPPEAR_SPEED = 1f;
    private const float DISAPPEAR_TIMER_MAX = 0.8f;
    private const float INITIAL_TEXT_VELOCITY = 3f;

    private TextMeshProUGUI _textMesh;
    private float _disappearTimer;
    private float _disappearSpeed;
    private Color _textColor;
    private Vector3 _moveVector;

    public void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _disappearTimer = DISAPPEAR_TIMER_MAX;
        _disappearSpeed = DISAPPEAR_SPEED;
        _textColor = _textMesh.faceColor;
        _textColor.a = 0.6f;
        _textMesh.faceColor = _textColor;
        _textMesh.fontSize = 0.3f;
        _moveVector = new Vector3(Random.Range(-0.7f, -1.0f), 0.1f) * INITIAL_TEXT_VELOCITY;
    }

    public void Update()
    {
        transform.position += Time.deltaTime * _moveVector;
        _moveVector -= 4f * Time.deltaTime * _moveVector;
        _moveVector.y -= 2f * Time.deltaTime;


        if (_disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            // First half of the popup lifetime
            float increaseScaleAmount = 1.3f;
            transform.localScale += increaseScaleAmount * Time.deltaTime * Vector3.one;
        }
        else
        {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 1.3f;
            transform.localScale -= decreaseScaleAmount * Time.deltaTime * Vector3.one;
        }

        _disappearTimer -= Time.deltaTime;
        if (_disappearTimer < 0)
        {
            _textColor.a -= _disappearSpeed * Time.deltaTime;
            // test -= (byte)(_disappearSpeed * Time.deltaTime);
            _textMesh.faceColor = _textColor;
            if (_textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
