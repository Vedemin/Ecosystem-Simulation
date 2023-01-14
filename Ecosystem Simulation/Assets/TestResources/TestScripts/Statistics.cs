using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    private int growInterval = 1; 
    private float nextTime = 0;
    // public float fpsText;
    // public float deltaTime;
    private float iterations = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        // float fps = 1.0f / deltaTime;
        // fpsText = Mathf.Ceil (fps);
        // // Debug.Log(fpsText);

        if (Time.time >= nextTime) {

            GameObject[] carnivore = GameObject.FindGameObjectsWithTag("carnivore");
            GameObject[] herbivore = GameObject.FindGameObjectsWithTag("herbivore");
            GameObject[] egg = GameObject.FindGameObjectsWithTag("egg");
            GameObject[] food = GameObject.FindGameObjectsWithTag("food");

            Debug.Log(iterations + " Carnivores pop: " + carnivore.Length + " Herbivores pop: " + herbivore.Length
            + " Eggs pop: " + egg.Length + " Food pop: " + food.Length);

            iterations++;

            nextTime += growInterval; 
         }
    }
}
