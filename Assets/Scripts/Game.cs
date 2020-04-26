using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    Galaxy Galaxy = new Galaxy();
    public GameObject planetTemplate;
    public Text textTemplate;
    // Start is called before the first frame update
    void Start()
    {
        Galaxy.Generate(100,100,1f);
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
            Sprite sprite = Resources.Load<Sprite>("105 Colorful 2D Planet Icons/" + SpriteString);
            goPlanet.GetComponent<SpriteRenderer>().sprite = sprite;
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
                        if (planet.owner != null)
                        {
                            print("Next iD would be: " + (planet.owner.iD + 1) + " Empirecount: " + Galaxy.Empires.Count);
                            if (planet.owner.iD + 1 < Galaxy.Empires.Count)
                            {
                                planet.changeOwner(Galaxy.Empires[planet.owner.iD + 1]);
                            }
                            else
                            {
                                planet.changeOwner(null);
                            }
                        }
                        else
                        {
                            planet.changeOwner(Galaxy.Empires[0]);
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
    }

    public void ProcessTurn()
    {
        Galaxy.ProcessTurn();
    }
}
