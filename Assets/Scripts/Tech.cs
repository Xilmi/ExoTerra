using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TechTypes
{
    FlatMinerals,
    FlatTranscendite,
    EmpireMineralsPerTurn,
    EmpireResearchPerTurn,
    EmpireTranscenditePerTurn,
    HomeWorldBonus,
    MineMineralBonus,
    LabResearchBonus,
    TempleTranscenditeBonus,
    ProductionBonus,
    GreenPlanetBonus,
    BluePlanetBonus,
    DryPlanetBonus,
    MineralPercentageBonus,
    ResearchPercentageBonus,
    TranscenditePercentageBonus,
    CombatBonus,
    LastVal
}
public class Tech
{
    public TechTypes TechType;
    public int TechLevel;
    public bool CostsMaintainance;
    public bool OneTimeBonusAvailable = true;
    public Tech(TechTypes techtype, bool hasMaintainanceCost = true)
    {
        TechLevel = 1;
        TechType = techtype;
        CostsMaintainance = hasMaintainanceCost;
        if(TechType == TechTypes.FlatMinerals
            || TechType == TechTypes.FlatTranscendite)
        {
            CostsMaintainance = false;
        }
    }
    public int GetTechCost(int extraLevels = 0)
    {
        return (TechLevel + extraLevels) * 10;
    }
    public int GetCostOfNextLevel()
    {
        return GetTechCost(1);
    }
    public int GetMaintainanceCost(int extralevels = 0)
    {
        int cost = 0;
        if(CostsMaintainance)
        {
            cost = TechLevel + extralevels;
        }
        return cost;
    }
    public int GetFlatValue()
    {
        return GetTechCost() * 2;
    }
    public int GetEmpireValuePerTurn()
    {
        return TechLevel * 3;
    }
    public int GetSimpleBonus()
    {
        return TechLevel;
    }
    public float GetPercentageBonus()
    {
        return 0.1f * TechLevel;
    }
    public float GetCombatBonus()
    {
        return 0.5f * TechLevel;
    }
    public void LevelUp()
    {
        TechLevel++;
        OneTimeBonusAvailable = true;
    }
}
