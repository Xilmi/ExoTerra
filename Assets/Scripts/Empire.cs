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
        PickRandomTech();
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
        GetTechBonuses();
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
            }
        }
        foreach (Fleet fl in gal.Fleets)
        {
            if (fl.owner == this)
            {
                MineralMaintainance += fl.ShipCount;
            }
        }
        MineralsPerTurn += EmpireMineralsPerTurn;
        ResearchPerTurn += EmpireResearchPerTurn;
        TranscenditePerTurn += EmpireTranscenditePerTurn;
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
                case TechTypes.FlatResearch:
                    if (tech.OneTimeBonusAvailable)
                    {
                        Research += tech.GetFlatValue();
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
    public void PickRandomTech()
    {
        if(isAIControlled == false)
        {
            return;
        }
        bool canTryAgain = true;
        while (canTryAgain)
        {
            canTryAgain = false;
            if (TechsToPick.Count > 0)
            {
                TechTypes techtype = TechsToPick[Random.Range(0, TechsToPick.Count)];
                bool haveLowerLevelOfTech = false;
                foreach (Tech tech in Technologies)
                {
                    if (tech.TechType == techtype)
                    {
                        haveLowerLevelOfTech = true;
                        if (Research >= tech.GetCostOfNextLevel())
                        {
                            tech.LevelUp();
                            Research -= tech.GetTechCost();
                            Game.printToConsole(EmpireName + " has researched " + techtype + " Level " + tech.TechLevel);
                            TechsToPick.Remove(techtype);
                            canTryAgain = true;
                            break;
                        }
                    }
                }
                if (haveLowerLevelOfTech == false)
                {
                    Tech tech = new Tech(techtype);
                    if (Research >= tech.GetTechCost())
                    {
                        Technologies.Add(tech);
                        Research -= tech.GetTechCost();
                        Game.printToConsole(EmpireName + " has researched " + techtype + " Level " + tech.TechLevel);
                        TechsToPick.Remove(techtype);
                        canTryAgain = true;
                    }
                }
            }
            if (TechsToPick.Count <= 0)
            {
                ReshuffleTechToPick();
            }
        }
    }
    public void ResearchTech(TechTypes type)
    {
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
                ReshuffleTechToPick();
            }
        }
    }
}
