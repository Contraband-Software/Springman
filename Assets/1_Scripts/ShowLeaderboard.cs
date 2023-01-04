using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLeaderboard : MonoBehaviour
{
    public void DoShowLeaderboard()
    {
        PlatformIntegrations.IntegrationsManager.Instance.socialManager.OpenLeaderBoardUI();
    }
}
