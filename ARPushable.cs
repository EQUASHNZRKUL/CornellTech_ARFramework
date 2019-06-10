using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class ARPushable : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;
    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }


    [SerializeField]
    [Tooltip("Instantiates this prefab on a gameObject at the touch location.")]
    GameObject m_PhysicalPrefab;
    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject physicalPrefab
    {
        get { return m_PhysicalPrefab; }
        set { m_PhysicalPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject testObject { get; private set; }

    public float JUMP_FORCE = 10.0f;

    void Awake()
    {
        // Console.WriteLine("StartTest");
        Debug.Log("StartTest");
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
    }

    /// <summary>
    /// Handles logic if Physics.Raycast finds a valid collision. 
    /// </summary>
    void PhysicsRayIntersect(RaycastHit hit) {
        var hitPose = hit.transform;
        testObject = Instantiate(m_PhysicalPrefab, hitPose.position, hitPose.rotation);
    }

    /// <summary>
    /// Handles logic if ARRaycast finds a collision. 
    /// </summary>
    void ARRayIntersect(List<ARRaycastHit> s_Hits) {
        // Raycast hits are sorted by distance, so the first one will be the closest hit.
        var hitPose = s_Hits[0].pose;
        if (spawnedObject == null)
        {
            spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
        }
        else
        {
            spawnedObject.transform.position = hitPose.position;
        }
    }

    void SendMessageTo(GameObject target, string message) 
    {
        if (target)
            target.SendMessage(message, spawnedObject, SendMessageOptions.DontRequireReceiver);
    }

    void Update()
    {
        // Checks for inputs
        // if (!TryGetTouchPosition(out Vector2 touchPosition))
        //     return;
        if (Input.touchCount <= 0)
            return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            // Distance calculations
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            // Debug.DrawRay(ray.origin, ray.direction * 10, Color.blue);
            bool physRayBool = Physics.Raycast(ray, out hit);
            bool arRayBool = m_ARRaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon);
            if (physRayBool) 
            { // PhysicsRayIntersect();
                Collider spawnedCollider = hit.collider;
                if ((arRayBool && spawnedCollider.gameObject.tag == "AR Placed Object") || (!arRayBool))
                { // Hits a spawned object
                    // var hitPose = hit.transform;
                    // testObject = Instantiate(m_PhysicalPrefab, hit.point, hitPose.rotation);
                    Debug.Log(hit.rigidbody);
                    // TODO: Try translation, could be an issue with Force.
                    // hit.rigidbody.AddForce(Vector3.up*JUMP_FORCE);
                    spawnedObject.transform.Translate(Vector3.up * 0.5f);
                }
                else if (spawnedCollider.gameObject.tag == "Plane Spawn") 
                { // Hits the plane
                    // Works if Physics.Raycast can hit ARPlanes. 
                    var hitPose = s_Hits[0].pose;
                    if (spawnedObject == null) 
                    { // Instantiate the sphere
                        spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                    }
                    else 
                    {
                        spawnedObject.transform.position = hitPose.position;
                    }
                }
            }
        }
    }
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_ARRaycastManager;

    ARSessionOrigin m_SessionOrigin;
}
