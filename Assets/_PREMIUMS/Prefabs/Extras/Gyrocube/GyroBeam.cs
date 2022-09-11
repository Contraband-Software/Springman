using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroBeam : MonoBehaviour
{
    public LineRenderer lineRen;
    public LineRenderer whiteLine;
    [HideInInspector] public PremSkinDetailsDemo premDetails;
    public SpriteRenderer circ1;
    public SpriteRenderer circ2;
    public SpriteRenderer circExplosion;
    public Animator circAnimator;
    public AudioSource explosion_sound;

    public void FireAtEnemy(Vector3 playerPos, Vector3 enemyPos, GameObject target)
    {
        AdjustEnemyEffects(target);

        lineRen.enabled = true;
        circ1.enabled = true;
        circ2.enabled = true;
        whiteLine.enabled = true;

        ChangeColourToSkin();

        lineRen.positionCount = 2;
        lineRen.SetPosition(0, playerPos);
        lineRen.SetPosition(1, enemyPos);
        whiteLine.positionCount = 2;
        whiteLine.SetPosition(0, playerPos);
        whiteLine.SetPosition(1, enemyPos);

        circ1.transform.position = playerPos;
        circ2.transform.position = enemyPos;
        circExplosion.transform.SetParent(null);
        circExplosion.gameObject.transform.position = enemyPos;

        circExplosion.gameObject.GetComponent<SelfDestruct>().DestructionCountdown();
        circAnimator.Play("Explode");
        LeanTween.value(gameObject, UpdateLineOpacity, 1f, 0f, 0.2f);
        LeanTween.value(gameObject, UpdateWhiteLineOpacity, whiteLine.startColor.a, 0f, 0.25f);
        LeanTween.value(gameObject, UpdateEndSize, circ1.transform.localScale.x, circ1.transform.localScale.x * 0.5f, 0.2f);
        LeanTween.delayedCall(0.15f, CallFadeOutEnds);
    }

    private void ChangeColourToSkin()
    {
        Color lineColour = premDetails.targetColor;
        lineRen.startColor = lineColour;
        lineRen.endColor = lineColour;

        circ1.color = lineColour;
        circ2.color = lineColour;

        circExplosion.color = premDetails.targetColor;
    }

    private void AdjustEnemyEffects(GameObject target)
    {
        //Replace Explosion sound on FE
        AudioSource explosion_SFX_FlyingEnemy = null;
        if (target.gameObject.CompareTag("SittingEnemy"))
        {
            explosion_SFX_FlyingEnemy = target.transform.Find("Sound_Emitter (1)").GetComponent<AudioSource>();
        }
        if(target.gameObject.CompareTag("FlyingEnemy"))
        {
            explosion_SFX_FlyingEnemy = target.transform.Find("Sound_Emitter").GetComponent<AudioSource>();
        }
        if(explosion_SFX_FlyingEnemy != null)
        {
            explosion_SFX_FlyingEnemy.clip = explosion_sound.clip;
            explosion_SFX_FlyingEnemy.volume = explosion_sound.volume;
        }
        

        //Change PFX on enemy so that its invisible (change colour to transparent)
        ParticleSystem pSys_FE_Explosion = target.transform.Find("Explosion").GetComponent<ParticleSystem>();
        var main = pSys_FE_Explosion.main;

        Color oldEffectColour = main.startColor.color;
        Color newEffectColour = new Color(oldEffectColour.r, oldEffectColour.g, oldEffectColour.b, 0f);
        main.startColor = new ParticleSystem.MinMaxGradient(newEffectColour, newEffectColour);
    }

    private void CallFadeOutEnds()
    {
        LeanTween.value(gameObject, UpdateEndOpacity, 1f, 0f, 0.1f).setOnComplete(SelfDestruct);
    }

    private void UpdateLineOpacity(float val)
    {
        Color newCol = lineRen.startColor;
        newCol.a = val;
        lineRen.startColor = newCol;
        lineRen.endColor = newCol;
    }

    private void UpdateWhiteLineOpacity(float val)
    {
        Color newCol = whiteLine.startColor;
        newCol.a = val;
        whiteLine.startColor = newCol;
        whiteLine.endColor = newCol;
    }

    private void UpdateEndSize(float val)
    {
        Vector3 newSize = new Vector3(val, val, val);
        circ1.transform.localScale = newSize;
        circ2.transform.localScale = newSize;
    }

    private void UpdateEndOpacity(float val)
    {
        Color newCol = circ1.color;
        newCol.a = val;
        circ1.color = newCol;
        circ2.color = newCol;
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
