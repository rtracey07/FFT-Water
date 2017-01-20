using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class OceanWaveGenerator : MonoBehaviour {

	public bool m_ApplyWave = false;
	public bool m_ApplyDeltaTime = false;

	private Mesh m_Mesh;

	void Start () {
		InitWaterLevel ();
	}

	private void InitWaterLevel()
	{
		
	}

	void OnValidate()
	{
		if (m_Mesh == null)
			m_Mesh = this.GetComponent<MeshFilter> ().sharedMesh;

		if (m_ApplyWave) {

			float[] verts = new float[m_Mesh.vertices.Length];
			List<Vector3> meshVerts = new List<Vector3> (m_Mesh.vertices.Length);

			for (int i = 0; i < m_Mesh.vertices.Length; i++) {
				verts [i] = m_Mesh.vertices [i].y;
			}

			FFT(verts);

			for (int i = 0; i < m_Mesh.vertices.Length; i++) {
				Vector3 v = m_Mesh.vertices [i];
				v.y = verts [i];
				meshVerts.Add (v);
			}

			m_Mesh.SetVertices (meshVerts);
			m_Mesh.RecalculateNormals ();
		}

		m_ApplyWave = false;
	}

	void Update () {
		if (m_ApplyDeltaTime) {
			UpdateWaveLevel ();
		}
	}

	private void UpdateWaveLevel() {
		
	}

	/* Performs a Bit Reversal Algorithm on a postive integer 
     * for given number of bits
     * e.g. 011 with 3 bits is reversed to 110 */
	public int BitReverse(int n, int bits) {
		int reversedN = n;
		int count = bits - 1;

		n >>= 1;
		while (n > 0) {
			reversedN = (reversedN << 1) | (n & 1);
			count--;
			n >>= 1;
		}

		return ((reversedN << count) & ((1 << bits) - 1));
	}

	/* Uses Cooley-Tukey iterative in-place algorithm with radix-2 DIT case
     * assumes no of points provided are a power of 2 */
	public void FFT(float[] buffer) {

		int bits = (int)Mathf.Log(buffer.Length, 2);
		for (int j = 1; j < buffer.Length / 2; j++) {

			int swapPos = BitReverse(j, bits);
			var temp = buffer[j];
			buffer[j] = buffer[swapPos];
			buffer[swapPos] = temp;
		}

		for (int N = 2; N <= buffer.Length; N <<= 1) {
			for (int i = 0; i < buffer.Length; i += N) {
				for (int k = 0; k < N / 2; k++) {

					int evenIndex = i + k;
					int oddIndex = i + k + (N / 2);
					var even = buffer[evenIndex];
					var odd = buffer[oddIndex];

					float term = -2 * Mathf.PI * k / (float)N;
					float exp = Mathf.Cos(term) *  Mathf.Sin(term) * odd;

					buffer[evenIndex] = even + exp;
					buffer[oddIndex] = even - exp;
				}
			}
		}
	}

}
