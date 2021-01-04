using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuController : MonoBehaviour { 
    // Diferencia entre el eje Z de dos objetos a partir de la cual se considera evitada la colisión
    private float POS_OFFSET = 1f;

    private MageController mc;
    private GameObject opp;
    private MageController opp_mc;
    // Start is called before the first frame update
    void Start()
    {
        mc = GetComponent<MageController>();
        opp = GameObject.Find("Player1");
        opp_mc = opp.GetComponent<MageController>();
    }

    // Update is called once per frame
    void Update()
    {
        mc.Move(-0.25f, 0);
        //Decision();
    }

    private void Decision() {
        if (!mc.Busy()) {
            List<GameObject> attacks = Attacked();
            if (attacks != null) {
                Defense(attacks);
            } else {
                StandBy();
            }
        }
    }

    /*
     * Realiza maniobra defensiva
     * attacks: lista de ataques peligrosos recibidos
     */
    private void Defense(List<GameObject> attacks) {
        Dodge(attacks);
    }

    /*
     * No hace nada
     */
    private void StandBy() {
        mc.Move(0, 0);
    }

    /*
     * Busca la posición Z del enemigo
     */
    private void Follow() {
        if (opp.transform.position.z - POS_OFFSET > gameObject.transform.position.z) {
            Debug.Log("sube");
            mc.Move(0, 1);
        } else if (opp.transform.position.z + POS_OFFSET < gameObject.transform.position.z) {
            Debug.Log("baja");
            mc.Move(0, -1);
        } else {
            mc.Move(0, 0);
        }
    }

    /*
     * Comprueba si está siendo atacado
     * Devuelve la lista de ataques peligrosos recibidos (null si no recibe ataques peligrosos)
     */
    private List<GameObject> Attacked() {
        GameObject[] enemy_spells = GameObject.FindGameObjectsWithTag("Player"+opp_mc.playerNumber+"Spell");
        if (enemy_spells.Length > 0) {
            List<GameObject> dangerousAttacks = new List<GameObject>();
            foreach (GameObject spell in enemy_spells) {
                if (DangerousAttack(spell)) { 
                    dangerousAttacks.Add(spell);
                }
            }
            if (dangerousAttacks.Count > 0) {
                Debug.Log("hay ataques: " + dangerousAttacks.Count);
                return dangerousAttacks;
            }
        }

        return null;
    }

    /*
     * Determina si un ataque es peligroso, es decir, hay peligro de que pueda impactar en la posición actual
     * Devuelve true si es peligroso, false si no lo es
     */
    private bool DangerousAttack(GameObject spell) {
        return spell.transform.position.x < transform.position.x && Mathf.Abs(spell.transform.position.z - transform.position.z) <= POS_OFFSET*2;
    }

    /*
     * Se mueve en el eje Z para evitar los ataques recibidos
     * spells: lista de ataques (hechizos) recibidos
     */
    private void Dodge(List<GameObject> spells) {
        GameObject spell = NearestAttack(spells);
        if (transform.position.z <= spell.transform.position.z) {
            Debug.Log("sube");
            mc.Move(0, -1);
        } else {
            Debug.Log("baja");
            mc.Move(0, 1);
        }
    }

    /*
     * Determina cual es el objeto más cercano de una lista de objetos
     * objs: lista de objetos
     */
    private GameObject NearestAttack(List<GameObject> objs) {
        if (objs.Count == 0) {
            return null;
        }

        GameObject closest = objs[0];
        if (objs.Count > 1) {
            float min = (objs[0].transform.position - transform.position).sqrMagnitude;
            foreach (GameObject obj in objs) {
                float distance = (obj.transform.position - transform.position).sqrMagnitude;
                if (distance < min) {
                    closest = obj;
                    min = distance;
                }
            }
        }

        return closest;
    }
}
