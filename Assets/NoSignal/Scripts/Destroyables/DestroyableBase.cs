using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestroyableBase : MonoBehaviour
{
    [Header("Loot")]

    public List<ItemClass> loot;
    public Transform lootSpawnPoint;
    public int rewardAmountMin;
    public int rewardAmountMax;


    [Header("Hit Effect")]

    protected float scalePercentage = 0.1f;
    protected float scaleAmount;
    protected float scaleTime = 0.1f;


    [Header("Audio")]

    public AudioSource audioSource;
    public AudioClip[] hitSounds, destroySounds;



    protected HealthBase healthScript;

    [HideInInspector] public int levelRequired;
    [HideInInspector] public int requiredID;
    [HideInInspector] public bool isDestroyed = false;




    #region Unity Methods
    public virtual void Start()
    {
        scaleAmount = transform.localScale.y * scalePercentage;
        healthScript = GetComponent<HealthBase>();

        if (!audioSource)
        {
            if(!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogError("AudioSource missing!", gameObject);
            }
        }
    }

    #endregion Unity Methods




    #region On Hit

    public virtual void OnHit()
    {
        if (isDestroyed) return;

        if(audioSource) PlayRandomAudioOnHit();
        ScaleAnimation();
        SpawnLoot();
    }

    public virtual void OnDeath()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        if (audioSource) PlayRandomAudioOnDestroy();
        ScaleAnimation();
        SpawnDeathLoot();
        KillMe();
    }

    #endregion On Hit




    #region Hit Process
    public virtual void ScaleAnimation()
    {
        DOTween.Sequence().Append(
            gameObject.transform.DOScale(new Vector3(transform.localScale.x - scaleAmount, transform.localScale.y, transform.localScale.z - scaleAmount), scaleTime)
            ).Append(
            gameObject.transform.DOScale(new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z), scaleTime)
            );
    }

    public virtual void SpawnLoot()
    {
        Instantiate(loot[Random.Range(0, loot.Count)].dropReference, lootSpawnPoint.position, Quaternion.identity);
    }

    public virtual void SpawnDeathLoot()
    {
        int rewardAmount = Random.Range(rewardAmountMin, rewardAmountMax);
        for (int i = 0; i < rewardAmount; i++)
        {
            Instantiate(loot[Random.Range(0, loot.Count)].dropReference, lootSpawnPoint.position, Quaternion.identity);
        }
    }

    #endregion Hit Process




    #region Death
    public virtual void KillMe()
    {
        isDestroyed = true;

        int randomX = Random.Range(-2000, 2000);
        int randomZ = Random.Range(-2000, 2000);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 100;
        rb.AddForce(new Vector3(randomX, 0, randomZ));
        //Destroy(gameObject);
        StartCoroutine("DestroyObject");
    }

    public virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(10f);
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    #endregion Death




    #region Audio

    public virtual void PlayRandomAudioOnHit()
    {
        int random = Random.Range(0, hitSounds.Length);
        audioSource.PlayOneShot(hitSounds[random]);
    }

    public virtual void PlaySpecificAudioOnHit(int _index)
    {
        audioSource.PlayOneShot(hitSounds[_index]);
    }

    public virtual void PlayRandomAudioOnDestroy()
    {
        int random = Random.Range(0, destroySounds.Length);
        audioSource.PlayOneShot(destroySounds[random]);
    }

    public virtual void PlaySpecificAudioOnDestroy(int _index)
    {
        audioSource.PlayOneShot(destroySounds[_index]);
    }


    #endregion Audio
}
