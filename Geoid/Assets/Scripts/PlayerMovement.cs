using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerMovement : MonoBehaviour
{
    
    public Rigidbody playerRb;
    public float rotationSpeed = 5f; // rotation speed of orientation to planet

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();//reference player rigidbody
    }

    Vector3 GetStrongestAttractorPosition()
    {
        if (Attractor.Attractors == null || Attractor.Attractors.Count == 0)
            return Vector3.zero;

        Attractor strongestAttractor = Attractor.Attractors[0];//make array
        float maxForce = 0f;
        const float G = 667.4f;

        foreach (Attractor attractor in Attractor.Attractors)//loop through attractors and find one with strongest pull
        {
            Vector3 direction = attractor.rb.position - playerRb.position;
            float distance = direction.magnitude;

            if (distance > 0f)
            {
                float forceMagnitude = G * (attractor.rb.mass * playerRb.mass) / Mathf.Pow(distance, 2);
                
                if (forceMagnitude > maxForce)
                {
                    maxForce = forceMagnitude;
                    strongestAttractor = attractor;
                }
            }
        }

        return strongestAttractor.rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            playerRb.AddForce(transform.forward * 10f);
        }
        if (Keyboard.current.sKey.isPressed)
        {
            playerRb.AddForce(-transform.forward * 10f);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            playerRb.AddForce(-transform.right * 10f);
        }
        if (Keyboard.current.dKey.isPressed)
        {
            playerRb.AddForce(transform.right * 10f);
        }

        //rotate player to face the planet with strongest gravity pull

        Vector3 planetPosition = GetStrongestAttractorPosition();
        

        Vector3 directionToPlanet = (planetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, directionToPlanet) * transform.rotation;
        playerRb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed));
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
