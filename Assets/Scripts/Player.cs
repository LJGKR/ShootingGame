using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public bool isTouchTop;
	public bool isTouchBottom;
	public bool isTouchRight;
	public bool isTouchLeft;
	public bool isHit;
	public bool isBoomTime;


	public int score;
	public int life;
	public float speed;
	public int power;
	public int maxPower;
	public int boom;
	public int maxBoom;
	public float maxShotDelay;
	public float curShotDelay;

	//public bool[] joyControl;
	//public bool isControl;
	//public bool isButtonA;
	//public bool isButtonB;

	public GameObject bulletObjA;
	public GameObject bulletObjB;
	public GameObject boomEffect;

	public GameManager gameManager;
	public ObjectManager objectManager;
	public SpriteRenderer spriteRenderer;

	Animator anim;

	public GameObject[] followers;


	void Awake()
	{
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		Move();
		Fire();
		Reload();
		Boom();
	}

	//public void JoyPanel(int type)
	//{
	//	for (int i = 0; i < 9; i++)
	//	{
	//		joyControl[i] = i == type;
	//	}
	//}

	//public void JoyDown()
	//{
	//	isControl = true;
	//}

	//public void JoyUp()
	//{
	//	isControl = false;
	//}

	void Move()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		//조이스틱 이동 설정
		//if (joyControl[0]) { h = -1; v = 1; }
		//if (joyControl[1]) { h = 0; v = 1; }
		//if (joyControl[2]) { h = 1; v = 1; }
		//if (joyControl[3]) { h = -1; v = 0; }
		//if (joyControl[4]) { h = 0; v = 0; }
		//if (joyControl[5]) { h = 1; v = 0; }
		//if (joyControl[6]) { h = -1; v = -1; }
		//if (joyControl[7]) { h = 0; v = -1; }
		//if (joyControl[8]) { h = 1; v = -1; }

		if ((h == 1 && isTouchRight) || (h == -1 && isTouchLeft) )
		{
			h = 0;
		}

		if ((v == 1 && isTouchTop) || (v == -1 && isTouchBottom) )
		{
			v = 0;
		}
		Vector3 curPos = transform.position; //플레이어 현재 위치를 가져옴
		Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

		transform.position = curPos + nextPos;

		if ((Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal")))
		{
			anim.SetInteger("Input", (int)h);
		}
	}

	//public void ButtonADown()
	//{
	//	isButtonA = true;
	//}

	//public void ButtonAUp()
	//{
	//	isButtonA = false;
	//}

	//public void ButtonBDown()
	//{
	//	isButtonB = true;
	//}

	void Fire()
	{
		if (!Input.GetButton("Fire1"))
			return;

		//if (!isButtonA)
		//	return;

		if (curShotDelay < maxShotDelay)
		{
			return;
		}

		switch (power)
		{
			case 1:
				//파워 1짜리 총
				GameObject bullet = objectManager.MakeObj("BulletPlayerA");
				bullet.transform.position = transform.position;
				Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
				rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				break;
			case 2:
				GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
				bulletR.transform.position = transform.position + Vector3.right * 0.2f;
				GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
				bulletL.transform.position = transform.position + Vector3.left * 0.2f;

				Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
				Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

				rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

				break;
			default:
				GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
				bulletRR.transform.position = transform.position + Vector3.right * 0.4f;
				GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
				bulletCC.transform.position = transform.position;
				GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
				bulletLL.transform.position = transform.position + Vector3.left * 0.4f;

				Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
				Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
				Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

				rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
				rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

				break;
		}

		curShotDelay = 0;
	}

	void Reload()
	{
		curShotDelay += Time.deltaTime;
	}

	void Boom()
	{
		if (!Input.GetButton("Fire2"))
			return;

		//if (!isButtonB)
		//	return;

		if (isBoomTime)
			return;

		if (boom == 0)
			return;

			boom--;
			isBoomTime = true;
			gameManager.UpdateBoomIcon(boom);

			boomEffect.SetActive(true);
			Invoke("OffBoomEffect", 4f);

			//Enemy 태그로 되어있는 오브젝트를 배열에 저장
			GameObject[] enemiesL = objectManager.GetPool("EnemyL");
			GameObject[] enemiesM = objectManager.GetPool("EnemyM");
			GameObject[] enemiesS = objectManager.GetPool("EnemyS");

			for (int i = 0; i < enemiesL.Length; i++)
			{
				if (enemiesL[i].activeSelf)
				{
					Enemy enemyLogic = enemiesL[i].GetComponent<Enemy>();
					enemyLogic.OnHit(1000);
				}
			}

			for (int i = 0; i < enemiesM.Length; i++)
			{
				if (enemiesM[i].activeSelf)
				{
					Enemy enemyLogic = enemiesM[i].GetComponent<Enemy>();
					enemyLogic.OnHit(1000);
				}
			}

			for (int i = 0; i < enemiesS.Length; i++)
			{
				if (enemiesS[i].activeSelf)
				{
					Enemy enemyLogic = enemiesS[i].GetComponent<Enemy>();
					enemyLogic.OnHit(1000);
				}
			}

			GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
			GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
			for (int i = 0; i < bulletsA.Length; i++)
			{
				if (bulletsA[i].activeSelf)
				{
					bulletsA[i].SetActive(false);
				}
			}

			for (int i = 0; i < bulletsB.Length; i++)
			{
				if (bulletsB[i].activeSelf)
				{
					bulletsB[i].SetActive(false);
				}
			}
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
				case "Bottom":
					isTouchBottom = true;
					break;
				case "Right":
					isTouchRight = true;
					break;
				case "Left":
					isTouchLeft = true;
					break;
			}
        }
		else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Enemy Bullet")
		{
			if (isHit)
				return;

			isHit = true;
			life--;
			gameManager.UpdateLifeIcon(life);
			gameManager.CallExplosion(transform.position, "P");

			if(life == 0)
			{
				gameManager.GameOver();
			}
			else
			{
				gameManager.RespawnPlayer();
			}

			gameObject.SetActive(false);
			collision.gameObject.SetActive(false);
		}
		else if(collision.gameObject.tag == "Item")
		{
			Item item = collision.gameObject.GetComponent<Item>();
			switch (item.type){
				case "Coin":
					score += 1000;
					break;
				case "Power":
					if (power == maxPower)
					{
						score += 500;
					}
					else
					{
						power++;
						AddFollower();
					}
					break;
				case "Boom":
					if (boom == maxBoom)
					{
						score += 500;
					}
					else
					{
						boom++;
						gameManager.UpdateBoomIcon(boom);
					}
					break;
			}
			collision.gameObject.SetActive(false);
		}
	}

	void AddFollower()
	{
		if(power == 4)
		{
			followers[0].SetActive(true);
		}
		else if(power == 5)
		{
			followers[1].SetActive(true);
		}
		else if(power == 6)
		{
			followers[2].SetActive(true);
		}
	}

	void OffBoomEffect()
	{
		boomEffect.SetActive(false);
		isBoomTime = false;
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Border")
		{
			switch (collision.gameObject.name)
			{
				case "Top":
					isTouchTop = false;
					break;
				case "Bottom":
					isTouchBottom = false;
					break;
				case "Right":
					isTouchRight = false;
					break;
				case "Left":
					isTouchLeft = false;
					break;
			}
		}
	}
}
