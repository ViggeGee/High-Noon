using UnityEngine;

public class Challenge : MonoBehaviour
{
    public enum ChallengeType
    {
        None,

        typeRacer,
        ButtonSmash,
        ShootingGallery,
        Spin,
        
        // LEVEL SPECIFIC CHALLENGES
        LogBalance
    };

    public ChallengeType challengeType;
}
