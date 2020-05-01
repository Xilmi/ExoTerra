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
    public bool producing = false;
    public Fleet FleetInOrbit;
    public int Production;
    public bool freeSwitchAvailable = false;
    public void Colonize(Empire emp)
    {
        owner = emp;
        freeSwitchAvailable = true;
    }
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
        if (RemainingSwitchDuration == 0)
        {
            if (PlanetSpecialization == PlanetSpecializations.Homeworld)
            {
                owner.Food += 2;
                owner.Minerals += 2;
                owner.Research += 2;
                owner.Transcendite += 2;
                owner.FoodPerTurn += 2;
                owner.MineralsPerTurn += 2;
                owner.ResearchPerTurn += 2;
                owner.TranscenditePerTurn += 2;
            }
            if (PlanetSpecialization == PlanetSpecializations.Farm)
            {
                Output = 2 + (int)PlanetSize;
                if (PlanetType == PlanetTypes.Terran || PlanetType == PlanetTypes.Tundra)
                {
                    Output += 2;
                }
                owner.Food += Output;
                owner.FoodPerTurn += Output;
            }
            if (PlanetSpecialization == PlanetSpecializations.Mine)
            {
                Output = 2 + (int)PlanetSize;
                if (PlanetType == PlanetTypes.Jungle || PlanetType == PlanetTypes.Swamp)
                {
                    Output += 2;
                }
                owner.Minerals += Output;
                owner.MineralsPerTurn += Output;
            }
            if (PlanetSpecialization == PlanetSpecializations.Lab)
            {
                Output = 2 + (int)PlanetSize;
                if (PlanetType == PlanetTypes.Ocean || PlanetType == PlanetTypes.Ice)
                {
                    Output += 2;
                }
                owner.Research += Output;
                owner.ResearchPerTurn += Output;
            }
            if (PlanetSpecialization == PlanetSpecializations.Temple)
            {
                Output = 2 + (int)PlanetSize;
                if (PlanetType == PlanetTypes.Arid || PlanetType == PlanetTypes.Desert)
                {
                    Output += 2;
                }
                owner.Transcendite += Output;
                owner.TranscenditePerTurn += Output;
            }
            if (RemainingSwitchDuration == 0)
            {
                if (producing)
                {
                    int ProductionOutput = 0;
                    if (PlanetSpecialization == PlanetSpecializations.Outpost)
                    {
                        ProductionOutput = 2 + (int)PlanetSize;
                    }
                    if (PlanetSpecialization == PlanetSpecializations.Homeworld)
                    {
                        ProductionOutput = 2;
                    }
                    ProductionOutput = Mathf.Max(0, Mathf.Min(ProductionOutput, owner.Minerals - owner.MineralMaintainance));
                    Output = ProductionOutput;
                    Game.printToConsole("ProductionOutput = " + ProductionOutput);
                    Production += ProductionOutput;
                    owner.MineralsPerTurn -= ProductionOutput;
                    owner.Minerals -= ProductionOutput;
                }
            }
        }
        if (PlanetSpecialization != PlanetSpecializations.None)
        {
            owner.FoodMaintainance += 1;
        }
    }
}
