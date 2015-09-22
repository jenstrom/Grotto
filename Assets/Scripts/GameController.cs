using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {


	public GameObject[] enemyPrefabs;
	private PlayerController player;
	private MapController map;
	private List<EnemyAI> activeEnemies = new List<EnemyAI>();
	private GameObject recentlyInstantiated;

	public float speed = 0.05f;
	private float distanceCovered;

	// Use this for initialization
	void Start () 
	{
		player = (PlayerController) FindObjectOfType (typeof(PlayerController));
		map = (MapController) FindObjectOfType (typeof(MapController));
		distanceCovered = 0f;
		player.PlayerInputAllowed = true;


		SpawnEnemy();

	}

	void Update () 
	{


		if (player.ActionTaken)
		{
			player.ActionTaken = false;

			for(int i = 0; i < activeEnemies.Count; i++)
			{
				activeEnemies[i].MakeChoice();
			}
		}

		if (player.ActionToTake != 0)
		{
			distanceCovered += speed;

			switch (player.ActionToTake)
			{
				//Player waiting
			case 1:
				if (EnemyMovement())
				{
					return;
				}
				break;

				//Player Moving
			case 2:
				bool playerMoving = PlayerMovement();
				bool enemyMoving = EnemyMovement();
				if (playerMoving || enemyMoving)
				{
					return;
				}
				break;

				//Player Attacking
			case 3:
				// Player Attack
				if (EnemyMovement())
				{
					return;
				}
				break;
			}
			distanceCovered = 0f;
			player.ActionToTake = 0;
			player.PlayerInputAllowed = true;
		}
	}



	bool PlayerMovement()
	{
		Vector3 startPosition = player.StartPosition;
		Vector3 goalPosition = player.MoveTo.transform.position;
		
		if (player.gameObject.transform.position != goalPosition)
		{
			player.gameObject.transform.position = Vector3.Lerp(startPosition, goalPosition, distanceCovered);
			return true;
		}
		return false;
	}

	bool EnemyMovement()
	{
		Vector3 startPosition;
		Vector3 goalPosition;
		bool stillMoving = false;

		for (int i = 0; i < activeEnemies.Count; i++) 
		{
			startPosition = activeEnemies[i].StartPosition;
			goalPosition = activeEnemies[i].MoveTo;

			if (activeEnemies[i].gameObject.transform.position != goalPosition)
			{
				activeEnemies[i].gameObject.transform.position = Vector3.Lerp(startPosition, goalPosition, distanceCovered);
				stillMoving = true;
			}
		}
		return stillMoving;
	}

	void SpawnEnemy()
	{
		List<Vector3> spawnPoints =  map.GetCoordinatesOfTileTypes(0);
		GameObject enemy = new GameObject();

		for (int i = 0; i < spawnPoints.Count; i++) 
		{
			spawnPoints[i] = new Vector3(spawnPoints[i].x, 0.5f, spawnPoints[i].z);
			enemy = (GameObject) Instantiate(enemyPrefabs[0], spawnPoints[i], Quaternion.identity);
			activeEnemies.Add(enemy.GetComponent<EnemyAI>());
		}

	}
	

}
