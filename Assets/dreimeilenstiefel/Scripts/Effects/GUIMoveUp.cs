﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIMoveUp : MonoBehaviour
{
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
    }
}