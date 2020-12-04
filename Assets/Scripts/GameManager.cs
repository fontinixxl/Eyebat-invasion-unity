﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject targetPrefab;
    public GameObject despawnSensor;

    private float spawnRangeYMin;
    private float spawnRangeYMax;
    // Offset distance off-screen on the X coordinate where the enemy will be spawning
    private readonly float offScreenXOffset = 1;
    private readonly float spawnYOffset = 2;
    private readonly float HUDOffset = 1.5f;

    [SerializeField]
    private float minSpawnRate = 1;
    [SerializeField]
    private float maxSpawnRate = 3;
    private readonly int[] spawnDirections = new int[2] { -1, 1 };

    // Start is called before the first frame update
    void Start()
    {
        // Subscrive to the TimesUpEvent so we can Stop the Game from the Manager.
        HUDController.TimesUpEvent += GameOver;

        // TODO: Fix target broken pivot that force me to make weird math to figure out corret spawnRange on Y
        spawnRangeYMin = (ScreenBounds.Height / 2);
        spawnRangeYMax = ScreenBounds.Height - spawnYOffset - HUDOffset;

        SpawnLeftRightSensor();

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
        while (true)
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
    public void GameOver()
    {
        StopCoroutine("SpawnTarget");
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

    // Restart game by reloading the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
