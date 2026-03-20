using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    void Start()
    {
        // Просто подгоняем камеру под ширину экрана
        float screenRatio = (float)Screen.width / Screen.height;
        Camera.main.orthographicSize = 5f * (9f / 16f) / screenRatio;
    }
}
