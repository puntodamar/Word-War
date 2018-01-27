using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public enum GameState { MainScreen, Options, Answering, TypingQuestion, DisableActions, GameOver, Paused }
public enum Difficulty {  Easy, Medium, Hard }
public enum Turn { PlayerA = 1, PlayerB }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Turn currentTurn;
    public GameState currentGameState;
    public Difficulty currentDifficulty = Difficulty.Medium;
    public Fort player1;
    public Fort player2;
    public string shownWord;
    public int startingTimerTime = 6;
    public int currentTimerTime;

    private float difficultyPercentage;
    private string correctAnswer;
    private bool isTimerActive;
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);

        
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!isTimerActive)
        {
            if(currentGameState == GameState.TypingQuestion || currentGameState == GameState.Answering)
            {
                isTimerActive = true;
                InvokeRepeating("CountdownTimer", 0, 1);
            }

        }
        
    }

    void Init()
    {
        currentTimerTime = startingTimerTime;

        if (currentDifficulty == Difficulty.Easy)
            difficultyPercentage      = .1f;
        if (currentDifficulty == Difficulty.Medium)
            difficultyPercentage    = .25f;
        if (currentDifficulty == Difficulty.Hard)
            difficultyPercentage      = .5f;

        StartCoroutine(PlayTurn());
    }

    void CountdownTimer()
    {
        if (currentTimerTime < 0) return;
        
        if(currentTimerTime == 0)
        {
            CancelInvoke("CountdownTimer");

            if(currentGameState == GameState.TypingQuestion)
            {
                if (shownWord == "" || shownWord.Length > 2)
                {
                    if (currentTurn == Turn.PlayerA)
                        player2.Attack(player1);
                    else player1.Attack(player2);
                }
            }

            else if(currentGameState == GameState.Answering)
            {
                CheckAnswer();
            }
            
            currentTurn         = (currentTurn == Turn.PlayerA) ? Turn.PlayerB : Turn.PlayerA;
            StartCoroutine(PlayTurn());
            ResetTimer();
        }

        if(currentGameState == GameState.TypingQuestion || currentGameState == GameState.Answering)
        {
            currentTimerTime--;
            UIManager.Instance.SetTime(currentTimerTime, currentGameState);
        }


    }

    IEnumerator PlayTurn()
    {
        shownWord = "";
        correctAnswer = "";
        InputManager.Instance.HideInputField();
        currentGameState = GameState.DisableActions;
        UIManager.Instance.ShowTurn("Player " + (int)currentTurn + "'s turn.");

        yield return new WaitForSecondsRealtime(InputManager.Instance.showInfoTimer);
        
        InputManager.Instance.ShowPlayerInputField(currentTurn);
        currentGameState = GameState.TypingQuestion;

        //switch (lastPlayingState)
        //{
        //    case GameState.TypingQuestion:
        //        currentGameState = GameState.Answering;
        //        break;
        //    case GameState.Answering:
        //        currentGameState = GameState.TypingQuestion;
        //        break;
        //}

        //lastPlayingState = currentGameState;
        //ResetTimer();
    }

    void ResetTimer()
    {
        currentTimerTime = startingTimerTime;
        InvokeRepeating("CountdownTimer", 0, 1);
    }

    public void SubmitQuestion(string question)
    {
        correctAnswer = question;
        CancelInvoke("CountdownTimer");
        currentTimerTime = 5;
        InputManager.Instance.HideInputField();
        question    = FormatQuestion(question);
        shownWord   = question;
        UIManager.Instance.ShowFormattedQuestion(question);

        currentGameState = GameState.Answering;
        
        Turn showInput = (currentTurn == Turn.PlayerA) ? Turn.PlayerB : Turn.PlayerA;
        InputManager.Instance.ShowPlayerInputField(showInput);
        ResetTimer();
    }

    public void CheckAnswer(string answer = "")
    {
        if(answer != correctAnswer && answer != "")
        {
            if (currentTurn == Turn.PlayerA)
                player1.Attack(player2);
            else
                player2.Attack(player1);
        }
        else
        {
            if (currentTurn == Turn.PlayerA)
                player2.Attack(player1);
            else
                player1.Attack(player2);
        }

        currentGameState = GameState.TypingQuestion;

        Turn showInput = (currentTurn == Turn.PlayerA) ? Turn.PlayerB : Turn.PlayerA;
        StartCoroutine(PlayTurn());
        ResetTimer();
    }

    string FormatQuestion(string question)
    {
        float removeWordCount           = Mathf.RoundToInt(question.Length * difficultyPercentage);
        int currentHiddenCharacterCount = 0;
        System.Text.StringBuilder sb    = new System.Text.StringBuilder(question);

        while (currentHiddenCharacterCount < removeWordCount)
        {
            int randomPick = Random.Range(0, question.Length-1);
            if(randomPick > .5f && sb[randomPick].ToString() != "*")
            {
                sb[randomPick] = '*';
                currentHiddenCharacterCount++;
            }                
        }

        return sb.ToString();
    }
}
