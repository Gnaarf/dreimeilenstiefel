using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    //House Prefab
    public GameObject housePrefab;

    // Start is called before the first frame update
    void Start()
    {
        SpawnHouse();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SpawnHouse()
    {
        float hstep = 0;
        float vstep = 0;

        GameObject houseHandler = Instantiate(housePrefab) as GameObject;
        houseHandler.transform.parent = this.transform;
        hstep = 0.2f + Random.Range(1, 7);
        vstep = 0.2f + Random.Range(1, 7);
        if (Random.Range(1, 2) == 1) { hstep = 16.03f; } else { vstep = 8.30f; }
        houseHandler.transform.localPosition = new Vector3(hstep, vstep, 0);
    }

}
