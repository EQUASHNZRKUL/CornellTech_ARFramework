using System;
using System.Collections.Generic;
// using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Represents a ball moved by raycasts/taps that is created when raycast intersects a plane. 
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class ARPushable : MonoBehaviour
{
    // Obj prefab ref
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at touch location")]
    GameObject arPrefab;

    /// The Prefab to instantiate on touch.
    public GameObject placedPrefab
    {
        get { return arPrefab; }
        set { arPrefab = value; }
    }

    // Visible, spawned object ref
    public GameObject spawnedObject {get; private set; }

    // Event to invoke for placing objects on planes.
    // public static event Action planePlacedObject;

    // Session Origin ref; used to make raycasts
    private ARSessionOrigin sessionOrigin;

    // Ray Cast manager ref
    ARRaycastManager RaycastManager; 

    // Frag if object was placed or it should be moved
    // private bool objectPlaced = false;

    // Bool if moving or not (needed for Update())
    private bool objectMotion = false;

    // Thrust value attached to the object
    // public float thrust;

    void Start()
    {
    // Ref to Ar session origin within GameObject
    Debug.Log("Start Test");
    sessionOrigin = GetComponent<ARSessionOrigin>();

    // Instance of object to be hidden until placed. 
    // spawnedObject = Instantiate(arPrefab);
    // spawnedObject.gameObject.SetActive(false);
    }

    // Awake is run on Load.
    void Awake()
    {
        RaycastManager = GetComponent<ARRaycastManager>(); 
    }

    //TODO: What is out?
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Input.mousePosition;
            touchPosition = new Vector2(mousePosition.x, mousePosition.y);
            return true;
        }
#else
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
#endif

        touchPosition = default;
        return false;
    }

    void Update()
    {   
        // Check if object is moving, if so then move it
        // if (objectMotion)
        //     spawnedObject.gameObject.transform += Vector3.up * 10.0f;

        // Checks if screen pressed
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        // List of AR Hits
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (RaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        { // If raycast hits a plane
            Debug.Log("HIT");
            // Find first hit
            // hits.OrderBy(h => h.distance);
            var pose = hits[0].pose;
            var hittype = hits[0].hitType;
            // TODO: this is gonna need to change once multiples added
            if (spawnedObject == null) 
                Debug.Log("Instantiate");
                spawnedObject = Instantiate(arPrefab, pose.position, pose.rotation);
        }
        else 
        // if (RaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinInfinity))
        { // if raycast hits anything else
            var pose = hits[0].pose;
            var hittype = hits[0].hitType;
            Debug.Log("MISS");
            // TODO: This is where the physics happens
            Vector3 cam_pos = sessionOrigin.camera.transform.position;
            Debug.DrawLine(cam_pos, pose.position, Color.red);
            objectMotion = true;
        }
    }
}