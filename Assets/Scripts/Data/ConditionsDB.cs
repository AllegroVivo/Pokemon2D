using System;
using System.Collections.Generic;

public class ConditionsDB
{
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
        }
    };
}