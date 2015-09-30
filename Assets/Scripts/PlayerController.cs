using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
{
    private Vector3 current;
	private Vector3[] moveQueue;
	private int moveQueueIndex;
    private Transform thisCollider;
    private bool[] validMoves = new bool[4];

    public Vector3 MoveTo { get; set; }
    public Vector3 MoveFrom { get; set; }
	public bool PlayerInputAllowed { get; set; }
	public bool ActionTaken { get; set; }

	public PlayerAction ActionToTake { get; set; }

	void Start () 
	{
		ActionToTake = PlayerAction.unset;
		ActionTaken = false;
		moveQueue = new Vector3[0];
		moveQueueIndex = 0;
        thisCollider = transform.FindChild("Collider");
    }

	void Update () 
	{

        //if (Input.GetButtonUp("Fire1"))
        //{
        //	RaycastHit hit;
        //	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //	Physics.Raycast(ray, out hit);

        //	Collider[] colliders = Physics.OverlapSphere(hit.transform.position, 0);

        //	if (colliders.Length == 1 && colliders[0].gameObject.tag == "Floor")
        //	{
        //		moveQueue = PathFinder.GetMoveArray(MoveTo.transform.position, hit.transform.position);
        //		moveQueueIndex = 0;
        //	}

        //	if (moveQueue.Length > 0)
        //	{
        //		for (int i = 0; i < moveQueue.Length-1; i++) 
        //		{
        //			Debug.DrawLine(moveQueue[i], moveQueue[i+1], Color.green, 5, false);
        //		}

        //		foreach (var move in moveQueue) {
        //			Debug.Log(move);
        //		}
        //	}
        //}

        //if (Input.GetButtonUp("Jump"))
        //{
        //	moveQueue = new Vector3[0];
        //	moveQueueIndex = 0;
        //}

        if (PlayerInputAllowed)
		{

			//if (moveQueue.Length > 0 && moveQueueIndex < moveQueue.Length)
			//{
			//	PlayerInputAllowed = false;
			//	moveQueue[moveQueueIndex] = new Vector3(moveQueue[moveQueueIndex].x, 0.5f, moveQueue[moveQueueIndex].z);
			//	SetPlayerAction(moveQueue[moveQueueIndex]);
			//	moveQueueIndex++;
			//	ActionToTake = PlayerAction.move;
			//	ActionTaken = true;
			//}

			if (Input.GetButton("Horizontal") || Input.GetButton("Vertical") || Input.GetButton("Jump"))
			{
				PlayerInput();
			}
		}	
	}

    public void PrepareTurn()
    {
        validMoves[(int)Direction.forward] = Physics.Linecast(current, current + Vector3.forward);
        validMoves[(int)Direction.right] = Physics.Linecast(current, current + Vector3.right);
        validMoves[(int)Direction.back] = Physics.Linecast(current, current + Vector3.back);
        validMoves[(int)Direction.left] = Physics.Linecast(current, current + Vector3.left);


    }

	void PlayerInput()
	{
        current = gameObject.transform.position;

        if (Input.GetButton("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                SetPlayerAction(Vector3.left);
            }

            if (Input.GetAxis("Horizontal") > 0)
            {
                SetPlayerAction(Vector3.right);
            }
        }
		else if (Input.GetButton("Vertical"))
        {	
			if (Input.GetAxis("Vertical") < 0)
			{
				SetPlayerAction(Vector3.back);
			}
			
			if (Input.GetAxis("Vertical") > 0)
			{
				SetPlayerAction(Vector3.forward);
			}	
		}

		if (Input.GetButton("Jump"))
		{
            PlayerInputAllowed = false;
            ActionToTake = PlayerAction.wait;
			ActionTaken = true;
			Debug.Log("ZzZz");
		}
	}
	
	void SetPlayerAction(Vector3 direction)
	{
        RaycastHit hit;
        bool blocked = Physics.Linecast(current, current + direction, out hit);

        if (!blocked)
        {
            PlayerInputAllowed = false;

            MoveFrom = current;
            MoveTo = current + direction;

            ActionToTake = PlayerAction.move;
            thisCollider.localPosition = direction;
            ActionTaken = true;
        }
        else if (hit.collider.gameObject.tag == "Enemy")
		{
            PlayerInputAllowed = false;

            ActionToTake = PlayerAction.attack;
			ActionTaken = true;
			Debug.Log("To Battle!");
		}
		else
		{
            //PlayerInputAllowed = true;
			Debug.Log("Ouch!");
		}
	}

    public void resetCollider()
    {
        thisCollider.localPosition = Vector3.zero;
    }

}
