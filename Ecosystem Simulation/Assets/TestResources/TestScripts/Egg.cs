using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public GameObject fishToBorn;
    private int growInterval = 1; 
    private float nextTime = 0;
    private int timeLeftToBorn = 5;
    public bool start = false;
    public GameObject newFish;

    // Start is called before the first frame update
    void Start()
    {
        if(fishToBorn != null)
            newFish = Instantiate(fishToBorn);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTime) {

            if(start){
                // Debug.Log(timeLeftToBorn);
                if(timeLeftToBorn <= 0){
                    BornNewFish();
                    Destroy(gameObject);
                }
                timeLeftToBorn -= 1;
            }
 
            nextTime += growInterval; 
         }
    }

    private void BornNewFish(){
        newFish.transform.position = transform.position;
        // Debug.Log("born");
    }
}
