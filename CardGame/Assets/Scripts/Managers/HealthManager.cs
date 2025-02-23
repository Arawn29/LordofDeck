using Fusion;
using Microlight.MicroBar;
using TMPro;
using UnityEngine;
public class HealthManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    public static HealthManager instance;
    public Canvas HealthBarCanvas;
    public MicroBar[] healthBars; // 0. index local player olacak.
    [Networked, Capacity(2)]
    public NetworkDictionary<PlayerRef, int> playersHealths => default;
    int maxHealth;
    private void Awake()
    {
        maxHealth = GameManager.instance.healthStorage;
        if (instance == null)
            instance = this;
        else Destroy(this);

        foreach (MicroBar healthBar in healthBars)
        {
            healthBar.Initialize(maxHealth);
            healthBar.gameObject.SetActive(false);
        }
    }
    public void SpawnBar(PlayerRef player)
    {
        if (Runner.LocalPlayer == player)
        {
            if (playersHealths.ContainsKey(player))
            {

                healthBars[0].gameObject.SetActive(true);
                healthBars[0].transform.Find("PlayerText").GetComponent<TextMeshProUGUI>().text = "Your Health";
                healthBars[0].transform.Find("HealthText").GetComponent<TextMeshProUGUI>().text = $"{playersHealths.Get(player)}  /  {maxHealth}";
            }
        }
        else
        {
            if (playersHealths.ContainsKey(player))
            {
                healthBars[1].gameObject.SetActive(true);
                healthBars[1].transform.Find("PlayerText").GetComponent<TextMeshProUGUI>().text = "Opponent Health";
                healthBars[1].transform.Find("HealthText").GetComponent<TextMeshProUGUI>().text = $"{playersHealths.Get(player)}  /  {maxHealth}";

            }

        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_HealthBarChange(PlayerRef player, int Damage)
    {
        if (!Runner.IsServer) return;
        if (playersHealths.TryGet(player, out int health))
        {
            Debug.LogWarning($"{player} ýn {health} caný vardý {Damage} hasar alacak");
            health -= Damage;
            playersHealths.Set(player, health);
            RPC_SetHealthBarProp(player);
            if (health <= 0)
            {
                GameManager.instance.RPC_ChooseWinner(player);
            }
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetHealthBarProp(PlayerRef player)
    {
        if (Runner.LocalPlayer == player)
        {
            if (playersHealths.TryGet(player, out int health))
            {
                if (health >= 0)
                {
                    healthBars[0].transform.Find("HealthText").GetComponent<TextMeshProUGUI>().text = $"{health}/{maxHealth}";
                    healthBars[0].UpdateBar(health);
                }
                else
                {
                    healthBars[0].transform.Find("HealthText").GetComponent<TextMeshProUGUI>().text = $"{0}/{maxHealth}";
                    healthBars[0].UpdateBar(0);
                }


            }


        }
        else
        {
            if (playersHealths.TryGet(player, out int health))
            {
                if (health >= 0)
                {
                    healthBars[1].UpdateBar(health);
                    healthBars[1].transform.Find("HealthText").GetComponent<TextMeshProUGUI>().text = $"{health}/{maxHealth}";

                }
                else
                {
                    healthBars[1].UpdateBar(0);
                    healthBars[1].transform.Find("HealthText").GetComponent<TextMeshProUGUI>().text = $"{0}/{maxHealth}";
                }
            }

        }
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            playersHealths.Add(player, GameManager.instance.healthStorage);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.IsServer == false) return;
        if (playersHealths.ContainsKey(player))
        {
            playersHealths.Remove(player);
        }
    }
}
