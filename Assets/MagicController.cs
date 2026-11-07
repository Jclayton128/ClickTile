using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MagicController : MonoBehaviour
{
    public static MagicController Instance { get; private set; }

    //refs
    [SerializeField] List<Image> _manaSlots = null;

    //settings
    [SerializeField] Sprite _nullManaIcon = null;

    //state

    [SerializeField] TileHandler.TileTypes[] _manaCollected =
        {TileHandler.TileTypes.Undefined,
        TileHandler.TileTypes.Undefined,
        TileHandler.TileTypes.Undefined,
        TileHandler.TileTypes.Undefined };

    private void Awake()
    {
        Instance = this;
    }

    public void GainMana(TileHandler.TileTypes manaType)
    {
        for (int i = 0; i < _manaCollected.Length; i++)
        {
            if (_manaCollected[i] == TileHandler.TileTypes.Undefined)
            {
                _manaCollected[i] = manaType;
                break;
            }
        }

        RenderManaCollected();
    }

    public void SpendMana(TileHandler.TileTypes manaType)
    {
        for (int i = 0; i < _manaCollected.Length; i++)
        {
            if (_manaCollected[i] == manaType)
            {
                _manaCollected[i] = TileHandler.TileTypes.Undefined;
            }
        }

        RenderManaCollected();
    }

    public void ClearAllMana()
    {
        for (int i = 0; i < _manaCollected.Length; i++)
        {
            _manaCollected[i] = TileHandler.TileTypes.Undefined;
        }
        RenderManaCollected();
    }

    private void RenderManaCollected()
    {
        for (int i = 0; i < _manaCollected.Length; i++)
        {
            if (_manaCollected[i] == TileHandler.TileTypes.Undefined)
            {
                _manaSlots[i].sprite = _nullManaIcon;
            }
            else
            {
                _manaSlots[i].sprite = TileController.Instance.GetManaSprite(_manaCollected[i]);
            }
        }
    }
}
