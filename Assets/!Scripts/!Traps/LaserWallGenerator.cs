using UnityEngine;

public class LaserWallGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 5;      // Number of rows (height of the wall)
    public int columns = 5;   // Number of columns (width of the wall)
    public Vector2 spacing = new Vector2(1f, 1f); // Distance between lasers (X: horizontal, Y: vertical)

    [Header("Gap Settings")]
    public int gapColumn = 2;       // The column where the gap (opening) appears
    public bool moveGap = true;     // If true, the gap moves up and down
    public float moveSpeed = 1f;    // How fast the gap moves (lower is slower)

    [Header("Laser Beam Prefab")]
    public GameObject laserBeamPrefab;  // Prefab to use for each laser unit

    private int currentGapRow = 0;      // The current vertical position of the moving gap
    private float gapTimer = 0f;        // Internal timer to drive movement with PingPong
    private GameObject[,] laserBeams;   // 2D array to store the spawned lasers for easy regeneration

    private void Start()
    {
        GenerateLaserWall();  
    }

    private void Update()
    {
        // Only move the gap if enabled and the wall has more than 1 row
        if (!moveGap || rows <= 1) return;

        // Update the timer based on move speed
        gapTimer += Time.deltaTime * moveSpeed;

        // Calculate new row for the gap using PingPong to oscillate back and forth
        int newGapRow = Mathf.FloorToInt(Mathf.PingPong(gapTimer, rows));

        // Only regenerate the wall if the gap row has changed
        if (newGapRow != currentGapRow)
        {
            currentGapRow = newGapRow;
            RegenerateLaserWall(); // Rebuild the wall with the new gap row
        }
    }

    void GenerateLaserWall()
    {
        // Safety check to ensure prefab is assigned
        if (!laserBeamPrefab)
        {
            Debug.LogError("Laser Beam Prefab not assigned!");
            return;
        }

        // Initialize the 2D array to store references to each laser beam
        laserBeams = new GameObject[rows, columns];

        // Loop through each cell in the grid
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Skip spawning a laser where the gap should be
                if (row == currentGapRow && col == gapColumn)
                    continue;

                // Calculate local position of the laser within the wall
                Vector3 position = new Vector3(
                    col * spacing.x,   // Horizontal offset (X)
                    row * spacing.y,   // Vertical offset (Y)
                    0f                 // Depth (Z stays at 0)
                );

                // Instantiate the laser as a child of this wall object
                GameObject laser = Instantiate(laserBeamPrefab, transform);
                laser.transform.localPosition = position;

                // Store the reference to allow destruction later
                laserBeams[row, col] = laser;
            }
        }
    }

    void RegenerateLaserWall()
    {
        // Loop through the 2D array and destroy all existing lasers
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (laserBeams[row, col] != null)
                {
                    Destroy(laserBeams[row, col]);
                }
            }
        }

        // Rebuild the laser grid with the updated gap position
        GenerateLaserWall();
    }
}
