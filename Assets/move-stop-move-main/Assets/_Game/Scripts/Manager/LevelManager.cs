using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelManager : Singleton<LevelManager>
{
    public Player player;

    private List<Bot> bots = new List<Bot>();

    [SerializeField] Level[] levels;
    public Level currentLevel;

    private int totalBot;
    private bool isRevive;

    private int levelIndex;

    public int TotalCharater => totalBot + bots.Count + 1;

    public void Start()
    {
        levelIndex = 0;
        OnLoadLevel(levelIndex);
        OnInit();
    }

    public void OnInit()
    {
        player.OnInit();

        for (int i = 0; i < currentLevel.botReal; i++)
        {
            SpawnNewBot(null);
        }

        totalBot = currentLevel.botTotal - currentLevel.botReal - 1;

        isRevive = false;

        SetTargetIndicatorAlpha(0);
    }

    public void OnReset()
    {
        player.OnDespawn();
        for (int i = 0; i < bots.Count; i++)
        {
            bots[i].OnDespawn();
        }

        bots.Clear();
        SimplePool.CollectAll();
    }

    public void OnLoadLevel(int level)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel.gameObject);
        }

        currentLevel = Instantiate(levels[level]);
    }

    public Vector3 RandomPoint()
    {
        const int maxAttempts = 50;
        const float minDistanceToOthers = Character.ATT_RANGE + Character.MAX_SIZE + 1f;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 spawnPos = currentLevel.RandomPoint();

            if (Vector3.Distance(spawnPos, player.TF.position) < minDistanceToOthers)
                continue;

            bool isTooCloseToAnyBot = false;
            foreach (var bot in bots)
            {
                if (Vector3.Distance(spawnPos, bot.TF.position) < minDistanceToOthers)
                {
                    isTooCloseToAnyBot = true;
                    break;
                }
            }

            if (!isTooCloseToAnyBot)
                return spawnPos;
        }

        return currentLevel.RandomPoint();
    }

    private void SpawnNewBot(IState<Bot> state)
    {
        Bot bot = SimplePool.Spawn<Bot>(PoolType.Bot, RandomPoint(), Quaternion.identity);
        bot.OnInit();
        bot.ChangeState(state);
        bots.Add(bot);

        bot.SetScore(player.Score > 0 ? Random.Range(player.Score - 7, player.Score + 7) : 1);
    }

    public void CharecterDeath(Character character)
    {
        if (character is Player)
        {
            HandlePlayerDeath();
        }
        else if (character is Bot bot)
        {
            HandleBotDeath(bot);
        }

        UIManager.Ins.GetUI<UIGameplay>().UpdateTotalCharacter();
    }

    private void HandlePlayerDeath()
    {
        UIManager.Ins.CloseAll();

        //revive
        if (!isRevive)
        {
            isRevive = true;
            UIManager.Ins.OpenUI<UIRevive>();
        }
        else
        {
            Fail();
        }
    }

    private void HandleBotDeath(Bot bot)
    {
        bots.Remove(bot);

        if (GameManager.Ins.IsState(GameState.Revive) || GameManager.Ins.IsState(GameState.Setting))
        {
            SpawnNewBotWithRandomState();
        }
        else
        {
            if (totalBot > 0)
            {
                totalBot--;
                SpawnNewBotWithRandomState();
            }

            if (bots.Count == 0)
            {
                Victory();
            }
        }
    }

    private void SpawnNewBotWithRandomState()
    {
        IState<Bot> randomState = Utilities.Chance(50, 100) ? new IdleState() : new PatrolState();
        SpawnNewBot(randomState);
    }

    private void Victory()
    {
        UIManager.Ins.CloseAll();

        // store coin in memory
        UserData.Ins.StoreCoin(player.Coin);

        UIManager.Ins.OpenUI<UIVictory>().SetCoin(player.Coin);
        player.ChangeAnim(Constant.ANIM_WIN);
    }

    public void Fail()
    {
        UIManager.Ins.CloseAll();

        // store coin in memory
        UserData.Ins.StoreCoin(player.Coin);

        UIManager.Ins.OpenUI<UIFail>().SetCoin(player.Coin); 
    }

    public void Home()
    {
        UIManager.Ins.CloseAll();
        OnReset();
        OnLoadLevel(levelIndex);
        OnInit();
        UIManager.Ins.OpenUI<UIMainMenu>();
    }

    public void NextLevel()
    {
        levelIndex++;
    }

    public void OnPlay()
    {
        for (int i = 0; i < bots.Count; i++)
        {
            bots[i].ChangeState(new PatrolState());
        }
    }

    public void OnRevive()
    {
        player.TF.position = RandomPoint();
        player.OnRevive();
    }

    public void SetTargetIndicatorAlpha(float alpha)
    {
        List<GameUnit> list = SimplePool.GetAllUnitIsActive(PoolType.TargetIndicator);

        for (int i = 0; i < list.Count; i++)
        {
            (list[i] as TargetIndicator).SetAlpha(alpha);
        }
    }
}
