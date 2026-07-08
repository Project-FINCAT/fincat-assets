using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool IsGamePaused { get; private set; }

    public static void SetPause(bool pause)
    {
        IsGamePaused = pause;

        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();

        if (player != null)
        {
            player.canMove = !pause;

            if (pause)
            {
                player.StopPlayer();
            }
        }
    }
}