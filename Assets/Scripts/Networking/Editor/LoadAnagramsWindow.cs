using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Extensions;
using Helpers;
using Localization;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Networking.Editor
{
    public class LoadAnagramsWindow : EditorWindow
    {
        private const int LoadWordsRepeats = 10;
        private static LoadAnagramsWindow _editorWindow;

        #region WindowProperties

        private AnagramsCollection _currentCollection = null;
        private IReadOnlyList<string> _currentAnagrams = null;
        private IReadOnlyList<string> _currentWords = null;
        private HashSet<string> _loadedAnagrams = new HashSet<string>();
        private HashSet<string> _loadedWords = new HashSet<string>();
        private IAnagramsLoader _anagramsLoader = null;

        #endregion

        [MenuItem("Window/Load Anagrams Window")]
        public static void ShowWindow()
        {
            _editorWindow = EditorWindow.GetWindowWithRect<LoadAnagramsWindow>(
                new Rect(0, 0, 400, 190),
                false,
                "Load Anagrams Window",
                true);
        }

        public void OnGUI()
        {
            AnagramsCollection collection = (AnagramsCollection) EditorGUILayout.ObjectField(
                "Anagrams collection",
                _currentCollection,
                typeof(AnagramsCollection));

            if (collection == null)
            {
                return;
            }

            if (collection != _currentCollection)
            {
                _currentCollection = collection;
                _currentAnagrams = collection.Anagrams;
                _currentWords = collection.AllWords;
            }

            EditorGUILayout.LabelField($"Collection anagrams count: {_currentAnagrams.Count}");
            EditorGUILayout.LabelField($"Collection words count: {_currentWords.Count}");
            EditorGUILayout.LabelField($"Loaded anagrams count: {_loadedAnagrams.Count}");
            EditorGUILayout.LabelField($"Loaded words count: {_loadedWords.Count}");

            if (GUILayout.Button("Load new"))
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(LoadNew());
            }

            if (GUILayout.Button("Add loaded"))
            {
                AddLoaded();
            }

            if (GUILayout.Button("Replace with loaded"))
            {
                ReplaceWithLoaded();
            }

            if (GUILayout.Button("Save changes"))
            {
                SaveChanges();
            }
        }

        private IEnumerator LoadNew()
        {
            _loadedAnagrams = new HashSet<string>();
            _loadedWords = new HashSet<string>();

            _anagramsLoader = null;
            switch (_currentCollection.Language)
            {
                case LocalizationLanguage.English:
                    _anagramsLoader = new RuAnagramsLoader();
                    break;
                case LocalizationLanguage.Russian:
                    _anagramsLoader = new RuAnagramsLoader();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            for (int i = 0; i < LoadWordsRepeats; i++)
            {
                string anagram = "";
                do
                {
                    anagram = LettersGenerator.GetUniqueRandomLetters(GameController.LettersToGenerateForAnagrams);
                } 
                while (_currentAnagrams.Contains(anagram) || _loadedAnagrams.Contains(anagram));

                yield return _anagramsLoader.LoadWords(anagram);
                if (_anagramsLoader.Error.IsNullOrWhiteSpace())
                {
                    _loadedAnagrams.Add(anagram);
                    foreach (string word in _anagramsLoader.Words)
                    {
                        _loadedWords.Add(word);
                    }
                    
                    Debug.Log($"New words are loaded. Loaded anagrams count: {_loadedAnagrams.Count}.\n" +
                              $"Total loaded words count: {_loadedWords.Count}");

                    yield return null;
                }
                else
                {
                    Debug.LogError(_anagramsLoader.Error);
                    yield break;
                }
            }
        }

        private void AddLoaded()
        {
            _currentCollection.AddNewAnagramsAndWords(_loadedAnagrams.ToList(), _loadedWords.ToList());
            _currentAnagrams = _currentCollection.Anagrams;
            _currentWords = _currentCollection.AllWords;
        }

        private void ReplaceWithLoaded()
        {
            _currentCollection.ReplaceAnagramsAndAllWord(_loadedAnagrams.ToList(), _loadedWords.ToList());
            _currentAnagrams = _currentCollection.Anagrams;
            _currentWords = _currentCollection.AllWords;
        }

        private new void SaveChanges()
        {
            EditorUtility.SetDirty(_currentCollection);
            AssetDatabase.SaveAssets();
        }
    }
}