using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Empire
{
    public string EmpireName;
    public string RulerName;
    public string HomeWold;
    public int iD;
    public int Food;
    public int FoodPerTurn;
    public int FoodMaintainance;
    public int Minerals;
    public int MineralsPerTurn;
    public int MineralMaintainance;
    public int Research;
    public int ResearchPerTurn;
    public int Transcendite;
    public int TranscenditePerTurn;
    public double FoodValue;
    public double MineralValue;
    public double ResearchValue;
    public double TranscenditeValue;
    public double OutpostValue;
    public Color empireColor;
    public bool isAIControlled;

    public void Init()
    {
        Food = 2;
        Minerals = 2;
        Research = 2;
        Transcendite = 2;
    }
    public void ProcessTurn()
    {
        Food -= FoodMaintainance;
        Minerals -= MineralMaintainance;
        Food = Mathf.Max(Food, 0);
        Minerals = Mathf.Max(Minerals, 0);
    }
    public void ResetPerTurnValues()
    {
        FoodPerTurn = 0;
        MineralsPerTurn = 0;
        ResearchPerTurn = 0;
        TranscenditePerTurn = 0;
        FoodMaintainance = 0;
        MineralMaintainance = 0;
    }
}
