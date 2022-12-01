using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGenerator : MonoBehaviour
{
    public GameObject plantToPlace;
    public GameObject terrain;
    public Vector3 breaksSizeX = new Vector3(0, 0, 5);
    public Vector3 breaksSizeY = new Vector3(5, 0, 0);
    public int x = 1;
    public GameObject plantTest;

    void Start()
    {   
        plantTest = Instantiate(plantToPlace);
        for(int i = 0; i < 4000; i++){
            Ray ray = new Ray (transform.position, -transform.up);
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
                
                transform.position = transform.position + x * breaksSizeX;     
            }
            else{
                x *= -1;
                transform.position = transform.position + breaksSizeY;
                transform.position = transform.position + x * breaksSizeX;
            }
        }
    }

    void Update()
    {
        Ray ray = new Ray (transform.position, -transform.up);
        RaycastHit hitInfo;

            if(Physics.Raycast (ray, out hitInfo, 1000)){
                Debug.Log(plantTest.GetComponent<Plant>().SpawnProbability(hitInfo.distance));
                Debug.DrawLine (ray.origin, hitInfo.point, Color.red);
                // Debug.Log(hitInfo.distance);
            }
    }
}
