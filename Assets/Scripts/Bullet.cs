using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public int dmg;
	public bool isRotate;

	void Update()
	{
		if (isRotate)
		{
			transform.Rotate(Vector3.forward * 10); //z�� ȸ��
		}
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Border Bullet")
		{
			gameObject.SetActive(false);
		}
	}
}
