using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGenerator : MonoBehaviour
{
    public List<GameObject> plantList;
    public float chunkSize = 10;
    private int x = 1;
    private GameObject plantTest;

    void Start()
    {   
        // plantTest = Instantiate(plantToPlace);
        for(int j = 0 ; j < plantList.Count; j++){
            transform.position = new Vector3(1, 270, 1);
            GameObject plantToPlace = plantList[j];
            for(int i = 0; i < (40000 * 5) / chunkSize; i++){
                Vector3 rayPosition = transform.position + new Vector3(NextFloat(0, chunkSize/2), 0, NextFloat(0, chunkSize/2));
                Ray ray = new Ray (rayPosition, -transform.up);
                RaycastHit hitInfo;

                if(Physics.Raycast (ray, out hitInfo, 1000)){
                    // Debug.DrawLine (ray.origin, hitInfo.point, Color.red);
                    GameObject plant = Instantiate(plantToPlace);
                    // plant.transform.parent = plantToPlace.transform;
                    if(plant.GetComponent<Plant>().isSpawned(hitInfo.distance)){
                        plant.transform.position = hitInfo.point;
                        plant.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
                    }
                    else{
                        Destroy(plant);
                    }
                    
                    transform.position = transform.position + x * new Vector3(chunkSize, 0, 0);     
                }
                else{
                    x *= -1;
                    transform.position = transform.position + new Vector3(0, 0, chunkSize);
                    transform.position = transform.position + x * new Vector3(chunkSize, 0, 0);
                }
            }
        }
    }

    void Update()
    {
        // Ray ray = new Ray (transform.position, -transform.up);
        // RaycastHit hitInfo;

        //     if(Physics.Raycast (ray, out hitInfo, 1000)){
        //         Debug.Log(plantTest.GetComponent<Plant>().SpawnProbability(hitInfo.distance));
        //         Debug.DrawLine (ray.origin, hitInfo.point, Color.red);
        //         // Debug.Log(hitInfo.distance);
        //     }
    }

    static float NextFloat(float min, float max){
        System.Random random = new System.Random();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }
}
