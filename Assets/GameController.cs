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
        //ActorController.Instance.SpawnPlayer();
        //ActorController.Instance.SpawnSlime();
        MagicController.Instance.ClearAllMana();
        MagicController.Instance.LoadStartingSpells();
        MagicController.Instance.DetermineCastableSpells();

    }

}

