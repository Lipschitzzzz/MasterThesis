﻿using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WebCamInput))]
public class HandTrackingSample : MonoBehaviour
{
    [SerializeField, FilePopup("*.tflite")]
    private string palmModelFile = "coco_ssd_mobilenet_quant.tflite";
    [SerializeField, FilePopup("*.tflite")]
    private string landmarkModelFile = "coco_ssd_mobilenet_quant.tflite";

    [SerializeField]
    private RawImage cameraView = null;
    // [SerializeField]
    // private RawImage debugPalmView = null;
    [SerializeField]
    private bool runBackground;

    private PalmDetect palmDetect;
    private HandLandmarkDetect landmarkDetect;

    // just cache for GetWorldCorners
    private readonly Vector3[] rtCorners = new Vector3[4];
    private readonly Vector3[] worldJoints = new Vector3[HandLandmarkDetect.JOINT_COUNT];
    private PrimitiveDraw draw;
    private List<PalmDetect.Result> palmResults;
    private HandLandmarkDetect.Result landmarkResult;
    private UniTask<bool> task;
    private CancellationToken cancellationToken;

    private Utilities utilities;

    public bool left = false;
    public bool right = false;
    public bool up = false;
    public bool down = false;

    private IEnumerator HandPoseEstimation()
    {
        for (; ; )
        {
            left = false;
            right = false;

            float[] a = { landmarkResult.joints[5].x, landmarkResult.joints[5].y };
            float[] b = { landmarkResult.joints[6].x, landmarkResult.joints[6].y };
            float[] c = { landmarkResult.joints[7].x, landmarkResult.joints[7].y };
            if (Mathf.Abs(utilities.CalculateAngle3Points(a, b, c) - 180) < 20)
            {
                if (landmarkResult.joints[5].x > landmarkResult.joints[7].x)
                {
                    left = true;
                    yield return new WaitForSeconds(1.0f);
                }

                if (landmarkResult.joints[5].x < landmarkResult.joints[7].x)
                {
                    right = true;
                    yield return new WaitForSeconds(1.0f);
                }

            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void Start()
    {
        palmDetect = new PalmDetect(palmModelFile);
        landmarkDetect = new HandLandmarkDetect(landmarkModelFile);
        // Debug.Log($"landmark dimension: {landmarkDetect.Dim}");

        draw = new PrimitiveDraw();
        utilities = new Utilities();

        var webCamInput = GetComponent<WebCamInput>();
        webCamInput.OnTextureUpdate.AddListener(OnTextureUpdate);
    }

    private void OnDestroy()
    {
        var webCamInput = GetComponent<WebCamInput>();
        webCamInput.OnTextureUpdate.RemoveListener(OnTextureUpdate);

        palmDetect?.Dispose();
        landmarkDetect?.Dispose();
    }

    private IEnumerator CalculateDirection()
    {
        float[] a = { landmarkResult.joints[5].x, landmarkResult.joints[5].y };
        float[] b = { landmarkResult.joints[6].x, landmarkResult.joints[6].y };
        float[] c = { landmarkResult.joints[7].x, landmarkResult.joints[7].y };
        if (Mathf.Abs(utilities.CalculateAngle3Points(a, b, c) - 180) < 20)
        {
            float x = landmarkResult.joints[5].x - landmarkResult.joints[7].x;
            float y = landmarkResult.joints[5].y - landmarkResult.joints[7].y;

            if (x > 0 && Mathf.Abs(x) > Mathf.Abs(y))
            {
                left = true;
            }

            else if (x < 0 && Mathf.Abs(x) > Mathf.Abs(y))
            {
                right = true;
            }
            if (y < 0 && Mathf.Abs(x) < Mathf.Abs(y))
            {
                up = true;
            }
            if (y > 0 && Mathf.Abs(x) < Mathf.Abs(y))
            {
                down = true;
            }

        }

        yield return null;
    }

    private void Update()
    {
        left = false;
        right = false;
        up = false;
        down = false;

        if (palmResults != null && palmResults.Count > 0)
        {
            DrawFrames(palmResults);
        }

        if (landmarkResult != null && landmarkResult.score > 0.5f && palmResults.Count > 0)
        {
            DrawCropMatrix(landmarkDetect.CropMatrix);
            DrawJoints(landmarkResult.joints);

            StartCoroutine(CalculateDirection());

        }

    }

    private void OnTextureUpdate(Texture texture)
    {
        if (runBackground)
        {
            if (task.Status.IsCompleted())
            {
                task = InvokeAsync(texture);
            }
        }
        else
        {
            Invoke(texture);
        }
    }
    private void Invoke(Texture texture)
    {
        palmDetect.Invoke(texture);
        cameraView.material = palmDetect.transformMat;
        cameraView.rectTransform.GetWorldCorners(rtCorners);

        palmResults = palmDetect.GetResults(0.7f, 0.3f);


        if (palmResults.Count <= 0) return;

        // Detect only first palm

        landmarkDetect.Invoke(texture, palmResults[0]);
        // debugPalmView.texture = landmarkDetect.inputTex;

        landmarkResult = landmarkDetect.GetResult();
    }

    private async UniTask<bool> InvokeAsync(Texture texture)
    {
        palmResults = await palmDetect.InvokeAsync(texture, cancellationToken);
        cameraView.material = palmDetect.transformMat;
        cameraView.rectTransform.GetWorldCorners(rtCorners);

        if (palmResults.Count <= 0) return false;

        landmarkResult = await landmarkDetect.InvokeAsync(texture, palmResults[0], cancellationToken);
        // debugPalmView.texture = landmarkDetect.inputTex;

        return true;
    }

    private void DrawFrames(List<PalmDetect.Result> palms)
    {
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        draw.color = Color.green;
        foreach (var palm in palms)
        {
            draw.Rect(MathTF.Lerp(min, max, palm.rect, true), 0.02f, min.z);

            foreach (var kp in palm.keypoints)
            {
                draw.Point(MathTF.Lerp(min, max, (Vector3)kp, true), 0.05f);
            }
        }
        draw.Apply();
    }

    void DrawCropMatrix(in Matrix4x4 matrix)
    {
        draw.color = Color.red;

        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        var mtx = matrix.inverse;
        Vector3 a = MathTF.LerpUnclamped(min, max, mtx.MultiplyPoint3x4(new Vector3(0, 0, 0)));
        Vector3 b = MathTF.LerpUnclamped(min, max, mtx.MultiplyPoint3x4(new Vector3(1, 0, 0)));
        Vector3 c = MathTF.LerpUnclamped(min, max, mtx.MultiplyPoint3x4(new Vector3(1, 1, 0)));
        Vector3 d = MathTF.LerpUnclamped(min, max, mtx.MultiplyPoint3x4(new Vector3(0, 1, 0)));

        draw.Quad(a, b, c, d, 0.02f);
        draw.Apply();
    }

    private void DrawJoints(Vector3[] joints)
    {
        draw.color = Color.blue;

        // Get World Corners
        Vector3 min = rtCorners[0];
        Vector3 max = rtCorners[2];

        Matrix4x4 mtx = Matrix4x4.identity;

        // Get joint locations in the world space
        float zScale = max.x - min.x;
        for (int i = 0; i < HandLandmarkDetect.JOINT_COUNT; i++)
        {
            Vector3 p0 = mtx.MultiplyPoint3x4(joints[i]);
            Vector3 p1 = MathTF.Lerp(min, max, p0);
            p1.z += (p0.z - 0.5f) * zScale;
            worldJoints[i] = p1;
        }

        // Cube
        for (int i = 0; i < HandLandmarkDetect.JOINT_COUNT; i++)
        {
            draw.Cube(worldJoints[i], 0.1f);
        }

        // Connection Lines
        var connections = HandLandmarkDetect.CONNECTIONS;
        for (int i = 0; i < connections.Length; i += 2)
        {
            draw.Line3D(
                worldJoints[connections[i]],
                worldJoints[connections[i + 1]],
                0.05f);
        }

        draw.Apply();
    }

}
