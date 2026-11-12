using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PointController : MonoBehaviour
{
    public static PointController Instance { get; private set; }

    //ref
    [SerializeField] TextMeshProUGUI _scoreTMP = null;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TileController.Instance.TileRemappingComplete += HandleTileRemappingComplete;
    }

    private void HandleTileRemappingComplete()
    {
        SumAllTilePoints();
    }

    private void SumAllTilePoints()
    {
        int sum = 0;
        foreach (var tile in TileController.Instance.Tiles)
        {
            sum += tile.Points;
        }

        _scoreTMP.text = sum.ToString();    
    }
}
