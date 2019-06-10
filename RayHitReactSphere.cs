using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RayHitReactSphere : MonoBehaviour
{
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

    public float JUMP_FORCE = 10.0f;

    // void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    // {
    //     Rigidbody m_rigidBody = gameObject.GetComponent<Rigidbody>();
    //     m_rigidBody.AddForce(Vector3.up * JUMP_FORCE);
    // }

    void OnRaycastEnter(GameObject sender)
    {
        // GetComponent<Renderer>().material.color = Color.red;
        Transform space = GetComponent<Rigidbody>().transform;
        testObject = Instantiate(m_PhysicalPrefab, space.position, space.rotation);
        // m_rigidBody.AddForce(Vector3.up * JUMP_FORCE);
    }
}