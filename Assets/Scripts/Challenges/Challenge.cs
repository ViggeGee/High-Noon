using UnityEngine;

public class Challenge : MonoBehaviour
{
    public enum ChallengeType
    {
        None,

        typeRacer,
        ButtonSmash,
        ShootingGallery
    };

    public ChallengeType challengeType;
}
