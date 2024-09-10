using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private GameData.UserData userData => DataManager.Ins.GameData.user;
    public int GetResource(RESOURCES resource)
    {
        switch (resource)
        {
            case RESOURCES.UNDO:
                return userData.boosterUndo;
            case RESOURCES.ADDSLOT:
                return userData.boosterAdd;
            case RESOURCES.SHUFFLE:
                return userData.boosterShuffle;
            case RESOURCES.MAGNET:
                return userData.boosterMagnet;
            case RESOURCES.GOLD:
                return userData.gold;
            case RESOURCES.HEALTH:
                return userData.health;
        }
        return 0;
    }

    public void AddResource(RESOURCES resource, int addAmount)
    {
        switch (resource)
        {
            case RESOURCES.UNDO:
                userData.boosterUndo += addAmount;
                break;
            case RESOURCES.ADDSLOT:
                userData.boosterAdd += addAmount;
                break;
            case RESOURCES.SHUFFLE:
                userData.boosterShuffle += addAmount;
                break;
            case RESOURCES.MAGNET:
                userData.boosterMagnet += addAmount;
                break;
            case RESOURCES.GOLD:
                userData.gold += addAmount;
                break;
            case RESOURCES.HEALTH:
                userData.health += addAmount;
                break;
        }
        DataManager.Ins.Save();
    }

    private void OnApplicationQuit()
    {
        if (GameManager.Ins.IsState(GameState.InGame) || GameManager.Ins.IsState(GameState.Pause))
        {
            AddResource(RESOURCES.HEALTH, -1);
        }
    }
}
