using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] int waveCount = 5;
    [SerializeField] float waveSpeed = 100;
    [SerializeField] int range_WaveX = 50;
    [SerializeField] int range_WaveY = 15;
    public CameraFilterPack_Vision_Blood blood;

    private void Awake()
    {
        blood = gameObject.GetComponent<CameraFilterPack_Vision_Blood>();
    }

    public IEnumerator WaveMotionCoroutine()
    {
        Debug.LogError("Start CameraWaveMotion");

        Vector3 originPos = transform.position;

        float waveX, waveY;
        for (int i = 0; i < waveCount; i++)
        {
            waveX = Random.Range(-range_WaveX, range_WaveX);
            waveY = Random.Range(-range_WaveY, range_WaveY);
            Vector3 wavePos= new Vector3(waveX, waveY, transform.position.z);

            while (true)
            {
                transform.position = Vector3.MoveTowards(transform.position, wavePos, Time.deltaTime * waveSpeed);
                if (transform.position.Equals(wavePos))
                    break;
                yield return null;
            }
            yield return null;
        }

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, Time.deltaTime * waveSpeed);
            if (transform.position.Equals(originPos))
                break;
            yield return null;
        }
        Debug.LogError("End CameraWaveMotion");
    }
}
