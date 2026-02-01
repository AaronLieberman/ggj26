using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(CheatInspector))]
public class CheatInspectorEditor : Editor
{

    float speedMultiplier = 0;

    private void OnEnable()
    {
        ShopManager shopManager = FindFirstObjectByType<ShopManager>();

        speedMultiplier = shopManager.Speed;

    }
    public override void OnInspectorGUI()
    {
        ShopManager shopManager = FindFirstObjectByType<ShopManager>();

        //DrawDefaultInspector();

        if (GUILayout.Button("Next Customer Amanda Tea"))
        {
            shopManager.DebugCustomerToShow = "AmandaTea";
        }
        if (GUILayout.Button("Next Customer Doug Crown"))
        {
            shopManager.DebugCustomerToShow = "DougCrown";
        }
        if (GUILayout.Button("Next Customer Francis Lyon"))
        {
            shopManager.DebugCustomerToShow = "FrancisLyon";
        }
        if (GUILayout.Button(" ext Customer Orange Steve"))
        {
            shopManager.DebugCustomerToShow = "OrangeSteve";
        }
        if (GUILayout.Button(" ext Customer (Jason) Bane"))
        {
            shopManager.DebugCustomerToShow = "Bane";
        }

        GUILayout.Label("Set Next Customer Timer Speed Multiplier. Currently: " + shopManager.SpeedMultiplier);
        GUILayout.BeginHorizontal();
        speedMultiplier = EditorGUILayout.FloatField("New Speed Multiplier", speedMultiplier);
        if (GUILayout.Button("Set Speed Multiplier"))
        {
            shopManager.SpeedMultiplier = speedMultiplier;
        }
        GUILayout.EndHorizontal();
    }
}
