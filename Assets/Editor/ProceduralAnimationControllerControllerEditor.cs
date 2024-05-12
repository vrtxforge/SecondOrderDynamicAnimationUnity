using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralAnimationController))]
public class ProceduralAnimationControllerEditor : Editor
{
    private const float defaultLength = 2.0f;

    private Vector3 positionConstants;
    private Vector3 rotationConstants;
    private Vector3 scaleConstants;
    private SecondOrderDynamics positionFunc;
    private SecondOrderDynamics rotationFunc;
    private SecondOrderDynamics scaleFunc;
    private Material mat;
    private EvaluationData positionEvalData;
    private EvaluationData rotationEvalData;
    private EvaluationData scaleEvalData;
    private bool constantsChanged = false;

    private void OnEnable()
    {
        var shader = Shader.Find("Hidden/Internal-Colored");
        mat = new Material(shader);
        positionEvalData = new EvaluationData();
        rotationEvalData = new EvaluationData();
        scaleEvalData = new EvaluationData();
        InitFunctions();
    }

    private void OnDisable()
    {
        positionFunc = null;
        rotationFunc = null;
        scaleFunc = null;
        positionEvalData = null;
        rotationEvalData = null;
        scaleEvalData = null;
        DestroyImmediate(mat);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UpdateInput();

        if (constantsChanged)
        {
            InitFunctions();
            EvaluateFunctions();
            Repaint(); // Force repaint to update the graph
            constantsChanged = false;
        }

        Rect rect = GUILayoutUtility.GetRect(10, 1000, 200, 200);
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(rect);
            GL.PushMatrix();

            GL.Clear(true, false, Color.black);
            mat.SetPass(0);

            float rectWidth = rect.width;
            float rectHeight = rect.height;

            float x_AxisOffset = rectHeight * Mathf.InverseLerp(positionEvalData.Y_min, positionEvalData.Y_max, 0);
            float defaultValueOffset = rectHeight * Mathf.InverseLerp(positionEvalData.Y_min, positionEvalData.Y_max, 1);

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
            if (positionEvalData.IsEmpty || rotationEvalData.IsEmpty || scaleEvalData.IsEmpty)
                EvaluateFunctions();

            // Draw graphs for position, rotation, and scale
            DrawGraph(positionEvalData, Color.cyan, rectWidth, rectHeight);
            DrawGraph(rotationEvalData, Color.magenta, rectWidth, rectHeight);
            DrawGraph(scaleEvalData, Color.yellow, rectWidth, rectHeight);

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
        if (positionConstants != pac.positionConstants || rotationConstants != pac.rotationConstants || scaleConstants != pac.scaleConstants)
        {
            positionConstants = pac.positionConstants;
            rotationConstants = pac.rotationConstants;
            scaleConstants = pac.scaleConstants;
            constantsChanged = true; // Set flag to indicate constants have changed
        }
    }

    private void InitFunctions()
    {
        var pac = (ProceduralAnimationController)target;
        positionConstants = pac.positionConstants;
        rotationConstants = pac.rotationConstants;
        scaleConstants = pac.scaleConstants;

        positionFunc = new SecondOrderDynamics(positionConstants.x, positionConstants.y, positionConstants.z, new Vector3(-defaultLength, 0, 0));
        rotationFunc = new SecondOrderDynamics(rotationConstants.x, rotationConstants.y, rotationConstants.z, new Vector3(-defaultLength, 0, 0));
        scaleFunc = new SecondOrderDynamics(scaleConstants.x, scaleConstants.y, scaleConstants.z, new Vector3(-defaultLength, 0, 0));
    }

    private void EvaluateFunctions()
    {
        EvaluateFunction(positionFunc, positionEvalData, positionConstants);
        EvaluateFunction(rotationFunc, rotationEvalData, rotationConstants);
        EvaluateFunction(scaleFunc, scaleEvalData, scaleConstants);
    }

    private void EvaluateFunction(SecondOrderDynamics func, EvaluationData evalData, Vector3 constants)
    {
        evalData.Clear();

        for (int i = 0; i < 300; i++)
        {
            float T = 0.016f; // constant deltaTime (60 frames per second)

            float x_input = Mathf.InverseLerp(0, 299, i) * 2 * defaultLength - defaultLength;
            float y_input = x_input > 0 ? 1 : 0;

            Vector3? funcValues = func.Update(T, new Vector3(x_input, y_input, 0), false);

            if (x_input <= 0) continue; // Data is gathered only after the Y value has changed

            evalData.Add(new Vector2(funcValues.Value.x, funcValues.Value.y));
        }
    }

    private void DrawGraph(EvaluationData evalData, Color color, float rectWidth, float rectHeight)
    {
        GL.Begin(GL.LINE_STRIP);
        GL.Color(color);
        for (int i = 0; i < evalData.Length; i++)
        {
            Vector2 point = evalData.GetItem(i);
            float x_remap = Mathf.InverseLerp(evalData.X_min, evalData.X_max, point.x) * rectWidth;
            float y_remap = Mathf.InverseLerp(evalData.Y_min, evalData.Y_max, point.y) * rectHeight;
            GL.Vertex3(x_remap, rectHeight - y_remap, 0.0f);
        }
        GL.End();
    }
}
