using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance { private set; get; }
    public event Action OnEndGame;

    public GameSettings gameSettings;
    public float gameTimer { private set; get; }
    public bool isPlaying { private set; get; }

    [Header("UI")]
    public GameObject startScreen;
    public GameObject gameScreen;
    public GameObject gameOverScreen;

    [Header("Game")]
    [SerializeField] private Bounds levelBounds;
    [SerializeField] private Transform level;
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private Bomb bombPrefab;

    [SerializeField] private Joystick joystick;

    public Vector3 playerSpawnPos = new Vector3(0, 0, 0);
    private PlayerController playerController;
    private ObjectPool<Bomb> bombsPool;
    private ObjectPool<EnemyController> enemiesPool;

    private List<Bomb> bombs = new List<Bomb>();
    private List<EnemyController> enemies = new List<EnemyController>();

    private Coroutine delayAddEnemy;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(levelBounds.center, levelBounds.size);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;


        bombsPool = new ObjectPool<Bomb>(10, () =>
        {
            Bomb bomb = Instantiate(bombPrefab, level);
            return bomb;
        },
        (Bomb bomb) => bomb.gameObject.SetActive(false));

        enemiesPool = new ObjectPool<EnemyController>(10, () =>
        {
            EnemyController enemy = Instantiate(enemyPrefab, level);
            return enemy;
        },
        (EnemyController enemy) => enemy.gameObject.SetActive(false));

        startScreen.SetActive(true);
    }

    private void Update()
    {
        if (isPlaying)
        {
            gameTimer += Time.deltaTime;
        }
    }

    public void StartGame()
    {
        joystick.gameObject.SetActive(true);

        startScreen.SetActive(false);
        gameScreen.SetActive(true);
        StartCoroutine(InitLevel());
    }

    private IEnumerator InitLevel()
    {
        AddPlayer();
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 10; i++)
        {
            AddBomb();
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
        delayAddEnemy = StartCoroutine(DelayAddEnemy());

        gameTimer = 0f;
        isPlaying = true;
    }

    public void RestartGame()
    {
        StartCoroutine(RestartLevel());
    }

    private IEnumerator RestartLevel()
    {
        bombs.ForEach(b => bombsPool.Put(b));
        bombs.Clear();
        yield return new WaitForEndOfFrame();

        enemies.ForEach(e => enemiesPool.Put(e));
        enemies.Clear();

        yield return new WaitForEndOfFrame();
        joystick.gameObject.SetActive(true);
        StartCoroutine(InitLevel());

        gameScreen.SetActive(true);
        gameOverScreen.SetActive(false);
    }

    #region Player
    private void AddPlayer()
    {
        playerController = Instantiate(playerPrefab, level, false);
        playerController.transform.position = playerSpawnPos;
        playerController.Init(joystick, gameSettings.playerSpeed);
        playerController.OnDeath += OnPlayerDeath;
    }

    private async void OnPlayerDeath()
    {
        isPlaying = false;
        joystick.gameObject.SetActive(false);

        playerController.OnDeath -= OnPlayerDeath;
        Destroy(playerController.gameObject);
        playerController = null;

        enemies.ForEach(e => e.StopEnemy());
        StopCoroutine(delayAddEnemy);
        delayAddEnemy = null;
        OnEndGame?.Invoke();

        await Task.Delay(2000);
        gameScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }
    #endregion

    #region Enemies
    private void AddEnemy()
    {
        EnemyController enemy = enemiesPool.Get();
        enemy.transform.localPosition = RandomPosition();
        enemy.gameObject.SetActive(true);
        enemy.Init(playerController.transform, gameSettings.enemySpeed);
        enemies.Add(enemy);
    }

    private IEnumerator DelayAddEnemy()
    {
        yield return new WaitForSeconds(Random.Range(gameSettings.spawnEnemyMinTime, gameSettings.spawnEnemyMaxTime));
        AddEnemy();

        StopCoroutine(delayAddEnemy);
        delayAddEnemy = StartCoroutine(DelayAddEnemy());
    }

    #endregion

    #region Bombs
    private void AddBomb()
    {
        Bomb bomb = bombsPool.Get();
        bomb.transform.localPosition = RandomPosition();
        bomb.gameObject.SetActive(true);
        bomb.OnCollisionEvent += OnBombDestroy;

        bombs.Add(bomb);
    }

    private void OnBombDestroy(Bomb bomb)
    {
        bomb.OnCollisionEvent -= OnBombDestroy;
        bombsPool.Put(bomb);

        //StartCoroutine(DelayAddBomd(Random.Range(1, 3)));
    }

    private IEnumerator DelayAddBomd(float delay)
    {
        yield return new WaitForSeconds(delay);
        AddBomb();
    }
    #endregion

    private Vector3 RandomPosition()
    {
        int tryCount = 10;
        while (true)
        {
            Vector3 randomPoint = RandomInLevelBounds();
            if (Vector3.Distance(randomPoint, playerController.transform.position) >= 2.5f)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 100f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            tryCount--;
            if (tryCount <= 0) { return Vector3.zero; }
        }
    }

    private Vector3 RandomInLevelBounds()
    {
        return new Vector3(
            Random.Range(levelBounds.min.x, levelBounds.max.x),
            -10,
            Random.Range(levelBounds.min.z, levelBounds.max.z)
        ) + levelBounds.center;
    }
}