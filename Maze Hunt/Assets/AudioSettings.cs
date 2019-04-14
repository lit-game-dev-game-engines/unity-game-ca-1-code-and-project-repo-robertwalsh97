using System.Collections;
using UnityEngine;

// Allows sound effects to be played at random intervals. Only used in the normal maze
public class AudioSettings : MonoBehaviour
{
    private bool isPlayingAudio = false;

    // Check if audio is playing every frame and play a sound if it isn't
    void Update()
    {
        if (!isPlayingAudio)
        {
            StartCoroutine(playSound());
        }
    }

    //Coroutine to play sound at a random time interval
    IEnumerator playSound()
    {
        isPlayingAudio = true;

        yield return new WaitForSeconds(Random.Range(3f, 10f));

        this.GetComponent<AudioSource>().Play();

        isPlayingAudio = false;
    }
}
