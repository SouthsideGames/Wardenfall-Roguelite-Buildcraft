using System;
using UnityEngine;

namespace SouthsideGames.DailyMissions
{
    public abstract class PopupBaseUI : MonoBehaviour
    {
       public static Action<PopupBaseUI> closed;
    }

}
