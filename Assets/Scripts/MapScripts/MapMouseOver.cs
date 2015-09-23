using UnityEngine;
using System.Collections;

public class MapMouseOver : MonoBehaviour {
    Vector3 currentTileCoord;

	public GameObject selection = Resources.Load ("Prefabs/MapPrefabs/MousePositionPrefab") as GameObject;

	void Awake () {
	
		selection = Instantiate(selection) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
        // OFFSET THE MOUSE POINTER (so it is center over the tile-"selection" 
        // the 0,0 is bottom left screen pixels so the screen resoluton does mather... need to build/check diffrently
        /* This is how i must do it.. to compensate for the fact that the grid dont have an offset of 0.5 in xy */
        int sHight = Screen.height / 18 / 2; //the offset in pixels 
        int sWidth = Screen.width / 34 / 2; //need to calculate it like this to match diffrent screen resolutions (this need to be tested)
        Vector3 mouse = new Vector3(Input.mousePosition.x + sHight, Input.mousePosition.y + sWidth, Input.mousePosition.z);
        Ray ray = Camera.main.ScreenPointToRay(mouse);

        /* This is how u normaly would do it... */
        //Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

        RaycastHit hitInfo;
		
		if( GetComponent<Collider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
			int x = Mathf.FloorToInt( hitInfo.point.x );
			int z = Mathf.FloorToInt( hitInfo.point.z );
			//Debug.Log ("Tile: " + x + ", " + z);

			currentTileCoord.x = x;
			currentTileCoord.z = z;

			selection.SetActive(true);

			GameObject map = GameObject.Find("MapController");
			MapController mapController = map.GetComponent<MapController>();


			// If the tile is walkable. 
			if( mapController.GetTileType(x,z).isWalkable )
			{
				selection.transform.position = currentTileCoord;
			}
			else {
				// Hide selection cube
				selection.SetActive(false);
			}
		
		}
		else {
			// Hide selection cube
			selection.SetActive(false);
		}
		
		if(Input.GetMouseButtonDown(0)) {
			//Debug.Log ("Click!");
		}
	}
}
