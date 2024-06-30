using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformManager : MonoBehaviour
{
    private static List<FallingPlatform> platforms = new List<FallingPlatform>();

    public static void RegisterPlatform(FallingPlatform platform)
    {
        if (!platforms.Contains(platform))
        {
            platforms.Add(platform);
        }
    }

    public static void UnregisterPlatform(FallingPlatform platform)
    {
        if (platforms.Contains(platform))
        {
            platforms.Remove(platform);
        }
    }

    public static void ResetAllPlatforms()
    {
        foreach (var platform in platforms)
        {
            platform.ResetPlatform();
        }
    }
}
