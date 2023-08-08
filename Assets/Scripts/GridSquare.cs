using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class GridSquare : MonoBehaviour
{
    public int order;
    public bool timeToClick;
    // private Color32 flashColor = new Color32(17, 72, 255, 255);
    
    [SerializeField] private Color32 flashToColor = new Color32(226, 183, 17, 255);
    [SerializeField] private Color32 origColor = new Color32(255, 255, 255, 128);

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // shows click pattern before player can click
    private IEnumerator ShowClick()
    {
        _spriteRenderer.DOColor(flashToColor, 0.2f);
        yield return new WaitForSeconds(0.3f);
        // _spriteRenderer.DOColor(Color.white, 0.2f);
        _spriteRenderer.DOColor(origColor, 0.2f);
    }

    public void FakeClick()
    {
        StartCoroutine(ShowClick());
    }

    public void LightUp()
    {
        _spriteRenderer.DOColor(flashToColor, 0.1f);
    }

    public void LightDown()
    {
        // _spriteRenderer.DOColor(Color.white, 0.1f);
        _spriteRenderer.DOColor(origColor, 0.1f);
    }
}
