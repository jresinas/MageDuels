using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellFireBallController : MonoBehaviour, ISpellController {
    // Número del jugador que lo ha invocado
    private int _playerNumber;
    // Nombre del hechizo
    private string _spellName;
    private Vector3 _direction = new Vector3(1f,0f,0f);
    private float _velocity = 10f;
    public GameObject explosion_prefab;

    public int playerNumber {
        get => _playerNumber;
        set => _playerNumber = value;
    }
    public string spellName {
        get => _spellName;
        set => _spellName = value;
    }
    public Vector3 direction {
        get => _direction;
    }
    public float velocity {
        get => _velocity;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(new Vector3(0.08f, 0f, 0f));
        transform.Translate(direction * velocity * Time.deltaTime);
    }

    public int ImpactDamage() {
        return 1;
    }

    public string ImpactReact() {
        return "Fall";
        //return "Damage";
    }

    public void Impact() {
        //GameObject exp_prefab = (GameObject)Resources.Load("Spells/Prefabs/Others/Explosion", typeof(GameObject));

        Destroy(gameObject);
        GameObject explosion = Instantiate(explosion_prefab, transform.position, transform.rotation);
        Destroy(explosion, 1f);
    }

}
