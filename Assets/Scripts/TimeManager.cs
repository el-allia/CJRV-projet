using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public enum DayPeriod { Morning, Noon, Afternoon }

    [Header("How fast time changes (for testing)")]
    [Tooltip("How many seconds before switching to the next period")]
    public float secondsPerPeriod = 30f;

    public event Action<DayPeriod> OnPeriodChanged;

    private DayPeriod currentPeriod = DayPeriod.Morning;
    private float timer = 0f;

    private void Start()
    {
        // Tell listeners what period we start in
        OnPeriodChanged?.Invoke(currentPeriod);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= secondsPerPeriod)
        {
            timer = 0f;
            NextPeriod();
        }
    }

    private void NextPeriod()
    {
        // Morning -> Noon -> Afternoon -> Morning ...
        currentPeriod = (DayPeriod)(((int)currentPeriod + 1) % 3);
        OnPeriodChanged?.Invoke(currentPeriod);
        Debug.Log("Period changed to: " + currentPeriod);
    }

    public DayPeriod GetCurrentPeriod()
    {
        return currentPeriod;
    }
}
