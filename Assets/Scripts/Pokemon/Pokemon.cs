using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using URandom = UnityEngine.Random;

[Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private Int32 _level;

    public PokemonBase Base => _base;
    public Int32 Level => _level;

    public String Name => Base.Name;
    public String Description => Base.Description;
    
    public Sprite FrontSprite => Base.FrontSprite;
    public Sprite BackSprite => Base.BackSprite;

    public PokemonType Type1 => Base.Type1;
    public PokemonType Type2 => Base.Type2;

    public Int32 BaseHP => Base.MaxHP;
    public Int32 BaseAttack => Base.Attack;
    public Int32 BaseDefense => Base.Defense;
    public Int32 BaseSpAtk => Base.SpAtk;
    public Int32 BaseSpDef => Base.SpDef;
    public Int32 BaseSpeed => Base.Speed;

    public Int32 MaxHP { get; private set; }
    public Int32 CurrentHP { get; set; }
    public Boolean IsFainted => CurrentHP <= 0;
    public Boolean HPChanged { get; set; }

    public Int32 Attack => GetStat(Stat.Attack);
    public Int32 Defense => GetStat(Stat.Defense);
    public Int32 SpAtk => GetStat(Stat.SpAttack);
    public Int32 SpDef => GetStat(Stat.SpDefense);
    public Int32 Speed => GetStat(Stat.Speed);
    
    public Dictionary<Stat, Int32> Stats { get; private set; }
    public Dictionary<Stat, Int32> StatBoosts { get; private set; }
    public Queue<String> StatusChanges { get; private set; } = new();

    public List<LearnableMove> LearnableMoves => Base.LearnableMoves;
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    
    public Condition Status { get; private set; }
    public Int32 StatusTime { get; set; }
    
    public Condition VolatileStatus { get; private set; }
    public Int32 VolatileStatusTime { get; set; }

    public event Action OnStatusChanged;

    public void Init()
    {
        Moves = new List<Move>();
        foreach (LearnableMove move in LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));
            if (Moves.Count >= 4)
                break;
        }
        
        CalculateStats();
        CurrentHP = MaxHP;

        ResetStatBoosts();
        
        Status = null;
        VolatileStatus = null;
    }

    private void ResetStatBoosts()
    {
        StatBoosts = new Dictionary<Stat, Int32>
        {
            { Stat.Attack, 0 },
            { Stat.Defense, 0 },
            { Stat.SpAttack, 0 },
            { Stat.SpDefense, 0 },
            { Stat.Speed, 0 },
            { Stat.Accuracy, 0 },
            { Stat.Evasion, 0 }
        };
    }

    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, Int32>
        {
            { Stat.Attack, Mathf.FloorToInt(BaseAttack * Level / 100f) + 5 },
            { Stat.Defense, Mathf.FloorToInt(BaseDefense * Level / 100f) + 5 },
            { Stat.SpAttack, Mathf.FloorToInt(BaseSpAtk * Level / 100f) + 5 },
            { Stat.SpDefense, Mathf.FloorToInt(BaseSpDef * Level / 100f) + 5 },
            { Stat.Speed, Mathf.FloorToInt(BaseSpeed * Level / 100f) + 5 }
        };

        MaxHP = Mathf.FloorToInt(BaseHP * Level / 100f) + Level + 10;
    }

    private Int32 GetStat(Stat stat)
    {
        Int32 statVal = Stats[stat];

        Int32 boost = StatBoosts[stat];
        Single[] boostValues = { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        
        return statVal;
    }

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        Single critical = URandom.value * 100f <= 6.25 ? 2f : 1f;
        Single typeEffectiveness = 
            TypeChart.GetEffectiveness(move.Type, Type1) * TypeChart.GetEffectiveness(move.Type, Type2);

        DamageDetails damageDetails = new()
        {
            TypeEffectiveness = typeEffectiveness,
            Critical = critical,
            Fainted = false
        };

        Single attack = move.IsSpecial ? attacker.SpAtk : attacker.Attack;
        Single defense = move.IsSpecial ? SpDef : Defense;
        
        Single modifiers = URandom.Range(0.85f, 1f) * typeEffectiveness * critical;
        Single a = (2 * attacker.Level + 10) / 250f;
        Single d = a * move.Power * (attack / defense) + 2;
        Int32 damage = Mathf.FloorToInt(d * modifiers);
        
        UpdateHP(damage);

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        List<Move> movesWithPP = Moves.Where(x => x.CurrentPP > 0).ToList(); 
        return movesWithPP[URandom.Range(0, movesWithPP.Count)];
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (StatBoost statBoost in statBoosts)
        {
            Stat stat = statBoost.stat;
            Int32 boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            StatusChanges.Enqueue($"{Name}'s {stat} " + (boost > 0 ? "rose!" : "fell!"));
            
            Debug.Log($"{stat} has been boosted to {StatBoosts[stat]}.");
        }
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoosts();
    }

    public void SetStatus(ConditionID condition)
    {
        if (Status != null)
            return;
        
        Status = ConditionsDB.Conditions[condition];
        Status!.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Name} {Status!.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }
    
    public void SetVolatileStatus(ConditionID condition)
    {
        if (VolatileStatus != null)
            return;
        
        VolatileStatus = ConditionsDB.Conditions[condition];
        VolatileStatus!.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Name} {VolatileStatus!.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public void UpdateHP(Int32 damage)
    {
        CurrentHP = Mathf.Clamp(CurrentHP - damage, 0, MaxHP);
        HPChanged = true;
    }

    public Boolean OnBeforeMove()
    {
        Boolean canPerformMove = true;
        
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;
        }
        
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
}