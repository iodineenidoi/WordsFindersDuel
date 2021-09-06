using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
    public class LocalizationEditor : EditorWindow
    {
        private static LocalizationEditor _editorWindow;

        #region WindowProperties

        private static LocalizationLanguage[] _languages;
        private TranslationsCollection _currentCollection = null;
        private string _findField = "";
        private string _findFieldLower = "";
        private string _newKeyField = "";
        private Vector2 _scrollPosition = Vector2.zero;
        
        #endregion


        [MenuItem("Window/Localization Editor")]
        public static void ShowWindow()
        {
            _languages = Enum.GetValues(typeof(LocalizationLanguage)).Cast<LocalizationLanguage>().ToArray();
            _editorWindow = EditorWindow.GetWindowWithRect<LocalizationEditor>(
                new Rect(0, 0, 600, 400),
                false,
                "Localization Editor",
                true);
            
        }

        private void OnGUI()
        {
            _currentCollection = (TranslationsCollection) EditorGUILayout.ObjectField(
                "Translations collection",
                _currentCollection,
                typeof(TranslationsCollection));

            if (_currentCollection != null)
                DrawCollectionEditor();
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(_currentCollection);
            AssetDatabase.SaveAssets();
        }

        private void DrawCollectionEditor()
        {
            ShowHeader();
            ShowSearchArea();
            ShowAddKeyArea();
            ShowTranslationsCollection();
        }

        private void ShowHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Translations count: {_currentCollection.TranslationsList.Count}");
            if (GUILayout.Button("Save changes"))
            {
                EditorUtility.SetDirty(_currentCollection);
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        private void ShowSearchArea()
        {
            EditorGUILayout.BeginHorizontal();
            _findField = EditorGUILayout.TextField(_findField);
            _findFieldLower = _findField.ToLowerInvariant();
            EditorGUILayout.EndHorizontal();
        }
        
        private void ShowAddKeyArea()
        {
            EditorGUILayout.BeginHorizontal();
            _newKeyField = EditorGUILayout.TextField(_newKeyField);
            if (GUILayout.Button("Add key"))
            {
                if (_newKeyField.IsNullOrWhiteSpace()) return;
                if (_newKeyField.Contains(" ")) return;
                if (_currentCollection.TranslationsList.Any(x => x.key == _newKeyField)) return;
                
                KeyTranslations keyTranslations = new KeyTranslations
                {
                    key = _newKeyField,
                    languages = new List<LocalizationLanguage>(),
                    values = new List<string>(),
                };

                for (int i = 0; i < _languages.Length; i++)
                {
                    keyTranslations.languages.Add(_languages[i]);
                    keyTranslations.values.Add(string.Empty);
                }
                
                _currentCollection.TranslationsList.Add(keyTranslations);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowTranslationsCollection()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            for (int i = 0; i < _currentCollection.TranslationsList.Count; i++)
            {
                KeyTranslations keyTranslations = _currentCollection.TranslationsList[i];

                if (!_findField.IsNullOrWhiteSpace() && 
                    !keyTranslations.key.ToLowerInvariant().Contains(_findFieldLower) &&
                    !keyTranslations.values.Any(x => x.ToLowerInvariant().Contains(_findFieldLower)))
                {
                    continue;
                }
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Key:", GUILayout.Width(40));
                keyTranslations.key = EditorGUILayout.TextField(keyTranslations.key);
                if (GUILayout.Button("Remove key"))
                {
                    _currentCollection.TranslationsList.Remove(keyTranslations);
                }
                EditorGUILayout.EndHorizontal();

                for (int langIndex = 0; langIndex < _languages.Length; langIndex++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(_languages[langIndex].ToString(), GUILayout.Width(60));

                    keyTranslations.values[langIndex] = EditorGUILayout.TextArea(
                        keyTranslations.values[langIndex],
                        GUILayout.Width(520));

                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}