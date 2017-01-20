using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class NormalVisualizer : MonoBehaviour {

	private Mesh m_Mesh;

	public bool m_Enable;

	void OnDrawGizmos()
	{
		if (m_Enable) {
			if (m_Mesh == null) {
				
				m_Mesh = GetComponent<MeshFilter> ().sharedMesh;

			} else {
				
				Ray r = new Ray ();

				for(int i=0; i< m_Mesh.vertexCount; i++) {
					r.origin = transform.TransformPoint (m_Mesh.vertices [i]);
					r.direction = m_Mesh.normals [i];
					Gizmos.DrawRay (r);
				}
			}
		}
	}
}
