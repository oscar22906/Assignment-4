using UnityEngine;
using UnityEngine.UI;

public class Row : MonoBehaviour
{
    public Tile[] tiles { get; private set; }
    public VerticalLayoutGroup layoutGroup;
    public float initialSpacing = 10f;
    public float spacingDecreasePercentage = 10f;

    private void Awake()
    {
        tiles = GetComponentsInChildren<Tile>();
        UpdateSpacing();
    }

    public string word
    {
        get
        {
            string word = "";

            for (int i = 0; i < tiles.Length; i++)
            {
                word += tiles[i].letter;
            }
            return word;
        }
    }

    // Call this method whenever a new tile is added to the row
    public void AddTile(Tile newTile)
    {
        // Add the new tile to the array
        Tile[] newTiles = new Tile[tiles.Length + 1];
        for (int i = 0; i < tiles.Length; i++)
        {
            newTiles[i] = tiles[i];
        }
        newTiles[tiles.Length] = newTile;
        tiles = newTiles;

        // Update spacing
        UpdateSpacing();
    }

    // Update spacing based on the number of tiles
    private void UpdateSpacing()
    {
        if (layoutGroup == null)
        {
            Debug.LogWarning("VerticalLayoutGroup not assigned");
            return;
        }

        // Calculate the new spacing
        float newSpacing = initialSpacing - (initialSpacing * spacingDecreasePercentage * tiles.Length / 100f);

        // Apply the new spacing
        layoutGroup.spacing = newSpacing;
    }
}
