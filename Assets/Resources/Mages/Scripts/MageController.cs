using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MageController : MonoBehaviour
{
    // Indica el tiempo (en seg) para incrementar 0.5 de energía
    private float ENERGY_INCREASE_TIME = 2.5f;

    // Numero de jugador 0 o 1
    public int playerNumber;
    // Velocidad de movimiento cuando anda hacia delante
    private float forwardSpeed = 1.8f;
    // Velocidad de movimiento cuando anda hacia atrás
    private float backSpeed = 1.4f;
    // Puntos de vida
    public float life = 10f;
    // Puntos de energía
    public float energy = 5f;
    // libro de hechizos (lista de hechizos seleccionados). El orden determina el botón de activación
    public string[] spellBook = new string[] { "SpellDown", "SpellCircle", "SpellFire", "SpellFireBall" };

    private Animator animator;
    private Rigidbody rb;

    public Text PlayerLife;
    public Text PlayerEnergy;

    // Variable auxiliar para cuando tiran al personaje. Identifica cuando está en el aire
    public bool fallInAir=false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(Energy());
    }


    // Update is called once per frame
    void Update()
    {
        Damage();

        PlayerLife.text = life.ToString();
        PlayerEnergy.text = energy.ToString();

    }

    IEnumerator Energy() {
        WaitForSeconds wait = new WaitForSeconds(ENERGY_INCREASE_TIME);
        if (energy < 10) {
            energy += 0.5f;
        }
        yield return wait;
        StartCoroutine(Energy());
    }


    /*
     * Controla el movimiento del personaje
     */
    public void Move(float HorizontalMove, float VerticalMove) {
        // Velocidad real de movimiento
        float speed = 0;
        //// Obtenemos la dirección del movimiento
        //float HorizontalMove = Input.GetAxis("Horizontal_"+playerNumber);
        //float VerticalMove = Input.GetAxis("Vertical_"+playerNumber);

        if (!Busy()) {
            // Vector del movimiento respecto al mundo
            Vector3 myInputs = new Vector3(HorizontalMove, 0f, VerticalMove);

            ClearMoveAnimations();
            if (Mathf.Abs(VerticalMove) > Mathf.Abs(HorizontalMove)) {
                speed = forwardSpeed;
                if (VerticalMove > 0) {
                    if (playerNumber == 0) {
                        animator.SetBool("MoveLeft", true);
                    } else {
                        animator.SetBool("MoveRight", true);
                    }
                }
                if (VerticalMove < -0) {
                    if (playerNumber == 0) {
                        animator.SetBool("MoveRight", true);
                    } else {
                        animator.SetBool("MoveLeft", true);
                    }
                }
            } else {
                if (HorizontalMove > 0) {
                    if (playerNumber == 0) {
                        animator.SetBool("MoveForward", true);
                        speed = forwardSpeed;
                    } else {
                        animator.SetBool("MoveBack", true);
                        speed = backSpeed;
                    }
                }
                if (HorizontalMove < -0) {
                    if (playerNumber == 0) {
                        animator.SetBool("MoveBack", true);
                        speed = backSpeed;
                    } else {
                        animator.SetBool("MoveForward", true);
                        speed = forwardSpeed;
                    }
                }
            }

            // Se modifica el multiplicador de velocidad de las animaciones de movimiento
            animator.SetFloat("MoveAnimationSpeed",myInputs.magnitude);
            // Se actualiza la posición en función a la velocidad y dirección del movimiento
            rb.MovePosition(rb.position + speed * Time.deltaTime * myInputs);
        }
    }

    /*
     * Controla la reacción del personaje al recibir daño
     */
    private void Damage() {
        if (animator.GetBool("Damage")) {
            rb.MovePosition(rb.position + 1.2f * Time.deltaTime * (playerNumber == 0? Vector3.left : Vector3.right));
        }

        if (animator.GetBool("Fall") && fallInAir) {
            rb.MovePosition(rb.position + 4.4f * Time.deltaTime * (playerNumber == 0 ? Vector3.left : Vector3.right));
        }
    }

    /*
     * Finaliza todas las animaciones de movimiento
     */
    private void ClearMoveAnimations() {
        animator.SetBool("MoveLeft", false);
        animator.SetBool("MoveRight", false);
        animator.SetBool("MoveForward", false);
        animator.SetBool("MoveBack", false);
    }

    /*
     * Finaliza todas las animaciones de hechizos
     */
    private void ClearSpellAnimations() {
        animator.SetBool("SpellDown", false);
        animator.SetBool("SpellCircle", false);
        animator.SetBool("SpellFire", false);
        animator.SetBool("SpellFireBall", false);
    }

    /*
     * Finaliza todas las animaciones
     */
    private void ClearAnimations() {
        ClearMoveAnimations();
        ClearSpellAnimations();
    }

    /* 
     * Indica si hay alguna animación ejecutandose que impida el movimiento del personaje
     */
    public bool Busy() {
        return animator.GetBool("SpellDown") ||
               animator.GetBool("SpellCircle") ||
               animator.GetBool("SpellFire") ||
               animator.GetBool("SpellFireBall") ||
               animator.GetBool("Damage") ||
               animator.GetBool("Fall");
    }


    public void StartCastingSpell(string spellName) {
        Spell spell = Initialize.spells[spellName];
        if (energy >= spell.energy) {
            animator.SetBool(spellName, true);
            energy -= spell.energy;
        }
    }

    /*
     * Se llama desde un evento de animación de un hechizo, e indica que comienza a emitirse el sistema de particulas asociado
     * Instancia el sistema de particulas correspondiente
     * spellName: nombre del hechizo  
     */
    public void CastSpell(string spellName) {
        Spell spell = Initialize.spells[spellName];

        GameObject spellInst = Instantiate(spell.prefab, transform.position + spell.offset + (playerNumber==1? 2*Vector3.left*spell.offset.x : Vector3.zero), (playerNumber == 1 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0)));
        spellInst.tag = "Player"+playerNumber+"Spell";
        Destroy(spellInst, spell.duration);
        
        ISpellController spellController = spellInst.GetComponent<ISpellController>();
        spellController.playerNumber = playerNumber;
        spellController.spellName = spellName;
    }

    /*
     * Se llama desde un evento de animación, e indica que la animación ha terminado 
     * Finaliza la animación del personaje
     * spellName: nombre del hechizo  
     */
    public void FinishAnimation(string spellName) {
        animator.SetBool(spellName, false);
    }

    /*
     * Detecta la colisión con un sistema de particulas (hechizos)
     * particle: objeto del sistema de particulas
     */
    private void OnTriggerEnter(Collider collider) {
        ISpellController spell = collider.GetComponent<ISpellController>();
        if (spell != null) {
            Debug.Log("El jugador " + playerNumber + " ha sido impactado por una " + spell.spellName + " lanzada por el jugador " + spell.playerNumber);

            if (spell.playerNumber != playerNumber) {
                Debug.Log("Impacto al enemigo");
                ClearAnimations();
                animator.SetBool(spell.ImpactReact(), true);
                life -= spell.ImpactDamage();
                spell.Impact();
            }
        }
    }
}
