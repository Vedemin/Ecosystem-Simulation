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

    void Start()
    {
        data = GetComponent<FishData>();
        hunger = data.stomachSize;
        timeToCheckSight = data.reactionTime;
    }

    void Update()
    {
        GetWhatTheFishSees();
    }

    private void GetWhatTheFishSees()
    {
        timeToCheckSight -= Time.deltaTime;
        if (timeToCheckSight < 0)
        {
            timeToCheckSight = data.reactionTime;
            objectsInSight.Clear();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, data.eyeSightDistance);
            // Co tu siê dzieje, to robimy sobie listê colliderów, które s¹ w zasiêgu same w sobie
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
                    if (Mathf.Abs(verticalAngle) <= data.eyeAngle.z) // Mieœci siê w wertykalnym zakresie wzroku ryby
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
                }
            }
        }
    }
}
