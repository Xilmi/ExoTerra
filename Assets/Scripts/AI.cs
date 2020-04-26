using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    public Galaxy gal;
    public void executeAI(Empire emp)
    {
        Colonize(emp);
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
                if(pl.PlanetSpecialization != PlanetSpecializations.Homeworld && pl.PlanetSpecialization != PlanetSpecializations.Factory)
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
            }
        }
    }
}
