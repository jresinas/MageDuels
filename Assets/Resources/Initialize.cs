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
        spells["SpellDown"] = new Spell("SpellDown", 1f, 3f, offset: new Vector3(0.8f, 0.5f, 0f));
        spells["SpellCircle"] = new Spell("SpellCircle", 1f, 2f, offset: new Vector3(0f, 0.2f, 0f));
        spells["SpellFire"] = new Spell("SpellFire", 2f, 2.2f, offset: new Vector3(0.8f, 1f, 0f));
        spells["SpellFireBall"] = new Spell("SpellFireBall", 1f, 10f, offset: new Vector3(0.6f, 1f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
    }
}
