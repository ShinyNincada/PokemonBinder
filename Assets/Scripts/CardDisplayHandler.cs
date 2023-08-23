using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDisplayHandler : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform rotateImage;
    [SerializeField] private float rotationSpeed = 1.0f; // Adjust this value to control rotation speed
    [SerializeField]  Vector3 maxRotation = new Vector3(45.0f, 45.0f, 45.0f); // Maximum rotation angles for each axis
    private Vector2 startDragPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        startDragPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
         // Calculate the drag movement in the X and Y directions
        float dragDeltaX = eventData.delta.x;
        float dragDeltaY = eventData.delta.y;

        // Calculate a smooth rotation speed based on the drag movement
        float smoothRotationSpeed = rotationSpeed * Time.deltaTime;

        float newRotationX = rotateImage.rotation.eulerAngles.x - dragDeltaY * smoothRotationSpeed;
        float newRotationY = rotateImage.rotation.eulerAngles.y + dragDeltaX * smoothRotationSpeed;

        newRotationX = (newRotationX > 180) ? newRotationX - 360 : newRotationX;
        newRotationY = (newRotationY > 180) ? newRotationY - 360 : newRotationY;
        // Calculate the new rotation angles after dragging
        newRotationX = Mathf.Clamp(newRotationX, -maxRotation.x, maxRotation.x);
        newRotationY = Mathf.Clamp(newRotationY,  -maxRotation.y, maxRotation.y);

        // Create a new Quaternion rotation with the updated angles
        Quaternion newRotation = Quaternion.Euler(newRotationX, newRotationY, 0);

        // Apply the new rotation using Slerp for smoother rotation transitions
        rotateImage.rotation = Quaternion.Slerp(rotateImage.rotation, newRotation, smoothRotationSpeed);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rotateImage.rotation = Quaternion.Slerp(rotateImage.rotation, Quaternion.Euler(Vector3.zero), rotationSpeed * Time.deltaTime);
    }
}
