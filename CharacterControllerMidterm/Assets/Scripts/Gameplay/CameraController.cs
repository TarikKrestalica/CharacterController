using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float xBound;

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.FallenOff())
        {
            this.transform.position = new Vector3(GameManager.player.transform.localPosition.x, this.transform.position.y, this.transform.position.z);
            PlayerController.SetFallenOffState(false);
        }
        
        if (GameManager.player.transform.localPosition.x > xBound - 1.5f)  // When player exceeds the camera's center, move camera with respect to player
        {
            this.transform.position = new Vector3(GameManager.player.transform.localPosition.x, this.transform.position.y, this.transform.position.z);
        }

        // Move camera up with respect to player
        this.transform.position = new Vector3(GameManager.player.transform.position.x, GameManager.player.transform.position.y, this.transform.position.z);
    }
}
