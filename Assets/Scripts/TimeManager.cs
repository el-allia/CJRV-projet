using System;
using UnityEngine;

/// <summary>
/// Manages game time (1 real minute = 1 game hour)
/// Unity 6 compatible version
/// </summary>
public class TimeManager : MonoBehaviour
{
    /// <summary>
    /// Periods of the day affecting customer spawn rates
    /// </summary>
    public enum DayPeriod
    {
        Morning,    // 6:00 - 10:59
        Noon,       // 11:00 - 14:59
        Afternoon   // 15:00 - 17:59
    }

    [Header("Time Settings")]
    [Tooltip("Starting hour of the day (24-hour format)")]
    [Range(0, 23)]
    public int startHour = 6;

    [Tooltip("Ending hour of the day (24-hour format)")]
    [Range(1, 24)]
    public int endHour = 18;

    [Tooltip("Time scale multiplier (1.0 = normal speed)")]
    [Range(0.1f, 10f)]
    public float timeScale = 1f;

    [Header("Period Definitions")]
    [Tooltip("Hour when noon period starts")]
    [Range(0, 23)]
    public int noonStartHour = 11;

    [Tooltip("Hour when afternoon period starts")]
    [Range(0, 23)]
    public int afternoonStartHour = 15;

    [Header("Debug")]
    [Tooltip("Show time changes in console")]
    public bool showDebugInfo = false;

    // Current time in minutes since midnight
    private float currentTimeInMinutes;

    // Current period of the day
    private DayPeriod currentPeriod;

    // Last displayed time (to avoid spamming logs)
    private string lastDisplayedTime;

    // Event fired when the period changes
    public event Action<DayPeriod> OnPeriodChanged;

    // Event fired when the day ends
    public event Action OnDayEnded;

    // Public property for formatted time display
    public string CurrentFormattedTime { get; private set; }

    // Is the day currently active
    private bool isDayActive = true;

    private void Start()
    {
        // Validate time settings
        ValidateTimeSettings();

        // Initialize time
        currentTimeInMinutes = startHour * 60f;
        currentPeriod = GetPeriodFromTime(currentTimeInMinutes);
        UpdateFormattedTime();

        Debug.Log($"=== Day Started ===");
        Debug.Log($"Time: {CurrentFormattedTime}");
        Debug.Log($"Period: {currentPeriod}");
        Debug.Log($"Day will end at: {endHour:00}:00");
    }

    private void Update()
    {
        if (!isDayActive) return;

        // Advance time (1 real minute = 1 game hour)
        // 60 minutes per hour * timeScale
        currentTimeInMinutes += Time.deltaTime * 60f * timeScale;

        // Check if day has ended
        if (currentTimeInMinutes >= endHour * 60f)
        {
            EndDay();
            return;
        }

        // Update formatted time string
        UpdateFormattedTime();

        // Log only when time changes (to avoid spam)
        if (showDebugInfo && CurrentFormattedTime != lastDisplayedTime)
        {
            Debug.Log($"Current time: {CurrentFormattedTime}");
            lastDisplayedTime = CurrentFormattedTime;
        }

        // Check for period changes
        DayPeriod newPeriod = GetPeriodFromTime(currentTimeInMinutes);
        if (newPeriod != currentPeriod)
        {
            currentPeriod = newPeriod;
            OnPeriodChanged?.Invoke(currentPeriod);
            
            Debug.Log($"=== Period Changed to {currentPeriod} at {CurrentFormattedTime} ===");
        }
    }

    /// <summary>
    /// Validates time settings to ensure they make sense
    /// </summary>
    private void ValidateTimeSettings()
    {
        if (startHour >= endHour)
        {
            Debug.LogWarning("TimeManager: Start hour must be before end hour! Fixing...");
            endHour = startHour + 12;
        }

        if (noonStartHour <= startHour)
        {
            noonStartHour = startHour + 1;
        }

        if (afternoonStartHour <= noonStartHour)
        {
            afternoonStartHour = noonStartHour + 1;
        }
    }

    /// <summary>
    /// Determines the period based on current time in minutes
    /// </summary>
    private DayPeriod GetPeriodFromTime(float minutes)
    {
        int hour = Mathf.FloorToInt(minutes / 60f);

        if (hour < noonStartHour)
            return DayPeriod.Morning;
        else if (hour < afternoonStartHour)
            return DayPeriod.Noon;
        else
            return DayPeriod.Afternoon;
    }

    /// <summary>
    /// Updates the formatted time string (HH:MM)
    /// </summary>
    private void UpdateFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentTimeInMinutes / 60f);
        int minutes = Mathf.FloorToInt(currentTimeInMinutes % 60f);
        CurrentFormattedTime = $"{hours:00}:{minutes:00}";
    }

    /// <summary>
    /// Ends the current day
    /// </summary>
    private void EndDay()
    {
        if (!isDayActive) return;

        isDayActive = false;
        currentTimeInMinutes = endHour * 60f;
        UpdateFormattedTime();

        Debug.Log($"=== Day Ended at {CurrentFormattedTime} ===");
        
        OnDayEnded?.Invoke();
    }

    /// <summary>
    /// Gets the current period of the day
    /// </summary>
    public DayPeriod GetCurrentPeriod()
    {
        return currentPeriod;
    }

    /// <summary>
    /// Gets the current hour (0-23)
    /// </summary>
    public int GetCurrentHour()
    {
        return Mathf.FloorToInt(currentTimeInMinutes / 60f);
    }

    /// <summary>
    /// Gets the current minute (0-59)
    /// </summary>
    public int GetCurrentMinute()
    {
        return Mathf.FloorToInt(currentTimeInMinutes % 60f);
    }

    /// <summary>
    /// Gets progress through the day (0 to 1)
    /// </summary>
    public float GetDayProgress()
    {
        float totalMinutes = (endHour - startHour) * 60f;
        float elapsedMinutes = currentTimeInMinutes - (startHour * 60f);
        return Mathf.Clamp01(elapsedMinutes / totalMinutes);
    }

    /// <summary>
    /// Checks if the day is currently active
    /// </summary>
    public bool IsDayActive()
    {
        return isDayActive;
    }

    /// <summary>
    /// Pauses time progression
    /// </summary>
    public void PauseTime()
    {
        enabled = false;
    }

    /// <summary>
    /// Resumes time progression
    /// </summary>
    public void ResumeTime()
    {
        if (isDayActive)
        {
            enabled = true;
        }
    }

    /// <summary>
    /// Resets and restarts the day
    /// </summary>
    public void RestartDay()
    {
        currentTimeInMinutes = startHour * 60f;
        currentPeriod = GetPeriodFromTime(currentTimeInMinutes);
        isDayActive = true;
        enabled = true;
        UpdateFormattedTime();

        Debug.Log("Day restarted!");
    }

    /// <summary>
    /// Gets time remaining until day end in minutes
    /// </summary>
    public float GetRemainingTime()
    {
        return (endHour * 60f) - currentTimeInMinutes;
    }

    /// <summary>
    /// Gets formatted string for remaining time
    /// </summary>
    public string GetRemainingTimeFormatted()
    {
        float remaining = GetRemainingTime();
        int hours = Mathf.FloorToInt(remaining / 60f);
        int minutes = Mathf.FloorToInt(remaining % 60f);
        return $"{hours:00}:{minutes:00}";
    }
}
