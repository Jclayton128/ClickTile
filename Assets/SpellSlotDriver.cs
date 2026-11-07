using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SpellSlotDriver : MonoBehaviour
{
    //refs
    [SerializeField] Image[] _manaCostIcons = null;
    [SerializeField] TextMeshProUGUI _spellName = null;

    //settings
    [SerializeField] float _tweenTime = 0.5f;
    [SerializeField] float _yPos_PartialLower = -50f;
    [SerializeField] float _yPos_FullyLower = -200f;

    //state
    Tween _moveTween;

    [SerializeField] SpellSO _loadedSpell;
    public SpellSO LoadedSpell => _loadedSpell;

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

    public void LowerSpellSlot_Partially()
    {
        _moveTween.Kill();
        _moveTween = transform.DOLocalMoveY(_yPos_PartialLower, _tweenTime);
    }

    public void LowerSpellSlot_Fully()
    {
        _moveTween.Kill();
        _moveTween = transform.DOLocalMoveY(_yPos_FullyLower, _tweenTime);
    }

    public void RaiseSpellSlot()
    {
        _moveTween.Kill();
        _moveTween = transform.DOLocalMoveY(0, _tweenTime);
    }



}
