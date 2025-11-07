using UnityEngine;

public static class LevelProgress
{
    private const string Prefix = "UNLOCKED_LEVEL_";

    public static void Unlock(string levelId)
    {
        if (string.IsNullOrEmpty(levelId))
            return;

        PlayerPrefs.SetInt(Prefix + levelId, 1);
        PlayerPrefs.Save();
    }

    public static bool IsUnlocked(string levelId)
    {
        if (string.IsNullOrEmpty(levelId))
            return false;

        return PlayerPrefs.GetInt(Prefix + levelId, 0) == 1;
    }
}
