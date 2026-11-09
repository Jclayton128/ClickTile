using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ActorHandler : MonoBehaviour
{
    //ref
    [SerializeField] bool _isPlayer = false;
    [SerializeField] bool _canBeTargetedByPlayerSpell = false;
    [SerializeField] bool _isAnchoredToTile = true;
    [SerializeField] bool _immobilizesTile = false;
    [SerializeField] SpriteRenderer _visualSR = null;


    //state
    Tween _fadeTween;
    TileHandler _newTile;
    float _remainingTweenTime;
    public bool ImmobilizesTile => _immobilizesTile;
    public TileHandler CurrentTile => transform.parent.parent.GetComponent<TileHandler>();
    public bool CanBeTargetedByPlayerSpell => _canBeTargetedByPlayerSpell;  

    private void Start()
    {
        TileController.Instance.TileRemappingComplete += HandleTileRemappingComplete;
    }

    private void HandleTileRemappingComplete()
    {
        if (CurrentTile) return;

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
        if (CurrentTile)
        {
            CurrentTile.TileSliding -= HandleTileSliding;
            //_currentTile.DeregisterOccupant();
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

    public void DestroySelf()
    {
        if (CurrentTile)
        {
            CurrentTile.TileSliding -= HandleTileSliding;
            //_currentTile.DeregisterOccupant();
        }
        TileController.Instance.TileRemappingComplete -= HandleTileRemappingComplete;
        transform.parent = null;
        ActorController.Instance.RemoveActorFromTurnOrder(this);

        Destroy(gameObject);
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

    #region AI
    public void BeginTurn()
    {
        if (_isPlayer) return;
        Debug.Log("Monster does his movement...");

        List<TileHandler> orthoTiles = TileController.Instance.GetOrthogonalTiles(CurrentTile);

        _fadeTween.Kill();
        _visualSR.color = new Color(_visualSR.color.r, _visualSR.color.g, _visualSR.color.b, 1);
        _fadeTween = _visualSR.DOFade(0, 0.5f);
        _fadeTween = _visualSR.DOFade(1, 0.5f).SetDelay(0.5f);

        Invoke(nameof(CompleteTurn), 1f);
    }

    private void CompleteTurn()
    {
        ActorController.Instance.HandlePriorityActorTurnCompletion();
    }

    #endregion
}
