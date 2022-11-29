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
<<<<<<< Updated upstream
    public int terrainLayerMask;
    public float[] distances;
    /*
     * 0 - 90* lewo
     * 1 - 45* lewo
     * 2 - prosto
     * 3 - 45* prawo
     * 4 - 90* prawo
     * 5 - 45* w dó³
     * 6 - 90* w dó³
     */
    public int state;
    /*
     * -1 - ucieczka
     * 0 - szukanie po¿ywienia
     * 1 - szukanie partnera
     * 2 - po¿ywienie znalezione: ryba p³ynie do niego
     * 3 - po¿ywienie znalezione: ryba je
     * 4 - partner znaleziony: ryba p³ynie do niego
     * 5 - partner znaleziony: ryba kopuluje
     */
=======
    public GameObject escaping;
    public GameObject pursuing;
>>>>>>> Stashed changes

    void Start()
    {
        data = GetComponent<FishData>();
        hunger = data.stomachSize;
        timeToCheckSight = data.reactionTime;
        state = 0;
    }

    void Update()
    {
        GetWhatTheFishSees();
        CheckForDanger();
        if(escaping != null){
            Escape();
        }
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
    }

    private void DecideWhatFishDoes()
    {
        switch (state)
        {
            case -1:
                Escape();
                break;
            case 0:
                SearchForFood();
                break;
            default:
                break;
        }
    }

    private void SearchForFood()
    {
        
    }

    private void GetWhatTheFishSees()
    {
        timeToCheckSight -= Time.deltaTime;
        if (timeToCheckSight < 0)
        {
            timeToCheckSight = data.reactionTime;
            objectsInSight.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, data.eyeSightDistance);
            // Co tu siï¿½ dzieje, to robimy sobie listï¿½ colliderï¿½w, ktï¿½re sï¿½ w zasiï¿½gu same w sobie
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
                    if (Mathf.Abs(verticalAngle) <= data.eyeAngle.z) // Mieï¿½ci siï¿½ w wertykalnym zakresie wzroku ryby
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
            otherData = gameObject.GetComponent(typeof(FishData)) as FishData;
            if(otherData == null) continue;
            if(otherData.type != 0 && otherData.stomachSize < data.stomachSize){ //czy inna ryba jest roÅ›linoÅ¼ercÄ… i czy jest bardziej gÅ‚odna od nas
                escaping = obj;
                return;
            }
        }
        escaping = null;
    }

    private void Escape(){
        /*
        szukanie kryjÃ³wki
        foreach(var obj in objectsInSight){
            if (obj is Hideout){
                direction = obj.transform.position - transform.position;
                direction.Normalize();
                MoveFish(direction);
                return;
            }
        }*/
        Vector3 direction = transform.position - escaping.transform.position;
        direction.Normalize()
        MoveFish(direction);
    }

    private void MoveFish(Vector3 direction){
        transform.position += direction;
    }
}
