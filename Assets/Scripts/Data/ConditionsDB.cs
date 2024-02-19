using System;
using System.Collections.Generic;
using URandom = UnityEngine.Random;

public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
            kvp.Value.ID = kvp.Key;
    }
    
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new()
    {
        {
            ConditionID.Poison,
            new Condition
            {
                Name = "Poison",
                ShortName = "PSN",
                StartMessage = "has been poisoned",
                OnAfterTurn = mon =>
                {
                    mon.UpdateHP(mon.MaxHP / 8);
                    mon.StatusChanges.Enqueue($"{mon.Name} was hurt due to poison");
                }
            }
        },
        {
            ConditionID.Burn,
            new Condition
            {
                Name = "Burn",
                ShortName = "BRN",
                StartMessage = "has been burned",
                OnAfterTurn = mon =>
                {
                    mon.UpdateHP(mon.MaxHP / 16);
                    mon.StatusChanges.Enqueue($"{mon.Name} was hurt by its burn");
                }
            }
        },
        {
            ConditionID.Paralyze,
            new Condition
            {
                Name = "Paralyze",
                ShortName = "PAR",
                StartMessage = "has been paralyzed",
                OnBeforeMove = mon =>
                {
                    if (URandom.Range(1, 5) == 1)
                    {
                        mon.StatusChanges.Enqueue($"{mon.Name} is paralyzed and can't move");
                        return false;
                    }

                    return true;
                }
            }
        },
        {
            ConditionID.Freeze,
            new Condition
            {
                Name = "Freeze",
                ShortName = "FRZ",
                StartMessage = "has been frozen",
                OnBeforeMove = mon =>
                {
                    if (URandom.Range(1, 5) == 1)
                    {
                        mon.CureStatus();
                        mon.StatusChanges.Enqueue($"{mon.Name} is no longer frozen");
                        return true;
                    }

                    return false;
                }
            }
        },
        {
            ConditionID.Sleep,
            new Condition
            {
                Name = "Sleep",
                ShortName = "SLP",
                StartMessage = "has fallen asleep",
                OnStart = mon =>
                {
                    mon.StatusTime = URandom.Range(1, 4);
                },
                OnBeforeMove = mon =>
                {
                    if (mon.StatusTime <= 0)
                    {
                        mon.CureStatus();
                        mon.StatusChanges.Enqueue($"{mon.Name} woke up!");
                        return true;
                    }
                    
                    mon.StatusTime--;
                    mon.StatusChanges.Enqueue($"{mon.Name} is fast asleep");
                    return false;
                }
            }
        },
        {
            ConditionID.Confusion,
            new Condition
            {
                Name = "Confusion",
                ShortName = "",
                StartMessage = "has become confused",
                OnStart = mon =>
                {
                    mon.VolatileStatusTime = URandom.Range(1, 5);
                },
                OnBeforeMove = mon =>
                {
                    if (mon.VolatileStatusTime <= 0)
                    {
                        mon.CureVolatileStatus();
                        mon.StatusChanges.Enqueue($"{mon.Name} is no longer confused!");
                        return true;
                    }
                    
                    mon.VolatileStatusTime--;

                    if (URandom.Range(1, 3) == 1)
                        return true;
                    
                    mon.StatusChanges.Enqueue($"{mon.Name} is confused");
                    mon.UpdateHP(mon.MaxHP / 8);
                    mon.StatusChanges.Enqueue("It hurt itself due to its confusion!");
                    return false;
                }
            }
        }
    };
}