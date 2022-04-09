﻿using UnityEngine;

namespace GRENADEable
{
    public class FPSCounter : MonoBehaviour
    {
        float deltaTime = 0.0f;

        void Start()
        {
            DontDestroyOnLoad(this);
            Application.targetFrameRate = 300;
        }

        void Update() => deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = Color.red;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
    }
}