using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RayHitReact : MonoBehaviour, IPointerEnterHandler
{
    public float JUMP_FORCE = 1.0f;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Rigidbody m_rigidBody = gameObject.GetComponent<Rigidbody>();
        m_rigidBody.AddForce(Vector3.up * JUMP_FORCE);
    }
}