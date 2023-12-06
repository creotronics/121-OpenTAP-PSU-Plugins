using OpenTap;
using System;
using System.Collections.Generic;
using Creotronics.OpenTAP.Instruments.PSU.API;

namespace Creotronics.OpenTAP.Instruments.PSU.TestSteps
{
    /// <summary>
    /// This test step can be used to set the power supply over current protection.
    /// <br>Properties:</br>
    /// <br>    - Power supply instrument to be used</br>
    /// <br>    - Power supply channel</br>
    /// <br>    - Over current protection level</br>
    /// </summary>
    [Display("Set Over Current Protection of Channel {Channel} to {Over Current}", Group:"PSU",Description:"Set the power supply output over current protection.")]
    public class SetOverCurrent : TestStep
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
        [Display(Name:"Channel", Group:"PSU Settings", Description:"Select the power supply channel to be used.", Order:1.2)]
        [AvailableValues(nameof(AvailableChannels))]
        public UInt16 Channel 
        { 
            get => _myPsuChannel;
            set => _myPsuChannel = value; 
        }

        private double _overCurrent;
        /// <summary>
        /// The over current to be set.
        /// </summary>
        [Display(Group: "PSU Settings", Name: "Over Current", Order: 1.3)]
        [Unit("A", UseEngineeringPrefix: false)]
        public double OverCurrent 
        { 
            get => _overCurrent;
            set { if (_overCurrent != value) _overCurrent = value; }
        }

        #endregion

        public SetOverCurrent()
        {
            // Default power supply channel.
            Channel = 1;

            // Verify if the over voltage is not set outside the operating range of the used power supply.
            Rules.Add(() => OverCurrent <= MyPSU.MaxCurrent[_myPsuChannel - 1], () => "An over current higher than " + MyPSU.MaxCurrent[_myPsuChannel - 1] + "A for channel " + _myPsuChannel + " is not supported by " + MyPSU.Name +
            ". Please set an over current between " + MyPSU.MinCurrent[_myPsuChannel - 1] + "A and " + MyPSU.MaxCurrent[_myPsuChannel - 1] + "A.", "OverCurrent");
            Rules.Add(() => OverCurrent >= MyPSU.MinCurrent[_myPsuChannel - 1], () => "An over current lower than " + MyPSU.MinCurrent[_myPsuChannel - 1] + "A for channel " + _myPsuChannel + " is not supported by " + MyPSU.Name +
            ". Please set an over current between " + MyPSU.MinCurrent[_myPsuChannel - 1] + "A and " + MyPSU.MaxCurrent[_myPsuChannel - 1] + "A.", "OverCurrent");
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        /// <summary>
        /// The actual test step. The power supply over current protection will be set via Scpi command.
        /// The value will be read back to verify. If successful, test step passed. If not, test fails.
        /// </summary>
        public override void Run()
        {
            // Set the over current protection.
            MyPSU.SetOverCurrentProtection(_overCurrent, _myPsuChannel);

            // Read the set over current back and verify if set correctly.
            if (MyPSU.GetOverCurrentProtection(_myPsuChannel) == _overCurrent)
            {
                Log.Info("Power supply over current protection of channel " + _myPsuChannel + " set to " + _overCurrent + "A.");
                UpgradeVerdict(Verdict.Pass);
            }
            else
            {
                Log.Error("Failed to set power supply over current protection of channel " + _myPsuChannel + " to " + _overCurrent + "A!");
                UpgradeVerdict(Verdict.Fail);
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
