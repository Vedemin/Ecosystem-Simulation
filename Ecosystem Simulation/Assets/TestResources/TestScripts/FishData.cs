using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishData : MonoBehaviour
{
    public int startingPopulation;
    public int type; // 0 - ro�lino�erca, 1 - mi�so�erca, 2 - wszystko�erca
    public float health;
    public float stomachSize; // Max ilo�� punkt�w po�ywienia
    public float stamina;
    public float speed; // Szybko�� poruszania si� na sekund�
    public float boostSpeed; // Szybko�� poruszania si� w trakcie ataku lub ucieczki
    public float boostCost; // Koszt szybkiego poruszania na sekund�
    public float minDepth;
    public float maxDepth;
    public float partnerAcceptChance;
    public float urge;

    public Vector3 eyeAngle; // X - min, Y - max, dla ka�dego oka mierzone od prosto przed siebie, Z - wertykalne
    public float eyeSightDistance;
    public float reactionTime;
}
