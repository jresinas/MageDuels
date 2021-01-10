using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {
    private MageController mc;
    // Start is called before the first frame update
    void Start() {
        mc = GetComponent<MageController>();
    }

    // Update is called once per frame
    void Update() {
        Move();
        Spells();
    }

    private void Move() {
        //// Obtenemos la dirección del movimiento
        float HorizontalMove = Input.GetAxis("Horizontal_" + mc.playerNumber);
        float VerticalMove = Input.GetAxis("Vertical_" + mc.playerNumber);

        mc.Move(HorizontalMove, VerticalMove);
    }

    /*
     * Controla la ejecución de los hechizos
     */
    private void Spells() {

        if (!mc.Busy()) {
            for (int i = 0; i < mc.spellBook.Length; i++) {
                if (Input.GetButton("Spell" + i + "_" + mc.playerNumber)) {
                    mc.StartCastingSpell(mc.spellBook[i]);
                }
            }
            //    if (Input.GetButton("Spell0_" + playerNumber)) {
            //        animator.SetBool(spellBook[0], true);
            //    }
            //    if (Input.GetButton("Spell1_" + playerNumber)) {
            //        animator.SetBool(spellBook[1], true);
            //    }
            //    if (Input.GetButton("Spell2_" + playerNumber)) {
            //        Debug.Log("Lanza: " + playerNumber);
            //        animator.SetBool(spellBook[2], true);
            //    }
            //    if (Input.GetButton("Spell3_" + playerNumber)) {
            //        animator.SetBool(spellBook[3], true);
            //    }
        }
    }
}
