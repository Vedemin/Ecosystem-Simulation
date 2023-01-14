using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public List<GameObject> fishToBornList;
    private int growInterval = 1; 
    public int typeToBorn;
    private float nextTime = 0;
    public int timeLeftToBorn = 25;
    public bool start = false;

    // Start is called before the first frame update
    void Start()
    {
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
                    return;
                }
                timeLeftToBorn -= 1;
            }
 
            nextTime += growInterval; 
         }
    }

    private void BornNewFish(){
        GameObject newFish = Instantiate(fishToBornList[typeToBorn]);
        newFish.transform.position = transform.position;
        // Debug.Log("born");
    }
}
