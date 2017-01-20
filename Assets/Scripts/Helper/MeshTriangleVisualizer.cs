using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class MeshTriangleVisualizer : MonoBehaviour {

	private Mesh m_Mesh;
	public bool m_Enable = false;

	public int firstTriangle = 0;
	public int lastTriangle = 0;

	[Range(0.1f, 1.0f)]
	public float m_GizmoSize;

	private Color[] m_Colors;

	void OnValidate()
	{
		if (firstTriangle > lastTriangle)
			lastTriangle = firstTriangle;

		if (firstTriangle < 0)
			firstTriangle = 0;
	}

	void OnDrawGizmos()
	{
		if (m_Enable) {
			if (m_Mesh == null) {
				m_Mesh = GetComponent<MeshFilter> ().sharedMesh;
				m_Colors = new Color[]{new Color(1,0,0), new Color(0,1,0), new Color(0,0,1)};
			}

			if (m_Mesh != null) {
				int index = 0;
				for (int i = firstTriangle; i < m_Mesh.triangles.Length / 3 && i <= lastTriangle; i++) {
					for (int j = 0; j < 3; j++) {
						index = m_Mesh.triangles [3 * i + j];

						if (m_Mesh.colors.Length != 0)
							Gizmos.color = m_Mesh.colors [index];
						else
							Gizmos.color = m_Colors [j];

						Gizmos.DrawWireSphere (transform.TransformPoint (m_Mesh.vertices [index]), m_GizmoSize);
					}
				}
			}
		}
	}
}
