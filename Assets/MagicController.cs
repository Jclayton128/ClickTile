using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MagicController : MonoBehaviour
{
    public static MagicController Instance { get; private set; }

    //refs
    [SerializeField] List<Image> _manaSlots = null;
    [SerializeField] List<SpellSlotDriver> _spellSlots = null;

    //settings
    [SerializeField] Sprite _nullManaIcon = null;
    [SerializeField] List<SpellSO> _startingSpells = null;

    //state
    [SerializeField] List<TileHandler.TileTypes> _manaSupply = new List<TileHandler.TileTypes>(4)
        {TileHandler.TileTypes.Undefined,
        TileHandler.TileTypes.Undefined,
        TileHandler.TileTypes.Undefined,
        TileHandler.TileTypes.Undefined};

    private void Awake()
    {
        Instance = this;
    }

    public void LoadStartingSpells()
    {
        foreach (var spellSlot in _spellSlots)
        {
            spellSlot.ClearSpellSlot();
        }


        for (int i = 0; i < _startingSpells.Count; i++)
        {
            _spellSlots[i].LoadSpellIntoSlot(_startingSpells[i]);
        }
    }

    public void GainMana(TileHandler.TileTypes manaType)
    {
        for (int i = 0; i < _manaSupply.Count; i++)
        {
            if (_manaSupply[i] == TileHandler.TileTypes.Undefined)
            {
                _manaSupply[i] = manaType;
                break;
            }
        }

        RenderManaCollected();
    }

    public void SpendMana(TileHandler.TileTypes manaType)
    {
        for (int i = 0; i < _manaSupply.Count; i++)
        {
            if (_manaSupply[i] == manaType)
            {
                _manaSupply[i] = TileHandler.TileTypes.Undefined;
            }
        }

        RenderManaCollected();
    }

    public void ClearAllMana()
    {
        for (int i = 0; i < _manaSupply.Count; i++)
        {
            _manaSupply[i] = TileHandler.TileTypes.Undefined;
        }
        RenderManaCollected();
    }

    private void RenderManaCollected()
    {
        for (int i = 0; i < _manaSupply.Count; i++)
        {
            if (_manaSupply[i] == TileHandler.TileTypes.Undefined)
            {
                _manaSlots[i].sprite = _nullManaIcon;
            }
            else
            {
                _manaSlots[i].sprite = TileController.Instance.GetManaSprite(_manaSupply[i]);
            }
        }
    }

    public void DetermineCastableSpells()
    {
        foreach (var spellSlot in _spellSlots)
        {
            if (spellSlot.LoadedSpell == null)
            {
                spellSlot.LowerSpellSlot_Fully();
            }
            else
            {
                if (spellSlot.LoadedSpell.CheckIfCastable(_manaSupply))
                {
                    spellSlot.RaiseSpellSlot();
                }
                else
                {
                    spellSlot.LowerSpellSlot_Partially();
                }

            }
        }

    }
}
