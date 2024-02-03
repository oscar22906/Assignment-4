using UnityEditor.U2D.Aseprite;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Meta.WitAi.TTS.Utilities;
using System.Collections;

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

    [Header("Other Stuff")]

    public TTSSpeaker Class_speaker;
    public UIEvent uIEvent;
    public EnemyHealth enemyHealth;
    public PlayerHealth playerHealth;

    public string TextSpeech;

    public int currentLevel = 1;
    public bool submitted;
    public float clearTime;

    public bool simpleSubmit;

    private Row[] rows;

    private string incorrectGuess;
    private string displayWord;

    private string[] solutions;
    private string[] failPhrase;
    private string word;

    private int rowIndex;
    private int columnIndex;


    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
        uIEvent.SetInvisible();
    }

    private void Start() 
    {

        LoadData();
        NewGame();
        
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
        // load solutions (eg. 1)
        TextAsset textFile = Resources.Load(currentLevel.ToString()) as TextAsset;
        solutions = textFile.text.Split("\n");
        // load fail phrase (eg 1FailPhrase)
        TextAsset phraseFile = Resources.Load(currentLevel.ToString() + "FailPhrase") as TextAsset;
        failPhrase = phraseFile.text.Split("\n");


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
        Class_speaker.Speak("Spell " + word);
    }

    private void Update()
    {
        Row currentRow = rows[rowIndex];

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Class_speaker.Speak(TextSpeech);
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
    private void SubmitRow(Row row) 
    {
        displayWord = row.word;
        Debug.Log("Guessed " + row.word + ". word is " + word);
        if(!IsCorrect(row.word))
        {
            // guessed wrong 
            incorrectGuess = row.word;
            Debug.Log("Guess does not match solutions");
            // set screen text to random insult
            string failPhraseString = RandomPhrase(failPhrase);

            Class_speaker.Speak(failPhraseString);
            
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
        
        if(simpleSubmit) // alternate sumbitting algorithm - does not cover all edge cases
        {
            for (int i = 0; i < row.tiles.Length; i++)
            {
                Tile tile = row.tiles[i];

                if(tile.letter == word[i])
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
        if (enemyHealth.currentHealth <= 0)
        {
            return;
        }
        StartCoroutine(NewGameTimer(clearTime));     
        uIEvent.Text(displayWord);
        uIEvent.Appear();
    
        if(IsCorrect(rows[rowIndex].word))
        {
            // guessed correctly
            Debug.Log("Guess matches solution");
            playerHealth.AttackEnemy();
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
