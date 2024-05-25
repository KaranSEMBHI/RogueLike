using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    static public void Move(Actor actor, Vector2 direction)
    {
        // see if someone is at the target position 

        Actor target = GameManager.Get.GetActorAtLocation(actor.transform.position

                       + (Vector3)direction);



        // if not, we can move 

        if (target == null)

        {

            actor.Move(direction);

            actor.UpdateFieldOfView();

        }



        // end turn in case this is the player 

        EndTurn(actor);

    }
    static private void EndTurn(Actor actor)
    {
        // Controleer of de actor een spelercomponent is
        Player playerComponent = actor.GetComponent<Player>();
        if (playerComponent != null)
        {
            // Voer de functie StartEnemyTurn van de GameManager uit
            GameManager.Get.StartEnemyTurn();
        }
    }

}
