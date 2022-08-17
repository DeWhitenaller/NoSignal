using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableStone : DestroyableBase
{

    #region Unity Methods

    public override void Start()
    {
        base.Start();

        if (!lootSpawnPoint)
        {
            try
            {
                lootSpawnPoint = transform.GetChild(0);
            }
            catch
            {
                Debug.LogError("Object has no lootSpawnPoint!", gameObject);
                throw;
            }
        }
    }

    #endregion Unity Methods



    #region Death

    public override IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        GetComponent<MeshCollider>().enabled = false;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    #endregion Death
}
