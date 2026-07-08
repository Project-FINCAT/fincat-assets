using System.Collections;
using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    public IEnumerator Shake()
    {
        Vector3 original = transform.localPosition;

        for (int i = 0; i < 10; i++)
        {
            transform.localPosition = original + Random.insideUnitSphere * 5f;
            yield return new WaitForSeconds(0.02f);
        }

        transform.localPosition = original;
    }
}