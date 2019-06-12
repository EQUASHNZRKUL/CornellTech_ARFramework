using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using OpenCVForUnity.CoreModule;
// using OpenCvForUnity.UnityUtils;
// using OpenCVForUnity.ImgprocModule;

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

    void OnEnable()
    {
        m_ARCameraManager.frameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        m_ARCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        // CAMERA IMAGE HANDLING
        XRCameraImage image;
        if (!m_ARCameraManager.TryGetLatestImage(out image))
            return;

        Debug.LogFormat("Dimensions: {0}\n\t Format: {1}\n\t Time: {2}\n\t ", 
            image.dimensions, image.format, image.timestamp);
        
        CameraImagePlane greyscale = image.GetPlane(0);

        image.Dispose();

        // Process the image here: 
        


        // m_Texture = new Texture2D(
        //     conversionParams.outputDimensions.x,
        //     conversionParams.outputDimensions.y,
        //     conversionParams.outputFormat,
        //     false);
        
        // m_Texture.LoadRawTextureData(buffer);
        // m_Texture.Apply();
        // buffer.Dispose();
    }

    void Update() {

    }

    ARCameraManager m_ARCameraManager;

    Texture2D m_Texture;
}
