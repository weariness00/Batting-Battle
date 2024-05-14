using UnityEngine;

public class MathUtility
{
    public static int[] GetRandomInt(int length, int min, int max)
    {
        int[] arr = new int[length];
        bool isSame;

        for (int i = 0; i < length; i++)
        {
            while (true)
            {
                arr[i] = Random.Range(min, max);
                isSame = false;
                for (int j = 0; j < i; j++)
                {
                    if (arr[j].Equals(arr[i]))
                    {
                        isSame = true;
                        break;
                    }
                }
                if (!isSame)
                    break;
            }
        }
        return arr;
    }

    public static Vector3 GetRandomVec3(Vector3 range)
    {
        return new Vector3(range.x < 0f ? Random.Range(range.x, 0f) : Random.Range(0f, range.x), range.y < 0f ? Random.Range(range.y, 0f) : Random.Range(0f, range.y), range.z < 0f ? Random.Range(range.z, 0f) : Random.Range(0f, range.z));
    }

    /// <summary>
    /// 0: right, 1: foward, 2: left, 3: back
    /// </summary>
    /// <param name="angle">XZ axis angle</param>
    /// <returns></returns>
    public static int GetXZDirection(float angle)
    {
        if (-45f < angle && angle <= 45f)
            return 0;
        else if (45f < angle && angle <= 135f)
            return 1;
        else if ((135f < angle && angle <= 180f) || (-180f < angle && angle <= -135f))
            return 2;
        else if (-135f < angle && angle <= -45f)
            return 3;

        return -1;
    }

    // 임시 MathUnitylity

    public static int GetHeadHitDirection(float angle)
    {
        if (0f < angle && angle <= 45f)
            return 0;
        else if (45f < angle && angle <= 135f)
            return 1;
        else if (135f < angle && angle <= 180f)
            return 2;
        else if (-180f < angle && angle <= 0f)
            return 3;

        return -1;
    }

    public static Vector3 GetHitDirectionXZ(Vector3 current, Vector3 target)
    {
        Vector3 displacement = target - current;

        return new Vector3(displacement.x, 0f, displacement.z).normalized;
    }

    public static float GetHitYawAngle(Vector3 hitDirection)
    {
        return Mathf.Atan2(hitDirection.z, hitDirection.x) * Mathf.Rad2Deg;
    }

    public static float GetHitYawAngle(Vector3 current, Vector3 target)
    {
        Vector3 hitDirection = GetHitDirectionXZ(current, target);

        return GetHitYawAngle(hitDirection);
    }
    
}
