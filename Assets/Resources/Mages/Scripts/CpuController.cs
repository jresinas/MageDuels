using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuController : MonoBehaviour { 
    // Diferencia entre el eje Z de dos objetos a partir de la cual se considera evitada la colisión
    private float POS_OFFSET = 1.5f;
    private float OFF_TEND = 0.5f;
    private float DEF_TEND = 0.5f;

    private MageController mc;
    private GameObject opp;
    private MageController opp_mc;

    private float offTend;
    private float defTend;

    //private int STEPS_LOOK = 5;
    private float MOVE_PLANNING_DIST = 1f;
    //private int MOVE_OPTIONS_NUMBER = 1000;

    private Vector3 currentBestMovement;
    //private float currentBestMovementScore;




    // Start is called before the first frame update
    void Start()
    {
        mc = GetComponent<MageController>();
        opp = GameObject.Find("Player1");
        opp_mc = opp.GetComponent<MageController>();
        offTend = OFF_TEND;
        defTend = DEF_TEND;
        currentBestMovement = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Tendency();
        //mc.Move(-0.25f, 0);
        //Decision();
        Move();
    }

    private void Tendency() {
        // energía del oponente
        offTend += 5 - opp_mc.energy;
        // energía propia
        offTend += mc.energy - 5;
        // vida del oponente 
        offTend += 5 - opp_mc.life;
        // vida propia
        offTend += mc.life - 5;
        //if (mc.Busy()) {
        //    offTend += 10;
        //}



        defTend = 1f - offTend;
    }

    private void Move() {
        Vector3 nextPosition = GetNextMove();

        if (Distance(currentBestMovement, transform.position) < MOVE_PLANNING_DIST) {
            currentBestMovement = transform.position;
        }

        if (GetPositionScore(nextPosition) > GetPositionScore(currentBestMovement)) {
            currentBestMovement = nextPosition;
        }
        Vector3 movement = (currentBestMovement - transform.position).normalized;
        Debug.Log("Mejor movimiento: "+(currentBestMovement-transform.position));
        mc.Move(movement.x, movement.z);
    }

    private Vector3 GetNextMove() {
        List<Vector3> options = GetMoveOptions();

        Vector3 bestOption = transform.position + Vector3.zero;
        float bestScore = GetPositionScore(bestOption);
        foreach (Vector3 option in options) {
            //Debug.Log("Opción: " + option);
            float score = GetPositionScore(option);
            //Debug.Log("Puntuación: " + score);
            if (score > bestScore) {
                bestOption = option;
                bestScore = score;
            }
        }

        return bestOption;
    }

    private List<Vector3> GetMoveOptions() {
        List<Vector3> options = new List<Vector3>();

        //for (int i = 0; i < MOVE_OPTIONS_NUMBER; i++) {
        //    float xInc = Random.Range(-MOVE_PLANNING_DIST, MOVE_PLANNING_DIST);
        //    float zInc = Random.Range(-MOVE_PLANNING_DIST, MOVE_PLANNING_DIST);
        //    options.Add(transform.position + new Vector3(xInc, 0f, zInc));
        //}
        for (int x = -1; x <= 1; x++) {
            for (int z = -1; z <= 1; z++) {
                options.Add(transform.position + new Vector3(x, 0f, z));
            }
        }

        return options;
    }

    private float GetPositionScore(Vector3 position) {
        float score = 0;
        List<GameObject> dangerousAttacks = DangerousAttacks(position);
        score += DistanceAttackScore(dangerousAttacks, position) * defTend;

        if (offTend > 0.7f) {
            score -= DistanceEnemyScore(position);
        } else if (defTend > 0.7f) {
            score += DistanceEnemyScore(position);
        } else {
            score -= SecurityDistanceScore(position);
        }
        
        // distancia al centro de su terreno
        //score -= DistanceMiddleScore(position);
        //score += SpeedScore(position);

        return score;
    }

    private float SpeedScore(Vector3 position) {
        if (position.x > Mathf.Abs(position.z)) {
            return -2;
        } else {
            return 2;
        }
    }

    private float DistanceMiddleScore(Vector3 position) {
        return Distance(new Vector3(3f,0f,0f), position);
    }

    private float SecurityDistanceScore(Vector3 position) {
        return Mathf.Abs(position.z - opp.transform.position.z) * 2 + Mathf.Abs(9 - (position.x - opp.transform.position.x)) * 4;
    }


    private float DistanceEnemyScore(Vector3 position) {
        return Distance(opp.transform.position, position) + Mathf.Abs(opp.transform.position.z - position.z) * 10;
    }


    private void Decision() {
        if (!mc.Busy()) {
            List<GameObject> attacks = new List<GameObject>();
            GameObject attack = Attacked();
            if (attack != null) {
                attacks.Add(attack);
            }
            if (attacks != null && attacks.Count > 0) {
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
    private List<GameObject> DangerousAttacks(Vector3 position) {
        GameObject[] enemy_spells = EnemySpells();
        if (enemy_spells.Length > 0) {
            List<GameObject> dangerousAttacks = new List<GameObject>();
            foreach (GameObject spell in enemy_spells) {
                if (DangerousAttack(spell, position)) {
                    dangerousAttacks.Add(spell);
                }
            }
            if (dangerousAttacks.Count > 0) {
                //Debug.Log("hay ataques: " + dangerousAttacks.Count);
                return dangerousAttacks;
            }
        }

        return null;
    }

    private GameObject Attacked() {
        GameObject[] enemy_spells = EnemySpells();
        if (enemy_spells.Length > 0) {
            //List<GameObject> dangerousAttacks = new List<GameObject>();
            float max = 0f;
            GameObject mostDanger = null;
            foreach (GameObject spell in enemy_spells) {
                float dangerValue = DangerousAttack(spell);
                if (dangerValue > max) {
                    mostDanger = spell;
                    max = dangerValue;
                }
            }
            return mostDanger;
        }

        return null;
    }


    /*
     * Devuelve todos los hechizos lanzados por el rival actualmente
     */
    private GameObject[] EnemySpells() {
        return GameObject.FindGameObjectsWithTag("Player" + opp_mc.playerNumber + "Spell");
    }

    /*
     * Determina si un ataque es peligroso, es decir, hay peligro de que pueda impactar en la posición actual
     * Devuelve true si es peligroso, false si no lo es
     */
    private float DangerousAttack(GameObject spell) {
        if (spell.transform.position.x < transform.position.x && Mathf.Abs(spell.transform.position.z - transform.position.z) <= POS_OFFSET * 2) {
            return 20f - Distance(spell, gameObject);
        } else {
            return 0f;
        }
    }

    private bool DangerousAttack(GameObject spell, Vector3 position) {
        //if (spell.transform.position.x < position.x && Mathf.Abs(spell.transform.position.z - position.z) <= POS_OFFSET * 2) {
        //    return 50f - Distance(spell.transform.position, position);
        //} else {
        //    return 0f;
        //}
        return spell.transform.position.x < position.x && Mathf.Abs(spell.transform.position.z - position.z) <= POS_OFFSET * 2;
        // usar spherecast con la dirección del hechizo para ver si impacta, en lugar de asumir que es el eje z
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
            float min = Distance(objs[0], gameObject);
            foreach (GameObject obj in objs) {
                float distance = Distance(obj, gameObject);
                if (distance < min) {
                    closest = obj;
                    min = distance;
                }
            }
        }

        return closest;
    }

    private float DistanceAttackScore(List<GameObject> objs, Vector3 position) {
        if (objs == null || objs.Count == 0) {
            return 100f;
        }

        GameObject closest = objs[0];
        float min = Distance(objs[0].transform.position, position);
        if (objs.Count > 1) {
            foreach (GameObject obj in objs) {
                float distance = Distance(obj.transform.position, position);
                if (distance < min) {
                    closest = obj;
                    min = distance;
                }
            }
        }

        return min + Mathf.Abs(closest.transform.position.z - position.z)*100;
        // usar spherecast con la dirección del hechizo para ver si impacta, en lugar de asumir que es el eje z
    }

    private float Distance(GameObject obj1, GameObject obj2) {
        return (obj1.transform.position - obj2.transform.position).sqrMagnitude;
    }

    private float Distance(Vector3 v1, Vector3 v2) {
        return (v1 - v2).sqrMagnitude;
    }
}
