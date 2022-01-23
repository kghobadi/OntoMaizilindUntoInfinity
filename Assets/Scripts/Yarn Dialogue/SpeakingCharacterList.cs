using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variable/Lists/SpeakingCharList")]
public class SpeakingCharacterList : RuntimeList<SpeakingCharacter>
{
    /// <summary>
    /// Attempts to find a character with the given name. If one is found,
    /// it is placed in result, and this method returns <see
    /// langword="true"/>; otherwise, result will be <see
    /// langword="null"/>, and this method will return <see
    /// langword="false"/>.
    /// </summary>
    /// <remarks>
    /// <paramref name="name"/> is compared in a case-insensitive manner.
    /// </remarks>
    /// <param name="name">The name of the character to find.</param>
    /// <param name="result">On return, the character to find, or <see
    /// langword="null"/>.</param>
    /// <returns><see langword="true"/> if a character with the given name
    /// was found; <see langword="false"/> otherwise.</returns>
    public bool TryGetCharacter(string name, out SpeakingCharacter result) {
        foreach (var item in this) {
            if (name.ToLowerInvariant() == item.CharacterName.ToLowerInvariant()) {
                result = item;
                return true;
            }
        }

        result = null;
        return false;
    }
}