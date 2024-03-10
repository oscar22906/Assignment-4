using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using Strobotnik.Klattersynth;

public class Board : MonoBehaviour
{

    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[] 
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z,
    };
    
    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    [Header("UI")]
    public TextMeshProUGUI wordDisplay;
    public DialogueController dialogueController;

    [Header("Other Stuff")]

    public UIEvent uIEvent;
    public EnemyHealth enemyHealth;
    public PlayerHealth playerHealth;
    public Speech speech;

    public string[] spellAlternatives = {"New word. It's ", "Spell ", "Next, spell ", "Now spell "};

    [Tooltip("Test tts voice")]
    public string TextSpeech;

    public string playerName;
    public bool submitted;
    public float clearTime;


    [Tooltip("Uses alternative marking algorithm that is simple but incomplete")]
    public bool simpleSubmit;

    [Tooltip("Word challenges continue after enemy is killed")]
    public bool testingMode = false;

    public Row[] rows;

    private string incorrectGuess;
    private string displayWord;

    private string[] solutions;
    private string[] failPhrase;
    private string[] successPhrase;
    private string word;

    private int correctGuesses;
    private int incorrectGuesses;
    public int rowIndex;
    private int columnIndex;

    private int currentLevel;


    private bool introScene = false;
    private IntroDialogue introDialogue;
    private Persistent persistent;
    private LoadData loadData;


    private void Awake()
    {
        GameObject persistentObj = GameObject.FindGameObjectWithTag("Persistent");
        if (persistentObj != null)
        {
            persistent = persistentObj.GetComponent<Persistent>();
            playerName = persistent.playerName;
            currentLevel = persistent.currentLevel;
        }
        else
        {
            playerName = "jerry";
            currentLevel = 1;
        }
        GameObject introObj = GameObject.FindGameObjectWithTag("Intro");
        if (introObj != null)
        {
            introDialogue = introObj.GetComponent<IntroDialogue>();
        }

        rows = GetComponentsInChildren<Row>();
        GameObject game = GameObject.FindGameObjectWithTag("Game");
        loadData = game.GetComponent<LoadData>();
        uIEvent.SetInvisible();

        if (introDialogue != null)
        {
            introScene = true;
            speech = null;
        }
    }

    private void Start() 
    {
        LoadData();
        if (!introScene)
        {
            NewGame();
        }
        else
        {
            return;
        }
    }

    public void NewGame()
    {
        ClearBoard();
        SetRandomWord();

        enabled = true;
    }

    public void TryAgain()
    {
        ClearBoard();
        enabled = true;
    }

    private void LoadData()
    {
        solutions = loadData.LoadSolutions(currentLevel);
        
        failPhrase = loadData.LoadFailPhrase(currentLevel);
        
        successPhrase = loadData.LoadSuccessPhrase(currentLevel);
    }

    private string RandomPhrase(string[] phraseBank)
    {
        // get random phrase from the chosen array phrase bank
        string phrase;
        phrase = phraseBank[Random.Range(0, phraseBank.Length)];
        phrase = phrase.Trim();
        return phrase;
    }

    private void SetRandomWord() 
    {
        word = solutions[Random.Range(0, solutions.Length)];
        word = word.ToLower().Trim();
        // speak challenge word
        Debug.Log("The word is " + word);
        speech.schedule(spellAlternatives[Random.Range(0, spellAlternatives.Length)] + word, false);
        
    }

    private void Update()
    {
        Row currentRow = rows[rowIndex];
        if (introScene)
        {
            if (introDialogue.currentIndex2 >= introDialogue.texts2.Length)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SubmitRow(currentRow);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // test tts
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);

            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);

            // logic for removing guess wrong stuff

        }
        else if (columnIndex >= currentRow.tiles.Length) 
        {
            if (Input.GetKeyDown(KeyCode.Return)) 
            {
                SubmitRow(currentRow);
            }
        }
        else 
        {
            if (submitted)
            {

                submitted = false;
                for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
                {
                    if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                    {
                        currentRow.tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                        columnIndex++;
                        break;
                    }
                }
            }
            if (!submitted)
            {
                for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
                {
                    if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                    {
                        currentRow.tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                        columnIndex++;
                        break;
                    }
                }
            }
        }
    }
    public void SubmitRow(Row row) 
    {
        displayWord = row.word;
        if (introScene)
        {
            introDialogue.GiveName(row.word);
        }
        if (!introScene)
        {
            Debug.Log("Guessed " + row.word + ". word is " + word);
            if (!IsCorrect(row.word))
            {
                // guessed wrong 
                incorrectGuesses++;
                incorrectGuess = row.word;
                Debug.Log("Guess does not match solutions");
                // set screen text to random insult
                string failPhraseString = RandomPhrase(failPhrase);

                // speak fail phrase - phrase must end with space, seperated with \n
                speech.speak(row.word + failPhraseString + playerName.ToLower().PadLeft(1), false);
                dialogueController.DisplayText(row.word + failPhraseString + " " + playerName + ".", false);
            }

            if (IsCorrect(row.word))
            {
                // correct guess
                // set screen text to random compliment
                string successPhraseString = RandomPhrase(successPhrase);
                // speak success phrase
                speech.speak(successPhraseString, false);
                dialogueController.DisplayText(successPhraseString, false);
                correctGuesses++;
            }




            if (!simpleSubmit)
            {
                string remaining = word;

                // check correct/incorrect letters first
                for (int i = 0; i < row.tiles.Length; i++)
                {
                    Tile tile = row.tiles[i];

                    if (tile.letter == word[i])
                    {
                        tile.SetState(correctState);

                        remaining = remaining.Remove(i, 1);
                        remaining = remaining.Insert(i, " ");
                    }
                    else if (!word.Contains(tile.letter))
                    {
                        tile.SetState(incorrectState);
                    }
                }

                // check wrong spots after
                for (int i = 0; i < row.tiles.Length; i++)
                {
                    Tile tile = row.tiles[i];

                    if (tile.state != correctState && tile.state != incorrectState)
                    {
                        if (remaining.Contains(tile.letter))
                        {
                            tile.SetState(wrongSpotState);

                            int index = remaining.IndexOf(tile.letter);
                            remaining = remaining.Remove(index, 1);
                            remaining = remaining.Insert(index, " ");
                        }
                        else
                        {
                            tile.SetState(incorrectState);
                        }
                    }
                }
            }

            if (simpleSubmit) // alternate sumbitting algorithm - does not cover all edge cases
            {
                for (int i = 0; i < row.tiles.Length; i++)
                {
                    Tile tile = row.tiles[i];

                    if (tile.letter == word[i])
                    {
                        // correct state
                        tile.SetState(correctState);
                    }
                    else if (word.Contains(tile.letter))
                    {
                        // wrong spot
                        tile.SetState(wrongSpotState);
                    }
                    else
                    {
                        // incorrect
                        tile.SetState(incorrectState);
                    }
                }
            }



            // reset columnIndex mark as submitted and disable script so player can't type
            columnIndex = 0;
            submitted = true;
            enabled = false;
            ClearBoard();
        }
    }

    private void ClearBoard()
    {
        // reset all tiles (character and state)
        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].tiles.Length; col++)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
            }
        }

        rowIndex = 0;
        columnIndex = 0;
    }

    IEnumerator ClearBoardTimer(float waitTime)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(waitTime);
        enabled = true;

        ClearBoard();
    }

    IEnumerator NewGameTimer(float waitTime)
    {
        // Wait for the specified time
        yield return new WaitForSeconds(waitTime);
        enabled = true;

        NewGame();
    }

    private bool IsCorrect(string guess) 
    {
            if (guess == word)
            {
                return true;
            }
        return false;
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {   
        if (introScene)
        {
            return;
        }
        if (!testingMode && enemyHealth.currentHealth <= 0)
        {
            return;
        }
        StartCoroutine(NewGameTimer(clearTime));     
        uIEvent.Text(displayWord);
        uIEvent.Appear();
    
        if(IsCorrect(rows[rowIndex].word))
        {
            // guessed correctly
            Debug.Log("Guess matches solution. Playing animation");
            playerHealth.AttackAnim();
        }

        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].tiles.Length; j++)
            {
                Tile tile = rows[i].tiles[j];
                tile.ResetToNormal();
                StartCoroutine(tile.FadeOutTextCoroutine(uIEvent.fadeOutTime)); // Call the FadeOut coroutine
            }
        }
    }


}
