using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Net;
using System;

[Serializable]
class SeekJSONType {
	public string type = "";
}

[Serializable]
class Envelope<Value> {
	public Value value = default(Value);
}

[Serializable]
public class GravityMessage {
	public float x = 0;
	public float y = 0;
	public float z = 0;
	public string userId = "";
	public string name = "";

	public override string ToString ()
	{
		return string.Format ("<GravityMessage[x:{0}, y:{1}, z:{2}, userId:{3}, name:{4}]>", x, y, z, userId, name);
	}
}

[Serializable]
class PressButtonMessage {
	public string value = "";
	public string userId = "";
	public string name = "";
}

class PlayerInput {
	public GravityMessage gravity = default(GravityMessage);
	public bool pressedA = false;
	public bool pressedB = false;
	public bool pressedC = false;
	public bool pressedUp = false;
	public bool pressedDown = false;
	public bool pressedLeft = false;
	public bool pressedRight = false;
}

public class WebsocketBehaviour : MonoBehaviour {
	public GameObject player;
	public Rigidbody bullet;
	private Dictionary<string, PlayerInput> playersInput;
	private Dictionary<string, GameObject> players;

	WebSocket ws;

	void Start () {
		this.players = new Dictionary<string, GameObject> ();
		this.playersInput = new Dictionary<string, PlayerInput> ();
		Connect ();
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.Space)) {
			Send ("Test Message");
			Instantiate(player, transform.position, transform.rotation);
		}
	}

	void FixedUpdate () {
		foreach (KeyValuePair<string, PlayerInput> pair in playersInput) {
			if (!players.ContainsKey (pair.Key)) {
				GameObject playerClone = (GameObject) Instantiate(this.player, transform.position, transform.rotation);
				playerClone.name = String.Format ("Player_{0}", pair.Key);
				players.Add (pair.Key, playerClone);
			}
			var playerObject = players [pair.Key];
			var player = playerObject.GetComponent<PlayerBehaviour> ();
			var input = playersInput [pair.Key];

			player.AddGravity(input.gravity);

			if (input.pressedA) {
				player.InputA ();
				Debug.Log ("press A");
				input.pressedA = false;
			}
			if (input.pressedB) {
				player.InputB ();
				input.pressedB = false;
				Debug.Log ("press B");
			}
			if (input.pressedC) {
				player.InputC ();
				input.pressedC = false;
				Debug.Log ("press C");
			}
			if (input.pressedUp) {
				player.InputUp ();
				input.pressedUp = false;
				Debug.Log ("press Up");
			}
			if (input.pressedDown) {
				player.InputDown ();
				Debug.Log ("press Down");
			}
			if (input.pressedLeft) {
				player.InputLeft ();
				Debug.Log ("press Left");
			}
			if (input.pressedRight) {
				player.InputRight ();
				Debug.Log ("press Right");
			}
		}
	}

	void OnApplicationQuit () {
		Disconnect ();
	}

	PlayerInput getPlayerInput(string userId) {
		if (!this.playersInput.ContainsKey(userId)) {
			this.playersInput.Add(userId, new PlayerInput());
		}
		return this.playersInput[userId];
	}

	void Connect () {
		string WSAddress = "ws://au8slot.herokuapp.com/view/ws/";
		//string WSAddress = "ws://localhost:9000/view/ws/";
		string WSSubProtocol = "slot-view";

		ws = new WebSocket (WSAddress, new string[1]{WSSubProtocol});

		ws.OnOpen += (sender, e) => {
			Debug.Log ("WebSocket Open");
		};

		ws.OnMessage += (sender, e) => {
			//Debug.Log ("WebSocket Message Type: " + e.Type + ", Data: " + e.Data);
			var m = JsonUtility.FromJson<SeekJSONType>(e.Data);
			if (m.type == "Gravity") {
				var n = JsonUtility.FromJson<Envelope<GravityMessage>>(e.Data);
				var gravity = n.value;
				var playerInput = this.getPlayerInput(gravity.userId);
				playerInput.gravity = gravity;
			} else if (m.type == "Event") {
				var n = JsonUtility.FromJson<Envelope<PressButtonMessage>>(e.Data);
				var pressButton = n.value;
				var playerInput = this.getPlayerInput(pressButton.userId);
				if (pressButton.value == "a") {
					playerInput.pressedA = true;
				}
				if (pressButton.value == "b") {
					playerInput.pressedB = true;
				}
				if (pressButton.value == "c") {
					playerInput.pressedC = true;
				}
				if (pressButton.value == "up") {
					playerInput.pressedUp = true;
				}
				if (pressButton.value == "down") {
					playerInput.pressedDown = true;
				}
				if (pressButton.value == "left") {
					playerInput.pressedLeft = true;
				}
				if (pressButton.value == "right") {
					playerInput.pressedRight = true;
				}
				if (pressButton.value == "release_down") {
					playerInput.pressedDown = false;
				}
				if (pressButton.value == "release_left") {
					playerInput.pressedLeft = false;
				}
				if (pressButton.value == "release_right") {
					playerInput.pressedRight = false;
				}
			}
			// Debug.Log(m.value);
		};

		ws.OnError += (sender, e) => {
			Debug.Log ("WebSocket Error Message: " + e.Message);
		};

		ws.OnClose += (sender, e) => {
			Debug.Log ("WebSocket Close");
		};

		ws.Connect ();

	}

	void Disconnect () {
		ws.Close ();
		ws = null;
	}

	void Send (string message) {
		ws.Send (message);
	}
		
}
