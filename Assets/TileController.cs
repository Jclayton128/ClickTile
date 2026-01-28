using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public static TileController Instance { get; private set; }
    public Action TileRemappingComplete;


    //ref
    [SerializeField] Transform _tileHolder = null;
    [SerializeField] TileHandler _tilePrefab = null;
    [SerializeField] List<TileTypeData> _tileTypesRaw = null;
    [SerializeField] GameObject _towerPrefab = null;

    //settings
    [SerializeField] int _arenaSize = 7;
    public float ClickTweenTime = 0.75f;
    float _yFactor = 0.7f;
    Vector2Int _startingTower = new Vector2Int(3, 3);

    //state
    bool _isProcessingClick = false;
    [SerializeField] List<TileHandler> _tilesRaw = new List<TileHandler>();
    Dictionary<Vector2Int, TileHandler> _tilesLocation = new Dictionary<Vector2Int, TileHandler>();
    
    #region Arena Setup

    private void Awake()
    {
        Instance = this;

    }

    public void BuildNewArena()
    {
        ConstructArena();
        RemapTileLocations();
        AddTower();
    }


    private void ConstructArena()
    {
        RemoveAllTiles();

        Vector2 walker = Vector2.zero;
        
        for (int ix = 0; ix < _arenaSize; ix++)
        {
            for (int iy = 0; iy < _arenaSize; iy++)
            {
                walker.x = ix;
                walker.y = iy * _yFactor;
                CreateNewTile(walker);
            }
        }

        //for (int x = -((_arenaSize - 1) / 2); x <= ((_arenaSize -1) / 2); x++)
        //{
        //    for (int y = -((_arenaSize - 1) / 2); y <= ((_arenaSize - 1) / 2); y++)
        //    {
        //        walker.x = x;
        //        walker.y = y * 0.7f;
        //        CreateNewTile(walker);

        //    }
        //}
    }
    private void AddTower()
    {
        if (_tilesLocation.ContainsKey(_startingTower))
        {
            Instantiate(_towerPrefab, _tilesLocation[_startingTower].VisualsTransform);
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

    private void CreateNewTile(Vector2 location, Vector2 slideVector)
    {
        TileHandler newTile = Instantiate(_tilePrefab, _tileHolder);
        newTile.transform.position = location;
        newTile.SlideTile(slideVector);

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
            _tilesLocation.Add(GetVec2Int(tile), tile);
        }
        TileRemappingComplete?.Invoke();

    }



    #endregion

    #region Helpers
    public Vector2Int GetVec2Int(TileHandler tile)
    {
        Vector2Int origin = Vector2Int.zero;
        origin.x = Mathf.RoundToInt(tile.transform.position.x);
        origin.y = Mathf.RoundToInt(tile.transform.position.y / _yFactor);
        return origin;

    }

    public TileHandler GetTileAtVec2Int(Vector2Int vector2Int)
    {
        if (_tilesLocation.ContainsKey(vector2Int))
        {
            return _tilesLocation[vector2Int];
        }
        else
        {
            return null;
        }
    }

    public TileHandler GetTileAtWorldPosition(Vector2 worldPosition)
    {
        Vector2Int origin = Vector2Int.zero;
        origin.x = Mathf.RoundToInt(worldPosition.x);
        origin.y = Mathf.RoundToInt(worldPosition.y / _yFactor);

        if (_tilesLocation.ContainsKey(origin))
        {
            return _tilesLocation[origin];
        }
        else
        {
            Debug.Log($"No tiles found at {origin}");
            return null;
        }
    }
    public List<TileHandler> GetOrthogonalTiles(TileHandler originTile)
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
        //Debug.Log($"clicked on {origin}, which has {orthoTiles.Count} neighbors");
        return orthoTiles;
    }
    #endregion

    #region Gameplay
    TileHandler _clickedTile;
    List<TileHandler> _orthoTiles = new List<TileHandler>();
    List<TileHandler> _consumableOrthoTiles = new List<TileHandler>();

    public void HandleTileClick(TileHandler clickedTile)
    {
        if (ActorController.Instance.PriorityActor != ActorController.Instance.Player) return;
        if (_isProcessingClick) return;
        _isProcessingClick = true;

        _clickedTile = null;
        _orthoTiles.Clear();
        _consumableOrthoTiles.Clear();

        _clickedTile = clickedTile;

        ActorController.Instance.MovePlayer(clickedTile);
        
        _orthoTiles = GetOrthogonalTiles(_clickedTile);
        Invoke(nameof(Delay_HandleTileClick), ActorController.Instance.MoveTweenTime);
    }

    private void Delay_HandleTileClick()
    {
        _consumableOrthoTiles = PruneNonconsumableOrthoTiles();

        GainPowerFromConsumedTiles();

        RemoveConsumedTiles();
        Invoke(nameof(SlideTilesTowardsOriginTile), ClickTweenTime - 0.1f); //ClickTweenTime + 

        //VibrateNonconsumedOrthoTiles

        float timeToComplete = ClickTweenTime + ClickTweenTime + 0.02f;
        Invoke(nameof(Delay2_HandleTileClick), timeToComplete);
    }

    private void Delay2_HandleTileClick()
    {
        RemapTileLocations();
        HandleClickProcessingComplete();
        ActorController.Instance.HandlePriorityActorTurnCompletion();
    }



    private List<TileHandler> PruneNonconsumableOrthoTiles()
    {
        // List<TileHandler> consumableOrthoTiles = new List<TileHandler>(_orthoTiles); //shim to allow all ortho tiles as consumable

        List<TileHandler> consumableOrthoTiles = new List<TileHandler>();

        foreach (var tile in _orthoTiles)
        {
            if (tile.OccupyingActor != null) continue;
            else
            {
                consumableOrthoTiles.Add(tile);
            }
        }

        //if (clickedTile.TileData.TileType != TileHandler.TileTypes.Veggies)
        //{
        //    foreach (var tile in orthoTiles)
        //    {
        //        //foreach ortho tile, compare it against the clicked Tile's TileType.

        //        if (clickedTile.TileData.Prey.Contains(tile.TileData.TileType))
        //        {
        //            consumableOrthoTiles.Add(tile);
        //        }
        //        else if (clickedTile.TileData.TileType == tile.TileData.TileType)
        //        {
        //            consumableOrthoTiles.Add(tile);
        //        }
        //    }
        //}
        

        return consumableOrthoTiles;
    }

    private void GainPowerFromConsumedTiles()
    {
        int count_grass = 0;
        int count_rock = 0;
        int count_snow = 0;
        int count_sand = 0;

        foreach (var tile in _consumableOrthoTiles)
        {
            if (tile.TileData.TileType == TileHandler.TileTypes.Grass)
            {
                Debug.Log("more grass");
                count_grass++;
            }

            else if (tile.TileData.TileType == TileHandler.TileTypes.Rock)
            {
                Debug.Log("more rock");
                count_rock++;
            }

            else if(tile.TileData.TileType == TileHandler.TileTypes.Snow)
            {
                Debug.Log("more snow");
                count_snow++;
            }

            else if(tile.TileData.TileType == TileHandler.TileTypes.Sand)
            {
                Debug.Log("more sand");
                count_sand++;
            }
        }

        string basic = $"Consumed {_consumableOrthoTiles.Count} and gained {count_grass} Grass, {count_rock} Rock, {count_sand} Sand, and {count_snow} Snow.";
        Debug.Log(basic);

    }

    private void RemoveConsumedTiles()
    {
        Vector2Int origin = GetVec2Int(_clickedTile);

        Vector2Int walker = origin;
        Dictionary<Vector2Int, TileHandler> consumableTileLocations = new Dictionary<Vector2Int, TileHandler>();
        foreach (var tile in _consumableOrthoTiles)
        {
            consumableTileLocations.Add(GetVec2Int(tile), tile);
        }

        walker.x -= 1;
        Vector2 slideDirection = new Vector2(1, 0);
        if (consumableTileLocations.ContainsKey(walker))
        {
            consumableTileLocations[walker].AbsorbTile(slideDirection);
        }

        walker = origin;
        walker.x += 1;
        slideDirection = new Vector2(-1, 0);
        if (consumableTileLocations.ContainsKey(walker))
        {
            consumableTileLocations[walker].AbsorbTile(slideDirection);
        }
        walker = origin;
        walker.y -= 1;
        slideDirection = new Vector2(0, 1 * _yFactor);
        if (consumableTileLocations.ContainsKey(walker))
        {
            consumableTileLocations[walker].AbsorbTile(slideDirection);
        }

        walker = origin;
        walker.y += 1;
        slideDirection = new Vector2(0, -1 * _yFactor);
        if (consumableTileLocations.ContainsKey(walker))
        {
            consumableTileLocations[walker].AbsorbTile(slideDirection);
        }
    }


    public void DeregisterTile(TileHandler tile)
    {
        _tilesRaw.Remove(tile);
    }

    private void SlideTilesTowardsOriginTile()
    {
        Vector2Int origin = GetVec2Int(_clickedTile);

        Dictionary<Vector2Int, TileHandler> consumableTileLocations = new Dictionary<Vector2Int, TileHandler>();
        foreach (var tile in _consumableOrthoTiles)
        {
            consumableTileLocations.Add(GetVec2Int(tile), tile);
        }

        Vector2Int walker = origin;
        Vector2 slideVector = Vector2.zero;
        if (consumableTileLocations.ContainsKey(new Vector2Int(origin.x-1, origin.y)))
        {
            //Slide western tiles to the east

            slideVector.x = 1;
            for (int i = 2; i < 99; i++)
            {
                walker.x = origin.x - i;


                if (_tilesLocation.ContainsKey(walker))
                {
                    _tilesLocation[walker].SlideTile(slideVector);
                }
                else
                {
                    if (walker.x >= _arenaSize)
                    {
                        //gap detected, but because its an edge
                        break;
                    }
                    else
                    {
                        //gap detected, spawn new tile
                        CreateNewTile(new Vector2(walker.x, walker.y * _yFactor), slideVector);
                        break;
                    }
                }
            }
        }

        if (consumableTileLocations.ContainsKey(new Vector2Int(origin.x + 1, origin.y)))
        {

            //slide eastern tiles to the west
            walker = origin;
            slideVector = Vector2Int.zero;
            slideVector.x = -1;
            for (int i = 2; i < 99; i++)
            {
                walker.x = origin.x + i;


                if (_tilesLocation.ContainsKey(walker))
                {
                    _tilesLocation[walker].SlideTile(slideVector);
                }
                else
                {
                    if (walker.x < 0)
                    {
                        //gap detected, but because its an edge
                        break;
                    }
                    else
                    {
                        //gap detected, spawn new tile
                        CreateNewTile(new Vector2(walker.x, walker.y * _yFactor), slideVector);
                        break;
                    }
                }
            }
        }

        if (consumableTileLocations.ContainsKey(new Vector2Int(origin.x, origin.y + 1)))
        {
            //slide northern tiles to the south
            walker = origin;
            slideVector = Vector2Int.zero;
            slideVector.y = -1 * _yFactor;
            for (int i = 2; i < 99; i++)
            {
                walker.y = origin.y + i;


                if (_tilesLocation.ContainsKey(walker))
                {
                    _tilesLocation[walker].SlideTile(slideVector);
                }
                else
                {
                    if (walker.y > _arenaSize)
                    {
                        //gap detected, but because its an edge
                        break;
                    }
                    else
                    {
                        //gap detected, spawn new tile
                        CreateNewTile(new Vector2(walker.x, walker.y * _yFactor), slideVector);
                        break;
                    }
                }
            }
        }

        if (consumableTileLocations.ContainsKey(new Vector2Int(origin.x, origin.y - 1)))
        {

            //slide southern tiles to the north
            walker = origin;
            slideVector = Vector2Int.zero;
            slideVector.y = 1 * _yFactor;
            for (int i = 2; i < 99; i++)
            {
                walker.y = origin.y - i;


                if (_tilesLocation.ContainsKey(walker))
                {
                    _tilesLocation[walker].SlideTile(slideVector);
                }
                else
                {
                    if (walker.y >= _arenaSize)
                    {
                        //gap detected, but because its an edge
                        break;
                    }
                    else
                    {
                        //gap detected, spawn new tile
                        CreateNewTile(new Vector2(walker.x, walker.y * _yFactor), slideVector);
                        break;
                    }
                }
            }
        }

    }

    private void HandleClickProcessingComplete()
    {
        _isProcessingClick = false;
    }

    #endregion

    #region All Tile Effects

    public void DimAllTiles()
    {
        foreach (var tile in _tilesRaw)
        {
            tile.DimTile();
        }
    }

    public void HalfdimOrthogonalTiles(TileHandler hoveredTile)
    {
        List<TileHandler> orthoTiles = GetOrthogonalTiles(hoveredTile);
        foreach (var tile in orthoTiles)
        {
            tile.HalfdimTile();
        }
    }

    public void UnraiseAllTiles()
    {
        foreach (var tile in _tilesRaw)
        {
            tile.UnraiseTile();
        }
    }

    public void HalfraiseAllOrthogonalTiles(TileHandler originTile)
    {
        List<TileHandler> orthoTiles = GetOrthogonalTiles(originTile);
        foreach (var tile in orthoTiles)
        {
            tile.HalfraiseTile();
        }
    }

    #endregion
}
