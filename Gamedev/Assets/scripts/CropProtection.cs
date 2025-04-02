using UnityEngine;

public class CropProtection : MonoBehaviour
{
    public bool isProtected = false;

    public void SetProtected(bool value)
    {
        isProtected = value;
    }

    public bool IsProtected()
    {
        return isProtected;
    }
}
