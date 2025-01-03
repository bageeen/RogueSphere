using UnityEngine;

public class GenerateTiledTexture : MonoBehaviour
{
    public Sprite patternSprite;  // Le sprite pattern à répéter
    public Vector2Int backgroundSize;  // Taille du background en tiles (ex: 10x10)
    public SpriteRenderer backgroundRenderer;  // Référence au SpriteRenderer du background
    public SpriteMask spriteMask;  // Référence au SpriteMask

    private const int MaxTextureSize = 16384;  // Limite de taille de texture

    void Start()
    {
        if (patternSprite == null || backgroundRenderer == null || spriteMask == null)
        {
            Debug.LogError("Assurez-vous que toutes les références sont correctement assignées dans l'Inspector.");
            return;
        }

        Texture2D tiledTexture = GenerateTexture();
        if (tiledTexture == null)
        {
            Debug.LogError("La texture générée dépasse les limites autorisées.");
            return;
        }

        Sprite tiledSprite = Sprite.Create(tiledTexture, new Rect(0, 0, tiledTexture.width, tiledTexture.height), new Vector2(0.5f, 0.5f));

        // Appliquer le sprite généré au SpriteRenderer du background
        backgroundRenderer.sprite = tiledSprite;

        // Appliquer le même sprite généré au Sprite Mask
        spriteMask.sprite = tiledSprite;
    }

    Texture2D GenerateTexture()
    {
        int tileWidth = patternSprite.texture.width;
        int tileHeight = patternSprite.texture.height;

        int width = tileWidth * backgroundSize.x;
        int height = tileHeight * backgroundSize.y;

        // Vérifier si les dimensions de la texture sont dans les limites autorisées
        if (width > MaxTextureSize || height > MaxTextureSize)
        {
            Debug.LogError($"La taille de la texture générée ({width}x{height}) dépasse la limite autorisée de {MaxTextureSize}x{MaxTextureSize}.");
            return null;
        }

        Texture2D texture = new Texture2D(width, height);

        for (int y = 0; y < backgroundSize.y; y++)
        {
            for (int x = 0; x < backgroundSize.x; x++)
            {
                Color[] pixels = patternSprite.texture.GetPixels();
                texture.SetPixels(x * tileWidth, y * tileHeight, tileWidth, tileHeight, pixels);
            }
        }

        texture.Apply();
        return texture;
    }
}
