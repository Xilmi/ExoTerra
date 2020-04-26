using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class Galaxy
{
    public List<Planet> Planets = new List<Planet>();
    public List<Empire> Empires = new List<Empire>();
    //width and hight in lightyers, density in stars per square-parsec (3x3 lightyears)
    public float GalaxyWidth;
    public float GalaxyHeight;
    public void Generate(float width, float height, float density)
    {
        GalaxyWidth = width;
        GalaxyHeight = height;
        int totalPlanetCount = Mathf.CeilToInt(width * height / 15.0f * density);
        int planetsGenerated = 0;
        while(planetsGenerated < totalPlanetCount)
        {
            Planet pl = new Planet();
            pl.Location.x = Random.Range(0f, width);
            pl.Location.y = Random.Range(0f, height);
            pl.Location.z = 0;
            bool shouldSkip = false;
            planetsGenerated++;
            foreach (Planet otherPlanet in Planets)
            {
                if(Vector3.Distance(pl.Location, otherPlanet.Location) < 2)
                {
                    shouldSkip = true;
                }
            }
            if(shouldSkip)
            {
                continue;
            }
            pl.PlanetSize = (PlanetSizes)Random.Range((int)PlanetSizes.Small, (int)PlanetSizes.LastVal);
            pl.PlanetSpecialization = PlanetSpecializations.Homeworld;
            pl.PlanetType = (PlanetTypes)Random.Range((int)PlanetTypes.Terran, (int)PlanetTypes.LastVal);
            pl.owner = null;
            Planets.Add(pl);
        }
        Empire empire = new Empire();
        empire.EmpireName = "Vegans";
        empire.RulerName = "Vegany";
        empire.HomeWold = "Vega";
        empire.iD = 0;
        empire.empireColor = Color.green;
        Empires.Add(empire);
        Empire empire2 = new Empire();
        empire2.EmpireName = "Mermaidians";
        empire2.RulerName = "Ari";
        empire2.HomeWold = "Liquor";
        empire2.iD = 1;
        empire2.empireColor = Color.blue;
        Empires.Add(empire2);
        Empire empire3 = new Empire();
        empire3.EmpireName = "Imperium";
        empire3.RulerName = "Jamelle";
        empire3.HomeWold = "Roma";
        empire3.iD = 2;
        empire3.empireColor = Color.magenta;
        Empires.Add(empire3);
        Empire empire4 = new Empire();
        empire4.EmpireName = "Femen";
        empire4.RulerName = "Alia";
        empire4.HomeWold = "Afrakis";
        empire4.iD = 3;
        empire4.empireColor = Color.yellow;
        Empires.Add(empire4);
    }
    public void ProcessTurn()
    {
        foreach(Planet planet in Planets)
        {
            if (planet.owner!= null)
            {
                if(planet.PlanetSpecialization == PlanetSpecializations.Homeworld)
                {
                    planet.owner.Food += 2;
                    planet.owner.Minerals += 2;
                    planet.owner.Research += 2;
                    planet.owner.Transcendite += 2;
                }
                if (planet.PlanetSpecialization == PlanetSpecializations.Farm)
                {
                    planet.owner.Food += 2 + (int)planet.PlanetSize;
                    if(planet.PlanetType == PlanetTypes.Terran || planet.PlanetType == PlanetTypes.Tundra)
                    {
                        planet.owner.Food += 2;
                    }
                }
                if (planet.PlanetSpecialization == PlanetSpecializations.Mine)
                {
                    planet.owner.Minerals += 2 + (int)planet.PlanetSize;
                    if (planet.PlanetType == PlanetTypes.Jungle || planet.PlanetType == PlanetTypes.Swamp)
                    {
                        planet.owner.Minerals += 2;
                    }
                }
                if (planet.PlanetSpecialization == PlanetSpecializations.Lab)
                {
                    planet.owner.Research += 2 + (int)planet.PlanetSize;
                    if (planet.PlanetType == PlanetTypes.Ocean || planet.PlanetType == PlanetTypes.Ice)
                    {
                        planet.owner.Research += 2;
                    }
                }
                if (planet.PlanetSpecialization == PlanetSpecializations.Temple)
                {
                    planet.owner.Transcendite += 2 + (int)planet.PlanetSize;
                    if (planet.PlanetType == PlanetTypes.Arid || planet.PlanetType == PlanetTypes.Desert)
                    {
                        planet.owner.Transcendite += 2;
                    }
                }
                planet.owner.Food -= 1;
            }
        }
        GameObject go = GameObject.Find("Score");
        TextMeshProUGUI tm = go.GetComponent<TextMeshProUGUI>();
        tm.text = Empires[0].EmpireName + " Food: " + Empires[0].Food;
    }
}
