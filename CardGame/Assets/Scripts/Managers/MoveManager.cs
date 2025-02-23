using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public static MoveManager instance;
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else Destroy(instance);
    }
    public IEnumerator CardsRotate(GameObject Orijin, List<GameObject> CardsGameObject, float duration, float maxAngle, float radius)
    {

        RectTransform orijin = Orijin.GetComponent<RectTransform>();
        Vector2 orijinVector = new Vector2(orijin.transform.position.x, orijin.transform.position.y);

        float StartAngle = -maxAngle / 2;
        float AngleStep = CardsGameObject.Count <= 1 ? 0 :maxAngle / (CardsGameObject.Count - 1);

        List<IEnumerator> coroutines = new List<IEnumerator>();

        for (int i = 0; i < CardsGameObject.Count; i++)
        {
            float Angle = StartAngle + AngleStep * i;
            float AngleRad = Mathf.Deg2Rad * Angle;
            float x = Mathf.Sin(AngleRad) * radius;
            float y = Mathf.Cos(AngleRad) * radius;
            Vector2 TargetPosition = new Vector2(x, y);

            coroutines.Add(CardrotateAndMove(CardsGameObject[i], TargetPosition + orijinVector, -Angle, duration));
        }

        foreach (IEnumerator coroutine in coroutines)
        {
            StartCoroutine(coroutine);
        }

        // Bütün kartların hareketi tamamlanana kadar bekle
        foreach (IEnumerator coroutine in coroutines)
        {
            yield return StartCoroutine(coroutine);
        }

    }

    private IEnumerator CardrotateAndMove(GameObject card, Vector2 targetPosition, float targetRotationZ, float duration)
    {

        Vector2 initialPosition = card.transform.position;
        Quaternion initialRotation = card.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, targetRotationZ));

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            card.transform.position = Vector2.Lerp(initialPosition, targetPosition, t);
            card.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);

            yield return null;
        }

        card.transform.position = targetPosition;
        card.transform.rotation = targetRotation;

    }
        
    public IEnumerator CardMovingTo(GameObject Target, Vector3 FirstPosition, Vector3 lastposition, float duration)
    {

        Vector3 StartPosition = FirstPosition;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            Target.transform.position = Vector3.Lerp(StartPosition, lastposition, t);

            yield return null;

            if (Vector3.Distance(StartPosition, lastposition) < 0.1f)
            {
                break;
            }

        }
        Target.transform.position = lastposition;


    }

    public IEnumerator CardNetworkMove(NetworkObject Mover, Vector3 TargetPosition, float duration,float scalePosition)
    {
        float elapsedTimeMove = 0f;
        Vector3 FirstPosition = Mover.transform.position;
        Vector3 finalTargetPos= (TargetPosition - Mover.transform.position) *scalePosition;
        while (elapsedTimeMove < duration)
        {
            elapsedTimeMove += Time.deltaTime;
            float time = elapsedTimeMove / duration;

            Mover.transform.position = Vector3.Lerp(Mover.transform.position,FirstPosition +finalTargetPos, time);
            yield return null;  
            if (Vector3.Distance(Mover.transform.position, TargetPosition) < 0.1f)
            {
                Mover.transform.position = TargetPosition;
                break;
            }

        }
    }

}