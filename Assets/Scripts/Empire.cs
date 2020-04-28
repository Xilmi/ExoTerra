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

    public void ProcessTurn()
    {
        Food += FoodPerTurn - FoodMaintainance;
        Minerals += MineralsPerTurn;
        Research += ResearchPerTurn;
        Transcendite += TranscenditePerTurn;
    }
    public void ResetPerTurnValues()
    {
        FoodPerTurn = 0;
        MineralsPerTurn = 0;
        ResearchPerTurn = 0;
        TranscenditePerTurn = 0;
        FoodMaintainance = 0;
    }
}
