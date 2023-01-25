using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Planet : MonoBehaviour {
	private int chunkSize = 16;
	// makes 4x4x4 chunks
	[SerializeField, Range(1, 8)]
	private int chunkCount = 4;

	[HideInInspector]
	public float[][][] chunks;

	[SerializeField]
	private GameObject chunkPrefab;
	
	private List<GameObject> _chunksObjects = new List<GameObject>();
	
	private void Awake() {
		FillChunks();

		for (int x = 0; x < chunkCount; x++) {
			for (int y = 0; y < chunkCount; y++) {
				for (int z = 0; z < chunkCount; z++) {
					Debug.Log(chunks[x][y][z]);
					GameObject chunk = Instantiate(chunkPrefab, transform.position, Quaternion.identity);
					
					chunk.transform.parent = transform;
					chunk.name = $"Chunk {x} {y} {z}";

					MarchingCubes marchingCubes = chunk.GetComponent<MarchingCubes>();
					marchingCubes.offset = new int3(x * chunkSize, y * chunkSize, z * chunkSize);
					marchingCubes.resolution = chunkSize;
					marchingCubes.voxels = chunks;
					
					_chunksObjects.Add(chunk);
				}
			}
		}
	}

	private void FillChunks() {
		int totalSize = chunkSize * chunkCount;
		chunks = new float[totalSize][][];
		
		for (int x = 0; x < totalSize; x++) {
			chunks[x] = new float[totalSize][];
			for (int y = 0; y < totalSize; y++) {
				chunks[x][y] = new float[totalSize];
				for (int z = 0; z < totalSize; z++) {
					chunks[x][y][z] = DistanceFromCenter(x, y, z, totalSize);
				}
			}
		}
	}
	
	private float DistanceFromCenter(int x, int y, int z, int totalSize) {
		return Vector3.Distance(new Vector3(x, y, z), new Vector3(totalSize/2f - 0.5f, totalSize/2f - 0.5f, totalSize/2f - 0.5f)) / (totalSize-1);
	}
}
