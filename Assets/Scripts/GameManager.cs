﻿using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public GameObject targetPrefab;
    public GameObject despawnSensor;

    private AudioSource audioSource;
    public  AudioSource AudioSource
    {
        get { return audioSource; }
    }

    private float spawnRangeYMin;
    private float spawnRangeYMax;
    // Offset distance off-screen on the X coordinate where the enemy will be spawning
    private readonly float offScreenXOffset = 1;
    private readonly float spawnYOffset = 2.5f;
    // TODO: Remove - it was used when the HUD was located on the top to avoid overlaping with the enemies
    //private readonly float HUDOffset = 1.5f;

    [SerializeField]
    private float minSpawnRate = 1;
    [SerializeField]
    private float maxSpawnRate = 3;
    private readonly int[] spawnDirections = new int[2] { -1, 1 };
    private bool isGameActive;
    public bool IsGameActive
    {
        get { return isGameActive; }
    }

    private void Awake()
    {
        // Create a Singleton instance of the GameManager
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        isGameActive = false;

        UIController.TimesUpEvent += GameOver;
        UIController.StartGameEvent += StartGame;
        UIController.RestartGameEvent += StartGame;

        // TODO: Fix target broken pivot that force me to make weird math to figure out corret spawnRange on Y
        spawnRangeYMin = (ScreenBounds.Height / 2);
        spawnRangeYMax = ScreenBounds.Height - spawnYOffset;
        //Debug.Log("max Y spawn = " + spawnRangeYMax);

        SpawnLeftRightSensor();
    }

    private void StartGame()
    {
        audioSource.Play();
        isGameActive = true;
        StartCoroutine("SpawnTarget");
    }

    private void SpawnLeftRightSensor()
    {
        float spawnRangeYPos = spawnRangeYMin + ((spawnRangeYMax - spawnRangeYMin) / 2);
        Vector3 spawnSensorPosition = new Vector3(ScreenBounds.Width + offScreenXOffset * 2, spawnRangeYPos,
            despawnSensor.transform.position.z);
        // Spawn right sensor
        Instantiate(despawnSensor, spawnSensorPosition, Quaternion.identity);
        // Spawn left sensor
        spawnSensorPosition.x *= -1;
        Instantiate(despawnSensor, spawnSensorPosition, Quaternion.identity);
    }

    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            int direction = spawnDirections[Random.Range(0, spawnDirections.Length)];
            float spawnXComponent = (ScreenBounds.Width + offScreenXOffset) * direction;
            float spawnYComponent = Random.Range(spawnRangeYMin, spawnRangeYMax);

            Vector3 spawnPosition = new Vector3(spawnXComponent, spawnYComponent, 0);

            Instantiate(targetPrefab, spawnPosition, targetPrefab.transform.rotation);
            float spawnRate = Random.Range(minSpawnRate, maxSpawnRate);

            yield return new WaitForSeconds(spawnRate);
        }
    }

    // Stop game logic
    // Called by the event TimesUp in the HUDController script
    public void GameOver()
    {
        audioSource.Stop();
        isGameActive = false;
        RemoveRemainingTargets();
    }

    private void RemoveRemainingTargets()
    {
        GameObject[] enemiesOnScene;
        enemiesOnScene = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemiesOnScene)
        {
            Destroy(enemy.gameObject);
        }
    }
}
