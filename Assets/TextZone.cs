using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[RequireComponent(typeof(Collider))]
public class TextZone : MonoBehaviour
{
    public string Message;
    public TMP_Text UI_Text;

    public BoxCollider bc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI_Text.text = Message;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UI_Text.text = "";
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 bounds = bc.size;
        Vector3 rot = bc.transform.rotation.eulerAngles;
        Gizmos.color = Color.yellow;
        // using Gizmos.Drawline, draw a cube centred at the position of the box collider, with the size and rotation of the box collider
        Gizmos.DrawLine(transform.position + new Vector3(-bounds.x / 2, -bounds.y / 2, -bounds.z / 2), transform.position + new Vector3(bounds.x / 2, -bounds.y / 2, -bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(-bounds.x / 2, -bounds.y / 2, -bounds.z / 2), transform.position + new Vector3(-bounds.x / 2, -bounds.y / 2, bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(-bounds.x / 2, -bounds.y / 2, bounds.z / 2), transform.position + new Vector3(bounds.x / 2, -bounds.y / 2, bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(bounds.x / 2, -bounds.y / 2, bounds.z / 2), transform.position + new Vector3(bounds.x / 2, -bounds.y / 2, -bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(-bounds.x / 2, -bounds.y / 2, -bounds.z / 2), transform.position + new Vector3(-bounds.x / 2, bounds.y / 2, -bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(-bounds.x / 2, bounds.y / 2, -bounds.z / 2), transform.position + new Vector3(bounds.x / 2, bounds.y / 2, -bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(bounds.x / 2, bounds.y / 2, -bounds.z / 2), transform.position + new Vector3(bounds.x / 2, bounds.y / 2, bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(bounds.x / 2, bounds.y / 2, bounds.z / 2), transform.position + new Vector3(-bounds.x / 2, bounds.y / 2, bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(-bounds.x / 2, bounds.y / 2, bounds.z / 2), transform.position + new Vector3(-bounds.x / 2, -bounds.y / 2, bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(-bounds.x / 2, bounds.y / 2, bounds.z / 2), transform.position + new Vector3(-bounds.x / 2, bounds.y / 2, -bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(bounds.x / 2, bounds.y / 2, bounds.z / 2), transform.position + new Vector3(bounds.x / 2, -bounds.y / 2, bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(bounds.x / 2, bounds.y / 2, -bounds.z / 2), transform.position + new Vector3(bounds.x / 2, -bounds.y / 2, -bounds.z / 2));
        Gizmos.DrawLine(transform.position + new Vector3(-bounds.x / 2, bounds.y / 2, -bounds.z / 2), transform.position + new Vector3(-bounds.x / 2, -bounds.y / 2, -bounds.z / 2));

    }
}
