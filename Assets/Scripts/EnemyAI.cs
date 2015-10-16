using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {

    private Vector3 Current;
    private Transform thisCollider;

	public EnemyAction ActionToTake {
		get;
		set;
	}

	public Vector3 MoveFrom {
		get;
		set;
	}

	public Vector3 MoveTo {
		get;
		set;
	}
    
	void Start () 
	{
		ActionToTake = EnemyAction.unset;
        thisCollider = transform.FindChild("Collider");
    }
    
    void Update () 
	{
		
	}

	public void MakeChoice()
	{
        Current = gameObject.transform.position;

        if (!ValidMoves())
        {
            return;
        }

        MoveFrom = Current;
        Vector3 playerPosition = Vector3.zero;
        bool aggro = false;

        Collider[] aggroRadar;
        aggroRadar = Physics.OverlapSphere(MoveFrom, 5);

        for (int i = 0; i < aggroRadar.Length; i++)
        {
            if (aggroRadar[i].tag == "Player")
            {
                playerPosition = aggroRadar[i].gameObject.transform.position;
                RaycastHit hit;
                Physics.Linecast(MoveFrom, playerPosition, out hit);

                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.Log("Oi!");
                    aggro = true;
                    gameObject.GetComponent<MeshRenderer>().material.color = new Color(255, 0, 0);
                    break;
                }
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material.color = new Color(0, 255, 0);
                aggro = false;
            }
        }

        if (aggro)
        {
            Vector3[] moveArray = new Vector3[1];
            moveArray = PathFinder.GetMoveArray(Current, playerPosition);
            MoveTo = moveArray[0];
            if (Physics.OverlapSphere(MoveTo, 0).Length > 0)
            {
                MoveTo = Current;
                Debug.Log("Rawr!");
            }
        }
        else
        {
            RandomMove();
		}
	}

	void Move(Vector3 direction)
	{
        
	}

    void RandomMove()
    {
        float choice = Random.value;
        Vector3 direction;
        bool blocked;

        if (choice < 0.25f)
        {
            direction = Vector3.forward;
        }
        else if (choice < 0.50f)
        {
            direction = Vector3.right;
        }
        else if (choice < 0.75f)
        {
            direction = Vector3.back;
        }
        else
        {
            direction = Vector3.left;
        }

        blocked = Physics.Linecast(Current, Current + direction);

        if (!blocked)
        {
            thisCollider.position = Current + direction;
            MoveTo = Current + direction;
        }
        else
        {
            RandomMove();
        }
    }

    bool ValidMoves()
    {
        Vector3[] directions =  {   Vector3.forward,
                                    Vector3.right,
                                    Vector3.back,
                                    Vector3.left    };
        RaycastHit hit;

        for (int i = 0; i < directions.Length; i++)
        {
            if (!Physics.Linecast(Current, Current + directions[i], out hit))
            {
                return true;
            }
            else
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void resetCollider()
    {
        thisCollider.localPosition = Vector3.zero;
    }
}












