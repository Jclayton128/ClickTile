using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell SO")]
public class SpellSO : ScriptableObject
{
    //settings
    [SerializeField] string _spellName = "Default Spell";
    [SerializeField] List<TileHandler.TileTypes> _manaCost = new List<TileHandler.TileTypes>();

    //state
    public string SpellName => _spellName;
    public List<TileHandler.TileTypes> ManaCost => _manaCost;

    public bool CheckIfCastable(List<TileHandler.TileTypes> availableMana)
    {
        List<TileHandler.TileTypes> manaCostRemaining = new List<TileHandler.TileTypes>(_manaCost);
        List<TileHandler.TileTypes> manaSupplyRemaining = new List<TileHandler.TileTypes>(availableMana);

        if (manaCostRemaining.Count > manaSupplyRemaining.Count)
        {
            return false;
        }

        for (int i = manaCostRemaining.Count - 1; i >= 0; i--)
        {

            for (int j = manaSupplyRemaining.Count - 1; j >= 0; j--)
            {
                if (manaSupplyRemaining[j] == manaCostRemaining[i])
                {
                    manaSupplyRemaining.RemoveAt(j);
                    manaCostRemaining.RemoveAt(i);
                    break;
                }
            }
        }

        if (manaCostRemaining.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}



