using UnityEngine;

public class sWakeAndDust : MonoBehaviour
{
    [Space(10)]
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem wakeSystem;
    [SerializeField] private ParticleSystem dustSystem;

    // Wake is called once per frame, in sPlayer FixedUpdate (is this bad?)
    public void GenerateWakeOrDust(float speed, float groundHeight, float distToGround)
    {
        //print(groundHeight + ", " + distToGround);
        if (speed > 2f && distToGround < 3f)
        {
            if (groundHeight < 0.25f) //Wake
            {
                dustSystem.enableEmission = false;
                wakeSystem.enableEmission = true;
            }
            else //Dust
            {
                dustSystem.enableEmission = true;
                wakeSystem.enableEmission = false;
            }
        }
        else //too slow, or airborne - OFF
        {
            dustSystem.enableEmission = false;
            wakeSystem.enableEmission = false;
        }
    }
}
