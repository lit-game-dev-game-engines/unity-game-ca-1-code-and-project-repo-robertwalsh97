using UnityEngine;
using UnityEngine.AI;

// Spawns a giant orb on the player and move it towards the end goal at a speed slightly faster than the player. Uses Unity's Navmesh to find a path.
public class PathfinderMovement : MonoBehaviour
{

    public GameObject player;
    public GameObject finish;
    private Vector3 playerPosition;
    private Vector3 finishPosition;
    public NavMeshAgent pathfinder;
    public GameObject pathfinderObject;
    public GameObject mazegen;
    private float difficulty;
    public float pathfinderSpeed;
    private bool startMoving = false;

    // Get the players speed and set the speed of the orb 20% faster than the player
    void Start()
    {
        pathfinderSpeed = player.GetComponent<PlayerMovement>().speed * 1.2f;
        pathfinder.GetComponent<NavMeshAgent>().speed = pathfinderSpeed;
    }

    // Spawn the pathfinder game object and check to see if it is at the endpoint
    void Update()
    {
        // Spawn the pathfinder and set its destination to the maze exit if it should be spawned
        if (startMoving)
        {
            pathfinder.GetComponent<NavMeshAgent>().enabled = true;
            finishPosition = (mazegen.GetComponent<MazeGenerator>().finishPoint);
            pathfinder.SetDestination(finishPosition);
        }

        // Check if the pathfinder is at the exit. Checking the x and z coordinates seperately avoids a bug where the pathfinder did not detect the end goals position
        if ((pathfinderObject.transform.position.x == finishPosition.x))
        {
            if ((pathfinderObject.transform.position.z == finishPosition.z))
            {
                startMoving = false;
                pathfinderObject.SetActive(false);
            }
        }
    }

    // Spawn the pathfinder at the players positon. Disabling and enabling it before and after the function fixes a bug where the sphere would not spawn on the player correctly
    public void spawnPathfinder()
    {
        pathfinder.GetComponent<NavMeshAgent>().enabled = false;
        playerPosition = new Vector3(player.transform.position.x, 2.0f, player.transform.position.z);
        pathfinder.transform.position = playerPosition;
        startMoving = true;
        pathfinderObject.SetActive(true);    
    }
}
