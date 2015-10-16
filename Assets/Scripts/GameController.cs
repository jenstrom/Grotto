using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public GameObject[] enemyPrefabs;
	private PlayerController player;
	private MapController map;
	private List<EnemyAI> activeEnemies = new List<EnemyAI>();

	public float moveSpeed = 0.05f;
    public float rotationSpeed = 0.1f;

	// Use this for initialization
	void Start () 
	{
		player = (PlayerController) FindObjectOfType (typeof(PlayerController));
		map = (MapController) FindObjectOfType (typeof(MapController));
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
            player.resetCollider();

        }

        if (player.ActionToTake != PlayerAction.unset)
		{

			switch (player.ActionToTake)
			{
			case PlayerAction.wait:
				if (PlayerWaiting())
				{
					return;
				}
				break;
                    
			case PlayerAction.move:
				if (PlayerMoving())
				{
					return;
				}
				break;
                    
			case PlayerAction.attack:
				if (PlayerAttacking())
				{
					return;
				}
				break;
			}
			player.ActionToTake = PlayerAction.unset;
			player.PlayerInputAllowed = true;
		}
	}

    bool PlayerWaiting()
    {
        //bool enemyRotated = EnemyRotation();
        bool enemyRotated = true;

        if (enemyRotated)
        {
            bool enemyMoved = EnemyMovement();
            bool enemyAttacked = EnemyAttack();

            if (enemyMoved && enemyAttacked)
            {
                return false;
            }
        }
        return true;
    }

    bool PlayerMoving()
    {
        bool playerRotated = PlayerRotation();
        bool enemyRotated = EnemyRotation();
        //bool enemyRotated = true;

        if (playerRotated && enemyRotated)
        {
            bool playerMoved = PlayerMovement();
            bool enemyMoved = EnemyMovement();

            if (playerMoved && enemyMoved)
            {
                bool enemyAttacked = EnemyAttack();

                if (enemyAttacked)
                {
                    return false;
                }
            }
        }
        return true;
    }

    bool PlayerAttacking()
    {
        bool playerRotated = PlayerRotation();

        if (PlayerRotation())
        {
            bool playerAttacked = PlayerAttack();

            if (playerAttacked)
            {
                //bool enemyRotated = EnemyRotation();
                bool enemyRotated = true;

                if (enemyRotated)
                {
                    bool enemyMoved = EnemyMovement();
                    bool enemyAttacked = EnemyAttack();

                    if (enemyMoved && enemyAttacked)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    bool PlayerRotation()
    { 
        Vector3 currentPosition = player.gameObject.transform.position;
        Vector3 goalPosition = player.MoveTo;

        if (player.gameObject.transform.rotation != Quaternion.LookRotation(player.MoveTo - player.MoveFrom))
        {
            Vector3 newDir = Vector3.RotateTowards(player.gameObject.transform.forward, player.MoveTo - player.MoveFrom, rotationSpeed, 0.0F);
            player.gameObject.transform.rotation = Quaternion.LookRotation(newDir);
            return false;
        }
        return true;
    }

    bool EnemyRotation()
    {
        Vector3 currentPosition;
        Vector3 goalPosition;
        bool rotationComplete = true;

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            currentPosition = activeEnemies[i].gameObject.transform.position;
            goalPosition = activeEnemies[i].MoveTo;

            if (activeEnemies[i].gameObject.transform.rotation != Quaternion.LookRotation(activeEnemies[i].MoveTo - activeEnemies[i].MoveFrom)) 
            {
                Vector3 newDir = Vector3.RotateTowards(activeEnemies[i].gameObject.transform.forward, activeEnemies[i].MoveTo - activeEnemies[i].MoveFrom, rotationSpeed, 0.0F);
                activeEnemies[i].gameObject.transform.rotation = Quaternion.LookRotation(newDir);
                rotationComplete = false;
            }
        }
        return rotationComplete;
    }

    bool PlayerMovement()
	{
        Vector3 currentPosition = player.gameObject.transform.position;
        Vector3 goalPosition = player.MoveTo;

        if (player.gameObject.transform.position != player.MoveTo)
		{
            float step = moveSpeed * Time.deltaTime;
			player.gameObject.transform.position = Vector3.MoveTowards(currentPosition, goalPosition, moveSpeed);
			return false;
		}
		return true;
	}

	bool EnemyMovement()
	{
		Vector3 currentPosition;
		Vector3 goalPosition;
		bool moveComplete = true;

		for (int i = 0; i < activeEnemies.Count; i++) 
		{
            currentPosition = activeEnemies[i].gameObject.transform.position;
            goalPosition = activeEnemies[i].MoveTo;

            if (activeEnemies[i].gameObject.transform.position != goalPosition)
			{
                float step = moveSpeed * Time.deltaTime;
				activeEnemies[i].gameObject.transform.position = Vector3.MoveTowards(currentPosition, goalPosition, moveSpeed);
				moveComplete = false;
			}
		}
		return moveComplete;
	}

    bool PlayerAttack()
    {
        return true;
    }

    bool EnemyAttack()
    {
        return true;
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
