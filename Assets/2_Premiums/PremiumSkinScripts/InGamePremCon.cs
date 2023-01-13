using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGamePremCon : MonoBehaviour
{
    public GameObject spawnedSkin;
    public int skinIndex;
    public GameObject player;

    [Header("Player Parts To Hide")]
    public SpriteRenderer PlayerTop;
    public SpriteRenderer PlayerBottom;
    public SpriteRenderer PlayerSkin;
    public SpriteRenderer PlayerSpring;
    public SpriteRenderer PlayerEyes;
   
    public void ApplySkin()
    {
        PlayerTop.enabled = false;
        PlayerBottom.enabled = false;
        PlayerSkin.enabled = false;
        PlayerSpring.enabled = false;
        PlayerEyes.enabled = false;

        spawnedSkin.transform.SetParent(player.transform);
        spawnedSkin.transform.localPosition = Vector3.zero;

        spawnedSkin.GetComponent<PremSkinDetailsDemo>().UpdateSkin();
    }
}
