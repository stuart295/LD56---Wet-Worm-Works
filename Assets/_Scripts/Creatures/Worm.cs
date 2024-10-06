using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Worm : Creature
{

    protected const string ANIM_SPEED = "Speed";
    protected const string ANIM_KILL= "Kill";


    public SpringJoint2D attachedJoint;

    public Animator anims;
    protected Vector2 wanderTarget = Vector2.zero;
    protected float lastWanderTime = 0;
    protected float wanderDelay = 0;
    protected bool attachedToObject = false;

    protected override bool FixVelocity => base.FixVelocity && !attachedToObject;

    protected virtual bool MoveTo(Vector3 position)
    {
        if (Vector3.Distance(rb.position, position) <= 0.1f) return true;

        UpdateFacing(position);

        Vector3 newPos = Vector3.MoveTowards(rb.position, position, def.moveSpeed*Time.deltaTime);
        rb.MovePosition(newPos);

        anims.SetFloat(ANIM_SPEED, 1f);

        return false;   

    }

    protected override void UpdateAI()
    {
        base.UpdateAI();
        anims.SetFloat(ANIM_SPEED, 0f);

        if (prey != null && (!def.eatsCorpses && prey.Dead))
        {
            OnPreyLost();

        }

        if (prey == null && Nutrients <= def.searchFoodThreshold)
        {
            //Getting hungry, search for food
            prey = GetNearestPrey();
        }

        //Hunt prey
        if (prey != null)
        {
            HuntPrey();
        }
        else if ((Time.time - lastWanderTime) >= wanderDelay)
        {
            if (wanderTarget == Vector2.zero)
            {
                wanderTarget = rb.position + new Vector2(Random.Range(-def.wanderDist, def.wanderDist), Random.Range(-def.wanderDist, def.wanderDist));
                wanderTarget = GameManager.Instance.ConstrainToBounds(wanderTarget, def.livesAtSurface);
            }

            if (MoveTo(wanderTarget))
            {
                wanderTarget = Vector2.zero;
                ResetWanderTime();
            }
        }


    }

    private bool IsWrongDepth(Vector2 pos)
    {
        bool tooDeep = def.livesAtSurface && pos.y <= GameManager.Instance.boundaryHeight;
        bool tooShallow = !def.livesAtSurface && pos.y >= GameManager.Instance.boundaryHeight;
        return tooDeep || tooShallow;
    }

    protected virtual void HuntPrey()
    {
        if (prey == null) return;
        if (IsWrongDepth(prey.transform.position))
        {
            OnPreyLost();
            return;

        }

        //Move to food
        if (MoveTo(prey.transform.position))
        {
            //If there, eat!
            OnReachedPrey();

        }
    }

    protected virtual void OnPreyLost()
    {
        prey = null;
    }

    protected virtual void OnReachedPrey()
    {
        Eat(prey);
        prey = null;
        ResetWanderTime();
    }

    private void ResetWanderTime()
    {
        wanderDelay = Random.Range(def.wanderDelayMin, def.wanderDelayMax);
        lastWanderTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (prey != null && collision.gameObject == prey.gameObject)
        {
            Creature otherCreature = collision.gameObject.GetComponent<Creature>();
            if (otherCreature == null) return;

            if (def.speciesToEat.Contains(otherCreature.def))
            {
                //Eat!
                OnReachedPrey();
            }
 
        }
    }

    private void UpdateFacing(Vector2 position)
    {
        Vector2 lookDir = (position - rb.position).normalized;
        float targetAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        float angleDiff = Mathf.DeltaAngle(rb.rotation, targetAngle);
        rb.rotation = Mathf.MoveTowards(rb.rotation, rb.rotation + angleDiff, Time.deltaTime * def.turnSpeed);
    }

    public override float Kill(bool leaveCorpse)
    {
        float nutrients = base.Kill(leaveCorpse);

        anims.SetTrigger(ANIM_KILL);

        return nutrients;
    }

    protected void DetachFromObject()
    {
        attachedJoint.enabled = false;
        rb.freezeRotation = true;
        attachedToObject = false;
    }

    protected void AttachToObject(GameObject target, bool collide=false)
    {
        attachedToObject = true;

        // Get the collider of the target object
        Collider2D targetCollider = target.GetComponent<Collider2D>();

        if (targetCollider != null)
        {
            // Find the closest point on the target's surface to the leech's position
            Vector2 leechPosition = transform.position;
            Vector2 nearestPoint = targetCollider.ClosestPoint(leechPosition);

            attachedJoint.connectedAnchor = target.transform.InverseTransformPoint(nearestPoint);

            // Debugging to ensure the point is correct
            Debug.DrawLine(leechPosition, nearestPoint, Color.red, 2f);
        }

        attachedJoint.enabled = true;
        attachedJoint.enableCollision = collide;
        attachedJoint.connectedBody = target.GetComponent<Rigidbody2D>();
        rb.freezeRotation = false;
    }



#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Selection.Contains(gameObject)) return;


        if (wanderTarget != Vector2.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(wanderTarget, 0.25f);
        }
    }
#endif
}
