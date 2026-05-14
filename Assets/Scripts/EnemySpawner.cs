using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public struct SpawnRange
    {
        public float min;
        public float max;
    }

    [Header("敵の出現座標")]
    
    [SerializeField,Tooltip("X座標のminからmaxまででランダム")]
    private SpawnRange rangeX;
    [SerializeField, Tooltip("Y座標のminからmaxまででランダム")]
    private SpawnRange rangeY;
    [SerializeField] private float spawnZ;

    [SerializeField,Tooltip("この中からランダムで出現")] 
    private List<GameObject> enemyPrefabs = new();
    [SerializeField, Tooltip("湧く間隔")]
    private float spawnTimer = 5f;

    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnTimer)
        {
            EnemySpown();
            timer = 0;
        }
    }
    void EnemySpown()
    {
        int rndIndex = UnityEngine.Random.Range(0, enemyPrefabs.Count);
        GameObject selectedEnemyPrefab = enemyPrefabs[rndIndex];

        float x = UnityEngine.Random.Range(rangeX.min, rangeX.max);
        float y = UnityEngine.Random.Range(rangeY.min, rangeY.max);
        Vector3 rndPos = new Vector3(x, y, spawnZ);        
        Instantiate(selectedEnemyPrefab,rndPos, Quaternion.identity);
    }
}
