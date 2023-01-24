using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour {
	public int resolution = 16;
	public float threshold = 0.5f;

	// 0-1 range, 0 is empty, 1 is full
	public float[][][] voxels;
	
	[SerializeField]
	private Mesh _mesh;
	[SerializeField]
	private Material _material;

	[SerializeField, Range(0.01f, 0.3f)]
	private float scale = 0.1f;
	
	[SerializeField]
	private Gradient _gradient = new Gradient();
	
	private ComputeBuffer _matricesBuffer;
	private Matrix4x4[] _matrices;
	
	private static readonly int matricesId = Shader.PropertyToID("_Matrices");
	private static MaterialPropertyBlock _propertyBlock;
	
	private void OnEnable() {
		const int stride = 16 * 4;
		int length = resolution * resolution * resolution;
		_matricesBuffer = new ComputeBuffer(length, stride);
		_matrices = new Matrix4x4[length];
		
		voxels = new float[resolution][][];
		for (int x = 0; x < resolution; x++) {
			voxels[x] = new float[resolution][];
			for (int y = 0; y < resolution; y++) {
				voxels[x][y] = new float[resolution];
				for (int z = 0; z < resolution; z++) {
					voxels[x][y][z] = Vector3.Distance(new Vector3(x, y, z), new Vector3(resolution/2f - 0.5f, resolution/2f - 0.5f, resolution/2f - 0.5f)) / resolution;
				}
			}
		}
		
		_propertyBlock ??= new MaterialPropertyBlock();
	}

	private void OnDisable() {
		_matricesBuffer.Release();
		
		_matricesBuffer = null;
	}

	private void Update() {
		for (int x = 0; x < resolution; x++) {
			for (int y = 0; y < resolution; y++) {
				for (int z = 0; z < resolution; z++) {
					_matrices[x + y * resolution + z * resolution * resolution] = Matrix4x4.TRS(GetVoxelPosition(x, y, z), Quaternion.identity, Vector3.one * scale);
					Vector4 color;
					if (voxels[x][y][z] < threshold) {
						color = _gradient.Evaluate(0);
					}
					else {
						color = _gradient.Evaluate(1);
					}

					_matrices[x + y * resolution + z * resolution * resolution].SetRow(3, color);
				}
			}
		}
		
		var bounds = new Bounds(transform.position, Vector3.one * resolution);
		_matricesBuffer.SetData(_matrices);
		_propertyBlock.SetBuffer(matricesId, _matricesBuffer);
		Graphics.DrawMeshInstancedProcedural(_mesh, 0, _material, bounds, _matricesBuffer.count, _propertyBlock);
	}

	private void OnValidate() {
		if (_matricesBuffer != null && enabled) {
			OnDisable();
			OnEnable();
		}
	}

	private Vector3 GetVoxelPosition(int x, int y, int z) {
		return (new Vector3(x, y, z) + transform.position);
	}
}
