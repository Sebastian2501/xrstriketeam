using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Accenture.rkiss.Utils
{
    public class TimerAction : MonoBehaviour
    {
        private Coroutine timerCoroutine;
        private int targetTime;
        private int currentTime;
        private UnityAction onTargetReached;
        private Dictionary<int, UnityAction> targetTimeActions = new Dictionary<int, UnityAction>();
        private UnityAction onTick;
        private bool isPaused;

        // Static method to start the timer
        public static void StartTimer(int targetNumber, UnityAction onTargetReached, UnityAction onTick = null)
        {
            TimerAction instance = GetInstance();
            instance.targetTime = targetNumber;
            instance.onTargetReached = onTargetReached;
            instance.onTick = onTick;
            instance.timerCoroutine = instance.StartCoroutine(instance.TimerCoroutine());
        }

        public static void AppendTimer(int targetNumber, UnityAction onTargetReached)
        {
            TimerAction instance = GetInstance();
            instance.targetTimeActions.Add(targetNumber, onTargetReached);
        }

        // Coroutine that handles the timer logic
        private IEnumerator TimerCoroutine()
        {
            currentTime = 0; // Reset number to 0
            isPaused = false;

            while (currentTime < targetTime)
            {
                if (!isPaused)
                {
                    currentTime += 1; // Increment number by 1 each second
                    onTick?.Invoke();

                    if (targetTimeActions.ContainsKey(currentTime))
                    {
                        targetTimeActions[currentTime]?.Invoke();
                        targetTimeActions.Remove(currentTime);
                    }

                    yield return new WaitForSeconds(1); // Wait for one second
                }
                else
                {
                    yield return null;
                }
            }

            onTargetReached?.Invoke(); // Invoke the UnityAction if not null
            timerCoroutine = null; // Clear the coroutine reference
        }

        // Method to cancel the timer
        public static void CancelTimer()
        {
            TimerAction instance = GetInstance();
            if (instance.timerCoroutine != null)
            {
                instance.StopCoroutine(instance.timerCoroutine);
                instance.timerCoroutine = null;
            }
        }

        // Method to pause the timer
        public static void PauseTimer()
        {
            TimerAction instance = GetInstance();
            instance.isPaused = true;
        }

        // Method to resume the timer
        public static void ResumeTimer()
        {
            TimerAction instance = GetInstance();
            instance.isPaused = false;
        }

        // Method to reset the timer
        public static void ResetTimer()
        {
            CancelTimer();
            StartTimer(GetInstance().targetTime, GetInstance().onTargetReached, GetInstance().onTick);
        }

        // Ensure a single instance of TimerAction is available
        private static TimerAction GetInstance()
        {
            TimerAction instance = FindObjectOfType<TimerAction>();
            if (instance == null)
            {
                GameObject timerObject = new GameObject("TimerActionObject");
                instance = timerObject.AddComponent<TimerAction>();
            }
            return instance;
        }

        void OnApplicationQuit()
        {
            CleanUpTimerInstance();
        }

        void CleanUpTimerInstance()
        {
            TimerAction instance = FindObjectOfType<TimerAction>();
            if (instance != null)
            {
                Destroy(instance);
            }
        }
    }
}