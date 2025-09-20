using UnityEngine;

public static class CursorLock
{
    public static bool IsLocked
    {
        get
        {
            return isLocked;
        }
        set
        {
            isLocked = value;

            if (isLocked)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    private static bool isLocked;
}