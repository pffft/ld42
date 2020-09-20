using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public enum Speed
    {
        FROZEN = 0,
        SNAIL = 10,
        SLOW = 15,
        MEDIUM_SLOW = 20,
        MEDIUM = 25,
        FAST = 35,
        VERY_FAST = 45,
        SNIPE = 50, // This is realistically the fastest speed you should use.
        LIGHTNING = 75
    };

    /// <summary>
    /// Keeps track of various useful positions, in the form of ProxyVector3s. 
    /// These can be used like Vector3s, but contain live updating data until they are
    /// queried with "GetValue()".
    /// 
    /// Also keeps track of arena positions.
    /// </summary>
    public static class Positions
    {
        #region Proxy vector3s

        /// <summary>
        /// The live player position.
        /// </summary>
        public static ProxyVector3 PLAYER_POSITION = new ProxyVector3(() => { return GameManager.Player.transform.position + CENTER; });

        /// <summary>
        /// Grabs the delayed player position. If the "PlayerLock" move is locked on, then
        /// this will return the player position at the time the move was run. Otherwise, this
        /// returns the same value as PLAYER_POSITION.
        /// <see cref="Moves.Basic.PlayerLock"/>
        /// </summary>
        public static ProxyVector3 DELAYED_PLAYER_POSITION = Moves.Basic.PlayerLock._delayed_player_position;

        // Experimental. Leads ahead of the player based on their current velocity and distance from boss.
        // This is quite realtime, but can be jittery as a result.
        public static ProxyVector3 LEADING_PLAYER_POSITION = new ProxyVector3(() =>
        {
            float distance = (GameManager.Boss.transform.position - GameManager.Player.transform.position).magnitude;
            //Vector3 offset = (distance / 2f * GameManager.Player.GetComponent<Rigidbody>().velocity.normalized);
            Vector3 offset = 1f * GameManager.Player.GetComponent<Rigidbody>().velocity.normalized;

            return PLAYER_POSITION.GetValue() + offset;
        });

        // Smooths the value of LEADING_PLAYER_POSITION using two samples over time.
        // This is less "realtime", but provides a smoother tracking.
        private static Vector3 last_lead = Vector3.zero;
        private static Vector3 curr_lead = Vector3.zero;
        public static ProxyVector3 SMOOTHED_LEADING_PLAYER_POSITION = new ProxyVector3(() =>
        {
            Vector3 raw_value = LEADING_PLAYER_POSITION.GetValue();

            last_lead = curr_lead;
            curr_lead = raw_value;

            return (last_lead + curr_lead) / 2.0f;

        });

        public static ProxyVector3 BOSS_POSITION = new ProxyVector3(() => { return GameManager.Boss.transform.position; });
        public static ProxyVector3 RANDOM_IN_ARENA = new ProxyVector3(() =>
        {
            float angle = Random.value * 360;
            float distance = Random.Range(0, GameManager.Arena.RadiusInWorldUnits);
            return distance * (Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward) + CENTER;
        });

        #endregion

        #region Arena Location constants
        public static readonly float BOSS_HEIGHT = 1.31f;
        private static readonly float FAR = 45f;
        private static readonly float MED = 30f;
        private static readonly float CLOSE = 15f;

        public static readonly Vector3 CENTER = new Vector3(0, BOSS_HEIGHT, 0);

        public static readonly Vector3 NORTH_FAR = new Vector3(0f, BOSS_HEIGHT, FAR);
        public static readonly Vector3 SOUTH_FAR = new Vector3(0f, BOSS_HEIGHT, -FAR);
        public static readonly Vector3 EAST_FAR = new Vector3(45f, BOSS_HEIGHT, 0);
        public static readonly Vector3 WEST_FAR = new Vector3(-45f, BOSS_HEIGHT, 0);

        public static readonly Vector3 NORTH_MED = new Vector3(0f, BOSS_HEIGHT, MED);
        public static readonly Vector3 SOUTH_MED = new Vector3(0f, BOSS_HEIGHT, -MED);
        public static readonly Vector3 EAST_MED = new Vector3(30f, BOSS_HEIGHT, 0);
        public static readonly Vector3 WEST_MED = new Vector3(-30f, BOSS_HEIGHT, 0);

        public static readonly Vector3 NORTH_CLOSE = new Vector3(0f, BOSS_HEIGHT, CLOSE);
        public static readonly Vector3 SOUTH_CLOSE = new Vector3(0f, BOSS_HEIGHT, -CLOSE);
        public static readonly Vector3 EAST_CLOSE = new Vector3(15f, BOSS_HEIGHT, 0);
        public static readonly Vector3 WEST_CLOSE = new Vector3(-15f, BOSS_HEIGHT, 0);
        #endregion
    }

}
