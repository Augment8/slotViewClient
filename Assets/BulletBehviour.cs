using UnityEngine;
using System.Collections;

public class BulletBehviour : MonoBehaviour {
	public string userId;
	private int lifeTime = 0;
	void Start () {
		
	}
	
	void Update () {
		lifeTime++;
		if (lifeTime > 90) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.CompareTag("Player")) {
			Destroy (gameObject);
			var player = collision.gameObject.GetComponent<PlayerBehaviour> ();
			player.lastAttackedUserId = userId;
		}
	}
}
