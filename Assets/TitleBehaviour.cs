using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class TitleBehaviour : MonoBehaviour {
	public int remainTime = 4;
	public string nextSceneName = "main";
	public TextMesh countText = null;

	IEnumerator LateStart(float time, string sceneName) {
		for (int i = remainTime; i >= 1; i--) {
			countText.text = string.Format("{0}", i);
			yield return new WaitForSeconds (1);
		}
		countText.text = "Start";
		SceneManager.LoadScene(sceneName);
	}

	// Use this for initialization
	void Start () {
		StartCoroutine(LateStart(remainTime, nextSceneName));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
