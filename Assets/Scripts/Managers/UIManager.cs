using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text showText;
    public Text timerText;

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
    }

    public void ShowTurn(string text)
    {
        StartCoroutine(ShowTurnCoroutine(text));
    }

    public void SetTime(int time, GameState state)
    {
        switch (state)
        {
            case GameState.TypingQuestion:
                timerText.text = "Time to Choose : " + time + " seconds";
                break;
            case GameState.Answering:
                timerText.text = "Time to Answer : " + time + " seconds";
                break;
        }

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
