using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class PathFinder
{
	public class Node
	{
		public Vector3 Position {
			get;
			set;
		}
		
		public Node Parent {
			get;
			set;
		}
		
		public Node(Vector3 position, Node parent)
		{
			position.y = 0f;
			Position = position;
			Parent = parent;
		}
		
		public Node(Vector3 position)
		{
			Position = position;
		}
		
		public List<Node> GetChildNodes()
		{
			List<Node> children = new List<Node>();
			
			List<int> indexesToRemoveAt = new List<int>();
			
			children.Add(new Node(Position + Vector3.forward, this));
			children.Add(new Node(Position + Vector3.right, this));
			children.Add(new Node(Position + Vector3.back, this));
			children.Add(new Node(Position + Vector3.left, this));
			
			for (int i = 0; i < children.Count; i++) {
				Collider[] colliders = Physics.OverlapSphere(children[i].Position, 0);

				if (colliders.Length > 0)
				{
					for (int j = 0; j < colliders.Length; j++) {
						if (colliders[j].gameObject.tag != "Floor")
						{
							if (colliders[j].gameObject.tag == "PlayerCollider")
							{
								continue;
							}
							indexesToRemoveAt.Add(i);
							break;
						}
					}
				}
			}
			
			indexesToRemoveAt.Sort();
			indexesToRemoveAt.Reverse();
			
			for (int i = 0; i < indexesToRemoveAt.Count; i++) {
				children.RemoveAt(indexesToRemoveAt[i]);
			}
			
			return children;
		}
	}
	
	static int CalculateTilesBetween(Vector3 pos1, Vector3 pos2)
	{
		return Mathf.Abs((int)pos1.x - (int)pos2.x) + 
			Mathf.Abs((int)pos1.z - (int)pos2.z);	
	}
	
	public static Vector3[] GetMoveArray(Vector3 start, Vector3 target)
	{
		List<Node> openList = new List<Node>(); 
		List<Node> closedList = new List<Node>();
		bool targetFound = false;
		Vector3[] arrayOfMoves;
		List<Vector3> listOfMoves = new List<Vector3>();
		
		closedList.Add(new Node(start));
		
		while (!targetFound)
		{
			List<Node> children = closedList[closedList.Count-1].GetChildNodes();
			List<int> indexesToRemoveAt = new List<int>();
			
			for (int i = 0; i < children.Count; i++) 
			{
				bool removed = false;
				for (int j = 0; j < closedList.Count; j++) 
				{
					if (children[i].Position == closedList[j].Position)
					{
						indexesToRemoveAt.Add(i);
						removed = true;
						continue;
					}
				}
				
				if (removed)
				{
					continue;
				}
				
				for (int k = 0; k < openList.Count; k++) 
				{
					if (children[i].Position == openList[k].Position)
					{
						indexesToRemoveAt.Add(i);
						continue;
					}
				}
			}
			
			indexesToRemoveAt.Sort();
			indexesToRemoveAt.Reverse();
			
			for (int i = 0; i < indexesToRemoveAt.Count; i++) {
				children.RemoveAt(indexesToRemoveAt[i]);
			}
			
			openList.AddRange( children );
			
			int range = 10;
			int indexOfClosest = 0;
			
			for (int i = 0; i < openList.Count; i++) {
				if (i == 0)
				{
					range = CalculateTilesBetween(openList[i].Position, target);
					indexOfClosest = i;
				}
				if (CalculateTilesBetween(openList[i].Position, target) < range)
				{
					range = CalculateTilesBetween(openList[i].Position, target);
					indexOfClosest = i;
				}
			}

			
			closedList.Add(openList[indexOfClosest]);
			openList.RemoveAt(indexOfClosest);
			
			if (closedList[closedList.Count -1].Position == target)
			{
				targetFound = true;
				
				Node node = closedList[closedList.Count-1];
				while (node.Parent != null)
				{
					listOfMoves.Add(node.Position);
					Node parent = node.Parent;
					node = parent;
				}
			}
		}
		listOfMoves.Reverse();
		arrayOfMoves = listOfMoves.ToArray();
		return arrayOfMoves;
	}

}
