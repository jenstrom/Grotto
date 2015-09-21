using UnityEngine;
using System.Collections;

public class MapMouseOver : MonoBehaviour {

	Vector3 currentTileCoord;

	public GameObject selection = Resources.Load ("Prefabs/MapPrefabs/MousePositionPrefab") as GameObject;

	//public MapController mapController;

	void Awake () {
	
		selection = Instantiate(selection) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;
		
		if( GetComponent<Collider>().Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
			int x = Mathf.FloorToInt( hitInfo.point.x );
			int z = Mathf.FloorToInt( hitInfo.point.z );
			//Debug.Log ("Tile: " + x + ", " + z);

			currentTileCoord.x = x + 0.5f;
			currentTileCoord.z = z + 0.5f;

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
