using UnityEngine;

public class Challenge : MonoBehaviour
{
    public enum ChallengeType
    {
        None,

        typeRacer,
        ButtonSmash,
        ShootingGallery,
        Spin
    };

    public ChallengeType challengeType;
}
