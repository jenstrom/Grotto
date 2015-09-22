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
		if (Input.GetButtonUp("Fire1"))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(ray, out hit);
			
			Collider[] colliders = Physics.OverlapSphere(hit.transform.position, 0);
			
			Vector3[] arrayOfMoves = new Vector3[0];
			
			if (colliders.Length == 1 && colliders[0].gameObject.tag == "Floor")
			{
				arrayOfMoves = PathFinder.GetMoveArray(gameObject.transform.position, hit.transform.position);
			}
			
			if (arrayOfMoves.Length > 0)
			{
				for (int i = 0; i < arrayOfMoves.Length-1; i++) 
				{
					Debug.DrawLine(arrayOfMoves[i], arrayOfMoves[i+1], Color.green, 5, false);
				}

			}
			
			
		}
	}

	public void MakeChoice()
	{
		Vector3 playerPosition = Vector3.zero;
		bool aggro = false;
		Vector3 Current = gameObject.transform.position;
		StartPosition = Current;

		Collider[] aggroRadar;
		aggroRadar = Physics.OverlapSphere(StartPosition, 5);

		for (int i = 0; i < aggroRadar.Length; i++) 
		{
			if (aggroRadar[i].tag == "PlayerCollider")
			{
				RaycastHit hit;
				Physics.Linecast(StartPosition, aggroRadar[i].gameObject.transform.position, out hit);

				if (hit.collider.gameObject.tag == "PlayerCollider")
				{
					Debug.Log("Oi!");
					aggro = true;
					playerPosition = aggroRadar[i].gameObject.transform.position;
					gameObject.GetComponent<MeshRenderer>().material.color = new Color(255,255,255);
					break;
				}
			}
			else
			{
				gameObject.GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
				aggro = false;
			}
		}

		if (aggro)
		{
			playerPosition.y = 0f;
			Vector3[] moveArray = new Vector3[1];
			moveArray = PathFinder.GetMoveArray(Current, playerPosition);
			MoveTo = moveArray[0];
			MoveTo = new Vector3(MoveTo.x, 0.5f, MoveTo.z);
			if (Physics.OverlapSphere(MoveTo, 0).Length > 0)
			{
				MoveTo = Current;
				Debug.Log("Rawr!");
			}
		}
		else
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
	}

	void Move(string direction)
	{
		Vector3 Current = StartPosition;

		switch (direction)
		{
			case "left":
				MoveTo = Current += Vector3.left;
//				MoveTo = new Vector3(Current.x -1, Current.y, Current.z);
				break;

			case "right":
				MoveTo = Current += Vector3.right;
//				MoveTo = new Vector3(Current.x +1, Current.y, Current.z);
				break;

			case "up":
				MoveTo = Current += Vector3.forward;
//				MoveTo = new Vector3(Current.x, Current.y +1, Current.z);
				break;

			case "down":
				MoveTo = Current += Vector3.back;
//				MoveTo = new Vector3(Current.x, Current.y -1, Current.z);
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
				validMoves[2] = Physics.OverlapSphere(new Vector3(Current.x, Current.y, Current.z -1),0f).Length > 0;
				validMoves[3] = Physics.OverlapSphere(new Vector3(Current.x, Current.y, Current.z +1),0f).Length > 0;

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












