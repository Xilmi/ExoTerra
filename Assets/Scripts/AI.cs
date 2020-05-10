using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    class Assignment
    {
        public Fleet fleet;
        public Planet planet;
        public int amount;
        public Assignment(Fleet fl, Planet pl, int ct)
        {
            fleet = fl;
            planet = pl;
            amount = ct;
        }
    }

    public Galaxy gal;
    public void executeAI(Empire emp)
    {
        DetermineValues(emp);
        PickTechnology(emp);
        HandleProduction(emp);
        SendFleets(emp);
        Colonize(emp);
    }
    public void HandleProduction(Empire emp)
    {
        foreach (Planet pl in gal.Planets)
        {
            if(pl.owner != emp
                || pl.GetProductionOutput(emp) < 1)
            {
                continue;
            }
            if (emp.MineralMaintainance < emp.MineralsPerTurn && emp.MineralValue < 1)
            {
                pl.producing = true;
            }
            else
            {
                pl.producing = false;
            }
        }
    }
    public void SendFleets(Empire emp)
    {
        List<Assignment> assignment = new List<Assignment>();
        List<Planet> servedAlready = new List<Planet>();
        foreach (Fleet fl in gal.Fleets)
        {
            if (fl.owner != emp
                || fl.destination == null
                || fl.FleetType != FleetTypes.Combat)
            {
                continue;
            }
            servedAlready.Add(fl.destination);
        }

        foreach (Fleet fl in gal.Fleets)
        {
            if(fl.owner != emp
                || fl.eta > 0)
            {
                continue;
            }
            Planet highestValueShiplessTarget = null;
            double highestValue = 0;
            foreach(Planet pl in gal.Planets)
            {
                if((pl.FleetInOrbit != null && pl.FleetInOrbit.owner == emp)
                    || servedAlready.Contains(pl))
                {
                    continue;
                }
                if(pl.exploredBy.Contains(emp) && pl.owner == null && pl.FleetInOrbit == null)
                { 
                    continue;
                }
                if(pl.owner == emp && pl.FleetInOrbit == null || pl.FleetInOrbit != null && pl.FleetInOrbit.owner == emp)
                {
                    continue;
                }
                if(pl.FleetInOrbit != null && pl.FleetInOrbit.owner != emp && pl.FleetInOrbit.GetPower() > fl.GetPower())
                {
                    continue;
                }
                double currentValue = GetPlanetValue(pl, emp);
                if(!pl.exploredBy.Contains(emp))
                {
                    Planet irrelevant;
                    currentValue /= ((gal.GetColonizationCost(pl, emp, out irrelevant) - 5) + Vector3.Distance(fl.location.Location, pl.Location));
                }
                else
                {
                    currentValue /= Vector3.Distance(fl.location.Location, pl.Location);
                }
                if (currentValue > highestValue)
                {
                    highestValue = currentValue;
                    highestValueShiplessTarget = pl;
                }
            }
            int amount = fl.ShipCount;
            if(fl.location.owner != null && fl.location.owner != fl.owner)
            {
                amount = fl.ShipCount - 1;
            }
            if (highestValueShiplessTarget != null)
            {
                Assignment ass = new Assignment(fl, highestValueShiplessTarget, amount);
                assignment.Add(ass);
                servedAlready.Add(highestValueShiplessTarget);
            }
        }
        foreach(Assignment ass in assignment)
        {
            gal.SendFleet(ass.fleet, ass.fleet.location, ass.planet, ass.amount);
        }
    }
    public void DetermineValues(Empire emp)
    {
        emp.RefreshPerTurnValues(gal);
        float mineralgoal = 0;
        float productionCapacity = 0;
        foreach (Planet pl in gal.Planets)
        {
            if(pl.owner != emp && pl.exploredBy.Contains(emp))
            {
                Planet irrelevant;
                mineralgoal += gal.GetColonizationCost(pl, emp, out irrelevant);
            }
            if(pl.owner == emp)
            {
                productionCapacity += pl.GetProductionOutput(emp);
            }
        }
        mineralgoal += productionCapacity;
        emp.MineralValue = Mathf.Max(mineralgoal, 15) / Mathf.Max(1, (emp.Minerals));
        emp.TranscenditeValue = Mathf.Max(1.0f, (float)gal.TranscenditeToWin / Mathf.Max(gal.Planets.Count * 5, ((float)emp.Transcendite * 2)));
        emp.ResearchValue = Mathf.Max(1.0f, emp.ResearchMaintainance * 2.0f / Mathf.Max(1.0f, emp.ResearchPerTurn));
        emp.OutpostValue = (emp.MineralsPerTurn - emp.MineralMaintainance) / Mathf.Max(1.0f, productionCapacity);
    }
    public void Colonize(Empire emp)
    {
        bool StillSeekPairings = true;
        while (StillSeekPairings)
        {
            double bestScore = 0;
            Planet bestSource = null;
            Planet bestDestination = null;
            int CostOfBest = 0;
            foreach(Planet colonizationTarget in gal.Planets)
            {
                if(!colonizationTarget.exploredBy.Contains(emp))
                {
                    continue;
                }
                if(colonizationTarget.owner == emp || gal.AlreadyBeingColonized(colonizationTarget, emp))
                {
                    continue;
                }
                if(colonizationTarget.owner != null && (colonizationTarget.FleetInOrbit == null || colonizationTarget.FleetInOrbit != null && colonizationTarget.FleetInOrbit.owner != emp))
                {
                    continue;
                }
                if(colonizationTarget.owner == null && colonizationTarget.FleetInOrbit != null && colonizationTarget.FleetInOrbit.owner != emp)
                {
                    continue;
                }
                Planet currentSource = null;
                int CurrentCost = gal.GetColonizationCost(colonizationTarget, emp, out currentSource);
                double score = PickSpecialization(colonizationTarget, emp, false) / CurrentCost;
                if(score > bestScore && currentSource != null)
                {
                    bestScore = score;
                    bestSource = currentSource;
                    bestDestination = colonizationTarget;
                    CostOfBest = CurrentCost;
                }
            }
            if(bestSource == null || bestDestination == null || emp.Minerals < CostOfBest)
            {
                StillSeekPairings = false;
            } 
            else
            {
                gal.Colonize(bestDestination, emp);
            }
        }
    }
    public double PickSpecialization(Planet planet, Empire emp, bool performSwitch = true)
    {
        PlanetSpecializations best = PlanetSpecializations.Mine;
        double bestScore = 0;
        double mineScore = emp.MineralValue * planet.GetMineralValueMultiplier(emp);
        bestScore = mineScore;
        double researchScore = emp.ResearchValue * planet.GetResearchValueMultiplier(emp);
        if(researchScore > bestScore)
        {
            bestScore = researchScore;
            best = PlanetSpecializations.Lab;
        }
        double transcenditeScore = emp.TranscenditeValue * planet.GetTranscenditeValueMultiplier(emp);
        if (transcenditeScore > bestScore)
        {
            bestScore = transcenditeScore;
            best = PlanetSpecializations.Temple;
        }
        double outpostScore = emp.OutpostValue * planet.GetOutpostValueMultiplier(emp);
        if (outpostScore > bestScore)
        {
            bestScore = outpostScore;
            best = PlanetSpecializations.Outpost;
        }
        if (performSwitch)
        {
            gal.SwitchSpecialization(planet, best, emp);
        }
        return bestScore;
    }
    public double GetPlanetValue(Planet planet, Empire emp)
    {
        double value = 0;
        if(planet.PlanetSpecialization == PlanetSpecializations.None || !planet.exploredBy.Contains(emp))
        {
            value += Mathf.Max((float)(planet.GetMineralValueMultiplier(emp) * emp.MineralValue),
                (float)(planet.GetOutpostValueMultiplier(emp) * emp.OutpostValue),
                (float)(planet.GetResearchValueMultiplier(emp) * emp.ResearchValue),
                (float)(planet.GetTranscenditeValueMultiplier(emp) * emp.TranscenditeValue));
        }
        else
        {
            value += planet.GetMineralOutput(emp) +
                planet.GetProductionOutput(emp) +
                planet.GetResearchOutput(emp) +
                planet.GetTranscenditeOutput(emp);
        }
        return value;
    }
    public double GetTechScore(Empire emp, TechTypes type)
    {
        double score = 0;
        float cost = emp.GetTechCostByType(type);
        Tech tech = new Tech(type);
        double roiMod = 3;
        switch (type)
        {
            case TechTypes.FlatMinerals:
                score = (tech.GetFlatValue() * emp.MineralValue) / (tech.GetTechCost() * emp.ResearchValue);
                break;
            case TechTypes.FlatTranscendite:
                score = (tech.GetFlatValue() * emp.TranscenditeValue) / (tech.GetTechCost() * emp.ResearchValue);
                break;
            case TechTypes.EmpireMineralsPerTurn:
                score = tech.GetEmpireValuePerTurn() * emp.MineralValue * roiMod / (cost * emp.ResearchValue);
                break;
            case TechTypes.EmpireResearchPerTurn:
                score = tech.GetEmpireValuePerTurn() * emp.ResearchValue * roiMod / (cost * emp.ResearchValue);
                break;
            case TechTypes.EmpireTranscenditePerTurn:
                score = tech.GetEmpireValuePerTurn() * emp.TranscenditeValue * roiMod / (cost * emp.ResearchValue);
                break;
            case TechTypes.HomeWorldBonus:
                score = tech.GetSimpleBonus() * emp.HomeWorldCount * roiMod * (emp.MineralValue + emp.ResearchValue + emp.TranscenditeValue + emp.OutpostValue) / (cost * emp.ResearchValue);
                break;
            case TechTypes.MineMineralBonus:
                score = tech.GetSimpleBonus() * emp.MineCount * roiMod * emp.MineralValue / (cost * emp.ResearchValue);
                break;
            case TechTypes.LabResearchBonus:
                score = tech.GetSimpleBonus() * emp.LabCount * roiMod * emp.ResearchValue / (cost * emp.ResearchValue);
                break;
            case TechTypes.TempleTranscenditeBonus:
                score = tech.GetSimpleBonus() * emp.TempleCount * roiMod * emp.TranscenditeValue / (cost * emp.ResearchValue);
                break;
            case TechTypes.ProductionBonus:
                score = tech.GetSimpleBonus() * emp.ProducerCount * roiMod * emp.OutpostValue / (cost * emp.ResearchValue);
                break;
            case TechTypes.GreenPlanetBonus:
                score = tech.GetSimpleBonus() * emp.GreenCount * roiMod * emp.MineralValue / (cost * emp.ResearchValue);
                break;
            case TechTypes.BluePlanetBonus:
                score = tech.GetSimpleBonus() * emp.BlueCount * roiMod * emp.ResearchValue/ (cost * emp.ResearchValue);
                break;
            case TechTypes.DryPlanetBonus:
                score = tech.GetSimpleBonus() * emp.DryCount * roiMod * emp.TranscenditeValue / (cost * emp.ResearchValue);
                break;
            case TechTypes.MineralPercentageBonus:
                score = emp.MineralsPerTurnBeforePercentageBonus * tech.GetPercentageBonus() * roiMod * emp.MineralValue / (cost * emp.ResearchValue);
                break;
            case TechTypes.ResearchPercentageBonus:
                score = emp.ResearchPerTurnBeforePercentageBonus * tech.GetPercentageBonus() * roiMod * emp.ResearchValue / (cost * emp.ResearchValue);
                break;
            case TechTypes.TranscenditePercentageBonus:
                score = emp.TranscenditePerTurnBeforePercentageBonus * tech.GetPercentageBonus() * roiMod * emp.TranscenditeValue / (cost * emp.ResearchValue);
                break;
            case TechTypes.CombatBonus:
                score = emp.ShipCount * tech.GetCombatBonus() / (cost * emp.ResearchValue);
                break;
        }
        return score;
    }
    public void PickTechnology(Empire emp)
    {
        bool couldAfford = true;
        while (couldAfford)
        {
            couldAfford = false;
            double highestScore = 0;
            TechTypes techToPick = TechTypes.LastVal;
            foreach (TechTypes techtype in emp.TechsToPick)
            {
                double currentScore = GetTechScore(emp, techtype);
                if (currentScore > highestScore)
                {
                    techToPick = techtype;
                    highestScore = currentScore;
                }
            }
            if (techToPick != TechTypes.LastVal && emp.Research >= emp.GetTechCostByType(techToPick))
            {
                couldAfford = emp.ResearchTech(techToPick);
            }
        }
    }
}
