using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ActorHandler : MonoBehaviour
{
    //ref
    [SerializeField] bool _isAnchoredToTile = true;
    [SerializeField] bool _immobilizesTile = false;
    [SerializeField] SpriteRenderer _visualSR = null;

    //state
    Tween _fadeTween;
    TileHandler _newTile;
    float _remainingTweenTime;
    public bool ImmobilizesTile => _immobilizesTile;
    TileHandler _currentTile;

    private void Start()
    {
        TileController.Instance.TileRemappingComplete += HandleTileRemappingComplete;
    }

    private void HandleTileRemappingComplete()
    {
        if (_currentTile) return;

        _newTile = TileController.Instance.GetTileAtWorldPosition(transform.position);
        
        if (!_isAnchoredToTile)
        {
            RegisterAtNewTile();
        }

    }

    public void HandleInitialTilePlacement(TileHandler initialTile)
    {
        RegisterAtInitialTile(initialTile);
    }

    private void RegisterAtNewTile()
    {
        if (_currentTile)
        {
            _currentTile.TileSliding -= HandleTileSliding;
            //_currentTile.DeregisterOccupant();
            _currentTile = null;
        }

        //newTile.RegisterOccupant(this);


        if (_newTile)
        {
            _newTile.TileSliding += HandleTileSliding;
            transform.parent = _newTile.VisualsTransform;
            transform.localPosition = Vector3.zero;

            _newTile = null;
        }
        else
        {
            transform.parent = null;
        }

    }

    private void RegisterAtInitialTile(TileHandler newTile)
    {       
        //newTile.RegisterOccupant(this);
        newTile.TileSliding += HandleTileSliding;
        transform.parent = newTile.VisualsTransform;
        transform.localPosition = Vector3.zero;
    }

    public void FadeInOut(float fadeInOutTime)
    {
        _fadeTween.Kill();
        _visualSR.color = new Color(_visualSR.color.r, _visualSR.color.g, _visualSR.color.b, 1);
        _fadeTween = _visualSR.DOFade(0, fadeInOutTime/2f);

    }

    public void TeleportToNewTile(TileHandler newTile, float totalTweenTime)
    {
        _fadeTween.Kill();
        _fadeTween = _visualSR.DOFade(0, totalTweenTime / 2f).SetEase(Ease.InCubic);
        _newTile = newTile;
        _remainingTweenTime = totalTweenTime/2f;


        Invoke(nameof(Delay_TeleportToNewTile), totalTweenTime / 2f);

    }

    private void Delay_TeleportToNewTile()
    {
        RegisterAtNewTile();

        _fadeTween = _visualSR.DOFade(1, _remainingTweenTime).SetEase(Ease.OutCubic);
    }


    private void HandleTileSliding()
    {
        if (_isAnchoredToTile)
        {
            //No need to remap to a new tile.
        }
        else
        {
            RegisterAtNewTile();
        }
    }
}
