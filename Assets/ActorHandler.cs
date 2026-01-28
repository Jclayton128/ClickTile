using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ActorHandler : MonoBehaviour
{
    //ref

    [SerializeField] SpriteRenderer _visualSR = null;

    //state
    Tween _fadeTween;
    TileHandler _newTile;
    float _remainingTweenTime;

    public void FadeInOut(float fadeInOutTime)
    {
        _fadeTween.Kill();
        _visualSR.color = new Color(_visualSR.color.r, _visualSR.color.g, _visualSR.color.b, 1);
        _fadeTween = _visualSR.DOFade(0, fadeInOutTime/2f);

    }

    public void TeleportToNewTile(TileHandler newTile, float totalTweenTime)
    {
        _fadeTween.Kill();
        _fadeTween = _visualSR.DOFade(0, totalTweenTime / 2f);
        _newTile = newTile;
        _remainingTweenTime = totalTweenTime/2f;

        Invoke(nameof(Delay_TeleportToNewTile), totalTweenTime / 2f);

    }

    private void Delay_TeleportToNewTile()
    {
        transform.parent = _newTile.VisualsTransform;
        transform.localPosition = Vector3.zero;

        _fadeTween = _visualSR.DOFade(1, _remainingTweenTime);
    }
}
