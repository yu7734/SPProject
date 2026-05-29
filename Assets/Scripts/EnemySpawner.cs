using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemySpawner : MonoBehaviour
{

    [Serializable]
    public struct SpawnRange
    {
        public float min;
        public float max;
    }
    [Header("ObjectPool")]
    [SerializeField] private int initPoolSize = 10;
    [SerializeField] private int maxPoolSize = 100;


    private Dictionary<GameObject, IObjectPool<GameObject>> pools = new();

    [Header("敵の出現座標")]

    [SerializeField, Tooltip("X座標のminからmaxまででランダム")]
    private SpawnRange rangeX;
    [SerializeField, Tooltip("Y座標のminからmaxまででランダム")]
    private SpawnRange rangeY;
    [SerializeField] private float spawnZ;

    [SerializeField, Tooltip("この中からランダムで出現")]
    private List<GameObject> enemyPrefabs = new();
    [SerializeField, Tooltip("湧く間隔")]
    private float spawnTimer = 5f;

    private float timer;

    void Awake()
    {
        SetupEnemyPool();
    }
    private void SetupEnemyPool()
    {
        foreach (var prefab in enemyPrefabs)
        {
            IObjectPool<GameObject> pool = null;
            pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab),

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
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: initPoolSize,  //10
            maxSize: maxPoolSize    //100
        );
            pools.Add(prefab, pool);
        }

    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnTimer)
        {
            EnemySpawn();
            timer = 0;
        }
    }
    void EnemySpawn()
    {
        int rndIndex = UnityEngine.Random.Range(0, enemyPrefabs.Count);
        GameObject selectedEnemyPrefab = enemyPrefabs[rndIndex];

        var pool = pools[selectedEnemyPrefab];
        GameObject enemy = pool.Get();
        
        float x = UnityEngine.Random.Range(rangeX.min, rangeX.max);
        float y = UnityEngine.Random.Range(rangeY.min, rangeY.max);
        enemy.transform.position = new Vector3(x, y, spawnZ);

    }
}
