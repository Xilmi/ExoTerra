using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    class Assignment
    {
        public Fleet fleet;
        public Planet planet;
        public float sendPercentage;
        public Assignment(Fleet fl, Planet pl, float perc)
        {
            fleet = fl;
            planet = pl;
            sendPercentage = perc;
        }
    }

    public Galaxy gal;
    public void executeAI(Empire emp)
    {
        DetermineValues(emp);
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
            if (emp.MineralValue < 1)
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
                || fl.eta > 0
                || fl.ShipCount < 2)
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
                double currentValue = GetPlanetValue(pl, emp);
                currentValue /= Vector3.Distance(fl.location.Location, pl.Location);
                if(currentValue > highestValue)
                {
                    highestValue = currentValue;
                    highestValueShiplessTarget = pl;
                }
            }
            float percentage = (fl.ShipCount - 1.0f) / (float)fl.ShipCount;
            Assignment ass = new Assignment(fl, highestValueShiplessTarget, percentage);
            assignment.Add(ass);
            servedAlready.Add(highestValueShiplessTarget);
        }
        foreach(Assignment ass in assignment)
        {
            gal.SendFleet(ass.fleet, ass.fleet.location, ass.planet, ass.sendPercentage);
        }
    }
    public void DetermineValues(Empire emp)
    {
        int mineralgoal = 0;
        int outposts = 1;
        int futureMineralsPerTurn = 0;
        foreach (Planet pl in gal.Planets)
        {
            if(pl.owner != emp)
            {
                mineralgoal += 5;
            }
            if(pl.owner == emp)
            {
                if(pl.PlanetSpecialization == PlanetSpecializations.Outpost || pl.SwitchingToSpecialization == PlanetSpecializations.Outpost)
                {
                    ++outposts;
                }
                if (pl.SwitchingToSpecialization == PlanetSpecializations.Mine)
                {
                    futureMineralsPerTurn += pl.GetMineralValueMultiplier(emp);
                }
            }
        }
        emp.MineralValue = mineralgoal / (emp.Minerals + (emp.MineralsPerTurn + futureMineralsPerTurn - emp.MineralMaintainance) * 10.0);
        emp.TranscenditeValue = 1;
        emp.ResearchValue = 1;
        emp.OutpostValue = gal.GetColonyCount(emp) * 0.2f / outposts;
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
                if(colonizationTarget.owner == emp || gal.AlreadyBeingColonized(colonizationTarget, emp))
                {
                    continue;
                }
                if(colonizationTarget.owner != null && (colonizationTarget.FleetInOrbit == null || colonizationTarget.FleetInOrbit != null && colonizationTarget.FleetInOrbit.owner != emp))
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
        if(planet.PlanetSpecialization == PlanetSpecializations.None)
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
            value /= 2.0;
        }
        return value;
    }
}
