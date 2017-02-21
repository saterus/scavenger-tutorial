﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public static GameManager instance = null;
	public BoardManager boardScript;
	public int playerFoodPoints = 100;
	public float turnDelay = 0.1f;
	[HideInInspector]public bool playersTurn = true;

	private int level = 3;
	private List<Enemy> enemies;
	private bool enemiesMoving;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);

		enemies = new List<Enemy> ();
		boardScript = GetComponent<BoardManager> ();
		InitGame ();
	}

	void InitGame ()
	{
		enemies.Clear ();
		boardScript.SetupScene (level);
	}

	public void GameOver ()
	{
		enabled = false;
	}

	void Update ()
	{
		if (playersTurn || enemiesMoving) {
			return;
		}

		StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList (Enemy enemy)
	{
		enemies.Add (enemy);
	}

	IEnumerator MoveEnemies ()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		} else {
			foreach (var enemy in enemies) {
				enemy.MoveEnemy ();
				yield return new WaitForSeconds (enemy.moveTime);
			}
		}

		playersTurn = true;
		enemiesMoving = false;
	}
}
