using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {

	public bool MakingChoice {
		get;
		set;
	}

	//1 = wait
	//2 = move
	//3 = attack
	//4 = ability
	public int ActionToTake {
		get;
		set;
	}

	public Vector3 StartPosition {
		get;
		set;
	}

	public Vector3 MoveTo {
		get;
		set;
	}

	// Use this for initialization
	void Start () 
	{
		MakingChoice = false;
		ActionToTake = 0;
		StartPosition = gameObject.transform.position;
		MoveTo = StartPosition;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void MakeChoice()
	{
			float choice = Random.value;
			
			if (choice < 0.25f)
			{
				Move("left");
			}
			else if (choice < 0.50f)
			{
				Move("right");
			}
			else if (choice < 0.75f)
			{
				Move("up");
			}
			else if (choice < 1f)
			{
				Move("down");
			}
		
	}

	void Move(string direction)
	{
		Vector3 Current = gameObject.transform.position;
		StartPosition = Current;

		switch (direction)
		{
			case "left":
				MoveTo = new Vector3(Current.x -1, Current.y, Current.z);
				break;

			case "right":
				MoveTo = new Vector3(Current.x +1, Current.y, Current.z);
				break;

			case "up":
				MoveTo = new Vector3(Current.x, Current.y +1, Current.z);
				break;

			case "down":
				MoveTo = new Vector3(Current.x, Current.y -1, Current.z);
				break;
		}

		Collider[] colliders = Physics.OverlapSphere(MoveTo,0f);
		
		if (colliders.Length > 0)
		{
			MoveTo = StartPosition;

			if (colliders[0].gameObject.tag == "Player")
			{
				Debug.Log("Rawr!");
			}
			else
			{
				bool[] validMoves = new bool[4];
				validMoves[0] = Physics.OverlapSphere(new Vector3(Current.x -1, Current.y, Current.z),0f).Length > 0;
				validMoves[1] = Physics.OverlapSphere(new Vector3(Current.x +1, Current.y, Current.z),0f).Length > 0;
				validMoves[2] = Physics.OverlapSphere(new Vector3(Current.x, Current.y -1, Current.z),0f).Length > 0;
				validMoves[3] = Physics.OverlapSphere(new Vector3(Current.x, Current.y +1, Current.z),0f).Length > 0;

				for (int i = 0; i < validMoves.Length; i++) 
				{
					if (!validMoves[i])
					{
						MakeChoice();
						break;
					}
				}
			}
		}
	}
}












