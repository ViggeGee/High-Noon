using UnityEngine;

public class Challenge : MonoBehaviour
{
    public enum ChallengeType
    {
        None,

        typeRacer,
        PointAndClick,
        Sequence,
        Brah,
        Boh,
        Nah
    };

    public ChallengeType challengeType;
}
