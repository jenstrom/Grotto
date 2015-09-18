using UnityEngine;
using System.Collections;

[System.Serializable]				//system.serialize makes custom classes to be able to show up in the editor. (also needs to be public)
public class TileType {
	 
	public string name;				//Name of this tileType. ex: Grass,Stone.
	public GameObject prefab;		//A prefab object that represent this tileType.
	public Color color;				//The color on the maptexture that represents this tiletype

	public bool display    = true;

	public bool isWalkable = true; 	//Are a player allowed to walk on the tile (default = true)
}






