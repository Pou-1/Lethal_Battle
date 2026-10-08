using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lethal_Battle.codes
{
    internal static class ClosestPlayerAndPath
    {
        private static bool generatingPath = false;
        private static float nextPathTime = 0f;
        private static Queue<Vector3> footprintQueue = new Queue<Vector3>();
        private static Coroutine? closestOpponentPathCoroutine;
        private static Coroutine? footprintCoroutine;
        private static GameObject? buddy;
        private static CoroutineRunner? coroutineBuddy;

        public static void Initialize()
        {
            if (buddy != null)
                return;

            buddy = new GameObject("Buddy");
            Object.DontDestroyOnLoad(buddy);

            coroutineBuddy = buddy.AddComponent<CoroutineRunner>();
        }

        private static IEnumerator ClosestOpponentPathLoop()
        {
            while (Plugin.hasBattleStarted)
            {
                PlayerControllerB? localPlayer = GameNetworkManager.Instance.localPlayerController;

                if (localPlayer == null || !localPlayer.IsSpawned || !localPlayer.isPlayerControlled || localPlayer.isPlayerDead)
                {
                    StopFootprintCoroutine();

                    yield return new WaitForSeconds(1f);
                    continue;
                }

                PlayerControllerB? opponent = GetClosestOpponentToLocalPlayer();

                if (opponent == null)
                {
                    StopFootprintCoroutine();

                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

                if (!generatingPath && Time.time >= nextPathTime)
                {
                    nextPathTime = Time.time + 1.5f;

                    Queue<Vector3> path = PathSystem.GeneratePathPoints(localPlayer.transform.position, opponent.transform.position, 1.5f);

                    if (path.Count > 0)
                    {
                        footprintQueue = path;

                        if (footprintCoroutine == null)
                            footprintCoroutine = coroutineBuddy!.StartCoroutine(SpawnFootprints());
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }

            closestOpponentPathCoroutine = null;
            StopFootprintCoroutine();
        }

        #region closest opponent

        public static PlayerControllerB? GetClosestOpponentToLocalPlayer()
        {
            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;

            if (localPlayer != null)
            {
                PlayerControllerB? closestOpponent = null;
                float closestOpponentDistance = float.MaxValue;

                foreach (PlayerControllerB opponent in StartOfRound.Instance.allPlayerScripts)
                {
                    if (opponent != null && opponent != localPlayer && opponent.isPlayerControlled && opponent.IsSpawned && !opponent.isPlayerDead)
                    {
                        float distance = Vector3.Distance(localPlayer.transform.position, opponent.transform.position);

                        if (distance < closestOpponentDistance)
                        {
                            closestOpponentDistance = distance;
                            closestOpponent = opponent;
                        }
                    }
                }
                return closestOpponent;
            }
            return null;
        }

        public static void StartClosestOpponentPath()
        {
            if (Plugin.hasBattleStarted)
            {
                Initialize();

                if (coroutineBuddy != null && closestOpponentPathCoroutine == null)
                {
                    nextPathTime = 0f;

                    closestOpponentPathCoroutine = coroutineBuddy.StartCoroutine(ClosestOpponentPathLoop());
                } else
                    Plugin.log?.LogError("PATH Coroutine runner is null :c, can't start path search");
            } else
                Plugin.log?.LogError("Can't start closest opponent path because battle didn't start :c");
        }

        public static void StopClosestOpponentPath()
        {
            if (coroutineBuddy != null)
            {
                if (closestOpponentPathCoroutine != null)
                {
                    coroutineBuddy.StopCoroutine(closestOpponentPathCoroutine);
                    closestOpponentPathCoroutine = null;
                }

                if (footprintCoroutine != null)
                {
                    coroutineBuddy.StopCoroutine(footprintCoroutine);
                    footprintCoroutine = null;
                }
            }

            footprintQueue.Clear();
            generatingPath = false;
            nextPathTime = 0f;
        }

        #endregion closest opponent

        #region footprints

        private static void StopFootprintCoroutine()
        {
            if (coroutineBuddy != null && footprintCoroutine != null)
                coroutineBuddy.StopCoroutine(footprintCoroutine);

            footprintCoroutine = null;
            footprintQueue.Clear();
            generatingPath = false;
        }

        private static IEnumerator SpawnFootprints()
        {
            generatingPath = true;

            Vector3? previous = null;
            bool leftFoot = true;

            while (footprintQueue.Count > 0)
            {
                Vector3 current = footprintQueue.Dequeue();

                Vector3 direction;

                if (previous.HasValue)
                {
                    direction =
                        (current - previous.Value).normalized;
                }
                else if (footprintQueue.Count > 0)
                {
                    direction = (footprintQueue.Peek() - current).normalized;
                }
                else
                {
                    PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;

                    direction = localPlayer.transform.forward;
                }

                Vector3 perp = Vector3.Cross(Vector3.up, direction).normalized;

                Vector3 offset = perp * (leftFoot ? -0.2f : 0.2f);

                offset *= Random.Range(0.9f, 1.1f);

                Vector3 footprintPosition = current + offset;

                if (Plugin.Instance.FootprintPrefab != null)
                {
                    PathSystem.SpawnFootprint(Plugin.Instance.FootprintPrefab, footprintPosition, direction, 2f);
                }

                previous = current;
                leftFoot = !leftFoot;

                yield return new WaitForSeconds(0.1f);
            }

            generatingPath = false;
            footprintCoroutine = null;
        }
        
        #endregion footprints

        private class CoroutineRunner : MonoBehaviour {}
    }
}