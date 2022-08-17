using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowObject : MonoBehaviour
{
    public List<MeshRenderer> plantGrowthMesh;
    public float growthTime = 8;
    public float refreshRate = 0.05f;
    [Range(0,1)]
    public float minGrowth = 0.2f;
    [Range(0, 1)]
    public float maxGrowth = 1.0f;

    private List<Material> growthMaterial = new List<Material>();
    private bool fullyGrown;
    void Start()
    {
        for (int i = 0; i < plantGrowthMesh.Count; i++)
        {
            for (int j = 0; j < plantGrowthMesh[i].materials.Length; j++)
            {
                plantGrowthMesh[i].materials[j].SetFloat("Grow_", minGrowth);
                growthMaterial.Add(plantGrowthMesh[i].materials[j]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < growthMaterial.Count; i++)
            {
                StartCoroutine(GrowPlant(growthMaterial[i]));
            }
        }
    }

    IEnumerator GrowPlant(Material mat)
    {
        float growValue = mat.GetFloat("Grow_");
        if (!fullyGrown)
        {
            while (growValue < maxGrowth)
            {
                growValue *= 1/(growthTime/refreshRate);
                mat.SetFloat("Grow_", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            while (growValue > minGrowth)
            {
                growValue -= 1 / (growthTime / refreshRate);
                mat.SetFloat("Grow_", growValue);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        if (growValue >= maxGrowth)
        {
            fullyGrown = true;
        }
        else
        {
            fullyGrown = false;
        }
    }
}
