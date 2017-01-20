using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour {

	public bool m_ApplyPerlinNoise = false;
	private bool m_Reset = true;
	private bool m_Init = false;

	public bool m_ApplyTime = false;
	private float m_Delta = 1.0f;

	[System.Serializable]
	public struct PerlinTerrainLevel
	{
		public float frequency;
		public float seed;
		public Vector2 movementSpeed;
	};

	public float m_Scale;	

	public bool m_MakeIsland;

	[Range(-1,1)]
	public float m_IslandOffset = 0.0f;

	public List<PerlinTerrainLevel> m_DetailLevels;

	public bool m_UseVertexColors;
	public Gradient m_VertexColors;

	public bool m_EnableRandomVertColorOffset = false;

	[Range(0,1)]
	public float m_RandRange;

	[HideInInspector]
	public int width, height;

	private Mesh m_Terrain;
	private List<Vector3> m_Verts;
	private List<Color> m_VertColors;

	void Start()
	{
		Init ();
	}

	void OnEnable()
	{
		Init ();
	}

	void Update()
	{
		if (m_ApplyTime) {
			m_Delta += Time.deltaTime;
			UpdateTerrain ();
		} else {
			m_Delta = 1.0f;
		}
	}

	private void Init()
	{
		m_Terrain = GetComponent<MeshFilter> ().sharedMesh;
		m_Verts = new List<Vector3> (width * height);
		m_VertColors = new List<Color> (width * height);
		m_Init = true;
	}
		
	void OnValidate()
	{
		if (m_ApplyPerlinNoise) {
			UpdateTerrain ();
			m_Reset = false;
		} else if (!m_Reset) {
			ResetTerrain ();
			m_Reset = true;
		}
	}

	public void UpdateTerrain()
	{
		if (!m_Init) {
			Init ();
		} 

		if (m_Init) {
			float vertHeight = 0.0f;
			m_Verts.Clear ();

			for (int i = 0; i < m_DetailLevels.Count; i++) {
				for (int j = 0; j < height; j++) {
					for (int k = 0; k < width; k++) {

						vertHeight = Mathf.PerlinNoise (
							((float)k / (float)width) * m_DetailLevels [i].frequency + m_DetailLevels [i].seed + m_Delta * m_DetailLevels[i].movementSpeed.x,
							((float)j / (float)height) * m_DetailLevels [i].frequency + m_DetailLevels [i].seed + m_Delta * m_DetailLevels[i].movementSpeed.y);

						vertHeight = 2 * vertHeight - 1;
						vertHeight *= m_Scale;
						vertHeight /= Mathf.Pow (2, i);
				
						if (m_MakeIsland) {
							float center = width / 2;
							vertHeight = Mathf.Lerp (
								vertHeight, 
								-m_Scale/10, 
								(Mathf.Abs (j - center) + Mathf.Abs (k - center))/width + m_IslandOffset);
						}

						if(i == 0)
							m_Verts.Add (new Vector3 ((float)j, vertHeight, (float)k));
						else
							m_Verts [j * width + k] += new Vector3 (0, vertHeight, 0);
					}
				}
			}
				
			m_Terrain.SetVertices (m_Verts);
			m_Terrain.RecalculateNormals ();

			if (m_UseVertexColors)
				SetVertColors ();
		}
	}

	public void ResetTerrain()
	{
		if (!m_Init) {
			Init ();
		} 

		if (m_Init) {
			m_Verts.Clear ();

			for (float i = 0; i < height; i++) {
				for (float j = 0; j < width; j++) {
					m_Verts.Add (new Vector3 (i, 0, j));
				}
			}

			m_Terrain.SetVertices (m_Verts);
			m_Terrain.RecalculateNormals ();
		}
	}

	private void SetVertColors()
	{
		m_VertColors.Clear ();
		float randOffset = 0.0f;

		for (int i = 0; i < width*height; i++) {
			if(m_EnableRandomVertColorOffset)
				randOffset = Random.Range (-m_RandRange, m_RandRange);
			m_VertColors.Add(m_VertexColors.Evaluate ((m_Verts [i].y + m_Scale) / (2 * m_Scale) + randOffset));
		}
			
		m_Terrain.SetColors (m_VertColors);
	}
}
