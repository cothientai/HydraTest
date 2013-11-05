using UnityEngine;
using UnityEditor;
using System.Collections;

public class AttackWindow : EditorWindow
{
    [MenuItem("Attacks/Attack Making Window")]
    static void Init()
    {
        AttackWindow window = (AttackWindow)EditorWindow.GetWindow(typeof(AttackWindow));
    }

    void OnGUI()
    {
        if (GUILayout.Button("Create new Attack"))
        {
            Attack attack = ScriptableObject.CreateInstance<Attack>();
            AssetDatabase.CreateAsset(attack, "Assets/Attacks/newAttack.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = attack;
        }
    }
}
