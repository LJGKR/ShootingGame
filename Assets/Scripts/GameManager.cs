using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
	public int stage;
	public Animator stageAnim;
	public Animator clearAnim;
	public Animator fadeAnim;
	public Transform playerPos;

    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

	public Player playersc;
	public GameObject player;
	public Text scoreText;
	public Image[] lifeImage;
	public Image[] boomImage;
	public GameObject gameOverSet;
	public ObjectManager objectManager;

	public List<Spawn> spawnList;
	public int spawnIndex;
	public bool spawnEnd;

	void Awake()
	{
		enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB"};
		spawnList = new List<Spawn>();
		StageStart();
	}

	public void StageStart()
	{
		//Stage UI�ε�
		stageAnim.SetTrigger("On");
		stageAnim.GetComponent<Text>().text = "STAGE " + stage + "\nStart";
		clearAnim.GetComponent<Text>().text = "STAGE " + stage + "\nClear";
		//Enemy Spawn ���� �о����
		ReadSpawnFile();

		//Fade In �������
		fadeAnim.SetTrigger("In");
	}

	public void StageEnd()
	{
		//Stage Clear UI�ε�
		clearAnim.SetTrigger("On");

		//Fade Out �������
		fadeAnim.SetTrigger("Out");

		//�÷��̾� ������ �ٽ� ���
		player.transform.position = playerPos.position;

		//Stage����
		stage++;
		if(stage > 2)
		{
			Invoke("GameOver", 5f);
		}
		else
			Invoke("StageStart", 5f);
	}
	void ReadSpawnFile()
	{
		//���� �ʱ�ȭ
		spawnList.Clear();
		spawnIndex = 0;
		spawnEnd = false;

		//������ Ŀ���� ���� �б�
		TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset; //�ؽ�Ʈ ���� �ҷ���
		StringReader reader = new StringReader(textFile.text);

		while(reader != null)
		{
			string line = reader.ReadLine(); //������ ���� �о� ��Ʈ�� ������ ����

			if(line == null)
			{
				break;
			}

			Spawn spawnData = new Spawn();
			spawnData.spawnDelay = float.Parse(line.Split(',')[0]);  //,�� �������� ������ ù��° ���� ����
			spawnData.type = line.Split(',')[1];
			spawnData.point = int.Parse(line.Split(',')[2]);
			spawnList.Add(spawnData);
		}
		reader.Close(); //�ؽ�Ʈ���� �ݱ�
		nextSpawnDelay = spawnList[0].spawnDelay;
	}

	void Update()
	{
		curSpawnDelay += Time.deltaTime;

		if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
		{
			spawnEnemy();
			curSpawnDelay = 0;
		}

		//UI ���� ����
		Player playerLogic = player.GetComponent<Player>();
		scoreText.text = string.Format("{0:n0}", playerLogic.score);
	}

	void spawnEnemy()
	{
		int enemyIndex = 0;

		switch (spawnList[spawnIndex].type)
		{
			case "S":
				enemyIndex = 0;
				break;
			case "M":
				enemyIndex = 1;
				break;
			case "L":
				enemyIndex = 2;
				break;
			case "B":
				enemyIndex = 3;
				break;
		}

		int enemyPoint = spawnList[spawnIndex].point;

		GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
		enemy.transform.position = spawnPoints[enemyPoint].position;

		Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
		Enemy enemyLogic = enemy.GetComponent<Enemy>();
		enemyLogic.player = player;
		enemyLogic.gameManager = this;
		enemyLogic.objectManager = objectManager;

		if(enemyPoint == 6 || enemyPoint == 8)
		{
			enemy.transform.Rotate(Vector3.back * 90);
			rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
		}
		else if(enemyPoint == 5 || enemyPoint == 7) 
		{
			enemy.transform.Rotate(Vector3.forward * 90);
			rigid.velocity = new Vector2(enemyLogic.speed, -1);
		}
		else
		{
			rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
		}

		//������ �ε��� ����
		spawnIndex++;
		if(spawnIndex == spawnList.Count)
		{
			spawnEnd = true;
			return;
		}

		//���� ������ ������ ����
		nextSpawnDelay = spawnList[spawnIndex].spawnDelay;
	}

	public void UpdateLifeIcon(int life)
	{
		for(int index=0; index<3; index++)
		{
			lifeImage[index].color = new Color(1, 1, 1, 0);
		}

		for (int index = 0; index < life; index++)
		{
			lifeImage[index].color = new Color(1, 1, 1, 1);
		}
	}

	public void UpdateBoomIcon(int boom)
	{
		for (int index = 0; index < 3; index++)
		{
			boomImage[index].color = new Color(1, 1, 1, 0);
		}

		for (int index = 0; index < boom; index++)
		{
			boomImage[index].color = new Color(1, 1, 1, 1);
		}
	}

	public void RespawnPlayer()
	{
		Invoke("RespawnPlayerExe", 2f);
	}

	void RespawnPlayerExe()
	{
		player.transform.position = Vector3.down * 3.5f;
		player.SetActive(true);
		playersc.spriteRenderer.color = new Color(1, 1, 1, 0.5f);
		Invoke("NoHit", 2f);
	}

	void NoHit()
	{
		Player playerLogic = player.GetComponent<Player>();
		playerLogic.isHit = false;
		playersc.spriteRenderer.color = new Color(1, 1, 1, 1);
	}
	public void CallExplosion(Vector3 pos, string type)
	{
		GameObject explosion = objectManager.MakeObj("Explosion");
		Explosion explosionLogic = explosion.GetComponent<Explosion>();

		explosion.transform.position = pos;
		explosionLogic.StartExplosion(type);
	}

	public void GameOver()
	{
		gameOverSet.SetActive(true);
	}

	public void GameRetry()
	{
		SceneManager.LoadScene(0);
	}
}
