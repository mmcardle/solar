using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunGravity : MonoBehaviour
{

    private float G = 6.673f * Mathf.Pow(10, -11);

    public GameObject sun;
    public GameObject earth;

    // Masses scaled
    private static float scaling = 1.0f * Mathf.Pow(10, -10);
    private float sunMass = 1.989f * Mathf.Pow(10, 30) * scaling;
    private float planetMass = 5.972f * Mathf.Pow(10, 24) * scaling;


    //private float earthMass = 100000000000000f;
    //private float moonMass = 0.001f;

    private float moonMass = 7.34767309f * Mathf.Pow(10, 22) * scaling; // 7.34767309 × 10^22 kilograms

    Dictionary <GameObject, Vector3> velocities = new Dictionary<GameObject, Vector3>();

    // Tweak variables
    private float time = 0.000005f;
    private int iterationsPerTime = 1;
    private int iterationsPerTimeMin = 1;
    private int iterationsPerTimeMax = 100;
    private float maxDistance = 1; 

    private bool sunGravityOn = true;
    private bool planetGravityOn = true; 
    private bool sunOnMoonGravityOn = true; 

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("G "+ G);
        Debug.Log("sunMass "+ sunMass);
        Debug.Log("planetMass "+ planetMass);

        if (sunGravityOn) {
            initialSunGravity();
        }

        if (planetGravityOn){
            initialPlanetGravity();
        }

    }

    void initialSunGravity() {
        Vector3 sunPosition = sun.transform.position;
        
        GameObject[] planets = GameObject.FindGameObjectsWithTag("planet");
        foreach (GameObject planet in planets) {
            
            Vector3 planetPosition = planet.transform.position;

            float distance = Vector3.Distance(planetPosition, sunPosition);

            maxDistance = Mathf.Max(maxDistance, distance);

            float relativePlanetMass = planetMass * distance;

            Vector3 force = (calculateForce(sun, sunMass, planet, relativePlanetMass) / relativePlanetMass);

            float initialVelocity = Mathf.Sqrt((G * sunMass) / distance);

            // TODO - Why do we need to scale here?
            float scaledVelocity = initialVelocity * 0.708585f;
            
            Debug.Log("Calculated Velocity for Planet " + planet.name + ": " + distance + " units away " + initialVelocity  + "-> " + scaledVelocity + " force: " + force);

            velocities.Add(planet, Vector3.Scale(new Vector3(scaledVelocity, scaledVelocity, scaledVelocity), planet.transform.forward));
        }
    }

    void initialPlanetGravity() {
        Vector3 earthPosition = earth.transform.position;
        Vector3 sunPosition = sun.transform.position;
        GameObject[] moonObjects = GameObject.FindGameObjectsWithTag("moon");
        foreach (GameObject moon in moonObjects) {

            Vector3 moonPosition = moon.transform.position;

            float distanceToEarth = Vector3.Distance(moonPosition, earthPosition);
            float distanceToSun = Vector3.Distance(moonPosition, sunPosition);

            //Vector3 force = (calculateForce(earth, planetMass, moon, moonMass) / moonMass);

            Vector3 forceEarth = calculateForce(earth, planetMass, moon, moonMass);
            Vector3 accelerationEarth = (forceEarth / moonMass);

            Vector3 forceSun = calculateForce(sun, sunMass, moon, moonMass);
            Vector3 accelerationSun = (forceSun / moonMass);

            float initialVelocityFromEarth = Mathf.Sqrt((G * planetMass) / distanceToEarth);
            float scaledVelocity = initialVelocityFromEarth * 25.708585f;
            
            if (sunOnMoonGravityOn) {
                float initialVelocityFromSun = Mathf.Sqrt((G * sunMass) / distanceToSun);
                scaledVelocity = (scaledVelocity + initialVelocityFromSun * 0.108585f) ;
            }

            // TODO - Why do we need to scale here?
            //float scaledVelocity = initialVelocity * 0.688585f;
            //float scaledVelocity = (initialVelocityFromEarth + initialVelocityFromSun) * 0.308585f;
            
            Debug.Log("Calculated Velocity for Moon " + moon.name + " " + distanceToEarth + " " + distanceToSun + " units away " + scaledVelocity + " forceDuetoEarth: " + accelerationEarth + " forceDuetoSun: " + accelerationSun);

            velocities.Add(moon, Vector3.Scale(new Vector3(scaledVelocity, scaledVelocity, scaledVelocity), moon.transform.forward));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            iterationsPerTime = Mathf.Min(iterationsPerTime + 1, iterationsPerTimeMax);
            Debug.Log("Up "+ iterationsPerTime);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)){
            iterationsPerTime = Mathf.Max(iterationsPerTime - 1, iterationsPerTimeMin);
            Debug.Log("Down " + iterationsPerTime);
        }
    }

    void FixedUpdate() {
        if (sunGravityOn) {
            deltaSunGravity();
        }

        if (planetGravityOn){
            deltaPlanetGravity();
        }

    }

    void deltaSunGravity() {
        
        Vector3 sunPosition = sun.transform.position;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("planet");
        foreach (GameObject planet in gameObjects) {
            for (int i = 0; i < iterationsPerTime; i++) {
                Vector3 planetPosition = planet.transform.position;
                Vector3 acceleration = (calculateForce(sun, sunMass, planet, planetMass) / planetMass);
                Vector3 velocity = velocities[planet];

                float relativeTime = time;
                // If you want to make speed relative to distance
                float distance = Vector3.Distance(planetPosition, sunPosition);
                relativeTime = relativeTime * (distance / maxDistance);

                planet.transform.position += (velocity * relativeTime + 0.5f * acceleration * relativeTime * relativeTime);
                Vector3 newPosition = planet.transform.position;
                Vector3 newVelocity = (newPosition - planetPosition) / relativeTime;
                velocities[planet] = newVelocity;
            }
        }

    }

    void deltaPlanetGravity() {
        
        GameObject[] moonObjects = GameObject.FindGameObjectsWithTag("moon");
        foreach (GameObject moon in moonObjects) {
            for (int i = 0; i < iterationsPerTime; i++) {
                Vector3 originalPosition = moon.transform.position;

                Vector3 forceEarth = calculateForce(earth, planetMass, moon, moonMass);
                Vector3 accelerationEarth = (forceEarth / moonMass);
                
                Vector3 forceSun = calculateForce(sun, sunMass, moon, moonMass);
                Vector3 accelerationSun = (forceSun / moonMass);

                // TODO - not working
                Vector3 acceleration = accelerationEarth;
                Vector3 velocity = velocities[moon];

                if (sunOnMoonGravityOn) {
                    acceleration = (acceleration * 3000.0f) + (accelerationSun * 0.002f);
                }

                float relativeTime = time * 1.0f;

                Vector3 delta = (velocity * relativeTime + 0.5f * acceleration * relativeTime * relativeTime);
                //Debug.Log("Moon delta" + delta);

                moon.transform.position += delta;

                Vector3 newPosition = moon.transform.position;

                Vector3 newVelocity = (newPosition - originalPosition) / relativeTime;

                velocities[moon] = newVelocity;
                //Debug.Log("Moon accelerationEarth" + accelerationEarth);
                //Debug.Log("Moon velocity" + newVelocity);
            }
        }
    }

    public Vector3 calculateForce(GameObject object1, float object1Mass, GameObject object2, float object2Mass) {

        Vector3 object1Position = object1.transform.position;
        Vector3 object2Position = object2.transform.position;

        float distance = Vector3.Distance(object1Position, object2Position);

        float distanceSquared = distance * distance;
        
        float force = G * object1Mass * object2Mass / distanceSquared;

        //Debug.Log("Force "+ force + "N");

        Vector3 heading = (object1Position - object2Position);

        //Debug.Log("Heading "+ heading);
        //Debug.Log("Heading magnitude "+ heading.magnitude);

        Vector3 forceWithDirection = (force * ( heading/heading.magnitude));
        //Debug.Log("Force with Dir "+ forceWithDirection);
        return forceWithDirection;

    }

}
