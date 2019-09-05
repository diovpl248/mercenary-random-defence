using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpeed : MonoBehaviour
{
    ParticleSystem ps;

    // Start is called before the first frame update
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        var main = ps.main;
        main.simulationSpeed = GameManager.Instance.GameSpeed;

        var position = transform.position;
        position.z = position.y;
        transform.position = position;
    }
}
