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
[RequireComponent(typeof(ARCameraManager))]
public class CameraImage_test : MonoBehaviour
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

    public float JUMP_FORCE;

    public 

    void Awake()
    {
        // Console.WriteLine("StartTest");
        Debug.Log("StartTest");
        m_ARRaycastManager = GetComponent<ARRaycastManager>();
        m_SessionOrigin = GetComponent<ARSessionOrigin>();
        m_ARCameraManager = GetComponent<ARCameraManager>();
    }

    void Update()
    {
        // CAMERA IMAGE HANDLING
        XRCameraImage image;
        if (m_ARCameraManager.TryGetLatestImage(out image))
        {
            Debug.LogFormat("Dimensions: {0}\n\t Format: {1}\n\t Time: {2}\n\t ", 
                image.dimensions, image.format, image.timestamp);
            for (int planeIndex = 0; planeIndex < image.planeCount; ++planeIndex)
            {
                // Log information about image plane
                XRCameraImagePlane plane = image.GetPlane(planeIndex);
                Debug.LogFormat("Plane {0}:\n\tsize: {1}\n\tpixelStride{2}", 
                    planeIndex, plane.data.Length, plane.pixelStride);
                
                // Do something with the data: 
                // MyCVAlgo(plane.data);
            }

            image.Dispose();
        }

        // TOUCH HANDLING
        // Checks for inputs
        if (Input.touchCount <= 0)
            return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            // Distance calculations
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            bool physRayBool = Physics.Raycast(ray, out hit);
            bool arRayBool = m_ARRaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon);
            if (physRayBool) 
            { // PhysicsRayIntersect();
                Collider spawnedCollider = hit.collider;
                if ((arRayBool && spawnedCollider.gameObject.tag == "AR Placed Object") || (!arRayBool))
                { // Hits a spawned object
                    Debug.Log(hit.rigidbody);
                    Vector3 forceDirection = hit.normal * -1.0f;
                    spawnedObject.GetComponent<Rigidbody>().AddForce(forceDirection*JUMP_FORCE);
                }
                else if (spawnedCollider.gameObject.tag == "Plane Spawn") 
                { // Hits the plane
                    // Works if Physics.Raycast can hit ARPlanes. 
                    var hitPose = s_Hits[0].pose;
                    if (spawnedObject == null) 
                    { // Instantiate the sphere
                        spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position + (Vector3.up*0.1f), hitPose.rotation);
                        spawnedObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
                    }
                    else 
                    {
                        spawnedObject.transform.position = hitPose.position + (Vector3.up*0.1f);
                        spawnedObject.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
                    }
                }
            }
        }
    }
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_ARRaycastManager;

    ARCameraManager m_ARCameraManager;

    ARSessionOrigin m_SessionOrigin;
}
