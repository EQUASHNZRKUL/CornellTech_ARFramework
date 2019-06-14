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
    public Mat imageMat = new Mat(480, 640, CvType.CV_8UC1);
    public Texture2D m_Texture;
    private int iterate = 0; 

    private int m_cachedWidth = 0;
    private int m_cachedHeight = 0;

    private ScreenOrientation? m_CachedOrientation = null;

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
        // Debug.Log("Computer Vision Algo-ing");
        Utils.copyToMat(greyscale, imageMat);
        Imgproc.threshold(imageMat, imageMat, 128, 255, Imgproc.THRESH_BINARY_INV);
        Debug.LogFormat("Mat Dimensions: {0} x {1}", imageMat.cols(), imageMat.rows());
    }

    void ConfigureImageInSpace()
    {
        // if (m_CachedOrientation != Screen.orientation || m_CachedScreenDimensions.x != Screen.width)
        // {
        //     m_CachedOrientation = Screen.orientation;
        //     m_CachedScreenDimensions = new Vector2(Screen.width, Screen.height);
        // }

        Vector2 ScreenDimension = new Vector2(Screen.width, Screen.height);
        Debug.LogFormat("Screen Dimensions: {0} x {1} \n Screen Orientation: {2}", 
            Screen.width, Screen.height, Screen.orientation);
        int w = Screen.width;
        int h = Screen.height; 
        m_RawImage.transform.position = new Vector3(-w/2, h/2, 0);
        m_RawImage.uvRect = new UnityEngine.Rect(-w/2, h/2, w, h);
    }

    void ConfigureRawImageInSpace(int img_w, int img_h)
    {
        // if (m_CachedOrientation != Screen.orientation || m_CachedScreenDimensions.x != Screen.width)
        // {
        //     m_CachedOrientation = Screen.orientation;
        //     m_CachedScreenDimensions = new Vector2(Screen.width, Screen.height);
        // }

        Vector2 ScreenDimension = new Vector2(Screen.width, Screen.height);
        Debug.LogFormat("Screen Dimensions: {0} x {1} \n Screen Orientation: {2}", 
            Screen.width, Screen.height, Screen.orientation);

        int scr_w = Screen.width;
        int scr_h = Screen.height; 
        
        // Screen/Camera Ratios; Larger one is used as general multiplier to maintain proportionality
        float w_ratio = (float)scr_w/(float)img_w;
        float h_ratio = (float)scr_h/(float)img_h;
        float scale = Math.Max(w_ratio, h_ratio);

        int new_w = (int) (img_w * scale);
        int new_h = (int) (img_h * scale);

        Debug.LogFormat("\n Old Dimensions: {0} x {1} \n New Dimensions: {2} x {3}", img_w, img_h, new_w, new_h);

        m_RawImage.transform.position = new Vector3(-scr_w/2, scr_h/2, 0);
        m_RawImage.uvRect = new UnityEngine.Rect(-scr_w/2, scr_h/2, img_w, img_h);
        m_RawImage.transform.localScale = m_RawImage.transform.localScale * scale;
    }

    void BuildGreyscaleTexture(Texture2D Y)
    {

    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        // CAMERA IMAGE HANDLING
        XRCameraImage image;
        if (!m_ARCameraManager.TryGetLatestImage(out image))
            Debug.Log("UH OH");
            return;

        Debug.LogFormat("Dimensions: {0}\n\t Format: {1}\n\t Time: {2}\n\t ", 
            image.dimensions, image.format, image.timestamp);
        
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
            Utils.fastMatToTexture2D(imageMat, m_Texture, true, 0);
        }

        // m_Texture.Resize(Screen.width, Screen.height);

        // m_Texture.Apply();
        m_RawImage.texture = (Texture) m_Texture;
        Debug.Log(m_Texture.GetPixel(300, 300));
        Debug.LogFormat("Texture Dimensions: {0} x {1}", m_Texture.width, m_Texture.height);
    }
}
