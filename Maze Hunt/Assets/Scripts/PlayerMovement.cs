using System.Collections;
using UnityEngine;

// Moves the player in the direction the camera is looking and play footsteps
public class PlayerMovement : MonoBehaviour
{

    public float speed = 3.0f;
    private bool isPlayingAudio;
    Vector3 forward;

    // Set the players position to that set in the Player prefs file
    void Start()
    {
        speed = PlayerPrefs.GetFloat("PlayerSpeed");
    }

    // Moves the player in the direction the camera is looking and trigger the coroutine to play footsteps
    void Update()
    {
        // The Samsung Gear VR interprets a left mouse click as the touchpad being touched
        if (Input.GetMouseButton(0))
        {
            forward = Camera.main.transform.forward;
            forward.y = 0;
            this.transform.position += forward * Time.deltaTime * speed;

            if(!isPlayingAudio)
            {
                StartCoroutine(playFootsteps());
            }
        }
    }

    // Plays footstep sound effect at a random pitch and volume every 0.5 seconds while the player is moving
    IEnumerator playFootsteps()
    {
        isPlayingAudio = true;

        yield return new WaitForSeconds(0.5f);

        this.GetComponent<AudioSource>().volume = Random.Range(0.8f, 1f);
        this.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1);
        this.GetComponent<AudioSource>().Play();

        isPlayingAudio = false;
    }
}
