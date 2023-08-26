using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Baracuda.Monitoring;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = System.Diagnostics.Debug;

public class TerrainGenerator : MonoBehaviour
{
    public int width;
    public int height;
    [Tooltip("Amount of \"walkers\", a higher amount will result in the map being emptier.")]
    public int walkerCount;
    [Tooltip("Amount of steps \"walkers\" will take, a higher amount will result in a snakier terrain.")]
    public int walkerPasses;
    public int minimumTiles;
    public GameObject container;
    public GameObject terrainPrefab;
    public NavmeshBaker navmeshBaker;
    
    // [Monitor]
    private int _emptyTiles = 0;
    private bool[][] _finalMap;

    private void Flood(bool[][] walls, ref bool[][] copy, int x, int y)
    {
        if (x > width - 1 || x < 0 || y > height - 1 || y < 0) //if outside, no point in checking
            return;
        if (walls[x][y]) //if we're on a wall, no point in checking
            return;
        _emptyTiles++;
        copy[x][y] = false; //set the tile as empty in copy map
        walls[x][y] = true; //set it as full in the original map, why? just so we don't check it twice, which would be bad
        Flood(walls, ref copy, x + 1, y); //recursively run the same function on the tiles north, south, east and west of us
        Flood(walls, ref copy, x - 1, y);
        Flood(walls, ref copy, x, y + 1);
        Flood(walls, ref copy, x, y - 1);
    }

    private IEnumerator GenerateMap()
    {
	    while (_emptyTiles < minimumTiles)
	    {
		    _emptyTiles = 0;
		    bool[][] walls = new bool[width][];
		    bool[][] copy = new bool[width][]; //set up 2 arrays, why? it's a surprise tool for later
		    for (int i = 0; i < width; i++)
		    {
			    walls[i] = new bool[height];
			    copy[i] = new bool[height];
			    for (int c = 0; c < height; c++)
			    {
				    walls[i][c] = true; //set every value in them to true
				    copy[i][c] = true;
			    }
		    }

		    for (int i = 0; i < walkerCount; i++)
		    {
			    int x = Random.Range(0, width), y = Random.Range(0, height); //place walker in a random spot
			    walls[x][y] = false;
			    for (int c = 0; c < walkerPasses; c++)
			    {
				    bool coin = Random.Range(0, 2) == 1; //flip a coin
				    if (coin)
				    {
					    if (y == 0)
					    {
						    y++;
					    }
					    else if (y == height - 1)
					    {
						    y--;
					    }
					    else
					    {
						    y += Random.Range(0, 2) == 1 ? -1 : 1;
					    }
				    }
				    else
				    {
					    if (x == 0)
					    {
						    x++;
					    }
					    else if (x == width - 1)
					    {
						    x--;
					    }
					    else
					    {
						    x += Random.Range(0, 2) == 1 ? -1 : 1;
					    }
				    }

				    if (!(x > width - 1 || x < 0 || y > height - 1 || y < 0))
				    {
					    walls[x][y] = false; //remove the wall at position
				    }
				    else
				    {
					    break; //if outside the map, we fucked, better bail
				    }
			    }
		    }

		    int checkX = width / 2, checkY = height / 2; //get a point in the middle
		    while (walls[checkX][checkY]) //find an empty tile via RNG
		    {
			    bool coin = Random.Range(0, 2) == 1; //same logic as walkers
			    if (coin)
			    {
				    if (checkY == 0)
				    {
					    checkY++;
				    }
				    else if (checkY == height - 1)
				    {
					    checkY--;
				    }
				    else
				    {
					    checkY += Random.Range(0, 2) == 1 ? -1 : 1;
				    }
			    }
			    else
			    {
				    if (checkX == 0)
				    {
					    checkX++;
				    }
				    else if (checkX == width - 1)
				    {
					    checkX--;
				    }
				    else
				    {
					    checkX += Random.Range(0, 2) == 1 ? -1 : 1;
				    }
			    }
		    }

		    Flood(walls, ref copy, checkX, checkY); //flood the map to find all connected empty tiles
		    _finalMap = copy;
            
		    yield return null;
	    }

	    for (int x = 0; x < width; x++)
	    {
		    for (int y = 0; y < height; y++)
		    {
			    Vector3 spawnPlace = new Vector3(x - (width / 2), y - (height / 2), 0);
			    if (!_finalMap[x][y]) continue; //no wall is needed here
			    if (x - 1 >= 0 && _finalMap[x - 1][y])
			    {
				    GameObject obj = Instantiate(terrainPrefab, spawnPlace, new Quaternion(), container.transform);
				    Vector3 pos = obj.transform.position;
				    pos.x -= 0.25f;
				    obj.transform.position = pos;
				    Vector3 rotate = obj.transform.eulerAngles;
				    rotate.z -= 90f;
				    obj.transform.eulerAngles = rotate;
			    }
			    
			    if (x + 1 < width && _finalMap[x + 1][y])
			    {
				    GameObject obj = Instantiate(terrainPrefab, spawnPlace, new Quaternion(), container.transform);
				    Vector3 pos = obj.transform.position;
				    pos.x += 0.25f;
				    obj.transform.position = pos;
				    Vector3 rotate = obj.transform.eulerAngles;
				    rotate.z -= -90f;
				    obj.transform.eulerAngles = rotate;
			    }
			    
			    if (y - 1 >= 0 && _finalMap[x][y - 1])
			    {
				    GameObject obj = Instantiate(terrainPrefab, spawnPlace, new Quaternion(), container.transform);
				    Vector3 pos = obj.transform.position;
				    pos.y -= 0.25f;
				    obj.transform.position = pos;
			    }

			    if (y + 1 < height && _finalMap[x][y + 1])
			    {
				    GameObject obj = Instantiate(terrainPrefab, spawnPlace, new Quaternion(), container.transform);
				    Vector3 pos = obj.transform.position;
				    pos.y += 0.25f;
				    obj.transform.position = pos;
				    Vector3 rotate = obj.transform.eulerAngles;
				    rotate.z += 180f;
				    obj.transform.eulerAngles = rotate;
			    }

			    // Single dot
			    if (x - 1 >= 0 && !_finalMap[x - 1][y] && 
			        x + 1 < width && !_finalMap[x + 1][y] && 
			        y - 1 >= 0 && !_finalMap[x][y - 1] && 
			        y + 1 < height && !_finalMap[x][y + 1])
			    {
				    if (Random.value < 0.9f)
				    {
					    _finalMap[x][y] = false;
				    }
				    else
				    {
					    GameObject obj = Instantiate(terrainPrefab, spawnPlace, new Quaternion(), container.transform);
					    Vector3 rotate = obj.transform.eulerAngles;
					    Vector3 pos = obj.transform.position;
					    rotate.z += 180f;
					    pos.y -= 0.25f;
					    obj.transform.position = pos;
					    obj.transform.eulerAngles = rotate;
					    GameObject obj2 = Instantiate(terrainPrefab, spawnPlace, new Quaternion(), container.transform);
					    pos = obj2.transform.position;
					    pos.y += 0.25f;
					    obj2.transform.position = pos;
				    }
			    }
		    }
	    }

	    StartCoroutine(navmeshBaker.GenerateMesh());
	    StartCoroutine(GetComponent<EnemyGen>().GenerateEnemies());
    }
    
    void OnDrawGizmos()
    {
	    Gizmos.color = Color.yellow;
	    for (int x = 0; x < width; x++)
	    {
		    for (int y = 0; y < height; y++)
		    {
			    if(_finalMap[x][y]) Gizmos.DrawCube(new Vector3(x - (width / 2), y - (height / 2), -5), new Vector3(1, 1, 1));
		    }
	    }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateMap());
    }
    
    private void Awake()
    {
	    this.StartMonitoring();
    }

    private void OnDestroy()
    {
	    this.StopMonitoring();
    }
}
