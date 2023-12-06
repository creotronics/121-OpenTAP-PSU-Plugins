using OpenTap;
using System;
using System.Collections.Generic;
using Creotronics.OpenTAP.Instruments.PSU.API;

namespace Creotronics.OpenTAP.Instruments.PSU.TestSteps
{
    /// <summary>
    /// This test step can be used to get the power supply current.
    /// <br>Properties:</br>
    /// <br>    - Power supply instrument to be used</br>
    /// <br>    - Power supply channel</br>
    /// <br>    - Check the value?</br>
    /// <br>    - Expected current level</br>
    /// <br>    - Accepted deviation (in %)</br>
    /// </summary>
    [Display("Get Current of Channel {Channel}", Group: "PSU", Description: "Get the power supply output current.")]
    public class GetCurrent : TestStep
    {
        #region Settings
        /// <summary>
        /// The power supply to be used for this test step.
        /// </summary>
        [Display(Group: "PSU Settings", Name: "PSU", Order: 1.1)]
        public IPSU MyPSU { get; set; }

        private UInt16 _myPsuChannel;
        /// <summary>
        /// List of power supply channels that can be used.
        /// </summary>
        public List<UInt16> AvailableChannels
        {
            // Get the channel cound supported by the power supply and make a selection list. 
            get
            {
                List<UInt16> channels = new List<UInt16>();

                for (UInt16 i = 0; i < MyPSU.Channels; i++)
                {
                    channels.Add((ushort)(i + 1));
                }

                return channels;
            }
        }

        /// <summary>
        /// The power supply channel to be used for the test step.
        /// </summary>
        [Display(Name: "Channel", Group: "PSU Settings", Description: "Select the power supply channel to be used.", Order: 1.2)]
        [AvailableValues(nameof(AvailableChannels))]
        public UInt16 Channel
        {
            get => _myPsuChannel;
            set => _myPsuChannel = value;
        }

        private bool _limitCheck;
        /// <summary>
        /// Enable if the read out current must be verified.
        /// </summary>
        [Display(Group: "Level Checking", Name: "Level Check", Order: 2.1)]
        public bool LimitCheckEnabled
        {
            get => _limitCheck; 
            set => _limitCheck = value; 
        }

        private double _currentLevel;
        /// <summary>
        /// The current level expected.
        /// </summary>
        [Display(Group: "Level Checking", Name: "Level", Order: 2.2,
            Description: "The expected current level to be read back.")]
        [EnabledIf("LimitCheckEnabled", true, HideIfDisabled = true)]
        [Unit("A", UseEngineeringPrefix: false)]
        public double CurrentLevel 
        { 
            get => _currentLevel;
            set => _currentLevel = value;
        }

        private UInt16 _currentDeviation;
        /// <summary>
        /// The current level deviation accepted.
        /// </summary>
        [Display(Group: "Level Checking", Name: "Deviation", Order: 2.3,
            Description: "The accepted deviation (in %).")]
        [EnabledIf("LimitCheckEnabled", true, HideIfDisabled = true)]
        [Unit("%", UseEngineeringPrefix: false)]
        public UInt16 CurrentDeviation 
        { 
            get => _currentDeviation; 
            set => _currentDeviation = value; 
        }

        #endregion

        public GetCurrent()
        {
            // Default power supply channel.
            Channel = 1;

            // Check if deviation is between 0 and 100 %
            Rules.Add(() => CurrentDeviation >= 0 && CurrentDeviation <= 100, "The read out current level deviation should be between 0 and 100%.", "CurrentDeviation");
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        /// <summary>
        /// The actual test step. The power supply current will be read via Scpi command.
        /// If value is within expected limits, test passes. If not, test fails.
        /// </summary>
        public override void Run()
        {
            // Get the current.
            double readCurrent = MyPSU.MeasureCurrent(_myPsuChannel);

            if (_limitCheck)
            {
                // Read out voltage level needs to be verified.

                // Calculate limits
                double minLevel = (_currentLevel * (100 - _currentDeviation) / 100);
                double maxLevel = (_currentLevel * (100 + _currentDeviation) / 100);

                if (readCurrent < maxLevel && readCurrent > minLevel)
                {
                    // Value is within limits
                    Log.Info("Power supply current of channel " + _myPsuChannel + " is " + readCurrent + "A. Current is within expected limits of " + _currentLevel + "A +/- " + _currentDeviation + "% (" + minLevel + "A - " + maxLevel + "A).");
                    UpgradeVerdict(Verdict.Pass);
                }
                else
                {
                    // Value is not within limits. Test fails.
                    Log.Error("Power supply current of channel " + _myPsuChannel + " is " + readCurrent + "A. This is not within expected limits of " + _currentLevel + "A +/- " + _currentDeviation + "% (" + minLevel + "A - " + maxLevel + "A).");
                    
                    // If the output is disabled it's logical that the test will fail (there will be no current flowing).
                    // Warn the user about this.
                    if (!MyPSU.GetOutputState(_myPsuChannel))
                    {
                        Log.Warning("The output of channel " + _myPsuChannel + " is not enabled! Verify if this is as expected.");
                    }

                    UpgradeVerdict(Verdict.Fail);
                }
            }
            else
            {
                // No need to verify the value. Test will pass.
                Log.Info("Power supply current of channel " + _myPsuChannel + " is " + readCurrent + "A.");
                UpgradeVerdict(Verdict.Pass);
            }

            RunChildSteps(); //If step has child steps.
        }

        public override void PostPlanRun()
        {
            // ToDo: Optionally add any cleanup code this step needs to run after the entire testplan has finished
            base.PostPlanRun();
        }
    }
}
