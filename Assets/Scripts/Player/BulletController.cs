using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] float speed = 100;

    public IEnumerator MoveBullet(Vector3 targetVec)
    {
        gameObject.SetActive(true);
        Vector3 origin_Position = transform.position;

        RotateBullet(targetVec);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetVec, Time.deltaTime * speed);

            if (transform.position.Equals(targetVec))
                break;

            yield return null;
        }

        transform.position = origin_Position;
        transform.localEulerAngles = Vector3.zero;

        gameObject.SetActive(false);
    }

    public void RotateBullet(Vector3 targetVec)
    {
        float angle = GetAngle(transform.position, targetVec) + transform.localEulerAngles.z;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
    }

    float GetAngle(Vector2 start, Vector2 end)
    {
        Vector2 v2 = end - start;
        return Mathf.Atan2(v2.x, v2.y) * Mathf.Rad2Deg;
    }
}
