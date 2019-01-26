using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    //House Prefab
    public GameObject housePrefab;
    public float stepSize = 0.7680001f;
    private bool isSpawnned = false;
    private Vector3 houseLocation;
    // Start is called before the first frame update
    void Start()
    {
        isSpawnned = SpawnHouse();
    }

    // Update is called once per frame
    void Update()
    {
        if(isSpawnned)
       Debug.Log(Vector3.Distance(houseLocation, GameObject.FindGameObjectWithTag("Player").transform.position));
        if (Vector3.Distance(houseLocation, GameObject.FindGameObjectWithTag("Player").transform.position) < 0.68f)
        {
            Debug.Log("Game Over");
            GUIManager.instance.GameOver();
        }
    }

    private bool SpawnHouse()
    {
        //House location
        Vector2 location;
        //Create a new house game object
        GameObject houseHandler = Instantiate(housePrefab) as GameObject;
        //Reset Game Object Transform
        houseHandler.transform.parent = this.transform;
        //Generate Random Loaction
        location.x = stepSize * Random.Range(1, 7);
        location.y = stepSize * Random.Range(1, 7);
        Debug.Log(location.x + " - " + location.y);
        //Push to the board edge 
        if (location.y > location.x) { location.x = 16.03f; } else { location.y = 7.56f; }
        houseHandler.transform.localPosition = new Vector3(location.x, location.y, 0);
        houseLocation = houseHandler.transform.position;
        return true;
    }
}
