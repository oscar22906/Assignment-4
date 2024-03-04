using UnityEngine;

public class MainMenuClick : MonoBehaviour
{
    public GameObject title;
    public GameObject buttonPanel;

    private int timesClicked = 0;

    void Update()
    {
        HandleMouseClick();
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left mouse button click
        {
            timesClicked++;

            if (timesClicked == 1)
            {
                title.SetActive(true);
            }
            else if (timesClicked == 2)
            {
                title.SetActive(true);
                buttonPanel.SetActive(true);
            }
        }
    }
}
