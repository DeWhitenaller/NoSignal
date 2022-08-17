using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableBase : MonoBehaviour
{
    /// <summary>
    /// this class gets accessed by InteractSphere
    /// </summary>
    
    
    
    [Header("UI")]

    public GameObject interactUI;



    [Header("Usability")]

    [SerializeField]
    private bool multipleUse; //Levers, Doors, NPCs

    [SerializeField]
    private bool useOnce;

    public bool alreadyUsed = false;



    [Header("Audio")]

    public AudioSource audioSource;
    public AudioClip[] sounds;




    private float interactTimer;

    private bool isInteractable = true;




    public virtual void Start()
    {
        if (!audioSource)
        {
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogError("AudioSource missing!", gameObject);
            }
        }
    }



    #region Interaction Methods

    public virtual void OnSight()
    {
        ShowInteractIcon();
    }


    public virtual void OnInteract()
    {
        if (!isInteractable) return;

        if (multipleUse)
        {
            isInteractable = false;

            if (interactTimer != 0f)
            {
                StartCoroutine("Interact");
            }
            else
            {
                Interaction();
            }
        }
        else if (useOnce && !alreadyUsed)
        {
            alreadyUsed = true;
            Interaction();
        }
        else
        {
            Interaction();
        }
    }


    public virtual void Interaction()
    {
    }

    public virtual IEnumerator Interact()
    {
        Interaction();

        yield return new WaitForSeconds(interactTimer);

        isInteractable = !isInteractable;
    }

    public virtual void OnExit()
    {
        HideInteractIcon();
    }

    #endregion Interaction Methods




    #region UI

    public virtual void ShowInteractIcon()
    {
        interactUI.SetActive(true);
    }

    public virtual void HideInteractIcon()
    {
        interactUI.SetActive(false);
    }

    #endregion UI




    #region Audio

    public virtual void PlayRandomAudio()
    {
        int random = Random.Range(0, sounds.Length);
        audioSource.PlayOneShot(sounds[random]);
    }

    public virtual void PlaySpecificAudio(int _index)
    {
        audioSource.PlayOneShot(sounds[_index]);
    }

    #endregion Audio
}
