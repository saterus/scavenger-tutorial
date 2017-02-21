using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class Player : MovingObject
{
	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;

	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private int food;

	protected override void Start ()
	{
		animator = GetComponent<Animator> ();

		food = GameManager.instance.playerFoodPoints;
		ChangeFood (0);

		base.Start ();
	}

	private void OnDisable ()
	{
		GameManager.instance.playerFoodPoints = food;
	}

	void Update ()
	{
		if (!GameManager.instance.playersTurn)
			return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int)Input.GetAxisRaw ("Horizontal");
		vertical = (int)Input.GetAxisRaw ("Vertical");

		if (horizontal != 0) {
			vertical = 0;
		}

		if (horizontal != 0 || vertical != 0) {
			AttemptMove<Wall> (horizontal, vertical);
		}
	}

	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		food--; // silently lose a food for the day.
		ChangeFood (0);
		base.AttemptMove<T> (xDir, yDir);

		RaycastHit2D hit;

		if (Move (xDir, yDir, out hit)) {
			// play sound effect
			SoundManager.instance.RandomSfx (moveSound1, moveSound2);
		}

		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			ChangeFood (pointsPerFood);
			SoundManager.instance.RandomSfx (eatSound1, eatSound2);
			other.gameObject.SetActive (false);
		} else if (other.tag == "Soda") {
			ChangeFood (pointsPerSoda);
			SoundManager.instance.RandomSfx (drinkSound1, drinkSound2);
			other.gameObject.SetActive (false);
		}
	}

	protected override void OnCantMove <T> (T component)
	{
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}

	public void LoseFood (int loss)
	{
		animator.SetTrigger ("playerHit");
		ChangeFood (-loss);
		CheckIfGameOver ();
	}

	private void Restart ()
	{
		SceneManager.LoadScene (0);
	}

	private void CheckIfGameOver ()
	{
		if (food <= 0) {
			SoundManager.instance.PlaySingle (gameOverSound);
			SoundManager.instance.musicSource.Stop ();
			GameManager.instance.GameOver ();
		}
	}

	private void ChangeFood (int change)
	{
		food += change;
		if (change > 0) {
			foodText.text = "+ " + change + " Food: " + food;	
		} else if (change < 0) {
			foodText.text = "-" + change + " Food: " + food;	
		} else {
			foodText.text = "Food: " + food;
		}
	}

}
