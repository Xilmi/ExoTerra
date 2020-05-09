using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Assignment
{
    public Assignment(string en, string ot)
    {
        enumText = en;
        outText = ot;
    }
    public string enumText;
    public string outText;
}
public class TextAssignments
{
    List<Assignment> assignement;
    public TextAssignments()
    {
        assignement = new List<Assignment>();
    }
    public void AddAssignment(string en, string ot)
    {
        Assignment neoAssignment = new Assignment(en, ot);
        assignement.Add(neoAssignment);
    }
    public string GetTextFor(string enumText)
    {
        foreach(Assignment ass in assignement)
        {
            if(ass.enumText == enumText)
            {
                return ass.outText;
            }
        }
        return "Text not found";
    }
}
