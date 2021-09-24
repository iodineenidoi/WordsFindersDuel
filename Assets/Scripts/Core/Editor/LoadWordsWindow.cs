using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.WordsValidators;
using UnityEditor;
using UnityEngine;

namespace Core.Editor
{
    public class LoadWordsWindow : EditorWindow
    {
        #region WindowProperties

        private WordsCollection _currentCollection = null;
        private string _pathToDir = "";
        private HashSet<string> _wordsFromFiles = new HashSet<string>();
        private static List<IWordsValidator> _wordsValidators = new List<IWordsValidator>();

        #endregion

        [MenuItem("Window/Load Anagrams Window")]
        public static void ShowWindow()
        {
            GetWindowWithRect<LoadWordsWindow>(
                new Rect(0, 0, 400, 190),
                false,
                "Load Anagrams Window",
                true);
            
            _wordsValidators.Clear();
            _wordsValidators.Add(new LettersLengthRangedWordsValidator(2, 14));
            _wordsValidators.Add(new UniqueLettersWordsValidator());
        }

        public void OnGUI()
        {
            _currentCollection = (WordsCollection) EditorGUILayout.ObjectField(
                "Anagrams collection",
                _currentCollection,
                typeof(WordsCollection));

            if (_currentCollection == null)
            {
                return;
            }

            EditorGUILayout.LabelField("Enter path to words:");
            _pathToDir = EditorGUILayout.TextArea(_pathToDir);

            if (GUILayout.Button("Load words"))
            {
                LoadFromDirectory();
            }

            if (GUILayout.Button("Save to collection"))
            {
                SaveToCollection();
            }
        }

        private void LoadFromDirectory()
        {
            if (!Directory.Exists(_pathToDir))
            {
                Debug.LogError($"Directory {_pathToDir} not found!");
                return;
            }
            
            _wordsFromFiles.Clear();

            string[] fileNames = Directory.GetFiles(_pathToDir);
            int index = -1;
            foreach (string fileName in fileNames)
            {
                index++;
                try
                {
                    foreach (string word in File.ReadAllLines(fileName, Encoding.UTF8))
                    {
                        _wordsFromFiles.Add(word);
                        EditorUtility.DisplayProgressBar(
                            "Loading words",
                            $"Loading from file: {fileName}. Word: {word}. Loaded words: {_wordsFromFiles.Count}",
                            (float) index / fileNames.Length);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private void SaveToCollection()
        {
            if (!_wordsFromFiles.Any())
            {
                return;
            }

            List<string> wordsToSave = _wordsFromFiles
                .Where(ValidateWord)
                .Select(word => word.ToUpperInvariant())
                .ToList();
            
            _currentCollection.ReplaceAllWords(wordsToSave);
            EditorUtility.SetDirty(_currentCollection);
            AssetDatabase.SaveAssets();
        }

        private bool ValidateWord(string word)
        {
            foreach (IWordsValidator validator in _wordsValidators)
            {
                if (!validator.ValidateWord(word))
                {
                    return false;
                }
            }

            return true;
        }
    }
}