using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefixParticle : MonoBehaviour
{
    Character character;

    public void EnableParticle(Character character)
    {
        this.character = character;
    }

    public void DisableParticle(){
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (character!= null)
        {
            transform.position = character.transform.position;

            if (character.characterInfo.currentHP <= 0)
            {
                DisableParticle();
                character = null;
            }
        }
    }
}
