using UnityEngine;
using System.Collections;

/*
 * This Script gets initialized in the MapController 
 * 
 * 
 * It Creates 2 orthographic Cameras that render 2 textures.
 * 
 * The textures are then applayed to 2 planes that are also created in this script.
 * 
 * A 3ed plane gets created and transformed to a child of the playerObject
 * 
 * That plaine have a texture applayed that acts lika a brusch to paint on the rendertexture.
 * 
 * To get all this to work you also need a custom shader (not created in this script) 
 * and u need to use "layers" to set which camera sees what.
 * 
 */

public class FogOfWarCreation {

	public void  GenerateFogOfWar(int mapSizeX, int mapSizeZ){

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
		fogOfWarTexture.name 			= "FogOfWarRenderTexture";
		fogOfWarTexture.anisoLevel 		= 0;
		
		// Assign the texture to the cameraOutput.
		fogCamComponent.targetTexture = fogOfWarTexture;
		
		GenerateFogOfWarPlane(mapSizeX, mapSizeZ, fogOfWarTexture);
		
		//Generate second camera
		GenerateFogOfWar2(mapSizeX, mapSizeZ);
	}
	
	/* Need refacoring, do not repeat code ***********************************************************/
	void GenerateFogOfWar2(int mapSizeX, int mapSizeZ){
		
		// Creates the Camera that Render the blendmaterial to a texture
		
		GameObject fogCam 	= new GameObject();
		fogCam.name 		= "FogOfWarCam2";
		
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
		fogCamComponent.clearFlags 			= UnityEngine.CameraClearFlags.SolidColor;
		// make the camera orthographic
		fogCamComponent.orthographic 		= true;
		// The camera must cover the whole map
		fogCamComponent.orthographicSize 	= mapSizeZ / 2;
		
		
		// Create the texture that the camera should render to.
		RenderTexture fogOfWarTexture 	= new RenderTexture( mapSizeX, mapSizeZ, 0);
		fogOfWarTexture.name 			= "FogOfWarRenderTexture2";
		fogOfWarTexture.anisoLevel 		= 0;
		
		// Assign the texture to the cameraOutput.
		fogCamComponent.targetTexture = fogOfWarTexture;
		
		
		//Generate Second Plane
		GenerateFogOfWarPlane2(mapSizeX, mapSizeZ, fogOfWarTexture);
	}
	
	
	void GenerateFogOfWarPlane2(int mapWidth, int mapHeight, RenderTexture fogOfWarTexture){
		
		// Create a plane that has a blendmaterial, that is the fog
		
		Mesh mesh 		 	= new Mesh();  		// The mesh (verticies, uvs, triangels).
		GameObject FogOfWar = new GameObject();	// The GameObject the mesh should be attached to.
		
		FogOfWar.name 	 = "FogOfWarPlane2";		// Name of the GameObject that appears in the "hierarchy list"
		mesh.name 		 = "FogOfWarMesh2" ;  	// Name of the mesh, appears in the inspector under meshRenderer (if u select the Gameobject floor)
		
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
		FogOfWar.transform.position = new Vector3(0, 3 + 0.4f, 0);
		
		// add the meshrenderer component to the gameObject. (and store the component in a variable)
		MeshRenderer renderer = FogOfWar.AddComponent<MeshRenderer>();
		// turn of casting and receveing Shadows on the MeshRenderer
		renderer.receiveShadows = false;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		// add our custom shader to the meshRender component (not created in this script)
		//renderer.material.shader = Shader.Find ("Custom/FogOfWarMaskTransparent");
		
		renderer.material = Resources.Load ("Materials/FogOfWarMaterials/SemiTransparentMaterial") as Material;

		// Assign the texture we created as a cameraOutput.
		renderer.material.mainTexture = fogOfWarTexture;
	}
	/************************************************************/

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
		
		// Create a plane that masks out what should be visible in the blendmaterial (fog)
		
		int planeSize 		= 18;
		
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
