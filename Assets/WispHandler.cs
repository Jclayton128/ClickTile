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
        transform.DOBlendableLocalMoveBy(Vector3.up * 0.3f, time).OnComplete(CompleteWisp);
        _tmp.DOFade(0, time).SetEase(Ease.OutCirc);
    }

    private void CompleteWisp()
    {
        Destroy(gameObject, 0.01f);
    }
}
