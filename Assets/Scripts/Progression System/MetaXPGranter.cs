using UnityEngine;

public static class MetaXPGranter
{
    public static int CalculateMetaXP(int waveNumber, int difficultyMultiplier, int traitCount)
    {
        // Basic XP formula, tweak as needed
        return 10 + (waveNumber * difficultyMultiplier) + (traitCount * 5);
    }
}