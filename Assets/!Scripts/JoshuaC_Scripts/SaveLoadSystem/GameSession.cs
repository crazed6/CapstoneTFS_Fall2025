using UnityEngine;

public static class GameSession
{
    public static bool IsNewSession = true;
    public static bool IsLoadedGame = false;

    //Active Slot index
    public static int ActiveSaveSlot = -1;
}