using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Mask : MonoBehaviour
{   
    void Awake()
    {
        GetComponent<Rigidbody2D>().simulated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlyOff()
    {
        float randomX = UnityEngine.Random.Range(-220f, -300f);
        float randomY = UnityEngine.Random.Range(-20f, 20f);

        var rb = GetComponent<Rigidbody2D>();
        rb.simulated = true;
        rb.linearVelocity = new Vector2(randomX, randomY);

        rb.angularVelocity = UnityEngine.Random.Range(360f, 720f);
    }
}



