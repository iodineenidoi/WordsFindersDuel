using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Extensions
{
    public static class CollectionExtensions
    {
        public static T GetRandom<T>(this List<T> source)
        {
            return source[Random.Range(0, source.Count)];
        }
    }
}