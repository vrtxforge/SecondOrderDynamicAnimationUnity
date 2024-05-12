using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(ProceduralAnimationController))]
public class ProceduralAnimationControllerEditor : Editor
{
    private const float defaultLength = 2.0f;

    private float f, z, r;
    private SecondOrderDynamics func;
    private Material mat;
    private EvaluationData evalData;

    private void OnEnable()
    {
        var shader = Shader.Find("Hidden/Internal-Colored");
        mat = new Material(shader);
        evalData = new EvaluationData();
        InitFunction();
    }

    private void OnDisable()
    {
        func = null;
        evalData = null;
        f = z = r = float.NaN;
        DestroyImmediate(mat);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UpdateInput();

        Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect);
            GL.PushMatrix();

            GL.Clear(true, false, Color.black);
            mat.SetPass(0);

            float rectWidth = rect.width;
            float rectHeight = rect.height;

            float x_AxisOffset = rectHeight * Mathf.InverseLerp(evalData.Y_min, evalData.Y_max, 0);
            float defaultValueOffset = rectHeight * Mathf.InverseLerp(evalData.Y_min, evalData.Y_max, 1);

            // Draw base graph
            GL.Begin(GL.LINES);
            GL.Color(new Color(1, 1, 1, 1));
            // Draw Y axis
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, rectHeight, 0);
            // Draw X axis
            GL.Vertex3(0, rectHeight - x_AxisOffset, 0);
            GL.Vertex3(rectWidth, rectHeight - x_AxisOffset, 0);
            // Draw default values
            GL.Color(Color.green);
            GL.Vertex3(0, rectHeight - defaultValueOffset, 0);
            GL.Vertex3(rectWidth, rectHeight - defaultValueOffset, 0);
            GL.End();

            // Evaluate function values
            if (evalData.IsEmpty) EvaluateFunction();

            // Re-evaluate function values after input values changed
            if (f != ((ProceduralAnimationController)target).positionConstants.x ||
                z != ((ProceduralAnimationController)target).positionConstants.y ||
                r != ((ProceduralAnimationController)target).positionConstants.z)
            {
                InitFunction();
                EvaluateFunction();
            }

            // Draw graph
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.cyan);
            for (int i = 0; i < evalData.Length; i++)
            {
                Vector2 point = evalData.GetItem(i);
                float x_remap = Mathf.InverseLerp(evalData.X_min, evalData.X_max, point.x) * rectWidth;
                float y_remap = Mathf.InverseLerp(evalData.Y_min, evalData.Y_max, point.y) * rectHeight;
                GL.Vertex3(x_remap, rectHeight - y_remap, 0.0f);
            }
            GL.End();

            GL.PopMatrix();
            GUI.EndClip();

            // Draw values
            EditorGUI.LabelField(new Rect(0, rect.y + rect.height - defaultValueOffset - 10, 20, 20), "1");
            EditorGUI.LabelField(new Rect(0, rect.y + rect.height - x_AxisOffset, 20, 20), "0");
        }
    }
    private void UpdateInput()
    {
        var pac = (ProceduralAnimationController)target;
        if (f != pac.positionConstants.x || z != pac.positionConstants.y || r != pac.positionConstants.z)
        {
            f = pac.positionConstants.x;
            z = pac.positionConstants.y;
            r = pac.positionConstants.z;
            InitFunction();
            EditorApplication.QueuePlayerLoopUpdate();
        }
        
    }


    private void InitFunction()
    {
        f = ((ProceduralAnimationController)target).positionConstants.x;
        z = ((ProceduralAnimationController)target).positionConstants.y;
        r = ((ProceduralAnimationController)target).positionConstants.z;

        func = new SecondOrderDynamics(f, z, r, new Vector3(-defaultLength, 0, 0));
    }
    private void EvaluateFunction()
    {
        evalData.Clear();

        for (int i = 0; i < 300; i++)
        {
            float T = 0.016f; // constant deltaTime (60 frames per second)

            float x_input = Mathf.InverseLerp(0, 299, i) * 2 * defaultLength - defaultLength;
            float y_input = x_input > 0 ? 1 : 0;

            Vector3? funcValues = func.Update(T, new Vector4(x_input, y_input, 0, 0), false);

            if (x_input <= 0) continue; // Data is gathered only after the Y value has changed

            evalData.Add(new Vector2(funcValues.Value.x, funcValues.Value.y));
        }
    }
}
