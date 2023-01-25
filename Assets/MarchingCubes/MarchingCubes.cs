using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MarchingCubes : MonoBehaviour {
	public int resolution;
	public float threshold;

	[SerializeField]
	private VoxelGrid _voxelGrid;
	
	private float[][][] voxels;

	private List<Vector3> _verticies;
	private List<int> _triangles;
	
	private Mesh _mesh;
	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;
	
	private void Awake() {
		_meshFilter = GetComponent<MeshFilter>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_mesh = new Mesh();
		_meshFilter.mesh = _mesh;
	}

	private void OnEnable() {
		voxels = new float[resolution][][];
		for (int x = 0; x < resolution; x++) {
			voxels[x] = new float[resolution][];
			for (int y = 0; y < resolution; y++) {
				voxels[x][y] = new float[resolution];
				for (int z = 0; z < resolution; z++) {
					voxels[x][y][z] = Vector3.Distance(new Vector3(x, y, z), new Vector3(resolution/2f - 0.5f, resolution/2f - 0.5f, resolution/2f - 0.5f)) / (resolution-1);
				}
			}
		}
		
		// pass data to voxelGrid
		_voxelGrid.voxels = voxels;
		_voxelGrid.resolution = resolution;
		_voxelGrid.threshold = threshold;
		
		_voxelGrid.Initialize();
		
		MarchCubes();
	}

	private void MarchCubes() {
		_verticies = new List<Vector3>();
		_triangles = new List<int>();
		
		for (int x = 0; x < resolution - 1; x++) {
			for (int y = 0; y < resolution - 1; y++) {
				for (int z = 0; z < resolution - 1; z++) {
					int value = GetSquareValue(x, y, z);
					var edges = MarchingData.TriangleTable[value];
					for (int i = 0; i < edges.Length; i += 3) {
						if (edges[i] == -1) {
							break;
						}
						
						int count = _verticies.Count;
			
						_verticies.Add(GetVertexPosition(x, y, z, edges[i]));
						_verticies.Add(GetVertexPosition(x, y, z, edges[i + 1]));
						_verticies.Add(GetVertexPosition(x, y, z, edges[i + 2]));
			
						_triangles.Add(count);
						_triangles.Add(count + 1);
						_triangles.Add(count + 2);
					}

				}
			}	
		}
		
		_mesh.vertices = _verticies.ToArray();
		_mesh.triangles = _triangles.ToArray();
		_mesh.RecalculateNormals();
	}
	
	private Vector3 GetVertexPosition(int x, int y, int z, int edge) {
		int vertex1 = MarchingData.EdgeVertexIndices[edge][0];
		int vertex2 = MarchingData.EdgeVertexIndices[edge][1];
		
		Vector3 vertex1Position = GetVoxelPosition(x, y, z, vertex1);
		Vector3 vertex2Position = GetVoxelPosition(x, y, z, vertex2);
		
		float vertex1Value = voxels[(int) vertex1Position.x][(int) vertex1Position.y][(int) vertex1Position.z];
		float vertex2Value = voxels[(int) vertex2Position.x][(int) vertex2Position.y][(int) vertex2Position.z];
		
		float t = (threshold - vertex1Value) / (vertex2Value - vertex1Value);
		return Vector3.Lerp(vertex1Position, vertex2Position, t);
	}

	private Vector3 GetVoxelPosition(int x, int y, int z, int vertex) {
		return new Vector3(x + MarchingData.VertexOffset[vertex][0], y + MarchingData.VertexOffset[vertex][1],
			z + MarchingData.VertexOffset[vertex][2]);
	}

	private int GetSquareValue(int x, int y, int z) {
		int value = 0;
		for (int i = 0; i < MarchingData.VertexOffset.Length; i++) {
			int vertexX = x + MarchingData.VertexOffset[i][0];
			int vertexY = y + MarchingData.VertexOffset[i][1];
			int vertexZ = z + MarchingData.VertexOffset[i][2];

			float vertexValue = voxels[vertexX][vertexY][vertexZ];
			if (vertexValue < threshold) {
				// adds a power of two to the value
				value |= 1 << i;
			}
		}

		return value;
	}
}
