using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CommonEditor : EditorWindow
{
    public virtual void DrawBlockGUI(string lab, SerializedProperty prop)
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(lab, GUILayout.Width(50));
        EditorGUILayout.PropertyField(prop, GUIContent.none);
        EditorGUILayout.EndHorizontal();
    }
}
