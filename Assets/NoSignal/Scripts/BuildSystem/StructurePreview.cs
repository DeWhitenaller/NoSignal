using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePreview : MonoBehaviour
{
    [SerializeField] 
    private List<Collider> collisions;

    private MeshRenderer[] meshRenderers;

    private Material greenMaterial, redMaterial;




    public LayerMask getsInterruptedBy;

    public bool isPlacable, snapped;




    #region Unity Methods

    private void Awake()
    {
        Initialization();
    }

    void Start()
    {
        CheckIfPlacable();
    }


    #endregion Unity Methods


    private void Initialization()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        greenMaterial = Resources.Load<Material>("Materials/StructurePreviewGreen");
        redMaterial = Resources.Load<Material>("Materials/StructurePreviewRed");
        collisions = new List<Collider>();
    }


    public void CheckIfPlacable()
    {
        if(collisions.Count == 0)
        {
            foreach (MeshRenderer mesh in meshRenderers)
            {
                mesh.material = greenMaterial;
            }

            isPlacable = true;
        }
        else
        {
            foreach (MeshRenderer mesh in meshRenderers)
            {
                mesh.material = redMaterial;
            }

            isPlacable = false;
        }
    }


    public void RemoveParentFromCollisionList()
    {
        collisions.Remove(transform.root.gameObject.GetComponent<Collider>());
    }


    #region OnTrigger


    private void OnTriggerEnter(Collider other)
    {
        if ((getsInterruptedBy.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            collisions.Add(other);
        }

        if (snapped)
        {
            RemoveParentFromCollisionList();
        }

        CheckIfPlacable();
    }

    private void OnTriggerStay(Collider other)
    {
        // the reason why this needs to be in the stay method as well:
        // if a structure is snapped, it collides with the object that holds the snappoint
        // that means that OnTriggerEnter gets called and it removes the colliding object (because otherwise this object couldn't be placed on snappoints)
        // anyway if this objects gets unsnapped without leaving the colliding object (if you move it further into the colliding object for example)
        // then the colliding object stays out of the collisions list and you can place this object inside of the colliding object

        if ((getsInterruptedBy.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            if (!snapped)
            {
                bool _isInList = false;

                foreach (Collider col in collisions)
                {
                    if(col == other)
                    {
                        _isInList = true;
                        break;
                    }
                }

                if (!_isInList)
                {
                    collisions.Add(other);
                    CheckIfPlacable();
                }
            }
            else
            {
                foreach (MeshRenderer mesh in meshRenderers)
                {
                    mesh.material = greenMaterial;
                }

                isPlacable = true;
            }
        }


    }

    private void OnTriggerExit(Collider other)
    {
        collisions.Remove(other);
        CheckIfPlacable();
    }

    #endregion OnTrigger
}
