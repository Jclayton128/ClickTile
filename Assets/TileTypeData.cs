using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TileTypeData")]
public class TileTypeData : ScriptableObject
{

    [SerializeField] TileHandler.TileTypes _tileType = TileHandler.TileTypes.Undefined;

    [SerializeField] Color _color = Color.white;
    [SerializeField] Sprite _icon = null;
    [SerializeField] List<TileHandler.TileTypes> _prey = null;
    [SerializeField] string _monoletter = "D";

    public TileHandler.TileTypes TileType => _tileType;
    public Color TileColor => _color;
    public Sprite TileIcon => _icon;
    public List<TileHandler.TileTypes> Prey => _prey;
    public string Monoletter => _monoletter;
}
