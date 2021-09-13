using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Networking
{
    public class RuAnagramsLoader : IAnagramsLoader
    {
        private class RequestResult
        {
            public List<string> result;
        }
        
        private string _uri = "https://anagram.poncy.ru/anagram-decoding.cgi?name=anagram_main&inword={0}&answer_type=3";
        
        public string Error { get; private set; }
        public List<string> Words { get; private set; }

        public IEnumerator LoadWords(string letters)
        {
            Error = null;
            Words = null;
            
            UnityWebRequest webRequest = UnityWebRequest.Get(string.Format(_uri, letters));
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Error = webRequest.error;
            }
            else
            {
                RequestResult data = JsonConvert.DeserializeObject<RequestResult>(webRequest.downloadHandler.text);
                Words = data.result;
            }
        }
    }
}