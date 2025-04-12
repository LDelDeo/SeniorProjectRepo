using UnityEngine;
using UnityEngine.UI;

public class WriteTicket : MonoBehaviour
{
    public RawImage drawArea;
    public int brushSize = 5;
    public Color brushColor = Color.black;

    private Texture2D texture;
    private RectTransform drawRect;
    private GameObject parkedCar;

    public FPController fPController;
    public FPShooting fPShooting;
    public GameObject ticketPanel;

    [Header("Script Grabs")]
    private CrimeCompletion crimeCompletion;

    void Start()
    {
        drawRect = drawArea.rectTransform;

        // Create a blank white texture
        texture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
        Color[] fillColor = new Color[texture.width * texture.height];
        for (int i = 0; i < fillColor.Length; i++) fillColor[i] = Color.white;
        texture.SetPixels(fillColor);
        texture.Apply();

        drawArea.texture = texture;

        
    }
    

    void Update()
    {
        crimeCompletion = FindObjectOfType<CrimeCompletion>();

        if (Input.GetMouseButton(0))
        {
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(drawRect, Input.mousePosition, null, out localPos);

            // Convert to texture space
            float x = localPos.x + drawRect.rect.width / 2;
            float y = localPos.y + drawRect.rect.height / 2;

            int texX = Mathf.RoundToInt((x / drawRect.rect.width) * texture.width);
            int texY = Mathf.RoundToInt((y / drawRect.rect.height) * texture.height);

            DrawCircle(texX, texY);
            texture.Apply();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fPController.enabled = false;
        fPShooting.enabled = false;

        parkedCar = GameObject.Find("IllegallyParkedCar");
    }

    void DrawCircle(int x, int y)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (i * i + j * j <= brushSize * brushSize)
                {
                    int px = x + i;
                    int py = y + j;
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, brushColor);
                    }
                }
            }
        }
    }

    public void ClearCanvas()
    {
        Color[] clearColor = new Color[texture.width * texture.height];
        for (int i = 0; i < clearColor.Length; i++)
            clearColor[i] = Color.white; // or transparent if you prefer

        texture.SetPixels(clearColor);
        texture.Apply();
    }


    public void TicketWrote()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        fPController.enabled = true;
        fPShooting.enabled = true;

        ClearCanvas();

        Destroy(parkedCar);

        ticketPanel.SetActive(false);
        
        crimeCompletion.CrimeStopped(crimeCompletion.tierOneXP, crimeCompletion.tierOneCredits);
    }
}
