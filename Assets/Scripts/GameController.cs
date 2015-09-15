using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {


	public GameObject[] enemyPrefabs;
	private PlayerController player;
	private TileMapController map;
	private List<EnemyAI> activeEnemies = new List<EnemyAI>();
	private GameObject recentlyInstantiated;

	public float speed = 0.05f;
	private float distanceCovered;

	// Use this for initialization
	void Start () 
	{
		player = (PlayerController) FindObjectOfType (typeof(PlayerController));
		map = (TileMapController) FindObjectOfType (typeof(TileMapController));
		distanceCovered = 0f;
		player.PlayerInputAllowed = true;



		recentlyInstantiated = (GameObject) Instantiate(enemyPrefabs[0], new Vector3(-7f, 4f, -0.5f), Quaternion.identity);
		activeEnemies.Add(recentlyInstantiated.GetComponent<EnemyAI>());

		recentlyInstantiated = (GameObject) Instantiate(enemyPrefabs[0], new Vector3(3f, 7f, -0.5f), Quaternion.identity);
		activeEnemies.Add(recentlyInstantiated.GetComponent<EnemyAI>());

		recentlyInstantiated = (GameObject) Instantiate(enemyPrefabs[0], new Vector3(-9f, -7f, -0.5f), Quaternion.identity);
		activeEnemies.Add(recentlyInstantiated.GetComponent<EnemyAI>());

		recentlyInstantiated = (GameObject) Instantiate(enemyPrefabs[0], new Vector3(8f, -6f, -0.5f), Quaternion.identity);
		activeEnemies.Add(recentlyInstantiated.GetComponent<EnemyAI>());




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

		int x = Random.Range(0, map.MapLayout.GetLength(0));
		int y = Random.Range(0, map.MapLayout.GetLength(1));
		GameObject enemy = (GameObject) Instantiate(enemyPrefabs[0], new Vector3(-map.MapLayout.GetLength(0)/2 + x, -map.MapLayout.GetLength(1)/2 + y, -0.5f), Quaternion.identity);
		activeEnemies.Add(enemy.GetComponent<EnemyAI>());
	}
	

}
