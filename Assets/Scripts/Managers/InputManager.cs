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

    public void ShowPlayerInputField(Turn turn, GameState player1TypingMode, GameState player2TypingMode)
    {

        if (turn == Turn.PlayerA)
        {
            inputWordPA.SetActive(true);
            InputField inputField = inputWordPA.GetComponent<InputField>();
            inputField.ActivateInputField();
            if (player1TypingMode == GameState.TypingQuestion)
                inputField.placeholder.GetComponent<Text>().text = "Enter a word...";

            else if (player1TypingMode == GameState.Answering)
                inputField.placeholder.GetComponent<Text>().text = "Enter your answer...";


        }
        else if (turn == Turn.PlayerB)
        {
            inputWordPB.SetActive(true);
            InputField inputField = inputWordPB.GetComponent<InputField>();
            inputField.ActivateInputField();
            if (player2TypingMode == GameState.TypingQuestion)
                inputField.placeholder.GetComponent<Text>().text = "Enter a word...";

            else if (player2TypingMode == GameState.Answering)
                inputField.placeholder.GetComponent<Text>().text = "Enter your answer...";
        }

        GameObject[] inputs = GameObject.FindGameObjectsWithTag("PlayerInputField");
        //for (int i = 0; i < inputs.Length; i++)
        //{
        //    if(inputs[i].act)
        //    {
        //        inputs[i].GetComponent<InputField>().Select();
        //        break;
        //    }
        //}
    }
}
