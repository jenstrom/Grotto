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
    private MapMouseOver mousePointer;

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
        mousePointer = (MapMouseOver)FindObjectOfType(typeof(MapMouseOver));
    }

	void Update () 
	{
        if (PlayerInputAllowed)
        {
            if (Input.GetButtonUp("Fire1"))
            {
                if (moveQueue.Length > 0)
                {
                    ClearMoveQueue();
                }
                else
                {
                    current = gameObject.transform.position;
                    Vector3 target = mousePointer.selection.transform.position;

                    moveQueue = PathFinder.GetMoveArray(current, target);
                    moveQueueIndex = 0;
                }

                if (moveQueue.Length > 0)
                {
                    //Debug.Log(target);
                    for (int i = 0; i < moveQueue.Length - 1; i++)
                    {
                        Debug.DrawLine(moveQueue[i], moveQueue[i + 1], Color.red, 5, true);
                    }
                }
            }

            if (Input.GetButtonUp("Jump"))
            {
                ClearMoveQueue();
            }

            if (moveQueue.Length > 0 && moveQueueIndex < moveQueue.Length)
            {
                ExecuteMoveQueue();
            }
            else
            {
                if (Input.GetButton("Horizontal") || Input.GetButton("Vertical") || Input.GetButton("Jump"))
                {
                    PlayerInput();
                }
            }
        }
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

            Move(direction);
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

    public void ExecuteMoveQueue()
    {
        PlayerInputAllowed = false;
        current = gameObject.transform.position;
        Vector3 nextMove = moveQueue[moveQueueIndex];

        RaycastHit hit;
        bool blocked = Physics.Linecast(current, nextMove, out hit);

        if (blocked)
        {
            moveQueue = new Vector3[0];
            moveQueueIndex = 0;
            PlayerInputAllowed = true;
        }
        else
        {
            moveQueueIndex++;

            if (moveQueueIndex == moveQueue.Length)
            {
                moveQueue = new Vector3[0];
                moveQueueIndex = 0;
            }

            Move(nextMove - current);
        }
    }

    public void ClearMoveQueue()
    {
        moveQueue = new Vector3[0];
        moveQueueIndex = 0;
    }

    public void Move(Vector3 direction)
    {
        MoveFrom = current;
        MoveTo = current + direction;

        ActionToTake = PlayerAction.move;
        thisCollider.position = current + direction;
        ActionTaken = true;
    }

    public void resetCollider()
    {
        thisCollider.localPosition = Vector3.zero;
    }

}
