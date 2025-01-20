using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helpers
{
    //-----------Always have reference for camera
    private static Camera _camera;
    public static Camera Camera
    {
        get
        {
            if (_camera == null)
                _camera = Camera.main;
            return _camera;
        }
    }

    //----------- WaitForSeconds handling
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait))
            return wait;
        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];

    }

    //-----------Knows if pounter is over UI element
    private static PointerEventData _eventDataCurrentPosition;
    public static List<RaycastResult> _results;
    public static bool IsOverUI()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }

    //-----------Spawns GameObject in worldPos ditectly at position of text for example
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
        return result;
    }

    //----------- Deletes all children of an gameObject
    public static void DeleteChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }
}
