using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;

public enum GameState { MainScreen, Options, Answering, TypingQuestion, DisableActions, GameOver, Paused }
public enum Difficulty {  Easy, Medium, Hard }
public enum Turn { PlayerA = 1, PlayerB }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Turn currentTurn;
    public Turn playerWithActiveInputField;
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
    private GameState lastPlayingState = GameState.Answering;
    
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
        playerWithActiveInputField = Turn.PlayerA;
        currentTimerTime = startingTimerTime;

        if (currentDifficulty == Difficulty.Easy)
            difficultyPercentage      = .1f;
        if (currentDifficulty == Difficulty.Medium)
            difficultyPercentage    = .25f;
        if (currentDifficulty == Difficulty.Hard)
            difficultyPercentage      = .5f;

        currentGameState = GameState.TypingQuestion;
        SwapTypingMode();
        
        //UpdateLastPlayingState();

        StartCoroutine(PlayTurn());
    }

    void SwapTypingMode()
    {
        switch (currentTurn)
        {
            case Turn.PlayerA:
                player1.typingMode = GameState.TypingQuestion;
                player2.typingMode = GameState.Answering;
                break;
            case Turn.PlayerB:
                player1.typingMode = GameState.Answering;
                player2.typingMode = GameState.TypingQuestion;
                break;
        }
    }

    void CountdownTimer()
    {
        if (currentTimerTime < 0) return;        

        // jika soal / jawaban kosong
        if(currentTimerTime == 0)
        {
            CancelInvoke("CountdownTimer");
            if(currentTurn == Turn.PlayerA)
            {
                if(player1.typingMode == GameState.TypingQuestion)
                {
                    if(currentTurn != playerWithActiveInputField)
                    {
                        player2.Attack(player1);
                        currentTurn = Turn.PlayerB;
                    }
                    else
                    {
                        player1.Attack(player2);
                    }
                    currentTurn = Turn.PlayerB;
                    playerWithActiveInputField = currentTurn;
                }
            }

            else if (currentTurn == Turn.PlayerB)
            {
                //if (player2.typingMode == GameState.TypingQuestion)
                //{
                //    player1.Attack(player2);
                //    currentTurn = Turn.PlayerA;
                //}

                if (player2.typingMode == GameState.TypingQuestion)
                {
                    if (currentTurn != playerWithActiveInputField)
                    {
                        player1.Attack(player2);
                        currentTurn = Turn.PlayerB;
                    }
                    else
                    {
                        player2.Attack(player1);
                    }
                    currentTurn = Turn.PlayerA;
                    playerWithActiveInputField = currentTurn;
                }
            }

            if (currentGameState == GameState.TypingQuestion)
                currentGameState = GameState.TypingQuestion;

            else if (currentGameState == GameState.Answering)
                currentGameState = GameState.TypingQuestion;

            SwapTypingMode();

            StartCoroutine(PlayTurn());
            ResetTimer();
        }

        // UPDATE UI
        if(currentGameState == GameState.TypingQuestion || currentGameState == GameState.Answering)
        {
            currentTimerTime--;
            UIManager.Instance.SetTime(currentTimerTime, currentGameState);
        }
    }

    IEnumerator PlayTurn()
    {
        shownWord       = "";
        correctAnswer   = "";
        GameState temp  = currentGameState;
        currentGameState = GameState.DisableActions;

        InputManager.Instance.HideInputField();      
       
        UIManager.Instance.ShowTurn("Player " + (int)currentTurn + "'s turn.");

        yield return new WaitForSecondsRealtime(InputManager.Instance.showInfoTimer);
        
        InputManager.Instance.ShowPlayerInputField(currentTurn, player1.typingMode, player2.typingMode);

        currentGameState = temp;
        //ResetTimer();
    }

    void UpdateLastPlayingState()
    {
        switch (lastPlayingState)
        {
            case GameState.TypingQuestion:
                currentGameState = GameState.Answering;
                break;
            case GameState.Answering:
                currentGameState = GameState.TypingQuestion;
                break;
        }

        lastPlayingState = currentGameState;
    }

    void ResetTimer()
    {
        currentTimerTime = startingTimerTime;
        InvokeRepeating("CountdownTimer", 0, 1);
    }

    public void SubmitQuestion(string question)
    {
        correctAnswer = question.ToLower();
        CancelInvoke("CountdownTimer");
        InputManager.Instance.HideInputField();
        question    = FormatQuestion(correctAnswer);
        shownWord   = question;
        UIManager.Instance.ShowFormattedQuestion(question);

        currentGameState = GameState.Answering;
        SwapTypingMode();
        Turn showInput = (currentTurn == Turn.PlayerA) ? Turn.PlayerB : Turn.PlayerA;
        InputManager.Instance.ShowPlayerInputField(showInput, player1.typingMode, player2.typingMode);
        ResetTimer();
    }

    public void CheckAnswer(string answer = "", bool resetTimer = false)
    {
        Debug.Log(answer);
        CancelInvoke("CountdownTimer");
        answer = answer.ToLower();
        Debug.Log("answer : " + answer + "---" + "correct : " + correctAnswer);

        if (currentTurn == Turn.PlayerA)
        {
            Debug.Log("a");
            // jika salah
            if (answer != correctAnswer || answer == "")
            {
                player1.Attack(player2);
            }

            // jika benar
            else if (answer == correctAnswer)
            {
                player2.Attack(player1);

            }

            player1.typingMode  = GameState.Answering;
            player2.typingMode  = GameState.TypingQuestion;
            currentTurn         = Turn.PlayerB;
        }

        else if(currentTurn == Turn.PlayerB)
        {
            Debug.Log("b");
            // jika salah
            if (answer != correctAnswer || answer == "")
            {
                player2.Attack(player1);
            }

            // jika benar
            else if (answer == correctAnswer)
            {
                player1.Attack(player2);
            }

            player1.typingMode = GameState.TypingQuestion;
            player2.typingMode = GameState.Answering;
            currentTurn = Turn.PlayerA;
        }
        currentGameState = GameState.TypingQuestion;
        //currentTurn = (currentTurn == Turn.PlayerA) ? Turn.PlayerB : Turn.PlayerB;
        //SwapTypingMode();


        StartCoroutine(PlayTurn());
        ResetTimer();

        //if (resetTimer)
        //{
        //    //ResetTimer();
        //    //StartCoroutine(PlayTurn());
        //}

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
