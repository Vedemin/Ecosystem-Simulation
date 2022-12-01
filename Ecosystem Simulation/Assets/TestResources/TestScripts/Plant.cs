using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float preferedDepth = 300f;
    public float depthTolerance = 30f;
    public float amountRemaining = 10;
    public float growSpeedRange = 10;
    public float maxFoodAmount = 100;
    public float neighbourhoodRange = 10;
    public float spreadProbability = 0.5f;
    public float depth;
    public Vector3 maxSize = new Vector3(5f, 0.5f, 5f);
    public int growInterval = 1; 
    float nextTime = 0;

    void Start()
    {
        amountRemaining = Random.Range(0.0f, maxFoodAmount);
        growSpeedRange  = Random.Range(1f, growSpeedRange);
    }

    void Update()
    {
        if (Time.time >= nextTime) {
 
            Grow();
            Spread();
 
            nextTime += growInterval; 
         }
    }

    //rozsiewanie sie roslin
    public void Spread(){
        if(transform.localScale.x > maxSize.x - growSpeedRange / 100){
            if(Random.Range(0f, 1f) > spreadProbability){
                Ray ray = new Ray (transform.position + new Vector3(Random.Range(-neighbourhoodRange, neighbourhoodRange), depth, Random.Range(-neighbourhoodRange, neighbourhoodRange)), -transform.up);
                RaycastHit hitInfo;

                if(Physics.Raycast (ray, out hitInfo, 1000)){
                        GameObject plant = Instantiate(this.gameObject);
                        // plant.transform.parent = plantToPlace.transform;
                        plant.transform.position = hitInfo.point;
                        plant.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                        plant.GetComponent<Plant>().depth = hitInfo.distance;
                        if(!plant.GetComponent<Plant>().isSpawned(plant.GetComponent<Plant>().depth)){
                            Destroy(plant);
                    }
                }
            }
        }
    }

    public float Consume(float amount)
    {
        if(amountRemaining < amount)
        {
            Destroy(gameObject);
            return amountRemaining;
        }
        else
        {
            amountRemaining -= amount;
            return amount;
        }
    }

    //wzrost rozmiaru i "owocow"
    public void Grow()
    {
        amountRemaining += growSpeedRange;
	    if(maxFoodAmount < amountRemaining){
	        amountRemaining = maxFoodAmount;
        }

        if(transform.localScale.x < maxSize.x){
            float growScale = growSpeedRange / 100;
            transform.localScale += new Vector3 (growScale, 0f, growScale);
        }
    }

    public float SpawnProbability(float depth)
    {
        float probability = -(1 / (depthTolerance * depthTolerance)) * ((depth - preferedDepth) * (depth - preferedDepth)) + 1;
        if(probability > 0.9f){
            return 0.8f;
        }
        if(probability < 0.2f){
            return 0.01f;
        }
        return probability;
	}

    public bool isSpawned(float depth){
        float random = Random.Range(0.0f, 1.0f);
        if(random <= SpawnProbability(depth)){
            return true;
        }
        else{
            return false;
        }
    }
}
