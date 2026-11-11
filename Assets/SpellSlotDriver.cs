using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class SpellSlotDriver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    //refs
    [SerializeField] Image[] _manaCostIcons = null;
    [SerializeField] TextMeshProUGUI _spellName = null;

    //settings
    [SerializeField] float _tweenTime = 0.5f;
    [SerializeField] float _yPos_PartialLower = -50f;
    [SerializeField] float _yPos_FullyLower = -200f;
    [SerializeField] float _yPos_Highlighted = 20f;

    //state
    Tween _moveTween;
    [SerializeField] SpellSO _loadedSpell;
    public SpellSO LoadedSpell => _loadedSpell;
    private float _previouslyCommandedPosition = 0;
    public bool IsCastable = false;


    public void ClearSpellSlot()
    {
        foreach (var icon in _manaCostIcons)
        {
            icon.enabled = false;
        }
        _spellName.text = string.Empty;
    }

    public void LoadSpellIntoSlot(SpellSO loadedSpell)
    {
        ClearSpellSlot();

        _loadedSpell = loadedSpell;
        _spellName.text = loadedSpell.SpellName;
        List<TileHandler.TileTypes> cost = loadedSpell.ManaCost;

        for (int i = 0; i < cost.Count; i++)
        {
            _manaCostIcons[i].enabled = true;
            _manaCostIcons[i].sprite = TileController.Instance.GetManaSprite(cost[i]);
        }

    }

    public void PositionSlot_Unfilled()
    {
        _moveTween.Kill();
        _moveTween = transform.DOLocalMoveY(_yPos_FullyLower, _tweenTime);
        _previouslyCommandedPosition = _yPos_FullyLower;
    }

    public void PositionSlot_FilledButUncastable()
    {
        _moveTween.Kill();
        _moveTween = transform.DOLocalMoveY(_yPos_PartialLower, _tweenTime);
        _previouslyCommandedPosition = _yPos_PartialLower;
    }

    public void PositionSlot_FilledAndCastable()
    {
        _moveTween.Kill();
        _moveTween = transform.DOLocalMoveY(0, _tweenTime);
        _previouslyCommandedPosition = 0;
    }

    public void PositionSlot_Highlighted()
    {
        _moveTween.Kill();
        _moveTween = transform.DOLocalMoveY(_previouslyCommandedPosition + _yPos_Highlighted, _tweenTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PositionSlot_Highlighted();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _moveTween.Kill();
        _moveTween = transform.DOLocalMoveY(_previouslyCommandedPosition, _tweenTime);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsCastable)
        {
            //_loadedSpell.Cast();
            MagicController.Instance.DetermineCastableSpells();
        }
    }
}
