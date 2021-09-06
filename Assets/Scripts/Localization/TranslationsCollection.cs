using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Translations Collection")]
    public class TranslationsCollection : ScriptableObject
    {
        [SerializeField] public List<KeyTranslations> TranslationsList;
    }
}