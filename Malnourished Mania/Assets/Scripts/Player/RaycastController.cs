using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum TrapFacingDirection
{
    up, down, left, right
}

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    #region Variables
    protected LayerMask collisionMask;

    protected const float skinWidth = 0.000005f;
    protected int horizontalRayCount = 4;
    protected int verticalRayCount = 4;
    [HideInInspector]
    protected float horizontalRaySpacing;
    [HideInInspector]
    protected float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D boxCollider;
    [HideInInspector]
    protected RaycastOrigins raycastOrigins;

    [HideInInspector]
    protected TrapFacingDirection trapFacingDirection = TrapFacingDirection.up; //0 = up, 1 = right, 2 = down, 3 = left
    #endregion

    public virtual void Awake() => boxCollider = GetComponent<BoxCollider2D>();

    public virtual void Start()
    {
        CalculateRaySpacing();
        UpdateRayCastOrigins();
    }

    #region Raycast collision methods 
    public void UpdateRayCastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public List<RaycastHit2D> GetAllCollisions(LayerMask hitMask, float rayLength)
    {
        List<RaycastHit2D> hitList = new List<RaycastHit2D>();

        hitList.AddRange(GetCollisionsAbove(hitMask, rayLength));
        hitList.AddRange(GetCollisionsBelow(hitMask, rayLength));
        hitList.AddRange(GetCollisionsToTheLeft(hitMask, rayLength));
        hitList.AddRange(GetCollisionsToTheRight(hitMask, rayLength));

        return hitList.Distinct().ToList();
    }

    public List<RaycastHit2D> GetCollisionsToTheRight(LayerMask hitMask, float rayLength)
    {
        List<RaycastHit2D> hitList = new List<RaycastHit2D>();

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topRight + Vector2.down * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, rayLength, hitMask);

            Debug.DrawRay(rayOrigin, Vector2.right * rayLength, Color.red);

            if (hit && !hitList.Contains(hit))
            {
                hitList.Add(hit);
            }
        }

        return hitList;
    }

    public List<RaycastHit2D> GetCollisionsToTheLeft(LayerMask hitMask, float rayLength)
    {
        List<RaycastHit2D> hitList = new List<RaycastHit2D>();

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.down * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, rayLength, hitMask);

            Debug.DrawRay(rayOrigin, Vector2.left * rayLength, Color.red);

            if (hit && !hitList.Contains(hit))
            {
                hitList.Add(hit);
            }
        }

        return hitList;
    }

    public List<RaycastHit2D> GetCollisionsAbove(LayerMask hitMask, float rayLength)
    {
        List<RaycastHit2D> hitList = new List<RaycastHit2D>();

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, hitMask);

            Debug.DrawRay(rayOrigin, Vector2.up * rayLength, Color.red);

            if (hit && !hitList.Contains(hit))
            {
                hitList.Add(hit);
            }
        }

        return hitList;
    }

    public List<RaycastHit2D> GetCollisionsBelow(LayerMask hitMask, float rayLength)
    {
        List<RaycastHit2D> hitList = new List<RaycastHit2D>();

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft + Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, hitMask);

            Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);

            if (hit && !hitList.Contains(hit))
            {
                hitList.Add(hit);
            }
        }

        return hitList;
    }
#endregion


    public void CalculateStaticTrapDirCase()
    {
        if (Vector3.Dot(transform.up, Vector3.up) > 0.5f)
        {
            trapFacingDirection = TrapFacingDirection.up;
        }
        else if (Vector3.Dot(transform.up, Vector3.right) > 0.5f)
        {
            trapFacingDirection = TrapFacingDirection.right;
        }
        else if (Vector3.Dot(transform.up, Vector3.down) > 0.5f)
        {
            trapFacingDirection = TrapFacingDirection.down;
        }
        else if (Vector3.Dot(transform.up, Vector3.left) > 0.5f)
        {
            trapFacingDirection = TrapFacingDirection.left;
        }
        else
        {
            Debug.LogError("Somehow no dot product found for " + gameObject.name + "! Up transform is " + gameObject.transform.up);
        }
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
