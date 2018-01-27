using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public GameObject inputWordPA;
    public GameObject inputWordPB;
    public float showInfoTimer = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    public void HideInputField()
    {
        inputWordPA.SetActive(false);
        inputWordPB.SetActive(false);
        inputWordPA.GetComponent<InputField>().text = "";
        inputWordPB.GetComponent<InputField>().text = "";
    }

    public void ShowPlayerInputField(Turn turn)
    {

        if (turn == Turn.PlayerA)
        {
            inputWordPA.SetActive(true);
            InputField inputField = inputWordPA.GetComponent<InputField>();
            inputField.Select();
            if (GameManager.Instance.currentGameState == GameState.TypingQuestion)
                inputField.placeholder.GetComponent<Text>().text = "Enter a word...";
            else if (GameManager.Instance.currentGameState == GameState.Answering) inputField.placeholder.GetComponent<Text>().text = "Enter your answer...";


        }
        else if (turn == Turn.PlayerB)
        {
            inputWordPB.SetActive(true);
            InputField inputField = inputWordPB.GetComponent<InputField>();
            inputWordPB.GetComponent<InputField>().Select();

            if (GameManager.Instance.currentGameState == GameState.TypingQuestion)
                inputField.placeholder.GetComponent<Text>().text = "Enter a word...";
            else if (GameManager.Instance.currentGameState == GameState.Answering) inputField.placeholder.GetComponent<Text>().text = "Enter your answer...";
        }
    }
}
