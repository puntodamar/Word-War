using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Text showText;
    public Text timerText;
    public Text infoText;
    public Image timerSlider;

    public GameObject gameOverDialog;
    public GameObject setDifficultyDialog;

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

    public void ShowUI()
    {
        Debug.Log("called");
        showText.gameObject.SetActive(true);
        //infoText.gameObject.SetActive(true);
        timerSlider.gameObject.transform.parent.gameObject.SetActive(true);
    }

    public void ShowInfo(string info, Color color)
    {
        StartCoroutine(ShowTextInfo(info, color));
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

    IEnumerator ShowTextInfo(string text, Color color)
    {
        infoText.text = text;
        infoText.color = color;
        yield return new WaitForSeconds(1f);
        infoText.text = "";
    }

    public void ShowFormattedQuestion(string question)
    {
        showText.text = question;
    }

    public void ShowGameOverText()
    {
        gameOverDialog.SetActive(true);
    }





}
