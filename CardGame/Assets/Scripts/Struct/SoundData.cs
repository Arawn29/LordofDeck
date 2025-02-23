using UnityEngine;

public enum ClipType
{
    GiveEffect,
    TakeEffect,
    Move

}

[System.Serializable]
public class SoundData
{
    public AudioClip AudioClip;
    public ClipType ClipType;

}
