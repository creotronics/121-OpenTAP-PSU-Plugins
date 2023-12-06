using OpenTap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Creotronics.OpenTAP.Instruments.PSU.API;

namespace Creotronics.OpenTAP.Instruments.PSU.AimTTi
{

    [Display("PL601-P", Groups: new[] { "PSU", "Aim TTi" }, Description: "Linear regulated DC power supply, single output, 60V, 1.5A.")]
    public class PL601_P : TTi_Base
    {
        // TODO: How to disable the channel selection if there is only one channel? Or how to set the channels to 2 channels if there are only 2?

        #region Settings
        /// <summary>
        /// By default PSU power is switched off at the end of the test sequence. If needed, enable LeavePowerOn to keep power on at end of test sequence.
        /// </summary>
        [Display(Name: "Leave power ON", Group: "Close Parameters", Order: 1.0, Description: "Leave power ON.")]
        public bool LeavePowerOn { get; set; }

        /// <summary>
        /// Channel count of the power supply will be displayed as information. It's however not possible to change this.
        /// </summary>
        [Display(Name: "Channels", Group: "Power Supply Specifications", Order: 2.0, Description: "The PSU channel count.", Collapsed: true)]
        [Browsable(true)]
        public UInt16 ChannelCount { get => base.Channels; }

        /// <summary>
        /// Minimum voltage of the power supply will be displayed as information. It's however not possible to change this.
        /// </summary>
        [Display(Name: "Minimum Voltage", Group: "Power Supply Specifications", Order: 2.1, Description: "The PSU minimum voltage that can be set.", Collapsed: true)]
        [Browsable(true)]
        public List<double> PsuMinVoltage { get => base.MinVoltage; }

        /// <summary>
        /// Maximum voltage of the power supply will be displayed as information. It's however not possible to change this.
        /// </summary>
        [Display(Name: "Maximum Voltage", Group: "Power Supply Specifications", Order: 2.2, Description: "The PSU maximum voltage that can be set (for each channel).", Collapsed: true)]
        [Browsable(true)]
        public List<double> PsuMaxVoltageList { get => base.MaxVoltage; }

        /// <summary>
        /// Minimum current of the power supply will be displayed as information. It's however not possible to change this.
        /// </summary>
        [Display(Name: "Minimum Current", Group: "Power Supply Specifications", Order: 2.3, Description: "The PSU minimum current that can be set.", Collapsed: true)]
        [Browsable(true)]
        public List<double> PsuMinCurrent { get => base.MinCurrent; }

        /// <summary>
        /// Maximum current of the power supply will be displayed as information. It's however not possible to change this.
        /// </summary>
        [Display(Name: "Maximum Current", Group: "Power Supply Specifications", Order: 2.4, Description: "The PSU maximum current that can be set.", Collapsed: true)]
        [Browsable(true)]
        public List<double> PsuMaxCurrent { get => base.MaxCurrent; }

        // ToDo: Add property here for each parameter the end user should be able to change

        #endregion

        public PL601_P()
        {
            Name = "Aim TTi, PL601-P";

            // Set the identification settings for Scpi (*IDN?).
            PsuBrand = "Thurbly Thandar";
            PsuModel = "PL601-P";

            // By default power supply will be switched off at end of a test sequence.
            LeavePowerOn = false;

            // Power supply information.

            // Define channel count of the PSU model.
            Channels = 1;
            // Define max and min voltage and current for each channel that can be set with this PSU model.
            MaxVoltage = new List<double> { 60 };
            MinVoltage = new List<double> { 0 };
            MaxCurrent = new List<double> { 1.5 };
            MinCurrent = new List<double> { 0 };
        }

        public override void Open()
        {
            base.Open();
        }

        public override void Close()
        {
            // At close, verify if the user want's to keep the power supply output on or off.
            if (LeavePowerOn)
                base.Close(true);
            else
                base.Close();
        }
    }
}
