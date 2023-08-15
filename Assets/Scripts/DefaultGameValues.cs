using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Static class that holds default game values.
/// </summary>
public static class DefaultGameValues
{
    public const float Gravity = -8;
    public const float Drag = 0.7f;
    public const float PlayerMainThrusterForce = 20;
    public const float PlayerSideThrusterForce = 160;
    public const float DeathThresholdVelocity = 6;
    public const float MusicMaxVolume = 0.8f;
    public const string NormalDifficulty = "Normal";
    public const string HardDifficulty = "Hard";
}

/// <summary>
/// These are used instead of string references, to avoid possible errors.
/// </summary>
public enum GameObjectTags
{
    PauseMenu,
    Player,
    GameManager,
    Safe,
    FollowCamera,
    Finish
}
