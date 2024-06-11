using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkersManager : MonoBehaviour
{
    public List<WorkerCreature> ConstructionWorkers = new List<WorkerCreature>();
    public int MaxConstructionWorkers = 1;

    public WorkerCreature GetConstructionWorkerByID(string id)
    {
        foreach (WorkerCreature worker in ConstructionWorkers)
        {
            if (worker.Data.ID == id)
            {
                return worker;
            }
        }
        return null;
    }
}
public class WorkerCreature
{
    public FighterData Data;
    public int WorkerLevel;
    public int SpeedLevel;
    public int EfficiencyLevel;
    public List<Skills> WorkerSkills;
    public bool IsBusy;
}
