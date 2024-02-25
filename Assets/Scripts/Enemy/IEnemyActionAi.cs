using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyActionAi
{
    public bool isAgressive { get; set; }
    public float MaxActionRange { get; set; }
    public bool PlayerInRange(Transform player);
}
