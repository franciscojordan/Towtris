using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spawner : MonoBehaviour
{
    public GameObject[] blockPrefabs;
    public Transform spawnPoint;
    public float moveSpeed = 5f;
    public float respawnDelay = 2f;
    public TMP_Text scoreMarker;
    public Camera mainCamera;
    public GameObject gameOverPanel; // Panel de Game Over
    public TMP_Text scoreText; // Texto para mostrar el puntaje
    public Collider baseCollider; // Collider que representa la base

    private GameObject currentBlock;
    private Rigidbody currentRigidbody;
    private bool gravityActive = false;
    private int blocksDropped = 0;
    private float maxHeight = 0f;
    private float initialSpawnHeight;
    private Vector3 initialCameraPosition;
    private Vector3 targetCameraPosition;
    private bool shouldMoveCamera = false;
    private bool gameIsOver = false;

    void Start()
    {
        initialSpawnHeight = spawnPoint.position.y;
        initialCameraPosition = mainCamera.transform.position;
        targetCameraPosition = initialCameraPosition;
        SpawnBlock();
        UpdateMarker();
    }

    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            return;
        }

        if (currentBlock != null && !gravityActive)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            Vector3 movement = new Vector3(0f, 0f, horizontalInput * -1) * moveSpeed * Time.deltaTime;
            currentBlock.transform.Translate(movement, Space.World);

            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateBlock();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && currentBlock != null)
        {
            currentRigidbody.useGravity = true;
            gravityActive = true;
            currentBlock = null;
            blocksDropped++;
            UpdateMarker();

            Invoke("SpawnBlock", respawnDelay);
        }

        if (shouldMoveCamera)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetCameraPosition, Time.deltaTime);
            if (Vector3.Distance(mainCamera.transform.position, targetCameraPosition) < 0.01f)
            {
                mainCamera.transform.position = targetCameraPosition;
                shouldMoveCamera = false;
            }
        }
    }

    void SpawnBlock()
    {
        spawnPoint.position = new Vector3(spawnPoint.position.x, initialSpawnHeight + maxHeight, spawnPoint.position.z);

        int randomIndex = Random.Range(0, blockPrefabs.Length);
        GameObject selectedPrefab = blockPrefabs[randomIndex];

        currentBlock = Instantiate(selectedPrefab, spawnPoint.position, selectedPrefab.transform.rotation);

        currentRigidbody = currentBlock.GetComponent<Rigidbody>();

        currentBlock.AddComponent<Block>();

        if (currentRigidbody != null)
        {
            currentRigidbody.useGravity = false;
        }

        gravityActive = false;
    }

    void RotateBlock()
    {
        currentBlock.transform.Rotate(90f, 0f, 0f);
    }

    public void UpdateMaxHeight(float blockHeight)
    {
        if (blockHeight > maxHeight)
        {
            maxHeight = blockHeight;
            AdjustCameraPosition();
        }
    }

    void UpdateMarker()
    {
            scoreMarker.text = blocksDropped.ToString();
    }

    void AdjustCameraPosition()
    {
        float cameraAdjustmentFactor = 0.5f;
        int heightRoundedToFive = Mathf.FloorToInt(maxHeight / 5) * 5;
        if (maxHeight >= heightRoundedToFive)
        {
            targetCameraPosition = new Vector3(
                initialCameraPosition.x - heightRoundedToFive * cameraAdjustmentFactor,
                initialCameraPosition.y + heightRoundedToFive * cameraAdjustmentFactor,
                initialCameraPosition.z
            );
            shouldMoveCamera = true;
        }
    }

    public void GameOver()
    {
        gameIsOver = true;
        gameOverPanel.SetActive(true); // Mostrar el panel de Game Over
        scoreText.text = "You lost\nScore: " + blocksDropped + "\nMax height: " + Mathf.Round(maxHeight) + "\nPress R to restart"; // Mostrar el puntaje
    }

    void RestartGame()
    {
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Cube"))
        {
            Destroy(block); // Destruir todos los bloques en la escena
        }

        maxHeight = 0f;
        blocksDropped = 0;
        gameIsOver = false;
        gameOverPanel.SetActive(false); // Ocultar el panel de Game Over
        SpawnBlock(); // Spawnear un nuevo bloque para empezar de nuevo
        UpdateMarker(); // Actualizar el marcador de altura
    }
}
