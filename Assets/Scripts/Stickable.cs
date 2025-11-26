using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class Stickable : MonoBehaviour
{
    [Header("Sticking")]
    [Tooltip("How close another Stickable must be (meters) when you release to merge.")]
    public float stickRadius = 0.06f;

    [Tooltip("If true, won't stick to objects currently being held.")]
    public bool ignoreHeldTargets = true;

    private static readonly Collider[] _hits = new Collider[128];

    private XRGrabInteractable _grab;

    private void Awake()
    {
        _grab = GetComponent<XRGrabInteractable>();
        if (_grab != null)
        {
            _grab.selectEntered.AddListener(OnSelectEntered);
            _grab.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDestroy()
    {
        if (_grab != null)
        {
            _grab.selectEntered.RemoveListener(OnSelectEntered);
            _grab.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // If we're already part of a cluster, redirect the grab to the cluster root
        var cluster = GetComponentInParent<ClusterRoot>();
        if (cluster != null && cluster.RootGrab != null && cluster.RootGrab != _grab)
        {
            var manager = _grab.interactionManager;
            if (manager != null)
            {
                manager.SelectExit(args.interactorObject, _grab);
                manager.SelectEnter(args.interactorObject, cluster.RootGrab);
            }
        }
    }

    private void OnSelectExited(SelectExitEventArgs args) => TryStick();

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

            // Ignore our own colliders
            if (col.transform.IsChildOf(transform)) continue;

            // Only stick to objects that have Stickable on them
            var other = col.GetComponentInParent<Stickable>();
            if (other == null || other == this) continue;

            var otherCluster = other.GetComponentInParent<ClusterRoot>();
            if (myCluster != null && otherCluster != null && myCluster == otherCluster) continue;

            if (ignoreHeldTargets && other._grab != null && other._grab.isSelected) continue;

            float sq = (other.transform.position - transform.position).sqrMagnitude;
            if (sq < bestSq)
            {
                bestSq = sq;
                best = other;
            }
        }

        if (best != null)
            ClusterRoot.Merge(this, best);
    }
}
