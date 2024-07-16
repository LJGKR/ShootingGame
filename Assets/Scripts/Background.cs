using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Background : MonoBehaviour
{
	public float speed;
	public int startIndex;
	public int endIndex;
	public Transform[] sprites;

	float viewHeight;

	void Awake()
	{
		//카메라의 크기를 구해옴
		viewHeight = Camera.main.orthographicSize * 2;
	}
	void Update()
	{
		Move();
		Scrolling();
	}

	//배경 움직임
	void Move()
	{
		Vector3 curPos = transform.position;
		Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
		transform.position = curPos + nextPos;
	}

	//배경의 움직임에 따라 위치를 바꿔주어 무한 스크롤처럼 보이게
	void Scrolling()
	{
		if (sprites[endIndex].position.y < viewHeight * (-1))
		{
			Vector3 backSpritePos = sprites[startIndex].localPosition;
			Vector3 frontSpritePos = sprites[endIndex].localPosition;

			sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * 10;

			int startIndexSave = startIndex;
			startIndex = endIndex;
			endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
		}
	}
}
