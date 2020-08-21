using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgent : MonoBehaviour
{
    public enum eAgentTag
    {
        NONE,
        Agent01,
        Agent02,
        Agent03,
    }
    
    [SerializeField] AutonomousBehavior[] m_behaviors = null;
    [SerializeField] [Range(0.0f, 20.0f)] float m_maxSpeed = 0.0f;
    [SerializeField] [Range(0.0f, 50.0f)] float m_maxForce = 0.0f;
    [SerializeField] [Range(0.0f, 20.0f)] float m_rotateRate = 1.0f;

    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    public Vector3 acceleration { get; set; }
    public Vector3 velocity { get; set; }
    public Vector3 forward { get { return velocity; } }
    public float maxSpeed { get { return m_maxSpeed; } }
    public float maxForce { get { return m_maxForce; } }

    private void Start()
    {
        acceleration = Vector3.zero;
        velocity = Vector3.zero;
    }

    void Update()
    {
        acceleration = Vector3.zero;

        //Apply force to acceleration for all behaviors
        foreach (AutonomousBehavior behavior in m_behaviors)
        {
            GameObject target = (behavior.targetTagName != "NONE") ? GetNearestGameObject(gameObject, behavior.targetTagName, behavior.perception) : null;
            AutonomousAgent targetAgent = (target) ? target.GetComponent<AutonomousAgent>() : null;

            Vector3 force = behavior.ExecuteOrder66(this, targetAgent, behavior.targetTagName);
            force = Vector3.ClampMagnitude(force, maxForce);
            force = force * behavior.strength;
            ApplyForce(force);
        }

        velocity = velocity + acceleration;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        transform.position = transform.position + velocity * Time.deltaTime;
        transform.position = WrapPosition(transform.position, new Vector3(-10.0f, -10.0f, -10.0f), new Vector3(10.0f, 10.0f, 10.0f));

        Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_rotateRate * Time.deltaTime);

        //Debug.DrawLine(transform.position, transform.position + velocity, Color.white);
    }

    void ApplyForce(Vector3 force)
    {
        acceleration = acceleration + force;
    }

    Vector3 WrapPosition(Vector3 position, Vector3 min, Vector3 max)
    {
        Vector3 newPosition = position;

        if (position.x > max.x) newPosition.x = min.x + (position.x - max.x);
        if (position.x < min.x) newPosition.x = max.x + (min.x - position.x);
        if (position.y > max.y) newPosition.y = min.y + (position.y - max.y);
        if (position.y < min.y) newPosition.y = max.y + (min.y - position.y);
        if (position.z > max.z) newPosition.z = min.z + (position.z - max.z);
        if (position.z < min.z) newPosition.z = max.z + (min.z - position.z);

        return newPosition;
    }

    public static GameObject GetNearestGameObject(GameObject sourceGameObject, string tag, float maxDistance = float.MaxValue)
    {
        GameObject nearest = null;
        GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag(tag);
        if (gameObjects.Length > 0)
        {
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (gameObjects[i] != sourceGameObject)
                {
                    float distance = (sourceGameObject.transform.position - gameObjects[i].transform.position).magnitude;
                    if (distance < nearestDistance)
                    {
                        nearest = gameObjects[i];
                        nearestDistance = distance;
                    }
                }
            }
            if (nearestDistance > maxDistance) nearest = null;
        }
        return nearest;
    }

    public static GameObject[] GetGameObjects(GameObject sourceGameObject, string tag, float maxDistance = float.MaxValue)
    {
        List<GameObject> returnGameObjects = new List<GameObject>();
        GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i] != sourceGameObject)
            {
                float distance = (sourceGameObject.transform.position - gameObjects[i].transform.position).magnitude;
                if (distance <= maxDistance)
                {
                    returnGameObjects.Add(gameObjects[i]);
                }
            }
        }
        return returnGameObjects.ToArray();
    }
}
