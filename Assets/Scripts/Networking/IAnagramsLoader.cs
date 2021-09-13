using System;
using System.Collections;
using System.Collections.Generic;

namespace Networking
{
    public interface IAnagramsLoader
    {
        public string Error { get; }
        public List<string> Words { get; }
        
        IEnumerator LoadWords(string letters);
    }
}