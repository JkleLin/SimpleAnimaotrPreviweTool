using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[CustomEditor(typeof(Animator))]
public class AniPreviewTool : Editor
{
    Animator animator;

    private void OnEnable() {
        if (animator == null) animator = ((Animator)target).GetComponentInChildren<Animator>();
        EditorApplication.update += Update;
    }

    private void OnDisable() {
        if (animator != null) {
            animator.Rebind();
            EditorApplication.update -= Update;
        }
    }

    private void Update() {
        if (is_preview && animator != null && animator.enabled) 
            animator.Update(Time.deltaTime);
        if (last_preview && !is_preview && animator != null) 
            animator.Rebind();
        last_preview = is_preview;
    }

    bool is_preview = false;
    bool last_preview = false;
    Vector2 scroll_rect;
    public override void OnInspectorGUI() {

        base.OnInspectorGUI();

        if (animator == null) return;

        GUILayout.BeginVertical();
        is_preview = GUILayout.Toggle(is_preview, "预览动画");
        if (AnimationMode.InAnimationMode()) {
            is_preview = false;
        }
        if (is_preview) {
            StopAnimationMode();
            scroll_rect = GUILayout.BeginScrollView(scroll_rect);
            foreach (AnimatorControllerParameter parameter in animator.parameters) {
                GUILayout.BeginHorizontal();
                switch (parameter.type) {
                    case AnimatorControllerParameterType.Bool:
                        GUILayout.Label(parameter.name, GUILayout.Width(105));
                        GUILayout.FlexibleSpace();
                        animator.SetBool(parameter.name, GUILayout.Toggle(animator.GetBool(parameter.name), ""));
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter.name, EditorGUILayout.FloatField(parameter.name, animator.GetFloat(parameter.name)));
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter.name, EditorGUILayout.IntField(parameter.name, animator.GetInteger(parameter.name)));
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        GUILayout.Label(parameter.name, GUILayout.Width(105));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Toggle(false, "", "Radio")) animator.SetTrigger(parameter.name);
                        break;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();

    }

    void StopAnimationMode() {
        if (AnimationMode.InAnimationMode())
            AnimationMode.StopAnimationMode();
    }

}
