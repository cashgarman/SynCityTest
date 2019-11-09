using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GOFComputeRunner : MonoBehaviour
{
    public ComputeShader computerShader;
    public RawImage targetImage;
    public TextMeshProUGUI numNodesText;

    private Texture2D currentGen;
    public RenderTexture nextGen;
    private int kernel;

    public int resolutionMultiplier = 1;
    private int seed;
    public float threshold = .5f;
    public bool alwaysSimulate;

    private int width;
    private int height;
    public TextMeshProUGUI FPSText;
    public TextMeshProUGUI resolutionText;
    public float zoomSensivity = 1f;
    public float zoom;

    private void Start()
    {
        // Randomize the world
        seed = Random.Range(0, 1337);

        resolutionMultiplier = Math.Max(1, PlayerPrefs.GetInt("Resolution"));
        resolutionText.text = $"Resolution: x{resolutionMultiplier}";
        
        width = Screen.width * resolutionMultiplier;
        height = Screen.height * resolutionMultiplier;
        Debug.Log($"width: {width} height: {height}");
        Debug.Log($"total cells: {width * height}");

        numNodesText.text = $"Cells: {width * height:n0}";

        // Create the textures that will contain the current generation of the game
        currentGen = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Create the render texture that contain the next generation of the game
        nextGen = new RenderTexture(width, height, 24) {enableRandomWrite = true};
        nextGen.Create();

        // Make sure the current generation of game always gets displayed
        targetImage.texture = currentGen;

        // Setup the compute shader
        kernel = computerShader.FindKernel("Simulate");
        computerShader.SetTexture(kernel, "CurrentGen", currentGen);
        computerShader.SetTexture(kernel, "NextGen", nextGen);
        computerShader.SetFloat("Width", width);
        computerShader.SetFloat("Height", height);
        
        // Randomize the starting condition
        Randomize();
        
        // Use the initial zoom
        transform.localScale = zoom * Vector3.one * zoomSensivity;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket) && PlayerPrefs.GetInt("Resolution") < 8)
        {
            resolutionMultiplier = Mathf.Clamp(PlayerPrefs.GetInt("Resolution") + 1, 1, 8);
            PlayerPrefs.SetInt("Resolution", resolutionMultiplier);
            resolutionText.text = $"Resolution: x{resolutionMultiplier}";
            Debug.Log($"Resolution: {PlayerPrefs.GetInt("Resolution")}");
            Restart();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftBracket) && PlayerPrefs.GetInt("Resolution") > 1)
        {
            resolutionMultiplier = Mathf.Clamp(PlayerPrefs.GetInt("Resolution") - 1, 1, 8);
            PlayerPrefs.SetInt("Resolution", resolutionMultiplier);
            resolutionText.text = $"Resolution: x{resolutionMultiplier}";
            Debug.Log($"Resolution: {PlayerPrefs.GetInt("Resolution")}");
            Restart();
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            zoom = Mathf.Clamp(zoom + Input.GetAxis("Mouse ScrollWheel") * zoomSensivity, 1f, 100f);
            transform.localScale = zoom * Vector3.one * zoomSensivity;
        }
        
        if (alwaysSimulate || Input.GetKey(KeyCode.Space))
        {
            SimulateGeneration();
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void Randomize()
    {
        // Randomize the cells
        var randomizeKernel = computerShader.FindKernel("Randomize");

        // Run the randomize compute shader
        computerShader.SetTexture(randomizeKernel, "NextGen", nextGen);
        computerShader.SetInt("Seed", seed);
        computerShader.SetFloat("Threshold", threshold);
        computerShader.GetKernelThreadGroupSizes(randomizeKernel, out var x, out var y, out var z);
        computerShader.Dispatch(randomizeKernel, Mathf.CeilToInt(width / (float)x), Mathf.CeilToInt(height / (float)y), 1);

        // Make the next generation the new current generation
        Graphics.CopyTexture(nextGen, currentGen);
    }

    private void SimulateGeneration()
    {
        // Simulate the game of life!
        computerShader.GetKernelThreadGroupSizes(kernel, out var x, out var y, out var z);
        computerShader.Dispatch(kernel, Mathf.CeilToInt(width / (float)x), Mathf.CeilToInt(height / (float)y), 1);
        
        // Make the next generation the new current generation
        Graphics.CopyTexture(nextGen, currentGen);

        // Update the FPS display
        FPSText.text = $"FPS: {1f / Time.smoothDeltaTime:F0}";
    }
}