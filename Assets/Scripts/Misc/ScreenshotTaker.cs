using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    public KeyCode screenshotKey = KeyCode.P;

    private void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            string filename = $"screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log($"Screenshot saved to {filename}");
        }
    }
}
