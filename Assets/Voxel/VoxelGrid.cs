using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VoxelGrid : MonoBehaviour {
	[HideInInspector]
	public int resolution = 16;
	[HideInInspector]
	public float threshold = 0.5f;

	// 0-1 range, 0 is empty, 1 is full
	[HideInInspector]
	public float[][][] voxels;
	
	[SerializeField]
	private Mesh _mesh;
	[SerializeField]
	private Material _material;

	[FormerlySerializedAs("scale")]
	[SerializeField, Range(0.01f, 0.3f)]
	private float voxelScale = 0.1f;
	
	[SerializeField]
	private Gradient _gradient = new Gradient();
	
	private ComputeBuffer _matricesBuffer;
	private Matrix4x4[] _matrices;
	
	private static readonly int matricesId = Shader.PropertyToID("_Matrices");
	private static MaterialPropertyBlock _propertyBlock;
	
	public void Initialize() {
		const int stride = 16 * 4;
		int length = resolution * resolution * resolution;
		_matricesBuffer = new ComputeBuffer(length, stride);
		_matrices = new Matrix4x4[length];

		_propertyBlock ??= new MaterialPropertyBlock();
	}

	private void OnDisable() {
		_matricesBuffer.Release();
		
		_matricesBuffer = null;
	}

	private void Update() {
		if (_matricesBuffer == null) return;
		
		for (int x = 0; x < resolution; x++) {
			for (int y = 0; y < resolution; y++) {
				for (int z = 0; z < resolution; z++) {
					_matrices[x + y * resolution + z * resolution * resolution] = Matrix4x4.TRS(GetVoxelPosition(x, y, z), Quaternion.identity, Vector3.one * voxelScale);
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
			Initialize();
		}
	}

	private Vector3 GetVoxelPosition(int x, int y, int z) {
		return (new Vector3(x, y, z) + transform.position);
	}
}
