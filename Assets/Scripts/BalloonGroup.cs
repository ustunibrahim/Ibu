using System.Collections.Generic;
using UnityEngine;

public class BalloonGroup : MonoBehaviour
{
    private List<MathBalloon> balloons;  // Baloncukları saklamak için liste

    private void Start()
    {
        balloons = new List<MathBalloon>(GetComponentsInChildren<MathBalloon>());
    }

    public void DisableOtherBalloons(MathBalloon clickedBalloon)
    {
        foreach (var balloon in balloons)
        {
            if (balloon != clickedBalloon)
            {
                balloon.gameObject.SetActive(false); // Diğer baloncukları devre dışı bırak
            }
        }
    }
}
