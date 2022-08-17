using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void SceneLoader(string sceneName)
        {
            //Laden der Szene über Szenen Name
            SceneManager.LoadScene(sceneName);
        }
}
