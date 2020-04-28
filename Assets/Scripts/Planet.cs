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
    Outpost,
    Lab,
    Temple,
    LastVal
}

public class Planet
{
    public GameObject guiPlanet;
    // Start is called before the first frame update
    public Empire owner;
    public PlanetSizes PlanetSize;
    public PlanetTypes PlanetType;
    public PlanetSpecializations PlanetSpecialization;
    public PlanetSpecializations SwitchingToSpecialization;
    public int RemainingSwitchDuration;
    public int Output;
    public Vector3 Location;
    public double GetSizeValueMultiplier()
    {
        return 1 + 0.5 * (int)PlanetSize;
    }
    public double GetFoodValueMultiplier()
    {
        double ret = GetSizeValueMultiplier();
        if (PlanetType == PlanetTypes.Terran || PlanetType == PlanetTypes.Tundra)
        {
            ret += 1; 
        }
        return ret;
    }
    public double GetMineralValueMultiplier()
    {
        double ret = GetSizeValueMultiplier();
        if (PlanetType == PlanetTypes.Jungle || PlanetType == PlanetTypes.Swamp)
        {
            ret += 1;
        }
        return ret;
    }
    public double GetResearchValueMultiplier()
    {
        double ret = GetSizeValueMultiplier();
        if (PlanetType == PlanetTypes.Ocean || PlanetType == PlanetTypes.Ice)
        {
            ret += 1;
        }
        return ret;
    }
    public double GetTranscenditeValueMultiplier()
    {
        double ret = GetSizeValueMultiplier();
        if (PlanetType == PlanetTypes.Arid || PlanetType == PlanetTypes.Desert)
        {
            ret += 1;
        }
        return ret;
    }
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
    public void setHomeworld(Empire homeWorldOf)
    {
        changeOwner(homeWorldOf);
        PlanetSpecialization = PlanetSpecializations.Homeworld;
    }
    public void processTurn()
    {
        if(RemainingSwitchDuration > 0)
        {
            RemainingSwitchDuration--;
            if (RemainingSwitchDuration == 0)
            {
                PlanetSpecialization = SwitchingToSpecialization;
            }
        }
        if (PlanetSpecialization == PlanetSpecializations.Homeworld)
        {
            owner.FoodPerTurn += 2;
            owner.MineralsPerTurn += 2;
            owner.ResearchPerTurn += 2;
            owner.TranscenditePerTurn += 2;
        }
        if (PlanetSpecialization == PlanetSpecializations.Farm)
        {
            owner.FoodPerTurn += 2 + (int)PlanetSize;
            Output = 2 + (int)PlanetSize;
            if (PlanetType == PlanetTypes.Terran || PlanetType == PlanetTypes.Tundra)
            {
                owner.FoodPerTurn += 2;
                Output += 2;
            }
        }
        if (PlanetSpecialization == PlanetSpecializations.Mine)
        {
            owner.MineralsPerTurn += 2 + (int)PlanetSize;
            Output = 2 + (int)PlanetSize;
            if (PlanetType == PlanetTypes.Jungle || PlanetType == PlanetTypes.Swamp)
            {
                owner.MineralsPerTurn += 2;
                Output += 2;
            }
        }
        if (PlanetSpecialization == PlanetSpecializations.Lab)
        {
            owner.ResearchPerTurn += 2 + (int)PlanetSize;
            Output = 2 + (int)PlanetSize;
            if (PlanetType == PlanetTypes.Ocean || PlanetType == PlanetTypes.Ice)
            {
                owner.ResearchPerTurn += 2;
                Output += 2;
            }
        }
        if (PlanetSpecialization == PlanetSpecializations.Temple)
        {
            owner.TranscenditePerTurn += 2 + (int)PlanetSize;
            Output = 2 + (int)PlanetSize;
            if (PlanetType == PlanetTypes.Arid || PlanetType == PlanetTypes.Desert)
            {
                owner.TranscenditePerTurn += 2;
                Output += 2;
            }
        }
        if (PlanetSpecialization != PlanetSpecializations.None)
        {
            owner.FoodMaintainance += 1;
        }
    }
}
