using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{

    //settings
    [SerializeField] int _healthStarting = 3;
    [SerializeField] int _healthCurrent;

    

    private void Awake()
    {
        _healthCurrent = _healthStarting;
    }

    public void ReceiveDamage(int damageToReceive)
    {
        _healthCurrent -= damageToReceive;
        if (_healthCurrent <= 0)
        {
            StartDying();
        }
    }

    private void StartDying()
    {
        Die();
    }

    private void Die()
    {
        GetComponent<ActorHandler>().DestroySelf();
    }
}
