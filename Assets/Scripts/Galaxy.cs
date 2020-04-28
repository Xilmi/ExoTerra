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
    public int MinTranscendence;
    public int TranscenditeToWin;
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
            pl.PlanetSpecialization = PlanetSpecializations.None;
            pl.PlanetType = (PlanetTypes)Random.Range((int)PlanetTypes.Terran, (int)PlanetTypes.LastVal);
            pl.owner = null;
            Planets.Add(pl);
        }
        Empire empire = new Empire();
        empire.EmpireName = "You";
        empire.RulerName = "Vegany";
        empire.HomeWold = "Vega";
        empire.iD = 0;
        empire.isAIControlled = false;
        empire.empireColor = Color.green;
        Empires.Add(empire);
        Empire empire2 = new Empire();
        empire2.EmpireName = "Mermaidians";
        empire2.RulerName = "Ari";
        empire2.HomeWold = "Liquor";
        empire2.iD = 1;
        empire2.empireColor = Color.blue;
        empire2.isAIControlled = true;
        Empires.Add(empire2);
        Empire empire3 = new Empire();
        empire3.EmpireName = "Imperium";
        empire3.RulerName = "Jamelle";
        empire3.HomeWold = "Roma";
        empire3.iD = 2;
        empire3.empireColor = Color.magenta;
        empire3.isAIControlled = true;
        Empires.Add(empire3);
        Empire empire4 = new Empire();
        empire4.EmpireName = "Femen";
        empire4.RulerName = "Alia";
        empire4.HomeWold = "Afrakis";
        empire4.iD = 3;
        empire4.empireColor = Color.yellow;
        empire4.isAIControlled = true;
        Empires.Add(empire4);
    }
    public void ProcessTurn()
    {
        foreach (Empire empire in Empires)
        {
            empire.ResetPerTurnValues();
        }
        bool setMinTranscendence = false;
        if(MinTranscendence == 0)
        {
            setMinTranscendence = true;
        }
        foreach (Planet planet in Planets)
        {
            if(setMinTranscendence == true)
            {
                MinTranscendence += 2;
            }
            if (planet.owner!= null)
            {
                planet.processTurn();
            }
        }
        int highestTranscendite = 0;
        int secondHighestTranscendite = MinTranscendence;
        Empire highestTranscenditeEmpire = null;
        foreach (Empire empire in Empires)
        {
            empire.ProcessTurn();
            if(empire.Transcendite > highestTranscendite)
            {
                highestTranscendite = empire.Transcendite;
                highestTranscenditeEmpire = empire;
            }
        }
        foreach(Empire sec in Empires)
        {
            if(sec == highestTranscenditeEmpire)
            {
                continue;
            }
            if(sec.Transcendite > secondHighestTranscendite)
            {
                secondHighestTranscendite = sec.Transcendite;
            }
        }
        TranscenditeToWin = Mathf.Max(secondHighestTranscendite * 2, MinTranscendence);
        updateGUI();
    }
    public bool Colonize(Planet planetToColonize, Empire empireToColonize)
    {
        bool colonizationSuccessfull = false;
        if (planetToColonize.owner != null)
        {
            Game.printToConsole("can't colonize planet as it's already owned");
            return false;
        }
        Planet closestBase;
        float minDistance = Mathf.Sqrt(Mathf.Pow(GalaxyWidth,2) + Mathf.Pow(GalaxyHeight,2));
        foreach (Planet pl in Planets)
        {
            if(pl.owner == empireToColonize)
            {
                if(pl.PlanetSpecialization == PlanetSpecializations.Homeworld || pl.PlanetSpecialization == PlanetSpecializations.Outpost)
                {
                    float currDistance = Vector3.Distance(pl.Location, planetToColonize.Location);
                    if(currDistance < minDistance)
                    {
                        closestBase = pl;
                        minDistance = currDistance;
                    }
                }
            }
        }
        int foodCost = Mathf.FloorToInt(minDistance);
        int mineralCost = 5;
        if(empireToColonize.Food < foodCost)
        {
            Game.printToConsole("Not enough food "+empireToColonize.Food+" of "+foodCost+" to colonize");
        }
        else if (empireToColonize.Minerals < mineralCost)
        {
            Game.printToConsole("Not enough minerals " + empireToColonize.Minerals + " of " + mineralCost + " to colonize");
        }
        else
        {
            planetToColonize.RemainingSwitchDuration = foodCost;
            planetToColonize.changeOwner(empireToColonize);
            empireToColonize.Food -= foodCost;
            empireToColonize.Minerals -= mineralCost;
            colonizationSuccessfull = true;
        }
        updateGUI();
        return colonizationSuccessfull;
    }
    public void updateGUI()
    {
        GameObject go = GameObject.Find("Score");
        TextMeshProUGUI tm = go.GetComponent<TextMeshProUGUI>();
        tm.text = "Food: " + Empires[0].Food + " (+" + Empires[0].FoodPerTurn + " -" + Empires[0].FoodMaintainance + ")"
            +"\nMinerals: " + Empires[0].Minerals + " (+" + Empires[0].MineralsPerTurn + ")" 
            + "\nScore: " + Empires[0].Transcendite + " (+" + Empires[0].TranscenditePerTurn + ")" + " / "+TranscenditeToWin+"\n"
            //+ "FoodValue: " + Empires[0].FoodValue +"\n"
            //+ "MineralValue: " + Empires[0].MineralValue + "\n"
            //+ "OutpostValue: " + Empires[0].OutpostValue + "\n"
            + Empires[1].EmpireName + " Score: " + Empires[1].Transcendite + " / " + TranscenditeToWin+ "\n"
            + Empires[2].EmpireName + " Score: " + Empires[2].Transcendite + " / " + TranscenditeToWin + "\n"
            + Empires[3].EmpireName + " Score: " + Empires[3].Transcendite + " / " + TranscenditeToWin + "\n";
        foreach (Planet pl in Planets)
        {
            pl.guiPlanet.transform.GetChild(0).GetComponent<TextMeshPro>().text = pl.PlanetType.ToString() + "\n";
            if (pl.owner == null)
            {
                continue;
            }
            if (pl.RemainingSwitchDuration > 0)
            {
                pl.guiPlanet.transform.GetChild(0).GetComponent<TextMeshPro>().text += pl.RemainingSwitchDuration.ToString() + " - " + pl.SwitchingToSpecialization;
            }
            else
            {
                pl.guiPlanet.transform.GetChild(0).GetComponent<TextMeshPro>().text += pl.PlanetSpecialization.ToString() + " +"+pl.Output;
            }
        }
    }
    public int GetColonyCount(Empire emp)
    {
        int count = 0;
        foreach(Planet pl in Planets)
        {
            if(pl.owner == emp)
            {
                count++;
            }
        }
        return count;
    }
    public void SwitchSpecialization(Planet pl, PlanetSpecializations spec, Empire emp)
    {
        if (pl != null)
        {
            if (pl.owner == emp)
            {
                if (pl.RemainingSwitchDuration == 0 || pl.PlanetSpecialization == PlanetSpecializations.None)
                {
                    if (pl.PlanetSpecialization != PlanetSpecializations.Homeworld)
                    {
                        pl.SwitchingToSpecialization = spec;
                        if (pl.PlanetSpecialization != PlanetSpecializations.None)
                        {
                            pl.RemainingSwitchDuration = GetColonyCount(emp) - 1;
                        }
                    }
                }
            }
        }
    }
}
