using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(menuName = "Spell SO")]
public class SpellSO : ScriptableObject
{
    public enum TargetingTypes { Aura, Orthogonal, Diagonal, TerrainFirstMana, Random}
    public enum EffectTypes { Damage, Shield, Heal, Poison, Stun, Ignite}
    
    //settings
    [SerializeField] string _spellName = "Default Spell";
    [SerializeField] List<TileHandler.TileTypes> _manaCost = new List<TileHandler.TileTypes>();

    [SerializeField] TargetingTypes _targetingType = TargetingTypes.Aura;
    [SerializeField] List<EffectTypes> _effects = null;
    [SerializeField] int _range = 1;
    [SerializeField] int _impact = 1;

    //state
    public string SpellName => _spellName;
    public List<TileHandler.TileTypes> ManaCost => _manaCost;
    public TargetingTypes TargetingType => _targetingType;
    public int Range => _range;
    public List<EffectTypes> Effects => _effects;


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

    public void Cast()
    {
        Debug.Log($"Casting " + SpellName);

        foreach (var mana in _manaCost)
        {
            MagicController.Instance.SpendMana(mana);
        }
    }
}



