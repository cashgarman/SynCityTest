using System.Collections.Generic;
using UnityEngine;

public class SelectiveCamera : MonoBehaviour
{
    private readonly List<GameObject> hiddenObjects = new List<GameObject>();

    public void AddHiddenObject(HideFromCamera hiddenObject)
    {
        // Add the game object to the list of objects to hide from this camera
        hiddenObjects.Add(hiddenObject.gameObject);
    }

    // Just before the camera renders
    private void OnPreRender()
    {
        // Hide all the hidden objects
        foreach (var hiddenObject in hiddenObjects)
        {
            hiddenObject.SetActive(false);
        }
    }

    // Just after the camera renders
    private void OnPostRender()
    {
        // Unhide all the hidden objects
        foreach (var hiddenObject in hiddenObjects)
        {
            hiddenObject.SetActive(true);
        }
    }
}