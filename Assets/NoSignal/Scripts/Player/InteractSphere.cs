using UnityEngine;


public class InteractSphere : MonoBehaviour
{
    /// <summary>
    /// creates an OverlapSphere to detect interactable objects
    /// if detecting an interactable object, this class executes it's OnSight Method
    /// if pressing E while OnSight gets called, this class executes it's OnInteract Method
    /// if the ray detects a new object or no object at all, this class executes the old object's HideInteractIcon Method
    /// 
    /// this class only executes Methods of the nearest object
    /// </summary>
    
    
    
    private InteractableBase nearestObjectScript;



    [SerializeField] private LayerMask objectMask;
    
    private Collider[] sphereHits;



    [SerializeField] private GameObject nearestObject;

    [SerializeField] private GameObject oldNearestObject;



    [SerializeField] private float sphereRadius;
    
    private float distance;
    
    private float nearestDistance;



    public bool sphereActive;





    void Start()
    {
        nearestDistance = 100f;
    }


    void Update()
    {
        if (!sphereActive) return;

        // OverlapSphere registers all interactables in a radius of the player
        sphereHits = Physics.OverlapSphere(gameObject.transform.position, sphereRadius, objectMask);

        // if there is any Object near the player
        if (sphereHits.Length != 0)
        {
            // find the nearest Object inside of the OverlapSphere-Array
            GetNearestObject();

            // check if the nearest Object is the same as 1 frame ago
            CompareNearestObject();

            // if the nearest Object is not already used -> player is able to interact with it
            OnInteractableHit();
        }
        // if the nearest Object isn't in the OverlapSphere anymore -> deactivate it and clear variables
        else if (nearestObject != null || oldNearestObject != null)
        {
            OnInteractableExitsSphere();
        }
    }


    private void OnInteractableExitsSphere()
    {
        nearestObjectScript.OnExit();
        nearestObjectScript = null;
        nearestObject = null;
        oldNearestObject = null;
    }


    private void OnInteractableHit()
    {
        if (!nearestObjectScript.alreadyUsed)
        {
            nearestObjectScript.OnSight();

            if (Input.GetKeyDown(KeyCode.E))
            {
                nearestObjectScript.OnInteract();
            }
        }
    }


    public void GetNearestObject()
    {
        foreach (var item in sphereHits)
        {
            distance = Vector3.Distance(gameObject.transform.position, item.gameObject.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestObject = item.gameObject;

            }
        }

        // get the InteractScript of the nearest Object and reset the distance
        nearestObjectScript = nearestObject.GetComponent<InteractableBase>();
        nearestDistance = 100f;
    }


    public void CompareNearestObject()
    {
        // if oldNearestObject is empty -> let it be the nearest Object
        if (oldNearestObject == null)
        {
            oldNearestObject = nearestObject;
        }
        else
        {
            // if the currently nearest Object is not the same as the one in the array (if there is a new nearest Object)
            if (oldNearestObject != nearestObject)
            {
                // deactivate the old one and clear the oldNearestObject variable
                oldNearestObject.GetComponent<InteractableBase>().OnExit();
                oldNearestObject = nearestObject;
            }
        }
    }

}