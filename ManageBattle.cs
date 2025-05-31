using Unity.Netcode;
using UnityEngine;
using LethalLib.Modules;
using System.Collections.Generic;
using PremiumScraps.Utils;
using System.Linq;

namespace Lethal_Battle
{
    internal class ManageBattle
    {
        public static void ItemsSpawner()
        {
            if (GameNetworkManager.Instance.localPlayerController.IsHost == false)
            {
                return;
            }

            List<Item> scraps = new List<Item>();
            string[] bannedScraps = { 
                "Boombox",
                "Flashlight",
                "Key",
                "LockPicker",
                "LungApparatus",
                "MapDevice",
                "ProFlashlight",
                "WalkieTalkie",
                "7Ball",
                "Bell",
                "BigBolt",
                "BottleBin",
                "Brush",
                "CashRegister",
                "ChemicalJug",
                "ClownHorn",
                "Cog1",
                "Dentures",
                "DustPan",
                "EggBeater",
                "EnginePart1",
                "FancyCup",
                "FancyLamp",
                "FishTestProp",
                "FlashLaserPointer",
                "GoldBar",
                "MagnifyingGlass",
                "MetalSheet",
                "MoldPan",
                "Mug",
                "PerfumeBottle",
                "Phone",
                "PickleJar",
                "PillBottle",
                "Ring",
                "RobotToy",
                "RubberDuck",
                "SodaCanRed",
                "SteeringWheel",
                "TeaKettle",
                "ToyCube",
                "RedLocustHive",
                "RadarBooster",
                "GunAmmo",
                "SprayPaint",
                "GiftBox",
                "WhoopieCushion",
                "WeedKillerBottle",
                "SoccerBall",
                "ControlPad",
                "GarbageLid",
                "PlasticCup",
                "ToiletPaperRolls",
                "Clock",
                "ToyTrain",
                "BeltBag",
                "Zeddog",

                "FrierenItem",
                "ChocoboItem",
                "AinzOoalGownItem",
                "HelmDominationItem",
                "TheKingItem",
                "HarryMasonItem",
                "CristalItem",
                "PuppySharkItem",
                "RupeeItem",
                "EaNasirItem",
                "SODAItem",
                "CroutonItem",
                "BalanItem",
                "MoogleItem",
                "GazpachoItem",
                "AbiItem",
            };

            /*
            foreach (var item in Items.scrapItems)
                scraps.Add(item.item);
            foreach (var item in Items.shopItems)
                scraps.Add(item.item);
            foreach (var item in Items.plainItems)
                scraps.Add(item.item);
            */
            foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                scraps.Add(item);

            scraps.RemoveAll(item => (bannedScraps.Contains(item.itemName)));
            //scraps.RemoveAll(item => (item == null || item.spawnPrefab == null || item.spawnPrefab.GetComponent<Scrap>() != null));

            Vector3 spawnPosition = new Vector3(24, -11, 12);

            for (int i = 0; i < scraps.Count; i++)
            {
                Plugin.log.LogError(scraps[i].name);
                //SpawnScrap(scraps[i], spawnPosition);
            }

            /*SpawnableMapObject scrapToSpawn = new SpawnableMapObject();
            Vector3 spawnPosition = new Vector3(24, -11, 12);
            for (int i = 0; i<StartOfRound.Instance.allPlayerObjects.Length; i++)
            {
                SpawnScrap(itemNameToSpawn, spawnPosition);
            }*/
        }

        private static void SpawnScrap(Item scrap, Vector3 position)
        {
            GameObject gameObject = Object.Instantiate(scrap.spawnPrefab, position,
                Quaternion.identity, RoundManager.Instance.spawnedScrapContainer.transform);

            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
            component.transform.rotation = Quaternion.Euler(component.itemProperties.restingRotation);
            component.fallTime = 0f;
            component.scrapValue = 0;
            component.NetworkObject.Spawn();
            component.FallToGround(true);
        }
    }
}
