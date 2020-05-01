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

    public void Spawn(Planet planet)
    {
        SetOwner(planet.owner);
        location = planet;
        ShipCount = 1;
        planet.FleetInOrbit = this;
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
        owner.MineralMaintainance += ShipCount;
        if(eta > 0)
        {
            eta--;
            if(eta == 0)
            {
                location = destination;
                destination = null;
                if (FleetType == FleetTypes.Combat)
                {
                    location.FleetInOrbit = this;
                }
                else
                {
                    location.Colonize(owner);
                    location = null;
                }
            }
        }
    }
}
