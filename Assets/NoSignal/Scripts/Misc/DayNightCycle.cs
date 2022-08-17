using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{

    public Material skybox;

    [SerializeField] 
    private Light directionalLight;

    [SerializeField] 
    private Color dayColor, nightColor;




    [SerializeField] 
    private float dayTemperature, nightTemperature;

    [SerializeField] 
    private float transitionMaxTime, dayTime, nightTime;

    [SerializeField] 
    private float transitionTimer;




    private bool isFadingToDay, isFadingToNight;





    private void Start()
    {
        skybox.SetColor("black", Color.white);
        SetDay();
        StartCoroutine(DayTime());    
    }

    private void Update()
    {
        if(isFadingToDay && transitionTimer < transitionMaxTime)
        {
            transitionTimer += Time.deltaTime;

            LerpLightSettingsToDay();

            if (transitionTimer >= transitionMaxTime)
            {
                TransitionToDayEnd();
                StartCoroutine(DayTime());
            }

        }
        else if (isFadingToNight && transitionTimer < transitionMaxTime)
        {
            transitionTimer += Time.deltaTime;

            LerpLightSettingsToNight();

            if (transitionTimer >= transitionMaxTime)
            {
                TransitionToNightEnd();
                StartCoroutine(NightTime());
            }
        }
    }



    private void SetDay()
    {
        directionalLight.colorTemperature = dayTemperature;
        directionalLight.color = dayColor;
    }




    #region Wait For Day/Night Seconds

    IEnumerator DayTime()
    {
        yield return new WaitForSeconds(dayTime);

        isFadingToNight = true;
    }

    IEnumerator NightTime()
    {
        yield return new WaitForSeconds(nightTime);

        isFadingToDay = true;
    }

    #endregion Wait For Day/Night Seconds





    #region Transitions

    public void TransitionToDayEnd()
    {
        isFadingToDay = false;
        transitionTimer = 0f;
    }

    public void TransitionToNightEnd()
    {
        isFadingToNight = false;
        transitionTimer = 0f;
    }

    private void LerpLightSettingsToDay()
    {
        directionalLight.colorTemperature = Mathf.Lerp(nightTemperature, dayTemperature, transitionTimer / transitionMaxTime);
        directionalLight.color = Color.Lerp(nightColor, dayColor, transitionTimer / transitionMaxTime);
    }

    private void LerpLightSettingsToNight()
    {
        directionalLight.colorTemperature = Mathf.Lerp(dayTemperature, nightTemperature, transitionTimer / transitionMaxTime);
        directionalLight.color = Color.Lerp(dayColor, nightColor, transitionTimer / transitionMaxTime);
    }

    #endregion Transitions
}
