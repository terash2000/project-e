using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Player : GameCharacter
{
    public const float DAMAGE_COOLDOWN_TIME = 0.2f;

    [SerializeField]
    private GameObject _healthBar;
    [SerializeField]
    private GameObject _blockBar;
    [SerializeField]
    private GameObject _healthText;
    [SerializeField]
    private GameObject _damageText;
    [SerializeField]
    private GameObject _blockText;
    [SerializeField]
    private SpriteRenderer _damageIcon;
    [SerializeField]
    private GameObject _previewDamage;
    [SerializeField]
    private GameObject _deathIcon;
    [SerializeField]
    private HorizontalLayoutGroup _statusContainer;
    [SerializeField]
    private GameObject _statusPrefab;
    [SerializeField]
    private Color _damageColor;

    private Vector3 _localScale = new Vector3(0.12f, 0.12f, 1f);
    private Vector3 _healthLocalScale;
    private float _healthBarSize;
    private float _damagePopupCooldown = 0f;
    private Queue<DamageQueueData> _damageQueue = new Queue<DamageQueueData>();

    protected override void Start()
    {
        base.Start();
        transform.localScale = _localScale;
        _healthLocalScale = _healthBar.transform.localScale;
        _healthBarSize = _healthLocalScale.x;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        _healthLocalScale.x = _healthBarSize * (float)PlayerData.Health / (float)PlayerData.MaxHealth;
        _healthBar.transform.localScale = _healthLocalScale;
        _healthText.GetComponent<TextMeshProUGUI>().text = PlayerData.Health.ToString();

        _blockText.GetComponent<TextMeshProUGUI>().text = PlayerData.Block.ToString();
        _blockText.SetActive(PlayerData.Block > 0);
        _blockBar.SetActive(PlayerData.Block > 0);

        // damage popup
        if (_damagePopupCooldown > 0)
        {
            _damagePopupCooldown -= Time.deltaTime;
        }
        else if (_damageQueue.Count != 0)
        {
            CreateDamagePopup(_damageQueue.Dequeue());
        }
    }

    protected override void UpdateStatusIcon()
    {
        for (int i = 0; i < _statusContainer.transform.childCount; i++)
        {
            Destroy(_statusContainer.transform.GetChild(i).gameObject);
        }

        foreach (KeyValuePair<Status.Type, int> status in StatusDict)
        {
            GameObject statusObj = Instantiate(_statusPrefab, _statusContainer.transform);
            statusObj.GetComponent<StatusDisplay>().Init(status.Key, status.Value);
        }
    }

    public override int TakeDamage(int damage, Status.Type? damageStatusEffect = null)
    {
        // Prevent Die() from being executed twice when the monster has more than one status effect
        if (PlayerData.Health == 0)
            return 0;

        Color color;
        if (damageStatusEffect == Status.Type.Acid)
            color = GameManager.Instance.AcidColor;
        else if (damageStatusEffect == Status.Type.Burn)
            color = GameManager.Instance.BurnColor;
        else
            color = _damageColor;

        int blockedAmount = 0;
        if (PlayerData.Block != 0)
        {
            if (damageStatusEffect == Status.Type.Acid)
            {
                // Acid attack will deal "Status.ACID_TO_BLOCK_MULTIPLIER" of damage to block but cannot carry over to health
                int damageToBlock = System.Convert.ToInt32(System.Math.Floor(damage * Status.ACID_TO_BLOCK_MULTIPLIER));
                blockedAmount = PlayerData.Block - damageToBlock < 0 ? PlayerData.Block : damageToBlock;
                damage = 0;
            }
            else
            {
                // Normal scenario, if the attack damage is more than the block, it'll get carried over to the health
                blockedAmount = PlayerData.Block - damage < 0 ? PlayerData.Block : damage;
                damage -= blockedAmount;
            }
            // Debug.Log("Attack damage of type " + damageStatusEffect + " has been blocked by " + blockedAmount + " and get carried over by " + damage);
            PlayerData.Block -= blockedAmount;
        }

        if (damage != 0 || blockedAmount != 0)
        {
            CreateDamagePopup(new DamageQueueData(damage, blockedAmount, color));
        }

        PlayerData.Health -= damage;
        if (PlayerData.Health <= 0)
        {
            PlayerData.Health = 0;
        }
        return PlayerData.Health;
    }

    private void CreateDamagePopup(DamageQueueData data)
    {
        if (_damagePopupCooldown > 0)
        {
            _damageQueue.Enqueue(data);
        }
        else
        {
            Canvas playerCanvas = GetComponentInChildren<Canvas>();
            Debug.Log("Player Canvas : ", playerCanvas);

            if (data.damage != 0 || data.color == _damageColor)
            {
                GameObject damagePopup = Instantiate(_previewDamage, playerCanvas.transform);
                damagePopup.SetActive(true);
                damagePopup.name = "Popup Damage Player";

                TextMeshProUGUI damagePopupText = damagePopup.GetComponent<TextMeshProUGUI>();
                damagePopupText.text = data.damage.ToString();
                damagePopupText.color = data.color;

                damagePopup.AddComponent<DamagePopup>();
                damagePopup.transform.SetParent(Arena.Instance.transform);
            }

            if (data.block != 0)
            {
                GameObject blockPopup = Instantiate(_previewDamage, playerCanvas.transform);
                blockPopup.SetActive(true);
                blockPopup.name = "Popup Block Player";

                TextMeshProUGUI blockPopupText = blockPopup.GetComponent<TextMeshProUGUI>();
                blockPopupText.text = data.block.ToString();
                blockPopupText.color = data.color;

                blockPopup.AddComponent<BlockPopup>();
                blockPopup.transform.SetParent(Arena.Instance.transform);
            }
        }

        _damagePopupCooldown = DAMAGE_COOLDOWN_TIME;
    }
}
