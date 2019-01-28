using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    public Transform targetTransform;
    public int offsetX = 5, offsetY = 2;

    // Underlying variable fading variable.
    private bool fadingOut;

    private float delaySeconds = -1;

    private bool done;

    private GameManager.GameEvent gameEventCallback;

    // Getter setter for the fading out.
    public bool FadingOut
    {
        set
        {
            fadingOut = value;

            if (fadingOut) done = false;

            // Reset alpha value.
            Color color = image.color;
            color.a = 0f;
            image.color = color;
        }

        get
        {
            return fadingOut;
        }
    }
    public Image image;

    // CameraController controls the camera and can also fix itself on an object to follow.
    void Update()
    {
        if (!fadingOut) return; 
        Color color = image.color;
        // If the alpha is 1, due to clamp it cannot be larger so its ok.
        if (TolerateApproximate(color.a, 0.98f, 0.1f))
        {
            if (!done)
            {
                if (gameEventCallback != null)
                { 
                    gameEventCallback();
                    gameEventCallback = null;
                }
                // Debug.Log("Fadeout done");
                done = true;
            }
            // Stop unnessary iterations.
            return;
        }
        // Slowly increments by 0.1 the alpha value of the black screen creating a fade out effect. 
        color.a = Mathf.Lerp(image.color.a, 1f, 0.1f);
        image.color = color;
    }

    void LateUpdate () {
        // Follows the target transform.
        this.transform.position = new Vector3(targetTransform.position.x + offsetX, targetTransform.position.y + offsetY, transform.position.z);

        // If countdown is LESS THAN OR EQUAL to 0 and GREATER THAN -1
        if (delaySeconds <= 0f && delaySeconds > -1f)
        {
            FadingOut = true;
            delaySeconds = -1;
            // Debug.Log("Fading scene");
        } 
        else if (delaySeconds != -1)
        {
            delaySeconds -= (1 * Time.deltaTime);
        }

    }
  
    // When called it begins a countdown from 'seconds' and an optional callback is set for after the fadeout is done.
    public void FadeOutWithDelay(int seconds, GameManager.GameEvent gameEventCallback = null)
    {
        delaySeconds = seconds;
        this.gameEventCallback = gameEventCallback;
    }

    // A local utility method to test for if a float is almost another.
    private bool TolerateApproximate(float a, float b, float tolerance)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }

}
