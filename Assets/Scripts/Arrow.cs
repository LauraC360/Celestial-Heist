using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Transform arrow;
    [SerializeField] private List<RacingPoint> checkpoints;
    [SerializeField] private Transform player; 
    private int currentCheckpointIndex = 0; 

    void Update()
    {
        if (checkpoints == null || checkpoints.Count == 0 || arrow == null || player == null)
            return;

        Vector3 targetPosition = checkpoints[currentCheckpointIndex].transform.position;

        Vector3 direction = (targetPosition - player.position).normalized;

        arrow.rotation = Quaternion.LookRotation(direction, player.up);

        if (Vector3.Distance(player.position, targetPosition) < 5f)
        {
            UpdateCheckpoint();
        }
    }

    private void UpdateCheckpoint()
    {
        currentCheckpointIndex++;
        if (currentCheckpointIndex >= checkpoints.Count)
        {
            currentCheckpointIndex = 0; 
        }
    }
}
