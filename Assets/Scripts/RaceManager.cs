using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    [System.Serializable]
    public class RouteData
    {
        [Tooltip("Name for this route (for debugging only).")]
        public string routeName;

        [Tooltip("Start point of this route.")]
        public Transform startPoint;

        [Tooltip("Parent object that contains all checkpoints for this route.")]
        public Transform checkpointsRoot;
    }

    [Header("Multi-route setup")]
    [Tooltip("Player that will be teleported to the route start.")]
    public Transform player;

    [Tooltip("All available routes inside this GameScene.")]
    public RouteData[] routes;

    [Header("Race rules")]
    [Tooltip("How many chances the player has when skipping checkpoints.")]
    public int maxChances = 3;

    // Runtime state for current route
    public int totalCheckpoints;   // number of checkpoints in the current route
    public int currentOrder = 0;   // which checkpoint order (0..N-1) the player should hit next
    public int chances;            // remaining chances

    private float timer = 0f;
    private bool raceStarted = false;
    private bool raceFinished = false;

    // All checkpoints for the current route
    private Checkpoint[] checkpoints;

    // Map from checkpoint.index (10,11,12 or 20..27) -> order (0..N-1)
    private Dictionary<int, int> indexToOrder = new Dictionary<int, int>();

    private void Awake()
    {
        // Simple singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {

        // Select route based on what was chosen in the title menu
        if (routes == null || routes.Length == 0)
        {
            Debug.LogError("RaceManager: No routes configured.");
            return;
        }

        int routeIndex = Mathf.Clamp(RouteSelection.SelectedRouteIndex, 0, routes.Length - 1);


        Debug.Log("RaceManager: Using route index = " + routeIndex);

        RouteData route = routes[routeIndex];

        if (route == null || route.startPoint == null || route.checkpointsRoot == null)
        {
            Debug.LogError("RaceManager: Route data not set correctly for index " + routeIndex);
            return;
        }

        // Get checkpoints for this route
        checkpoints = route.checkpointsRoot.GetComponentsInChildren<Checkpoint>();
        if (checkpoints == null || checkpoints.Length == 0)
        {
            Debug.LogError("RaceManager: No Checkpoint components found under route " + route.routeName);
            return;
        }

        // Build index -> order map
        indexToOrder.Clear();
        List<Checkpoint> list = new List<Checkpoint>(checkpoints);
        list.Sort((a, b) => a.index.CompareTo(b.index));

        for (int i = 0; i < list.Count; i++)
        {
            Checkpoint cp = list[i];
            if (!indexToOrder.ContainsKey(cp.index))
            {
                indexToOrder.Add(cp.index, i); // order 0..N-1
            }
            else
            {
                Debug.LogWarning("RaceManager: Duplicate checkpoint index " + cp.index + " in route " + route.routeName);
            }
        }

        totalCheckpoints = list.Count;
        Debug.Log("RaceManager: Using route '" + route.routeName + "' with " + totalCheckpoints + " checkpoints.");

        // Teleport player to the start point
        if (player != null)
        {
            player.position = route.startPoint.position;
            player.rotation = route.startPoint.rotation;
        }
        else
        {
            Debug.LogWarning("RaceManager: Player reference not set.");
        }

        // Reset and start race
        StartRace();
    }

    private void Update()
    {
        if (raceStarted && !raceFinished)
        {
            timer += Time.deltaTime;
        }
    }

    public void StartRace()
    {
        timer = 0f;
        raceStarted = true;
        raceFinished = false;

        currentOrder = 0;
        chances = maxChances;

        Debug.Log("Race started. Total checkpoints: " + totalCheckpoints);
    }

    /// <summary>
    /// Called by a Checkpoint when the player enters its trigger.
    /// index here is your logical index, e.g. 10, 11, 12 or 20..27.
    /// </summary>
    public void OnCheckpoint(int index)
    {
        Debug.Log("Checkpoint hit with index: " + index);

        if (raceFinished || !raceStarted) return;

        if (!indexToOrder.ContainsKey(index))
        {
            Debug.LogWarning("RaceManager: Unknown checkpoint index " + index);
            return;
        }

        int order = indexToOrder[index]; // 0..N-1
        Debug.Log("Converted to order: " + order + ", expected order: " + currentOrder);

        // Hit the correct next checkpoint
        if (order == currentOrder)
        {
            currentOrder++;

            HideCheckpointByIndex(index);

            // Reached the last checkpoint
            if (currentOrder >= totalCheckpoints)
            {
                FinishRace();
            }
        }
        else
        {
            // Player skipped some checkpoints
            int missed = Mathf.Abs(order - currentOrder);
            chances -= missed;

            Debug.Log("Skipped " + missed + " checkpoint(s). Chances left: " + chances);

            if (chances <= 0)
            {
                FailRace();
                return;
            }

            // Force progress to the checkpoint they just hit
            currentOrder = order + 1;
            HideCheckpointByIndex(index);
        }
    }

    /// <summary>
    /// Disable a checkpoint once it has been passed, by its index (10,11,12 or 20..27).
    /// </summary>
    void HideCheckpointByIndex(int index)
    {
        if (checkpoints == null) return;

        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (checkpoints[i] != null && checkpoints[i].index == index)
            {
                checkpoints[i].gameObject.SetActive(false);
                break;
            }
        }
    }

    void FinishRace()
    {
        raceFinished = true;
        raceStarted = false;

        Debug.Log("Finished in " + timer.ToString("F2") + " seconds");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Finish(true, timer);
        }
    }

    void FailRace()
    {
        raceFinished = true;
        raceStarted = false;

        Debug.Log("Failed: no chances left");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Finish(false, timer);
        }
    }
}