using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCircleController : MonoBehaviour, ISpellController {
    // Número del jugador que lo ha invocado
    private int _playerNumber;
    // Nombre del hechizo
    private string _spellName;

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
    }

    public int ImpactDamage() {
        return 0;
    }

    public string ImpactReact() {
        return null;
    }

    public void Impact() {

    }
}
