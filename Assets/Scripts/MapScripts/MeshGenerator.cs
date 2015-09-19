using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour {

	public int terrainHeight = 3;

	public SquareGrid squareGrid;
	
	List<Vector3> vertices;
	List<int> triangles;
	
	Dictionary<int,List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>> ();
	List<List<int>> outlines = new List<List<int>> ();
	HashSet<int> checkedVertices = new HashSet<int>();
	
	public void GenerateMesh(int[,] map, float squareSize) {
		squareGrid = new SquareGrid(map, squareSize);
		
		vertices = new List<Vector3>();
		triangles = new List<int>();
		
		for (int x = 0; x < squareGrid.squares.GetLength(0); x ++) {
			for (int z = 0; z < squareGrid.squares.GetLength(1); z ++) {
				TriangulateSquare(squareGrid.squares[x,z]);
			}
		}
		
		Mesh mesh 		 = new Mesh(); 			// The mesh (verticies, uvs, triangels).
		GameObject roof  = new GameObject();	// The GameObject the mesh should be attached to.
		roof.name 		 = "Roof";				// Name of the GameObject that appears in the "hierarchy list"
		mesh.name 		 = "RoofPMesh" ;  		// Name of the mesh, appears in the inspector under meshRenderer (if u select the Gameobject floor)
		
		mesh.vertices = vertices.ToArray(); 	// Assign the possitions of the verticies with a array of vector3's
		mesh.triangles = triangles.ToArray();  	// Generate the triangels from positions of the vertices 
		mesh.RecalculateNormals();
		
		// add the meshfilter to the gameobject and assign the mesh we just created.
		roof.AddComponent<MeshFilter>().mesh = mesh;
		
		// add the meshrenderer component to the gameObject. (and store the component in a variable)
		MeshRenderer renderer = roof.AddComponent<MeshRenderer>();
		
		// add the standard material to the meshRender component
		renderer.material.shader = Shader.Find("Standard");

		// Generate UVs
		int tileAmount = 32;
		Vector2[] uvs = new Vector2[vertices.Count];
		for (int i =0; i < vertices.Count; i ++) {
			float percentX = Mathf.InverseLerp(-map.GetLength(0)*squareSize, map.GetLength(0)*squareSize,vertices[i].x) * tileAmount;
			float percentY = Mathf.InverseLerp(-map.GetLength(0)*squareSize, map.GetLength(0)*squareSize,vertices[i].z) * tileAmount;
			uvs[i] = new Vector2(percentX,percentY);
		}
		mesh.uv = uvs;

		// Assign a texture
		Texture2D text = (Texture2D)Resources.Load("Textures/MapTextures/Roof_Tile_01");
		renderer.material.mainTexture = (Texture2D)text;

		CreateWallMesh();
	}


	public void GenerateFloorMesh(int[,] map, float squareSize) {
		squareGrid = new SquareGrid(map, squareSize);
		
		vertices = new List<Vector3>();
		triangles = new List<int>();
		
		for (int x = 0; x < squareGrid.squares.GetLength(0); x ++) {
			for (int z = 0; z < squareGrid.squares.GetLength(1); z ++) {
				TriangulateSquare(squareGrid.squares[x,z]);
			}
		}
		
		Mesh mesh 		 = new Mesh(); 			// The mesh (verticies, uvs, triangels).
		GameObject floor = new GameObject();	// The GameObject the mesh should be attached to.
		floor.name 		 = "Floor";				// Name of the GameObject that appears in the "hierarchy list"
		mesh.name 		 = "FloorPMesh" ;  		// Name of the mesh, appears in the inspector under meshRenderer (if u select the Gameobject floor)
		
		mesh.vertices  = vertices.ToArray(); 	// Assign the possitions of the verticies with a array of vector3's
		mesh.triangles = triangles.ToArray();  	// Generate the triangels from positions of the vertices 
		mesh.RecalculateNormals();

		// Generate UVs
		int tileAmount = 100;
		Vector2[] uvs = new Vector2[vertices.Count];
		for (int i =0; i < vertices.Count; i ++) {
			float percentX = Mathf.InverseLerp(-map.GetLength(0)*squareSize, map.GetLength(0)*squareSize,vertices[i].x) * tileAmount;
			float percentY = Mathf.InverseLerp(-map.GetLength(0)*squareSize, map.GetLength(0)*squareSize,vertices[i].z) * tileAmount;
			uvs[i] = new Vector2(percentX,percentY);
		}
		mesh.uv = uvs;
		
		// add the meshfilter to the gameobject and assign the mesh we just created.
		floor.AddComponent<MeshFilter>().mesh = mesh;

		floor.AddComponent<MeshCollider>();


		//MOUSEOVER 
		floor.AddComponent<MapMouseOver>();


		// add the meshrenderer component to the gameObject. (and store the component in a variable)
		MeshRenderer renderer = floor.AddComponent<MeshRenderer>();
		
		// add the standard material to the meshRender component
		renderer.material.shader = Shader.Find("Standard");

		floor.transform.Translate( new Vector3 (0, -terrainHeight, 0) );

		// Assign a texture
		Texture2D text = (Texture2D)Resources.Load("Textures/MapTextures/Floor_Tile_01");
		renderer.material.mainTexture = (Texture2D)text;
	}
	
	void CreateWallMesh() {
		
		CalculateMeshOutlines ();
		
		List<Vector3> wallVertices  = new List<Vector3> ();
		List<int> wallTriangles     = new List<int> ();
		Mesh wallMesh 				= new Mesh ();
		float wallHeight 			= terrainHeight;

		GameObject wall  = new GameObject();	// The GameObject the mesh should be attached to.
		wall.name 		 = "Walls";				// Name of the GameObject that appears in the "hierarchy list"
		wallMesh.name 	 = "WallsPMesh" ;  		// Name of the mesh, appears in the inspector under meshRenderer (if u select the Gameobject floor)
		
		foreach (List<int> outline in outlines) {
			for (int i = 0; i < outline.Count -1; i ++) {
				int startIndex = wallVertices.Count;
				wallVertices.Add(vertices[outline[i]]); // left
				wallVertices.Add(vertices[outline[i+1]]); // right
				wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // bottom left
				wallVertices.Add(vertices[outline[i+1]] - Vector3.up * wallHeight); // bottom right
				
				wallTriangles.Add(startIndex + 0);
				wallTriangles.Add(startIndex + 2);
				wallTriangles.Add(startIndex + 3);
				
				wallTriangles.Add(startIndex + 3);
				wallTriangles.Add(startIndex + 1);
				wallTriangles.Add(startIndex + 0);
			}
		}
		wallMesh.vertices  = wallVertices.ToArray ();
		wallMesh.triangles = wallTriangles.ToArray ();

		// add the meshfilter to the gameobject and assign the mesh we just created.
		wall.AddComponent<MeshFilter>().mesh = wallMesh;

		wall.AddComponent<MeshCollider>();

		// add the meshrenderer component to the gameObject. (and store the component in a variable)
		MeshRenderer renderer = wall.AddComponent<MeshRenderer>();
		
		// add the standard material to the meshRender component
		renderer.material.shader = Shader.Find("Standard");

		// Generate UVs
		Vector2[] uvs = new Vector2[wallVertices.Count];
		for (int i =0; i < wallVertices.Count; i ++) {
			float percentX = wallVertices[i].x + wallVertices[i].z /2;
			float percentY = wallVertices[i].y;
			uvs[i] = new Vector2(percentX,percentY);
		}
		wallMesh.uv = uvs;
		
		// Assign a texture
		Texture2D text = (Texture2D)Resources.Load("Textures/MapTextures/Wall_01");
		renderer.material.mainTexture = (Texture2D)text;

	}
	
	void TriangulateSquare(Square square) {
		switch (square.configuration) {
		case 0:
			break;
			
			// 1 points:
		case 1:
			MeshFromPoints(square.centreLeft, square.centreBottom, square.bottomLeft);
			break;
		case 2:
			MeshFromPoints(square.bottomRight, square.centreBottom, square.centreRight);
			break;
		case 4:
			MeshFromPoints(square.topRight, square.centreRight, square.centreTop);
			break;
		case 8:
			MeshFromPoints(square.topLeft, square.centreTop, square.centreLeft);
			break;
			
			// 2 points:
		case 3:
			MeshFromPoints(square.centreRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 6:
			MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.centreBottom);
			break;
		case 9:
			MeshFromPoints(square.topLeft, square.centreTop, square.centreBottom, square.bottomLeft);
			break;
		case 12:
			MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreLeft);
			break;
		case 5:
			MeshFromPoints(square.centreTop, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft, square.centreLeft);
			break;
		case 10:
			MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.centreBottom, square.centreLeft);
			break;
			
			// 3 point:
		case 7:
			MeshFromPoints(square.centreTop, square.topRight, square.bottomRight, square.bottomLeft, square.centreLeft);
			break;
		case 11:
			MeshFromPoints(square.topLeft, square.centreTop, square.centreRight, square.bottomRight, square.bottomLeft);
			break;
		case 13:
			MeshFromPoints(square.topLeft, square.topRight, square.centreRight, square.centreBottom, square.bottomLeft);
			break;
		case 14:
			MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centreBottom, square.centreLeft);
			break;
			
			// 4 point:
		case 15:
			MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
			checkedVertices.Add(square.topLeft.vertexIndex);
			checkedVertices.Add(square.topRight.vertexIndex);
			checkedVertices.Add(square.bottomRight.vertexIndex);
			checkedVertices.Add(square.bottomLeft.vertexIndex);
			break;
		}
		
	}
	
	void MeshFromPoints(params Node[] points) {
		AssignVertices(points);
		
		if (points.Length >= 3)
			CreateTriangle(points[0], points[1], points[2]);
		if (points.Length >= 4)
			CreateTriangle(points[0], points[2], points[3]);
		if (points.Length >= 5) 
			CreateTriangle(points[0], points[3], points[4]);
		if (points.Length >= 6)
			CreateTriangle(points[0], points[4], points[5]);
		
	}
	
	void AssignVertices(Node[] points) {
		for (int i = 0; i < points.Length; i ++) {
			if (points[i].vertexIndex == -1) {
				points[i].vertexIndex = vertices.Count;
				vertices.Add(points[i].position);
			}
		}
	}
	
	void CreateTriangle(Node a, Node b, Node c) {
		triangles.Add(a.vertexIndex);
		triangles.Add(b.vertexIndex);
		triangles.Add(c.vertexIndex);
		
		Triangle triangle = new Triangle (a.vertexIndex, b.vertexIndex, c.vertexIndex);
		AddTriangleToDictionary (triangle.vertexIndexA, triangle);
		AddTriangleToDictionary (triangle.vertexIndexB, triangle);
		AddTriangleToDictionary (triangle.vertexIndexC, triangle);
	}
	
	void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle) {
		if (triangleDictionary.ContainsKey (vertexIndexKey)) {
			triangleDictionary [vertexIndexKey].Add (triangle);
		} else {
			List<Triangle> triangleList = new List<Triangle>();
			triangleList.Add(triangle);
			triangleDictionary.Add(vertexIndexKey, triangleList);
		}
	}
	
	void CalculateMeshOutlines() {
		
		for (int vertexIndex = 0; vertexIndex < vertices.Count; vertexIndex ++) {
			if (!checkedVertices.Contains(vertexIndex)) {
				int newOutlineVertex = GetConnectedOutlineVertex(vertexIndex);
				if (newOutlineVertex != -1) {
					checkedVertices.Add(vertexIndex);
					
					List<int> newOutline = new List<int>();
					newOutline.Add(vertexIndex);
					outlines.Add(newOutline);
					FollowOutline(newOutlineVertex, outlines.Count-1);
					outlines[outlines.Count-1].Add(vertexIndex);
				}
			}
		}
	}
	
	void FollowOutline(int vertexIndex, int outlineIndex) {
		outlines [outlineIndex].Add (vertexIndex);
		checkedVertices.Add (vertexIndex);
		int nextVertexIndex = GetConnectedOutlineVertex (vertexIndex);
		
		if (nextVertexIndex != -1) {
			FollowOutline(nextVertexIndex, outlineIndex);
		}
	}
	
	int GetConnectedOutlineVertex(int vertexIndex) {
		List<Triangle> trianglesContainingVertex = triangleDictionary [vertexIndex];
		
		for (int i = 0; i < trianglesContainingVertex.Count; i ++) {
			Triangle triangle = trianglesContainingVertex[i];
			
			for (int j = 0; j < 3; j ++) {
				int vertexB = triangle[j];
				if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB)) {
					if (IsOutlineEdge(vertexIndex, vertexB)) {
						return vertexB;
					}
				}
			}
		}
		
		return -1;
	}
	
	bool IsOutlineEdge(int vertexA, int vertexB) {
		List<Triangle> trianglesContainingVertexA = triangleDictionary [vertexA];
		int sharedTriangleCount = 0;
		
		for (int i = 0; i < trianglesContainingVertexA.Count; i ++) {
			if (trianglesContainingVertexA[i].Contains(vertexB)) {
				sharedTriangleCount ++;
				if (sharedTriangleCount > 1) {
					break;
				}
			}
		}
		return sharedTriangleCount == 1;
	}
	
	struct Triangle {
		public int vertexIndexA;
		public int vertexIndexB;
		public int vertexIndexC;
		int[] vertices;
		
		public Triangle (int a, int b, int c) {
			vertexIndexA = a;
			vertexIndexB = b;
			vertexIndexC = c;
			
			vertices = new int[3];
			vertices[0] = a;
			vertices[1] = b;
			vertices[2] = c;
		}
		
		public int this[int i] {
			get {
				return vertices[i];
			}
		}
		
		
		public bool Contains(int vertexIndex) {
			return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
		}
	}
	
	public class SquareGrid {
		public Square[,] squares;
		
		public SquareGrid(int[,] map, float squareSize) {
			int nodeCountX = map.GetLength(0);
			int nodeCountY = map.GetLength(1);
			//float mapWidth = nodeCountX * squareSize;
			//float mapHeight = nodeCountY * squareSize;
			
			ControlNode[,] controlNodes = new ControlNode[nodeCountX,nodeCountY];
			
			for (int x = 0; x < nodeCountX; x ++) {
				for (int z = 0; z < nodeCountY; z ++) {
					//Vector3 pos = new Vector3(-mapWidth/2 + x * squareSize + squareSize/2, 0, -mapHeight/2 + y * squareSize + squareSize/2);

					Vector3 pos = new Vector3(x * squareSize + squareSize/2 , 3, z * squareSize + squareSize/2 );
					controlNodes[x,z] = new ControlNode(pos,map[x,z] == 1, squareSize);
				}
			}
			
			squares = new Square[nodeCountX -1,nodeCountY -1];
			for (int x = 0; x < nodeCountX-1; x ++) {
				for (int z = 0; z < nodeCountY-1; z ++) {
					squares[x,z] = new Square(controlNodes[x,z+1], controlNodes[x+1,z+1], controlNodes[x+1,z], controlNodes[x,z]);
				}
			}
			
		}
	}
	
	public class Square {
		
		public ControlNode topLeft, topRight, bottomRight, bottomLeft;
		public Node centreTop, centreRight, centreBottom, centreLeft;
		public int configuration;
		
		public Square (ControlNode _topLeft, ControlNode _topRight, ControlNode _bottomRight, ControlNode _bottomLeft) {
			topLeft = _topLeft;
			topRight = _topRight;
			bottomRight = _bottomRight;
			bottomLeft = _bottomLeft;
			
			centreTop = topLeft.right;
			centreRight = bottomRight.above;
			centreBottom = bottomLeft.right;
			centreLeft = bottomLeft.above;
			
			if (topLeft.active)
				configuration += 8;
			if (topRight.active)
				configuration += 4;
			if (bottomRight.active)
				configuration += 2;
			if (bottomLeft.active)
				configuration += 1;
		}
		
	}
	
	public class Node {
		public Vector3 position;
		public int vertexIndex = -1;
		
		public Node(Vector3 _pos) {
			position = _pos;
		}
	}
	
	public class ControlNode : Node {
		
		public bool active;
		public Node above, right;
		
		public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
			active = _active;
			above = new Node(position + Vector3.forward * squareSize/2f);
			right = new Node(position + Vector3.right * squareSize/2f);
		}
		
	}
}
