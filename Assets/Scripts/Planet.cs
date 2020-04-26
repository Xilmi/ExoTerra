using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlanetSizes
{
    Small,
    Medium,
    Large,
    LastVal
}
public enum PlanetTypes
{
    Terran,
    Arid,
    Ocean,
    Jungle,
    Tundra,
    Desert,
    Ice,
    Swamp,
    LastVal
}
public enum PlanetSpecializations
{
    None,
    Homeworld,
    Farm,
    Mine,
    Factory,
    Lab,
    Temple,
    LastVal
}

public class Planet
{
    public GameObject guiPlanet;
    public Text guiText;
    // Start is called before the first frame update
    public Empire owner;
    public PlanetSizes PlanetSize;
    public PlanetTypes PlanetType;
    public PlanetSpecializations PlanetSpecialization;
    public PlanetSpecializations SwitchingToSpecialization;
    public int RemainingSwitchDuration;
    public Vector3 Location;
    public string Name;
    public void changeOwner(Empire newOwner)
    {
        owner = newOwner;
        if (owner == null)
        {
            guiPlanet.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            guiPlanet.GetComponent<SpriteRenderer>().color = owner.empireColor;
        }
    }
}
