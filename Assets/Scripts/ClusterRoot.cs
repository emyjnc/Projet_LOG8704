using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ClusterRoot : MonoBehaviour
{
    public XRGrabInteractable RootGrab { get; private set; }

    private readonly List<Stickable> _pieces = new List<Stickable>();

    private void EnsureComponents()
    {
        // Root rigidbody (single body for the whole cluster)
        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // Root grab interactable (we redirect grabs here)
        RootGrab = GetComponent<XRGrabInteractable>();
        if (RootGrab == null) RootGrab = gameObject.AddComponent<XRGrabInteractable>();

        // Good defaults for VR �held object� behavior
        RootGrab.throwOnDetach = false;
        RootGrab.movementType = XRGrabInteractable.MovementType.Kinematic;
    }

    private void AddPiece(Stickable piece)
    {
        if (piece == null) return;
        if (_pieces.Contains(piece)) return;

        _pieces.Add(piece);

        // Parent without moving it (keep world placement exactly)
        piece.transform.SetParent(transform, worldPositionStays: true);

        // Disable the piece Rigidbody so all colliders become a compound collider under root RB
        var prb = piece.GetComponent<Rigidbody>();
        if (prb != null)
        {
            prb.linearVelocity = Vector3.zero;
            prb.angularVelocity = Vector3.zero;
            prb.useGravity = false;
            prb.isKinematic = true;
            prb.detectCollisions = false;
            //prb.enabled = false; // key: makes collider part of the parent's RB compound
        }
    }

    private static ClusterRoot CreateRootAt(Vector3 position)
    {
        var go = new GameObject("ClusterRoot");
        go.transform.position = position;
        go.transform.rotation = Quaternion.identity;

        var root = go.AddComponent<ClusterRoot>();
        root.EnsureComponents();
        return root;
    }

    public static void Merge(Stickable a, Stickable b)
    {
        if (a == null || b == null) return;

        var rootA = a.GetComponentInParent<ClusterRoot>();
        var rootB = b.GetComponentInParent<ClusterRoot>();

        // Case 1: neither is in a cluster
        if (rootA == null && rootB == null)
        {
            var root = CreateRootAt((a.transform.position + b.transform.position) * 0.5f);
            root.AddPiece(a);
            root.AddPiece(b);
            return;
        }

        // Case 2: only one has a cluster
        if (rootA != null && rootB == null)
        {
            rootA.EnsureComponents();
            rootA.AddPiece(b);
            return;
        }

        if (rootA == null && rootB != null)
        {
            rootB.EnsureComponents();
            rootB.AddPiece(a);
            return;
        }

        // Case 3: already same cluster
        if (rootA == rootB) return;

        // Case 4: merge clusters (move smaller into bigger)
        var into = (rootA._pieces.Count >= rootB._pieces.Count) ? rootA : rootB;
        var from = (into == rootA) ? rootB : rootA;

        into.EnsureComponents();

        // Copy list to avoid modifying while iterating
        var fromPieces = new List<Stickable>(from._pieces);
        foreach (var p in fromPieces)
            into.AddPiece(p);

        Object.Destroy(from.gameObject);
    }
}
