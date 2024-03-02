using TMPro;
using UnityEngine;
using System.Collections;
using Strobotnik.Klattersynth;

public class IntroBoard : MonoBehaviour
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

    public UIEvent uIEvent;
    public Speech speech;

    [Tooltip("Test tts voice")]
    public string TextSpeech;

    public string playerName = "Jerry";
    public int currentLevel = 0;
    public bool submitted;
    public float clearTime;

    private Row[] rows;

    private string displayWord;

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
        NewGame();
        
    }

    public void NewGame()
    {
        ClearBoard();

        enabled = true;
    }

    public void TryAgain()
    {
        ClearBoard();
        enabled = true;
    }


    private void Update()
    {
        Row currentRow = rows[rowIndex];

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
    private void SubmitRow(Row row) 
    {
        displayWord = row.word;
        Debug.Log("Guessed " + row.word + ". word is " + word);

        speech.speak(row.word);

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
        StartCoroutine(NewGameTimer(clearTime));     
        uIEvent.Text(displayWord);
        uIEvent.Appear();
    

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
