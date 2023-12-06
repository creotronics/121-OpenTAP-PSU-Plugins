using OpenTap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Creotronics.OpenTAP.Instruments.PSU.API;

namespace Creotronics.OpenTAP.Instruments.PSU.TestSteps
{
    /// <summary>
    /// This test step can be used to enable or disable the power supply output channel.
    /// <br>Properties:</br>
    /// <br>    - Power supply instrument to be used</br>
    /// <br>    - Power supply channel</br>
    /// <br>    - Enable / disable</br>
    /// </summary>
    [Display("Set Output of Channel {Channel}", Group:"PSU",Description:"Enable or disable the power supply output.")]
    public class SetOutput : TestStep
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

                // TODO: if there are no power supply instruments, next line will fail.
                // Check if there are power supply instruments present.

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

        private bool _outputEnable;
        /// <summary>
        /// The channel output state to be set (enable or disable).
        /// </summary>
        [Display(Group: "PSU Settings", Name: "Output enable", Order: 1.3)]
        public bool OutputEnable 
        { 
            get => _outputEnable;
            set { if (_outputEnable != value) _outputEnable = value; }
        }

        #endregion

        public SetOutput()
        {
            // Default power supply channel.
            Channel = 1;
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        /// <summary>
        /// The actual test step. The power supply voltage will be set via Scpi command.
        /// The value will be read back to verify. If successful, test step passed. If not, test fails.
        /// </summary>
        public override void Run()
        {
            // Set output.
            MyPSU.SetOutputState(_outputEnable, _myPsuChannel);

            // Verify if the output has been enabled.
            if (MyPSU.GetOutputState(_myPsuChannel) == _outputEnable)
            {
                Log.Info("Power supply output of channel " + _myPsuChannel + " is " + (_outputEnable?"enabled.":"disabled."));
                UpgradeVerdict(Verdict.Pass);
            }
            else
            {
                Log.Error("Failed to " + (_outputEnable?"enable":"disable") + " power supply output channel " + _myPsuChannel + "!");
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
