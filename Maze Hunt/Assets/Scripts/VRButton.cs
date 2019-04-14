using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Loads scene, quits app or changes button colour. Is called when the player highlights/selects buttons in-game
public class VRButton : MonoBehaviour
{

    public Image backgroundImage;
    public Color normalColour;
    public Color highlightColour;

    // Changes button colour if the player looks at a button. Colours are set in Unity editor
    public void onGazeEnter()
    {
        backgroundImage.color = highlightColour;
    }

    // Returns the button colour to its original colour when the player looks away from the button. Colours are set in Unity editor
    public void onGazeExit()
    {
        backgroundImage.color = normalColour;
    }

    // Loads the chosen scene when the button is selected. Only called by the 'Play Normal Maze', 'Play Maze Hunt' and 'New Maze' buttons
    public void onClick(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    // Quits the game when the button is selected. Only called by the 'Quit' button
    public void QuitGame()
    {
        Application.Quit();
    }
}

