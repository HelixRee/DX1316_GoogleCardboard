using UnityEngine;
using UnityEngine.UI;

public class Picture : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetImageTex(RenderTexture renderTexture)
    {
        // Make the render texture active to read its pixels
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        if (renderTexture == null)
        {
            Debug.Log("wadafak");
            return;
        }
        // Create a new Texture2D to hold the pixel data
        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);

        // Read the pixels from the render texture into the Texture2D and apply
        texture2D.ReadPixels(rect, 0, 0);
        texture2D.Apply();

        // Restore the previous active render texture
        RenderTexture.active = currentActiveRT;

        // Create a new Sprite from the Texture2D and assign it to the SpriteRenderer
        Sprite newSprite = Sprite.Create(texture2D, rect, new Vector2(0.5f, 0.5f)); // Pivot at center
        image.sprite = newSprite;

        // Optional: Clean up the temporary render texture if it's not reused often (careful with this if you re-run this method frequently)
        // Destroy(renderTexture); 
    }
}
