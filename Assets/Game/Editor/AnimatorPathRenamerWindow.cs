using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class AnimationPathRenamerWindow : EditorWindow
{
    [SerializeField] private AnimationClip[] clips;  // shows as normal array
    [SerializeField] private string oldName = "Visual";
    [SerializeField] private string newName = "CharacterVisual";

    [MenuItem("Tools/Animation Path Renamer")]
    public static void ShowWindow()
    {
        GetWindow<AnimationPathRenamerWindow>("Animation Path Renamer");
    }

    private void OnGUI()
    {
        // Use SerializedObject properly on this window instance
        SerializedObject so = new SerializedObject(this);
        so.Update();

        EditorGUILayout.LabelField("Animation Path Renamer", EditorStyles.boldLabel);
        GUILayout.Space(4);

        EditorGUILayout.PropertyField(so.FindProperty("clips"), new GUIContent("Animation Clips"), true);
        EditorGUILayout.PropertyField(so.FindProperty("oldName"));
        EditorGUILayout.PropertyField(so.FindProperty("newName"));

        GUILayout.Space(10);

        if (GUILayout.Button("Rename Paths in Clips", GUILayout.Height(30)))
        {
            so.ApplyModifiedProperties(); // Apply before renaming
            RenamePathsInClips(clips, oldName, newName);
        }

        so.ApplyModifiedProperties();
    }

    private void RenamePathsInClips(AnimationClip[] clips, string oldName, string newName)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("No AnimationClips assigned.");
            return;
        }

        string pattern = $@"(?<=^|/)({Regex.Escape(oldName)})(?=$|/)";
        var regex = new Regex(pattern);
        int totalRenamed = 0;

        foreach (var clip in clips)
        {
            if (clip == null)
                continue;

            Undo.RegisterCompleteObjectUndo(clip, "Rename Animation Paths");

            var curveBindings = AnimationUtility.GetCurveBindings(clip);
            var objectBindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);

            int renameCount = 0;

            // Regular curves
            foreach (var binding in curveBindings)
            {
                if (regex.IsMatch(binding.path))
                {
                    string newPath = regex.Replace(binding.path, newName);
                    var curve = AnimationUtility.GetEditorCurve(clip, binding);
                    AnimationUtility.SetEditorCurve(clip, binding, null);

                    var newBinding = binding;
                    newBinding.path = newPath;
                    AnimationUtility.SetEditorCurve(clip, newBinding, curve);

                    renameCount++;
                }
            }

            // Object reference curves (sprites, materials, etc.)
            foreach (var binding in objectBindings)
            {
                if (regex.IsMatch(binding.path))
                {
                    string newPath = regex.Replace(binding.path, newName);
                    var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                    AnimationUtility.SetObjectReferenceCurve(clip, binding, null);

                    var newBinding = binding;
                    newBinding.path = newPath;
                    AnimationUtility.SetObjectReferenceCurve(clip, newBinding, keyframes);

                    renameCount++;
                }
            }

            totalRenamed += renameCount;
            Debug.Log($"Renamed '{oldName}' to '{newName}' in {renameCount} bindings of clip '{clip.name}'.");
            EditorUtility.SetDirty(clip);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Finished renaming across {clips.Length} clips. Total bindings changed: {totalRenamed}.");
    }
}



