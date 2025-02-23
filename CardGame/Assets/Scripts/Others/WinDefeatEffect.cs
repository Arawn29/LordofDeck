using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;
using System.Collections;

public class WinDefeatEffect : MonoBehaviour
{
    public Volume volume;
    public float minValue = 0.1f;
    public float maxValue = 1.5f;
    public float duration = 2f; // Kaç saniyede geçiþ olacaðýný belirler

    private void Awake()
    {
        volume = GetComponent<Volume>();
    }

    private void Start()
    {
        if (volume.profile.TryGet(out Bloom _bloom))
        {
            StartCoroutine(SmoothEffect(_bloom));
        }
        else
        {
            Debug.LogError("Bloom efekti bulunamadý!");
        }
    }

    IEnumerator SmoothEffect(Bloom bloom)
    {
        float elapsedTime = 0f;
        float startValue = maxValue;

        while (elapsedTime < duration)
        {
            float newValue = Mathf.Lerp(startValue, minValue, elapsedTime / duration);
            bloom.threshold.Override(newValue);
            elapsedTime += Time.deltaTime;
            yield return null; // Her karede devam et
        }

        bloom.threshold.Override(minValue); // Son deðeri kesin olarak ayarla
    }
}