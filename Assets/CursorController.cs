using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D mouseUpSprite;
    [SerializeField] private Texture2D mouseDownSprite;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(mouseDownSprite, Vector2.zero, CursorMode.Auto);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(mouseUpSprite, Vector2.zero, CursorMode.Auto);
        }
    }
}
