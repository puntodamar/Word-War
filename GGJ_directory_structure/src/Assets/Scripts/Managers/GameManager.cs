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
    public GameState currentGameState = GameState.MainScreen;
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
        //Invoke("Init", 2);
        Init();
        
    }

    private void Update()
    {
        if (!isTimerActive && currentGameState != GameState.MainScreen)
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
        //UIManager.Instance.ShowUI();
        playerWithActiveInputField  = Turn.PlayerA;
        currentTimerTime            = startingTimerTime;
        currentDifficulty           = (Difficulty)PlayerPrefs.GetInt("difficulty");

        

        if (currentDifficulty == Difficulty.Easy)
            difficultyPercentage      = .1f;
        if (currentDifficulty == Difficulty.Medium)
            difficultyPercentage    = .25f;
        if (currentDifficulty == Difficulty.Hard)
            difficultyPercentage      = .5f;


        Debug.Log(difficultyPercentage);
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
                        UIManager.Instance.ShowInfo(Turn.PlayerB + " launched an attack!", Color.red);
                        player2.Attack(player1);
                        currentTurn = Turn.PlayerB;
                    }
                    else
                    {
                        player1.Attack(player2);
                        UIManager.Instance.ShowInfo("Attack successfull!", Color.green);
                    }
                    currentTurn = Turn.PlayerB;
                    playerWithActiveInputField = currentTurn;
                }
            }

            else if (currentTurn == Turn.PlayerB)
            {

                if (player2.typingMode == GameState.TypingQuestion)
                {
                    if (currentTurn != playerWithActiveInputField)
                    {
                        UIManager.Instance.ShowInfo(Turn.PlayerA + " launched an attack!", Color.red);
                        player1.Attack(player2);
                        currentTurn = Turn.PlayerB;
                    }
                    else
                    {
                        player2.Attack(player1);
                        UIManager.Instance.ShowInfo("Attack successfull!", Color.green);
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
        Debug.Log(question);
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
        CancelInvoke("CountdownTimer");
        answer = answer.ToLower();
        Debug.Log("answer : " + answer + "---" + "correct : " + correctAnswer);

        if (currentTurn == Turn.PlayerA)
        {
            // jika salah
            if (answer != correctAnswer || answer == "")
            {
                UIManager.Instance.ShowInfo("Failed to dechiper intel !", Color.red);
                player1.Attack(player2);
            }

            // jika benar
            else if (answer == correctAnswer)
            {
                UIManager.Instance.ShowInfo("Intel dechipered!", Color.green);
                player2.Attack(player1);

            }

            player1.typingMode  = GameState.Answering;
            player2.typingMode  = GameState.TypingQuestion;
            currentTurn         = Turn.PlayerB;
        }

        else if(currentTurn == Turn.PlayerB)
        {
            // jika salah
            if (answer != correctAnswer || answer == "")
            {
                UIManager.Instance.ShowInfo("Failed to dechiper intel !", Color.red);
                player2.Attack(player1);
            }

            // jika benar
            else if (answer == correctAnswer)
            {
                UIManager.Instance.ShowInfo("Intel dechipered!", Color.green);
                player1.Attack(player2);
            }

            player1.typingMode = GameState.TypingQuestion;
            player2.typingMode = GameState.Answering;
            currentTurn = Turn.PlayerA;
        }

        if(currentGameState != GameState.GameOver)
        {
            currentGameState = GameState.TypingQuestion;

            StartCoroutine(PlayTurn());
            ResetTimer();
        }


    }

    string FormatQuestion(string question)
    {
        float removeWordCount= Mathf.CeilToInt(question.Length * difficultyPercentage);
        Debug.Log("remove : " + removeWordCount);
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

    public void CheckDeath()
    {
        if (player1.fortHealth > 0 && player2.fortHealth > 0) return;

        if(player1.fortHealth <= 0)
        {
            UIManager.Instance.ShowGameOverText(2);
            UIManager.Instance.ShowTurn("Player 2 WINS !");
        }
            
        else if(player2.fortHealth <= 0)
        {
            UIManager.Instance.ShowGameOverText(1);
            UIManager.Instance.ShowTurn("Player 1 WINS !");
        }
            

        
    }
}
