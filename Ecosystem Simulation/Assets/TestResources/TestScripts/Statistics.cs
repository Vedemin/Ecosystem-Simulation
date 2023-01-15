using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

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
        File.WriteAllText(@"log_file.txt", string.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        // deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        // float fps = 1.0f / deltaTime;
        // fpsText = Mathf.Ceil (fps);
        // // Debug.Log(fpsText);

        if (Time.time >= nextTime) {

            GameObject[] carnivores = GameObject.FindGameObjectsWithTag("carnivore");
            GameObject[] herbivores = GameObject.FindGameObjectsWithTag("herbivore");
            GameObject[] eggs = GameObject.FindGameObjectsWithTag("egg");
            GameObject[] food = GameObject.FindGameObjectsWithTag("food");

            foreach(GameObject herbivore in herbivores){
                herbivore.GetComponent<FishAI>().age++;
                if(herbivore.GetComponent<FishAI>().hunger > 0)
                    herbivore.GetComponent<FishAI>().hunger--;
                else
                    herbivore.GetComponent<FishAI>().health--;
                
                if(herbivore.GetComponent<FishAI>().urge >= 0 && herbivore.GetComponent<FishAI>().urge < herbivore.GetComponent<FishAI>().data.urge)
                    herbivore.GetComponent<FishAI>().urge++;
            }

            foreach(GameObject carnivore in carnivores){
                carnivore.GetComponent<FishAI>().age++;
                if(carnivore.GetComponent<FishAI>().hunger > 0)
                    carnivore.GetComponent<FishAI>().hunger--;
                else
                    carnivore.GetComponent<FishAI>().health--;
                
                if(carnivore.GetComponent<FishAI>().urge >= 0 && carnivore.GetComponent<FishAI>().urge < carnivore.GetComponent<FishAI>().data.urge)
                    carnivore.GetComponent<FishAI>().urge++;
            }

            foreach(GameObject plant in food){
                plant.GetComponent<Plant>().Grow();
                plant.GetComponent<Plant>().Spread();
            }

            foreach(GameObject egg in eggs){
                if(egg.GetComponent<Egg>().start){
                    if(egg.GetComponent<Egg>().timeLeftToBorn <= 0){
                        egg.GetComponent<Egg>().BornNewFish();
                    }
                    else{
                        egg.GetComponent<Egg>().timeLeftToBorn--;
                    }
                }
            }

            string log = iterations + " Carnivores pop: " + carnivores.Length + " Herbivores pop: " + herbivores.Length
            + " Eggs pop: " + eggs.Length + " Food pop: " + food.Length;

            Debug.Log(log);

            File.AppendAllText(@"log_file.txt", log + Environment.NewLine);

            iterations++;

            nextTime += growInterval; 
         }
    }
}
