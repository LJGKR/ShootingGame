using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	public string enemyName;
	public int enemyScore;

	public float speed;
    public int health;
	public Sprite[] sprites;

	public float maxShotDelay;
	public float curShotDelay;

	public GameObject bulletObjA;
	public GameObject bulletObjB;
	public GameObject itemCoin;
	public GameObject itemBoom;
	public GameObject itemPower;
	public GameObject player;
	public ObjectManager objectManager;
	public GameManager gameManager;


	SpriteRenderer spriteRenderer;
	Animator anim;

	public int patternIndex;
	public int curPatterCount;
	public int[] maxPatterCount;

	void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		if(enemyName == "B")
			anim = GetComponent<Animator>();
	}

	void OnEnable()
	{
		switch (enemyName)
		{
			case "B":
				health = 2000;
				Invoke("Stop", 2);
				break;
			case "L":
				health = 20;
				break;
			case "M":
				health = 10;
				break;
			case "S":
				health = 5;
				break;
		}
	}

	void Stop()
	{
		if (!gameObject.activeSelf)
			return;

		Rigidbody2D rigid = GetComponent<Rigidbody2D>();
		rigid.velocity = Vector3.zero;

		Invoke("Think", 2);
	}
	void Think()
	{
		if (!gameObject.activeSelf)
			return;

		patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
		curPatterCount = 0;

		switch (patternIndex)
		{
			case 0:
				FireForward();
				break;
			case 1:
				FireShot();
				break;
			case 2:
				FireArc();
				break;
			case 3:
				FireAround();
				break;
		}
	}

	void FireForward()
	{
		GameObject bulletR = objectManager.MakeObj("BulletBossA");
		bulletR.transform.position = transform.position + Vector3.right * 0.5f + Vector3.down * 1.0f;
		GameObject bulletRR = objectManager.MakeObj("BulletBossA");
		bulletRR.transform.position = transform.position + Vector3.right * 0.6f + Vector3.down * 1.0f;
		GameObject bulletL = objectManager.MakeObj("BulletBossA");
		bulletL.transform.position = transform.position + Vector3.left * 0.5f + Vector3.down * 1.0f;
		GameObject bulletLL = objectManager.MakeObj("BulletBossA");
		bulletLL.transform.position = transform.position + Vector3.left * 0.6f + Vector3.down * 1.0f;

		Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
		Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
		Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
		Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();


		rigidR.AddForce(Vector2.down * 6, ForceMode2D.Impulse);
		rigidRR.AddForce(Vector2.down * 6, ForceMode2D.Impulse);
		rigidL.AddForce(Vector2.down * 6, ForceMode2D.Impulse);
		rigidLL.AddForce(Vector2.down * 6, ForceMode2D.Impulse);

		curPatterCount++;

		if(curPatterCount < maxPatterCount[patternIndex])
		{
			Invoke("FireForward", 2);
		}
		else
		{
			Invoke("Think", 3);
		}
	}

	void FireShot()
	{
		for(int i=0; i<5; i++)
		{
			GameObject bullet = objectManager.MakeObj("BulletEnemyB");
			bullet.transform.position = transform.position;

			Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
			Vector2 dirVec = player.transform.position - transform.position;
			Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
			dirVec += ranVec;
			rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
		}

		curPatterCount++;

		if (curPatterCount < maxPatterCount[patternIndex])
		{
			Invoke("FireShot", 3.5f);
		}
		else
		{
			Invoke("Think", 3);
		}
	}

	void FireArc()
	{
		//부채꼴 모양
		GameObject bullet = objectManager.MakeObj("BulletEnemyA");
		bullet.transform.position = transform.position;
		bullet.transform.rotation = Quaternion.identity;

		Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
		Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 *  curPatterCount / maxPatterCount[patternIndex]), -1);
		rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);

		curPatterCount++;

		if (curPatterCount < maxPatterCount[patternIndex])
		{
			Invoke("FireArc", 0.15f);
		}
		else
		{
			Invoke("Think", 3);
		}
	}

	void FireAround()
	{
		int roundNumA = 50;
		int roundNumB = 40;
		int roundNum = curPatterCount % 2 == 0 ? roundNumA : roundNumB;

		for(int i=0; i< roundNum; i++)
		{
			GameObject bullet = objectManager.MakeObj("BulletBossB");
			bullet.transform.position = transform.position;
			bullet.transform.rotation = Quaternion.identity;

			Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
			Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum),
				Mathf.Sin(Mathf.PI * 2 * i / roundNum));
			rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);

			Vector3 rotVec = Vector3.forward * 360 * i / roundNum + Vector3.forward * 90;
			bullet.transform.Rotate(rotVec);
		}
		
		curPatterCount++;

		if (curPatterCount < maxPatterCount[patternIndex])
		{
			Invoke("FireAround", 1f);
		}
		else
		{
			Invoke("Think", 3);
		}
	}

	void Update()
	{
		if (enemyName == "B")
			return;

		Fire();
		Reload();
	}

	void Fire()
	{
		if (curShotDelay < maxShotDelay)
		{
			return;
		}

		if(enemyName == "S")
		{
			GameObject bullet = objectManager.MakeObj("BulletEnemyA");
			bullet.transform.position = transform.position;
			Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

			Vector3 dirVec = player.transform.position - transform.position;
			rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
		}
		else if(enemyName == "L")
		{
			GameObject bulletR = objectManager.MakeObj("BulletEnemyB"); 
			bulletR.transform.position = transform.position + Vector3.right * 0.3f;
			GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
			bulletL.transform.position = transform.position + Vector3.left * 0.3f;

			Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
			Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

			Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
			Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);

			rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
			rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
		}

		curShotDelay = 0;
	}

	void Reload()
	{
		curShotDelay += Time.deltaTime;
	}

	public void OnHit(int dmg)
	{
		if(health < 0)
			return;

		health -= dmg;

		if (enemyName == "B")
		{
			anim.SetTrigger("OnHit");
		}
		else
		{
			spriteRenderer.sprite = sprites[1];
			Invoke("ReturnSprite", 0.1f);
		}

		if (health <= 0)
		{
			Player playerLogic = player.GetComponent<Player>();
			playerLogic.score += enemyScore;

			//아이템 드랍 설정
			int ran = enemyName == "B" ? 0 : Random.Range(0, 10);
			if(ran < 5)
			{
				//아이템 노드랍
				Debug.Log("No Item!");
			}
			else if(ran < 8)
			{
				//코인 드랍
				GameObject itemCoin = objectManager.MakeObj("ItemCoin");
				itemCoin.transform.position = transform.position;
			}
			else if(ran < 9)
			{
				//파워 드랍
				GameObject itemPower = objectManager.MakeObj("ItemPower");
				itemPower.transform.position = transform.position;
			}
			else if(ran < 10)
			{
				//필살기 드랍
				GameObject itemBoom = objectManager.MakeObj("ItemBoom");
				itemBoom.transform.position = transform.position;
			}
			gameObject.SetActive(false);
			transform.rotation = Quaternion.identity;
			gameManager.CallExplosion(transform.position, enemyName);

			if(enemyName == "B")
			{
				gameManager.StageEnd();
			}
		}
	}

	void ReturnSprite()
	{
		spriteRenderer.sprite = sprites[0];
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Border Bullet" && enemyName != "B")
		{
			gameObject.SetActive(false);
			transform.rotation = Quaternion.identity;
		}
		else if(collision.gameObject.tag == "Player Bullet")
		{
			Bullet bullet = collision.gameObject.GetComponent<Bullet>();
			OnHit(bullet.dmg);
			collision.gameObject.SetActive(false);
		}
	}
}
