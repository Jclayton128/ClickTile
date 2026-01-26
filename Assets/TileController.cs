using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public static TileController Instance { get; private set; }

    //ref
    [SerializeField] Transform _tileHolder = null;
    [SerializeField] TileHandler _tilePrefab = null;
    [SerializeField] List<TileTypeData> _tileTypesRaw = null;

    //settings
    [SerializeField] int _arenaSize = 7;
    public float ClickTweenTime { get; private set; } = 0.45f;


    //statev
    [SerializeField] List<TileHandler> _tilesRaw = new List<TileHandler>();
    Dictionary<Vector2Int, TileHandler> _tilesLocation = new Dictionary<Vector2Int, TileHandler>();
    
    #region Arena Setup

    private void Awake()
    {
        Instance = this;
        ConstructArena();       

        RemapTileLocations();
    }

    private void ConstructArena()
    {
        RemoveAllTiles();

        Vector2 walker = Vector2.zero;
        
        for (int x = -((_arenaSize - 1) / 2); x <= ((_arenaSize -1) / 2); x++)
        {
            for (int y = -((_arenaSize - 1) / 2); y <= ((_arenaSize - 1) / 2); y++)
            {
                walker.x = x;
                walker.y = y;
                CreateNewTile(walker);

            }
        }
    }

    private void CreateNewTile(Vector2 location)
    {
        TileHandler newTile = Instantiate(_tilePrefab, _tileHolder);
        newTile.transform.position = location;

        int rand = UnityEngine.Random.Range(0, _tileTypesRaw.Count);
        newTile.SetTile(_tileTypesRaw[rand]);

        _tilesRaw.Add(newTile);
    }

    private void CreateNewTile(Vector2 location, Vector2Int direction)
    {
        TileHandler newTile = Instantiate(_tilePrefab, _tileHolder);
        newTile.transform.position = location;
        newTile.SlideTile(direction);

        int rand = UnityEngine.Random.Range(0, _tileTypesRaw.Count);
        newTile.SetTile(_tileTypesRaw[rand]);

        _tilesRaw.Add(newTile);

        //Debug.Log($"new tile created at {location}");
    }

    private void RemoveAllTiles()
    {
        if (_tilesRaw.Count > 0)
        {
            for (int i = _tilesRaw.Count - 1; i >= 0; i--)
            {
                _tilesRaw[i].RemoveTile();
            }
            _tilesRaw.Clear();
        }

    }

    private void RemapTileLocations()
    {
        _tilesLocation.Clear();
        foreach (var tile in _tilesRaw)
        {
            Vector2Int vec2 = Vector2Int.zero;
            vec2.x = Mathf.RoundToInt(tile.transform.position.x);
            vec2.y = Mathf.RoundToInt(tile.transform.position.y);
            _tilesLocation.Add(vec2, tile);
        }
    }

    private Vector2Int GetVec2Int(TileHandler tile)
    {
        Vector2Int origin = Vector2Int.zero;
        origin.x = Mathf.RoundToInt(tile.transform.position.x);
        origin.y = Mathf.RoundToInt(tile.transform.position.y);
        return origin;

    }

    #endregion

    #region Gameplay
    public void HandleTileClick(TileHandler clickedTile)
    {
        List<TileHandler> orthoTiles = GetOrthogonalTiles(clickedTile);
        List<TileHandler> consumableOrthoTiles = PruneNonconsumableOrthoTiles(clickedTile, orthoTiles);
        RemoveConsumedTiles(clickedTile);
        SlideTilesTowardsOriginTile(clickedTile);
        Invoke(nameof(RemapTileLocations), ClickTweenTime + 0.01f);
    }

    private List<TileHandler> GetOrthogonalTiles(TileHandler originTile)
    {
        List<TileHandler> orthoTiles = new List<TileHandler>();

        Vector2Int origin = GetVec2Int(originTile);

        Vector2Int walker = origin;
        walker.x -= 1;
        if (_tilesLocation.ContainsKey(walker))
        {
            orthoTiles.Add(_tilesLocation[walker]);
        }

        walker = origin;
        walker.x += 1;
        if (_tilesLocation.ContainsKey(walker))
        {
            orthoTiles.Add(_tilesLocation[walker]);
        }

        walker = origin;
        walker.y -= 1;
        if (_tilesLocation.ContainsKey(walker))
        {
            orthoTiles.Add(_tilesLocation[walker]);
        }

        walker = origin;
        walker.y += 1;
        if (_tilesLocation.ContainsKey(walker))
        {
            orthoTiles.Add(_tilesLocation[walker]);
        }

        return orthoTiles;
    }

    private List<TileHandler> PruneNonconsumableOrthoTiles(TileHandler clickedTile, List<TileHandler> orthoTiles)
    {
        List<TileHandler> consumableOrthoTiles = new List<TileHandler>();
        //foreach ortho tile, compare it against the clicked Tile's TileType.


        return consumableOrthoTiles;
    }

    private void RemoveConsumedTiles(TileHandler originTile)
    {
        Vector2Int origin = GetVec2Int(originTile);

        Vector2Int walker = origin;
        walker.x -= 1;
        Vector2Int direction = new Vector2Int(1, 0);
        if (_tilesLocation.ContainsKey(walker))
        {
            _tilesLocation[walker].AbsorbTile(direction);
        }

        walker = origin;
        walker.x += 1;
        direction = new Vector2Int(-1, 0);
        if (_tilesLocation.ContainsKey(walker))
        {
            _tilesLocation[walker].AbsorbTile(direction);
        }
        walker = origin;
        walker.y -= 1;
        direction = new Vector2Int(0, 1);
        if (_tilesLocation.ContainsKey(walker))
        {
            _tilesLocation[walker].AbsorbTile(direction);
        }

        walker = origin;
        walker.y += 1;
        direction = new Vector2Int(0, -1);
        if (_tilesLocation.ContainsKey(walker))
        {
            _tilesLocation[walker].AbsorbTile(direction);
        }
    }


    public void DeregisterTile(TileHandler tile)
    {
        _tilesRaw.Remove(tile);
    }

    private void SlideTilesTowardsOriginTile(TileHandler originTile)
    {
        Vector2Int origin = GetVec2Int(originTile);

        //Slide western tiles to the east
        Vector2Int walker = origin;
        Vector2Int direction = Vector2Int.zero;
        direction.x = 1;
        for (int i = 2; i < 99; i++)
        {
            walker.x = origin.x - i;


            if (_tilesLocation.ContainsKey(walker))
            {
                _tilesLocation[walker].SlideTile(direction);
            }
            else
            {
                if (Mathf.Abs(walker.x) > ((_arenaSize - 1) / 2) + 1.1f)
                {
                    //gap detected, but because its an edge
                    break;
                }
                else
                {
                    //gap detected, spawn new tile
                    CreateNewTile(walker, direction);
                    break;
                }
            }
        }

        //slide eastern tiles to the west
        walker = origin;
        direction = Vector2Int.zero;
        direction.x = -1;
        for (int i = 2; i < 99; i++)
        {
            walker.x = origin.x + i;


            if (_tilesLocation.ContainsKey(walker))
            {
                _tilesLocation[walker].SlideTile(direction);
            }
            else
            {
                if (Mathf.Abs(walker.x) > ((_arenaSize - 1) / 2) + 1.1f)
                {
                    //gap detected, but because its an edge
                    break;
                }
                else
                {
                    //gap detected, spawn new tile
                    CreateNewTile(walker, direction);
                    break;
                }
            }
        }

        //slide northern tiles to the south
        walker = origin;
        direction = Vector2Int.zero;
        direction.y = -1;
        for (int i = 2; i < 99; i++)
        {
            walker.y = origin.y + i;


            if (_tilesLocation.ContainsKey(walker))
            {
                _tilesLocation[walker].SlideTile(direction);
            }
            else
            {
                if (Mathf.Abs(walker.y) > ((_arenaSize - 1) / 2) + 1.1f)
                {
                    //gap detected, but because its an edge
                    break;
                }
                else
                {
                    //gap detected, spawn new tile
                    CreateNewTile(walker, direction);
                    break;
                }
            }
        }

        //slide southern tiles to the north
        walker = origin;
        direction = Vector2Int.zero;
        direction.y = 1;
        for (int i = 2; i < 99; i++)
        {
            walker.y = origin.y - i;


            if (_tilesLocation.ContainsKey(walker))
            {
                _tilesLocation[walker].SlideTile(direction);
            }
            else
            {
                if (Mathf.Abs(walker.y) > ((_arenaSize - 1) / 2) + 1.1f)
                {
                    //gap detected, but because its an edge
                    break;
                }
                else
                {
                    //gap detected, spawn new tile
                    CreateNewTile(walker, direction);
                    break;
                }
            }
        }

    }

    #endregion
}
