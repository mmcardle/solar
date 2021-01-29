using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGravity : MonoBehaviour
{

    public GameObject sun;
    public float sunMass = 10000000000000.0f;

    public GameObject planet;
    public float planetMass = 1000.0f;
    public float moonMass = 0.000001f;

    Dictionary <GameObject, Vector3> velocities = new Dictionary<GameObject, Vector3>();

    // Tweak variables
    //private float time = 0.001f;

    // Start is called before the first frame update
    void Start()
    {
        /*GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("moon");
        foreach (GameObject moon in gameObjects) {
            velocities.Add(moon, new Vector3(0f, 0f, 4.1f));
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {

        /*GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("moon");

        foreach (GameObject moon in gameObjects) {
            Vector3 originalPostition = moon.transform.position;

            Vector3 accFromPlanet = calculateForce(planet, planetMass, moon, moonMass);
            Vector3 accelerationPlanet = (accFromPlanet / moonMass);
            Vector3 accFromSun = calculateForce(sun, sunMass, moon, moonMass);
            Vector3 accelerationSun = (accFromSun / moonMass);

            //Vector3 acceleration = accelerationPlanet + accelerationSun;
            Vector3 acceleration = accelerationPlanet;
            
            Debug.Log("Moon: accFromPlanet " + accFromPlanet);
            Debug.Log("Moon: " + accelerationPlanet);

            Vector3 velocity = velocities[moon];

            moon.transform.position += (velocity * time + 0.5f * acceleration * time * time);

            Vector3 newPosition = moon.transform.position;

            Vector3 newVelocity = (newPosition - originalPostition) / time;

            velocities[moon] = newVelocity;

        }*/
    }

    public Vector3 calculateForce(GameObject centralObject, float centralMass, GameObject moon, float moonMass) {

        Vector3 planetPosition = centralObject.transform.position;
        Vector3 moonPosition = moon.transform.position;

        float distance = Vector3.Distance(planetPosition, moonPosition);

        float distanceSquared = distance * distance;

        float G = 6.67f * Mathf.Pow(10, -11);

        float force = 1000000000.0f * G * centralMass * moonMass / distanceSquared;

        Debug.Log("Force "+ force);

        Vector3 heading = (planetPosition - moonPosition);

        Debug.Log("Heading "+ heading);

        Vector3 forceWithDirection = (force * ( heading/heading.magnitude));

        Debug.Log("Force with Dir "+ forceWithDirection);

        return (forceWithDirection);

    }

}
