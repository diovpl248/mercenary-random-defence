using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Transform leftTop;
    public Transform rightBottom;

    float halfHeight;
    float halfWidth;

    public float orthographicSize;
    float zoomSpeed = 0.5f;
    public float minSize = 100f;
    public float maxSize;

    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        orthographicSize = cam.orthographicSize;
        halfHeight = orthographicSize;
        halfWidth = orthographicSize;
    }

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            orthographicSize += deltaMagnitudeDiff * zoomSpeed;
            //축소시 최소크기, 확대시 최대크기 지정
            cam.orthographicSize = Mathf.Clamp(orthographicSize, minSize, maxSize - 1.0f);
        }

        halfHeight = cam.orthographicSize;
        halfWidth = cam.aspect * halfHeight;

        Vector3 vector = new Vector3(0, 0, 0);

        //확대시 화면 밖으로 나가지 않도록 함
        
        if (cam.transform.position.x - halfWidth < leftTop.transform.position.x)
        {
            vector.x = leftTop.transform.position.x - cam.transform.position.x + halfWidth;
        }
        else if (cam.transform.position.x + halfWidth > rightBottom.transform.position.x)
        {
            vector.x = rightBottom.transform.position.x - cam.transform.position.x - halfWidth;
        }

        if (cam.transform.position.y + halfHeight > leftTop.transform.position.y)
        {
            vector.y = leftTop.transform.position.y - cam.transform.position.y - halfHeight;
        }
        else if (cam.transform.position.y - halfHeight < rightBottom.transform.position.y)
        {
            vector.y = rightBottom.transform.position.y - cam.transform.position.y + halfHeight;
        }

        cam.transform.Translate(vector);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Vector3 delta = eventData.delta/-5f;
        Vector3 delta = eventData.delta/-5f;
        //delta.y = delta.z =  0;
        delta.z = 0;
        if(cam.transform.position.x + delta.x - halfWidth < leftTop.transform.position.x)
        {
            delta.x = leftTop.transform.position.x - cam.transform.position.x + halfWidth;
        }
        else if (cam.transform.position.x + delta.x + halfWidth > rightBottom.transform.position.x)
        {
            delta.x = rightBottom.transform.position.x - cam.transform.position.x - halfWidth;
        }

        if (cam.transform.position.y + delta.y + halfHeight > leftTop.transform.position.y)
        {
            delta.y = leftTop.transform.position.y - cam.transform.position.y - halfHeight;
        }
        else if (cam.transform.position.y + delta.y - halfHeight < rightBottom.transform.position.y)
        {
            delta.y = rightBottom.transform.position.y - cam.transform.position.y + halfHeight;
        }

        cam.transform.Translate(delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public float CamRatio()
    {
        return orthographicSize / cam.orthographicSize;
    }
}
