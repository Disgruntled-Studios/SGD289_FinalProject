using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/AudioLibrary")]
public class AudioLibrary : ScriptableObject
{
    [SerializeField] private SoundEntry[] _sounds;
    
    [System.Serializable]
    private struct SoundEntry
    {
        public string name;
        public AudioClip clip;
    }

    public AudioClip GetClip(string soundName)
    {
        foreach (var entry in _sounds)
        {
            if (entry.name == soundName)
            {
                return entry.clip;
            }
        }

        return null;
    }
}
