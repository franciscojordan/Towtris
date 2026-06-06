using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private bool hasLanded = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded)
        {
            hasLanded = true;
            Spawner spawner = FindObjectOfType<Spawner>();
            if (spawner != null)
            {
                spawner.UpdateMaxHeight(transform.position.y);
            }
        }
    }
}
