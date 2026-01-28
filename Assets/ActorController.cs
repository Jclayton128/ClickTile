using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public static ActorController Instance { get; private set; }

    //settings

    [SerializeField] ActorHandler _wizardPrefab;
    [SerializeField] ActorHandler _slimePrefab;
    [SerializeField] ActorHandler _flyPrefab;
    public float MoveTweenTime = 0.75f;

    //state
    ActorHandler _player;
    public ActorHandler Player => _player;


    private void Awake()
    {
        Instance = this;
    }

    public void MovePlayer(TileHandler destinationTile)
    {
        if (!_player)
        {
            _player = Instantiate(_wizardPrefab, destinationTile.VisualsTransform);
            _player.HandleInitialTilePlacement(destinationTile);
        }
        else
        {
            _player.TeleportToNewTile(destinationTile, MoveTweenTime);
        }
    }

    [ContextMenu ("Spawn Fly")]
    public void SpawnFly()
    {
        TileHandler tileToPlaceAt = TileController.Instance.GetTileAtVec2Int(new Vector2Int(1, 1));
        Instantiate(_flyPrefab, tileToPlaceAt.VisualsTransform);
    }

    [ContextMenu("Spawn Slime")]
    public void SpawnSlime()
    {
        TileHandler tileToPlaceAt = TileController.Instance.GetTileAtVec2Int(new Vector2Int(1, 4));
        Instantiate(_slimePrefab, tileToPlaceAt.VisualsTransform);
    }
}
