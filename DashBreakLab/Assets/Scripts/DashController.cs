using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class DashController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 7f, dashDistance = 13f, dashDuration = .12f, cooldown = .55f;
    [SerializeField] LayerMask dashHitMask;
    [SerializeField] bool useSphereCast = true;
    Rigidbody body; CapsuleCollider capsule; float cooldownRemaining; bool isDashing;
    readonly HashSet<Breakable> brokenThisDash = new();
    public bool IsDashing => isDashing;
    public bool UseSphereCast => useSphereCast;
    public string LastResult { get; private set; } = "Ready — dash through the orange targets.";

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        int breakableLayer = LayerMask.NameToLayer("Breakable");
        if (dashHitMask.value == 0) dashHitMask = breakableLayer >= 0 ? 1 << breakableLayer : 1 << 6;
        body.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }
    Vector3 ReadMove()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current; if (k == null) return Vector3.zero;
        return new Vector3(((k.dKey.isPressed || k.rightArrowKey.isPressed) ? 1 : 0) - ((k.aKey.isPressed || k.leftArrowKey.isPressed) ? 1 : 0), 0, ((k.wKey.isPressed || k.upArrowKey.isPressed) ? 1 : 0) - ((k.sKey.isPressed || k.downArrowKey.isPressed) ? 1 : 0)).normalized;
#else
        return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
#endif
    }
    void Update()
    {
        cooldownRemaining = Mathf.Max(0f, cooldownRemaining - Time.deltaTime);
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current; if (k != null && k.qKey.wasPressedThisFrame) useSphereCast = !useSphereCast; if (k != null && k.spaceKey.wasPressedThisFrame && !isDashing && cooldownRemaining <= 0f) StartCoroutine(Dash());
#else
        if (Input.GetKeyDown(KeyCode.Q)) useSphereCast = !useSphereCast; if (Input.GetKeyDown(KeyCode.Space) && !isDashing && cooldownRemaining <= 0f) StartCoroutine(Dash());
#endif
    }
    void FixedUpdate() { if (isDashing) return; Vector3 input = ReadMove(); body.linearVelocity = input * walkSpeed; if (input.sqrMagnitude > .01f) transform.forward = input; }
    void OnDrawGizmos()
    {
        Vector3 start = transform.position + Vector3.up * .9f; Vector3 end = start + transform.forward * 13f; Gizmos.color = useSphereCast ? Color.cyan : Color.red; Gizmos.DrawLine(start, end);
        if (useSphereCast) { Gizmos.DrawWireSphere(start, .45f); Gizmos.DrawWireSphere(end, .45f); }
    }
    IEnumerator Dash()
    {
        Vector3 direction = ReadMove(); if (direction.sqrMagnitude < .01f) direction = transform.forward; direction.Normalize(); isDashing = true; cooldownRemaining = cooldown; brokenThisDash.Clear(); DashVfxAudio.Dash(transform.position);
        Vector3 origin = transform.position + Vector3.up * capsule.radius; const int allLayers = ~0;
        RaycastHit[] hits = useSphereCast ? Physics.SphereCastAll(origin, capsule.radius * .9f, direction, dashDistance, allLayers, QueryTriggerInteraction.Collide) : Physics.RaycastAll(origin, direction, dashDistance, allLayers, QueryTriggerInteraction.Collide);
        System.Array.Sort(hits, (a,b) => a.distance.CompareTo(b.distance)); float allowedDistance = dashDistance; int broken = 0;
        foreach (var hit in hits) { if (hit.collider.transform.IsChildOf(transform)) continue; var target = hit.collider.GetComponent<Breakable>(); if (target != null) { if (brokenThisDash.Add(target)) { target.Break(); broken++; } continue; } if (!hit.collider.isTrigger) { allowedDistance = Mathf.Min(allowedDistance, Mathf.Max(0, hit.distance - .65f)); break; } }
        LastResult = $"{(useSphereCast ? "SphereCast" : "Raycast")} checked {hits.Length} path hits — broke {broken} target(s).";
        Vector3 start = body.position; float elapsed = 0; while (elapsed < dashDuration) { elapsed += Time.fixedDeltaTime; body.MovePosition(Vector3.Lerp(start, start + direction * allowedDistance, elapsed / dashDuration)); yield return new WaitForFixedUpdate(); } isDashing = false;
    }
}