using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishData : MonoBehaviour
{
    public int type; // 0 - roœlino¿erca, 1 - miêso¿erca, 2 - wszystko¿erca
    public float health;
    public float stomachSize; // Max iloœæ punktów po¿ywienia
    public float stamina;
    public float speed; // Szybkoœæ poruszania siê na sekundê
    public float boostSpeed; // Szybkoœæ poruszania siê w trakcie ataku lub ucieczki
    public float boostCost; // Koszt szybkiego poruszania na sekundê

    public Vector3 eyeAngle; // X - min, Y - max, dla ka¿dego oka mierzone od prosto przed siebie, Z - wertykalne
    public float eyeSightDistance;
    public float reactionTime;
}
