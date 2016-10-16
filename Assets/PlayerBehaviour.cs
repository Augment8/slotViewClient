using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : MonoBehaviour {
	public Rigidbody bullet;
	public int maxBulletNum = 8;
	public float bulletSpeed = 200;
	public int hitPoint = 0;
	public TextMesh nameMesh;
	public TextMesh nameMesh2;
	public string lastAttackedUserId;
	public float maxSpeed = 5;
	public float Speed = 10;
	private GravityMessage gravity = null;
	private int bulletNum = 0;
	private string username;

	public void InputA () {
		Attack (1);
	}

	public void InputB () {
		Attack (-1);
	}

	public void InputC() {
		Jump ();
	}

	public void InputUp () {
		Jump ();
	}

	public void InputDown () {
		var body = GetComponent<Rigidbody>();
		body.AddForce (0, -100, 0);
	}

	public void InputLeft () {
		var body = GetComponent<Rigidbody>();
		if (body.velocity.x > 0) {
			body.velocity = new Vector3 (0, body.velocity.y, 0);
		}
		if (body.velocity.x >= -maxSpeed) {
			body.AddForce (-Speed, 0, 0);
		}
	}

	public void InputRight () {
		var body = GetComponent<Rigidbody>();
		if (body.velocity.x < 0) {
			body.velocity = new Vector3 (0, body.velocity.y, 0);
		}
		if (body.velocity.x <= maxSpeed) {
			body.AddForce (Speed, 0, 0);
		}
	}


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (gravity != null) {
			username = gravity.name;
			nameMesh.text = gravity.name;
			nameMesh2.text = gravity.name;
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.CompareTag("floor")) {
		}

		if (collision.gameObject.CompareTag ("die")) {
			this.Die ();
		}

		if (collision.gameObject.CompareTag ("bullet")) {
			if (this.transform != collision.transform.parent) {
				Debug.Log ("Hit bullet");
				Rigidbody bullet_body = collision.gameObject.GetComponent<Rigidbody> ();
				var body = GetComponent<Rigidbody> ();
				body.AddForce ((body.position - bullet_body.position + new Vector3(0,3,0)) * 3 * hitPoint);
				hitPoint += 10;
			}
		}
	}

	public void AddGravity (GravityMessage gravity) {
		this.gravity = gravity;
		var body = GetComponent<Rigidbody>();
		var gun = transform.FindChild ("gun").gameObject;
		gun.transform.rotation = Quaternion.LookRotation (new Vector3( -gravity.y, -gravity.x, 0), Vector3.forward);
		// body.AddForce (new Vector3 (gravity.x, 0, -gravity.z));

	}

	public void RemoveBullet() {
		bulletNum--;
	}

	private void Bang(Vector3 force, Vector3 position) {
		var bullet_body = (Rigidbody) Instantiate(bullet, position, Quaternion.identity);
		bullet_body.AddForce(force);

		BulletBehviour bullet_ = bullet_body.GetComponent<BulletBehviour>();
		bullet_.userId = this.name;

		bullet_body.transform.parent = this.transform;

		bulletNum++;
	}

	public void Attack (int neg) {
		if (this.transform.childCount > maxBulletNum) {
			return;
		}
		var body = GetComponent<Rigidbody>();
		var gun = transform.FindChild ("gun").gameObject;
		var rotation = gun.transform.rotation;

		// var force = new Vector3 (neg*bulletSpeed, 3, 0);
		var force = new Vector3 (this.gravity.x * 200 , -this.gravity.y * 200,  0);
		Bang (force, new Vector3 (this.gravity.x, -this.gravity.y, 0) / 6f + body.position);
		//Bang (new Vector3(neg * 60,600,0), new Vector3 (neg, 2, 0) * 0.8f + body.position);
		//Bang (force + new Vector3(0,400,0), new Vector3 (neg, 1, 0) * 0.8f + body.position);
		//body.AddForce (-neg*200,0,0);
	}

	public void Jump () {
		if (this.transform.childCount > maxBulletNum) {
			return;
		}
		var body = GetComponent<Rigidbody>();
		// var force = new Vector3 (gravity.x*4, -600f, 0);
		// var bullet_position = new Vector3 (0, -0.8f, 0) + body.position;
		// Bang (force, bullet_position);
		body.velocity = new Vector3( body.velocity.x, 0, 0);
		body.AddForce (0, 800 * (gravity.y / 10 + 1), 0);
		/*
		 * var body = GetComponent<Rigidbody>();
		if (jumpNum < 2) {
			body.AddForce (new Vector3 (0, 300, 0));
			jumpNum++;
		}
		*/

	}

	public void Die(){
		var body = GetComponent<Rigidbody>();
		body.velocity = Vector3.zero;
		body.position = new Vector3(Random.value * 40 - 20, 12, 0);
		hitPoint = 0;
		var scoreManager = GameObject.Find ("ScoreManager");
		var score = scoreManager.GetComponent<ScoreManagerBehaviour> ();
		score.down (this.name);

		if (lastAttackedUserId != null) {
			score.up (lastAttackedUserId);
			lastAttackedUserId = null;
		}
	}

	public string getUsername() {
		return username;
	}
}
