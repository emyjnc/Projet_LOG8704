using System.Collections.Generic;
using UnityEngine;

public class ClusterRoot : MonoBehaviour
{
    [Tooltip("If true, cluster won't fall due to gravity.")]
    public bool disableGravity = true;

    [Tooltip("If true, cluster rigidbody is kinematic (best for 'place and build').")]
    public bool makeKinematic = true;

    private readonly List<Stickable> _pieces = new List<Stickable>();

    private void EnsureRigidbody()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = !disableGravity;
        rb.isKinematic = makeKinematic;
        rb.detectCollisions = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    private void AddPiece(Stickable piece)
    {
        if (piece == null) return;
        if (_pieces.Contains(piece)) return;

        _pieces.Add(piece);

        piece.transform.SetParent(transform, worldPositionStays: true);

        var prb = piece.GetComponent<Rigidbody>();
        if (prb != null)
        {
            prb.linearVelocity = Vector3.zero;
            prb.angularVelocity = Vector3.zero;
            Destroy(prb); // important fix
        }
    }


    private static ClusterRoot CreateRootAt(Vector3 position)
    {
        var go = new GameObject("ClusterRoot");
        go.transform.position = position;
        go.transform.rotation = Quaternion.identity;

        var root = go.AddComponent<ClusterRoot>();
        root.EnsureRigidbody();
        return root;
    }

    public static void Merge(Stickable a, Stickable b)
    {
        if (a == null || b == null) return;

        var rootA = a.GetComponentInParent<ClusterRoot>();
        var rootB = b.GetComponentInParent<ClusterRoot>();

        if (rootA == null && rootB == null)
        {
            var root = CreateRootAt((a.transform.position + b.transform.position) * 0.5f);
            root.AddPiece(a);
            root.AddPiece(b);
            return;
        }

        if (rootA != null && rootB == null)
        {
            rootA.EnsureRigidbody();
            rootA.AddPiece(b);
            return;
        }

        if (rootA == null && rootB != null)
        {
            rootB.EnsureRigidbody();
            rootB.AddPiece(a);
            return;
        }

        if (rootA == rootB) return;

        // Merge smaller cluster into larger
        var into = (rootA._pieces.Count >= rootB._pieces.Count) ? rootA : rootB;
        var from = (into == rootA) ? rootB : rootA;

        into.EnsureRigidbody();

        var fromPieces = new List<Stickable>(from._pieces);
        foreach (var p in fromPieces)
            into.AddPiece(p);

        Destroy(from.gameObject);
    }
}
