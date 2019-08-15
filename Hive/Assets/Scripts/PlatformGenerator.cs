using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{

    public GameObject platformPrefab;
    public float heightBounds, sideBounds;
    public float platformMaxWidth;
    public int maxPlatformEachFloor = 10, minPlatformEachFloor = 3;

    // Start is called before the first frame update
    void Start()
    {
        GeneratePlatform();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GeneratePlatform()
    {

        for(int i = 0; i < heightBounds; i+= 3)
        {
            int randomNumberOfPlatforms = Random.Range(minPlatformEachFloor, maxPlatformEachFloor);
            float platformWidth = platformMaxWidth;
            for (int j = 0; j < randomNumberOfPlatforms; j++) 
            {
                GameObject platform = Instantiate(platformPrefab, transform);
                float randomWidth = Random.Range(4, platformWidth);
                float newBounds = sideBounds - randomWidth;
                float randomX = Random.Range(-newBounds, newBounds);
                platform.transform.position = new Vector2(randomX, i);
                platform.transform.localScale = new Vector3(randomWidth, 1, 0);
                //platformWidth -= randomWidth;
            }

        }
    }
}
