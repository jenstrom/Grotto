using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {

	public bool generateFogOfWar = true;

	public int numberOfLevels   = 2;				//How many map texures do we have
	public TileType[] tileTypes = new TileType[8];	//Holds the.. you guessed it, tiletypes.

	public int[,] mapDataTiles;						//2d Array to hold all the tiles for the map. // coordinates and index number for the tiletype

	public FogOfWarCreation fow;
	
	// Methods that could be usefull outside of this script
	/* *************************************************************** */

	// Returns a tileType at a specific x z coordinate
	public TileType GetTileType(int xCoordinate, int zCoordinate){
		// Find the tiletype number of a tile at x,z coordinates
		int numberOfType = (int)mapDataTiles.GetValue( xCoordinate, zCoordinate );
		// Return the tiletype
		return tileTypes[numberOfType];
	}  

	// returns a list of cordinates that a specific tileType is attached to
	public List<Vector3> GetCoordinatesOfTileTypes(int tiletype){
		List<Vector3> listOfCoordinates = new List<Vector3>();

		int arrayWidth  = mapDataTiles.GetLength(0);
		int arrayHeight = mapDataTiles.GetLength(1);

		for (int x = 0; x < arrayWidth; ++x) {
			for (int z = 0; z < arrayHeight; ++z) {
				if( mapDataTiles[x, z].Equals(tiletype) )
					listOfCoordinates.Add( new Vector3 (x, 0, z) );
			}
		}
		return listOfCoordinates;
	}
	
	// Get list of coordinates that have walkable tiles. 
	public List<Vector3> GetCoordinatesOfWalkableTiles(){
		List<Vector3> listOfCoordinates = new List<Vector3>();
		
		int arrayWidth  = mapDataTiles.GetLength(0);
		int arrayHeight = mapDataTiles.GetLength(1);
		
		for (int x = 0; x < arrayWidth; ++x) {
			for (int z = 0; z < arrayHeight; ++z) {
				if( GetTileType(x,z).isWalkable )
				listOfCoordinates.Add( new Vector3 (x ,0f, z) );
			}
		}
		return listOfCoordinates;
	}

	// maybe methods we need
	//void SetTileType(){}

	//void GenerateTrap(int numberOfTraps){}

	//void GenerateTrap(int xCoordinate, int zCoordinate){}

	/* *************************************************************** */
	

	//Awake is used to initialize any variables or game state before the game starts.
	void Awake () {
		GenerateTileTypes();

		ProcessImage();

		// Generate Floor, Wall and Roof meshes
		int[,] map = new int[mapDataTiles.GetLength(0), mapDataTiles.GetLength(1)];

		for( int x = 0; x < mapDataTiles.GetLength(0); x++)	{
			for( int z = 0; z < mapDataTiles.GetLength(1); z++)	{
				if ( mapDataTiles[x,z] == 3 )
					map[x,z] = 1;
				else
				map[x,z] = 0;
			}
		}
		int[,] floorMap = new int[mapDataTiles.GetLength(0), mapDataTiles.GetLength(1)];
		for( int x = 0; x < mapDataTiles.GetLength(0); x++)	{
			for( int z = 0; z < mapDataTiles.GetLength(1); z++)	{
				if ( mapDataTiles[x,z] != 3 )
					floorMap[x,z] = 1;
				else
					floorMap[x,z] = 0;
			}
		}
		MeshGenerator meshGen = GetComponent<MeshGenerator>();
		meshGen.GenerateMesh(map, 1);
		meshGen.GenerateFloorMesh(floorMap, 1);
	}

	void Start () {

		//Example on how to get a list of coordinates for a tileType
		//Debug.Log( "X = " + GetCoordinatesOfTileTypes(1)[0].x + " Z = " + GetCoordinatesOfTileTypes(1)[0].z	);

		//Example on how to use GetTileType method
		//Debug.Log( GetTileType(0,0).isWalkable );
	}

	//Fill up the TileTypes with some default data types
	void GenerateTileTypes(){
		tileTypes [0].name 		 = "Enemy_1 SpawnPoint";												// Name of this tileType. ex: Grass,Stone.
		tileTypes [0].isWalkable = true;																// Allowed to walk on, or not.
		tileTypes [0].prefab 	 = null;																// A premade prefab object that represent this tileType is connected
		tileTypes [0].display 	 = true;																// Display anything at this tile coordinates. (maybe usless)
		tileTypes [0].color		 = Color.red;															// Color on the image

		tileTypes [1].name 		 = "Player SpawnPoint";
		tileTypes [1].isWalkable = true;
		tileTypes [1].prefab  	 = null;
		tileTypes [1].display 	 = true;
		tileTypes [1].color 	 = Color.green;

		tileTypes [2].name 		 = "Floor";
		tileTypes [2].isWalkable = true;
		tileTypes [2].prefab  	 = null;
		tileTypes [2].display 	 = true;
		tileTypes [2].color 	 = Color.grey;

		tileTypes [3].name 		 = "Wall";
		tileTypes [3].isWalkable = false;
		tileTypes [3].prefab 	 = null;
		tileTypes [3].display 	 = true;
		tileTypes [3].color 	 = Color.black;

		tileTypes [4].name 		 = "Torch";
		tileTypes [4].isWalkable = true;
        tileTypes [4].prefab     = Resources.Load("Prefabs/MapPrefabs/Torch_01_Prefab") as GameObject;
		tileTypes [4].display 	 = true;
        tileTypes [4].color      = Color.magenta;

        tileTypes [5].name 		 = "Map Goal Point";
		tileTypes [5].isWalkable = true;
		tileTypes [5].prefab 	 = null;
		tileTypes [5].display 	 = true;
		tileTypes [5].color 	 = Color.blue;

		tileTypes [6].name 		 = "Door";
		tileTypes [6].isWalkable = true;
        tileTypes [6].prefab     = null;
		tileTypes [6].display 	 = true;
        tileTypes [6].color 	 = Color.yellow;

        tileTypes[7].name        = "Barrel";
        tileTypes[7].isWalkable  = false;
        tileTypes[7].prefab      = Resources.Load("Prefabs/MapPrefabs/Barrel_01_Prefab") as GameObject;
        tileTypes[7].display     = true;
        tileTypes[7].color       = new Color( 0f, 1f, 1f );
    }
	
	void ProcessImage() {
		// use this when we have many maps, so we can randomize.
		// Texture2D tex = Resources.Load ("Level_"+Random.Range(0,levelCount)) as Texture2D;

		// Load the image in memory as a Texture2D
		Texture2D texture = Resources.Load ("Maps/Level_1") as Texture2D;

		mapDataTiles = new int[texture.width, texture.height]; // array with the size of the pixels

		// GetPixel32 returns an array of the pixel colors. The returned array is a flattened 2D array, where pixels are laid out left to right, bottom to top (i.e. row after row). 
		Color32[] arrayPixels = texture.GetPixels32();

		// Loop thru all the pixels in texture to generate the map data
		for (int i = 0; i < texture.width; i++) {
			for (int j = 0; j < texture.height; j++) {

				// The position of a pixel, one new pixel every loop
				Vector3 v3Position = new Vector3( i, 0.0f, j);

				// Color in format: RGBA(0.502, 0.502, 0.502, 1.000)
				Color color = arrayPixels[i + texture.width * j]; 

				GenerateMapData(v3Position, color);
            }
        }

        // Loop thru all the pixels in texture to place items on the map
        for (int i = 0; i < texture.width; i++) {
            for (int j = 0; j < texture.height; j++) {

                // The position of a pixel, one new pixel every loop
                Vector3 v3Position = new Vector3(i, 0.0f, j);

                // Color in format: RGBA(0.502, 0.502, 0.502, 1.000)
                Color color = arrayPixels[i + texture.width * j];

                // If the pixel color is white, then ignore it. ColorSqrDistanse(white, white) = 0,0;
                if (ColorSqrDistance(Color.black, color) > 0.1f)
                {
                    //PlaceItem(v3Position,color);
                    PlaceRotatedItem(v3Position, color);
                }
            }
        }

        PlaceRoatedDoors();

        if (generateFogOfWar){
			//Generate FogOfWar from fogofwar script (based on map size) 
			FogOfWarCreation fow = new FogOfWarCreation();
			fow.GenerateFogOfWar(texture.width, texture.height);
		}

		//Remove maptexture from memory
		Resources.UnloadAsset(texture);
		texture = null;
	}

	void GenerateMapData(Vector3 v3Position, Color color) {
		// Loop the tiletypes
		for (int i = 0; i < tileTypes.Length; i++) {
			// If the color of the tiletype match the color of the pixel (within the threshold).
			if (ColorSqrDistance(tileTypes[i].color, color) < 0.1f) {
				// fill the mapdatatiles array with the coordinates and the tiletype number.
				mapDataTiles[ (int)v3Position.x, (int)v3Position.z] = i;
			}
		}
	}

    void PlaceRoatedDoors() {
        for (int x = 0; x < mapDataTiles.GetLength(0); x++) {
            for (int z = 0; z < mapDataTiles.GetLength(1); z++) {
                //if door
                if (mapDataTiles[x,z] == 6 ) {
                    var left    = mapDataTiles[x - 1, z];
                    var right   = mapDataTiles[x + 1, z];
                    var up      = mapDataTiles[x, z + 1];
                    var down    = mapDataTiles[x, z - 1];

                    // Put the prefab in the scene and save its transform to a variable.
                    GameObject prefab = Instantiate(Resources.Load("Prefabs/MapPrefabs/Door_01_Prefab") ) as GameObject;

                    Transform t = prefab.transform;
                    
                    // if sourronding tiles is walls
                    if (left == 3 || right == 3) {
                        t.position    = new Vector3(x + .5f, 0, z);
                        t.eulerAngles = new Vector3(0, 90, 0);
                    }
                    if (up == 3 || down == 3) {
                        t.position    = new Vector3(x, 0, z + .5f);
                        t.eulerAngles = new Vector3(0,0,0);
                    }
                    else
                    {
                        //Debug.Log("Doors must be placed next to a wall");
                    }

                    t.parent = transform;  // This makes the prefab be a child of the mapController GameObjec
                }
            }
        }
    }

    void PlaceRotatedItem(Vector3 v3Position, Color color)
    {
        // Loop the tiletypes
        for (int i = 0; i < tileTypes.Length; i++) {

            // If the color of the tiletype match the color of the pixel (within the threshold).
            if (ColorSqrDistance(tileTypes[i].color, color) < 0.1f && tileTypes[i].prefab != null) {

                // Put the prefab in the scene and save its transform to a variable.
                GameObject prefab = Instantiate(tileTypes[i].prefab) as GameObject;

                Transform t = prefab.transform;
                t.position = v3Position; // Set the position of the prefab at the pixellocation.
                
                //Rotate the item if there is a wall next to it (make it face away from the wall)
                var left    = mapDataTiles[(int)v3Position.x - 1, (int)v3Position.z];
                var right   = mapDataTiles[(int)v3Position.x + 1, (int)v3Position.z];
                var up      = mapDataTiles[(int)v3Position.x, (int)v3Position.z + 1];
                var down    = mapDataTiles[(int)v3Position.x, (int)v3Position.z - 1];

                // if sourronding tiles is walls
                if (right   == 3) { t.eulerAngles = new Vector3(0, 0, 0); }
                if (left    == 3) { t.eulerAngles = new Vector3(0, 180, 0); }
                if (up      == 3) { t.eulerAngles = new Vector3(0, 270, 0); }
                if (down    == 3) { t.eulerAngles = new Vector3(0, 90, 0); }

                // if diagonal rotate and move out from wall
                if (right == 3 && up == 3) {
                    t.eulerAngles = new Vector3(0, 315, 0);
                    t.position = new Vector3( v3Position.x - .1f, v3Position.y, v3Position.z - .1f) ;
                }  // right top
                if (right == 3 && down == 3) {
                    t.eulerAngles = new Vector3(0, 45, 0);
                    t.position = new Vector3( v3Position.x - .1f, v3Position.y, v3Position.z + .1f) ;
                } // right bottom

                if (left == 3 && up == 3) {
                    t.eulerAngles = new Vector3(0, 225, 0);
                    t.position = new Vector3(v3Position.x + .1f, v3Position.y, v3Position.z - .1f);
                }   // left top
                if (left == 3 && down == 3) {
                    t.eulerAngles = new Vector3(0, 135, 0);
                    t.position = new Vector3(v3Position.x + .1f, v3Position.y, v3Position.z + .1f);
                } // left bottom

                // else { Debug.Log("Item is not placed near a wall and i dont care"); }

                //Debug.....
                //if (tileTypes[i].color == Color.magenta)
                //{
                //    var fu = v3Position.x + 1;
                //    var hej = GetTileType((int)v3Position.x +1, (int)v3Position.z);
                //    Debug.Log(hej.name + " float= " + v3Position.x + " int =" + (int)v3Position.x +  " fu = " + fu );
                //}

                t.parent = transform;  // This makes the prefab be a child of the mapController GameObject
                return;
            }
        }
        //Debug.Log ("Error, color not found");
    }



    // NOT IN USE ANYMORE 
    /* ***************************************************************** */
    void PlaceItem(Vector3 v3Position, Color color)
    {
        // Loop the tiletypes
        for (int i = 0; i < tileTypes.Length; i++)
        {

            // If the color of the tiletype match the color of the pixel (within the threshold).
            if (ColorSqrDistance(tileTypes[i].color, color) < 0.1f && tileTypes[i].prefab != null)
            {

                // Put the prefab in the scene and save its transform to a variable.
                GameObject prefab = Instantiate(tileTypes[i].prefab) as GameObject;

                Transform t = prefab.transform;
                t.position = v3Position; // Set the position of the prefab at the pixellocation.
                t.parent = transform;  // This makes the prefab be a child of the mapController GameObject
                return;
            }
        }
        //Debug.Log ("Error, color not found");
    }
    /* ***************************************************************** */


    // Comparing the color of the pixel in the texture and the color specified in the tileType
    // Looking at the distance between the two colors and considering them a match if the distance is below a threshold.
    float ColorSqrDistance(Color c1, Color c2) {
		return ((c2.r - c1.r) * (c2.r - c1.r) + (c2.b - c1.b) * (c2.b - c1.b) + (c2.g - c1.g) * (c2.g - c1.g));
	}

}