using UnityEngine;

public class CameraSizer : MonoBehaviour
{
    public Vector2 targetAspect = new Vector2(16, 9);
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        UpdateCrop();
    }

    private void FixedUpdate()
    {
        UpdateCrop();
    }

    public void UpdateCrop()
    {
        var screenRatio = Screen.width / (float) Screen.height;
        var targetRatio = targetAspect.x / targetAspect.y;

        if (Mathf.Approximately(screenRatio, targetRatio))
        {
            _camera.rect = new Rect(0, 0, 1, 1);
        }
        else if (screenRatio > targetRatio)
        {
            var normalizedWidth = targetRatio / screenRatio;
            var barThickness = (1f - normalizedWidth) / 2f;
            _camera.rect = new Rect(barThickness, 0, normalizedWidth, 1);
        }
        else
        {
            var normalizedHeight = screenRatio / targetRatio;
            var barThickness = (1f - normalizedHeight) / 2f;
            _camera.rect = new Rect(0, barThickness, 1, normalizedHeight);
        }
    }
}