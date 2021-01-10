using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellDownController : MonoBehaviour, ISpellController {
    // Número del jugador que lo ha invocado
    private int _playerNumber;
    // Nombre del hechizo
    private Vector3 _direction = new Vector3(1f, 0f, 0f);
    private float _velocity = 0.01f;
    private string _spellName;

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
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        transform.Translate(direction * velocity);
    }

    public int ImpactDamage() {
        return 1;
    }

    public string ImpactReact() {
        return "Damage";
    }

    public void Impact() {

    }
}
