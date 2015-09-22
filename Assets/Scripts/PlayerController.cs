using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
{
	//public GameObject playerCollider;
	private Vector3[] moveQueue;
	private int moveQueueIndex;

	public GameObject MoveTo {
		get;
		set;
	}

	public Vector3 StartPosition {
		get;
		set;
	}

	public bool PlayerInputAllowed {
		get;
		set;
	}

	public bool ActionTaken {
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

	void Start () 
	{
		ActionToTake = 0;
		ActionTaken = false;
		StartPosition = gameObject.transform.position;
		MoveTo = (GameObject) Instantiate(new GameObject(), StartPosition, Quaternion.identity);
		MoveTo.AddComponent<BoxCollider>();
		MoveTo.tag = "PlayerCollider";
		moveQueue = new Vector3[0];
		moveQueueIndex = 0;
		
	}

	void Update () 
	{

		if (Input.GetButtonUp("Fire1"))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(ray, out hit);
			
			Collider[] colliders = Physics.OverlapSphere(hit.transform.position, 0);

			if (colliders.Length == 1 && colliders[0].gameObject.tag == "Floor")
			{
				moveQueue = PathFinder.GetMoveArray(MoveTo.transform.position, hit.transform.position);
				moveQueueIndex = 0;
			}

			if (moveQueue.Length > 0)
			{
				for (int i = 0; i < moveQueue.Length-1; i++) 
				{
					Debug.DrawLine(moveQueue[i], moveQueue[i+1], Color.green, 5, false);
				}

				foreach (var move in moveQueue) {
					Debug.Log(move);
				}
			}
		}

		if (Input.GetButtonUp("Jump"))
		{
			moveQueue = new Vector3[0];
			moveQueueIndex = 0;
		}

		if (PlayerInputAllowed)
		{

			if (moveQueue.Length > 0 && moveQueueIndex < moveQueue.Length)
			{
				PlayerInputAllowed = false;
				moveQueue[moveQueueIndex] = new Vector3(moveQueue[moveQueueIndex].x, 0.5f, moveQueue[moveQueueIndex].z);
				PlayerAction(moveQueue[moveQueueIndex]);
				moveQueueIndex++;
				ActionToTake = 2;
				ActionTaken = true;
			}

			if (Input.GetButton("Horizontal") || Input.GetButton("Vertical") || Input.GetButton("Jump"))
			{
				PlayerInput();

				/*
				if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
				{
					if (Input.GetAxis("Horizontal") < 0)
					{
						MoveTo = new Vector3(Current.x -1, Current.y, Current.z);
					}
					
					if (Input.GetAxis("Horizontal") > 0)
					{
						MoveTo = new Vector3(Current.x +1, Current.y, Current.z);
					}
					
					if (Input.GetAxis("Vertical") < 0)
					{
						MoveTo = new Vector3(Current.x, Current.y -1, Current.z);
					}
					
					if (Input.GetAxis("Vertical") > 0)
					{
						MoveTo = new Vector3(Current.x, Current.y +1, Current.z);
					}

				}

				PlayerInputAllowed = false;
				Collider[] colliders = Physics.OverlapSphere(MoveTo,0.4f);
				bool occupied = false;

				for (int i = 0; i < colliders.Length; i++) {
					if (!colliders[i].CompareTag("Floor"))
					{
						Debug.Log(colliders[i].tag);
						MoveTo = Current;
						occupied = true;
					}
				}

				if (!occupied)
				{
					ActionToTake = 2;
					ActionTaken = true;
					MoveToObject = (GameObject) Instantiate(moveToCollider, MoveTo, Quaternion.identity);
					Debug.Log(MoveToObject.transform.position);
				}
				*/
			}
		}
		/*
		if (gameObject.transform.position != MoveTo)
			{
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, MoveTo, 0.1f);
			}
			else
			{
			current = MoveTo;
				PerformingAction = false;
			}

		/*

		if (Input.GetMouseButtonUp(0))
		{
			current = gameObject.transform.position;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			Physics.Raycast(ray, out hit);

			moveQueue = new Vector3[] {hit.point};

			//PlayerMove(moveQueue);
		}

		if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
		{
			moveQueue = new Vector3[] {GetKeyboardMove(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))};
		}
		//PlayerMove(moveQueue);
		if (gameObject.transform.position != moveQueue[0])
		{
			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, moveQueue[0], 1f);
		}

		Debug.Log(PlayerInput);
	}


	void PlayerMove(Vector3[] moveQueue)
	{
		for(int i = 0; i < moveQueue.Length; i++)
		{
				gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, moveQueue[i], 1f);
				//gameObject.transform.position = moveQueue[i];
		}
		*/
	}

	void PlayerInput()
	{
		Vector3 current = gameObject.transform.position;

		if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
		{
			if (Input.GetAxis("Horizontal") < 0)
			{
				PlayerAction(current += Vector3.left);
				//PlayerAction(new Vector3(current.x -1, current.y, current.z));
			}
			
			if (Input.GetAxis("Horizontal") > 0)
			{
				PlayerAction(current += Vector3.right);
				//PlayerAction(new Vector3(current.x +1, current.y, current.z));
			}
			
			if (Input.GetAxis("Vertical") < 0)
			{
				PlayerAction(current += Vector3.back);
				//PlayerAction(new Vector3(current.x, current.y -1, current.z -1));
			}
			
			if (Input.GetAxis("Vertical") > 0)
			{
				PlayerAction(current += Vector3.forward);
//				PlayerAction(new Vector3(current.x, current.y +1, current.z));
			}	
		}

		if (Input.GetButton("Jump"))
		{
			PlayerInputAllowed = false;
			ActionToTake = 1;
			ActionTaken = true;
			Debug.Log("ZzZz");
		}
	}
	
	void PlayerAction(Vector3 target)
	{
		Collider[] onTargetTile = Physics.OverlapSphere(target, 0f);

		if (onTargetTile.Length == 0)
		{
			PlayerInputAllowed = false;
			StartPosition = gameObject.transform.position;

			MoveTo.transform.position = target;

			ActionToTake = 2;
			ActionTaken = true;
		}
		else if (onTargetTile[0].tag == "Enemy")
		{
			PlayerInputAllowed = false;
			ActionToTake = 3;
			ActionTaken = true;
			Debug.Log("To Battle!");
		}
		else
		{
			Debug.Log("Ouch!");
		}
	}
	
	/*
	Vector3 GetKeyboardMove (float horizontal, float vertical)
	{
		Vector3 targetDestination = Current;

		if (horizontal < 0)
		{
			targetDestination = new Vector3(Current.x -1, Current.y, Current.z);
		}
		
		if (horizontal > 0)
		{
			targetDestination = new Vector3(Current.x +1, Current.y, Current.z);
		}

		if (vertical < 0)
		{
			targetDestination = new Vector3(Current.x, Current.y, Current.z -1);
		}
		
		if (vertical > 0)
		{
			targetDestination = new Vector3(Current.x, Current.y, Current.z +1);
		}

		if (Physics.OverlapSphere(targetDestination,0).Length > 0)
		{
			targetDestination = Current;
		}
		return targetDestination;
		
	}
	*/
	
}
