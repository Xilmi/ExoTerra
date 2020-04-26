using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    Galaxy Galaxy = new Galaxy();
    AI AI = new AI();
    public GameObject planetTemplate;
    public Text textTemplate;
    // Start is called before the first frame update
    void Start()
    {
        Galaxy.Generate(30,30,1f);
        AI.gal = Galaxy;
        int Homeworldsassigned = 0;
        foreach(Planet planet in Galaxy.Planets)
        {
            GameObject goPlanet;
            Text goText;
            goPlanet = Instantiate(planetTemplate);
            goText = Instantiate(textTemplate);
            goText.text = "PlanetName";
            planet.guiPlanet = goPlanet;
            planet.guiText = goText;
            print("Planet generated at: " + planet.Location.x + " " + planet.Location.y+ " with size: "+ planet.PlanetSize);
            float xPositionOnGrid = planet.Location.x - Galaxy.GalaxyWidth / 2.0f;
            float yPositionOnGrid = planet.Location.y - Galaxy.GalaxyHeight / 2.0f;
            goPlanet.transform.Translate(xPositionOnGrid, yPositionOnGrid, 0f);
            float scaling = 0.25f + (float)planet.PlanetSize * 0.25f;
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
            if (planet.PlanetType == PlanetTypes.Jungle)
            {
                SpriteString = "planet_073";
            }
            if (planet.PlanetType == PlanetTypes.Arid)
            {
                SpriteString = "planet_047";
            }
            if (planet.PlanetType == PlanetTypes.Tundra)
            {
                SpriteString = "planet_058";
            }
            if (planet.PlanetType == PlanetTypes.Ice)
            {
                SpriteString = "planet_051";
            }
            if (planet.PlanetType == PlanetTypes.Desert)
            {
                SpriteString = "planet_024";
            }
            if (planet.PlanetType == PlanetTypes.Swamp)
            {
                SpriteString = "planet_076";
            }
            Sprite sprite = Resources.Load<Sprite>("105 Colorful 2D Planet Icons/" + "planet_051");
            goPlanet.GetComponent<SpriteRenderer>().sprite = sprite;
            if(Homeworldsassigned < Galaxy.Empires.Count)
            {
                planet.setHomeworld(Galaxy.Empires[Homeworldsassigned]);
                ++Homeworldsassigned;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Planet pl in Galaxy.Planets)
        {
            pl.guiText.transform.position = Camera.main.WorldToScreenPoint(pl.guiPlanet.transform.position);
        }

        if (Input.GetMouseButtonDown(0))
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
                    if (planet.guiPlanet == goClicked)
                    {
                        Galaxy.Colonize(planet,Galaxy.Empires[0]);
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
}
