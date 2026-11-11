using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    public void StartNewRun()
    {
        TileController.Instance.BuildNewArena();
        ActorController.Instance.SpawnPlayer();
        ActorController.Instance.SpawnArmy();
        ActorController.Instance.SpawnArmy();
        ActorController.Instance.SpawnArmy();
        ActorController.Instance.SpawnSlime();
        ActorController.Instance.SpawnSlime();
        ActorController.Instance.SpawnSlime();
    }
}
