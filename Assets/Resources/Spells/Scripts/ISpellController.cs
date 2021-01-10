using UnityEngine;

interface ISpellController
{
    int playerNumber { get; set; }
    string spellName { get; set; }
    Vector3 direction { get; }
    float velocity { get; }

    int ImpactDamage();
    string ImpactReact();
    void Impact();
}
