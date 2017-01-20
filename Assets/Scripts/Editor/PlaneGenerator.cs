using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PlaneGenerator : EditorWindow {

	private int width;
	private int height;
	private string planeName = "Plane";
	private bool addTerrainGenerator;
	private Material mat;

	[MenuItem("Custom/Plane Generator")]
	public static void OpenPlaneGenerator()
	{
		EditorWindow.GetWindow (typeof(PlaneGenerator));
	}

	void OnGUI()
	{
		GUILayout.BeginVertical ();
	
		planeName = EditorGUILayout.TextField ("Name", planeName);
		width = EditorGUILayout.IntField ("Width", width);
		height = EditorGUILayout.IntField ("Height", height);
		mat = (Material)EditorGUILayout.ObjectField ("Material", mat, typeof(Material));
		addTerrainGenerator = EditorGUILayout.Toggle ("Add Terrain Generator", addTerrainGenerator);
			
		if (GUILayout.Button ("Generate"))
			GeneratePlane ();

		GUILayout.EndVertical ();
	}

	private void GeneratePlane()
	{
		GameObject plane = new GameObject (planeName);
		Mesh planeMesh = new Mesh ();
		planeMesh.name = "Plane";

		List<Vector3> verts = new List<Vector3>(width*height);
		List<Vector3> normals = new List<Vector3>(width*height);
		List<Vector2> uvs = new List<Vector2>(width*height);
		List<int> triangles = new List<int> ();

		for (float i = 0.0f; i < height; i++) {
			for (float j = 0.0f; j < width; j++) {
							
				verts.Add (new Vector3 (i, 0, j));
				uvs.Add (new Vector2 (i / (float)(height), j / (float)(width)));
				normals.Add (Vector3.up);
			}
		}

		for (int i = 0; i < height*width - width - 1; i++) {
			if ((i+1) % width != 0) {
				triangles.Add (i);
				triangles.Add (i + 1);
				triangles.Add (i + width);

				triangles.Add (i + width);
				triangles.Add (i + 1);
				triangles.Add (i + 1 + width);
			}
		}

		planeMesh.SetVertices (verts);
		planeMesh.SetNormals (normals);
		planeMesh.SetUVs (0, uvs);
		planeMesh.SetUVs (1, uvs);
		planeMesh.SetUVs (2, uvs);
		planeMesh.SetUVs (3, uvs);

		planeMesh.SetTriangles (triangles, 0);

		plane.AddComponent<MeshFilter> ();
		plane.GetComponent<MeshFilter> ().mesh = planeMesh;

		plane.AddComponent<MeshRenderer> ();

		if (addTerrainGenerator) {
			plane.AddComponent<TerrainGenerator> ();
			plane.GetComponent<TerrainGenerator> ().width = width;
			plane.GetComponent<TerrainGenerator> ().height = height;
		}

		if(mat != null)
			plane.GetComponent<MeshRenderer> ().material = mat;
	}
}
