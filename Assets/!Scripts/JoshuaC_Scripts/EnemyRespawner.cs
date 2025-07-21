using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;

public class EnemyRespawner : MonoBehaviour
{
    [System.Serializable]
    private class EnemySpawnData
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
    }

    private List <EnemySpawnData> spawnPoints = new List<EnemySpawnData>();
    private List<GameObject> currentEnemies = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //Find enemies in the scene with the tag "enemy" (Lower case is important)
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject enemy in enemies)
        {
            var identity = enemy.GetComponent<EnemyIdentity>(); //Get the EnemyIdentity component from the enemy
            if (identity != null)
            {

                //Save spawning information
                spawnPoints.Add(new EnemySpawnData {


                    prefab = identity.enemyPrefab, //assumes the object in scene is a prefab
                    position = enemy.transform.position, //get the position of the enemy in the scene
                    rotation = enemy.transform.rotation //get the rotation of the enemy in the scene
                }
            );

                currentEnemies.Add(enemy); //add the enemy to the current enemies list
            }

        }
    }

    public void RespawnAllEnemies()
    {

        //Destroy all current enemies

        foreach (GameObject enemy in currentEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

            currentEnemies.Clear(); //clear the current enemies list

            //Re-instantiate enemies at original spawn points

            foreach (EnemySpawnData data in spawnPoints)
            {
                GameObject newEnemy = Instantiate(data.prefab, data.position, data.rotation); //instantiate the enemy prefab at the original position and rotation
                newEnemy.tag = "enemy"; //set the tag of the new enemy to "enemy"
                currentEnemies.Add(newEnemy); //add the new enemy to the current enemies list
            }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
