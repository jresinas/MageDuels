using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellFireController : MonoBehaviour, ISpellController {
    // Número del jugador que lo ha invocado
    private int _playerNumber;
    // Nombre del hechizo
    private string _spellName;

    private float COLLIDER_MAX_CENTER_X = 2.4f;
    private float COLLIDER_MAX_HEIGHT = 4f;

    public int playerNumber {
        get => _playerNumber;
        set => _playerNumber = value;
    }
    public string spellName {
        get => _spellName;
        set => _spellName = value;
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        if (collider.height < COLLIDER_MAX_HEIGHT) {
            collider.height+=0.1f;
        }
        Vector3 v = collider.center;
        if (collider.center.x < COLLIDER_MAX_CENTER_X) {
            v += new Vector3(0.05f, 0f, 0f);
            collider.center = v;
        }
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
