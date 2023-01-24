using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour {
	public int resolution = 10;
	
	[SerializeField]
	private Mesh _mesh;
	[SerializeField]
	private Material _material;

	[SerializeField, Range(0.01f, 0.3f)]
	private float scale = 0.1f;
	
	private ComputeBuffer _matricesBuffer;
	private Matrix4x4[] _matrices;
	
	private static readonly int matricesId = Shader.PropertyToID("_Matrices");
	private static MaterialPropertyBlock _propertyBlock;
	
	private void OnEnable() {
		const int stride = 16 * 4;
		int length = resolution * resolution * resolution;
		_matricesBuffer = new ComputeBuffer(length, stride);
		_matrices = new Matrix4x4[length];
		
		for (int x = 0; x < resolution; x++) {
			for (int y = 0; y < resolution; y++) {
				for (int z = 0; z < resolution; z++) {
					_matrices[x + y * resolution + z * resolution * resolution] = Matrix4x4.TRS(GetVoxelPosition(x, y, z), Quaternion.identity, Vector3.one * scale);
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
