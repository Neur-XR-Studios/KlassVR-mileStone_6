using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liskov : MonoBehaviour
{
    private Spell equpedSpell;
    // Start is called before the first frame update
    public void EqupedSpell(Spell spell)
    { equpedSpell = spell; }

    public void CastSpell()
    {
        if (equpedSpell!=null)
        {
            equpedSpell.Cast();
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
public class SpellController
{
    public SpellController()
    {
        Liskov liskov = new Liskov();
        Spell fireSpell = new FireSpell();
        Spell iceSpell = new IceSpell();
        liskov.EqupedSpell(fireSpell);
        liskov.CastSpell();

        liskov.EqupedSpell(iceSpell);
        liskov.CastSpell();
    }
}
public abstract class Spell
{
    public abstract void Cast();
}
public class FireSpell:Spell
{
    public override void Cast()
    {
        throw new System.NotImplementedException();
    }
}
public class IceSpell: Spell
{
    public override void Cast()
    {
        throw new System.NotImplementedException();
    }
}