using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FleetTypes
{
    Combat,
    Colonization
}
public class Fleet
{
    public GameObject guiFleet;
    public Empire owner;
    public int ShipCount;
    public Planet location;
    public Planet destination;
    public FleetTypes FleetType = FleetTypes.Combat;
    public int eta;

    public void Spawn(Planet planet, Planet dest = null, int shipCount = 1)
    {
        SetOwner(planet.owner);
        location = planet;
        ShipCount = shipCount;
        if (dest == null)
        {
            planet.FleetInOrbit = this;
        }
        else
        {
            destination = dest;
            eta = Mathf.CeilToInt(Vector3.Distance(location.Location, dest.Location));
        }
    }
    public void SpawnColonization(Planet source, Planet dest)
    {
        SetOwner(source.owner);
        location = source;
        destination = dest;
        ShipCount = 1;
        FleetType = FleetTypes.Colonization;
        eta = Mathf.CeilToInt(Vector3.Distance(source.Location, dest.Location));
    }
    public void SetOwner(Empire emp)
    {
        owner = emp;
    }
    public void processTurn()
    {
        if(eta > 0)
        {
            eta--;
            if(eta == 0)
            {
                location = destination;
                destination = null;
                if (FleetType == FleetTypes.Combat)
                {
                    if (location.FleetInOrbit != null)
                    {
                        if(location.FleetInOrbit.owner == owner)
                        {
                            location.FleetInOrbit.ShipCount += ShipCount;
                            ShipCount = 0;
                            location = null;
                        } 
                        else
                        { 
                            while(ShipCount > 0 && location.FleetInOrbit != null && location.FleetInOrbit.ShipCount > 0)
                            {
                                ShipCount--;
                                location.FleetInOrbit.ShipCount--;
                                if(location.FleetInOrbit.ShipCount <= 0)
                                {
                                    location.FleetInOrbit = null;
                                    break;
                                }
                            }
                            if(ShipCount > 0)
                            {
                                location.FleetInOrbit = this;
                            }
                        }
                    }
                    else
                    {
                        location.FleetInOrbit = this;
                    }
                }
                else
                {
                    if (location.FleetInOrbit != null && location.FleetInOrbit.owner != owner)
                    {
                        ShipCount = 0;
                    }
                    else
                    {
                        location.Colonize(owner);
                    }
                    location = null;
                }
            }
        }
    }
}
