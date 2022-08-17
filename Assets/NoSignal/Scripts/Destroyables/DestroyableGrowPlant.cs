using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableGrowPlant : DestroyablePlant
{
    protected PlantGrow plantGrow;

    [SerializeField] 
    protected GameObject objectToDestroyOnDeath;




    #region Unity Methods

    public override void Start()
    {
        base.Start();
        plantGrow = GetComponentInParent<PlantGrow>();
    }

    #endregion Unity Methods




    #region Death

    public override IEnumerator DestroyObject()
    {
        Transform parent = transform.parent;
        parent.transform.parent = null;

        plantGrow.OnRemovePlant();
        
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(1f);

        Destroy(objectToDestroyOnDeath);
    }

    #endregion Death
}
