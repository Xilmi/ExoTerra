﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Game : MonoBehaviour
{
    Galaxy Galaxy = new Galaxy();
    public AI AI = new AI();
    public GameObject planetTemplate;
    public GameObject fleetTemplate;
    public GameObject colonyShipTemplate;
    public GameObject SelectionTemplate;
    public GameObject buttonPrefab;
    public GameObject controlPanel;
    public GameObject specButton;
    public GameObject produceButton;
    public GameObject detailText;
    public GameObject notificationText;
    public GameObject researchPicker;
    public GameObject researchButton;
    public GameObject slider;
    public Planet lastSelected = null;
    public Fleet lastSelectedFleet = null;
    public bool ForcePanelUpdate = false;
    TextAssignments translator;
    // Start is called before the first frame update
    void Start()
    {
        translator = new TextAssignments();
        translator.AddAssignment(TechTypes.BluePlanetBonus.ToString(), "Aquanautics");
        translator.AddAssignment(TechTypes.DryPlanetBonus.ToString(), "Ariditics");
        translator.AddAssignment(TechTypes.GreenPlanetBonus.ToString(), "Biology");
        translator.AddAssignment(TechTypes.EmpireMineralsPerTurn.ToString(), "Planned Economy");
        translator.AddAssignment(TechTypes.EmpireResearchPerTurn.ToString(), "Technocracy");
        translator.AddAssignment(TechTypes.EmpireTranscenditePerTurn.ToString(), "Astrology");
        translator.AddAssignment(TechTypes.FlatMinerals.ToString(), "Deposits");
        translator.AddAssignment(TechTypes.FlatResearch.ToString(), "Recordings");
        translator.AddAssignment(TechTypes.FlatTranscendite.ToString(), "Scriptures");
        translator.AddAssignment(TechTypes.HomeWorldBonus.ToString(), "Autonomy");
        translator.AddAssignment(TechTypes.MineMineralBonus.ToString(), "Logistics");
        translator.AddAssignment(TechTypes.LabResearchBonus.ToString(), "Scholarship");
        translator.AddAssignment(TechTypes.TempleTranscenditeBonus.ToString(), "Spirituality");
        translator.AddAssignment(TechTypes.MineralPercentageBonus.ToString(), "Simplifications");
        translator.AddAssignment(TechTypes.ResearchPercentageBonus.ToString(), "Mathematics");
        translator.AddAssignment(TechTypes.TranscenditePercentageBonus.ToString(), "Propaganda");
        translator.AddAssignment(TechTypes.ProductionBonus.ToString(), "Starports");
        Galaxy.Generate(30, 30, 1f);
        Galaxy.parent = this;
        AI.gal = Galaxy;
        SelectionTemplate.SetActive(false);
        controlPanel.SetActive(false);
        notificationText.SetActive(false);
        researchPicker.SetActive(false);
        researchButton.SetActive(false);
        slider.SetActive(false);
        int Homeworldsassigned = 0;
        foreach (Planet planet in Galaxy.Planets)
        {
            GameObject goPlanet;
            goPlanet = Instantiate(planetTemplate);
            planet.guiPlanet = goPlanet;
            float xPositionOnGrid = planet.Location.x - Galaxy.GalaxyWidth / 2.0f;
            float yPositionOnGrid = planet.Location.y - Galaxy.GalaxyHeight / 2.0f;
            goPlanet.transform.Translate(xPositionOnGrid, yPositionOnGrid, 0f);
            float scaling = 0.4f + (float)planet.PlanetSize * 0.1f;
            goPlanet.transform.localScale *= scaling;
            string SpriteString = "";
            if (planet.PlanetType == PlanetTypes.Terran)
            {
                SpriteString = "planet_045";
            }
            if (planet.PlanetType == PlanetTypes.Ocean)
            {
                SpriteString = "planet_048";
            }
            if (planet.PlanetType == PlanetTypes.Desert)
            {
                SpriteString = "planet_024";
            }
            if (planet.PlanetType == PlanetTypes.Tundra)
            {
                SpriteString = "planet_077";
            }
            if (planet.PlanetType == PlanetTypes.Ice)
            {
                SpriteString = "planet_051";
            }
            if (planet.PlanetType == PlanetTypes.Barren)
            {
                SpriteString = "planet_066";
            }
            Sprite sprite = Resources.Load<Sprite>("105 Colorful 2D Planet Icons/" + SpriteString);
            goPlanet.GetComponent<SpriteRenderer>().sprite = sprite;
            if (Homeworldsassigned < Galaxy.Empires.Count)
            {
                planet.setHomeworld(Galaxy.Empires[Homeworldsassigned]);
                planet.processTurn();
                Galaxy.CreateFleet(Galaxy.Empires[Homeworldsassigned], planet);
                if (!Galaxy.Empires[Homeworldsassigned].isAIControlled)
                {
                    CenterView(planet.guiPlanet.transform.position);
                }
                ++Homeworldsassigned;
            }
        }
        foreach (Fleet fleet in Galaxy.Fleets)
        {
            fleet.processTurn();
        }
        foreach (Empire emp in Galaxy.Empires)
        {
            emp.Init();
        }
        DrawFleets();
    }

    // Update is called once per frame
    void Update()
    {
        bool haveToUpdatePanel = ForcePanelUpdate;
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && SelectionTemplate.activeSelf == false && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject[] gos = GameObject.FindGameObjectsWithTag("Planet");
            GameObject[] gofleets = GameObject.FindGameObjectsWithTag("Fleet");
            GameObject goSelectedPlanet = null;
            GameObject goSelectedFleet = null;
            GameObject goRightClicked = null;
            foreach (GameObject go in gos)
            {
                if (go.GetComponent<CircleCollider2D>().OverlapPoint(wp))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        goSelectedPlanet = go;
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        goRightClicked = go;
                    }
                }
            }
            foreach (GameObject go in gofleets)
            {
                if (go.GetComponent<BoxCollider2D>().OverlapPoint(wp))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        goSelectedFleet = go;
                    }
                }
            }
            if (goSelectedPlanet || goSelectedFleet || goRightClicked)
            {
                foreach (Planet planet in Galaxy.Planets)
                {
                    if (goSelectedPlanet)
                    {
                        Behaviour b = (Behaviour)planet.guiPlanet.GetComponent("Halo");
                        b.enabled = false;
                    }
                    if (planet.guiPlanet == goSelectedPlanet)
                    {
                        lastSelected = planet;
                        haveToUpdatePanel = true;
                        if (planet.owner != null)
                        {
                            print("Planet: Size: " + planet.PlanetSize + " Type: " + planet.PlanetType + " Owner: " + planet.owner.EmpireName + " iD: " + planet.owner.iD);
                        }
                        else
                        {
                            print("Planet: Size: " + planet.PlanetSize + " Type: " + planet.PlanetType + " Owner: uninhabitet");
                        }
                    }
                    if(planet.guiPlanet == goRightClicked)
                    {
                        if(lastSelectedFleet != null && lastSelectedFleet.owner == Galaxy.Empires[0] && lastSelectedFleet.eta == 0)
                        {
                            Galaxy.SendFleet(lastSelectedFleet, lastSelectedFleet.location, planet, slider.GetComponent<Slider>().value);
                        }
                    }
                }
                foreach (Fleet fleet in Galaxy.Fleets)
                {
                    if (goSelectedFleet)
                    {
                        Behaviour b = (Behaviour)fleet.guiFleet.GetComponent("Halo");
                        b.enabled = false;
                    }
                    if (fleet.guiFleet == goSelectedFleet)
                    {
                        lastSelectedFleet = fleet;
                        haveToUpdatePanel = true;
                        print("Registered click on fleet");
                    }
                }
            }
            else if(Input.GetMouseButtonDown(1))
            {
                CenterView(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        Camera cam = GameObject.FindObjectOfType<Camera>();
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (cam.orthographicSize > 1)
            {
                cam.orthographicSize--; ;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cam.orthographicSize < 100)
            {
                cam.orthographicSize++; ;
            }
        }
        if (lastSelected != null && haveToUpdatePanel)
        {
            ForcePanelUpdate = false;
            Behaviour b = (Behaviour)lastSelected.guiPlanet.GetComponent("Halo");
            controlPanel.SetActive(true);
            TextMeshProUGUI tmp = detailText.GetComponent<TextMeshProUGUI>();
            tmp.text = lastSelected.PlanetSize + "\n"
                + lastSelected.PlanetType + "\n"
                + lastSelected.PlanetSpecialization + "\n";
            TextMeshProUGUI cButton = specButton.GetComponentInChildren<TextMeshProUGUI>();
            specButton.SetActive(true);
            TextMeshProUGUI pButton = produceButton.GetComponentInChildren<TextMeshProUGUI>();
            if (lastSelected.owner == Galaxy.Empires[0])
            {
                cButton.text = "Change";
                if (lastSelected.PlanetSpecialization != PlanetSpecializations.Outpost && lastSelected.PlanetSpecialization != PlanetSpecializations.Homeworld)
                {
                    produceButton.SetActive(false);
                }
                else
                {
                    if (lastSelected.PlanetSpecialization == PlanetSpecializations.Homeworld)
                    {
                        specButton.SetActive(false);
                    }
                    produceButton.SetActive(true);
                }
                if (lastSelected.producing)
                {
                    pButton.text = "Stop Producing";
                }
                else
                {
                    pButton.text = "Start Producing";
                }
            }
            else
            {
                produceButton.SetActive(false);
                if (Galaxy.AlreadyBeingColonized(lastSelected, Galaxy.Empires[0]))
                {
                    specButton.SetActive(false);
                }
                else
                {
                    if (lastSelected.owner == null)
                    {
                        cButton.text = "Colonize";
                    }
                    else if(lastSelected.FleetInOrbit != null && lastSelected.FleetInOrbit.owner == Galaxy.Empires[0])
                    {
                        cButton.text = "Invade";
                    }
                    else
                    {
                        //cButton.text = "Invade";
                        specButton.SetActive(false);
                    }
                }
            }
            b.enabled = true;
        }
        if (lastSelectedFleet != null && haveToUpdatePanel)
        {
            Behaviour b = (Behaviour)lastSelectedFleet.guiFleet.GetComponent("Halo");
            b.enabled = true;
        }
        if(lastSelectedFleet != null && lastSelectedFleet.eta == 0)
        {
            slider.SetActive(true);
        }
        else
        {
            slider.SetActive(false);
        }
        if(Galaxy.Empires[0].Research >= Galaxy.Empires[0].GetCheapestTech().GetTechCost())
        {
            researchButton.SetActive(true);
        }
        else
        {
            researchButton.SetActive(false);
        }
    }

    public void ProcessTurn()
    {
        foreach (Empire emp in Galaxy.Empires)
        {
            if (emp.isAIControlled)
            {
                AI.executeAI(emp);
            }
        }
        Galaxy.ProcessTurn();
        if(Galaxy.GetWinner() != null)
        {
            notificationText.GetComponent<TextMeshProUGUI>().text = Galaxy.GetWinner().EmpireName + " has won!";
            notificationText.SetActive(true);
        }
        DrawFleets();
    }
    static public void printToConsole(string textToPrint)
    {
        print(textToPrint);
    }

    public void OnFactoryButtonClicked()
    {
        SwitchSpecialization(lastSelected, PlanetSpecializations.Outpost);
    }
    public void OnMineButtonClicked()
    {
        SwitchSpecialization(lastSelected, PlanetSpecializations.Mine);
    }
    public void OnLabButtonClicked()
    {
        SwitchSpecialization(lastSelected, PlanetSpecializations.Lab);
    }
    public void OnTempleButtonClicked()
    {
        SwitchSpecialization(lastSelected, PlanetSpecializations.Temple);
    }

    public void SwitchSpecialization(Planet lastSelected, PlanetSpecializations spec)
    {
        Galaxy.SwitchSpecialization(lastSelected, spec, Galaxy.Empires[0]);
        SelectionTemplate.SetActive(false);
        Galaxy.updateGUI();
    }

    public void OnColonizeButtonClicked()
    {
        if (lastSelected.owner != Galaxy.Empires[0])
        {
            Galaxy.Colonize(lastSelected, Galaxy.Empires[0]);
        }
        if (lastSelected.owner == Galaxy.Empires[0])
        {
            if (lastSelected.RemainingSwitchDuration == 0)
            {
                SelectionTemplate.SetActive(true);
            }
        }
        ForcePanelUpdate = true;
    }

    public void OnProduceButtonClicked()
    {
        if (lastSelected.owner == Galaxy.Empires[0])
        {
            TextMeshProUGUI buttonText = produceButton.GetComponentInChildren<TextMeshProUGUI>();
            if (lastSelected.producing)
            {
                lastSelected.producing = false;
                buttonText.text = "Start Producing";
            }
            else
            {
                lastSelected.producing = true;
                buttonText.text = "Stop Producing";
            }
            Galaxy.updateGUI();
        }
    }

    public void DrawFleets()
    {
        foreach (Fleet fleet in Galaxy.Fleets)
        {
            if (fleet.guiFleet == null)
            {
                GameObject goFleet;
                if (fleet.FleetType == FleetTypes.Combat)
                {
                    goFleet = Instantiate(fleetTemplate);
                }
                else
                {
                    goFleet = Instantiate(colonyShipTemplate);
                }
                fleet.guiFleet = goFleet;
            }
            Vector3 fleetOffset = new Vector3(1f, 1f, 0f);
            if (fleet.location != null)
            {
                fleet.guiFleet.transform.position = fleet.location.guiPlanet.transform.position + fleetOffset;
                fleet.guiFleet.GetComponent<SpriteRenderer>().color = fleet.owner.empireColor;
            }
        }
        Galaxy.updateGUI();
    }
    public void CenterView(Vector3 centerPosition)
    {
        Vector2 wp = centerPosition;
        Vector3 newPosition = wp;
        newPosition.z = -10;
        Camera.main.transform.position = newPosition;
    }
    public void PickResearch()
    {
        if (researchPicker.activeSelf == false)
        {
            researchPicker.SetActive(true);
            foreach (TechTypes techtype in Galaxy.Empires[0].TechsToPick)
            {
                GameObject go = Instantiate(buttonPrefab);
                go.transform.SetParent(researchPicker.transform);
                go.GetComponentInChildren<Text>().text = translator.GetTextFor(techtype.ToString()) + " "+ Galaxy.Empires[0].GetTechCostByType(techtype)+" ("+ Galaxy.Empires[0].GetTechCostByType(techtype, true)+")";
                go.name = techtype.ToString();
                go.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(go.name));
            }
        }
        else
        {
            researchPicker.SetActive(false);
            DestroyResearchPickerChildren();
        }
    }
    public void OnButtonClick(string name)
    {
        Galaxy.Empires[0].ResearchTech((TechTypes)Enum.Parse(typeof(TechTypes), name));
        researchPicker.SetActive(false);
        DestroyResearchPickerChildren();
        Galaxy.Empires[0].RefreshPerTurnValues(Galaxy);
        if (Galaxy.Empires[0].Research >= Galaxy.Empires[0].GetCheapestTech().GetTechCost())
        {
            PickResearch();
        }
        Galaxy.updateGUI();
    }
    public void DestroyResearchPickerChildren()
    {
        foreach (Transform child in researchPicker.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
