using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticle : MonoBehaviour
{
    AudioSource audioSource;
    ParticleSystem ps;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ps = GetComponent<ParticleSystem>();
    }

    protected void setZOrder()
    {
        Vector3 position = transform.position;
        position.z = position.y * 0.01f;
        transform.position = position;
    }

    IEnumerator ReturnToPool()
    {
        for(float time = 2f; time>=0;time-=Time.deltaTime)
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
   
        StopAllCoroutines();

        StartCoroutine(ReturnToPool());
        
        audioSource.Play();
    }

    private void Update()
    {
        // Z오더
        setZOrder();

        var main = ps.main;
        main.simulationSpeed = GameManager.Instance.GameSpeed;
    }
}
