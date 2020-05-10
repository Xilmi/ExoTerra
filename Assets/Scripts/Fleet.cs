using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum FleetTypes
{
    Combat,
    Colonization,
    LastVal
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
    public List<Empire> VisibleTo = new List<Empire>();
    public void ClearVisibility()
    {
        VisibleTo.Clear();
    }
    public void UpdateVisibility(Empire emp, bool visible)
    {
        if (emp != null)
        {
            if (visible)
            {
                if (!VisibleTo.Contains(emp))
                {
                    VisibleTo.Add(emp);
                }
            }
            else
            {
                if (VisibleTo.Contains(emp))
                {
                    VisibleTo.Remove(emp);
                }
            }
        }
    }
    public void Spawn(Planet planet, Planet dest = null, int shipCount = 1)
    {
        SetOwner(planet.owner);
        UpdateVisibility(planet.owner, true);
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
    public void SpawnTransport(Planet source, Planet dest, FleetTypes type = FleetTypes.Colonization)
    {
        SetOwner(source.owner);
        location = source;
        destination = dest;
        ShipCount = 1;
        FleetType = type;
        eta = Mathf.CeilToInt(Vector3.Distance(source.Location, dest.Location));
    }
    public void SetOwner(Empire emp)
    {
        owner = emp;
    }
    public float GetPower()
    {
        float power = 0;
        if(FleetType == FleetTypes.Combat)
        {
            power = ShipCount * (1 + owner.CombatBonus);
        }
        return power;
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
                                float attackerPowerPerUnit = 1.0f + owner.CombatBonus;
                                float defenderPowerPerUnit = 1.0f + location.FleetInOrbit.owner.CombatBonus;
                                float attackerPower = ShipCount * attackerPowerPerUnit;
                                float defenderPower = location.FleetInOrbit.ShipCount * defenderPowerPerUnit;
                                if(attackerPower >= defenderPower)
                                {
                                    location.FleetInOrbit.ShipCount = 0;
                                    ShipCount = Mathf.RoundToInt((attackerPower - defenderPower) / attackerPowerPerUnit);
                                }
                                else
                                {
                                    ShipCount = 0;
                                    location.FleetInOrbit.ShipCount = Mathf.RoundToInt((defenderPower - attackerPower) / defenderPowerPerUnit);
                                }
                                if(location.FleetInOrbit.ShipCount <= 0)
                                {
                                    location.FleetInOrbit = null;
                                    break;
                                }
                            }
                            if(ShipCount > 0)
                            {
                                location.FleetInOrbit = this;
                                location.Explore(owner);
                            }
                        }
                    }
                    else
                    {
                        location.FleetInOrbit = this;
                        location.Explore(owner);
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
                        if (location.owner == null || (location.FleetInOrbit != null && location.FleetInOrbit.owner == owner))
                        {
                            location.Colonize(owner);
                        }
                        else
                        {
                            location.Clear();
                        }
                    }
                    location = null;
                }
            }
        }
    }
}
