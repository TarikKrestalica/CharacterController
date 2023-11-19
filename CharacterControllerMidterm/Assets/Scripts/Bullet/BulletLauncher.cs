using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLauncher : MonoBehaviour
{
    [SerializeField] private GameObject bullet;

    [Range(1f, 7f)]
    [SerializeField] private float bulletDelay;
    private float curBulletDelay;

    void Start()
    {
        if(bullet == null)
        {
            bullet = GameObject.FindGameObjectWithTag("Bullet");
        }
        curBulletDelay = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(curBulletDelay < bulletDelay)
        {
            curBulletDelay += Time.deltaTime;
            return;
        }
        
        Instantiate(bullet, this.transform.position - this.transform.right, Quaternion.identity);
        curBulletDelay = 0;
    }
}
