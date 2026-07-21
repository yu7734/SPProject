using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Splines.SplineInstantiate;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public struct SpawnRange
    {
        public float min;
        public float max;
    }
    [Serializable]
    public struct SpawnEnemy
    {
         public GameObject enemyPrefabs;
         public float spawn;
    }
    [Serializable]
    public struct EnemyInterval
    {
        [Tooltip("初期の湧く間隔")]
        public float initSpawnTimer;
        [Tooltip("敵の湧き時間が減るまでの時間")]
        public float timeUntilDecrease;
        [Tooltip("敵の湧き時間の減少幅")]
        public float enemyIntervalDecrease;
        [Tooltip("敵の湧き時間の最低値")]
        public float minInterval;
    }
    [Header("ObjectPool")]
    [SerializeField, Tooltip("敵の初期生成数")]
    private int initPoolSize = 10;
    [SerializeField, Tooltip("敵の生成上限(これ以上はdeleteされる)")]
    private int maxPoolSize = 100;


    private Dictionary<GameObject, IObjectPool<GameObject>> pools = new();

    [Header("敵の湧きパターン")]
    [TextArea(1, 3)]
    public string EnemySpawnerMemo = "ここに使い方の注意点などを書けます";

    [Header("敵の出現座標")]

    [SerializeField, Tooltip("X座標のminからmaxまででランダム")]
    private SpawnRange rangeX;
    [SerializeField, Tooltip("Y座標のminからmaxまででランダム")]
    private SpawnRange rangeY;
    /// <summary> 敵が湧くZ座標 </summary>
    [SerializeField] private float spawnZ;

    /// <summary> そのenemySpawnerでゲーム中に湧く敵のリスト </summary>
    [SerializeField, Tooltip("この中からランダムで出現")]
    private List<SpawnEnemy> enemyList = new();

    /// <summary> その時点で湧く敵のリスト </summary>
    private List<SpawnEnemy> availableEnemy = new();

    /// <summary> その敵が一体だけ湧くならtrue </summary>
    [SerializeField, Tooltip("一体だけ湧くか")]
    private bool onlySpawn = false;
    [SerializeField]
    private EnemyInterval interval;

    /// <summary> 通常のタイマー </summary>
    private float timer;
    void Awake()
    {
        SetupEnemyPool();
    }
    /// <summary>
    /// オブジェクトプールの初期設定
    /// </summary>
    /// <remarks>
    /// 関数の順番として、生成->再利用->返却->削除
    /// </remarks>
    private void SetupEnemyPool()
    {
        foreach (var prefab in enemyList)
        {
            IObjectPool<GameObject> pool = null;
            pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab.enemyPrefabs),

            actionOnGet: (obj) => {
                obj.SetActive(true);

                if (obj.TryGetComponent<EnemyAttackBase>(out var enemyAttackScript))
                {
                    enemyAttackScript.OnReset();
                }
                if (obj.TryGetComponent<EnemyManager>(out var enemyManagerScript))
                {
                    enemyManagerScript.MyPool = pool;
                    enemyManagerScript.OnReset();
                }
            },
            actionOnRelease: (obj)  =>{
                obj.SetActive(false);
                if (onlySpawn) timer = 0;
                //Debug.Log($"{gameObject.name} {timer}");
            },
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: initPoolSize,  //10
            maxSize: maxPoolSize    //100
            );
            pools.Add(prefab.enemyPrefabs, pool);
        }

    }
    private void Update()
    {

        timer += Time.deltaTime;
        if (timer >= Math.Max((interval.initSpawnTimer - (int)(Time.timeSinceLevelLoad / interval.timeUntilDecrease) * interval.enemyIntervalDecrease),interval.minInterval))
        {
            int activeEnemyCount = 0;                                           // 全てのプールの「貸し出し中」の合計をチェック
            foreach (var pool in pools.Values)
            {
                if (pool is ObjectPool<GameObject> concretePool)
                {
                    activeEnemyCount += concretePool.CountActive;
                }
            }
                                                                            
            if (activeEnemyCount < maxPoolSize)                                 // 画面内の敵が maxPoolSize 未満の時だけその時点で湧ける敵を湧かせる
            {
                availableEnemy.Clear();
                foreach (var data in enemyList)
                {
                    if(data.spawn <= Time.timeSinceLevelLoad)
                    {
                        availableEnemy.Add(data);
                    }
                }
                EnemySpawn();
                timer = 0;

            }
        }
    }
    /// <summary>
    /// プレハブの中からランダムに指定して、
    /// その敵のプールを取得してランダムな座標に出現させる
    /// </summary>
    void EnemySpawn()
    {
        if (availableEnemy.Count == 0) return;
        int rndIndex = UnityEngine.Random.Range(0, availableEnemy.Count);
        
        SpawnEnemy selectedEnemyPrefab = availableEnemy[rndIndex];

        var pool = pools[selectedEnemyPrefab.enemyPrefabs];
        GameObject enemy = pool.Get();

        float x = UnityEngine.Random.Range(rangeX.min, rangeX.max);
        float y = UnityEngine.Random.Range(rangeY.min, rangeY.max);
        enemy.transform.position = new Vector3(x, y, spawnZ);
        enemy.transform.rotation = selectedEnemyPrefab.enemyPrefabs.transform.rotation;
    }
}
