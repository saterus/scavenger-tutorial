using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using ListSampling;


public class BoardManager : MonoBehaviour
{

	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}

		public int Rand ()
		{
			return Random.Range (minimum, maximum + 1);
		}
	}

	public int columns = 8;
	public int rows = 8;
	public Count wallCount = new Count (5, 9);
	public Count foodCount = new Count (1, 5);
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	private Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3> ();

	Vector3 exitPosition()
	{
		return new Vector3(columns - 1, rows - 1, 0f);
	}

	void InitializeList ()
	{
		gridPositions.Clear ();
		foreach (Vector3 position in InnerGameBoardIterator()) {
			gridPositions.Add (position);
		}
	}

	void BoardSetup ()
	{
		boardHolder = new GameObject ("Board").transform;

		foreach (Vector3 position in OuterGameBoardIterator()) {					
			GameObject toInstantiate;

			if (IsEdgePosition (position)) {
				toInstantiate = outerWallTiles.SampleFrom();
			} else {
				toInstantiate = floorTiles.SampleFrom();
			}
				
			GameObject instance = Instantiate (toInstantiate, position, Quaternion.identity) as GameObject;

			instance.transform.SetParent (boardHolder);

		}
	}

	void LayoutObjectAtRandom (GameObject[] tiles, Count range)
	{
		int objectCount = range.Rand ();
		for (int i = 0; i < objectCount; i++) {
			Instantiate (tiles.SampleFrom(), RandomPosition (), Quaternion.identity);
		}
	}

	Vector3 RandomPosition ()
	{
		Vector3 position = gridPositions.SampleFrom();
		gridPositions.Remove (position);
		return position;
	}


	IEnumerable<Vector3> InnerGameBoardIterator ()
	{
		for (int x = 1; x < columns - 1; x++) {
			for (int y = 1; y < rows - 1; y++) {
				yield return new Vector3 (x, y);
			}
		}
	}

	IEnumerable<Vector3> OuterGameBoardIterator ()
	{
		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				yield return new Vector3 (x, y);
			}
		}
	}

	bool IsEdgePosition (Vector3 position)
	{
		return position.x == -1 || position.x == columns || position.y == -1 || position.y == rows;
	}

	Count enemyCountForLevel(int level)
	{
		int num = (int) Mathf.Log (level, 2f);
		return new Count (num, num);
	}
		
	public void SetupScene (int level)
	{
		InitializeList ();
		BoardSetup ();
		LayoutObjectAtRandom (wallTiles, wallCount);
		LayoutObjectAtRandom (foodTiles, foodCount);
		LayoutObjectAtRandom (enemyTiles, enemyCountForLevel (level));
		Instantiate (exit, exitPosition(), Quaternion.identity);
	}
}
