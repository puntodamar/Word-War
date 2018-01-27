using UnityEngine;
using UnityEngine.UI;
public class Submit : MonoBehaviour
{
    public InputField inputAnswer;

    public void SubmitInput()
    {
        if(inputAnswer.text.Length >= 3)
        {
            if(GameManager.Instance.currentGameState == GameState.TypingQuestion)
                GameManager.Instance.SubmitQuestion(inputAnswer.text);
            else if(GameManager.Instance.currentGameState == GameState.Answering)
                GameManager.Instance.CheckAnswer(inputAnswer.text);
        }
            
    }
}
