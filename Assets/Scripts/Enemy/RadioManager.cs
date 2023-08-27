using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AlertState
{
    CALM,
    CAUTION,
    ALERT
}

public class RadioManager : MonoBehaviour
{
    public AlertState alertState = AlertState.CALM;
    public float alertCooldown = 30f;
    public float cautionCooldown = 60f;

    public delegate void OnAlertStateChange(AlertState state);
    public OnAlertStateChange onAlertStateChange;

    private Vector2? _lastReportedPlayerPos = null;
    private float _alertCooldown = 0f;
    private Dictionary<GameObject, Vector2> _enemyDestinations = new();
    private Dictionary<int, Vector2> _squadDestinations = new();


    public Vector2? RequestPlayerPos()
    {
        // Add some variance, perhaps?
        return _lastReportedPlayerPos;
    }

    private void RefreshSquadDestinations()
    {
        foreach (var squad in _enemyDestinations.Keys.Select(x => x.GetComponent<Enemy>().squad).Distinct())
        {
            if (_enemyDestinations.Where(x => x.Key.GetComponent<Enemy>().squad == squad)
                .All(x => Vector2.Distance(x.Key.transform.position, x.Value) < 0.5f))
            {
                Vector2 destPos = new();
                for (int i = 0; i < 100; i++)
                {
                    destPos = new(Random.Range(-50, 50), Random.Range(-50, 50));
                    if (_enemyDestinations.All((k) =>
                            Vector2.Distance(destPos, k.Value) > k.Key.GetComponent<Enemy>().visibilityRange)) break;
                }

                _squadDestinations[squad] = destPos;
            }
        }
    }

    public void DeleteEnemy(GameObject enemy)
    {
        _enemyDestinations.Remove(enemy);
    }

    public Vector2 RequestNewPosition(GameObject requester)
    {
        Vector2 destPos = new();

        Enemy enemy = requester.GetComponent<Enemy>();

        if (alertState == AlertState.CALM)
        {
            if (enemy.squad != -1)
            {
                RefreshSquadDestinations();
                Vector2 destOffset = new(Random.Range(-5, 5), Random.Range(-5, 5));
                if (!_squadDestinations.ContainsKey(enemy.squad))
                {
                    Vector2 squadDestPos = new();
                    for (int i = 0; i < 100; i++)
                    {
                        squadDestPos = new(Random.Range(-50, 50), Random.Range(-50, 50));
                        if (_enemyDestinations.All((k) =>
                                Vector2.Distance(squadDestPos, k.Value) > k.Key.GetComponent<Enemy>().visibilityRange)) break;
                    }

                    _squadDestinations[enemy.squad] = squadDestPos;
                }
                destPos = _squadDestinations[enemy.squad] + destOffset;
                while (Physics2D.OverlapPoint(destPos, LayerMask.GetMask("Enemy", "Terrain")))
                {
                    destOffset = new Vector2(Random.Range(-5, 5), Random.Range(-5, 5));
                    destPos = _squadDestinations[enemy.squad] + destOffset;
                }
            }
            else
            {
                for (int i = 0; i < 100; i++)
                {
                    destPos = new(Random.Range(-50, 50), Random.Range(-50, 50));
                    while (Physics2D.OverlapPoint(destPos, LayerMask.GetMask("Enemy", "Terrain"))) destPos = new(Random.Range(-50, 50), Random.Range(-50, 50));
                    if (_enemyDestinations.All((k) =>
                            Vector2.Distance(destPos, k.Value) > k.Key.GetComponent<Enemy>().visibilityRange)) break;
                }
            }
        }
        else if (alertState == AlertState.CAUTION)
        {
            for (int i = 0; i < 100; i++)
            {
                Vector2 destOffset = new(Random.Range(-20, 20), Random.Range(-20, 20));
                if (_lastReportedPlayerPos != null) destPos = (Vector2)(_lastReportedPlayerPos + destOffset);
                while (Physics2D.OverlapPoint(destPos, LayerMask.GetMask("Enemy", "Terrain")))
                {
                    destOffset = new Vector2(Random.Range(-20, 20), Random.Range(-20, 20));
                    destPos = (Vector2)(_lastReportedPlayerPos + destOffset);
                }
                if (_enemyDestinations.All((k) =>
                        Vector2.Distance(destPos, k.Value) > k.Key.GetComponent<Enemy>().visibilityRange)) break;
            }
        }
        else if (alertState == AlertState.ALERT)
        {
            if (_lastReportedPlayerPos != null) destPos = (Vector2)(_lastReportedPlayerPos);
        }

        _enemyDestinations[requester] = destPos;

        return destPos;
    }

    public void ReportPlayerPos(Vector2 playerPos)
    {
        if (alertState != AlertState.ALERT) RaiseAlarm(); // Player spotted, raise the alarm
        else _alertCooldown = alertCooldown;
        _lastReportedPlayerPos = playerPos;
    }

    private void RaiseAlarm()
    {
        MusicManager.StartBattleMusic();
        _alertCooldown = alertCooldown;
        alertState = AlertState.ALERT;
        onAlertStateChange.Invoke(alertState);
    }

    private void RaiseCaution()
    {
        _alertCooldown = cautionCooldown;
        alertState = AlertState.CAUTION;
        onAlertStateChange.Invoke(alertState);
    }

    private void RaiseCalm()
    {
        MusicManager.StopBattleMusic();
        alertState = AlertState.CALM;
        onAlertStateChange.Invoke(alertState);
    }

    private void ElapseTimer()
    {
        if (alertState == AlertState.CALM) return;
        if (alertState == AlertState.CAUTION) RaiseCalm();
        if (alertState == AlertState.ALERT) RaiseCaution();
    }

    private void FixedUpdate()
    {
        if (_alertCooldown > 0f)
        {
            _alertCooldown -= Time.fixedDeltaTime;
            if (_alertCooldown <= 0f) ElapseTimer();
        }
    }
}
