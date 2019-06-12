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
[RequireComponent(typeof(ARCameraManager))]
public class CameraImage_test : MonoBehaviour
{

    void Awake()
    {
        Debug.Log("StartTest");
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
    }
    ARCameraManager m_ARCameraManager;

}
