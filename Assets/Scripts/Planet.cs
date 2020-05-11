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
    Urban,
    Terran,
    Ocean,
    Desert,
    Tundra,
    Ice,
    Barren,
    LastVal
}
public enum PlanetSpecializations
{
    None,
    Homeworld,
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
    public Vector3 Location;
    public bool producing = false;
    public Fleet FleetInOrbit;
    public int Production;
    public List<Empire> exploredBy = new List<Empire>();
    public bool freeSwitchAvailable = false;
    public void Explore(Empire emp)
    {
        if(!exploredBy.Contains(emp))
        {
            exploredBy.Add(emp);
        }
    }
    public void Clear()
    {
        owner = null;
        PlanetSpecialization = PlanetSpecializations.None;
        RemainingSwitchDuration = 0;
        producing = false;
        Production = 0;
    }
    public void Colonize(Empire emp)
    {
        owner = emp;
        if(PlanetType == PlanetTypes.Urban)
        {
            PlanetSpecialization = PlanetSpecializations.Homeworld;
        }
        if (PlanetSpecialization == PlanetSpecializations.None)
        {
            freeSwitchAvailable = true;
        }
    }
    public int GetTypeTechBonus(Empire emp)
    {
        int ret = 0;
        if (PlanetType == PlanetTypes.Terran || PlanetType == PlanetTypes.Tundra)
        {
            ret += emp.GreenPlanetBonus;
        }
        if (PlanetType == PlanetTypes.Ocean || PlanetType == PlanetTypes.Ice)
        {
            ret += emp.BluePlanetBonus;
        }
        if (PlanetType == PlanetTypes.Desert || PlanetType == PlanetTypes.Barren)
        {
            ret += emp.DryPlanetBonus;
        }
        return ret;
    }
    public int GetSizeValueMultiplier(Empire emp)
    {
        return 2 + (int)PlanetSize + GetTypeTechBonus(emp);
    }
    public int GetMineralValueMultiplier(Empire emp)
    {
        int ret = GetSizeValueMultiplier(emp);
        if (PlanetType == PlanetTypes.Terran || PlanetType == PlanetTypes.Tundra)
        {
            ret += 2; 
        }
        return ret;
    }
    public int GetMineralOutput(Empire emp)
    {
        int value = 0;
        if(PlanetSpecialization == PlanetSpecializations.Homeworld)
        {
            value = 3 + emp.HomeWorldBonus;
        }
        else if(PlanetSpecialization == PlanetSpecializations.Mine)
        {
            value = GetMineralValueMultiplier(emp) + emp.MineMineralBonus;
        }
        return value;
    }
    public int GetResearchValueMultiplier(Empire emp)
    {
        int ret = GetSizeValueMultiplier(emp);
        if (PlanetType == PlanetTypes.Ocean || PlanetType == PlanetTypes.Ice)
        {
            ret += 2;
        }
        return ret;
    }
    public int GetResearchOutput(Empire emp)
    {
        int value = 0;
        if (PlanetSpecialization == PlanetSpecializations.Homeworld)
        {
            value = 3 + emp.HomeWorldBonus;
        }
        else if (PlanetSpecialization == PlanetSpecializations.Lab)
        {
            value = GetResearchValueMultiplier(emp) + emp.LabResearchBonus;
        }
        return value;
    }
    public int GetTranscenditeValueMultiplier(Empire emp)
    {
        int ret = GetSizeValueMultiplier(emp);
        if (PlanetType == PlanetTypes.Desert || PlanetType == PlanetTypes.Barren)
        {
            ret += 2;
        }
        return ret;
    }
    public int GetTranscenditeOutput(Empire emp)
    {
        int value = 0;
        if (PlanetSpecialization == PlanetSpecializations.Homeworld)
        {
            value = 3 + emp.HomeWorldBonus;
        }
        else if (PlanetSpecialization == PlanetSpecializations.Temple)
        {
            value = GetTranscenditeValueMultiplier(emp) + emp.TempleTranscenditeBonus;
        }
        return value;
    }
    public int GetOutpostValueMultiplier(Empire emp)
    {
        int ret = GetSizeValueMultiplier(emp);
        return ret;
    }
    public int GetProductionOutput(Empire emp)
    {
        int value = 0;
        if (PlanetSpecialization == PlanetSpecializations.Homeworld)
        {
            value = 3 + emp.HomeWorldBonus + emp.ProductionBonus;
        }
        else if (PlanetSpecialization == PlanetSpecializations.Outpost)
        {
            value = GetOutpostValueMultiplier(emp) + emp.ProductionBonus;
        }
        return value;
    }
    public void changeOwner(Empire newOwner)
    {
        owner = newOwner;
    }
    public void setHomeworld(Empire homeWorldOf)
    {
        changeOwner(homeWorldOf);
        PlanetSpecialization = PlanetSpecializations.Homeworld;
        PlanetType = PlanetTypes.Urban;
        PlanetSize = PlanetSizes.Medium;
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
        if (RemainingSwitchDuration == 0)
        {
            if (FleetInOrbit == null || FleetInOrbit.owner == owner)
            {
                if (producing)
                {
                    int prod = Mathf.Min(GetProductionOutput(owner), owner.Minerals - owner.MineralsSpentOnProductionThisTurn);
                    owner.MineralsSpentOnProductionThisTurn += prod;
                    Production += prod;
                }
            }
        }
    }
}
