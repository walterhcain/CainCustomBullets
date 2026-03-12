using System;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections;
using Rocket.Unturned.Events;
using System.Collections.Generic;

namespace walterhcain.CainCustomBullets
{
    public class RBullets : RocketPlugin<RBulletsConfig>
    {
        public static RBullets Instance;
        public static string Version = "1.1.0";
        public Dictionary<CSteamID, CSteamID> trackers;
        public Dictionary<CSteamID, DateTime> tracktimer;
        public DateTime lastcalled_2;

        protected override void Load()
        {
            Instance = this;
            DamageTool.damagePlayerRequested += RubberBulletsUpdated;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += CainMovement;
            Logger.Log("----------------------------");
            Logger.Log("Cain's Custom Bullets Plugin has been successfully loaded");
            Logger.Log("----------------------------");
            Logger.Log("Version " + Version);
            if (Configuration.Instance.rubberEnabled)
            {
                Logger.Log("Rubber Bullets are enabled");
            }
            if (Configuration.Instance.tranqEnabled)
            {
                Logger.Log("Tranquilizer Bullets are enabled");
            }
            if (Configuration.Instance.trainingEnabled)
            {
                Logger.Log("Training Bullets are enabled");
            }
            if (Configuration.Instance.rubberEnabled)
            {
                Logger.Log("Zombie Bullets are enabled");
            }
            if (Configuration.Instance.trackingEnabled)
            {
                Logger.Log("Tracking Bullets are enabled");
                trackers = new Dictionary<CSteamID, CSteamID>();
                tracktimer = new Dictionary<CSteamID, DateTime>();
                lastcalled_2 = DateTime.Now;
            }
            Logger.Log("----------------------------");
        }

        

        protected override void Unload()
        {
            DamageTool.damagePlayerRequested -= RubberBulletsUpdated;
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= CainMovement;
            Logger.Log("Cain's Custom Bullets Plugin has been successfully unloaded");
            
        }

        private void CainMovement(UnturnedPlayer player, UnityEngine.Vector3 position)
        {
            if (Configuration.Instance.trackingEnabled)
            {
                if (trackers.Count > 0)
                {
                    foreach (KeyValuePair<CSteamID, CSteamID> kvp in trackers)
                    {
                        if(kvp.Value == player.CSteamID)
                        {
                            UnturnedPlayer pl = UnturnedPlayer.FromCSteamID(kvp.Key);
                            pl.Player.quests.askSetMarker(pl.CSteamID, true, player.Position);
                            pl.Player.quests.save();
                        }
                    }
                }
            }
        }

        private void RubberBulletsUpdated(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            try
            {
                if (parameters.killer != Provider.server)
                {


                    if (parameters.player == null)
                    {
                        return;
                    }
                    UnturnedPlayer player = UnturnedPlayer.FromCSteamID(parameters.killer);
                    if (player == null)
                    {
                        return;
                    }

                    ItemGunAsset currentWeapon;
                    ItemAsset currentEquipped;
                    ushort magId = 0;

                    currentEquipped = player.Player.equipment.asset;
                    if (currentEquipped == null)
                    {
                        return;
                    }
                    if (currentEquipped.type != EItemType.GUN)
                    {
                        return;
                    }
                    currentWeapon = (ItemGunAsset)currentEquipped;
                    if (currentWeapon == null)
                    {
                        return;
                    }
                    magId = player.Player.equipment.state[8];
                    if (magId == 0 || Rocket.Unturned.Items.UnturnedItems.GetItemAssetById(magId).type != EItemType.MAGAZINE)
                    {
                        magId = currentWeapon.getMagazineID();
                    }
                    if (Instance.Configuration.Instance.rubberEnabled && Instance.Configuration.Instance.rubberList.Contains(magId))
                    {
                        parameters.player.life.breakLegs();
                        parameters.player.life.save();
                        Logger.Log(player.CharacterName + " successfully broke " + UnturnedPlayer.FromPlayer(parameters.player).CharacterName + "'s legs");
                    }
                    if (Instance.Configuration.Instance.tranqEnabled && Instance.Configuration.Instance.tranqList.Contains(magId))
                    {
                        parameters.player.life.serverModifyHallucination(40);
                        parameters.player.life.serverModifyWater(45);
                        parameters.player.life.serverModifyFood(20);
                        parameters.player.life.serverModifyVirus(35);
                        parameters.player.life.save();
                       // hurtPlayer.life.askView((byte)(20 * (1.0 - (double)hurtPlayer.skills.mastery(1, 2))));

                        Logger.Log(player.CharacterName + " successfully tranquilized " + UnturnedPlayer.FromPlayer(parameters.player).CharacterName + "'s legs");
                    }
                    if (Instance.Configuration.Instance.trainingEnabled && Instance.Configuration.Instance.trainingList.Contains(magId))
                    {
                        UnturnedChat.Say(player, "Your target is at " + UnityEngine.Vector3.Distance(UnturnedPlayer.FromPlayer(parameters.player).Position, player.Position) + "m away");
                        UnturnedChat.Say(player, "You hit them in the " + parameters.limb.ToString().ToLower());
                        UnturnedChat.Say(UnturnedPlayer.FromPlayer(parameters.player), "You got shot in the " + parameters.limb.ToString().ToLower());
                        parameters.damage = 0;
                    }
                    if (Instance.Configuration.Instance.zombieEnabled && Instance.Configuration.Instance.zombieList.Contains(magId))
                    {
                        parameters.damage = 0;
                        UnturnedPlayer up = UnturnedPlayer.FromPlayer(parameters.player);
                        UnityEngine.Vector3 pos = up.Player.transform.position;
                        
                        byte bound = up.Player.movement.bound;
                        EZombieSpeciality speciality = EZombieSpeciality.BOSS_NUCLEAR;
                        byte type = 1;
                        byte shirt = (byte)UnityEngine.Random.Range(0, 1);
                        byte pants = (byte)UnityEngine.Random.Range(0, 1);
                        byte hat = (byte)UnityEngine.Random.Range(0, 1);
                        byte gear = (byte)UnityEngine.Random.Range(0, 1);
                        byte move = (byte)UnityEngine.Random.Range(0, 4);
                        byte idle = (byte)UnityEngine.Random.Range(0, 3);
                        UnityEngine.Vector3 point = up.Position;
                        point.y += 0.1f;
                        ZombieManager.instance.addZombie(bound, type, (byte)speciality, shirt, pants, hat, gear, move, idle, point, UnityEngine.Random.Range(0f, 360f), false);
                        UnturnedChat.Say(player.CharacterName + " successfully spawned a zombie on " + UnturnedPlayer.FromPlayer(parameters.player).CharacterName);
                    }
                    if (Instance.Configuration.Instance.trackingEnabled && Instance.Configuration.Instance.trackList.Contains(magId))
                    {
                        parameters.damage = 10;
                        UnturnedPlayer up = UnturnedPlayer.FromPlayer(parameters.player);
                        UnturnedPlayer pl = UnturnedPlayer.FromCSteamID(parameters.killer);
                        UnityEngine.Vector3 pos = up.Player.transform.position;

                        trackers.Add(parameters.killer, up.CSteamID);
                        tracktimer.Add(parameters.killer, DateTime.Now.AddSeconds(Configuration.Instance.tracktime));
                        pl.Player.quests.askSetMarker(pl.CSteamID, true, player.Position);
                        pl.Player.quests.save();
                        UnturnedChat.Say(pl, "You have begun tracking " + up.CharacterName);

                    }
                }
                else
                {
                    return;
                }
            }
            catch (System.Exception e)
            {
                Logger.Log(e);
            }
        }
        void FixedUpdate()
        {
            if (Configuration.Instance.trackingEnabled)
            {
                if ((DateTime.Now - lastcalled_2).TotalMilliseconds >= 200)
                {
                    if (tracktimer.Count > 0)
                    {
                        bool a = false;
                        CSteamID cid = (CSteamID)0;
                        foreach(KeyValuePair<CSteamID, DateTime> kvp in tracktimer)
                        {
                            if(kvp.Value <= DateTime.Now)
                            {
                                a = true;
                                cid = kvp.Key;
                            }
                        }
                        trackers.Remove(cid);
                        tracktimer.Remove(cid);

                    }
                    lastcalled_2 = DateTime.Now;
                }
            }
        }
    }
}
