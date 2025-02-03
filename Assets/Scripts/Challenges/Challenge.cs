using UnityEngine;

public class Challenge : MonoBehaviour
{
    public enum ChallengeType
    {
        ERROR,

        typeRacer,
        PointAndClick,
        Sequence,
        Brah,
        Boh,
        Nah
    };

    public ChallengeType challengeType;
}
