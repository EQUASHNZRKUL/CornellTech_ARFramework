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
        ARCameraManager.cameraFrameReceived += Update;
    }

    void OnDisable()
    {
        ARCameraManager.cameraFrameReceived -= Update;
    }

    unsafe void Update()
    {
        // CAMERA IMAGE HANDLING
        XRCameraImage image;
        if (!m_ARCameraManager.TryGetLatestImage(out image))
            return;

        Debug.LogFormat("Dimensions: {0}\n\t Format: {1}\n\t Time: {2}\n\t ", 
            image.dimensions, image.format, image.timestamp);

        XRCameraImageConversionParams conversionParams = new CameraImageConversionParams
        (
            // Get entire image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Choose RGBA format
            outputFormat = TextureFormat.RGBA32,

            // No Transformation
            transformation = CameraImageTransformation.None
        );

        // Get byte size of final image
        int size = BadImageFormatException.GetConvertedDataSize(conversionParams);

        // Allocate buffer to store image
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract image data
        image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

        // Image is stored, so we can safely discard the XRCameraImage object
        image.Dispose();

        // Process the image here: 
            m_Texture = new Texture2D(
                conversionParams.outputDimensions.x,
                conversionParams.outputDimensions.y,
                conversionParams.outputFormat,
                false);
            
            m_Texture.LoadRawTextureData(buffer);
            m_Texture.Apply();
            buffer.Dispose();
    }

    ARCameraManager m_ARCameraManager;

    Texture2D m_Texture;
}
