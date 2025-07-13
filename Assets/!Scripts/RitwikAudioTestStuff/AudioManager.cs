using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private Dictionary<string, AudioSource> sfx = new Dictionary<string, AudioSource>();
    private Dictionary<string, AudioSource> bgm = new Dictionary<string, AudioSource>();
    private Dictionary<string, AudioSource> voice = new Dictionary<string, AudioSource>();

    private string currentBGM = "";

    [System.Serializable]
    public class AudioClipEntry
    {
        public string type;
        public string id;
        public AudioClip clip;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        LoadAudioFromCSV("audio_clips");
    }

    private void LoadAudioFromCSV(string csvFileName)
    {
        TextAsset csv = Resources.Load<TextAsset>(csvFileName);
        if (csv == null)
        {
            Debug.LogError("CSV file not found: " + csvFileName);
            return;
        }

        string[] lines = csv.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // skip header
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] parts = lines[i].Split(',');
            if (parts.Length < 3) continue;

            string type = parts[0].Trim();
            string id = parts[1].Trim();
            string path = parts[2].Trim();

            AudioClip clip = Resources.Load<AudioClip>(path);
            if (clip == null)
            {
                Debug.LogWarning($"Clip not found at path: {path}");
                continue;
            }

            GameObject audioObject = new GameObject($"Audio_{type}_{id}");
            audioObject.transform.parent = this.transform;

            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.clip = clip;

            switch (type.ToUpper())
            {
                case "SFX": sfx[id] = source; break;
                case "BGM": bgm[id] = source; break;
                case "VOICE": voice[id] = source; break;
                default: Debug.LogWarning($"Unknown audio type: {type}"); break;
            }
        }

        Debug.Log("AudioManager: Finished loading audio from CSV.");
    }

    // ---------- PLAYBACK METHODS ----------

    public void PlaySFX(string id)
    {
        if (sfx.TryGetValue(id, out var source))
            source.Play();
        else
            Debug.LogWarning($"SFX '{id}' not found.");
    }

    public void StopSFX(string id)
    {
        if (sfx.TryGetValue(id, out var source))
            source.Stop();
    }

    public void PlayBGM(string id)
    {
        if (currentBGM == id) return;
        StopBGM();

        if (bgm.TryGetValue(id, out var source))
        {
            source.loop = true;
            source.Play();
            currentBGM = id;
        }
        else
            Debug.LogWarning($"BGM '{id}' not found.");
    }

    public void StopBGM()
    {
        foreach (var bgmSource in bgm.Values)
        {
            if (bgmSource.isPlaying)
                bgmSource.Stop();
        }
        currentBGM = "";
    }

    public void PlayVoice(string id)
    {
        StopAllVoice();

        if (voice.TryGetValue(id, out var source))
            source.Play();
        else
            Debug.LogWarning($"Voice '{id}' not found.");
    }

    public void StopVoice(string id)
    {
        if (voice.TryGetValue(id, out var source))
            source.Stop();
    }

    public void StopAllVoice()
    {
        foreach (var voiceSource in voice.Values)
        {
            if (voiceSource.isPlaying)
                voiceSource.Stop();
        }
    }
}
