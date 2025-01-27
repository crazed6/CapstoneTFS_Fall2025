using UnityEngine;

public class EnemyDebug : MonoBehaviour
{

    public class Enemy : MonoBehaviour
    {
        public GameObject smallerEnemyPrefab;  // Reference to the smaller enemy prefab
        public float damageAmount = 10f;       // Amount of damage per click

        void Update()
        {
            // Detect if the left mouse button was clicked
            if (Input.GetMouseButtonDown(0)) // 0 means left mouse button
            {
                // Raycast from the camera to where the mouse is pointing
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the raycast hits this enemy
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        // Call TakeDamage() on the enemy that was clicked
                        Enemy enemy = hit.collider.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage((int)damageAmount);
                        }
                    }
                }
            }
        }

    }
