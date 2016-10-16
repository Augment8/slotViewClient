using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject target;
	private Vector3 offsetPosition;
	//private StageController stageController;
	private float distance = 10f;//プレイヤーキャラまでの距離。固定で設定

	//カメラの表示領域表示用
	private Vector3 cameraBottomLeft;
	private Vector3 cameraTopLeft;
	private Vector3 cameraBottomRight;
	private Vector3 cameraTopRight;
	public float cameraRangeWidth;
	public float cameraRangeHeight;

	//カメラの表示領域を緑ラインで表示
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(cameraBottomLeft, cameraTopLeft);
		Gizmos.DrawLine(cameraTopLeft, cameraTopRight);
		Gizmos.DrawLine(cameraTopRight, cameraBottomRight);
		Gizmos.DrawLine(cameraBottomRight, cameraBottomLeft);
	}

	void Start()
	{
		//プレイヤーキャラを取得
		target = GameObject.FindGameObjectWithTag("Player");
		offsetPosition = transform.position - target.transform.position;
		//ステージコントローラーを取得
		//stageController = GameObject.Find("Stage").GetComponent();
	}

	void Update()
	{
		Vector3 newPosition;//カメラの目標座標
		Vector3 limitPosition;//制限されたカメラの目標座標
		float newX = 0f;
		float newY = 0f;
		//プレイヤーキャラの位置にカメラの座標を設定する。キャラのちょっと上にする
		newPosition = target.transform.position + offsetPosition + Vector3.up*3f;
		//ビューポート座標をワールド座標に変換
		cameraBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
		cameraTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance));
		cameraTopLeft = new Vector3(cameraBottomLeft.x, cameraTopRight.y, cameraBottomLeft.z);
		cameraBottomRight = new Vector3(cameraTopRight.x, cameraBottomLeft.y, cameraTopRight.z);
		cameraRangeWidth = Vector3.Distance(cameraBottomLeft, cameraBottomRight);
		cameraRangeHeight = Vector3.Distance(cameraBottomLeft, cameraTopLeft);
		//カメラの稼働領域をステージ領域に制限
		//newX = Mathf.Clamp(newPosition.x, stageController.StageRect.xMin + cameraRangeWidth/2, stageController.StageRect.xMax-cameraRangeWidth/2);
		//newY = Mathf.Clamp(newPosition.y, 0, stageController.StageRect.yMax - cameraRangeHeight/2);
		//座標をカメラ位置に設定
		limitPosition = new Vector3(newX,newY,this.transform.position.z);
		transform.position = limitPosition;
	}

}