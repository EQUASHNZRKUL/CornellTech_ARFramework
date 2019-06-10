using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RayHitReactSphere : MonoBehaviour
{
    public float JUMP_FORCE = 10.0f;

    // void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    // {
    //     Rigidbody m_rigidBody = gameObject.GetComponent<Rigidbody>();
    //     m_rigidBody.AddForce(Vector3.up * JUMP_FORCE);
    // }

    void OnRaycastEnter(GameObject sender)
    {
        GetComponent<Renderer>().material.color = Color.red;
        // Rigidbody m_rigidBody = GetComponent<Rigidbody>();
        // m_rigidBody.AddForce(Vector3.up * JUMP_FORCE);
    }
}