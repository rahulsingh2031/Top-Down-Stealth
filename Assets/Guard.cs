using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static System.Action OnGuardSpottedPlayer;
    [SerializeField] Transform pathHolder;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float waitTime = 1.5f;
    [SerializeField] private float viewDistance = 2.0f;
    [SerializeField] private Light spotlight;
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask viewMask;
    private Color originalSpotlightColor;
    private float viewAngle;
    bool isPlayerDetected;

    Vector3[] allWaypoints;
    private bool enemySpotted = false;

    public float timeToSpotPlayer = 0.5f;
    public float playerVisibleTimer;

    private void Start()
    {
        viewAngle = spotlight.spotAngle;
        originalSpotlightColor = spotlight.color;

        allWaypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < pathHolder.childCount; i++)
        {
            allWaypoints[i] = pathHolder.GetChild(i).position;
            allWaypoints[i].y = transform.position.y;
        }
        StartCoroutine(FollowPath(allWaypoints));
    }
    private void Update()
    {
        if (CheckPlayerInSight() && !isPlayerDetected)
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }
        playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0f, timeToSpotPlayer);
        spotlight.color = Color.Lerp(originalSpotlightColor, Color.red, playerVisibleTimer / timeToSpotPlayer);
        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            OnGuardSpottedPlayer?.Invoke();
            isPlayerDetected = true;
        }
    }
    bool CheckPlayerInSight()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= viewDistance)
        {
            Vector3 playerDirection = player.transform.position - transform.position;
            float AngleBetweenPlayerAndGuard = Vector3.Angle(transform.forward, playerDirection);
            if (AngleBetweenPlayerAndGuard <= viewAngle / 2)
            {//Case 3: NOT -> hit any obstacles (object having viewMask layer)
                if (!Physics.Linecast(transform.position, player.transform.position, viewMask))
                {
                    print("hello world intruder");
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {

        transform.position = waypoints[0];
        int targetWaypointNumber = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointNumber];
        transform.LookAt(targetWaypoint);

        while (!enemySpotted)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, Time.deltaTime * moveSpeed);
            if (transform.position == targetWaypoint)
            {
                targetWaypointNumber = (targetWaypointNumber + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointNumber];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }


    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {

        Vector3 moveDir = (lookTarget - transform.position).normalized;

        float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {

            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, rotateSpeed * Time.deltaTime);

            yield return null;
        }
    }
    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(waypoint.position, 0.2f);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;

        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }

}
