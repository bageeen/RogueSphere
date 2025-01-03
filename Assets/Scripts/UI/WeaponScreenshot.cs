using UnityEngine;
using System.IO;

public class WeaponScreenshot : MonoBehaviour
{
    public GameObject prefab;
    public Camera screenshotCamera;
    public string customSavePath = "Screenshots"; // Default directory for saving screenshots

    private GameObject weaponInstance;

    public Sprite CaptureScreenshot(GameObject playerBody)
    {
        // Clean up any previous weapon instances
        CleanUp();

        // Move player body and weapon to an off-screen position
        Vector3 originalPosition = playerBody.transform.position;
        Quaternion originalRotation = playerBody.transform.rotation;
        Transform originalParent = playerBody.transform.parent;

        playerBody.transform.parent = null;
        playerBody.transform.position = new Vector3(1000, 1000, originalPosition.z);
        playerBody.transform.rotation = Quaternion.identity;

        weaponInstance = Instantiate(prefab, playerBody.transform);

        // Ensure the screenshot camera is properly positioned and sized
        SetupScreenshotCamera();

        // Take screenshot
        RenderTexture renderTexture = new RenderTexture(500, 600, 24); // Adjusted to be more square-like
        screenshotCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        screenshotCamera.Render();

        Texture2D screenshot = new Texture2D(500, 600, TextureFormat.RGB24, false); // Adjusted to be more square-like
        screenshot.ReadPixels(new Rect(0, 0, 500, 600), 0, 0); // Adjusted to be more square-like
        screenshot.Apply();

        // Convert Texture2D to Sprite
        Rect rect = new Rect(0, 0, screenshot.width, screenshot.height);
        Sprite screenshotSprite = Sprite.Create(screenshot, rect, new Vector2(0.5f, 0.5f));

        // Clean up
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        CleanUp();


        // Move player body back to original position
        playerBody.transform.position = originalPosition;
        playerBody.transform.rotation = originalRotation;
        playerBody.transform.parent = originalParent;
        playerBody.transform.SetSiblingIndex(0);

        return screenshotSprite;
    }

    private void CleanUp()
    {
        bool isGamePaused = Time.timeScale == 0;
        if (weaponInstance != null)
        {
            if (isGamePaused)
            {
                DestroyImmediate(weaponInstance);
            }
            else
            {
                Destroy(weaponInstance);
            }
            weaponInstance = null;
        }
    }

    private void SetupScreenshotCamera()
    {
        // Configure the camera to capture the desired region
        screenshotCamera.orthographic = true; // Assuming 2D setup
        screenshotCamera.orthographicSize = 5; // Maintain this size for the weapon and body
        screenshotCamera.aspect = 5f / 6f; // Adjusted aspect ratio to be more square-like (width/height ratio)
        screenshotCamera.transform.position = new Vector3(1000, 1000, -10); // Position it off-screen
    }
}
