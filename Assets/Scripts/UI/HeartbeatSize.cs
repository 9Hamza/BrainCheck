using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HeartbeatSize : MonoBehaviour
{

    private Vector3 originalScale;
    private float sizeFactor = 1.2f;
    private float time = 1f;

    private void Awake()
    {
        // cache original scale valu
        originalScale = transform.localScale;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(HeartbeatAnimation());
    }

    public IEnumerator HeartbeatAnimation()
    {
        while (true)
        {
            transform.DOScale(originalScale * sizeFactor, time);
            yield return new WaitForSeconds(time);
            transform.DOScale(originalScale, time);
            yield return new WaitForSeconds(time);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
