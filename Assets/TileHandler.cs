using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileHandler : MonoBehaviour
{
    public enum TileTypes { Undefined, Grass, Mouse, Wolf}

    //refs

    [SerializeField] List<SpriteRenderer> _srs = new List<SpriteRenderer>();
    [SerializeField] SpriteRenderer _iconSR = null;
    [SerializeField] SpriteRenderer _borderSR = null;

    //settings

    [SerializeField] float hoverTweenTime = 0.125f;
    [SerializeField] float _scaleIncrease = 1.125f;

    //state
    Tween _sizeTween;
    Tween _slideTween;
    Collider2D _coll;
    public TileTypeData TyleType { get; private set; }

    private void Awake()
    {
        _coll = GetComponent<Collider2D>();
    }

    private void OnMouseEnter()
    {
        _sizeTween.Kill();
        _sizeTween = transform.DOScale(Vector2.one * _scaleIncrease, hoverTweenTime).SetEase(Ease.OutBack);
    }

    private void OnMouseExit()
    {
        _sizeTween.Kill();
        _sizeTween = transform.DOScale(Vector2.one, hoverTweenTime).SetEase(Ease.OutBack);
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

    public void RemoveTile()
    {
        TileController.Instance.DeregisterTile(this);
        //Debug.Log("destroying tile");
        Destroy(gameObject);    
    }

    public void AbsorbTile(Vector2Int direction)
    {
        TileController.Instance.DeregisterTile(this);
        _coll.enabled = false;

        SlideTile(direction);

        _sizeTween.Kill();
        _sizeTween = transform.DOScale(Vector2.zero, TileController.Instance.ClickTweenTime).OnComplete(RemoveTile);

        foreach (var sr in _srs)
        {
            sr.sortingOrder -= 10;
        }

    }

    public void SlideTile(Vector2Int direction)
    {
        _slideTween.Kill();
        _slideTween = transform.DOBlendableMoveBy(new Vector3(direction.x, direction.y, 0), TileController.Instance.ClickTweenTime);
    }

    public void SetTile(TileTypeData tileType)
    {
        TyleType = tileType;
        //take in a TileDate struct
        //rerender with new TileData
        _iconSR.sprite = tileType.TileIcon;
        _borderSR.color = tileType.TileColor;
    }
}
