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

    /// <summary>
    /// Invoked whenever an object is placed in on a plane.
    /// </summary>
    public static event Action onPlacedObject;

    public float JUMP_FORCE = 10.0f;

    void Awake()
    {
        Console.WriteLine("StartTest");
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
            Debug.DrawRay (ray.origin, ray.direction * 10, Color.blue);
            bool physRayBool = Physics.Raycast(ray, out hit);
            bool arRayBool = m_ARRaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon);
            if (physRayBool) { // PhysicsRayIntersect();
                Collider spawnedCollider = hit.collider;
                Rigidbody spawnedRigidBody = hit.rigidbody;
                if ((hit.distance < s_Hits[0].distance) && (spawnedCollider.gameObject.tag == "AR Placed Object")) {
                    // Hit a spawned object
                    spawnedRigidBody.AddForce(Vector3.up*JUMP_FORCE)
                    // SendMessageTo(spawnedObject, "OnRayCastEnter");
                }
                else { //ARRayIntersect();
                    // Raycast hits are sorted by distance, so the first one will be the closest hit.
                    var hitPose = s_Hits[0].pose;
                    if (spawnedObject == null)
                    { //Instantiate a new sphere
                        // TODO: possible fix to enlarged sphere bug - use world coords of hitPose/s_Hits[0]
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

            // else if (m_ARRaycastManager.Raycast(touchPosition, s_Hits, TrackableType.All))
}
