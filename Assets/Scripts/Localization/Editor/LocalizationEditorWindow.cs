using System.Linq;
using UnityEditor;
using UnityEngine;

public class LocalizationEditorWindow : EditorWindow
{
    private LocalizationDatabase db;
    private Vector2 scroll;

    [MenuItem("Tools/Localization Editor")]
    public static void Open()
    {
        GetWindow<LocalizationEditorWindow>("Localization");
    }

    private void OnGUI()
    {
        DrawHeader();

        if (!db)
        {
            EditorGUILayout.HelpBox("Assign or Create a LocalizationDatabase asset.", MessageType.Info);
            if (GUILayout.Button("Create Database Asset"))
            {
                db = CreateInstance<LocalizationDatabase>();
                var path = EditorUtility.SaveFilePanelInProject("Save Database", "LocalizationDatabase", "asset", "");
                if (!string.IsNullOrEmpty(path))
                {
                    AssetDatabase.CreateAsset(db, path);
                    AssetDatabase.SaveAssets();
                    EditorGUIUtility.PingObject(db);
                }
            }
            return;
        }

        DrawLanguagesToolbar();
        EditorGUILayout.Space(8);

        if (db.languages.Count == 0)
        {
            EditorGUILayout.HelpBox("No languages. Click 'Add Language' to create one.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Ensure Key Consistency (Sync Keys)"))
        {
            Undo.RecordObject(db, "Ensure Key Consistency");
            db.EnsureKeyConsistency();
            foreach (var lang in db.languages) EditorUtility.SetDirty(lang);
        }

        DrawTable();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal();
        db = (LocalizationDatabase)EditorGUILayout.ObjectField("Database", db, typeof(LocalizationDatabase), false);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLanguagesToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        if (GUILayout.Button("Add Language", GUILayout.Width(120)))
        {
            var lang = CreateInstance<Language>();
            lang.languageName = "New Language";
            var path = EditorUtility.SaveFilePanelInProject("Save Language", "Language_New", "asset", "");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(lang, path);
                AssetDatabase.SaveAssets();
                Undo.RecordObject(db, "Add Language");
                db.languages.Add(lang);
                EditorUtility.SetDirty(db);
            }
        }

        GUILayout.FlexibleSpace();
        db.defaultLanguageIndex = EditorGUILayout.IntField("Default Index", db.defaultLanguageIndex, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();


        for (int i = 0; i < db.languages.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            db.languages[i] = (Language)EditorGUILayout.ObjectField(db.languages[i], typeof(Language), false);
            if (db.languages[i])
            {
                db.languages[i].languageName = EditorGUILayout.TextField(db.languages[i].languageName);
            }
            if (GUILayout.Button("X", GUILayout.Width(24)))
            {
                Undo.RecordObject(db, "Remove Language");
                db.languages.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawTable()
    {

        var keySet = new System.Collections.Generic.SortedSet<string>();
        foreach (var lang in db.languages.Where(l => l != null))
            foreach (var k in lang.Keys())
                if (!string.IsNullOrEmpty(k)) keySet.Add(k);

        EditorGUILayout.Space(4);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Keys", EditorStyles.boldLabel, GUILayout.Width(260));

        foreach (var lang in db.languages)
        {
            if (!lang) continue;
            GUILayout.Label(lang.languageName, EditorStyles.boldLabel);
        }
        EditorGUILayout.EndHorizontal();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var key in keySet)
        {
            EditorGUILayout.BeginHorizontal();


            EditorGUI.BeginChangeCheck();
            var newKey = EditorGUILayout.TextField(key, GUILayout.Width(240));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(db, "Rename Key");
                foreach (var lang in db.languages.Where(l => l != null))
                {
                    var e = lang.entries.FirstOrDefault(x => x.key == key);
                    if (e != null) e.key = newKey;
                }
            }

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                Undo.RecordObject(db, "Delete Key");
                foreach (var lang in db.languages.Where(l => l != null))
                    lang.RemoveKey(key);
                EditorGUILayout.EndHorizontal();
                continue;
            }


            foreach (var lang in db.languages)
            {
                if (!lang) { GUILayout.Label("<missing>"); continue; }
                lang.EnsureKey(key);
                var entry = lang.entries.First(e => e.key == key);
                EditorGUI.BeginChangeCheck();
                entry.value = EditorGUILayout.TextField(entry.value);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(lang);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();


        EditorGUILayout.Space(6);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Key", GUILayout.Width(140)))
        {
            var newKey = "NewKey";
            int suffix = 1;
            var all = new System.Collections.Generic.HashSet<string>(keySet);
            while (all.Contains(newKey)) newKey = $"NewKey_{suffix++}";

            Undo.RecordObject(db, "Add Key");
            foreach (var lang in db.languages.Where(l => l != null))
                lang.EnsureKey(newKey);
            foreach (var lang in db.languages.Where(l => l != null))
                EditorUtility.SetDirty(lang);
        }
        EditorGUILayout.EndHorizontal();
    }
}
