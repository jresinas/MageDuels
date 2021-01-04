using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MageController : MonoBehaviour
{
    // Numero de jugador 0 o 1
    public int playerNumber;
    // Velocidad de movimiento cuando anda hacia delante
    private float forwardSpeed = 1.8f;
    // Velocidad de movimiento cuando anda hacia atrás
    private float backSpeed = 1.4f;
    // Puntos de vida
    private int life = 10;
    // libro de hechizos (lista de hechizos seleccionados). El orden determina el botón de activación
    private string[] spellBook = new string[4] { "SpellDown", "SpellCircle", "SpellFire", "SpellFireBall" };

    private Animator animator;
    private Rigidbody rb;

    public List<ParticleCollisionEvent> collisionEvents;
    public Text PlayerLife;


    public bool fallInAir=false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }


    // Update is called once per frame
    void Update()
    {
        Move();
        Spells();
        Damage();
        //ChangeCharacter();

        PlayerLife.text = life.ToString();

    }

    /*
     * Controla el movimiento del personaje
     */
    private void Move() {
        // Velocidad real de movimiento
        float speed = 0;
        // Obtenemos la dirección del movimiento
        float HorizontalMove = Input.GetAxis("Horizontal_"+playerNumber);
        float VerticalMove = Input.GetAxis("Vertical_"+playerNumber);


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
     * Controla el retroceso del personaje al recibir daño
     */
    private void Damage() {
        if (animator.GetBool("Damage")) {
            rb.MovePosition(rb.position + 1.2f * Time.deltaTime * (playerNumber == 0? Vector3.left : Vector3.right));
        }

        if (animator.GetBool("Fall") && fallInAir) {
            rb.MovePosition(rb.position + 2.4f * Time.deltaTime * (playerNumber == 0 ? Vector3.left : Vector3.right));
        }
    }


    /*
     * Controla la ejecución de los hechizos
     */
    private void Spells() {
            
        if (!Busy()) { 
            for (int i = 0; i < spellBook.Length; i++) {
                if (Input.GetButton("Spell" + i + "_" + playerNumber)) {
                    animator.SetBool(spellBook[i], true);
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
    private bool Busy() {
        return animator.GetBool("SpellDown") ||
               animator.GetBool("SpellCircle") ||
               animator.GetBool("SpellFire") ||
               animator.GetBool("SpellFireBall") ||
               animator.GetBool("Damage") ||
               animator.GetBool("Fall");
    }


    /*
     * Se llama desde un evento de animación de un hechizo, e indica que comienza a emitirse el sistema de particulas asociado
     * Instancia el sistema de particulas correspondiente
     * spellName: nombre del hechizo  
     */
    public void CastSpell(string spellName) {
        Spell spell = Initialize.spells[spellName];

        GameObject spellInst = Instantiate(spell.prefab, transform.position + spell.offset + (playerNumber==1? 2*Vector3.left*spell.offset.x : Vector3.zero), (playerNumber == 1 ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0)));
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
    //void OnParticleCollision(GameObject particle) {
    //    ISpellController spell = particle.GetComponentInParent<ISpellController>();
    //    Debug.Log("Impacto");

    //    if (spell.playerNumber != playerNumber) {
    //        Debug.Log("Impacto al enemigo");
    //        ClearAnimations();
    //        animator.SetBool(spell.ImpactReact(), true);
    //        life -= spell.ImpactDamage();
    //        spell.Impact();
    //    }
    //}

    private void OnTriggerEnter(Collider collider) {
        ISpellController spell = collider.GetComponent<ISpellController>();
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
