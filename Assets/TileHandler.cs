using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class TileHandler : MonoBehaviour
{
    public enum TileTypes { Undefined, DirtFire, GrassEarth, RockDeath, SnowAir, SandAir}

    public Action TileSliding;

    //refs
    [SerializeField] List<SpriteRenderer> _srs = new List<SpriteRenderer>();
    [SerializeField] SpriteRenderer _tileSR = null;
    [SerializeField] SpriteRenderer _iconSR = null;
    [SerializeField] Transform _visualsTransform = null;
    [SerializeField] TextMeshPro _pointTMP = null;

    public Transform VisualsTransform => _visualsTransform;

    //settings
    [SerializeField] float hoverTweenTime = 0.125f;
    [SerializeField] float _hoverYIncrease = 0.1f;

    //state
    Tween _hoverTween;
    Tween _slideTween;
    Tween _colorTween;
    Collider2D _coll;
    public TileTypeData TileData { get; private set; }
    //public ActorHandler _occupyingActor;
    public ActorHandler OccupyingActor => CheckForOccupyingActor();

    [SerializeField] int _points = 0;
    public int Points => _points;

    private ActorHandler CheckForOccupyingActor()
    {
        ActorHandler ah = GetComponentInChildren<ActorHandler>();
        if (ah)
        {
            return ah;
        }
        else
        {
            return null;
        }

    }

    private void Awake()
    {
        _coll = GetComponent<Collider2D>();
    }

    public void SetTile(TileTypeData tileType)
    {
        TileData = tileType;

        //take in a TileDate struct
        //rerender with new TileData

        _tileSR.sprite = tileType.TileSprite;
        _iconSR.sprite = tileType.TileIcon;

        SetPoints(1);

    }

    #region Mouse Handlers

    private void OnMouseEnter()
    {
        //_sizeTween.Kill();
        //_sizeTween = transform.DOScale(Vector2.one * _scaleIncrease, hoverTweenTime).SetEase(Ease.OutBack);

        if (_slideTween.IsActive()) return;

        TileController.Instance.UnraiseAllTiles();
        //TileController.Instance.HalfraiseAllOrthogonalTiles(this);
        FullraiseTile();
    }

    private void OnMouseExit()
    {
        //_sizeTween.Kill();
        //_sizeTween = transform.DOScale(Vector2.one, hoverTweenTime).SetEase(Ease.OutBack);

        //if (_slideTween.IsActive()) return;

        UnraiseTile();

        //DimTile();

    }

    private void OnMouseUpAsButton()
    {
        HandleClick();
    }

    private void HandleClick()
    {
        //
        TileController.Instance.HandleTileClick(this);
    }

    #endregion

    #region Tile Mechanics

    public void RemoveTile()
    {
        TileController.Instance.DeregisterTile(this);
        //Debug.Log("destroying tile");
        Destroy(gameObject);    
    }

    public void AbsorbTile(Vector2 direction)
    {
        TileController.Instance.DeregisterTile(this);
        _coll.enabled = false;

        //SlideTile(direction);

        _tileSR.DOFade(0, TileController.Instance.ClickTweenTime).SetEase(Ease.InCubic);
        _iconSR.DOFade(0, TileController.Instance.ClickTweenTime).SetEase(Ease.InCubic);
        _visualsTransform.DOLocalMoveY(0.2f, TileController.Instance.ClickTweenTime).OnComplete(RemoveTile);


        //_sizeTween.Kill();
        //_sizeTween = transform.DOScale(Vector2.zero, TileController.Instance.ClickTweenTime)

        //foreach (var sr in _srs)
        //{
        //    sr.sortingOrder -= 10;
        //}

    }

    public void SlideTile(Vector2 slideVector)
    {
        OnMouseExit();

        TileSliding?.Invoke();

        _slideTween.Kill();
        _slideTween = transform.DOBlendableMoveBy(new Vector3(slideVector.x, slideVector.y, 0), TileController.Instance.ClickTweenTime).SetEase(Ease.OutBounce);
    }

  

    public void DimTile()
    {
        _colorTween.Kill();
        _colorTween = _tileSR.DOColor(new Color(0.8f, .8f, .8f, 1f), hoverTweenTime);
    }

    public void HalfdimTile()
    {
        _colorTween.Kill();
        _colorTween = _tileSR.DOColor(new Color (0.9f, .9f, .9f, 1f), hoverTweenTime);
    }

    public void BrightenTile()
    {
        _colorTween.Kill();
        _tileSR.DOColor(Color.white, hoverTweenTime);
    }

    public void FullraiseTile()
    {
        _hoverTween.Kill();
        _hoverTween = _visualsTransform.transform.DOLocalMoveY(_hoverYIncrease, hoverTweenTime).SetEase(Ease.OutBack);
    }

    public void HalfraiseTile()
    {
        _hoverTween.Kill();
        _hoverTween = _visualsTransform.transform.DOLocalMoveY(_hoverYIncrease / 2f, hoverTweenTime).SetEase(Ease.OutBack);
    }

    public void UnraiseTile()
    {
        _hoverTween.Kill();
        _hoverTween = _visualsTransform.DOLocalMoveY(0, hoverTweenTime).SetEase(Ease.OutBack);
    }

    #endregion


    #region Point Mechanics

    public void AddPoints(int pointsToAdd)
    {
        _points += pointsToAdd;
        _pointTMP.text = _points.ToString();
    }

    public void SetPoints(int pointsToSetAt)
    {
        _points = pointsToSetAt;
        _pointTMP.text = _points.ToString();   
    }

    #endregion

    #region Occupant Handlers

    public void RegisterOccupant(ActorHandler newOccupant)
    {
        //if (_occupyingActor != null)
        //{
        //    Debug.Log($"This tile is already occupied by {_occupyingActor}");
        //    return;
        //}
        
        //_occupyingActor = newOccupant;
    }

    public void DeregisterOccupant()
    {
        //_occupyingActor = null;
    }

    #endregion

}
