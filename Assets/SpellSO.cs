using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using DG.Tweening;


[CreateAssetMenu(menuName = "Spell SO")]
public class SpellSO : ScriptableObject
{
    public enum TargetingTypes { Aura, Orthogonal, Diagonal, TerrainFirstMana, Random}
    public enum EffectTypes { Damage, Shield, Heal, Poison, Stun, Ignite}

    public enum DepictionTypes { Projectile, Beam, Pillar, FromGround}
    
    //settings
    [SerializeField] string _spellName = "Default Spell";
    [SerializeField] List<TileHandler.TileTypes> _manaCost = new List<TileHandler.TileTypes>();

    [SerializeField] TargetingTypes _targetingType = TargetingTypes.Aura;
    [SerializeField] List<EffectTypes> _effects = null;

    [SerializeField] DepictionTypes _depiction = DepictionTypes.Projectile;
    [SerializeField] GameObject _projectilePrefab = null;
    [SerializeField] float _depictionLength = 0.5f;

    [SerializeField] int _range = 1;
    [SerializeField] int _magnitude = 1;

    //state
    public string SpellName => _spellName;
    public List<TileHandler.TileTypes> ManaCost => _manaCost;
    //public TargetingTypes TargetingType => _targetingType;
    //public List<EffectTypes> Effects => _effects;
    //public DepictionTypes Depiction => _depiction;
    //public int Range => _range;
    //public int Magnitude => _magnitude;



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

        //create the depiction
        switch (_depiction)
        {
            case DepictionTypes.Projectile:
                CastProjectile();
                break;
        }
    }

    private void CastProjectile()
    {
        if (_targetingType == TargetingTypes.Orthogonal)
        {
            List<GameObject> targets = FindOrthogonalTargetsWithinRange();
            foreach (var target in targets)
            {
                GameObject projectile = Instantiate(_projectilePrefab, ActorController.Instance.Player.CurrentTile.transform.position, Quaternion.identity);
                projectile.transform.DOJump(target.transform.position, 1f, 1, _depictionLength);
                target.GetComponent<HealthHandler>().ReceiveDamage(_magnitude);
            }
        }

        else if (_targetingType == TargetingTypes.Diagonal)
        {
            List<GameObject> targets = FindDiagonalTargetsWithinRange();
            foreach (var target in targets)
            {
                GameObject projectile = Instantiate(_projectilePrefab, ActorController.Instance.Player.CurrentTile.transform.position, Quaternion.identity);
                projectile.transform.DOJump(target.transform.position, 1f, 1, _depictionLength);
            }
        }
    }

    private List<GameObject> FindOrthogonalTargetsWithinRange()
    {
        List<GameObject> targets = new List<GameObject>();
        Vector2Int playerV2I = TileController.Instance.GetVec2Int(ActorController.Instance.Player.CurrentTile);
        Vector2Int testV2I;
        TileHandler testTile;

        //Check X+
        for (int i = 0; i <= _range; i++)
        {
            testV2I = playerV2I + new Vector2Int(i, 0);
            testTile = null;

            testTile = TileController.Instance.GetTileAtVec2Int(testV2I);
            if (testTile != null && testTile.OccupyingActor != null &&
                testTile.OccupyingActor.CanBeTargetedByPlayerSpell)
            {
                targets.Add(testTile.OccupyingActor.gameObject);
                break;
            }
        }

        //Check X-
        for (int i = 0; i <= _range; i++)
        {
            testV2I = playerV2I + new Vector2Int(-i, 0);
            testTile = null;

            testTile = TileController.Instance.GetTileAtVec2Int(testV2I);
            if (testTile != null && testTile.OccupyingActor != null &&
                testTile.OccupyingActor.CanBeTargetedByPlayerSpell)
            {
                targets.Add(testTile.OccupyingActor.gameObject);
                break;
            }
        }

        //Check Y+
        for (int i = 0; i <= _range; i++)
        {
            testV2I = playerV2I + new Vector2Int(0, i);
            testTile = null;

            testTile = TileController.Instance.GetTileAtVec2Int(testV2I);
            if (testTile != null && testTile.OccupyingActor != null &&
                testTile.OccupyingActor.CanBeTargetedByPlayerSpell)
            {
                targets.Add(testTile.OccupyingActor.gameObject);
                break;
            }
        }

        //Check Y-
        for (int i = 0; i <= _range; i++)
        {
            testV2I = playerV2I + new Vector2Int(0, -i);
            testTile = null;

            testTile = TileController.Instance.GetTileAtVec2Int(testV2I);
            if (testTile != null && testTile.OccupyingActor != null &&
                testTile.OccupyingActor.CanBeTargetedByPlayerSpell)
            {
                targets.Add(testTile.OccupyingActor.gameObject);
                break;
            }
        }
        return targets;
        //Debug.Log($"Targeting found {targets.Count} targets after checking {tilesChecked} tiles");

    }

    private List<GameObject> FindDiagonalTargetsWithinRange()
    {
        List<GameObject> targets = new List<GameObject>();
        Vector2Int playerV2I = TileController.Instance.GetVec2Int(ActorController.Instance.Player.CurrentTile);
        Vector2Int testV2I;
        TileHandler testTile;

        //Check X+, Y+
        for (int i = 0; i <= _range; i++)
        {
            testV2I = playerV2I + new Vector2Int(i, i);
            testTile = null;

            testTile = TileController.Instance.GetTileAtVec2Int(testV2I);
            if (testTile != null && testTile.OccupyingActor != null &&
                testTile.OccupyingActor.CanBeTargetedByPlayerSpell)
            {
                targets.Add(testTile.OccupyingActor.gameObject);
                break;
            }
        }

        //Check X+, Y-
        for (int i = 0; i <= _range; i++)
        {
            testV2I = playerV2I + new Vector2Int(i, -i);
            testTile = null;

            testTile = TileController.Instance.GetTileAtVec2Int(testV2I);
            if (testTile != null && testTile.OccupyingActor != null &&
                testTile.OccupyingActor.CanBeTargetedByPlayerSpell)
            {
                targets.Add(testTile.OccupyingActor.gameObject);
                break;
            }
        }

        //Check X-, Y-
        for (int i = 0; i <= _range; i++)
        {
            testV2I = playerV2I + new Vector2Int(-i, -i);
            testTile = null;

            testTile = TileController.Instance.GetTileAtVec2Int(testV2I);
            if (testTile != null && testTile.OccupyingActor != null &&
                testTile.OccupyingActor.CanBeTargetedByPlayerSpell)
            {
                targets.Add(testTile.OccupyingActor.gameObject);
                break;
            }
        }

        //Check X-, Y+
        for (int i = 0; i <= _range; i++)
        {
            testV2I = playerV2I + new Vector2Int(-i, i);
            testTile = null;

            testTile = TileController.Instance.GetTileAtVec2Int(testV2I);
            if (testTile != null && testTile.OccupyingActor != null &&
                testTile.OccupyingActor.CanBeTargetedByPlayerSpell)
            {
                targets.Add(testTile.OccupyingActor.gameObject);
                break;
            }
        }
        return targets;
        //Debug.Log($"Targeting found {targets.Count} targets after checking {tilesChecked} tiles");

    }
}



