using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Mathf;

namespace Unity.Flayer.InputSystem
{
    public static class InputManager
    {
        public static KeyCode[] KeyCodes = (KeyCode[])System.Enum.GetValues(typeof(KeyCode));
        /// <summary>
        /// 1.0f - 2.0f
        /// </summary>
        public static float AxisGravity = 1.1f;
        public static float AxisSensitivity = .1f;
        public static int UpdateInterval = 5;
        static bool initialized = false;

        public static Dictionary<string, KeyCode> KeyBinds = new Dictionary<string, KeyCode>()
        {
            {"None" , KeyCode.None},
            {"Primary", KeyCode.Mouse0 },
            {"Secondary", KeyCode.Mouse1 },
            {"Jump", KeyCode.Space },
            {"Reload", KeyCode.R },
            {"Use", KeyCode.E },
            {"Drop", KeyCode.G },
            {"Walk", KeyCode.LeftShift },
            {"Crouch", KeyCode.LeftControl },
            {"Console", KeyCode.F1 },
        };
        public static Dictionary<string, (KeyCode, KeyCode, float)> AxisBinds = new Dictionary<string, (KeyCode, KeyCode, float)>()
        {
            {"None" , (KeyCode.None, KeyCode.None , 0f)},
            {"Vertical", (KeyCode.W, KeyCode.S, 0f) },
            {"Horizontal", (KeyCode.A, KeyCode.D, 0f) },
        };

        public static void InitAxis()
        {
            if (initialized) return;
            initialized = true;
            InputUpdateCaller inputUpdateCaller = new InputUpdateCaller();
            Thread thread = new Thread(new ThreadStart(() => { inputUpdateCaller.Call(UpdateInterval); }));

        }
        public static void Update()
        {
            var keys = AxisBinds.Keys;
            foreach (var item in keys)
            {
                if (AxisBinds[item].Item3 < .1) AxisBinds[item] = (AxisBinds[item].Item1, AxisBinds[item].Item2, 0f);

                if (Input.GetKey(AxisBinds[item].Item1) && Input.GetKey(AxisBinds[item].Item2)) continue;
                else if (Input.GetKey(AxisBinds[item].Item1)) {
                    AxisBinds[item] = (AxisBinds[item].Item1, AxisBinds[item].Item2, Clamp(AxisBinds[item].Item3 + AxisSensitivity, -1, 1));
                }
                else if (Input.GetKey(AxisBinds[item].Item2)) {
                    AxisBinds[item] = (AxisBinds[item].Item1, AxisBinds[item].Item2, Clamp(AxisBinds[item].Item3 - AxisSensitivity, -1, 1));
                }
                else AxisBinds[item] = (AxisBinds[item].Item1, AxisBinds[item].Item2, AxisBinds[item].Item3 / AxisGravity);
            }
        }

        public static float GetBindedAxis(string axis) => AxisBinds.TryGetValue(axis, out var code) ? code.Item3 : ReportMissingBind(axis) ? 0f : 0f;

        public static bool GetBind(string key) => KeyBinds.TryGetValue(key, out KeyCode code) ? Input.GetKey(code) : ReportMissingBind(key);

        public static bool GetBindDown(string key) => KeyBinds.TryGetValue(key, out KeyCode code) ? Input.GetKeyDown(code) : ReportMissingBind(key);

        public static bool GetBindUp(string key) => KeyBinds.TryGetValue(key, out KeyCode code) ? Input.GetKeyUp(code) : ReportMissingBind(key);

        public static bool ReportMissingBind(string key)
        {
            Debug.LogWarning($"Bind For {key} Missing");
            return false;
        }

        public static void ChangeValue(string key)
        {

        }

        static KeyCode GetKeyPressed(bool GetKeyDown = false)
        {
            foreach (var item in KeyCodes)
            {
                if (GetKeyDown)
                {
                    if (Input.GetKeyDown(item)) return item;
                }
                else
                {
                    if (Input.GetKey(item)) return item;
                }
            }
            return KeyCode.None;
        }
    }
    class InputUpdateCaller
    {
        public void Call(int interval)
        {
            while (true)
            {
                Task.Delay(interval);
                InputManager.Update();
            }
        }
    }
}
