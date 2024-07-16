using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Follower : MonoBehaviour
{
	public float maxShotDelay;
	public float curShotDelay;
	public ObjectManager objectManager;

	public Vector3 followPos;
	public int followDelay;
	public Transform Parent; //따라갈 부모 트랜스폼
	public Queue<Vector3> parentPos;

	void Awake()
	{
		parentPos = new Queue<Vector3>();
	}

	void Update()
	{
		Watch();
		Follow();
		Fire();
		Reload();
	}

	void Watch()
	{
		//선입선출 큐를 활용하여 부모의 위치를 관리
		if (!parentPos.Contains(Parent.position))
		{
			parentPos.Enqueue(Parent.position);
		}

		if(parentPos.Count > followDelay)
		{
			followPos = parentPos.Dequeue();
		}
		else if(parentPos.Count < followDelay)
		{
			followPos = Parent.position;
		}
	}

	void Follow()
	{
		transform.position = followPos;
	}

	void Fire()
	{
		if (!Input.GetButton("Fire1"))
		{
			return;
		}

		if (curShotDelay < maxShotDelay)
		{
			return;
		}

		GameObject bullet = objectManager.MakeObj("BulletFollower");
		bullet.transform.position = transform.position;

		Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
		rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

		curShotDelay = 0;
	}

	void Reload()
	{
		curShotDelay += Time.deltaTime;
	}
}
