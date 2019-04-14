using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Detects collision between the player and other game objects. Also triggers pause screen and determines how special abilities work
public class PlayerCollision : MonoBehaviour
{
    public GameObject sphere;
    public GameObject player;
    public GameObject enemy;
    public GameObject menu;
    public GameObject line;
    public GameObject pathfinder;
    public GameObject respawnEnemyButton;
    public GameObject respawnPlayerButton;
    public GameObject spawnMinimapButton;
    public GameObject spawnPathfinderButton;
    public GameObject youWinText;
    public GameObject youLoseText;
    public GameObject mazeGenerator;
    public GameObject minimap;
    private List<GameObject> items = new List<GameObject>();
    public Material blackFade;
    public Material blackOpaque;
    public float fadeSpeed;
    public float waitForFade;
    private bool increaseAlpha = false;
    private Color black;
    Renderer rend;
    private Vector3 menuPosition;
    private bool showWinText = false;
    private bool showLoseText = false;
    private float enemyDefaultSpeed;
    private float playerDefaultSpeed;
    public bool respawnEnemyItemObtained = false;
    public bool respawnPlayerItemObtained = false;
    public bool spawnMinimapItemObtained = false;
    public bool spawnPathfinderItemObtained = false;
    private bool isPauseMenu = false;
    private bool isEnemyDead = false;
    private float enemyRespawnDelay = 10;
    private float mapRespawnDelay = 25;
    private float timeOfDeath = 0.0f;
    private float timePassed = 0.0f;
    private bool minimapActive = false;
    private float timeOfMapSpawn = 0.0f;

    // Assign variables
    void Start()
    {
        rend = GetComponent<Renderer>();
        black = new Color(0f, 0f, 0f, 1f);
        menu.SetActive(false);
        youWinText.SetActive(false);
        youLoseText.SetActive(false);
        respawnEnemyButton.SetActive(false);
        respawnPlayerButton.SetActive(false);
        spawnMinimapButton.SetActive(false);
        spawnPathfinderButton.SetActive(false);
        enemyDefaultSpeed = enemy.GetComponent<NavMeshAgent>().speed;
        playerDefaultSpeed = player.GetComponent<PlayerMovement>().speed;
    }

    // Setup pause menu when the player presses the pause button
    void Update()
    {
        // Set the time and check to see if the enemy should be respawned
        timePassed += Time.deltaTime;
        checkRespawn();

        // Setup pause menu on button press
        if (Input.GetMouseButton(1)) // Uncomment for PC. Right click to enter the pause menu
        //if (OVRInput.GetDown(OVRInput.Button.Back)) // Uncomment for Samsung Gear VR. Press back to enter the pause menu
        {
            //Stop player and enemy from moving and disable minimap
            enemy.GetComponent<NavMeshAgent>().speed = 0;
            player.GetComponent<PlayerMovement>().speed = 0;
            minimap.SetActive(false);
            increaseAlpha = true;
            isPauseMenu = true;

            // Enable respawn enemy item in the pause menu if the player has collected it
            if (!respawnEnemyItemObtained) { respawnEnemyButton.SetActive(false); }
            else { respawnEnemyButton.SetActive(true); }

            // Enable respawn player item in the pause menu if the player has collected it
            if (!respawnPlayerItemObtained) { respawnPlayerButton.SetActive(false); }
            else { respawnPlayerButton.SetActive(true); }

            // Enable minimap item in the pause menu if the player has collected it
            if (!spawnMinimapItemObtained) { spawnMinimapButton.SetActive(false); }
            else { spawnMinimapButton.SetActive(true); }

            // Enable spawn pathfinder item in the pause menu if the player has collected it
            if (!spawnPathfinderItemObtained) { spawnPathfinderButton.SetActive(false); }
            else { spawnPathfinderButton.SetActive(true); }
        }

        // Increase alpha of the invisible sphere around the player so it restricts their view to the pause menu
        if (increaseAlpha)
        {
            rend.material.SetColor("_Color", new Color(0f, 0f, 0f, Mathf.MoveTowards(rend.material.color.a, 1f, fadeSpeed * Time.deltaTime)));
        }

        // If the player sphere is completely black, spawn pause menu items
        if (rend.material.color == black)
        {
            // Disable finish line so it does not clip through player sphere
            line.SetActive(false);

            // Move the player above the level so the player sphere does not clip through objects in the level
            player.transform.position = new Vector3(player.transform.position.x, 15, player.transform.position.z);
            menu.SetActive(true);

            // If the player has won/lost show the appropriate text and disable items
            if (showLoseText && !showWinText)
            {
                respawnEnemyItemObtained = false;
                respawnPlayerItemObtained = false;
                spawnMinimapItemObtained = false;
                spawnPathfinderItemObtained = false;
                youLoseText.SetActive(true);
                youWinText.SetActive(false);
                minimap.SetActive(false);
            }
            if (showWinText && !showLoseText)
            {
                respawnEnemyItemObtained = false;
                respawnPlayerItemObtained = false;
                spawnMinimapItemObtained = false;
                spawnPathfinderItemObtained = false;
                youWinText.SetActive(true);
                youLoseText.SetActive(false);
                minimap.SetActive(false);
            }

            // Changing the sphere material avoids a bug where menu items weren't visible. Increase the sphere size to accomodate all menu items
            sphere.GetComponent<RectTransform>().localScale = new Vector3(8,8,8);
            rend.material = blackOpaque;

            //Set the menu position to that of the player
            menuPosition = new Vector3(player.transform.position.x + 0.01f, player.transform.position.y, player.transform.position.z + 0.01f);
            menu.transform.position = menuPosition;

            // Exit the pause menu and restore variables
            if (isPauseMenu && Input.GetMouseButton(2)) // Uncomment for PC. Press middle mouse button to exit pause menu
            //if (isPauseMenu && OVRInput.GetDown(OVRInput.Button.Back)) // Uncomment for Samsung Gear VR. Press back to exit pause menu
            {
                if (minimapActive) minimap.SetActive(true);
                line.SetActive(true);
                ResumeGame();
            }
        }
    }

    // Detect collision between the player and other game objects and determine what happens on collision
    void OnTriggerEnter(Collider col)
    {
        // If the player collides with the enemy or finish point, stop the player/enemy, show appropriate text and play the appropriate sound effect
        if(col.gameObject.name == "Enemy" || col.gameObject.name == "SpotLight")
        {
            increaseAlpha = true;
            enemy.GetComponent<NavMeshAgent>().speed = 0;
            player.GetComponent<PlayerMovement>().speed = 0;
            
        
            if (col.gameObject.name == "SpotLight")
            {
                showWinText = true;
                showLoseText = false;
                line.GetComponent<AudioSource>().Play();

            }
            if (col.gameObject.name == "Enemy")
            {
                showLoseText = true;
                showWinText = false;
                mazeGenerator.GetComponent<AudioSource>().Play();
            }
        }

        // If the player collides with the respawn enemy item, remove the item from the maze and add it to the players inventory. 
        // Add to list of items that need to be respawned if the player restarts and play appropriate sound effect
        if(col.gameObject.name == "respawnEnemyItem(Clone)")
        {
            items.Add(col.gameObject);
            col.gameObject.SetActive(false);
            respawnEnemyItemObtained = true;
            this.GetComponent<AudioSource>().Play();
        }

        // If the player collides with the respawn player item, remove the item from the maze and add it to the players inventory. 
        // Add to list of items that need to be respawned if the player restarts and play appropriate sound effect
        if (col.gameObject.name == "respawnPlayerItem(Clone)")
        {
            items.Add(col.gameObject);
            col.gameObject.SetActive(false);
            respawnPlayerItemObtained = true;
            this.GetComponent<AudioSource>().Play();
        }

        // If the player collides with the minimap item, remove the item from the maze and add it to the players inventory. 
        // Add to list of items that need to be respawned if the player restarts and play appropriate sound effect
        if (col.gameObject.name == "spawnMinimapItem(Clone)")
        {
            items.Add(col.gameObject);
            col.gameObject.SetActive(false);
            spawnMinimapItemObtained = true;
            this.GetComponent<AudioSource>().Play();
        }

        // If the player collides with the spawn pathfinder item, remove the item from the maze and add it to the players inventory. 
        // Add to list of items that need to be respawned if the player restarts and play appropriate sound effect
        if (col.gameObject.name == "spawnPathfinderItem(Clone)")
        {
            items.Add(col.gameObject);
            col.gameObject.SetActive(false);
            spawnPathfinderItemObtained = true;
            this.GetComponent<AudioSource>().Play();
        }
    }

    // Reset maze objects and variables when the player restarts the game
    public void resetMaze()
    {
        //Disabling and re-enabling the navmesh component fixes a bug where the enemy would not reset to its spawn position correctly on restart
        player.transform.position = mazeGenerator.GetComponent<MazeGenerator>().playerSpawnPosition;
        enemy.GetComponent<NavMeshAgent>().enabled = false;
        enemy.transform.position = mazeGenerator.GetComponent<MazeGenerator>().enemySpawnPosition;
        enemy.GetComponent<NavMeshAgent>().enabled = true;

        // Respawn collected items
        for (int i = 0; i < items.Count; i++)
        {
            items[i].SetActive(true);
        }

        respawnEnemyItemObtained = false;
        respawnPlayerItemObtained = false;
        spawnMinimapItemObtained = false;
        items.Clear();
        ResetValues();
    }

    //Place the player at their position before the pause screen and reset some values
    public void ResumeGame()
    {
        isPauseMenu = false;
        player.transform.position = new Vector3(player.transform.position.x, PlayerPrefs.GetFloat("PlayerHeight"), player.transform.position.z);
        ResetValues();
    }

    // Reset values for when the player resumes the game
    public void ResetValues()
    {
        increaseAlpha = false;
        rend.material = blackFade;
        rend.material.SetColor("_Color", new Color(0f, 0f, 0f, 0f));

        menu.SetActive(false);

        sphere.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);

        enemy.GetComponent<NavMeshAgent>().speed = enemyDefaultSpeed;
        player.GetComponent<PlayerMovement>().speed = playerDefaultSpeed;

        showLoseText = false;
        showWinText = false;
        youLoseText.SetActive(false);
        youWinText.SetActive(false);

        line.SetActive(true);
    }

    // Disable the enemy when the respawn enemy item is used
    public void killEnemy()
    {
        respawnEnemyItemObtained = false;
        respawnEnemyButton.SetActive(false);

        if (enemy.activeSelf)
        {
            this.GetComponent<AudioSource>().Play();
            isEnemyDead = true;
            timeOfDeath = timePassed;
            enemy.SetActive(false);
        }

        ResumeGame();
    }

    // Check to see if the enemy should be respawned after being disabled
    private void checkRespawn()
    {
        if (isEnemyDead)
        {
            if ((timePassed - timeOfDeath) > enemyRespawnDelay)
            {
                enemy.SetActive(true);
                isEnemyDead = false;
            }
        }

        if (minimapActive)
        {
            if ((timePassed - timeOfMapSpawn) > mapRespawnDelay)
            {
                minimap.SetActive(false);
                minimapActive = false;
            }
        }
    }

    // Reset the players position to the start of the maze when they use the respawn player item
    public void resetPlayer()
    {
        this.GetComponent<AudioSource>().Play();
        respawnPlayerItemObtained = false;
        respawnPlayerButton.SetActive(false);
        player.transform.position = mazeGenerator.GetComponent<MazeGenerator>().playerSpawnPosition;
        ResumeGame();
    }

    // Spawn the minimap when the player uses the minimpa item
    public void spawnMinimap()
    {
        this.GetComponent<AudioSource>().Play();
        timeOfMapSpawn = timePassed;
        minimapActive = true;
        spawnMinimapItemObtained = false;
        spawnMinimapButton.SetActive(false);
        minimap.SetActive(true);
        ResumeGame();
    }

    // Spawn the pathfinder when the player uses the pathfinder item
    public void spawnPathfinder()
    {
        this.GetComponent<AudioSource>().Play();
        spawnPathfinderItemObtained = false;
        spawnPathfinderButton.SetActive(false);
        ResumeGame();
    }
}
