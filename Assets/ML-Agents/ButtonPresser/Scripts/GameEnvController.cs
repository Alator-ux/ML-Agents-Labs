using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class GameEnvController : MonoBehaviour
{
    public int buttonsOnEpisode = 4;
    public int boxesOnEpisode = 3;

    private AgentPusher agent;
    public GridedDistributor buttonsDistributor;
    public GridedDistributor boxDistributor;
    public GridedDistributor agentsDistributor;
    public Door door;
    public MeshCollider goal;

    void Start()
    {
        ResetScene();
    }

    void ResetScene()
    {
        var buttons = buttonsDistributor.Respawn(buttonsOnEpisode);
        boxDistributor.Respawn(boxesOnEpisode);
        var activators = new DoorActivator[buttons.Length];
        for (var i = 0; i < buttons.Length; i++)
            activators[i] = buttons[i].GetComponent<Button>();
        door.ResetActivators(activators);

        agent = agentsDistributor.Respawn(1)[0].GetComponent<AgentPusher>();
        agent.SetTarget(buttons[0].transform);
        agengt = envController;
    }

    public void OnGoalTriggered()
    {
        agent.EndEpisode();
        ResetScene();
    }
    void FixedUpdate()
    {
    }
}
