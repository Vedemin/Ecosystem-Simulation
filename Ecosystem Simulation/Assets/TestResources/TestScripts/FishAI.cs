using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour
{
    public Transform lookCheckerHorizontal;
    public Transform lookCheckerVertical;
    public FishData data;
    public float health;
    public float hunger;
    public float stamina;
    public List<GameObject> objectsInSight;
    public float timeToCheckSight;
    public int agentLayerMask;
    public int terrainLayerMask;
    private List<Vector3> checkedPositions;
    public float maxPositions;
    private float timeSinceLastCheck = 0f;
    public float[] distances;
    /*
     * 0 - 90* lewo
     * 1 - 45* lewo
     * 2 - prosto
     * 3 - 45* prawo
     * 4 - 90* prawo
     * 5 - 45* w d�
     * 6 - 90* w d�
     */
    public int state;
    /*
     * -1 - ucieczka
     * 0 - szukanie po�ywienia
     * 1 - szukanie partnera
     * 2 - po�ywienie znalezione: ryba p�ynie do niego
     * 3 - po�ywienie znalezione: ryba je (Unused)
     * 4 - partner znaleziony: ryba p�ynie do niego
     * 5 - partner znaleziony: ryba kopuluje
     * 6 - ryba ściga inna rybę
     */
    public GameObject escaping;
    public GameObject pursuing;
    public GameObject plantToEat;

    void Start()
    {
        data = GetComponent<FishData>();
        health = data.health;
        hunger = data.stomachSize;
        timeToCheckSight = data.reactionTime;
        stamina = data.stamina;
        state = 0;
        distances = new float[7];
        checkedPositions = new List<Vector3>();
    }

    void Update()
    {
        if (health <= 0){
            //Die
        }
        GetWhatFishSees();
        CheckForDanger();
        DecideWhatFishDoes();
    }

    private void GetDistances()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.right, out hit, data.eyeSightDistance, terrainLayerMask))
        {
            distances[0] = hit.distance;
        }
        if (Physics.Raycast(transform.position, transform.forward - transform.right, out hit, data.eyeSightDistance, terrainLayerMask))
        {
            distances[1] = hit.distance;
        }
        if (Physics.Raycast(transform.position, transform.forward, out hit, data.eyeSightDistance, terrainLayerMask))
        {
            distances[2] = hit.distance;
        }
        if (Physics.Raycast(transform.position, transform.forward + transform.right, out hit, data.eyeSightDistance, terrainLayerMask))
        {
            distances[3] = hit.distance;
        }
        if (Physics.Raycast(transform.position, transform.right, out hit, data.eyeSightDistance, terrainLayerMask))
        {
            distances[4] = hit.distance;
        }
        if (Physics.Raycast(transform.position, transform.forward - transform.up, out hit, data.eyeSightDistance, terrainLayerMask))
        {
            distances[5] = hit.distance;
        }
        if (Physics.Raycast(transform.position, -transform.up, out hit, data.eyeSightDistance, terrainLayerMask))
        {
            distances[6] = hit.distance;
        }
    }

    private void DecideWhatFishDoes()
    {
        GetDistances();
        switch (state)
        {
            case -1:
                Escape();
                break;
            case 0:
                SearchForFood();
                break;
            case 2:
                SwimTowardsPlant();
                LookForPlants();
                break;
            case 6:
                Chase();
                LookForPrey();
                break;
            default:
                break;
        }
    }

    private void SearchForFood()
    {
        if (timeSinceLastCheck <= 0)
        {
            timeSinceLastCheck = 5f;
            if (checkedPositions.Count >= 8)
            {
                if(data.type == 0) {
                    LookForPlants();
                }
                if(data.type == 1){
                    LookForPrey();
                }
            }
        } else
        {
            timeSinceLastCheck -= Time.deltaTime;
        }

    }

    private Vector3 GetNewDir()
    {
        bool isChosen = false;
        Vector3 newDir = new Vector3();
        while (isChosen == false)
        {
            isChosen = true;
            newDir = new Vector3(Random.Range(0, 1), 0, Random.Range(0, 1)) * data.eyeSightDistance; // rzucamy nowy okrąg oddalony o zasięg wzroku
            for (int i = 0; i < checkedPositions.Count; i++)
            {
                if (Vector3.Distance(transform.position + newDir, checkedPositions[i]) < data.eyeSightDistance / 2)
                {
                    isChosen = false;
                    break;
                }
            }
        }
        return newDir;
        /*
        if (checkedPositions.Count >= maxPositions)
        {
            checkedPositions.RemoveAt(0);
        }
        checkedPositions.Add(transform.position + newDir) */
    }

    private void GetWhatFishSees()
    {
        timeToCheckSight -= Time.deltaTime;
        if (timeToCheckSight < 0)
        {
            timeToCheckSight = data.reactionTime;
            objectsInSight.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, data.eyeSightDistance);
            // Co tu si� dzieje, to robimy sobie list� collider�w, kt�re s� w zasi�gu same w sobie
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject.layer == agentLayerMask && (transform.position - hitCollider.transform.position).magnitude > 0.5f)
                {
                    lookCheckerHorizontal.LookAt(hitCollider.transform);
                    lookCheckerHorizontal.localEulerAngles = new Vector3(0, lookCheckerHorizontal.localEulerAngles.y, 0);
                    float horizontalAngle = lookCheckerHorizontal.localEulerAngles.y;
                    lookCheckerVertical.LookAt(hitCollider.transform);
                    lookCheckerVertical.localEulerAngles = new Vector3(lookCheckerVertical.localEulerAngles.x, 0, 0);
                    float verticalAngle = lookCheckerVertical.localEulerAngles.x;
                    if (verticalAngle > 180)
                    {
                        verticalAngle = 360 - verticalAngle;
                    }
                    Debug.Log(hitCollider.gameObject.name + " " + horizontalAngle + " " + verticalAngle);
                    if (Mathf.Abs(verticalAngle) <= data.eyeAngle.z) // Mie�ci si� w wertykalnym zakresie wzroku ryby
                    {
                        if (horizontalAngle > 180)
                        {
                            horizontalAngle = -(360 - horizontalAngle);
                        }
                        if (Mathf.Abs(horizontalAngle) >= data.eyeAngle.x && Mathf.Abs(horizontalAngle) <= data.eyeAngle.y)
                        {
                            // Ryba widzi obiekt
                            objectsInSight.Add(hitCollider.gameObject.transform.root.gameObject);
                        }
                    }
                }/*
                else if (hitCollider.gameObject.layer == terrainLayerMask)
                {
                    
                }*/
            }
        }
    }

    private void CheckForDanger(){
        foreach(var obj in objectsInSight){
            var otherData = obj.GetComponent(typeof(FishData)) as FishData;
            if(otherData == null) continue;
            if(otherData.type != 0 && otherData.stomachSize < data.stomachSize){ //czy inna ryba jest roślinożercą i czy jest bardziej głodna od nas
                escaping = obj;
                state = -1;
                return;
            }
        }
        escaping = null;
        state = 0;
    }

    private void Escape(){
        /*
        szukanie kryjówki
        foreach(var obj in objectsInSight){
            if (obj is Hideout){
                direction = obj.transform.position - transform.position;
                direction.Normalize();
                MoveFish(direction);
                return;
            }
        }*/
        Vector3 direction = transform.position - escaping.transform.position;
        direction.Normalize();
        MoveFish(direction * data.boostSpeed);
    }

    private void LookForPrey(){
        foreach(var obj in objectsInSight){
            var otherData = obj.GetComponent(typeof(FishData)) as FishData;
            if (otherData == null) continue;
            if (otherData.stomachSize > data.stomachSize){
                pursuing = obj;
                state = 6;
                return;
            }
        }
        pursuing = null;
        state = 0;
    }

    private void Chase(){
        if ((pursuing.transform.position - transform.position).magnitude <= 2){
            AttackFish();
            return;
        }
        Vector3 direction = pursuing.transform.position - transform.position;
        direction.Normalize();
        MoveFish(direction * data.boostSpeed);
    }

    private void AttackFish(){
        var opponent = pursuing.GetComponent(typeof(FishAI)) as FishAI;
        opponent.health--;
        hunger++;
    }

    private void LookForPlants(){
        foreach(var obj in objectsInSight){
            var otherData = obj.GetComponent(typeof(Plant)) as Plant;
            if (otherData == null) continue;
            if (otherData.amountRemaining > 0){
                plantToEat = obj;
                state = 2;
                return;
            }
        }
        plantToEat = null;
        state = 0;
    }

    private void SwimTowardsPlant(){
        if ((plantToEat.transform.position - transform.position).magnitude <= 2){
            EatPlant();
            return;
        }
        Vector3 direction = plantToEat.transform.position - transform.position;
        direction.Normalize();
        MoveFish(direction * data.speed);
    }


    private void EatPlant(){
        hunger += plantToEat.Consume(1);
    } 

    private void MoveFish(Vector3 direction){
        transform.position += direction;
    }
}
