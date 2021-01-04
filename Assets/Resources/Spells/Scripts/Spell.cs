using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell {
    // Nombre del hechizo
    public string name;
    // Objeto prefab con el sistema de particulas asociado al hechizo
    public GameObject prefab;
    // Coste de energía de invocar el hechizo
    public float energy;
    // Duración del sistema de particulas del hechizo antes de desaparecer
    public float duration;
    // Posición de origen del sistema de particulas del hechizo respecto al personaje
    public Vector3 offset;

    public Spell(string name, float energy, float duration, Vector3 offset = new Vector3()) {
        this.name = name;
        this.prefab = (GameObject)Resources.Load("Spells/Prefabs/" + name, typeof(GameObject));
        this.energy = energy;
        this.duration = duration;
        this.offset = offset;
    }
}
