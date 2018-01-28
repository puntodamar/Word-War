using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Splash : MonoBehaviour {

    public Image fader;
    public Sprite[] splash;

    public float fadeTime = 3f;
    public float timeToDisplay = .1f;
    public float distanceBetweenSplash = .3f;
    
    Color alpha;
    int currentSplashIndex = 0;

    private void Start()
    {
        Invoke("ChangeSplashImage", 2);
        Invoke("GoToMain", 4);
    }


    void ChangeSplashImage()
    {
        fader.overrideSprite = splash[currentSplashIndex];
        currentSplashIndex++;
    }

    void GoToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
