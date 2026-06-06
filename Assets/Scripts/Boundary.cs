using UnityEngine;

public class Boundary : MonoBehaviour
{
    public Spawner spawner; // Referencia al Spawner

    private void OnTriggerEnter(Collider other)
    {
        spawner.GameOver(); // Llamar al método GameOver del Spawner cuando ocurra cualquier colisión
    }
}
