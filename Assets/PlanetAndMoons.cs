using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetAndMoons : MonoBehaviour
{

    private float G = 6.673f * Mathf.Pow(10, -11);

    public GameObject sun;
    public GameObject earth;

    // Masses scaled by 10^-15
    private float sunMass = 1.989f * Mathf.Pow(10, 15);
    private float planetMass = 5.972f * Mathf.Pow(10, 9);

    //private float earthMass = 100000000000000f;
    //private float moonMass = 0.001f;

    Dictionary <GameObject, Vector3> velocities = new Dictionary<GameObject, Vector3>();

    // Tweak variables
    private float time = 0.001f;
    private float maxDistance = 1; 

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log("G "+ G);
        Debug.Log("sunMass "+ sunMass);
        Debug.Log("planetMass "+ planetMass);

        Vector3 sunPosition = sun.transform.position;
        
        GameObject[] planets = GameObject.FindGameObjectsWithTag("planet");
        foreach (GameObject planet in planets) {
            
            Vector3 planetPosition = planet.transform.position;

            float distance = Vector3.Distance(planetPosition, sunPosition);

            maxDistance = Mathf.Max(maxDistance, distance);

            float relativePlanetMass = planetMass * distance;

            Vector3 force = (calculateForce(sun, sunMass, planet, relativePlanetMass) / relativePlanetMass);

            float initialVelocity = Mathf.Sqrt((G * sunMass) / distance);

            // TODO - Whey do we need to scale here?
            float scaledVelocity = initialVelocity * 0.708585f;
            
            Debug.Log("Calculated Velocity for Planet " + distance + " units away " + initialVelocity  + "-> " + scaledVelocity + " force: " + force);

            velocities.Add(planet, Vector3.Scale(new Vector3(scaledVelocity, scaledVelocity, scaledVelocity), planet.transform.forward));
        }
        
        /*Vector3 earthPosition = earth.transform.position;
        GameObject[] moonObjects = GameObject.FindGameObjectsWithTag("moon");
        foreach (GameObject moon in moonObjects) {

            Vector3 moonPosition = moon.transform.position;

            float distance = Vector3.Distance(moonPosition, earthPosition);

            Vector3 force = (calculateForce(earth, earthMass, moon, moonMass) / moonMass);

            float initialVelocity = Mathf.Sqrt((G * earthMass) / distance);

            // TODO - Whey do we need to scale here?
            float scaledVelocity = initialVelocity * 0.708585f;
            
            Debug.Log("Calculated Velocity for Moon" + distance + " units away " + initialVelocity  + "-> " + scaledVelocity + " force: " + force);

            velocities.Add(moon, Vector3.Scale(new Vector3(scaledVelocity, scaledVelocity, scaledVelocity), moon.transform.forward));
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        
        Vector3 sunPosition = sun.transform.position;

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("planet");
        foreach (GameObject planet in gameObjects) {
            Vector3 planetPosition = planet.transform.position;
            Vector3 acceleration = (calculateForce(sun, sunMass, planet, planetMass) / planetMass);
            Vector3 velocity = velocities[planet];

            float distance = Vector3.Distance(planetPosition, sunPosition);
            float relativeTime = time * (distance / maxDistance);

            planet.transform.position += (velocity * relativeTime + 0.5f * acceleration * relativeTime * relativeTime);
            Vector3 newPosition = planet.transform.position;
            Vector3 newVelocity = (newPosition - planetPosition) / relativeTime;
            velocities[planet] = newVelocity;
        }

        /*
        GameObject[] moonObjects = GameObject.FindGameObjectsWithTag("moon");
        foreach (GameObject moon in moonObjects) {
            Vector3 originalPosition = moon.transform.position;

            Vector3 forceEarth = calculateForce(earth, earthMass, moon, moonMass);
            Vector3 accelerationEarth = (forceEarth / moonMass);
            
            Vector3 forceSun = calculateForce(sun, sunMass, moon, moonMass);
            Vector3 accelerationSun = (forceSun / moonMass);

            //Vector3 acceleration = accelerationEarth + accelerationSun;
            Vector3 acceleration = accelerationEarth;
            //Vector3 acceleration = accelerationEarth;
            Vector3 velocity = velocities[moon];

            float relativeTime = time * 0.5f;

            moon.transform.position += (velocity * relativeTime + 0.5f * acceleration * relativeTime * relativeTime);

            Vector3 newPosition = moon.transform.position;

            Vector3 newVelocity = (newPosition - originalPosition) / relativeTime;

            velocities[moon] = newVelocity;

        }
        */
        
        /*GameObject[] moonObjects = GameObject.FindGameObjectsWithTag("moon");
        foreach (GameObject moon in moonObjects) {
            Vector3 originalPosition = moon.transform.position;
            Vector3 acceleration = (calculateForce(sun, sunMass, moon, moonMass) / moonMass);
            Vector3 velocity = velocities[moon];
            moon.transform.position += (velocity * time + 0.5f * acceleration * time * time);
            Vector3 newPosition = moon.transform.position;
            Vector3 newVelocity = (newPosition - originalPosition) / time;
            velocities[moon] = newVelocity;
        }*/
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
