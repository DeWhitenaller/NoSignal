using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] 
    private InventoryManager inventoryManager;

    public StructureClass currentStructure;

    [SerializeField] 
    private StructurePreview currentPreviewScript;

    [SerializeField] 
    private Camera mainCam;




    public GameObject currentStructureObject;

    [SerializeField] 
    private GameObject snapPointParent, snapPoint;




    [SerializeField] 
    private float rayDistance;

    private Vector3 structurePlacePosition;

    [SerializeField] 
    private LayerMask solidLayer, snapPointLayer;




    private bool structurePlacable;
    public bool buildMode;



    

    #region Unity Methods

    void Update()
    {
        if (!buildMode) return;

        if (currentStructureObject != null)
        {
            BuildRayCast();
            CheckMouseInput();
        }
    }


    private void OnDisable()
    {
        DeactivateCurrentStructure();
    }

    #endregion Unity Methods



    #region RayCast
    private void BuildRayCast()
    {
        SolidObjectDetectRay(out RaycastHit[] _solidObjectHits);
        SnapPointDetectRay(out RaycastHit _snapPointHit);

        if (_solidObjectHits.Length != 0 || _snapPointHit.collider)
        {
            structurePlacable = true;

            if (!currentStructureObject.activeInHierarchy)
            {
                currentStructureObject.SetActive(true);
            }

            if (_snapPointHit.collider)
            {
                OnSnapPointDetected(_snapPointHit.collider);
            }
            else if(_solidObjectHits[0].collider)
            {
                OnSolidObjectDetected(_solidObjectHits);
            }
        }
        else
        {
            // set current structure inactive if not detecting ground layer or snap point
            if (currentStructureObject.activeInHierarchy)
            {
                currentStructureObject.SetActive(false);
            }
            structurePlacable = false;
            structurePlacePosition = Vector3.zero;
        }
    }

    private void SnapPointDetectRay(out RaycastHit _snapPointHit)
    {
        Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit rayHit, rayDistance, snapPointLayer);
        _snapPointHit = rayHit;
    }

    private void SolidObjectDetectRay(out RaycastHit[] _solidObjectHits)
    {
        RaycastHit[] hits = Physics.RaycastAll(mainCam.transform.position, mainCam.transform.forward, rayDistance, solidLayer);
        _solidObjectHits = hits;
    }


    #endregion RayCast



    #region OnRaycastHit

    private void OnSnapPointDetected(Collider _snapPointCollider)
    {
        snapPoint = _snapPointCollider.gameObject;
        currentPreviewScript.snapped = true;
        currentStructureObject.transform.position = _snapPointCollider.transform.position;
        currentStructureObject.transform.rotation = _snapPointCollider.transform.rotation;
        structurePlacePosition = _snapPointCollider.transform.position;
        currentStructureObject.transform.parent = _snapPointCollider.transform;
    }

    private void OnSolidObjectDetected(RaycastHit[] _solidObjectHits)
    {
        snapPoint = null;

        if (currentPreviewScript.snapped)
        {
            currentPreviewScript.snapped = false;
            currentPreviewScript.CheckIfPlacable();
        }

        if (_solidObjectHits[0].collider.gameObject == currentStructureObject)
        {
            if (_solidObjectHits.Length > 1)
            {
                currentStructureObject.transform.position = _solidObjectHits[1].point;
            }
        }
        else
        {
            currentStructureObject.transform.position = _solidObjectHits[0].point;
        }
        currentPreviewScript.snapped = false;
        structurePlacePosition = _solidObjectHits[0].point;
        currentStructureObject.transform.parent = null;
    }



    #endregion OnRaycastHit



    private void CheckMouseInput()
    {
        if (currentPreviewScript.isPlacable && currentStructureObject.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlaceStructure();
                inventoryManager.UseEquippedItem();
            }
        }
    }

    private void PlaceStructure()
    {
        if (currentPreviewScript.snapped)
        {
            if (currentPreviewScript.transform.parent.childCount != 1)
            {
                // if there is already a child existing, it means that there is already a placed Structure, so return
                return;
            }
            Debug.Log(currentPreviewScript.transform.parent.childCount, currentPreviewScript.gameObject);
            Instantiate(currentStructure.structureObjectReference, structurePlacePosition, snapPoint.transform.rotation, snapPoint.transform);

        }
        else
        {
            Instantiate(currentStructure.structureObjectReference, structurePlacePosition, currentStructureObject.transform.rotation);
        }
    }




    #region Activate / Refresh / Clear

    public void ActivateCurrentStructure(StructureClass _structure)
    {
        currentStructure = _structure;
        currentStructureObject = Instantiate(currentStructure.structureObjectPreviewReference);
        currentPreviewScript = currentStructureObject.GetComponentInChildren<StructurePreview>();
        currentPreviewScript.getsInterruptedBy = _structure.previewGetsInterruptedBy;
        snapPointLayer = _structure.snapPointLayer;
        solidLayer = _structure.solidLayer;
    }



    public void RefreshCurrentStructure(StructureClass _structure)
    {
        if (currentStructureObject != null)
            Destroy(currentStructureObject);

        currentStructure = _structure;
        currentStructureObject = Instantiate(currentStructure.structureObjectPreviewReference);
        currentPreviewScript = currentStructureObject.GetComponent<StructurePreview>();
        currentPreviewScript.getsInterruptedBy = _structure.previewGetsInterruptedBy;
        snapPointLayer = _structure.snapPointLayer;
        solidLayer = _structure.solidLayer;
    }



    public void ClearCurrentStructure()
    {
        Destroy(currentStructureObject);
        currentStructure = null;
        currentStructureObject = null;
        currentPreviewScript = null;
    }



    public void DeactivateCurrentStructure()
    {
        if (currentStructureObject == null) return;

        Destroy(currentStructureObject);
        currentStructureObject = null;
        currentStructure = null;
    }

    #endregion Activate / Refresh / Clear

}
