using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Architecture.Audio;
using Architecture.Managers;
using UnityEngine.SceneManagement;

public class EffectController : MonoBehaviour
{
    //This script is used to interface between an action happening and the correct effect playing as a result
    //A decision must be made whether to activate the standard animation, or the premium variant.

    //this script should have references to the sound database used in PlayerController, as well as references to default effects.
    [Header("Important References")]
    public PlayerController playerCon;
    public GameObject premiumPlayer;

    [Header("Sound Effect DataBase")]
    public List<Sound> death_sounds = new List<Sound>();
    public List<Sound> bounce_sounds = new List<Sound>();

    [Header("Statuses")]
    public bool premiumSkinActive = false;
    private bool deathEffectPlayed = false;

    [Header("Premium Effect Panels")]
    public Bounce_Effects premium_bounce_effects;
    public Death_Effects premium_death_effects;
    public KillFlying_Effects premium_kill_flying_effects;
    public KillSitting_Effects premium_kill_sitting_effects;
         
    void Start()
    {
        //check if the current skin is premium
        if (UserGameData.Instance.currentSkinPremium || SceneManager.GetActiveScene().name == "Trial")
        {
            print("EffecController Start()");
            premiumSkinActive = true;
            premiumPlayer = GameObject.Find("Player/" + (UserGameData.Instance.activePremiumSkinName + "(Clone)"));


            //BOUNCE
            premium_bounce_effects = premiumPlayer.GetComponent<Bounce_Effects>();
            if (premium_bounce_effects != null)
            {
                premium_bounce_effects.playerCon = playerCon;
                premium_bounce_effects.effectCon = this;
            }

            //DEATH
            premium_death_effects = premiumPlayer.GetComponent<Death_Effects>();
            if(premium_death_effects != null)
            {
                premium_death_effects.playerCon = playerCon;
                premium_death_effects.effectCon = this;
            }

            //KILL FLYING
            premium_kill_flying_effects = premiumPlayer.GetComponent<KillFlying_Effects>();
            if(premium_kill_flying_effects != null)
            {
                premium_kill_flying_effects.playerCon = playerCon;
            }

            //KILL SITTING
            premium_kill_sitting_effects = premiumPlayer.GetComponent<KillSitting_Effects>();
            if(premium_kill_sitting_effects != null)
            {
                premium_kill_sitting_effects.playerCon = playerCon;
            }
        }


        deathEffectPlayed = false;
    }

    //All The Possible Effect Calls
    
    //whenever the player bounces
    public void BounceEffect()
    {

        //not a premium skin or the premium does not override the standard bounce effect
        if(!premiumSkinActive || !premium_bounce_effects.override_standard_bounce_effect)
        {
            playerCon.animator.Play(playerCon.bounce_animation);//EFFECT
            playerCon.bounceDust.Play();//EFFECT

            if (Architecture.Managers.UserGameData.Instance.soundsOn)
            {
                playerCon.bounceSound.Play();//EFFECT
            }
        }
        else
        {
            premium_bounce_effects.PlayEffect();
        }
    }

    //death animation that plays whenever the player dies
    public void DeathAllEffect()
    {
        if (!deathEffectPlayed)
        {
            if (premiumSkinActive)
            {
                premium_death_effects.PlayEffect();
            }
            
            deathEffectPlayed = true;
        }
    }

    //checks if a skin will want to set off the kill animation manually instead of on-click
    public bool AllowAutoKillFlyingEnemy()
    {
        //if not a premium skin, auto kill the enemy on click
        if (premiumSkinActive)
        {
            //if the skin doesnt have an animation for the enemy kill, allow auto kill on click
            if (premium_kill_flying_effects.manuallyTriggerFE_DeathAnim)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true;
        }
    }

    public void KillFlyingEnemyEffect(GameObject flyingEnemyObject = null)
    {
        premium_kill_flying_effects.PlayEffect(flyingEnemyObject);
    }

    public void KillSittingEnemyEffect(GameObject sittingEnemy)
    {
        if(premium_kill_sitting_effects != null)
        {
            premium_kill_sitting_effects.PlayEffect(sittingEnemy);
        }
    }
}
