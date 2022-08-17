using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyablePlant : DestroyableBase
{
    [SerializeField] 
    protected ItemClass seedLoot;

    [SerializeField] 
    protected int seedLootAmountMin, seedLootAmountMax;



    #region Unity Methods

    public override void Start()
    {
        base.Start();
    }

    #endregion Unity Methods




    #region Hit Process

    public override void SpawnDeathLoot()
    {
        int _rewardAmount = Random.Range(rewardAmountMin, rewardAmountMax);

        for (int i = 0; i < _rewardAmount; i++)
        {
            Instantiate(loot[Random.Range(0, loot.Count)].dropReference, lootSpawnPoint.position, Quaternion.identity);
        }


        int _seedRewardAmount = Random.Range(seedLootAmountMin, seedLootAmountMax);

        for (int i = 0; i < _seedRewardAmount; i++)
        {
            Instantiate(seedLoot.dropReference, lootSpawnPoint.position, Quaternion.identity);
        }
    }

    #endregion Hit Process




    #region Death

    public override IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    #endregion Death
}
