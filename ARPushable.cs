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


    void Awake()
    {
        Console.WriteLine("StartTest");
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
    }

    void Update()
    {
        // Checks for inputs
        // if (!TryGetTouchPosition(out Vector2 touchPosition))
        //     return;
        if (Input.touchCount <= 0)
            return;

        Touch touch = Input.GetTouch(0);
        // TODO: figure out which of these should come first (distance function). 

        if (touch.phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast Hit");
                if (hit.rigidbody != null) {
                    var hitPose = hit.transform; 
                    hit.collider.enabled = false;
                    testObject = Instantiate(m_PhysicalPrefab, hitPose.position, hitPose.rotation);
                }
                else if (m_ARRaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = s_Hits[0].pose;
                    if (spawnedObject == null)
                        spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation); 
                    else
                        spawnedObject.transform.position = hitPose.position; 
                }
            }
        }
    }
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_ARRaycastManager;

    ARSessionOrigin m_SessionOrigin;
}
