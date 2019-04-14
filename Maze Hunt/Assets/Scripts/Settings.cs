using UnityEngine;
using UnityEngine.UI;

// Take values from in game sliders and save them to file with Unitys Player Prefs feature
public class Settings : MonoBehaviour
{
    public GameObject heightSlider;
    public GameObject playerSpeedSlider;
    public GameObject difficultySlider;
    public GameObject mazeSizeSlider;

    // Check to see if there the player prefs file has been created and sets the player prefs to default if it is not
    // If the values have been changed from default then set the in-game sliders to this value
    void Start()
    {
        // Player height
        if (PlayerPrefs.GetFloat("PlayerHeight") != 2)
        {
            heightSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("PlayerHeight");
        }
        else
        {
            heightSlider.GetComponent<Slider>().value = 2;
            PlayerPrefs.SetFloat("PlayerHeight", 2);
        }


        // Player speed
        if (PlayerPrefs.GetFloat("PlayerSpeed") != 3)
        {
            playerSpeedSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("PlayerSpeed");
        }
        else
        {
            playerSpeedSlider.GetComponent<Slider>().value = 6;
            PlayerPrefs.SetFloat("PlayerSpeed", 3);
        }


        // Difficulty
        if (PlayerPrefs.GetFloat("Difficulty") != 3)
        {
            difficultySlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("Difficulty");
        }
        else
        {
            difficultySlider.GetComponent<Slider>().value = 3;
            PlayerPrefs.SetFloat("Difficulty", 3);
        }


        // Maze Size
        if (PlayerPrefs.GetInt("MazeSize") != 15)
        {
            mazeSizeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MazeSize");
        }
        else
        {
            mazeSizeSlider.GetComponent<Slider>().value = 3;
            PlayerPrefs.SetInt("MazeSize", 15);
        }
    }


    // Set the player height to the slider value. Called from the slider object itself
    public void setHeight (float height)
    {
        PlayerPrefs.SetFloat("PlayerHeight", height);
    }

    // Set the player speed to the slider value. Half the value so that the slider can use the whole numbers option.Called from the slider object itself
    public void setPlayerSpeed(float speed)
    {
        speed = speed / 2;
        PlayerPrefs.SetFloat("PlayerSpeed", speed);
    }

    // Set the difficulty to the slider value. Called from the slider object itself
    public void setDifficulty(float difficulty)
    {
        PlayerPrefs.SetFloat("Difficulty", difficulty);
    }

    // Set the player speed to the slider value. Half the value so that the slider can use the whole numbers option. Multiply by 5 to save the actual maze size. Called from the slider object itself
    public void setMazeSize(float size)
    {
        int mazeSize = (int) size;
        mazeSize = mazeSize * 5;
        PlayerPrefs.SetInt("MazeSize", mazeSize);
    }
}
