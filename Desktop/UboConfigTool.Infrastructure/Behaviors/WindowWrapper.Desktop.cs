using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Windows;
using UboConfigTool.Infrastructure.Events;

namespace UboConfigTool.Infrastructure.Behaviors
{
    /// <summary>
    /// Defines a wrapper for the <see cref="Window"/> class that implements the <see cref="IWindow"/> interface.
    /// </summary>
    public class WindowWrapper : IWindow
    {
        private readonly Window window;

        /// <summary>
        /// Initializes a new instance of <see cref="WindowWrapper"/>.
        /// </summary>
        public WindowWrapper()
        {
            //this.window = new Window();
             this.window = new MetroWindow();
        }

        /// <summary>
        /// Ocurrs when the <see cref="Window"/> is closed.
        /// </summary>
        public event EventHandler Closed
        {
            add { this.window.Closed += value; }
            remove { this.window.Closed -= value; }
        }

        /// <summary>
        /// Gets or Sets the content for the <see cref="Window"/>.
        /// </summary>
        public object Content
        {
            get { return this.window.Content; }
            set { this.window.Content = value; }
        }

        /// <summary>
        /// Gets or Sets the <see cref="Window.Owner"/> control of the <see cref="Window"/>.
        /// </summary>
        public object Owner
        {
            get { return this.window.Owner; }
            set { this.window.Owner = value as Window; }
        }

        /// <summary>
        /// Gets or Sets the <see cref="FrameworkElement.Style"/> to apply to the <see cref="Window"/>.
        /// </summary>
        public Style Style
        {
            get { return this.window.Style; }
            set { this.window.Style = value; }
        }

        /// <summary>
        /// Opens the <see cref="Window"/>.
        /// </summary>
        public void Show()
        {
            IEventAggregator eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            if( eventAggregator != null)
            {
                eventAggregator.GetEvent<PopupDialogBeginEvent>().Publish( true );
            }

            this.window.ResizeMode = ResizeMode.NoResize;
            this.window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.window.ShowDialog();
        }

        /// <summary>
        /// Closes the <see cref="Window"/>.
        /// </summary>
        public void Close()
        {
            IEventAggregator eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            if( eventAggregator != null )
            {
                eventAggregator.GetEvent<PopupDialogEndEvent>().Publish( true );
            }

            this.window.Close();
        }
    }
}