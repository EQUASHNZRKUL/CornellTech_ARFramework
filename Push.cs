using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Represents a ball moved by raycasts/taps that is created when raycast intersects a plane. 
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PushTrack : MonoBehavior
{
    // Obj prefab ref
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at touch location")]
    GameObject arPrefab;

    public GameObject placedPrefab
    {
        get { return arPrefab; }
        set { arPrefab = value; }
    }

    // Object ref
    public GameObject spawnedObject {get; private set; }

    // Event to invoke for placing objects on planes.
    public static event Action planePlacedObject;

    // Session Origin ref; used to make raycasts
    private ARSessionOrigin sessionOrigin;

    // Ray Cast manager ref
    ARRaycastManager RaycastManager; 

    // Frag if object was placed or it should be moved
    private bool objectPlaced = false;

    // Bool if moving or not (needed for Update())
    private bool objectMotion = false;

    // Thrust value attached to the 
    public float thrust

    void Start()
    {
    // Ref to Ar session origin within GameObject
    sessionOrigin = GetComponent<ARSessionOrigin>();

    // Instance of object to be hidden until placed. 
    spawnedObject = Instantiate(arPrefab);
    spawnedObject.gameObject.SetActive(false);
    }

    void Update()
    {   
        // Check if object is moving, if so then move it
        if (objectMotion)
            spawnedObject.gameObject.transform += Vector3.up * 10.0f;

        // Checks if screen pressed
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        // List of AR Hits
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        // Get centerpoint of screen
        var screenPoint = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 4);

        // Find raycast hit surface
        if (sessionOrigin.Raycast(screenPoint, hits))
        {
            // Find first hit
            hits.OrderBy(h => h.distance);
            var pose = hits[0].pose;
            

        }
    }

}