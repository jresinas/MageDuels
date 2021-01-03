using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinController : MonoBehaviour
{
    MageController mc;

    // Start is called before the first frame update
    void Start()
    {
        mc = this.transform.parent.GetComponent<MageController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Se llama desde un evento de animación de un hechizo, e indica que comienza a emitirse el sistema de particulas asociado
     * Llama al metodo CastSpell del objeto padre
     * spellName: nombre del hechizo  
     */
    private void CastSpell(string spellName) {
        mc.CastSpell(spellName);
    }

    /*
     * Se llama desde un evento de animación de un hechizo, e indica que la animación ha terminado 
     * Llama al metodo FinishSpellAnimation del objeto padre
     * spellName: nombre del hechizo  
     */
    private void FinishAnimation(string spellName) {
        mc.FinishAnimation(spellName);

    }

    private void FallInAir(int state) {
        mc.fallInAir = (state>0);
    }
}
