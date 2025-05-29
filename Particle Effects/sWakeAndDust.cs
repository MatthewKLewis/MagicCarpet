using UnityEngine;

public class sWakeAndDust : MonoBehaviour
{
    [Space(10)]
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem wakeSystem;
    [SerializeField] private ParticleSystem dustSystem;

    // Wake is called once per frame, in sPlayer FixedUpdate (is this bad?)
    public void GenerateWakeOrDust(bool makeWake)
    {
        if (makeWake)
        {
            //print(transform.position.y);
            if (transform.position.y < 1f) //Wake
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
