using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WispHandler : MonoBehaviour
{

    [SerializeField] TextMeshPro _tmp = null;

    public void SetWisp(string wispText, float time)
    {
        _tmp.text = wispText;
        transform.DOBlendableLocalMoveBy(Vector3.up * 0.1f, time).OnComplete(CompleteWisp);
        transform.DOScale(Vector3.one * 1.3f, time);
        _tmp.DOFade(0, time).SetEase(Ease.OutCirc);
    }

    private void CompleteWisp()
    {
        Destroy(gameObject, 0.01f);
    }
}
