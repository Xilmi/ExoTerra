using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    Galaxy Galaxy = new Galaxy();
    AI AI = new AI();
    public GameObject planetTemplate;
    public GameObject SelectionTemplate;
    Planet lastSelected;
    // Start is called before the first frame update
    void Start()
    {
        Galaxy.Generate(30,30,1f);
        AI.gal = Galaxy;
        SelectionTemplate.SetActive(false);
        int Homeworldsassigned = 0;
        foreach(Planet planet in Galaxy.Planets)
        {
            GameObject goPlanet;
            goPlanet = Instantiate(planetTemplate);
            planet.guiPlanet = goPlanet;
            print("Planet generated at: " + planet.Location.x + " " + planet.Location.y+ " with size: "+ planet.PlanetSize);
            float xPositionOnGrid = planet.Location.x - Galaxy.GalaxyWidth / 2.0f;
            float yPositionOnGrid = planet.Location.y - Galaxy.GalaxyHeight / 2.0f;
            goPlanet.transform.Translate(xPositionOnGrid, yPositionOnGrid, 0f);
            float scaling = 0.4f + (float)planet.PlanetSize * 0.1f;
            goPlanet.transform.localScale *= scaling;
            //string SpriteString = "";
            //if (planet.PlanetType == PlanetTypes.Terran)
            //{
            //    SpriteString = "planet_045";
            //}
            //if (planet.PlanetType == PlanetTypes.Ocean)
            //{
            //    SpriteString = "planet_048";
            //}
            //if (planet.PlanetType == PlanetTypes.Jungle)
            //{
            //    SpriteString = "planet_073";
            //}
            //if (planet.PlanetType == PlanetTypes.Arid)
            //{
            //    SpriteString = "planet_047";
            //}
            //if (planet.PlanetType == PlanetTypes.Tundra)
            //{
            //    SpriteString = "planet_058";
            //}
            //if (planet.PlanetType == PlanetTypes.Ice)
            //{
            //    SpriteString = "planet_051";
            //}
            //if (planet.PlanetType == PlanetTypes.Desert)
            //{
            //    SpriteString = "planet_024";
            //}
            //if (planet.PlanetType == PlanetTypes.Swamp)
            //{
            //    SpriteString = "planet_076";
            //}
            Sprite sprite = Resources.Load<Sprite>("105 Colorful 2D Planet Icons/" + "planet_051");
            goPlanet.GetComponent<SpriteRenderer>().sprite = sprite;
            if(Homeworldsassigned < Galaxy.Empires.Count)
            {
                planet.setHomeworld(Galaxy.Empires[Homeworldsassigned]);
                planet.processTurn();
                ++Homeworldsassigned;
            }
        }
        Galaxy.updateGUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && SelectionTemplate.activeSelf == false)
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject[] gos = GameObject.FindGameObjectsWithTag("Planet");
            GameObject goClicked = null;
            foreach (GameObject go in gos)
            {
                if (go.GetComponent<CircleCollider2D>().OverlapPoint(wp))
                {
                    goClicked = go;
                }
            }
            if (goClicked)
            {
                foreach (Planet planet in Galaxy.Planets)
                {
                    Behaviour b = (Behaviour)planet.guiPlanet.GetComponent("Halo");
                    b.enabled = false;
                    if (planet.guiPlanet == goClicked)
                    {
                        lastSelected = planet;
                        if (Galaxy.Colonize(planet, Galaxy.Empires[0]))
                        {
                            SelectionTemplate.SetActive(true);
                        }
                        if (planet.owner != null)
                        {
                            print("Planet: Size: " + planet.PlanetSize + " Type: " + planet.PlanetType + " Owner: " + planet.owner.EmpireName + " iD: " + planet.owner.iD);
                        }
                        else
                        {
                            print("Planet: Size: " + planet.PlanetSize + " Type: " + planet.PlanetType + " Owner: uninhabitet");
                        }
                    }
                }
            }
        }
        Camera cam = GameObject.FindObjectOfType<Camera>();
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = wp;
            newPosition.z = -10;
            cam.transform.position = newPosition;
        }
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
        if (lastSelected != null)
        {
            Behaviour b = (Behaviour)lastSelected.guiPlanet.GetComponent("Halo");
            b.enabled = true;
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
    }
    static public void printToConsole(string textToPrint)
    {
        print(textToPrint);
    }

    public void OnFarmButtonClicked()
    {
        SwitchSpecialization(lastSelected, PlanetSpecializations.Farm);
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
}
