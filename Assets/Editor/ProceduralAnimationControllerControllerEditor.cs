using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralAnimationController))]
public class ProceduralAnimationControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var pac = target as ProceduralAnimationController;

        pac.target = (Transform)EditorGUILayout.ObjectField("Target", pac.target, typeof(Transform), true);
        Vector3 positionConstants = pac.positionConstants;
        Vector3 rotationConstants = pac.rotationConstants;
        Vector3 scaleConstants = pac.scaleConstants;

        pac.animatePosition = EditorGUILayout.Toggle("Animate Position", pac.animatePosition);

        if (pac.animatePosition)
        {
            EditorGUILayout.LabelField("Position Constants", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("f", GUILayout.Width(10));
            float fValue = Mathf.Max(EditorGUILayout.FloatField(positionConstants.x, GUILayout.Width(EditorGUIUtility.labelWidth * 0.5f)), float.Epsilon);
            positionConstants.x = fValue;


            GUILayout.Label("z", GUILayout.Width(10));
            positionConstants.y = EditorGUILayout.FloatField(positionConstants.y, GUILayout.Width(EditorGUIUtility.labelWidth * 0.5f));


            GUILayout.Label("r", GUILayout.Width(10));
            positionConstants.z = EditorGUILayout.FloatField(positionConstants.z, GUILayout.Width(EditorGUIUtility.labelWidth * 0.5f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);


            pac.positionConstants = positionConstants;
        }

        pac.animateRotation = EditorGUILayout.Toggle("Animate Rotation", pac.animateRotation);

        if (pac.animateRotation)
        {
            EditorGUILayout.LabelField("Rotation Constants", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("f", GUILayout.Width(10));
            rotationConstants.x = EditorGUILayout.FloatField(rotationConstants.x, GUILayout.Width(EditorGUIUtility.labelWidth * 0.5f));

            GUILayout.Label("z", GUILayout.Width(10));
            rotationConstants.y = EditorGUILayout.FloatField(rotationConstants.y, GUILayout.Width(EditorGUIUtility.labelWidth * 0.5f));

            GUILayout.Label("r", GUILayout.Width(10));
            rotationConstants.z = EditorGUILayout.FloatField(rotationConstants.z, GUILayout.Width(EditorGUIUtility.labelWidth * 0.5f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            pac.rotationConstants = rotationConstants;
        }


        pac.animateScale = EditorGUILayout.Toggle("Animate Scale", pac.animateScale);

        if (pac.animateScale)
        {
            EditorGUILayout.LabelField("Scale Constants", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("f", GUILayout.Width(10));
            scaleConstants.x = EditorGUILayout.FloatField(scaleConstants.x, GUILayout.Width(EditorGUIUtility.labelWidth * 0.5f));

            GUILayout.Label("z", GUILayout.Width(10));
            scaleConstants.y = EditorGUILayout.FloatField(scaleConstants.y, GUILayout.Width(EditorGUIUtility.labelWidth * 0.5f));

            GUILayout.Label("r", GUILayout.Width(10));
            scaleConstants.z = EditorGUILayout.FloatField(scaleConstants.z, GUILayout.Width(EditorGUIUtility.labelWidth * 0.5f));
            EditorGUILayout.EndHorizontal();

            pac.scaleConstants = scaleConstants;
        }

        Undo.RecordObject(pac, "Animation Data Changed");
        PrefabUtility.RecordPrefabInstancePropertyModifications(pac);
    }
}