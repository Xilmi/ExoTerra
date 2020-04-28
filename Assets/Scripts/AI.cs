using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    public Galaxy gal;
    public void executeAI(Empire emp)
    {
        DetermineValues(emp);
        Colonize(emp);
    }
    public void DetermineValues(Empire emp)
    {
        int foodgoal = 0;
        int mineralgoal = 0;
        int outposts = 1;
        int futureFoodPerTurn = 0;
        int futureMineralsPerTurn = 0;
        foreach (Planet pl in gal.Planets)
        {
            if(pl.owner != emp)
            {
                foodgoal += 5;
                mineralgoal += 5;
            }
            if(pl.owner == emp)
            {
                if(pl.PlanetSpecialization == PlanetSpecializations.Outpost || pl.SwitchingToSpecialization == PlanetSpecializations.Outpost)
                {
                    ++outposts;
                }
                if(pl.SwitchingToSpecialization == PlanetSpecializations.Farm)
                {
                    futureFoodPerTurn += Mathf.RoundToInt((float)pl.GetFoodValueMultiplier() * 2.0f);
                }
                if (pl.SwitchingToSpecialization == PlanetSpecializations.Mine)
                {
                    futureMineralsPerTurn += Mathf.RoundToInt((float)pl.GetMineralValueMultiplier() * 2.0f);
                }
            }
        }
        emp.FoodValue = foodgoal / (emp.Food + (emp.FoodPerTurn + futureFoodPerTurn - emp.FoodMaintainance) * 10.0);
        emp.MineralValue = mineralgoal / (emp.Minerals + (emp.MineralsPerTurn + futureMineralsPerTurn) * 10.0);
        emp.TranscenditeValue = 1;
        emp.ResearchValue = 0;
        emp.OutpostValue = gal.GetColonyCount(emp) * 0.2f / outposts;
    }
    public void Colonize(Empire emp)
    {
        bool StillSeekPairings = true;
        while (StillSeekPairings)
        {
            float bestPairDistance = Mathf.Sqrt(Mathf.Pow(gal.GalaxyWidth, 2) + Mathf.Pow(gal.GalaxyHeight, 2));
            Planet bestSource = null;
            Planet bestDestination = null;
            foreach (Planet pl in gal.Planets)
            {
                if(pl.owner != emp)
                {
                    continue;
                }
                if(pl.PlanetSpecialization != PlanetSpecializations.Homeworld && pl.PlanetSpecialization != PlanetSpecializations.Outpost)
                {
                    continue;
                }
                foreach(Planet colonizationTarget in gal.Planets)
                {
                    if(colonizationTarget.owner != null)
                    {
                        continue;
                    }
                    float currentDistance = Vector3.Distance(pl.Location, colonizationTarget.Location);
                    if(currentDistance < bestPairDistance)
                    {
                        bestPairDistance = currentDistance;
                        bestSource = pl;
                        bestDestination = colonizationTarget;
                    }
                }
            }
            if(bestSource == null || bestDestination == null || bestPairDistance > emp.Food || emp.Minerals < 5)
            {
                StillSeekPairings = false;
            } 
            else
            {
                gal.Colonize(bestDestination, emp);
                double FoodValue = emp.FoodValue * bestDestination.GetFoodValueMultiplier();
                double MineralValue = emp.MineralValue * bestDestination.GetMineralValueMultiplier();
                double ResearchValue = emp.ResearchValue * bestDestination.GetResearchValueMultiplier();
                double TranscenditeValue = emp.TranscenditeValue * bestDestination.GetTranscenditeValueMultiplier();
                double OutpostValue = emp.OutpostValue;
                PlanetSpecializations spec = PlanetSpecializations.None;
                if(FoodValue >= MineralValue && FoodValue >= ResearchValue && FoodValue >= TranscenditeValue && FoodValue >= OutpostValue)
                {
                    spec = PlanetSpecializations.Farm;
                }
                if (MineralValue >= FoodValue && MineralValue >= ResearchValue && MineralValue >= TranscenditeValue && MineralValue >= OutpostValue)
                {
                    spec = PlanetSpecializations.Mine;
                }
                if (ResearchValue >= MineralValue && ResearchValue >= FoodValue && ResearchValue >= TranscenditeValue && ResearchValue >= OutpostValue)
                {
                    spec = PlanetSpecializations.Lab;
                }
                if (TranscenditeValue >= MineralValue && TranscenditeValue >= ResearchValue && TranscenditeValue >= FoodValue && TranscenditeValue >= OutpostValue)
                {
                    spec = PlanetSpecializations.Temple;
                }
                if (OutpostValue >= MineralValue && OutpostValue >= ResearchValue && OutpostValue >= FoodValue && OutpostValue >= TranscenditeValue)
                {
                    spec = PlanetSpecializations.Outpost;
                }
                gal.SwitchSpecialization(bestDestination, spec, emp);
            }
        }
    }
}
