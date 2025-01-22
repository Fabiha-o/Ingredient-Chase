using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game/Player Stats", order = 1)]
public class PlayerStatsSO : ScriptableObject
{
    public int health = 100;
    public float speed = 5.0f;
}
