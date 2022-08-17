using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTree : DestroyableBase
{
    #region Unity Methods

    public override void Start()
    {
        base.Start();
        lootSpawnPoint = transform.GetChild(0);
        
    }

    #endregion Unity Methods



    #region Death

    public override IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(10f);
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    #endregion Death
}
