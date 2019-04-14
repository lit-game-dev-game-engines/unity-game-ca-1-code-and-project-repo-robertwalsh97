using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
    //Allows classes to appear in Unity Inspector
    [System.Serializable]

    //Holds the walls for each cell
    public class Cell
    {
        public bool visited; //Has this cell been visited?
        public GameObject north; //1
        public GameObject west; //2
        public GameObject east; //3
        public GameObject south; //4
    }

    public GameObject wall;    //Wall prefab
    public GameObject respawnEnemyItem;
    public GameObject respawnPlayerItem;
    public GameObject spawnMinimapItem;
    public GameObject spawnPathfinderItem;
    public GameObject player;
    public GameObject floor;
    public GameObject enemy;
    public Vector3 playerSpawnPosition;
    public Vector3 enemySpawnPosition;
    private GameObject newItem;
    private Vector3 itemPosition;
    private GameObject itemHolder;
    public GameObject finish;
    public int xSize = 15;   //Num cells on x axis
    public int ySize = 15;   //Num cells on y axis
    private float wallLength = 1.0f;
    private Vector3 initialPosition;
    public Vector3 finishPoint;
    private GameObject wallHolder;
    public Cell[] cells;
    private int currentCell = 0;
    private int totalCells;
    private int visitedCells = 0;
    private bool startedBuilding = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int backingUpNow = 0;
    private int wallToBreak = 0;
    public float pathWidth = 3.0f;
    private float pathCenter = 0;
    public NavMeshSurface paths;
    private bool meshCreated = false;
    private float PlayerHeight = 2.0f;
    private int randomNum;


    // Use this for initialization
    void Start()
    {
        xSize = PlayerPrefs.GetInt("MazeSize");
        ySize = PlayerPrefs.GetInt("MazeSize");
        PlayerHeight = PlayerPrefs.GetFloat("PlayerHeight");
        pathCenter = pathWidth / 2;
        CreateWalls();
    }

    // Build the navmesh if it hasn't already been created
    void LateUpdate()
    {
        if(meshCreated == false)
        {
            paths.BuildNavMesh();
            meshCreated = true;
        }
    }

    void CreateWalls()
    {
        //Creat empty Gameobject that includes all walls generated so the hierarchy doesn't get cluttered with walls. 
        wallHolder = new GameObject();
        wallHolder.name = "Maze Walls";

        itemHolder = new GameObject();
        itemHolder.name = "Items";

        //Set the position where the first cell will create walls
        initialPosition = new Vector3((-xSize / 2) + wallLength / 2, 0.0f, (-ySize / 2) + wallLength / 2);
        Vector3 myPosition = initialPosition; //myPosition value is changed before it is used
        GameObject newWall; //New instance of wall prefab

        //For X Axis
        //Create west and east walls for each cell
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j <= xSize; j++)
            {
                myPosition = new Vector3(initialPosition.x + (j * pathWidth) - pathCenter, 2.75f, initialPosition.z + (i * pathWidth) - pathCenter);
                //Create new wall game object from wall prefab using position calculated. Quaternion.identity means no rotation
                newWall = Instantiate(wall, myPosition, Quaternion.identity) as GameObject;
                //Makes the wallholder object the parent of this wall
                newWall.transform.parent = wallHolder.transform;
            }
        }

        //For Y Axis
        //Create north and south walls for each cell
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPosition = new Vector3(initialPosition.x + (j * pathWidth), 2.75f, initialPosition.z + (i * pathWidth) - pathWidth);
                newWall = Instantiate(wall, myPosition, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                newWall.transform.parent = wallHolder.transform;
            }
        }

        CreateCells();
    }

    void CreateCells()
    {
        lastCells = new List<int>(); //List allows for dynamic allocation for different maze sizes
        lastCells.Clear();
        totalCells = xSize * ySize;
        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Cell[xSize * ySize];
        int westEastProcess = 0;
        int northSouthProcess = 0;
        int termCount = 0;

        //Stores all the walls in allWalls array
        for (int i = 0; i < children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;  //Store current child in walls array
        }

        //Assigns walls to the cells
        for (int cellProcess = 0; cellProcess < cells.Length; cellProcess++)
        {
            if (termCount == xSize)
            {
                westEastProcess++;
                termCount = 0;
            }


            /*The new custom class Cell is assigned to the current entry of the cells object array.
              When creating the walls, the west and east walls of each cells are made first, moving left to right for each row.
              It's not until all vertical walls are made that the horizontal walls are created.
              In a 15 * 15 maze, there are 240 vertical and horizontal walls, for a total of 480. There are 225 cells.
              A 15 * 15 maze actually has 16 vertical walls per row, and 16 horizontal walls per column.
              westEastProcess accounts for the first 240 walls, northSouthProcess accounts for the final 240.*/

            cells[cellProcess] = new Cell();
            cells[cellProcess].west = allWalls[westEastProcess];
            cells[cellProcess].south = allWalls[northSouthProcess + (xSize + 1) * ySize];

            //Increment values to move onto the next wall
            westEastProcess++;
            termCount++;
            northSouthProcess++;

            cells[cellProcess].east = allWalls[westEastProcess];
            cells[cellProcess].north = allWalls[(northSouthProcess + (xSize + 1) * ySize) + xSize - 1];

        }

        CreateMaze();
    }

    //Use depth first search to carve paths through the cells
    void CreateMaze()
    {
        while (visitedCells < totalCells) //Have all the cells been visited?
        {
            if (startedBuilding) // Continue building the maze
            {
                GiveMeNeighbour(); // Picks a random neighbour cell

                if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true) 
                {
                    //Break the wall between the current cell and neighbour cell, set the current cell as the neighbour cell and mark it as visited
                    BreakWall();
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell); // Lastcells is a list of all cells that have been visited since we last backtracked
                    currentCell = currentNeighbour;

                    if (lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1; //Backingup allows you to move to the correct cell in the list of lastCells
                        backingUpNow = backingUp;
                    }
                }
            }
            else //Start the maze building process
            {
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }

            //Debug.Log("Finished");

        }


        //Player spawn point - Always bottom right cell of the maze
        playerSpawnPosition = new Vector3(cells[0].west.transform.position.x + pathCenter, PlayerHeight, cells[0].north.transform.position.z - pathCenter);
        player.transform.position = playerSpawnPosition;

        //Finish spawn point - Always top left cell of the maze
        int lastCell = totalCells - 1;
        finishPoint = new Vector3(cells[lastCell].west.transform.position.x + pathCenter, 11.0f, cells[lastCell].north.transform.position.z - pathCenter);
        finish.transform.position = finishPoint;

        //Finish spawn point - Always bottom left cell of the maze
        enemySpawnPosition = new Vector3(cells[0].west.transform.position.x + pathCenter, 7.0f, cells[lastCell].north.transform.position.z - pathCenter);
        enemy.transform.position = enemySpawnPosition;

        //Change size of floor dynamically (Causes texture to dissapear, difficult to scale & position properly)
        //floor.transform.localScale = new Vector3((xSize * pathWidth) + pathWidth, 0.0f, (ySize * pathWidth) + pathWidth);
        //floor.transform.position = new Vector3((xSize *2) - pathWidth, -0.55f, (ySize *2) - pathWidth);
        //GetComponent<Renderer>.material.mainTexture = 

    }

    //Pick a random neighbour cell
    void GiveMeNeighbour()
    {
        int neighbourLength = 0; // Contains the neighbours that have been found
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int inBoundsCheck = 0; // Used to check if the cell is out of bounds
        inBoundsCheck = ((currentCell + 1) / xSize);//When you divide a cell num by the size of row you get a rough num of how many rows you have to climb to reach that cell
        inBoundsCheck -= 1; // Subtract that num by 1 to give you the num of rows that are under that cell
        inBoundsCheck *= xSize; // Now multiply the num to get how many cells are covered by the check
        inBoundsCheck += xSize; // The size of the row needs to be added

        //Check to see if the west wall is in bounds
        if (currentCell + 1 < totalCells && (currentCell + 1) != inBoundsCheck)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbours[neighbourLength] = currentCell + 1;
                connectingWall[neighbourLength] = 3;
                neighbourLength++;
            }
        }

        // Check to see if the east wall is in bounds
        if (currentCell - 1 >= 0 && currentCell != inBoundsCheck)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[neighbourLength] = currentCell - 1;
                connectingWall[neighbourLength] = 2;
                neighbourLength++;
            }
        }

        // Check to see if the north wall is in bounds
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[neighbourLength] = currentCell + xSize;
                connectingWall[neighbourLength] = 1;
                neighbourLength++;
            }
        }

        // Check to see if the south wall is in bounds
        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbours[neighbourLength] = currentCell - xSize;
                connectingWall[neighbourLength] = 4;
                neighbourLength++;
            }
        }

        //Choose a random neighbour from the available neighbours
        if (neighbourLength != 0)
        {
            int chosenCell = Random.Range(0, neighbourLength);
            currentNeighbour = neighbours[chosenCell];
            wallToBreak = connectingWall[chosenCell];
        }
        else // There are no available neighbours (i.e. dead end) Start backing up.
        {
            if (backingUp > 0) 
            {
                if(backingUpNow == backingUp)
                {
                    //If a number other than 1 or 2 are picked, do nothing. Increasing the second argument adds chance an item will spawn at a given dead end
                    //100% chance to spawn 1 of 4 items
                    randomNum = Random.Range(1, 5);
                    //Debug.Log(randomNum);
                    if(randomNum == 1)
                    {
                        SpawnItem(respawnEnemyItem);
                    }
                    else if (randomNum == 2)
                    {
                        SpawnItem(respawnPlayerItem);
                    }
                    else if (randomNum == 3)
                    {
                        SpawnItem(spawnMinimapItem);
                    }
                    else if(randomNum == 4)
                    {
                        SpawnItem(spawnPathfinderItem);
                    }
                }

                //Make the current cell the last entry in the list i.e back up one
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }

    // Destroy the chosen wall game object
    void BreakWall()
    {
        switch(wallToBreak)
        {
            case 1: Destroy(cells[currentCell].north); break;
            case 2: Destroy(cells[currentCell].west); break;
            case 3: Destroy(cells[currentCell].east); break;
            case 4: Destroy(cells[currentCell].south); break;

        }
    }

    void SpawnItem(GameObject item)
    {
        //item spawn points - Always spawns at a dead end
        itemPosition = new Vector3(cells[currentCell].west.transform.position.x + pathCenter, 1.0f, cells[currentCell].north.transform.position.z - pathCenter);
        newItem = Instantiate(item, itemPosition, Quaternion.identity) as GameObject;
        newItem.transform.parent = itemHolder.transform;
    }
}
