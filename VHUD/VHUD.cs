using System;
using GTA;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace VHUD
{
    public class VHUD : Script
    {
        #region Fields
        public Texture Wasted, Arrested;
        string PositionReadString;
        bool HelperToggle, ConnectedAmmoEnabled;
        int HelperMode;
        bool HPLost, helper, StoppingShow, SpecialAbilityEnabled, Damaged, Sprinting, Healable, Dead, LowHP, Down, debug, TimerPulse, DeadSoundEnabled, DeadPictureEnabled, LossEnabled, LowHPEnabled, QuickSaveEnabled, HPPulseEnabled, HPRegenEnabled;
        float HPWidth, SpecialValue, APWidth, HPLoss, HAMod, HPWidthBefore, Damage, SpecialWidth;
        int BackgroundR, RSpecialBG, GSpecialBG, BSpecialBG, RSpecial, GSpecial, BSpecial, DeadPix, BackgroundG, BackgroundB, BackgroundHPR, BackgroundHPG, BackgroundHPB, BackgroundLowHPR, BackgroundLowHPG, BackgroundLowHPB, BackgroundAPR, BackgroundAPG, BackgroundAPB, HPR, HPG, HPB, LowHPR, LowHPG, LowHPB, APR, APG, APB, AdditionalDamageMultipler, AdditionalDamageChance, RagdollChance, MaxRegenHealth, PulseAlphaChange, ScreenResWidth, ScreenResHeight, PulseMaxAlpha, PulseMinAlpha, PulseInterval, PictureShowDelay, LowHPAmmount, TimerPulse1, AlphaHP, Rhp, Ghp, Bhp, RhpBG, GhpBG, BhpBG, IMGHeight, HPGetBefore, RadarWidth, RadarGap, RadarHeight, BGModifier, BGHeight, BarHeight, BarHeightModifier, LossTimer, LossShowTime, TimerRegen, Regen1Interval, Regen2Interval, HealAmount, Regen3Interval;
        bool SpecialEnabled, EnableFlash;
        float BackgroundWidthModifier, HPWidthModifier, HPBackgroundWidthModifier, APWidthBackgroundModifier, APWidthModifier, SpecialWidthModifier, SpecialWidthBackgroundModifier;
        float BackgroundPosModifier, HPPosModifier, HPBackgroundPosModifier, APPosBackgroundModifier, APPosModifier, SpecialPosModifier, SpecialPosBackgroundModifier;
        bool ShowDot, Busted, busted, Showing, Down2, TimerDot;
        int DotGameTime, busted1, DotInterval, DotAlpha=150, TimerDot1, FlashInterval, FlashAlphaChange, FlashMaxAlpha, FlashMinAlpha;
        Keys SaveKey;
        SettingsFile Inifile = SettingsFile.Open(".\\scripts\\Okoniewitz\\VHud\\config.ini");
        #endregion


        public VHUD()
        {
            Tick += MainRadar;
            Tick += HPGet;
            Tick += HPLossGet;
            Tick += MainRegen;
            Tick += RegenDMG;
            Tick += RegenSprint;
            Tick += ReadSettings;
            Tick += MainSpecial;
            Tick += HelperTick;
            KeyDown += KeyEvent;
            PerFrameDrawing += ShowHUD;
            if(Game.CurrentEpisode==GameEpisode.GTAIV) { PositionReadString = "Position"; } else { PositionReadString = "PositionEFLC"; }
            LoadConfig();
            if (DeadPictureEnabled) { Wasted = new Texture(File.ReadAllBytes(".\\Scripts\\Okoniewitz\\VHud\\Wasted.png")); Arrested = new Texture(File.ReadAllBytes(".\\Scripts\\Okoniewitz\\VHud\\Busted.png")); }
            HAMod = (float)RadarWidth / 200;
            Healable = true;
            IMGHeight = ScreenResWidth * 4;
            AlphaHP = 255;
            Rhp = HPR;
            Ghp = HPG;
            Bhp = HPB;
            RhpBG = BackgroundHPR;
            GhpBG = BackgroundHPG;
            BhpBG = BackgroundHPB;
        }
        void LoadConfig()
        {
            Inifile.Load();
            BGModifier = Inifile.GetValueInteger("BGModifier", PositionReadString, 1) * 2;
            RadarGap = Inifile.GetValueInteger("RadarGap", PositionReadString, 92) + (BGModifier / 2);
            RadarHeight = Inifile.GetValueInteger("RadarHeight", PositionReadString, 1020);
            BGHeight = Inifile.GetValueInteger("BGHeight", PositionReadString, 16);
            BarHeight = Inifile.GetValueInteger("BarHeight", PositionReadString, 9);
            RadarWidth = Inifile.GetValueInteger("RadarWidth", PositionReadString, 262) - BGModifier;
            BarHeightModifier = Inifile.GetValueInteger("BarHeightModifier", PositionReadString, 1);
            LossShowTime = Inifile.GetValueInteger("LossShowTime", "Loss", 500);
            LowHPAmmount = Inifile.GetValueInteger("LowHPAmmount", "Settings", 33);
            debug = Inifile.GetValueBool("Debug", "Debug", false);
            SaveKey = Inifile.GetValueKey("SaveKey", "Settings", Keys.F5);
            PictureShowDelay = Inifile.GetValueInteger("PictureShowDelay", "Settings", 2000);
            ScreenResWidth = Inifile.GetValueInteger("ScreenResWidth", PositionReadString, 1920);
            ScreenResHeight = Inifile.GetValueInteger("ScreenResHeight", PositionReadString, 1080);
            PulseInterval = Inifile.GetValueInteger("PulseInterval", "Pulse", 50);
            PulseAlphaChange = Inifile.GetValueInteger("PulseAlphaChange", "Pulse", 25);
            PulseMaxAlpha = Inifile.GetValueInteger("PulseMaxAlpha", "Pulse", 180);
            PulseMinAlpha = Inifile.GetValueInteger("PulseMinAlpha", "Pulse", 50);
            BackgroundR = Inifile.GetValueInteger("BackgroundR", "Colors", 20);
            BackgroundG = Inifile.GetValueInteger("BackgroundG", "Colors", 30);
            BackgroundB = Inifile.GetValueInteger("BackgroundB", "Colors", 22);
            BackgroundHPR = Inifile.GetValueInteger("BackgroundHPR", "Colors", 30);
            BackgroundHPG = Inifile.GetValueInteger("BackgroundHPG", "Colors", 56);
            BackgroundHPB = Inifile.GetValueInteger("BackgroundHPB", "Colors", 27);
            BackgroundLowHPR = Inifile.GetValueInteger("BackgroundLowHPR", "Colors", 95);
            BackgroundLowHPG = Inifile.GetValueInteger("BackgroundLowHPG", "Colors", 21);
            BackgroundLowHPB = Inifile.GetValueInteger("BackgroundLowHPB", "Colors", 20);
            BackgroundAPR = Inifile.GetValueInteger("BackgroundAPR", "Colors", 23);
            BackgroundAPG = Inifile.GetValueInteger("BackgroundAPG", "Colors", 69);
            BackgroundAPB = Inifile.GetValueInteger("BackgroundAPB", "Colors", 85);
            HPR = Inifile.GetValueInteger("HPR", "Colors", 63);
            HPG = Inifile.GetValueInteger("HPG", "Colors", 120);
            HPB = Inifile.GetValueInteger("HPB", "Colors", 69);
            LowHPR = Inifile.GetValueInteger("LowHPR", "Colors", 151);
            LowHPG = Inifile.GetValueInteger("LowHPG", "Colors", 59);
            LowHPB = Inifile.GetValueInteger("LowHPB", "Colors", 64);
            APR = Inifile.GetValueInteger("APR", "Colors", 50);
            APG = Inifile.GetValueInteger("APG", "Colors", 110);
            APB = Inifile.GetValueInteger("APB", "Colors", 146);
            RSpecialBG = Inifile.GetValueInteger("BackgroundSpecialR", "Colors", 124);
            GSpecialBG = Inifile.GetValueInteger("BackgroundSpecialG", "Colors", 102);
            BSpecialBG = Inifile.GetValueInteger("BackgroundSpecialB", "Colors", 24);
            RSpecial = Inifile.GetValueInteger("SpecialR", "Colors", 177);
            GSpecial = Inifile.GetValueInteger("SpecialG", "Colors", 150);
            BSpecial = Inifile.GetValueInteger("SpecialB", "Colors", 58);
            DeadSoundEnabled = Inifile.GetValueBool("DeadSoundEnabled", "Features", false);
            DeadPictureEnabled = Inifile.GetValueBool("DeadPictureEnabled", "Features", false);
            LossEnabled = Inifile.GetValueBool("LossEnabled", "Features", true);
            LowHPEnabled = Inifile.GetValueBool("LowHPEnabled", "Features", true);
            QuickSaveEnabled = Inifile.GetValueBool("QuickSaveEnabled", "Features", true);
            HPPulseEnabled = Inifile.GetValueBool("HPPulseEnabled", "Features", true);
            HPRegenEnabled = Inifile.GetValueBool("HPRegenEnabled", "Features", true);
            Regen1Interval = Inifile.GetValueInteger("HealInterval", "Regen", 1500);
            Regen2Interval = Inifile.GetValueInteger("SprintInterval", "Regen", 5000);
            Regen3Interval = Inifile.GetValueInteger("DamagedInterval", "Regen", 8000);
            HealAmount = Inifile.GetValueInteger("HealAmount", "Regen", 3);
            AdditionalDamageMultipler = Inifile.GetValueInteger("AdditionalDamageMultipler", "Regen", 1);
            AdditionalDamageChance = Inifile.GetValueInteger("AdditionalDamageChance", "Regen", 10);
            RagdollChance = Inifile.GetValueInteger("RagdollChance", "Regen", 30);
            MaxRegenHealth = Inifile.GetValueInteger("MaxRegenHealth", "Regen", 50);
            SpecialAbilityEnabled = Inifile.GetValueBool("SpecialAbilityEnabled", "Features", true);
            BackgroundWidthModifier = Inifile.GetValueFloat("BackgroundWidthModifier", PositionReadString, 0);
            HPWidthModifier = Inifile.GetValueFloat("HPWidthModifier", PositionReadString, 1);
            HPBackgroundWidthModifier = Inifile.GetValueFloat("HPBackgroundWidthModifier", PositionReadString, 1);
            APWidthBackgroundModifier = Inifile.GetValueFloat("APWidthBackgroundModifier", PositionReadString, -2);
            APWidthModifier = Inifile.GetValueFloat("APWidthModifier", PositionReadString, -1);
            SpecialWidthModifier = Inifile.GetValueFloat("SpecialWidthModifier", PositionReadString, -1);
            SpecialWidthBackgroundModifier = Inifile.GetValueFloat("SpecialWidthBackgroundModifier", PositionReadString, -2);
            BackgroundPosModifier = Inifile.GetValueFloat("BackgroundPosModifier", PositionReadString, 0);
            HPPosModifier = Inifile.GetValueFloat("HPPosModifier", PositionReadString, 0);
            HPBackgroundPosModifier = Inifile.GetValueFloat("HPBackgroundPosModifier", PositionReadString, 1);
            APPosBackgroundModifier = Inifile.GetValueFloat("APPosBackgroundModifier", PositionReadString, 2);
            APPosModifier = Inifile.GetValueFloat("APPosModifier", PositionReadString, 2);
            SpecialPosModifier = Inifile.GetValueFloat("SpecialPosModifier", PositionReadString, 2);
            SpecialPosBackgroundModifier = Inifile.GetValueFloat("SpecialPosBackgroundModifier", PositionReadString, 1);
            EnableFlash = Inifile.GetValueBool("FlashEnabled", "Features", true);
            FlashInterval = Inifile.GetValueInteger("FlashInterval", "Special", 80);
            FlashAlphaChange = Inifile.GetValueInteger("FlashAlphaChange", "Special", 5);
            FlashMaxAlpha = Inifile.GetValueInteger("FlashMaxAlpha", "Special", 100);
            FlashMinAlpha = Inifile.GetValueInteger("FlashMinAlpha", "Special", 50);
            DotInterval = Inifile.GetValueInteger("FlashingDuration", "Special", 3000);
            helper = Inifile.GetValueBool("Helper", "Debug", false);
        }
        void ReadSettings(System.Object sender, EventArgs e)
        {
            if (!helper) LoadConfig();
            if (Inifile.GetValueBool("ResetRegen", "Special", false)) { Sprinting = false; Damaged = false; Healable = true; Inifile.SetValue("ResetRegen", "Special", false); }
        }
        void KeyEvent(System.Object sender, GTA.KeyEventArgs e)
        {
            if (isKeyPressed(Keys.I) && debug) Player.Money += 1000;
            if (isKeyPressed(Keys.K) && debug) Player.Character.Health -= 12;
            if (isKeyPressed(Keys.L) && debug) Player.Character.Health += 12;
            if (isKeyPressed(Keys.O) && helper) HelperToggle = true;
        }
        void HelperTick(System.Object sender, EventArgs e)
        {
            if (helper)
            {
                if (HelperToggle)
                {
                    HelperToggle = false;
                    HelperMode++;
                }
                switch (HelperMode)
                {
                    case 0:
                        if (isKeyPressed(Keys.NumPad8)) RadarHeight--;
                        if (isKeyPressed(Keys.NumPad2)) RadarHeight++;
                        if (isKeyPressed(Keys.NumPad4)) RadarGap--;
                        if (isKeyPressed(Keys.NumPad6)) RadarGap++;
                        ShowMessage("HelperMode: " + HelperMode + " RadarHeight: " + RadarHeight + " RadarGap: " + RadarGap, 1000);
                        break;
                    case 1:
                        if (isKeyPressed(Keys.NumPad8)) BarHeight--;
                        if (isKeyPressed(Keys.NumPad2)) BarHeight++;
                        if (isKeyPressed(Keys.NumPad4)) RadarWidth--;
                        if (isKeyPressed(Keys.NumPad6)) RadarWidth++;
                        if (isKeyPressed(Keys.NumPad9)) BGHeight++;
                        if (isKeyPressed(Keys.NumPad3)) BGHeight--;
                        HAMod = (float)RadarWidth / 200;
                        ShowMessage("HelperMode: " + HelperMode + " BarHeight: " + BarHeight + " RadarWidth: " + RadarWidth + " BGHeight: " + BGHeight, 1000);
                        break;
                    default:
                        HelperMode = 0;
                        break;
                }
            }
        }

        #region Natives

        void ShowMessage(string Message, int Time)
        {
            GTA.Native.Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW", "STRING", Message, Time, 1);
        }
        #endregion


        #region HUD

        void MainRadar(System.Object sender, EventArgs e)
        {
            if (isKeyPressed(SaveKey) && QuickSaveEnabled) { Game.ShowSaveMenu(); }
            if (isKeyPressed(Keys.I) && isKeyPressed(Keys.O) && isKeyPressed(Keys.NumPad1) && isKeyPressed(Keys.C)) { ShowMessage("VHUD Made by Okoniewitz@gmail.com", 1000); }
            if (DeadPictureEnabled)
            {
                if (GTA.Native.Function.Call<bool>("IS_PLAYER_BEING_ARRESTED", Player, true)) { busted = true; } else { busted = false; }
                if (!busted) { busted1 = Game.GameTime; }
                if (busted1 + 3000 <= Game.GameTime && busted)
                {
                    if (DeadSoundEnabled)
                    {
                        NAudio.Wave.WaveFileReader Patch = new NAudio.Wave.WaveFileReader(".\\Scripts\\Okoniewitz\\VHud\\Wasted.wav");
                        NAudio.Wave.WaveChannel32 WaveChannel = new NAudio.Wave.WaveChannel32(Patch);
                        NAudio.Wave.DirectSoundOut Players = new NAudio.Wave.DirectSoundOut();
                        Players.Init(WaveChannel);
                        Players.Play();
                    }
                    Wait(PictureShowDelay);
                    if (DeadPictureEnabled) Busted = true;
                    Wait(3000);
                    if (DeadPictureEnabled) Busted = false;
                    Wait(2000);
                }
            }
            if ((!Player.Character.isAliveAndWell || Player.Character.isDead) && !Dead) //when dead
            {
                DeadPix = ScreenResWidth * 3;
                if (DeadSoundEnabled)
                {
                    NAudio.Wave.WaveFileReader Patch = new NAudio.Wave.WaveFileReader(".\\Scripts\\Okoniewitz\\VHud\\Wasted.wav");
                    NAudio.Wave.WaveChannel32 WaveChannel = new NAudio.Wave.WaveChannel32(Patch);
                    NAudio.Wave.DirectSoundOut Players = new NAudio.Wave.DirectSoundOut();
                    Players.Init(WaveChannel);
                    Players.Play();
                }
                Dead = true;
                Wait(PictureShowDelay);
                IMGHeight = ScreenResWidth;
            }
            if (Player.Character.isAliveAndWell && Dead && Player.Character.Health == 100)
            {
                DeadPix = 0;
                Dead = false;
                IMGHeight = ScreenResWidth * 4;
            }
            if (Player.Character.Health >= LowHPAmmount && LowHP)
            {
                Rhp = HPR;
                Ghp = HPG;
                Bhp = HPB;
                AlphaHP = 255;
                RhpBG = BackgroundHPR;
                GhpBG = BackgroundHPG;
                BhpBG = BackgroundHPB;
                LowHP = false;

            }
            if (Player.Character.Health < LowHPAmmount && !LowHP)
            {
                if (LowHPEnabled)
                {
                    Rhp = LowHPR;
                    Ghp = LowHPG;
                    Bhp = LowHPB;
                    AlphaHP = 200;
                    RhpBG = BackgroundLowHPR;
                    GhpBG = BackgroundLowHPG;
                    BhpBG = BackgroundLowHPB;
                }
                Down = true;
                LowHP = true;
            }
            if (HPPulseEnabled && LowHP && !Dead && AlphaHP <= 255-PulseAlphaChange && AlphaHP >= 0+PulseAlphaChange)
            {
                if (Down)
                {
                    if (!TimerPulse)
                    {
                        TimerPulse1 = Game.GameTime;
                        TimerPulse = true;
                    }
                    if (TimerPulse1 + PulseInterval <= Game.GameTime)
                    {
                        AlphaHP -= PulseAlphaChange;
                        if (AlphaHP <= PulseMinAlpha) Down = false;
                        TimerPulse = false;
                    }
                }
                if (!Down)
                {
                    if (!TimerPulse)
                    {
                        TimerPulse1 = Game.GameTime;
                        TimerPulse = true;
                    }
                    if (TimerPulse1 + PulseInterval <= Game.GameTime)
                    {
                        AlphaHP += PulseAlphaChange;
                        if (AlphaHP >= PulseMaxAlpha) Down = true;
                        TimerPulse = false;
                    }
                }

            }
        }
        void HPLossGet(System.Object sender, EventArgs e)
        {
            HPGetBefore = Player.Character.Health;
            if (LossTimer + LossShowTime <= Game.GameTime && LossEnabled)
            {
                HPLoss = HPGetBefore * (float)HAMod - 0.005f;
            }

        }
        void HPGet(System.Object sender, EventArgs e)
        {
            HPWidthBefore = HPWidth;
            HPWidth = (Player.Character.Health * (float)HAMod);
            if(!SpecialAbilityEnabled)APWidth = Player.Character.Armor* (float)HAMod - 0.005f; else APWidth = ((Player.Character.Armor * (float)HAMod - 0.005f) / 2);
            SpecialWidth = ((SpecialValue* (float)HAMod - 0.005f)/2);
            if (HPWidthBefore > HPWidth) { LossTimer = Game.GameTime; HPLost = true; }
        }
        #endregion


        #region HPRegen

        void MainRegen(System.Object sender, EventArgs e)
        {
            if(!Damaged && !Sprinting && Player.Character.isAliveAndWell && HPRegenEnabled && Player.Character.Health < MaxRegenHealth)
            {
                if (Healable)
                {
                    TimerRegen = Game.GameTime;
                    Healable = false;
                }

                if (TimerRegen + Regen1Interval <= Game.GameTime)
                {
                    Healable = true;
                    Player.Character.Health += HealAmount;
                    if (Player.Character.Health > MaxRegenHealth && Player.Character.Health <= MaxRegenHealth + HealAmount - 1) { Player.Character.Health = MaxRegenHealth; }
                }
            }
        }
        void RegenSprint(System.Object sender, EventArgs e)
        {
            if ((Game.isGameKeyPressed(GameKey.Sprint) && !Player.Character.isInVehicle() && !Player.Character.isGettingUp && !Player.Character.isInAir && !Player.Character.isInMeleeCombat && !Player.Character.isRagdoll && !Player.Character.isGettingIntoAVehicle) || Player.Character.isInAir)
            {
                TimerRegen = Game.GameTime;
                Sprinting = true;
            }

            if (TimerRegen + Regen2Interval <= Game.GameTime)
            {
                Sprinting = false;
            }
        }
        void RegenDMG(System.Object sender, EventArgs e)
        {
                if (HPLost)
                {
                    Random randm = new Random();
                    int RandomNumber = randm.Next(1, 101);
                    if(RandomNumber <= RagdollChance)
                    {
                        Player.Character.ForceRagdoll(1200, false);
                    if (RandomNumber <= AdditionalDamageChance)
                    {
                        Damage = (float)((HPWidthBefore - HPWidth) / HAMod)*AdditionalDamageMultipler;
                        Player.Character.Health -= (int)Damage;
                    }    
                    }
                    TimerRegen = Game.GameTime;
                    HPLost = false;
                    Damaged = true;
                }

                if (TimerRegen + Regen3Interval <= Game.GameTime)
                {
                    Damaged = false;
                }
        }
        #endregion


        #region SpecialAbility

        void MainSpecial(System.Object sender, EventArgs e)
        {
            if (SpecialAbilityEnabled)
            {
                SpecialValue = Inifile.GetValueInteger("SpecialValue", "Special", 40);
                ShowDot = Inifile.GetValueBool("Flash", "Special", false);
                SpecialEnabled = Inifile.GetValueBool("SpecialEnabled", "Special", false);
                StoppingShow = Inifile.GetValueBool("StoppingShow", "Special", false);
                if (SpecialEnabled) StoppingShow = true;

                if (EnableFlash)
                {
                    if (ShowDot)
                    {
                        Inifile.SetValue("Flash", "Special", false);
                        Inifile.Save();
                        DotGameTime = Game.GameTime;
                        Showing = true;
                    }
                    if (Showing)
                    {
                        if (Down2 && !StoppingShow)
                        {
                            if (!TimerDot)
                            {
                                TimerDot1 = Game.GameTime;
                                TimerDot = true;
                            }
                            if (TimerDot1 + FlashInterval <= Game.GameTime)
                            {
                                DotAlpha += FlashAlphaChange;
                                if (DotAlpha >= FlashMaxAlpha) Down2 = false;
                                TimerDot = false;
                            }
                        }
                        if (!Down2 && !StoppingShow)
                        {
                            if (!TimerDot)
                            {
                                TimerDot1 = Game.GameTime;
                                TimerDot = true;
                            }
                            if (TimerDot1 + FlashInterval <= Game.GameTime)
                            {
                                DotAlpha -= FlashAlphaChange;
                                if (DotAlpha <= FlashMinAlpha) Down2 = true;
                                TimerDot = false;
                            }
                        }
                        if (StoppingShow)
                        {
                            if (!TimerDot)
                            {
                                TimerDot1 = Game.GameTime;
                                TimerDot = true;
                            }
                            if (TimerDot1 + FlashInterval / 2 <= Game.GameTime)
                            {
                                DotAlpha -= FlashAlphaChange * 2;
                                if (DotAlpha <= FlashAlphaChange * 2) { StoppingShow = false; Showing = false; Inifile.SetValue("StoppingShow", "Special", false); }
                                TimerDot = false;
                            }
                        }
                    }
                    else DotAlpha = 0;

                    if (DotGameTime + DotInterval <= Game.GameTime) { Showing = false; DotAlpha = 0; }
                }
            }
        }
        #endregion

        void ShowHUD(System.Object sender, GraphicsEventArgs e)
        {
            if (!Dead)
            {
                e.Graphics.DrawRectangle(RadarWidth / 2 + RadarGap, RadarHeight + (BGHeight / 2) + BackgroundPosModifier, RadarWidth + BackgroundWidthModifier, BGHeight, Color.FromArgb(255, BackgroundR, BackgroundG, BackgroundB)); //Background
                e.Graphics.DrawRectangle((RadarWidth / 4) + RadarGap + HPBackgroundPosModifier, RadarHeight + (BGHeight / 2), RadarWidth / 2 + HPBackgroundWidthModifier, BarHeight, Color.FromArgb(255, RhpBG, GhpBG, BhpBG)); //HP Background
                e.Graphics.DrawRectangle(RadarGap + HPLoss / 2 + HPPosModifier, RadarHeight + (BGHeight / 2) + 1, HPLoss + HPWidthModifier, BarHeight - BarHeightModifier, Color.FromArgb(AlphaHP, LowHPR, LowHPG, LowHPB)); //HP Loss
                e.Graphics.DrawRectangle(RadarGap + HPWidth / 2 + HPPosModifier, RadarHeight + (BGHeight / 2) + 1, HPWidth + HPWidthModifier-2, BarHeight - BarHeightModifier-2, Color.FromArgb(AlphaHP, Rhp, Ghp, Bhp)); //HP bar
                e.Graphics.DrawRectangle(RadarGap + HPWidth / 2 + HPPosModifier, RadarHeight + (BGHeight / 2) + 1, HPWidth + HPWidthModifier, BarHeight - BarHeightModifier, Color.FromArgb(AlphaHP-40, Rhp, Ghp, Bhp)); //HP bar

                if (!SpecialAbilityEnabled)
                {
                    e.Graphics.DrawRectangle(RadarWidth + RadarGap + APPosBackgroundModifier, RadarHeight + (BGHeight / 2), RadarWidth / 2 - APWidthBackgroundModifier, BarHeight, Color.FromArgb(255, BackgroundAPR, BackgroundAPG, BackgroundAPB)); //Armor BG
                    e.Graphics.DrawRectangle((RadarWidth * 2 / 8) + RadarGap + APWidth / 2 + APPosModifier, RadarHeight + (BGHeight / 2) + 1, APWidth + APWidthModifier - 2, BarHeight - BarHeightModifier - 2, Color.FromArgb(255 - 20, APR, APG, APB)); //Armor
                    e.Graphics.DrawRectangle((RadarWidth * 2 / 8) + RadarGap + APWidth / 2 + APPosModifier, RadarHeight + (BGHeight / 2) + 1, APWidth + APWidthModifier, BarHeight - BarHeightModifier, Color.FromArgb(255 - 40, APR, APG, APB)); //Armor
                }
                if (SpecialAbilityEnabled)
                {
                    e.Graphics.DrawRectangle(RadarWidth * 5 / 8 + RadarGap + APPosBackgroundModifier, RadarHeight + (BGHeight / 2), RadarWidth / 4 + APWidthBackgroundModifier, BarHeight, Color.FromArgb(255, BackgroundAPR, BackgroundAPG, BackgroundAPB)); //Armor BG
                    e.Graphics.DrawRectangle(RadarWidth * 7 / 8 + RadarGap + SpecialPosBackgroundModifier, RadarHeight + (BGHeight / 2), RadarWidth / 4 + SpecialWidthBackgroundModifier, BarHeight, Color.FromArgb(255, RSpecialBG, GSpecialBG, BSpecialBG)); //Special BG
                    if (APWidth > 0) e.Graphics.DrawRectangle((RadarWidth * 4 / 8) + RadarGap + APWidth / 2 + APPosModifier, RadarHeight + (BGHeight / 2) + 1, APWidth + APWidthModifier-2, BarHeight - BarHeightModifier-2, Color.FromArgb(255, APR, APG, APB)); //Armor
                    if (APWidth > 0) e.Graphics.DrawRectangle((RadarWidth * 4 / 8) + RadarGap + APWidth / 2 + APPosModifier, RadarHeight + (BGHeight / 2) + 1, APWidth + APWidthModifier, BarHeight - BarHeightModifier, Color.FromArgb(255 - 40, APR, APG, APB)); //Armor
                    if (SpecialWidth > 0)
                    {
                        int DotAlphaTemp, DotAlphaTemp2, DotAlphaTemp3;
                        e.Graphics.DrawRectangle((RadarWidth * 6 / 8) + RadarGap + SpecialWidth / 2 + SpecialPosModifier, RadarHeight + (BGHeight / 2) + 1, SpecialWidth + SpecialWidthModifier, BarHeight - BarHeightModifier, Color.FromArgb(255, RSpecial, GSpecial, BSpecial));
                        if (EnableFlash)
                        {
                            if (DotAlpha < 20) { DotAlphaTemp3 = 0; } else { DotAlphaTemp3 = DotAlpha - 20; }
                            e.Graphics.DrawRectangle((RadarWidth * 6 / 8) + RadarGap + SpecialWidth / 2 + SpecialPosModifier, RadarHeight + (BGHeight / 2) + 1, SpecialWidth + SpecialWidthModifier + 4, BarHeight - BarHeightModifier + 4, Color.FromArgb(DotAlphaTemp3, RSpecial, GSpecial, BSpecial));
                            if (DotAlpha < 40) { DotAlphaTemp2 = 0; } else { DotAlphaTemp2 = DotAlpha - 40; }
                            e.Graphics.DrawRectangle((RadarWidth * 6 / 8) + RadarGap + SpecialWidth / 2 + SpecialPosModifier, RadarHeight + (BGHeight / 2) + 1, SpecialWidth + SpecialWidthModifier + 6, BarHeight - BarHeightModifier + 6, Color.FromArgb(DotAlphaTemp2, RSpecial, GSpecial, BSpecial));
                            if (DotAlpha < 60) { DotAlphaTemp = 0; } else { DotAlphaTemp = DotAlpha - 60; }
                            e.Graphics.DrawRectangle((RadarWidth * 6 / 8) + RadarGap + SpecialWidth / 2 + SpecialPosModifier, RadarHeight + (BGHeight / 2) + 1, SpecialWidth + SpecialWidthModifier + 8, BarHeight - BarHeightModifier + 8, Color.FromArgb(DotAlphaTemp, RSpecial, GSpecial, BSpecial));
                        }  
                    }//Special 
                }
            }
            if (debug)
            {
                e.Graphics.DrawText("Showing: " + Showing.ToString() + " DotAlpha: " + DotAlpha.ToString() + " Down: " + Down2.ToString(), 0, 0);
                e.Graphics.DrawText("HP: " + Player.Character.Health + " Armor: " + Player.Character.Armor + " Special: " + SpecialValue, 0, 30);
            }
            if (DeadPictureEnabled) e.Graphics.DrawSprite(Wasted, IMGHeight / 2, ScreenResHeight / 2, ScreenResWidth, ScreenResHeight, 0, Color.FromArgb(150, 255, 255, 255)); //Wasted
            if(Busted) e.Graphics.DrawSprite(Wasted, ScreenResWidth / 2, ScreenResHeight / 2, ScreenResWidth, ScreenResHeight, 0, Color.FromArgb(150, 255, 255, 255));
        }
    }
}

