using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public static ActorController Instance { get; private set; }

    //settings

    [SerializeField] ActorHandler _wizardPrefab;
    [SerializeField] ActorHandler _slimePrefab;
    [SerializeField] ActorHandler _flyPrefab;
    public float MoveTweenTime = 0.75f;
    Vector2Int _playerStartingSpot = new Vector2Int(3, 2); //Just below the tower.

    //state
    ActorHandler _player;
    public ActorHandler Player => _player;

    [SerializeField] List<ActorHandler> _actorTurnOrder = new List<ActorHandler>();
    public ActorHandler PriorityActor => _actorTurnOrder[0];

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnPlayer()
    {
        _player = Instantiate(_wizardPrefab, TileController.Instance.GetTileAtVec2Int(_playerStartingSpot).VisualsTransform);
        _player.HandleInitialTilePlacement(TileController.Instance.GetTileAtVec2Int(_playerStartingSpot));
        AddActorToStartOfTurnOrder(_player);

        MagicController.Instance.ClearAllMana();
        MagicController.Instance.LoadStartingSpells();
        MagicController.Instance.DetermineCastableSpells();
    }

    public void MovePlayer(TileHandler destinationTile)
    {
        _player.TeleportToNewTile(destinationTile, MoveTweenTime);
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
        ActorHandler slime = Instantiate(_slimePrefab, tileToPlaceAt.VisualsTransform);

        AddActorToEndOfTurnOrder(slime);
    }

    #region Turn Mechanics

    public void StartBattle()
    {
        _actorTurnOrder.Clear();
    }

    public void AddActorToStartOfTurnOrder(ActorHandler newActor)
    {
        _actorTurnOrder.Insert(0, newActor);
    }

    public void AddActorToEndOfTurnOrder(ActorHandler newActor)
    {
        _actorTurnOrder.Add(newActor);
    }

    public void HandlePriorityActorTurnCompletion()
    {
        ActorHandler completedActor = PriorityActor;
        _actorTurnOrder.Remove(completedActor);
        AddActorToEndOfTurnOrder(completedActor);

        PriorityActor.BeginTurn();
    }

    #endregion
}
