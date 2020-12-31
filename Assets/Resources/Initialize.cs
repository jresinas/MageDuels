using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize : MonoBehaviour
{
    //public static List<Spell> spells = new List<Spell>();
    public static Dictionary<string, Spell> spells = new Dictionary<string, Spell>();
    // Start is called before the first frame update
    void Start()
    {
        spells["SpellDown"] = new Spell("SpellDown", 3f, offset: new Vector3(0.8f, 0.2f, -0.3f));
        spells["SpellCircle"] = new Spell("SpellCircle", 2f, offset: new Vector3(0f, 0.2f, 0f));
        spells["SpellFire"] = new Spell("SpellFire", 3f, offset: new Vector3(0.8f, 1f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
    }
}
