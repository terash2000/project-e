using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour
{
    public Color Color = Color.white;

    [Range(0, 16)]
    public int OutlineSize = 1;

    private SpriteRenderer _spriteRenderer;

    void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateOutline(true);
    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void Update()
    {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        _spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", Color);
        mpb.SetFloat("_OutlineSize", OutlineSize);
        _spriteRenderer.SetPropertyBlock(mpb);
    }
}
