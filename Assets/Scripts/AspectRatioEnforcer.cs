using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AspectRatioEnforcer : MonoBehaviour
{
    public Vector2 targetAspectRatio = new Vector2(4, 3);

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        float targetRatio = targetAspectRatio.x / targetAspectRatio.y;
        float windowRatio = (float)Screen.width / Screen.height;
        float scaleHeight = windowRatio / targetRatio;

        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }
}