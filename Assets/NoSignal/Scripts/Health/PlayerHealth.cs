using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthBase
{
    protected override void OnHealthUnderZero()
    {
        Destroy(gameObject);
    }

}
