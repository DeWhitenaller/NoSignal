using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrow : MonoBehaviour
{
    private Inventory inventory;



    [SerializeField] 
    private Transform plantHolder;

    private List<GameObject> plantState;

    private GameObject currentPlantGrowObject;




    [SerializeField] 
    private float state1Time, state2Time;




    [SerializeField] 
    private bool spaceAvailable = true;



    

    void Start()
    {
        inventory = GetComponent<Inventory>();
    }


    IEnumerator Grow()
    {       
        plantState[0].SetActive(true);
        yield return new WaitForSeconds(state1Time);
        Destroy(plantState[0]);

        plantState[1].SetActive(true);
        yield return new WaitForSeconds(state2Time);
        Destroy(plantState[1]);

        plantState[2].SetActive(true);
    }

    private void CancelGrowProcess()
    {
        Destroy(currentPlantGrowObject);
        currentPlantGrowObject = null;
    }

    private bool CheckIfSeedIsStillThere()
    {
        if (inventory.items[0].GetItem() != null && inventory.items[0].GetItem().type == ItemClass.State.Plant)
        {
            RemoveSeedFromInventory();
            return true;
        }

        return false;
    }

    private void RemoveSeedFromInventory()
    {
        if(inventory.items[0].GetQuantity() > 1)
        {
            inventory.items[0].SubQuantity(1);
        }
        else
        {
            inventory.items[0].Clear();
        }
    }

    public void OnRemovePlant()
    {
        spaceAvailable = true;

        StartCoroutine(AfterPlantRemoveTimer());
    }

    IEnumerator AfterPlantRemoveTimer()
    {
        yield return new WaitForSeconds(2);

        CheckForSeed();
    }

    public void CheckForSeed()
    {
        if (!spaceAvailable) return;
        if (plantHolder.transform.childCount != 0) return;


        if (inventory.items[0].GetItem() != null && inventory.items[0].GetItem().type == ItemClass.State.Plant)
        {
            InstantiatePlant();
            StartGrowProcess();
        }
    }

    private void InstantiatePlant()
    {
        currentPlantGrowObject = Instantiate(inventory.items[0].GetItem().plantGrowObject, plantHolder);

        plantState = new List<GameObject>(currentPlantGrowObject.transform.childCount);
    }


    private void StartGrowProcess()
    {
        for (int i = 0; i < currentPlantGrowObject.transform.childCount; i++)
        {
            plantState.Add(currentPlantGrowObject.transform.GetChild(i).gameObject);
            plantState[i].SetActive(false);
        }

        spaceAvailable = false;

        RemoveSeedFromInventory();
        StartCoroutine(Grow());
    }
}
