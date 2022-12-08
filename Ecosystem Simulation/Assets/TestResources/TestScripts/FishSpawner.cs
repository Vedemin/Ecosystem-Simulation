using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> prefabList;
    void Start()
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            FishData fishData = prefabList[i].GetComponent<FishData>();
            for (int x = 0; x < fishData.startingPopulation; x++)
            {
                bool chosen = false;
                Vector3 spawnPoint = new Vector3(Random.Range(5, 995), 270, Random.Range(5, 995));
                while (chosen == false)
                {
                    chosen = true;
                    RaycastHit hit;
                    if (Physics.Raycast(spawnPoint, Vector3.down, out hit, 270))
                    {
                        spawnPoint.y = hit.distance - 5;
                        if (hit.distance < fishData.minDepth)
                        {
                            chosen = false;
                        } else {
                            if (hit.distance > fishData.maxDepth)
                            {
                                spawnPoint.y = fishData.maxDepth - 5;
                                if (spawnPoint.y > 270 - fishData.minDepth)
                                {
                                    chosen = false;
                                }
                            }
                        }
                    } else
                    {
                        chosen = false;
                    }
                    if (chosen == false)
                        spawnPoint = new Vector3(Random.Range(5, 995), 0, Random.Range(5, 995));
                }
                var fish = Instantiate(prefabList[i]);
                fish.transform.SetParent(this.transform);
                fish.transform.position = spawnPoint;
            }
        }
    }
}
