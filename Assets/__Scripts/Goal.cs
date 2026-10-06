using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Goal : MonoBehaviour {
    // A static field accessbile by code anywhere
    static public bool goalMet = false;

    public GameObject explosionPrefab;

    void OnTriggerEnter(Collider other) {
        // When the trigger is hit by something
        // Check to see if it's a Projectile
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null) {
            // If so, set goalMet to true
            goalMet = true;

            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Also set the alpha of the color to higher opacity
            Material mat = GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = 0.75f;
            mat.color = c;
        }
    }
}
