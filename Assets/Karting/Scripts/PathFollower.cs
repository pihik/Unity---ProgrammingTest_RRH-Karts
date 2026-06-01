using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartGame.KartSystems
{
    public class PathFollower : MonoBehaviour
    {
        [SerializeField] float speed = 10f;
        [SerializeField] float turnSpeed = 5f;
        [SerializeField] float obstacleDetectionRange = 5f;

        Rigidbody rb;
        public bool canMove;
        bool isAvoidingObstacle;
        bool blockedPath;
        Vector3 avoidanceDirection;

        [SerializeField] RoadPart target;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Start()
        {
            target = PathGenerator.instance.roadParts[0];
            if (target == null)
            {
                Debug.LogError("Target is null. Solution: Put RoadParts to PathGenerator and generate path");
            }
        }

        void FixedUpdate()
        {
            if (canMove)
            {
                if (isAvoidingObstacle)
                {
                    AvoidObstacle();
                }
                else
                {
                    MoveAlongPath();
                }
            }
        }

        void MoveAlongPath(bool goingBackToOppositeDirection = false)
        {
            Vector3 direction = (target.MyWaypoint - transform.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, lookRotation, turnSpeed * Time.deltaTime);

            float currentSpeed = (blockedPath) ? speed / 2 : speed; // if path is blocked reduce speed by half
            rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);
        }

        void AvoidObstacle()
        {
            blockedPath = false;
            Vector3 leftDirection = -transform.right;
            Vector3 rightDirection = transform.right;

            Vector3 raycastStartingPos = transform.position + new Vector3(0, PathGenerator.instance.HeightOffset * 2, 0);

            // For only one kart it's ok to use raycast, otherwise I would use probably Right and Left collider or Physics.OverlapSphere
            bool isLeftBlocked = Physics.Raycast(raycastStartingPos, leftDirection + (transform.forward * 2.5f), obstacleDetectionRange, LayerMask.GetMask("Obstacle"));
            bool isRightBlocked = Physics.Raycast(raycastStartingPos, rightDirection + (transform.forward * 2.5f), obstacleDetectionRange, LayerMask.GetMask("Obstacle"));

            Debug.DrawRay(raycastStartingPos, (leftDirection + (transform.forward * 2.5f)) * obstacleDetectionRange, Color.yellow); // Clear after testing
            Debug.DrawRay(raycastStartingPos, (rightDirection + (transform.forward * 2.5f)) * obstacleDetectionRange, Color.yellow); // Clear after testing
            
            // Decide the avoidance direction based on obstacle detection
            if (isLeftBlocked && !isRightBlocked)
            {
                avoidanceDirection = rightDirection;
                Debug.Log("turning right");
            }
            else if (!isLeftBlocked && isRightBlocked)
            {
                avoidanceDirection = leftDirection;
                Debug.Log("turning left");
            }
            else if (!isLeftBlocked && !isRightBlocked) { avoidanceDirection = (Random.Range(0, 2) == 0) ? leftDirection : rightDirection; }
            else
            {
                blockedPath = true;
                avoidanceDirection = (Random.Range(0, 2) == 0) ? leftDirection : rightDirection;
            }

            // If avoidance direction is not zero, turn smoothly towards it
            if (avoidanceDirection != Vector3.zero)
            {
                Turn(avoidanceDirection);
            }
        }

        void Turn(Vector3 direction)
        {
            // For Further code optimalization I can put merge this code with part of MoveAlongPath method
            Vector3 blendedDirection = Vector3.Slerp(transform.forward, direction, Time.deltaTime * turnSpeed).normalized;

            Quaternion avoidanceRotation = Quaternion.LookRotation(blendedDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, avoidanceRotation, turnSpeed * 2 * Time.deltaTime);

            rb.MovePosition(rb.position + blendedDirection * speed * Time.deltaTime);
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Obstacle")))
            {
                isAvoidingObstacle = true;
            }
            else if (other.name.Equals(target.name))
            {
                target = PathGenerator.instance.roadParts[target.NextRoadPart.Index]; // Set next target
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Obstacle")))
            {
                isAvoidingObstacle = false;
            }
        }
    }
}
