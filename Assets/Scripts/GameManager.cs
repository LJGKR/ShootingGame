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
		//Stage UI로딩
		stageAnim.SetTrigger("On");
		stageAnim.GetComponent<Text>().text = "STAGE " + stage + "\nStart";
		clearAnim.GetComponent<Text>().text = "STAGE " + stage + "\nClear";
		//Enemy Spawn 파일 읽어오기
		ReadSpawnFile();

		//Fade In 흐려지기
		fadeAnim.SetTrigger("In");
	}

	public void StageEnd()
	{
		//Stage Clear UI로딩
		clearAnim.SetTrigger("On");

		//Fade Out 밝아지기
		fadeAnim.SetTrigger("Out");

		//플레이어 포지션 다시 잡기
		player.transform.position = playerPos.position;

		//Stage증가
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
		//변수 초기화
		spawnList.Clear();
		spawnIndex = 0;
		spawnEnd = false;

		//리스폰 커스텀 파일 읽기
		TextAsset textFile = Resources.Load("Stage " + stage) as TextAsset; //텍스트 파일 불러옴
		StringReader reader = new StringReader(textFile.text);

		while(reader != null)
		{
			string line = reader.ReadLine(); //파일의 줄을 읽어 스트링 변수에 저장

			if(line == null)
			{
				break;
			}

			Spawn spawnData = new Spawn();
			spawnData.spawnDelay = float.Parse(line.Split(',')[0]);  //,를 기준으로 나눠서 첫번째 값을 저장
			spawnData.type = line.Split(',')[1];
			spawnData.point = int.Parse(line.Split(',')[2]);
			spawnList.Add(spawnData);
		}
		reader.Close(); //텍스트파일 닫기
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

		//UI 점수 로직
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

		//리스폰 인덱스 증가
		spawnIndex++;
		if(spawnIndex == spawnList.Count)
		{
			spawnEnd = true;
			return;
		}

		//다음 리스폰 딜레이 갱신
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
