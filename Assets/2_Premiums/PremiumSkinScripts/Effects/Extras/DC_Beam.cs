using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Beam : MonoBehaviour
{
    public LineRenderer beam1;
    public LineRenderer beam2;
    public LineRenderer beam1_white;
    public LineRenderer beam2_white;
    [HideInInspector] public PremSkinDetailsDemo premDetails;
    public SpriteRenderer circ;
    public SpriteRenderer circ2;
    public SpriteRenderer circExplosion;
    public Animator circAnimator;
    public AudioSource explosion_sound;

    public void FireAtEnemy(Vector2 shootPoint1, Vector2 shootPoint2, Vector2 enemyPos, GameObject target)
    {
        AdjustEnemyEffects(target);

        Vector2 midShootPoint = (shootPoint1 + shootPoint2) / 2;
        Vector2 difference = enemyPos - midShootPoint;
        Vector2 beam1_target = shootPoint1 + difference;
        Vector2 beam2_target = shootPoint2 + difference;

        beam1.enabled = true;
        beam2.enabled = true;
        beam1_white.enabled = true;
        beam2_white.enabled = true;
        circ.enabled = true;
        circ2.enabled = true;

        ChangeColourToSkin();

        beam1.positionCount = 2;
        beam1.SetPosition(0, shootPoint1);
        beam1.SetPosition(1, beam1_target);
        beam1_white.positionCount = 2;
        beam1_white.SetPosition(0, shootPoint1);
        beam1_white.SetPosition(1, beam1_target);

        beam2.positionCount = 2;
        beam2.SetPosition(0, shootPoint2);
        beam2.SetPosition(1, beam2_target);
        beam2_white.positionCount = 2;
        beam2_white.SetPosition(0, shootPoint2);
        beam2_white.SetPosition(1, beam2_target);

        circ.transform.position = enemyPos;
        circ.transform.position = midShootPoint;
        circExplosion.transform.SetParent(null);
        circExplosion.gameObject.transform.position = enemyPos;

        circExplosion.gameObject.GetComponent<SelfDestruct>().DestructionCountdown();
        circAnimator.Play("Explode");
        LeanTween.value(gameObject, UpdateLineOpacity, 1f, 0f, 0.2f);
        LeanTween.value(gameObject, UpdateWhiteLineOpacity, beam1_white.startColor.a, 0f, 0.25f);
        LeanTween.value(gameObject, UpdateEndSize, circ.transform.localScale.x, circ.transform.localScale.x * 0.5f, 0.2f);
        LeanTween.delayedCall(0.15f, CallFadeOutEnds);
    }

    private void ChangeColourToSkin()
    {
        Color lineColour = premDetails.targetColor;
        beam1.startColor = lineColour;
        beam1.endColor = lineColour;
        beam2.startColor = lineColour;
        beam2.endColor = lineColour;

        circ.color = lineColour;
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
        if (target.gameObject.CompareTag("FlyingEnemy"))
        {
            explosion_SFX_FlyingEnemy = target.transform.Find("Sound_Emitter").GetComponent<AudioSource>();
        }
        if (explosion_SFX_FlyingEnemy != null)
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
        Color newCol = beam1.startColor;
        newCol.a = val;
        beam1.startColor = newCol;
        beam1.endColor = newCol;
        beam2.startColor = newCol;
        beam2.endColor = newCol;
    }

    private void UpdateWhiteLineOpacity(float val)
    {
        Color newCol = beam1_white.startColor;
        newCol.a = val;
        beam1_white.startColor = newCol;
        beam1_white.endColor = newCol;
        beam2_white.startColor = newCol;
        beam2_white.endColor = newCol;
    }

    private void UpdateEndSize(float val)
    {
        Vector3 newSize = new Vector3(val, val, val);
        circ.transform.localScale = newSize;
        circ2.transform.localScale = newSize;
        //circ2.transform.localScale = newSize;
    }

    private void UpdateEndOpacity(float val)
    {
        Color newCol = circ.color;
        newCol.a = val;
        circ.color = newCol;
        circ2.color = newCol;
        //circ2.color = newCol;
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
