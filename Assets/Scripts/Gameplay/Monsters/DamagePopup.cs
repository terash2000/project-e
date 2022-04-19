using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private const float DISAPPEAR_SPEED = 1f;
    private const float DISAPPEAR_TIMER_MAX = 0.8f;
    private const float INITIAL_TEXT_VELOCITY = 4f;

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
        _moveVector = new Vector3(Random.Range(0.2f, 0.5f), 1f) * INITIAL_TEXT_VELOCITY;
    }

    public void Update()
    {
        transform.position += Time.deltaTime * _moveVector;
        _moveVector -= 4f * Time.deltaTime * _moveVector;


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
            if (transform.localScale.x < 0 || transform.localScale.y < 0)
            {
                Destroy(gameObject);
                return;
            }
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
