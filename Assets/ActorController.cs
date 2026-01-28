using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public static ActorController Instance { get; private set; }

    //settings

    [SerializeField] ActorHandler _wizardPrefab;
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
        }
        else
        {
            _player.TeleportToNewTile(destinationTile, MoveTweenTime);
        }
    }
}
