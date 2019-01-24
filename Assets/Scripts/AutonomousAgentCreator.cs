using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgentCreator : MonoBehaviour
{
    [SerializeField] AutonomousAgent[] m_agents = null;
    int m_agentIndex = 0;


    void Update()
    {
        RaycastHit hit;
        for(int i = 0; i < m_agents.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) m_agentIndex = i;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            for (int i = 0; i < 5; i++)
            {
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    Vector3 position = new Vector3(hit.point.x, hit.point.y + Random.value * 20.0f, hit.point.z);
                    Instantiate(m_agents[m_agentIndex], position, Quaternion.identity);
                }
            }
        }
    }
}
