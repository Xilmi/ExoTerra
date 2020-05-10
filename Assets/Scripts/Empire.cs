using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Empire
{
    public string EmpireName;
    public string RulerName;
    public string HomeWold;
    public int iD;
    public int Minerals;
    public int MineralsPerTurn;
    public int MineralMaintainance;
    public int MineralsSpentOnProductionThisTurn;
    public int Research;
    public int ResearchPerTurn;
    public int ResearchMaintainance;
    public int Transcendite;
    public int TranscenditePerTurn;
    public double FoodValue;
    public double MineralValue;
    public double ResearchValue;
    public double TranscenditeValue;
    public double OutpostValue;
    public Color empireColor;
    public bool isAIControlled;
    public List<Tech> Technologies;
    public List<TechTypes> TechsToPick;
    public int EmpireMineralsPerTurn;
    public int EmpireResearchPerTurn;
    public int EmpireTranscenditePerTurn;
    public int HomeWorldBonus;
    public int MineMineralBonus;
    public int LabResearchBonus;
    public int TempleTranscenditeBonus;
    public int ProductionBonus;
    public int GreenPlanetBonus;
    public int BluePlanetBonus;
    public int DryPlanetBonus;
    public float ExtraDifficulty;
    public float MineralPercentageBonus;
    public float ResearchPercentageBonus;
    public float TranscenditePercentageBonus;
    public float CombatBonus;
    public int HomeWorldCount;
    public int MineCount;
    public int LabCount;
    public int TempleCount;
    public int GreenCount;
    public int BlueCount;
    public int DryCount;
    public int ProducerCount;
    public int MineralsPerTurnBeforePercentageBonus;
    public int ResearchPerTurnBeforePercentageBonus;
    public int TranscenditePerTurnBeforePercentageBonus;
    public int ShipCount;

    public void Init()
    {
        Minerals = 2;
        Research = 2;
        Transcendite = 2;
        Technologies = new List<Tech>();
        TechsToPick = new List<TechTypes>();
        ReshuffleTechToPick();
    }
    public void ProcessTurn(Galaxy gal)
    {
        RefreshPerTurnValues(gal);
        Minerals += MineralsPerTurn;
        Research += ResearchPerTurn;
        Transcendite += TranscenditePerTurn;
        Minerals -= MineralMaintainance;
        Minerals = Mathf.Max(Minerals, 0);
        Research -= ResearchMaintainance;
        Research = Mathf.Max(Research, 0);
    }
    public void RefreshPerTurnValues(Galaxy gal)
    {
        MineralsSpentOnProductionThisTurn = 0;
        MineralsPerTurn = 0;
        ResearchPerTurn = 0;
        TranscenditePerTurn = 0;
        MineralMaintainance = 0;
        ResearchMaintainance = 0;
        HomeWorldCount = 0;
        MineCount = 0;
        LabCount = 0;
        TempleCount = 0;
        GreenCount = 0;
        BlueCount = 0;
        DryCount = 0;
        ProducerCount = 0;
        MineralsPerTurnBeforePercentageBonus = 0;
        ResearchPerTurnBeforePercentageBonus = 0;
        TranscenditePerTurnBeforePercentageBonus = 0;
        GetTechBonuses();
        ShipCount = 0;
        foreach (Planet pl in gal.Planets)
        {
            if (pl.owner == this)
            {
                if (pl.FleetInOrbit == null || pl.FleetInOrbit.owner == pl.owner)
                {
                    MineralsPerTurn += pl.GetMineralOutput(this);
                    ResearchPerTurn += pl.GetResearchOutput(this);
                    TranscenditePerTurn += pl.GetTranscenditeOutput(this);
                    if (pl.producing)
                    {
                        MineralMaintainance += pl.GetProductionOutput(this);
                    }
                }
                if (pl.PlanetSpecialization != PlanetSpecializations.Homeworld)
                {
                    MineralMaintainance += 1;
                }
                if (pl.PlanetSpecialization == PlanetSpecializations.Homeworld || pl.PlanetSpecialization != PlanetSpecializations.Outpost)
                {
                    if (pl.PlanetSpecialization == PlanetSpecializations.Homeworld)
                    {
                        HomeWorldCount++;
                    }
                    ProducerCount++;
                }
                if(pl.PlanetSpecialization == PlanetSpecializations.Mine)
                {
                    MineCount++;
                }
                else if (pl.PlanetSpecialization == PlanetSpecializations.Lab)
                {
                    LabCount++;
                }
                else if (pl.PlanetSpecialization == PlanetSpecializations.Temple)
                {
                    TempleCount++;
                }
                if(pl.PlanetType == PlanetTypes.Terran || pl.PlanetType == PlanetTypes.Terran)
                {
                    GreenCount++;
                }
                else if (pl.PlanetType == PlanetTypes.Ocean || pl.PlanetType == PlanetTypes.Ice)
                {
                    BlueCount++;
                }
                else if (pl.PlanetType == PlanetTypes.Desert || pl.PlanetType == PlanetTypes.Barren)
                {
                    DryCount++;
                }
            }
        }
        foreach (Fleet fl in gal.Fleets)
        {
            if (fl.owner == this)
            {
                MineralMaintainance += fl.ShipCount;
                if(fl.FleetType == FleetTypes.Combat)
                {
                    ShipCount += fl.ShipCount;
                }
            }
        }
        MineralsPerTurn += EmpireMineralsPerTurn;
        ResearchPerTurn += EmpireResearchPerTurn;
        TranscenditePerTurn += EmpireTranscenditePerTurn;
        MineralsPerTurnBeforePercentageBonus = MineralsPerTurn;
        ResearchPerTurnBeforePercentageBonus = ResearchPerTurn;
        TranscenditePerTurnBeforePercentageBonus = TranscenditePerTurn;
        MineralsPerTurn = Mathf.RoundToInt(MineralsPerTurn * (1.0f + MineralPercentageBonus + ExtraDifficulty));
        ResearchPerTurn = Mathf.RoundToInt(ResearchPerTurn * (1.0f + ResearchPercentageBonus + ExtraDifficulty));
        TranscenditePerTurn = Mathf.RoundToInt(TranscenditePerTurn * (1.0f + TranscenditePercentageBonus + ExtraDifficulty));
    }
    public void GetTechBonuses()
    {
        ResearchMaintainance = 0;
        foreach (Tech tech in Technologies)
        {
            ResearchMaintainance += tech.GetMaintainanceCost();
            switch(tech.TechType)
            {
                case TechTypes.FlatMinerals:
                    if(tech.OneTimeBonusAvailable)
                    {
                        Minerals += tech.GetFlatValue();
                        tech.OneTimeBonusAvailable = false;
                    }
                    break;
                case TechTypes.FlatTranscendite:
                    if (tech.OneTimeBonusAvailable)
                    {
                        Transcendite += tech.GetFlatValue();
                        tech.OneTimeBonusAvailable = false;
                    }
                    break;
                case TechTypes.EmpireMineralsPerTurn:
                    EmpireMineralsPerTurn = tech.GetEmpireValuePerTurn();
                    break;
                case TechTypes.EmpireResearchPerTurn:
                    EmpireResearchPerTurn = tech.GetEmpireValuePerTurn();
                    break;
                case TechTypes.EmpireTranscenditePerTurn:
                    EmpireTranscenditePerTurn = tech.GetEmpireValuePerTurn();
                    break;
                case TechTypes.HomeWorldBonus:
                    HomeWorldBonus = tech.GetSimpleBonus();
                    break;
                case TechTypes.MineMineralBonus:
                    MineMineralBonus = tech.GetSimpleBonus();
                    break;
                case TechTypes.LabResearchBonus:
                    LabResearchBonus = tech.GetSimpleBonus();
                    break;
                case TechTypes.TempleTranscenditeBonus:
                    TempleTranscenditeBonus = tech.GetSimpleBonus();
                    break;
                case TechTypes.ProductionBonus:
                    ProductionBonus = tech.GetSimpleBonus();
                    break;
                case TechTypes.GreenPlanetBonus:
                    GreenPlanetBonus = tech.GetSimpleBonus();
                    break;
                case TechTypes.BluePlanetBonus:
                    BluePlanetBonus = tech.GetSimpleBonus();
                    break;
                case TechTypes.DryPlanetBonus:
                    DryPlanetBonus = tech.GetSimpleBonus();
                    break;
                case TechTypes.MineralPercentageBonus:
                    MineralPercentageBonus = tech.GetPercentageBonus();
                    break;
                case TechTypes.ResearchPercentageBonus:
                    ResearchPercentageBonus = tech.GetPercentageBonus();
                    break;
                case TechTypes.TranscenditePercentageBonus:
                    TranscenditePercentageBonus = tech.GetPercentageBonus();
                    break;
                case TechTypes.CombatBonus:
                    CombatBonus = tech.GetCombatBonus();
                    break;
            }
        }
    }
    public void ReshuffleTechToPick()
    {
        TechsToPick.Clear();
        while(TechsToPick.Count < 3)
        {
            TechTypes techtype = (TechTypes)Random.Range(0, (int)TechTypes.LastVal);
            if(TechsToPick.Contains(techtype))
            {
                continue;
            }
            TechsToPick.Add(techtype);
        }
    }
    public int GetTechCostByType(TechTypes type, bool maintainance = false)
    {
        Tech current = null;
        bool haveAlready = false;
        foreach (Tech tech in Technologies)
        {
            if (tech.TechType == type)
            {
                current = tech;
                haveAlready = true;
                break;
            }
        }
        if (current == null)
        {
            current = new Tech(type);
        }
        int techcost = current.GetTechCost();
        if(maintainance)
        {
            techcost = current.GetMaintainanceCost();
        }
        if (haveAlready)
        {
            techcost = current.GetCostOfNextLevel();
            if (maintainance)
            {
                techcost = current.GetMaintainanceCost(1);
            }
        }
        return techcost;
    }
    public Tech GetTechByType(TechTypes techtype)
    {
        Tech current = null;
        foreach (Tech tech in Technologies)
        {
            if (tech.TechType == techtype)
            {
                current = tech;
                break;
            }
        }
        if (current == null)
        {
            current = new Tech(techtype);
        }
        return current;
    }
    public Tech GetCheapestTech()
    {
        Tech cheapest = null;
        int lowestCost = int.MaxValue;
        if(TechsToPick == null)
        {
            return cheapest;
        }    
        foreach(TechTypes techtype in TechsToPick)
        {
            Tech current = null;
            bool haveAlready = false;
            foreach (Tech tech in Technologies)
            {
                if (tech.TechType == techtype)
                {
                    current = tech;
                    haveAlready = true;
                    break;
                }
            }
            if(current == null)
            {
                current = new Tech(techtype);
            }
            int techcost = current.GetTechCost();
            if(haveAlready)
            {
                techcost = current.GetCostOfNextLevel();
            }
            if (techcost < lowestCost)
            {
                cheapest = current;
                lowestCost = techcost;
            }
        }
        return cheapest;
    }
    public bool ResearchTech(TechTypes type)
    {
        bool couldAfford = false;
        bool haveLowerLevelOfTech = false;
        foreach (Tech tech in Technologies)
        {
            if (tech.TechType == type)
            {
                haveLowerLevelOfTech = true;
                if (Research >= tech.GetCostOfNextLevel())
                {
                    tech.LevelUp();
                    Research -= tech.GetTechCost();
                    TechsToPick.Remove(type);
                    couldAfford = true;
                    Game.printToConsole(EmpireName + " researched " + type + " Level: " + tech.TechLevel);
                    ReshuffleTechToPick();
                    break;
                }
            }
        }
        if (haveLowerLevelOfTech == false)
        {
            Tech tech = new Tech(type);
            if (Research >= tech.GetTechCost())
            {
                Technologies.Add(tech);
                Research -= tech.GetTechCost();
                TechsToPick.Remove(type);
                couldAfford = true;
                Game.printToConsole(EmpireName + " researched " + type + " Level: " + tech.TechLevel);
                ReshuffleTechToPick();
            }
        }
        return couldAfford;
    }
}
