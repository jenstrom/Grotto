using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//created with http://www.policyalmanac.org/games/aStarTutorial.htm

public class PathFinder : MonoBehaviour
{
    public class Node
	{
        private MapController map;

        public int MovesFromStart { get; set; }
        public int TilesFromTarget { get; set; }
        public int SupposedRangeFromTarget { get; set; }

        public Vector3 Position { get; set; }
		
		public Node Parent { get; set; }
		
		public Node(Vector3 position, int g, int h, Node parent)
		{
			Position = position;
			Parent = parent;
            MovesFromStart = g;
            TilesFromTarget = h;
            SupposedRangeFromTarget = MovesFromStart + TilesFromTarget;
		}
		
		public Node(Vector3 position, int g, int h)
		{
			Position = position;
            MovesFromStart = g;
            TilesFromTarget = h;
            SupposedRangeFromTarget = MovesFromStart + TilesFromTarget;
        }
    }

    static int CalculateTilesBetween(Vector3 pos1, Vector3 pos2)
	{
		return Mathf.Abs((int)pos1.x - (int)pos2.x) + 
			Mathf.Abs((int)pos1.z - (int)pos2.z);	
	}

    static List<Node> GetChildNodes(Node parent, Vector3 target, MapController map)
    {
        List<Node> children = new List<Node>();

        Vector3[] directions = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };

        foreach (var direction in directions)
        {
            int x = (int)parent.Position.x + (int)direction.x;
            int z = (int)parent.Position.z + (int)direction.z;

            TileType tile = map.GetTileType(x, z);

            if (tile.isWalkable)
            {
                children.Add(new Node(parent.Position + direction, parent.MovesFromStart + 1,
                    CalculateTilesBetween(parent.Position + direction, target),
                    parent));
            }
        }

        return children;
    }

    public static Vector3[] GetMoveArray(Vector3 start, Vector3 target)
	{
        start.y = 0.5f;
        target.y = 0.5f;

        MapController map = (MapController)FindObjectOfType(typeof(MapController));
        List<Node> openList = new List<Node>(); 
		List<Node> closedList = new List<Node>();
		bool targetFound = false;
        Vector3[] arrayOfMoves;
		List<Vector3> listOfMoves = new List<Vector3>();

		closedList.Add(new Node(start, 0, CalculateTilesBetween(start, target)));

        int loops = 0;
        while (!targetFound)
        {
        List<Node> children = GetChildNodes(closedList[closedList.Count - 1], target, map);

        for (int i = 0; i < closedList.Count; i++)
        {
            for (int c = children.Count-1; c >= 0; c--)
            {
                if (children[c].Position == closedList[i].Position)
                {
                    children.RemoveAt(c);
                }
            }
        }

            for (int i = 0; i < openList.Count; i++)
            {
                for (int c = children.Count - 1; c >= 0; c--)
                {
                    if (children[c].Position == openList[i].Position)
                    {
                        if (openList[i].Parent.MovesFromStart > children[c].Parent.MovesFromStart)
                        {
                            openList[i].Parent = children[c].Parent;
                            openList[i].MovesFromStart = openList[i].Parent.MovesFromStart + 1;
                            openList[i].SupposedRangeFromTarget = openList[i].MovesFromStart + openList[i].TilesFromTarget;
                        }
                        children.RemoveAt(c);
                    }
                }
            }

            openList.AddRange(children);

            int range = openList[0].SupposedRangeFromTarget;
            int indexOfClosest = 0;

            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].SupposedRangeFromTarget < range)
                {
                    range = openList[i].SupposedRangeFromTarget;
                    indexOfClosest = i;
                }
            }

            closedList.Add(openList[indexOfClosest]);
            openList.RemoveAt(indexOfClosest);

            if (closedList[closedList.Count - 1].Position == target)
            {
                targetFound = true;

                Node node = closedList[closedList.Count - 1];
                while (node.Parent != null)
                {
                    listOfMoves.Add(node.Position);
                    Node parent = node.Parent;
                    node = parent;
                }
            }
        loops++;
        if (loops > 10000)
            {
                Debug.Log("infinite loop in pathfinder");
                return new Vector3[0];
            }
        }

        //for (int i = 0; i < openList.Count - 1; i++)
        //{
        //    Debug.DrawLine(openList[i].Position, openList[i + 1].Position, Color.green, 600, true);
        //}

        //for (int i = 0; i < closedList.Count - 1; i++)
        //{
        //    Debug.DrawLine(closedList[i].Position, closedList[i + 1].Position, Color.blue, 600, true);
        //}

        listOfMoves.Reverse();
        arrayOfMoves = listOfMoves.ToArray();
        return arrayOfMoves;
	}
}
