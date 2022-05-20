using UnityEngine;

namespace Khatim_F2
{
    [CreateAssetMenu(fileName = "PlayerVisual_Data", menuName = "Player/PlayerVisualData")]
    public class PlayerVisualData : ScriptableObject
    {
        #region Public Variables
        [Space, Header("Player Data")]
        [Tooltip("Player Colour")]
        public Color playerColour = Color.white;

        [Tooltip("Player Name")]
        public string playerName = "Player";
        #endregion
    }
}