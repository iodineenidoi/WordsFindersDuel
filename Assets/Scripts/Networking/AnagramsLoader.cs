using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Entities;
using Gaming;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Networking
{
    public class AnagramsLoader : MonoBehaviour
    {
        [SerializeField] private string uri = "";
        [SerializeField] private MessageBox messageBox = null;
        [SerializeField] private LobbyMenu lobbyMenu = null;
        [SerializeField] private GameRoot gameRoot = null;

        public bool IsReady { get; private set; }
        public List<string> LoadedWords { get; private set; }
        public string Letters { get; private set; }

        public void LoadAnagrams(string letters)
        {
            Letters = letters;
            StartCoroutine(GetRequest());
        }

        private IEnumerator GetRequest()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(string.Format(uri, Letters)))
            {
                IsReady = false;

                yield return webRequest.SendWebRequest();
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    messageBox.Show(webRequest.error, "Ок", () =>
                    {
                        gameRoot.Stop();
                    });
                    yield break;
                }

                AnagramResponse response = JsonUtility.FromJson<AnagramResponse>(webRequest.downloadHandler.text);
                LoadedWords = response.result.ToList();
                IsReady = true;
            }
        }
    }
}