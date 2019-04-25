using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ModifyTexture : MonoBehaviour
{
    public Shader shader;
    
    private Texture2D texture;
    private Thread backgroundThread;
    private readonly Color[] buffer = new Color[64*64];
    private float timeSinceStart;
    
    void Start()
    {
        texture = new Texture2D(64, 64, TextureFormat.RGBA32, true);
        GetComponent<RawImage>().texture = texture;
        
        backgroundThread = new Thread(ThreadedLoop);
        backgroundThread.Start();
    }

    public void Update()
    {
        timeSinceStart = Time.realtimeSinceStartup;
        texture.SetPixels(buffer);
        texture.Apply();
    }

    void ThreadedLoop()
    {
        while (true)
        {
            var color = Color.Lerp(Color.red, Color.blue, (1 + Mathf.Sin(timeSinceStart)) / 2f);
        
            for(var x = 0; x < 64; ++x)
            {
                for (var y = 0; y < 64; ++y)
                {
                    buffer[y * 64 + x] = color;
                }    
            }
        }
    }
}
