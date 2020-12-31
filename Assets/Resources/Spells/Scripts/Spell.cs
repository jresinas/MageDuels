using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell {
    // Nombre del hechizo
    public string name;
    // Objeto prefab con el sistema de particulas asociado al hechizo
    public GameObject prefab;
    // Duración del sistema de particulas del hechizo antes de desaparecer
    public float duration;
    // Posición de origen del sistema de particulas del hechizo respecto al personaje
    public Vector3 offset;

    public Spell(string name, float duration, Vector3 offset = new Vector3()) {
        this.name = name;
        this.prefab = (GameObject)Resources.Load("Spells/Prefabs/" + name, typeof(GameObject));
        this.duration = duration;
        this.offset = offset;
    }
}
