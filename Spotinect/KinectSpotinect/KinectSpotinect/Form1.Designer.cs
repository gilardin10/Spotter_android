using Microsoft.Kinect;
using Kinect.Toolbox;
using Coding4Fun.Kinect.WinForm;
using System.Windows.Forms;
using System;

namespace KinectSpotinect
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Form1";
        }

        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                nui.Initialize(RuntimeOptions.UseSkeletalTracking);
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Runtime initialization failed. " +
                   "Please make sure Kinect device is plugged in.");
                return;
            }

            #region add events

            nui.SkeletonFrameReady +=
              new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            SwG.OnGestureDetected += On_GestureDetected;

            #endregion
        }
    }
}

