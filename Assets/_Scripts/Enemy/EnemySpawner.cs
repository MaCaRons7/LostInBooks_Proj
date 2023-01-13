using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Vector3 corner1;
    public Vector3 corner2;
    public bool isStop;
    public GameObject[] enemys;
    public static EnemySpawner instance;
    private int nowWave;
    private int nowController;
    private float nowWaveTime;
    private float waitTime;
    private EnemyHeap heap = new EnemyHeap();
    public enum EnemyType { Enemy_Normal,Enemy_Sheild, Enemy_TrojanHorse };
    [Serializable]
    public struct Controller 
    {
        public int num;
        public float beginTime;
        public float deltaTime;
        public EnemyType enemyType;
    }
    [Serializable]
    public struct Wave
    {
        public float waitingTime;
        public Controller[] controllers;
    }
    public Wave[] waves;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if(isStop) return;
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }
        else waitTime = 0;
        if (waves.Length <= nowWave || waves[nowWave].controllers.Length <= 0) return;
        nowWaveTime += Time.deltaTime;
        while (nowController < waves[nowWave].controllers.Length && waves[nowWave].controllers[nowController].beginTime <= nowWaveTime)
        {
            heap.Insert(waves[nowWave].controllers[nowController]);
            nowController++;
        }
        if (heap.GetNum() > 0)
        {
            while (heap.GetTop().beginTime <= nowWaveTime)
            {
                Controller temp = heap.GetTop();
                RandomInstantiateEnemy(temp.enemyType);
                heap.ChangeTop(temp.beginTime + temp.deltaTime);
            }
        }
        if (nowController >= waves[nowWave].controllers.Length && heap.GetTop().beginTime >= 10000)
        {
            NewWaveBegin();
        }
    }

    private void RandomInstantiateEnemy(EnemyType enemy)
    {
        GameObject x = Instantiate(enemys[(int)enemy]);
        x.transform.position = new Vector3(UnityEngine.Random.Range(corner1.x, corner2.x),
            UnityEngine.Random.Range(corner1.y, corner2.y), UnityEngine.Random.Range(corner1.z, corner2.z));
    }

    public void NewWaveBegin()
    {
        nowWave++;
        if (nowWave < waves.Length)
        {
            waitTime = waves[nowWave].waitingTime;
        }
        nowController = 0;
        nowWaveTime = 0;
        heap.Clear();
    }

    public void Wait(float x)
    {
        waitTime += x;
    }
}
