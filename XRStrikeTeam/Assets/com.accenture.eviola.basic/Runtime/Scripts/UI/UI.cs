using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Accenture.eviola.UI
{
    public class Utils
    {
        /// <summary>
        /// Associate a listener to a button.
        /// The idea is having a unique method instead of AddListener/RemoveListener
        /// </summary>
        static public bool SetButtonListener(Button btn, bool b, UnityAction callback) {
            return Misc.DoIfNotNull(btn, (Button _btn) => {
                if (b)
                {
                    _btn.onClick.AddListener(callback);
                }
                else {
                    _btn.onClick.RemoveListener(callback);
                }
            });
        }

        /// <summary>
        /// Associate a listener to a toggle.
        /// The idea is having a unique method instead of AddListener/RemoveListener
        /// </summary>
        static public bool SetToggleListener(Toggle tog, bool b, UnityAction<bool> callback) {
            return Misc.DoIfNotNull(tog, (Toggle _tog) => {
                if (b)
                {
                    _tog.onValueChanged.AddListener(callback);
                }
                else {
                    _tog.onValueChanged.RemoveListener(callback);
                }
            });
        }

        /// <summary>
        /// Associate a listener to a slider.
        /// The idea is having a unique method instead of AddListener/RemoveListener
        /// </summary>
        static public bool SetSliderListener(Slider sli, bool b, UnityAction<float> callback) {
            return Misc.DoIfNotNull(sli, (Slider _sli) => {
                if (b)
                {
                    _sli.onValueChanged.AddListener(callback);
                }
                else {
                    _sli.onValueChanged.RemoveListener(callback);
                }
            });
        }
    }
}