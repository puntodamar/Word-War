using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text showText;
    public Text timerText;
    public Image timerSlider;

    private float time;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);

        showText.text = "";

        Init();
    }

    void Init()
    {
        timerText.text = "";
        timerSlider.color = Color.yellow;
    }

    public void ShowTurn(string text)
    {
        timerSlider.fillAmount = 1;
        StartCoroutine(ShowTurnCoroutine(text));
    }

    public void SetTime(int time, GameState state)
    {
        this.time = time;
        switch (state)
        {
            case GameState.TypingQuestion:
                timerText.text = time+"s";
                break;
            case GameState.Answering:
                timerText.text = time + "s";
                break;
        }

        float amount = (float)time;
        //timerSlider.fillAmount = amount * .1f * 2;
        timerSlider.fillAmount = amount * .1f;
    }

    IEnumerator ShowTurnCoroutine(string text)
    {
        showText.text = text;
        yield return new WaitForSecondsRealtime(InputManager.Instance.showInfoTimer + .5f);
        showText.text = "";
    }

    public void ShowFormattedQuestion(string question)
    {
        showText.text = question;
    }



}
