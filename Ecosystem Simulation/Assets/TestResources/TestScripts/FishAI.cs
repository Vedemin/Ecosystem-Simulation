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
    private List<Vector3> checkedPositions; // The last place is always the new destination
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
     * 5 - partner znaleziony: ryba kopuluje (Unused)
     * 6 - ryba ściga inna rybę
     */
    public GameObject escaping;
    public GameObject pursuing;
    public GameObject plantToEat;
    public GameObject partner;
    public GameObject egg;

    LayerMask lm;
    LayerMask tm;
    void Start()
    {
        lm = LayerMask.GetMask("Agents");
        tm = LayerMask.GetMask("Terrain");
        data = gameObject.GetComponent(typeof(FishData)) as FishData;
        health = data.health;
        hunger = data.stomachSize;
        timeToCheckSight = data.reactionTime;
        stamina = data.stamina;
        state = 0;
        distances = new float[7];
        checkedPositions = new List<Vector3>();
        data.eyeSightDistance = 150;
    }

    void Update()
    {
        if (health <= 0){
            Destroy(gameObject);
        }
        TakeMeAboveTheGround();
        GetWhatFishSees();
        //Debug.Log(objectsInSight.Count);
        if (data.type == 0) CheckForDanger();
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
        //GetDistances();
        switch (state)
        {
            case -1:
                Escape();
                break;
            case 0:
                SearchForFood();
                if(data.type == 0) LookForPlants();
                if(data.type == 1) LookForPrey();
                break;
            case 1:
                LookForPartner();
                SearchForFood();
                break;
            case 2:
                SwimTowardsPlant();
                LookForPlants();
                break;
            case 4:
                SwimTowardsPartner();
                break;
            case 6:
                Chase();
                LookForPrey();
                break;
            default:
                break;
        }
    }

    private void CleanUp()
    {
        if (checkedPositions.Count > 8)
        {
            checkedPositions.RemoveAt(0);
        }
    }

    private void TakeMeAboveTheGround(){
        Ray ray = new Ray (transform.position, -transform.up);
        RaycastHit hitInfo;
        if(!Physics.Raycast (ray, out hitInfo, 500)){
            Debug.DrawLine (ray.origin, hitInfo.point, Color.green);
            transform.position += new Vector3(0, 1, 0);
        }
    }

    private void SearchForFood()
    {
        int c = checkedPositions.Count;
        if (c > 0) // Mamy już przynajmniej jeden wygenerowany cel
        {
            if (transform.position.x == checkedPositions[c - 1].x && transform.position.z == checkedPositions[c - 1].z)
            {
                checkedPositions.Add(GetNewDir());
                CleanUp();
            }
            else
            {
                Vector3 rawDir = (checkedPositions[c - 1] - transform.position);
                Vector3 direction = rawDir.normalized * data.speed * Time.deltaTime;
                if (rawDir.magnitude < direction.magnitude)
                {
                    transform.position = checkedPositions[c - 1];
                } else
                {
                    MoveFish(direction);
                }

            }
        }
        else
        {
            checkedPositions.Add(GetNewDir());
        }

    }

    private Vector3 GetNewDir()
    {
        bool isChosen = false;
        Vector3 newTarget = new Vector3();
        while (isChosen == false) // Pętla nieskończona, ale dlaczego??????
        {
            isChosen = true;
            float angle = Random.Range(0, 360);
            Vector2 randomValues = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 newDir = new Vector3(randomValues.x, 0, randomValues.y).normalized * data.eyeSightDistance;
            //Vector3 newDir = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized * data.eyeSightDistance; // rzucamy nowy okrąg oddalony o zasięg wzroku
            newTarget = transform.position + newDir;
            if (newTarget.x > 0 && newTarget.z > 0 && newTarget.x < 1000 && newTarget.z < 1000)
            {
                for (int i = 0; i < checkedPositions.Count; i++)
                {
                    if (Vector3.Distance(newTarget, checkedPositions[i]) < data.eyeSightDistance / 2)
                    {
                        isChosen = false;
                        break;
                    }
                }
                if (isChosen == true)
                {
                    RaycastHit hit;
                    Vector3 origin = new Vector3(newTarget.x, 270, newTarget.z);
                    if (Physics.Raycast(origin, Vector3.down, out hit, 280, tm))
                    {
                        if (hit.distance > data.maxDepth || hit.distance < data.minDepth)
                        {
                            isChosen = false;
                        }
                        else
                        {
                            newTarget.y = 270 - hit.distance + (data.eyeSightDistance / 4); // Przypisujemy głębokość
                        }
                    }
                    else
                    {
                        isChosen = false; // Jeśli jest poniżej wszystkiego, to punkt jest poza mapą
                    }
                }
            } else
            {
                isChosen = false;
            }
            
            
        }
        return newTarget;
    }

    private void GetWhatFishSees()
    {
        timeToCheckSight -= Time.deltaTime;
        if (timeToCheckSight < 0)
        {
            timeToCheckSight = data.reactionTime;
            objectsInSight.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, data.eyeSightDistance, lm);
            // Co tu si� dzieje, to robimy sobie list� collider�w, kt�re s� w zasi�gu same w sobie
            foreach (var hitCollider in hitColliders)
            {
                if (Vector3.Distance(transform.position, hitCollider.transform.position) > 1.26f)
                {
                    //Debug.Log(Vector3.Distance(transform.position, hitCollider.transform.position));
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
                    //Debug.Log(hitCollider.gameObject.name + " " + horizontalAngle + " " + verticalAngle);
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
                            //Debug.Log(hitCollider.gameObject.transform.root.gameObject.name + " " + horizontalAngle + " " + verticalAngle);
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
        return;
        foreach(var obj in objectsInSight){
            if (obj == null) return;
            var otherData = obj.GetComponent(typeof(FishData)) as FishData;
            if(otherData == null) continue;
            if(otherData.type != 0){ //czy inna ryba jest roślinożercą i czy jest bardziej głodna od nas
                escaping = obj;
                state = -1;
                return;
            }
        }
        escaping = null;
        state = 0;
    }

    private void Escape(){
        if (escaping == null) return;
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
            if (obj == null) continue;
            var otherData = obj.GetComponent(typeof(FishData)) as FishData;
            if (otherData == null) continue;
            if (otherData.type == 0){
                pursuing = obj;
                state = 6;
                return;
            }
        }
        pursuing = null;
        state = 0;
    }

    private void Chase(){
        if (pursuing == null) return;
        if ((pursuing.transform.position - transform.position).magnitude <= data.boostSpeed * Time.deltaTime){
            AttackFish();
            return;
        }
        Vector3 direction = pursuing.transform.position - transform.position;
        direction.Normalize();
        MoveFish(direction * data.boostSpeed * Time.deltaTime);
    }

    private void AttackFish(){
        var opponent = pursuing.GetComponent(typeof(FishAI)) as FishAI;
        opponent.health--;
        // Debug.Log(opponent.health);
        hunger++;
    }

    private void LookForPlants(){
        // Debug.Log("Loopfor plant");
        foreach(var obj in objectsInSight){
            if (obj == null) continue;
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
        if (plantToEat == null) return;
        if ((plantToEat.transform.position - transform.position).magnitude <= data.speed * Time.deltaTime){
            EatPlant();
            return;
        }
        Vector3 direction = plantToEat.transform.position - transform.position;
        direction.Normalize();
        MoveFish(direction * data.speed * Time.deltaTime);
    }


    private void EatPlant(){
        var plantData = plantToEat.GetComponent(typeof(Plant)) as Plant;
        hunger += plantData.Consume(100);
    } 

    private void LookForPartner(){
        // Debug.Log("LookForPartner");
        foreach(var obj in objectsInSight){
            if (obj == null) continue;
            var otherData = obj.GetComponent(typeof(FishData)) as FishData;
            if (otherData == null) continue;
            //To do zmiany bedzie jezeli bedzie mozna rozroznic gatunek a nie tylko typ
            if (otherData.type == data.type){
                //Czy tez szuka partnera
                if(state == obj.GetComponent<FishAI>().state){
                    if(Random.Range(0.0f, 1.0f) < otherData.partnerAcceptChance){
                        partner = obj;
                        state = 4;
                        data.speed *= 2;
                        return;
                    }
                    state = 0;
                }
            }
        }
        partner = null;
        state = 1;
    }

    private void SwimTowardsPartner(){
        if (partner == null) return;
        // Debug.Log("SwimTowardsPartner");
        if ((partner.transform.position - transform.position).magnitude <= data.speed * Time.deltaTime){
            data.speed /= 2;
            Copulate();
            return;
        }
        Vector3 direction = partner.transform.position - transform.position;
        direction.Normalize();
        MoveFish(direction * data.speed * Time.deltaTime);
    }

    private void Copulate(){
        Ray ray = new Ray (transform.position, -transform.up);
        RaycastHit hitInfo;
        if(Physics.Raycast (ray, out hitInfo, 1000)){
            // Debug.Log("copulation");
            GameObject newEgg = Instantiate(egg);
            newEgg.transform.position = hitInfo.point;
            // newEgg.GetComponent<Egg>().start = true;
            newEgg.GetComponent<Egg>().fishToBorn = gameObject;
            var partnerData = partner.GetComponent(typeof(FishData)) as FishData;
            var p = partner.GetComponent(typeof(FishAI)) as FishAI;
            p.state = 0;
            partner = null;
            state = 0;
        }
    }

    private void MoveFish(Vector3 direction){
        transform.position += direction;
    }
}
