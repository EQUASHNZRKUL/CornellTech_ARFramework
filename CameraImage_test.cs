using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using OpenCVForUnity;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
// [RequireComponent(typeof(ARCameraManager))]
// [RequireComponent(typeof(RawImage))]
public class CameraImage_test : MonoBehaviour
{
    public Mat imageMat;
    public Texture2D m_Texture;

    [SerializeField]
    ARCameraManager m_ARCameraManager;
    public ARCameraManager cameraManager
    {
        get {return m_ARCameraManager; }
        set {m_ARCameraManager = value; }
    }

    [SerializeField]
    RawImage m_RawImage;
    public RawImage rawImage 
    {
        get { return m_RawImage; }
        set { m_RawImage = value; }
    }

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

    void ComputerVisionAlgo(IntPtr greyscale) 
    {
        Debug.Log("Computer Vision Algo-ing");
        Utils.copyToMat(greyscale, imageMat);
        Mat tempMat; 
        Imgproc.threshold(imageMat, tempMat, 128, 255, Imgproc.THRESH_BINARY_INV);
        imageMat = tempMat;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        // CAMERA IMAGE HANDLING
        XRCameraImage image;
        if (!m_ARCameraManager.TryGetLatestImage(out image))
            return;

        Debug.LogFormat("Dimensions: {0}\n\t Format: {1}\n\t Time: {2}\n\t ", 
            image.dimensions, image.format, image.timestamp);
        int w = image.dimensions[0];
        int h = image.dimensions[1];
        
        XRCameraImagePlane greyscale = image.GetPlane(0);

        if (m_Texture == null || m_Texture.width != image.width || m_Texture.height != image.height)
        {
            var format = TextureFormat.R8;
            m_Texture = new Texture2D(image.width, image.height, format, false);
        }

        image.Dispose();

        // Process the image here: 
        // ComputerVisionAlgo(greyscale);
        unsafe {
            IntPtr greyPtr = (IntPtr) greyscale.data.GetUnsafePtr();
            ComputerVisionAlgo(greyPtr);
            Debug.LogFormat("imageMat Dimensions: {0}, {1}", imageMat.rows(), imageMat.cols());
            Utils.fastMatToTexture2D(imageMat, m_Texture, true, 0);
            Debug.LogFormat("m_Texture Dimensions: {0}, {1}", m_Texture.height, m_Texture.width);
        }

        m_Texture.Apply();

        m_RawImage.texture = m_Texture;

        // m_Texture = new Texture2D(
        //     conversionParams.outputDimensions.x,
        //     conversionParams.outputDimensions.y,
        //     conversionParams.outputFormat,
        //     false);
        
        // m_Texture.LoadRawTextureData(buffer);
        // m_Texture.Apply();
        // buffer.Dispose();
    }
}
