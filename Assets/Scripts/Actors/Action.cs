using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    static public void MoveOrHit(Actor actor, Vector2 direction)
    {
        // see if someone is at the target position
        Actor target = GameManager.Get.GetActorAtLocation(actor.transform.position + (Vector3)direction);

        // if not, we can move
        if (target == null)
        {
            Move(actor, direction);
        }
        else
        {
            Hit(actor, target);
        }

        // end turn in case this is the player
        EndTurn(actor);
    }
    static public void Move(Actor actor, Vector2 direction)
    {
        actor.Move(direction);
        actor.UpdateFieldOfView();
    }

    static public void Hit(Actor actor, Actor target)
    {
        int damage = actor.Power - target.Defense;
        string description = $"{actor.name} attacks {target.name}";
        Color color = actor.GetComponent<Player>() ? Color.white : Color.red;

        if (damage > 0)
        {
            UIManager.Get.AddMessage($"{description} for {damage} hit points.", color);
            target.DoDamage(damage);
        }
        else
        {
            UIManager.Get.AddMessage($"{description} but does no damage.", color);
        }
    }

    static private void EndTurn(Actor actor)
    {
        if (actor.GetComponent<Player>())
        {
            GameManager.Get.StartEnemyTurn();
        }
    }

}