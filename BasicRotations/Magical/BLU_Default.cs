namespace BasicRotations.Magical;

[SourceCode(Path = "main/BasicRotations/Magical/BLU_Default.cs")]
public sealed class BLU_Default : BLU_Base
{
    #region General rotation info
    public override string GameVersion => VERSION;
    public override string RotationName => $"{USERNAME}'s {ClassJob.Abbreviation} [{Type}]";
    public override CombatType Type => CombatType.PvE;
    #endregion General rotation info

    #region Rotation Configs
    protected override IRotationConfigSet CreateConfiguration()
    {
        return base.CreateConfiguration()
            .SetBool(CombatType.PvE, "MoonFluteBreak", false, "Use Moon Flute")
            .SetBool(CombatType.PvE, "SingleAOE", true, "Use high-damage AoE skills on single target")
            .SetBool(CombatType.PvE, "GamblerKill", false, "Use skills with a chance to fail")
            .SetBool(CombatType.PvE, "UseFinalSting", false, "Use Final Sting")
            .SetFloat(EasyCombat.Basic.Configuration.ConfigUnitType.Percent, CombatType.PvE, "FinalStingHP", 0, "Target HPP for Final Sting");
    }
    #endregion

    #region Countdown logic

    #endregion

    #region GCD Logic

    #endregion

    #region oGCD Logic

    #endregion

    #region Extra Methods

    #endregion

    public override bool CanHealAreaSpell => base.CanHealAreaSpell && BlueId == BLUID.Healer;
    public override bool CanHealSingleSpell => base.CanHealSingleSpell && BlueId == BLUID.Healer;



    private bool MoonFluteBreak => Configs.GetBool("MoonFluteBreak");
    private bool UseFinalSting => Configs.GetBool("UseFinalSting");
    private float FinalStingHP => Configs.GetFloat("FinalStingHP");

    private static bool QuickLevel => false;

    private bool GamblerKill => Configs.GetBool("GamblerKill");

    private bool SingleAOE => Configs.GetBool("SingleAOE");

    protected override bool EmergencyAbility(IAction nextGCD, out IAction act)
    {
        if (nextGCD.IsTheSameTo(false, SelfDestruct, FinalSting))
        {
            if (Swiftcast.CanUse(out act)) return true;
        }
        return base.EmergencyAbility(nextGCD, out act);
    }

    protected override bool GeneralGCD(out IAction act)
    {
        act = null;

        if (Player.HasStatus(true, StatusID.WaningNocturne)) return false;

        if (PhantomFlurry.IsCoolingDown && !PhantomFlurry.ElapsedOneChargeAfter(1) || Player.HasStatus(true, StatusID.PhantomFlurry))
        {
            if (!Player.WillStatusEnd(0.1f, true, StatusID.PhantomFlurry) && Player.WillStatusEnd(1, true, StatusID.PhantomFlurry) && PhantomFlurry2.CanUse(out act, CanUseOption.MustUse)) return true;
            return false;
        }

        if (Player.HasStatus(true, StatusID.SurpanakhasFury))
        {
            if (Surpanakha.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;
        }


        if (UseFinalSting && CanUseFinalSting(out act)) return true;


        if (MoonFluteBreak && DBlueBreak(out act)) return true;

        if (PrimalSpell(out act)) return true;

        if (AreaGCD(out act)) return true;

        if (SingleGCD(out act)) return true;

        return base.GeneralGCD(out act);
    }

    protected override bool HealSingleGCD(out IAction act)
    {
        if (BlueId == BLUID.Healer)
        {
            if (IsEsunaStanceNorth && WeakenPeople.Any() || DyingPeople.Any())
            {
                if (Exuviation.CanUse(out act, CanUseOption.MustUse)) return true;
            }
            if (AngelsSnack.CanUse(out act)) return true;
            if (Stotram.CanUse(out act)) return true;
            if (PomCure.CanUse(out act)) return true;
        }
        else
        {
            if (WhiteWind.CanUse(out act, CanUseOption.MustUse)) return true;
        }

        return base.HealSingleGCD(out act);
    }

    private bool DBlueBreak(out IAction act)
    {
        if (TripleTrident.OnSlot && TripleTrident.WillHaveOneChargeGCD(OnSlotCount(Whistle, Tingle), 0))
        {
            if (Whistle.CanUse(out act)) return true;
            if (!Player.HasStatus(true, StatusID.Tingling)
                && Tingle.CanUse(out act, CanUseOption.MustUse)) return true;
            if (OffGuard.CanUse(out act)) return true;
            if (TripleTrident.CanUse(out act, CanUseOption.MustUse)) return true;
        }

        if (AllOnSlot(Whistle, FinalSting, BasicInstinct) && UseFinalSting)
        {
            if (Whistle.CanUse(out act)) return true;
            if (OffGuard.CanUse(out act)) return true;
            if (Tingle.CanUse(out act)) return true;
        }

        if (CanUseMoonFlute(out act)) return true;

        if (!Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

        if (NightBloom.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Eruption.CanUse(out act, CanUseOption.MustUse)) return true;
        if (MatraMagic.CanUse(out act, CanUseOption.MustUse)) return true;
        if (JKick.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Devour.CanUse(out act, CanUseOption.MustUse)) return true;
        if (ShockStrike.CanUse(out act, CanUseOption.MustUse)) return true;
        if (GlassDance.CanUse(out act, CanUseOption.MustUse)) return true;
        if (MagicHammer.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Surpanakha.CurrentCharges >= 3 && Surpanakha.CanUse(out act, CanUseOption.MustUse | CanUseOption.EmptyOrSkipCombo)) return true;
        if (PhantomFlurry.CanUse(out act, CanUseOption.MustUse)) return true;

        if (WhiteDeath.CanUse(out act)) return true;
        if (IsBurst && !MoonFluteBreak && BothEnds.CanUse(out act, CanUseOption.MustUse)) return true;
        if (Quasar.CanUse(out act, CanUseOption.MustUse)) return true;
        if (FeatherRain.CanUse(out act, CanUseOption.MustUse)) return true;
        if (MountainBuster.CanUse(out act, CanUseOption.MustUse)) return true;
        if (GlassDance.CanUse(out act, CanUseOption.MustUse)) return true;

        if (SonicBoom.CanUse(out act)) return true;

        return false;
    }

    private static bool CanUseMoonFlute(out IAction act)
    {
        if (!MoonFlute.CanUse(out act) && !HasHostilesInRange) return false;

        if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

        if (Player.HasStatus(true, StatusID.Harmonized)) return true;

        return false;
    }

    private bool CanUseFinalSting(out IAction act)
    {
        act = null;
        if (!UseFinalSting) return false;
        if (!FinalSting.CanUse(out _)) return false;

        var useFinalSting = Player.HasStatus(true, StatusID.WaxingNocturne, StatusID.Harmonized);

        if (AllOnSlot(Whistle, MoonFlute, FinalSting) && !AllOnSlot(BasicInstinct))
        {
            if (HostileTarget?.GetHealthRatio() > FinalStingHP) return false;

            if (Whistle.CanUse(out act)) return true;
            if (MoonFlute.CanUse(out act)) return true;
            if (useFinalSting && FinalSting.CanUse(out act)) return true;
        }

        if (AllOnSlot(Whistle, MoonFlute, FinalSting, BasicInstinct))
        {
            if (Player.HasStatus(true, StatusID.WaxingNocturne) && OffGuard.CanUse(out act)) return true;

            if (HostileTarget?.GetHealthRatio() > FinalStingHP) return false;
            if (Whistle.CanUse(out act)) return true;
            if (MoonFlute.CanUse(out act)) return true;
            if (useFinalSting && FinalSting.CanUse(out act)) return true;
        }

        return false;
    }

    private bool SingleGCD(out IAction act)
    {
        act = null;
        if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

        if (QuickLevel && StickyTongue.CanUse(out act)) return true;

        if (AllOnSlot(Bristle, SongOfTorment) && SongOfTorment.CanUse(out _))
        {
            if (Bristle.CanUse(out act)) return true;
            if (SongOfTorment.CanUse(out act)) return true;
        }
        if (SongOfTorment.CanUse(out act)) return true;

        if (RevengeBlast.CanUse(out act)) return true;

        if (GamblerKill)
        {
            if (Missile.CanUse(out act)) return true;
            if (TailScrew.CanUse(out act)) return true;
            if (Doom.CanUse(out act)) return true;
        }

        if (SharpenedKnife.CanUse(out act)) return true;

        if (CurrentMp < 1000 && BloodDrain.CanUse(out act)) return true;
        if (SonicBoom.CanUse(out act)) return true;
        if (DrillCannons.CanUse(out act, CanUseOption.MustUse)) return true;
        if (PerpetualRay.CanUse(out act)) return true;
        if (AbyssalTransfixion.CanUse(out act)) return true;
        if (Reflux.CanUse(out act)) return true;
        if (WaterCannon.CanUse(out act)) return true;

        if (CondensedLibra.CanUse(out act)) return true;

        if (StickyTongue.CanUse(out act)) return true;

        if (FlyingSardine.CanUse(out act)) return true;

        return false;
    }

    private bool AreaGCD(out IAction act)
    {
        act = null;
        if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

        if (GamblerKill)
        {
            if (Launcher.CanUse(out act, CanUseOption.MustUse)) return true;
            if (Level5Death.CanUse(out act, CanUseOption.MustUse)) return true;
        }

        if (false)
        {
            if (AcornBomb.CanUse(out act, CanUseOption.MustUse))
            {
                return true;
            }

            if (Faze.CanUse(out act, CanUseOption.MustUse)) return true;
            if (Snort.CanUse(out act, CanUseOption.MustUse)) return true;
            if (BadBreath.CanUse(out act, CanUseOption.MustUse)) return true;
            if (Chirp.CanUse(out act, CanUseOption.MustUse)) return true;
            if (Level5Petrify.CanUse(out act, CanUseOption.MustUse)) return true;
        }

        if (HasCompanion && ChocoMeteor.CanUse(out act, CanUseOption.MustUse)) return true;

        if (HostileTargets.GetObjectInRadius(6).Count() < 3)
        {
            if (HydroPull.CanUse(out act)) return true;
        }

        if (TheRamVoice.CanUse(out act)) return true;

        if (!IsMoving && (HostileTarget?.HasStatus(false, StatusID.DeepFreeze) ?? false) && TheRamVoice.CanUse(out act)) return true;

        //雷电咆哮
        if (TheDragonVoice.CanUse(out act)) return true;

        if (Blaze.CanUse(out act)) return true;
        if (FeculentFlood.CanUse(out act)) return true;
        if (FlameThrower.CanUse(out act)) return true;
        if (AquaBreath.CanUse(out act)) return true;
        if (HighVoltage.CanUse(out act)) return true;
        if (Glower.CanUse(out act)) return true;
        if (PlainCracker.CanUse(out act)) return true;
        if (TheLook.CanUse(out act)) return true;
        if (InkJet.CanUse(out act)) return true;
        if (FireAngon.CanUse(out act)) return true;
        if (MindBlast.CanUse(out act)) return true;
        if (AlpineDraft.CanUse(out act)) return true;
        if (ProteanWave.CanUse(out act)) return true;
        if (Northerlies.CanUse(out act)) return true;
        if (Electrogenesis.CanUse(out act)) return true;
        if (WhiteKnightsTour.CanUse(out act)) return true;
        if (BlackKnightsTour.CanUse(out act)) return true;
        if (Tatamigaeshi.CanUse(out act)) return true;

        if (MustardBomb.CanUse(out act)) return true;
        if (AetherialSpark.CanUse(out act)) return true;
        if (MaledictionOfWater.CanUse(out act)) return true;
        if (FlyingFrenzy.CanUse(out act)) return true;
        if (DrillCannons.CanUse(out act)) return true;
        if (Weight4Tonze.CanUse(out act)) return true;
        if (Needles1000.CanUse(out act)) return true;
        if (Kaltstrahl.CanUse(out act)) return true;
        if (PeripheralSynthesis.CanUse(out act)) return true;
        if (FlameThrower.CanUse(out act)) return true;
        if (FlameThrower.CanUse(out act)) return true;
        if (SaintlyBeam.CanUse(out act)) return true;

        return false;
    }

    private bool PrimalSpell(out IAction act)
    {
        act = null;
        if (Player.HasStatus(true, StatusID.WaxingNocturne)) return false;

        if (WhiteDeath.CanUse(out act)) return true;
        if (DivineCataract.CanUse(out act)) return true;

        if (TheRoseOfDestruction.CanUse(out act)) return true;

        if (IsBurst && !MoonFluteBreak && TripleTrident.CanUse(out act)) return true;
        if (IsBurst && !MoonFluteBreak && MatraMagic.CanUse(out act)) return true;

        if (Devour.CanUse(out act)) return true;
        //if (MagicHammer.ShouldUse(out act)) return true;

        var option = SingleAOE ? CanUseOption.MustUse : CanUseOption.None;
        if (IsBurst && !MoonFluteBreak && NightBloom.CanUse(out act, option)) return true;
        if (IsBurst && !MoonFluteBreak && BothEnds.CanUse(out act, option)) return true;

        if (IsBurst && !MoonFluteBreak && Surpanakha.CurrentCharges >= 3 && Surpanakha.CanUse(out act, option | CanUseOption.EmptyOrSkipCombo)) return true;

        if (Quasar.CanUse(out act, option)) return true;
        if (!IsMoving && JKick.CanUse(out act, option)) return true;

        if (Eruption.CanUse(out act, option)) return true;
        if (FeatherRain.CanUse(out act, option)) return true;

        if (ShockStrike.CanUse(out act, option)) return true;
        if (MountainBuster.CanUse(out act, option)) return true;

        if (GlassDance.CanUse(out act, option)) return true;

        //if (MountainBuster.ShouldUse(out act, option)) return true;


        return false;
    }
}
