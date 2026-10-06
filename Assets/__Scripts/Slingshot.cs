using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour {
    // fields set in the Unity Inspector pane
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    public LineRenderer rubberBand;

    public AudioClip launchSound;

private AudioSource audioSource;

    // fields set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        rubberBand.positionCount = 2;
        rubberBand.enabled = false;

        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseEnter() {
        launchPoint.SetActive(true);
    }

    void OnMouseExit() {
        launchPoint.SetActive(false);
    }

    void OnMouseDown() {

        if (MissionDemolition.GET_MODE() != GameMode.playing) {
            return;
        }

        aimingMode = true;

        rubberBand.enabled = true;

        projectile = Instantiate(projectilePrefab) as GameObject;

        projectile.transform.position = launchPos;

        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update() {
        if (!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;

        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;

        float maxMagnitude = GetComponent<SphereCollider>().radius;

        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        rubberBand.SetPosition(0, launchPos);
        rubberBand.SetPosition(1, projectile.transform.position);

        if (Input.GetMouseButtonUp(0)) {
            aimingMode = false;

            audioSource.PlayOneShot(launchSound);

            rubberBand.enabled = false;

            Rigidbody projRB = projectile.GetComponent<Rigidbody>();

            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            FollowCam.POI = projectile;

            Instantiate<GameObject>(projLinePrefab, projectile.transform);

            projectile = null;

            MissionDemolition.SHOT_FIRED();
        }
    }
}