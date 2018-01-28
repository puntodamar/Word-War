using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDifficultyDialog : MonoBehaviour
{
    public GameObject setDifficultyDialogBox;

    private void Start()
    {
        SetButtonColor();
    }

    void SetButtonColor()
    {
        int difficulty = PlayerPrefs.GetInt("difficulty");

        Button[] buttons = setDifficultyDialogBox.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if(difficulty == i)
                buttons[i].GetComponent<Image>().color = Color.gray;
            else
                buttons[i].GetComponent<Image>().color = Color.white;
        }
    }


    public void ChangeDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt("difficulty", (int)difficulty);
        ToggleSetDifficultyDialog();
        
    }

    public void ToggleSetDifficultyDialog()
    {
        SetButtonColor();
        setDifficultyDialogBox.SetActive(!setDifficultyDialogBox.activeInHierarchy);

    }
}
