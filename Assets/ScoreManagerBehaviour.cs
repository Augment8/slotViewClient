using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Score {
	public string userId;
	public int score;
	public TextMesh scoreText;
	public void up() {
		this.score++;
	}

	public void down() {
		this.score--;
	}

	public void draw () {
		var n = GameObject.Find (userId);
		if (n == null) {
			return;
		}
		PlayerBehaviour player = n.GetComponent<PlayerBehaviour>();
		if (player == null) {
			return;
		}
		scoreText.text = string.Format ("{0} {1}", player.getUsername(), this.score);
	}
}

public class ScoreManagerBehaviour : MonoBehaviour {
	public TextMesh scoreTextPrefab;
	private Dictionary<string, Score> scores;

	// Use this for initialization
	void Start () {
		this.scores = new Dictionary<string, Score> ();
	}

	Score reset(string userId) {
		var score = new Score ();
		score.userId = userId;
		score.score = 0;
		score.scoreText = (TextMesh) Instantiate (scoreTextPrefab, new Vector3(17, 20 - scores.Count, 0), Quaternion.identity);
		scores.Add (userId, score);
		return score;
	}

	public void up(string userId) {
		Score score;
		if (!scores.ContainsKey (userId)) {
			score = reset (userId);
		} else {
			score = scores [userId];
		}
		score.up ();
	}

	public void down(string userId) {
		Score score;
		if (!scores.ContainsKey (userId)) {
			score = reset (userId);
		} else {
			score = scores [userId];
		}
		score.down ();
	}
	
	// Update is called once per frame
	void Update () {
		foreach (KeyValuePair<string, Score> pair in scores) {
			pair.Value.draw ();
		}
	}
}
