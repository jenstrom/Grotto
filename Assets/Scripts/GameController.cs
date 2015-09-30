using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {


	public GameObject[] enemyPrefabs;
	private PlayerController player;
	private MapController map;
	private List<EnemyAI> activeEnemies = new List<EnemyAI>();

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

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                activeEnemies[i].resetCollider();
            }
        }

		if (player.ActionToTake != PlayerAction.unset)
		{
			distanceCovered += speed;

			switch (player.ActionToTake)
			{
			case PlayerAction.wait:
				if (EnemyMovement())
				{
					return;
				}
				break;
                    
			case PlayerAction.move:
				bool playerMoving = PlayerMovement();
				bool enemyMoving = EnemyMovement();
				if (playerMoving || enemyMoving)
				{
					return;
				}
				break;
                    
			case PlayerAction.attack:
				// Player Attack
				if (EnemyMovement())
				{
					return;
				}
				break;
			}
			distanceCovered = 0f;
			player.ActionToTake = PlayerAction.unset;
            player.resetCollider();
			player.PlayerInputAllowed = true;
		}
	}



	bool PlayerMovement()
	{
		Vector3 startPosition = player.MoveFrom;
		Vector3 goalPosition = player.MoveTo;
		
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
			startPosition = activeEnemies[i].MoveFrom;
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

		for (int i = 0; i < spawnPoints.Count; i++) 
		{
			spawnPoints[i] = new Vector3(spawnPoints[i].x, 0.5f, spawnPoints[i].z);
			var enemy = (GameObject) Instantiate(enemyPrefabs[0], spawnPoints[i], Quaternion.identity);
			activeEnemies.Add(enemy.GetComponent<EnemyAI>());
		}

	}
	

}
