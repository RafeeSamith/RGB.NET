﻿using System.Collections.Generic;
using System.Linq;
using RGB.NET.Core;

namespace RGB.NET.Devices.Asus
{
    /// <inheritdoc cref="AbstractRGBDevice{TDeviceInfo}" />
    /// <inheritdoc cref="IAsusRGBDevice" />
    /// <summary>
    /// Represents a generic Asus-device. (keyboard, mouse, headset, mousepad).
    /// </summary>
    public abstract class AsusRGBDevice<TDeviceInfo> : AbstractRGBDevice<TDeviceInfo>, IAsusRGBDevice
        where TDeviceInfo : AsusRGBDeviceInfo
    {
        #region Properties & Fields

        /// <inheritdoc />
        /// <summary>
        /// Gets information about the <see cref="T:RGB.NET.Devices.Asus.AsusRGBDevice" />.
        /// </summary>
        public override TDeviceInfo DeviceInfo { get; }

        /// <summary>
        /// Gets or sets the update queue performing updates for this device.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        protected AsusUpdateQueue UpdateQueue { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AsusRGBDevice{TDeviceInfo}"/> class.
        /// </summary>
        /// <param name="info">The generic information provided by Asus for the device.</param>
        protected AsusRGBDevice(TDeviceInfo info)
        {
            this.DeviceInfo = info;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the device.
        /// </summary>
        public void Initialize(IDeviceUpdateTrigger updateTrigger)
        {
            InitializeLayout();

            if (Size == Size.Invalid)
            {
                Rectangle ledRectangle = new Rectangle(this.Select(x => x.LedRectangle));
                Size = ledRectangle.Size + new Size(ledRectangle.Location.X, ledRectangle.Location.Y);
            }

            UpdateQueue = new AsusUpdateQueue(updateTrigger);
            UpdateQueue.Initialize(DeviceInfo.Device);
        }

        /// <summary>
        /// Initializes the <see cref="Led"/> and <see cref="Size"/> of the device.
        /// </summary>
        protected abstract void InitializeLayout();

        /// <inheritdoc />
        protected override void UpdateLeds(IEnumerable<Led> ledsToUpdate) => UpdateQueue.SetData(ledsToUpdate.Where(x => x.Color.A > 0));

        /// <inheritdoc />
        public override void SyncBack()
        {
            // DarthAffe 16.06.2019: This doesn't work since the SDK only returns the colors we set.
            //foreach (Led led in LedMapping.Values)
            //{
            //    int index = (int)led.CustomData;
            //    IAuraRgbLight light = DeviceInfo.Device.Lights[index];
            //    SetLedColorWithoutRequest(led, new Color(light.Red, light.Green, light.Blue));
            //}
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            try { UpdateQueue?.Dispose(); }
            catch { /* at least we tried */ }

            base.Dispose();
        }

        #endregion
    }
}
