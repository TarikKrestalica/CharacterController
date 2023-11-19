
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Range(0.1f, 7f)]
    [SerializeField] private float speed;
    private Rigidbody2D rb;

    private StatSystem statSystem;
    // Start is called before the first frame update
    void Start()
    {
        statSystem = GetComponent<StatSystem>();
        rb = GetComponent<Rigidbody2D>();
        if(rb == null)
        {
            Debug.LogError("You are missing an rigidbody 2D component!");
        }

        rb.velocity = -this.transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            return;
        }

        Destroy(this.gameObject);
    }
}
