using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TileTypeData")]
public class TileTypeData : ScriptableObject
{

    [SerializeField] TileHandler.TileTypes _tileType = TileHandler.TileTypes.Undefined;

    [SerializeField] Sprite _tileSprite = null;
    [SerializeField] Sprite _tileIcon = null;

    public TileHandler.TileTypes TileType => _tileType;
    public Sprite TileSprite => _tileSprite;
    public Sprite TileIcon => _tileIcon;

}
