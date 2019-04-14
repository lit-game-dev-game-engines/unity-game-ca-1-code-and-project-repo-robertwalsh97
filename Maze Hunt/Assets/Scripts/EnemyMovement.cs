using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Sets the speed and destination of the enemy and plays a footstep sound effect
public class EnemyMovement : MonoBehaviour
{

    public GameObject player;
    private Vector3 playerPosition;
    public NavMeshAgent enemy;
    private float difficulty;
    private float enemySpeed;
    private bool isPlayingAudio;

    void Start()
    {
        // Use Player Prefs to read in the value for difficulty, which is set in settings
        // Enemy speed must be a percentage of player speed so that the enemy is never faster than the player
        // Enemy speed is set in its Navmesh component
        difficulty = PlayerPrefs.GetFloat("Difficulty");

        if (difficulty == 1) enemySpeed = player.GetComponent<PlayerMovement>().speed * 0.1f;
        if (difficulty == 2) enemySpeed = player.GetComponent<PlayerMovement>().speed * 0.3f;
        if (difficulty == 3) enemySpeed = player.GetComponent<PlayerMovement>().speed * 0.5f;
        if (difficulty == 4) enemySpeed = player.GetComponent<PlayerMovement>().speed * 0.7f;
        if (difficulty == 5) enemySpeed = player.GetComponent<PlayerMovement>().speed * 0.9f;

        enemy.GetComponent<NavMeshAgent>().speed = enemySpeed;
    }

    // Update is called once per frame
    void Update ()
    {
        // Set the enemy target position to the player every frame
        playerPosition = new Vector3 (player.transform.position.x, 0.0f, player.transform.position.z);
        enemy.SetDestination(playerPosition);

        // If the player is moving and not playing a sound, start the coroutine to play footsteps
        if (!isPlayingAudio && enemy.GetComponent<NavMeshAgent>().speed > 0)
        {
            StartCoroutine(playFootsteps());
        }
    }

    // Plays footstep sound effect if the player is moving. Must wait for 0.5 seconds before continuing so footstep doesn't play every frame
    IEnumerator playFootsteps()
    {
        isPlayingAudio = true;

        yield return new WaitForSeconds(0.5f);

        // Randomise each footstep so each step sounds different
        this.GetComponent<AudioSource>().volume = Random.Range(0.8f, 1f);
        this.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1);
        this.GetComponent<AudioSource>().Play();

        isPlayingAudio = false;
    }
}
