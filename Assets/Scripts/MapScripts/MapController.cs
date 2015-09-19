using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {
	
	public int numberOfLevels   = 2;				//How many map texures do we have
	public TileType[] tileTypes = new TileType[7];	//Holds the.. you guessed it, tiletypes.

	public int[,] mapDataTiles;						//2d Array to hold all the tiles for the map. // coordinates and index number for the tiletype

	
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

		tileTypes [4].name 		 = "Trap_1 SpawnPoint";
		tileTypes [4].isWalkable = true;
		tileTypes [4].prefab 	 = null;
		tileTypes [4].display 	 = true;
		tileTypes [4].color 	 = Color.yellow;

		tileTypes [5].name 		 = "Map Goal Point";
		tileTypes [5].isWalkable = true;
		tileTypes [5].prefab 	 = null;
		tileTypes [5].display 	 = true;
		tileTypes [5].color 	 = Color.blue;

		tileTypes [6].name 		 = "Healthpotion SpawnPoint";
		tileTypes [6].isWalkable = true;
		tileTypes [6].prefab 	 = Resources.Load ("Prefabs/MapPrefabs/FloorPrefab") as GameObject;
		tileTypes [6].display 	 = true;
		tileTypes [6].color 	 = Color.magenta;
	}
	
	void ProcessImage() {
		// use this when we have many maps, so we can randomize.
		// Texture2D tex = Resources.Load ("Level_"+Random.Range(0,levelCount)) as Texture2D;

		// Load the image in memory as a Texture2D
		Texture2D texture = Resources.Load ("Maps/Level_1") as Texture2D;

		mapDataTiles = new int[texture.width, texture.height]; // array with the size of the pixels

		// GetPixel32 returns an array of the pixel colors. The returned array is a flattened 2D array, where pixels are laid out left to right, bottom to top (i.e. row after row). 
		Color32[] arrayPixels = texture.GetPixels32();

		// Loop thru all the pixels in texture
		for (int i = 0; i < texture.width; i++) {
			for (int j = 0; j < texture.height; j++) {

				// The position of a pixel, one new pixel every loop
				Vector3 v3Position = new Vector3( 0.5f + i, 0.0f, 0.5f + j);

				// Color in format: RGBA(0.502, 0.502, 0.502, 1.000)
				Color color = arrayPixels[i + texture.width * j]; 

				GenerateMapData(v3Position, color);

				// If the pixel color is white, then ignore it. ColorSqrDistanse(white, white) = 0,0;
				if (ColorSqrDistance(Color.black, color) > 0.1f) {
					PlaceItem(v3Position,color);
				}
			}
		}

		//******************************** Under construction
		GenerateFogOfWar(texture.width, texture.height);
		//***************************************************

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
	
	void PlaceItem(Vector3 v3Position, Color color) {
		// Loop the tiletypes
		for (int i = 0; i < tileTypes.Length; i++) {

			// If the color of the tiletype match the color of the pixel (within the threshold).
			if (ColorSqrDistance(tileTypes[i].color, color) < 0.1f && tileTypes[i].prefab != null ) {
			
				// Put the prefab in the scene and save its transform to a variable.
				GameObject prefab = Instantiate(tileTypes[i].prefab) as GameObject;

				Transform t = prefab.transform;
				t.position  = v3Position; // Set the position of the prefab at the pixellocation.
				t.parent    = transform;  // This makes the prefab be a child of the mapController GameObject
				return;
			}
		}
		//Debug.Log ("Error, color not found");
	}
	
	// Comparing the color of the pixel in the texture and the color specified in the tileType
	// Looking at the distance between the two colors and considering them a match if the distance is below a threshold.
	float ColorSqrDistance(Color c1, Color c2) {
		return ((c2.r - c1.r) * (c2.r - c1.r) + (c2.b - c1.b) * (c2.b - c1.b) + (c2.g - c1.g) * (c2.g - c1.g));
	}






	/* *************************************  FOG OF WAR CREATION ************************************* */
	void GenerateFogOfWar(int mapSizeX, int mapSizeZ){

		// Creates the Camera that Render the blendmaterial to a texture

		GameObject fogCam 	= new GameObject();
		fogCam.name 		= "FogOfWarCam";
		
		// The Y value just needs to be higher than the terrain.. the camera is orthographic.
		fogCam.transform.position    = new Vector3(mapSizeX / 2, 50, mapSizeZ / 2);
		// Rotate the camera so it looks down
		fogCam.transform.eulerAngles = new Vector3(90, 0, 0);
		// add the gameObject to a layer (layer not created in script)
		fogCam.layer = 8; //FogOfWarLayer
		
		// Add a cameraComponent to the gameObject
		Camera fogCamComponent = fogCam.AddComponent<Camera>();
		
		// The camera should only see things in the FogOfWarLayer
		fogCamComponent.cullingMask     	= (1 << LayerMask.NameToLayer("FogOfWarLayer"));
		// ..
		fogCamComponent.clearFlags 		= UnityEngine.CameraClearFlags.Depth;
		// make the camera orthographic
		fogCamComponent.orthographic 		= true;
		// The camera must cover the whole map
		fogCamComponent.orthographicSize 	= mapSizeZ / 2;
		

		// Create the texture that the camera should render to.
		RenderTexture fogOfWarTexture 	= new RenderTexture( mapSizeX, mapSizeZ, 0);
		fogOfWarTexture.name 			= "FogOfWarTexture";
		fogOfWarTexture.anisoLevel 		= 0;

		// Assign the texture to the cameraOutput.
		fogCamComponent.targetTexture = fogOfWarTexture;
		

		GenerateFogOfWarPlane(mapSizeX, mapSizeZ, fogOfWarTexture);
	}

	void GenerateFogOfWarPlane(int mapWidth, int mapHeight, RenderTexture fogOfWarTexture){

		// Create a plane that has a blendmaterial, that is the fog

		Mesh mesh 		 	= new Mesh();  		// The mesh (verticies, uvs, triangels).
		GameObject FogOfWar = new GameObject();	// The GameObject the mesh should be attached to.

		FogOfWar.name 	 = "FogOfWarPlane";		// Name of the GameObject that appears in the "hierarchy list"
		mesh.name 		 = "FogOfWarMesh" ;  	// Name of the mesh, appears in the inspector under meshRenderer (if u select the Gameobject floor)

		// Assign the possitions of the verticies with a array of vector3's;
		mesh.vertices 	 = new Vector3[] { 
										  new Vector3(    	 0, 0,	       0), 	// bottom left
										  new Vector3(    	 0, 0, mapHeight), 	// top left
										  new Vector3(mapWidth, 0, mapHeight),	// top right
										  new Vector3(mapWidth, 0,   	   0)	// bottom right
										};
		// Assign the texture coordinates.
		mesh.uv 		= new Vector2[] {
										  new Vector2(0, 0), 
										  new Vector2(0, 1),
										  new Vector2(1, 1), 
										  new Vector2(1, 0)
										};
		// Generate the triangels (2) from positions of the vertices 
		mesh.triangles 	= new int[] { 0, 1, 2, 0, 2, 3};
		// add the meshfilter to the gameobject and assign the mesh we just created.
		FogOfWar.AddComponent<MeshFilter>().mesh = mesh;
		// the Y = 3,5f is to place it above the terrain 
		FogOfWar.transform.position = new Vector3(0, 3 + 0.5f, 0);

		// add the meshrenderer component to the gameObject. (and store the component in a variable)
		MeshRenderer renderer = FogOfWar.AddComponent<MeshRenderer>();
		// turn of casting and receveing Shadows on the MeshRenderer
		renderer.receiveShadows = false;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		// add our custom shader to the meshRender component (not created in this script)
		renderer.material.shader = Shader.Find ("Custom/FogOfWarMask");
		// Assign the texture we created as a cameraOutput.
		renderer.material.mainTexture = fogOfWarTexture;

		GenerateFogOfWarSeeThroughPlane();
	}

	void GenerateFogOfWarSeeThroughPlane() {

		// Create a plane thet masks out what should be visible in the blendmaterial (fog)

		int planeSize 		= 10;

		Mesh mesh 		 	= new Mesh();  	// The mesh (verticies, uvs, triangels).
		GameObject FogOfWarSeeThroughPlane = new GameObject();// The GameObject the mesh should be attached to.
		
		FogOfWarSeeThroughPlane.name 	= "FogOfWarSeeThroughPlane";	// Name of the GameObject that appears in the "hierarchy list"
		mesh.name 		 				= "FogOfWarSeeThroughMesh" ;	// Name of the mesh, appears in the inspector under meshRenderer (if u select the Gameobject floor)
		
		// Assign the possitions of the verticies with a array of vector3's;
		mesh.vertices 	 = new Vector3[] { 
			new Vector3(    	0, 0,	      0), 	// bottom left
			new Vector3(    	0, 0, planeSize), 	// top left
			new Vector3(planeSize, 0, planeSize),	// top right
			new Vector3(planeSize, 0,   	  0)	// bottom right
		};
		// Assign the texture coordinates.
		mesh.uv 		= new Vector2[] {
			new Vector2(0, 0), 
			new Vector2(0, 1),
			new Vector2(1, 1), 
			new Vector2(1, 0)
		};
		// Generate the triangels (2) from positions of the vertices 
		mesh.triangles 	= new int[] { 0, 1, 2, 0, 2, 3};
		
		// add the meshfilter to the gameobject and assign the mesh we just created.
		FogOfWarSeeThroughPlane.AddComponent<MeshFilter>().mesh = mesh;

		//Find the player object and use it as parent to this gameobject
		GameObject player = GameObject.Find("Player");
		FogOfWarSeeThroughPlane.transform.parent = player.transform;
		// the Y = 3,2f is to place it above the terrain but below the fogofwarplane. Center the plane over the player.
		FogOfWarSeeThroughPlane.transform.position = new Vector3( player.transform.position.x - (planeSize/2), 3 + 0.2f, player.transform.position.z - (planeSize/2) );

		// add the gameobject to a layer
		FogOfWarSeeThroughPlane.layer = LayerMask.NameToLayer("FogOfWarLayer");

		// add the meshrenderer component to the gameObject. (and store the component in a variable)
		MeshRenderer renderer = FogOfWarSeeThroughPlane.AddComponent<MeshRenderer>();
		// turn of casting and receveing Shadows on the MeshRenderer
		renderer.receiveShadows = false;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		// add the material 
		renderer.material = Resources.Load ("Materials/FogOfWarMaterials/MaskMaterial") as Material;
	}



}