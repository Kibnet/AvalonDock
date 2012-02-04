﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using AvalonDock.Layout;

namespace AvalonDock.Controls
{

    [TemplatePart(Name = "PART_MenuPin", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_AutoHidePin", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ClosePin", Type = typeof(FrameworkElement))]
    public class AnchorablePaneTitle : Control
    {
        static AnchorablePaneTitle()
        {
            IsHitTestVisibleProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(true));
            FocusableProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(false));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnchorablePaneTitle), new FrameworkPropertyMetadata(typeof(AnchorablePaneTitle)));
        }


        internal AnchorablePaneTitle()
        { }


        Border _menuPinContainer = null;
        Border _menuAutoHideContainer = null;
        Border _menuCloseContainer = null;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _menuPinContainer = this.GetTemplateChild("PART_MenuPin") as Border;
            _menuAutoHideContainer = this.GetTemplateChild("PART_AutoHidePin") as Border;
            _menuCloseContainer = this.GetTemplateChild("PART_ClosePin") as Border;

            if (_menuAutoHideContainer != null)
                _menuAutoHideContainer.MouseLeftButtonUp += (s, e) => OnToggleAutoHide();
        }

        #region Model

        /// <summary>
        /// Model Dependency Property
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(LayoutAnchorable), typeof(AnchorablePaneTitle),
                new FrameworkPropertyMetadata((LayoutAnchorable)null));

        /// <summary>
        /// Gets or sets the Model property.  This dependency property 
        /// indicates model attached to this view.
        /// </summary>
        public LayoutAnchorable Model
        {
            get { return (LayoutAnchorable)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        #endregion

        private void OnToggleAutoHide()
        {
            var manager = Model.Root.Manager;

            manager.ToggleAutoHide(Model);
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _isMouseDown = false;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (_isMouseDown && e.LeftButton == MouseButtonState.Pressed)
            {
                var pane = this.FindVisualAncestor<LayoutAnchorablePaneControl>();
                if (pane != null)
                {
                    var paneModel = pane.Model as LayoutAnchorablePane;
                    var manager = paneModel.Root.Manager;

                    manager.StartDraggingFloatingWindowForPane(paneModel);
                }
            }

            _isMouseDown = false;
        }

        bool _isMouseDown = false;
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            base.OnMouseLeftButtonUp(e);

            if (Model != null)
                FocusElementManager.SetFocusOnLastElement(Model);
        }
    }
}
