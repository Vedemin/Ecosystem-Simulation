using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    private int growInterval = 1; 
    private float nextTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTime) {

            GameObject[] carnivore = GameObject.FindGameObjectsWithTag("carnivore");
            GameObject[] herbivore = GameObject.FindGameObjectsWithTag("herbivore");
            GameObject[] egg = GameObject.FindGameObjectsWithTag("egg");
            GameObject[] food = GameObject.FindGameObjectsWithTag("food");

            Debug.Log("Carnivores pop: " + carnivore.Length + " Herbivores pop: " + herbivore.Length
            + " Eggs pop: " + egg.Length + " Food pop: " + food.Length);

            nextTime += growInterval; 
         }
    }
}
