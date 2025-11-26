using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Stickable : MonoBehaviour
{
    [Header("Stick Search")]
    [Tooltip("How close another Stickable must be to merge (meters).")]
    public float stickRadius = 0.06f;

    [Tooltip("If true, don't stick to targets that are still moving.")]
    public bool ignoreMovingTargets = true;

    [Header("Movement -> Stick Trigger")]
    [Tooltip("Seconds the object must be 'not moving' before trying to stick.")]
    public float settleTime = 0.12f;

    [Tooltip("Velocity under which the object is considered not moving (m/s).")]
    public float linearSpeedEpsilon = 0.02f;

    [Tooltip("Angular velocity under which the object is considered not moving (rad/s).")]
    public float angularSpeedEpsilon = 0.2f;

    [Header("Debug")]
    public bool debugLogs = false;

    private static readonly Collider[] _hits = new Collider[128];

    private Rigidbody _rb;
    private float _lastTimeMoving;
    private bool _wasMoving;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>(); // optional (but recommended)
        _lastTimeMoving = Time.time;
    }

    private void FixedUpdate()
    {
        // Don�t attempt to stick if we are already part of a cluster (still ok, but avoids spam).
        // If you want cluster pieces to keep merging, remove this early return.
        // (They'll still merge if you move the root; see ClusterRoot section.)
        if (GetComponentInParent<ClusterRoot>() != null && transform.parent.GetComponent<ClusterRoot>() != null)
        {
            // still allow merging with other clusters when the root stops moving:
            // pieces won't run this if they�re under a root and not moving independently
        }

        bool moving = IsMoving();

        if (moving)
        {
            _lastTimeMoving = Time.time;
        }
        else
        {
            // Transition: moving -> not moving, and has been settled long enough
            if (_wasMoving && (Time.time - _lastTimeMoving) >= settleTime)
            {
                TryStick();
            }
        }

        _wasMoving = moving;
    }

    private bool IsMoving()
    {
        if (_rb == null) return false;

        // If kinematic, physics velocities aren’t reliable (often zero). Treat as not moving.
        // (If you move kinematic rigidbodies via transform and still want sticking, add transform-delta logic.)
        if (_rb.isKinematic) return false;

        if (_rb.linearVelocity.sqrMagnitude > linearSpeedEpsilon * linearSpeedEpsilon) return true;
        if (_rb.angularVelocity.sqrMagnitude > angularSpeedEpsilon * angularSpeedEpsilon) return true;

        // If it's not sleeping yet, consider it still moving/settling
        if (!_rb.IsSleeping()) return true;

        return false;
    }


    public void TryStick()
    {
        var myCluster = GetComponentInParent<ClusterRoot>();

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            stickRadius,
            _hits,
            Physics.AllLayers,
            QueryTriggerInteraction.Ignore
        );

        Stickable best = null;
        float bestSq = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            var col = _hits[i];
            if (col == null) continue;

            // Ignore our own colliders/children
            if (col.transform.IsChildOf(transform)) continue;

            var other = col.GetComponentInParent<Stickable>();
            if (other == null || other == this) continue;

            var otherCluster = other.GetComponentInParent<ClusterRoot>();
            if (myCluster != null && otherCluster != null && myCluster == otherCluster) continue;

            if (ignoreMovingTargets && other.IsMoving()) continue;

            float sq = (other.transform.position - transform.position).sqrMagnitude;
            if (sq < bestSq)
            {
                bestSq = sq;
                best = other;
            }
        }

        if (best == null) return;

        if (debugLogs)
            Debug.Log($"[Stickable] Merging {name} with {best.name}", this);

        ClusterRoot.Merge(this, best);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, stickRadius);
    }
}
