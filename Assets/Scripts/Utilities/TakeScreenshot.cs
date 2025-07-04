using System.IO;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
    //Hardcoded!!
    private const string TargetFolder = @"C:\GameDev\Screenshots";


    [ContextMenu("Take Screenshot")]
    public void TakeScreenshotNow()
    {
        string fileName = "screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        string filePath = Path.Combine(TargetFolder, fileName);

        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Screenshot saved to: " + filePath);
    }
}
