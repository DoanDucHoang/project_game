using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform player; 

    private void Update()
    {
        transform.position = new Vector3(0f, player.position.y, transform.position.z);
    }
}
