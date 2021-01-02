interface  ISpellController
{
    int playerNumber { get; set; }
    string spellName { get; set; }
    int ImpactDamage();
    string ImpactReact();
    void Impact();
}
