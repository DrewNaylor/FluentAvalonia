﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Styling;
using FluentAvalonia.Core;
using FluentAvalonia.UI.Media;
using System;

namespace FluentAvalonia.UI.Controls
{
	/// <summary>
	/// A button that hides a <see cref="ColorPicker"/> within a <see cref="Flyout"/>
	/// </summary>
    public partial class ColorPickerButton : TemplatedControl, IStyleable
    {
		static ColorPickerButton()
		{
			IsMoreButtonVisibleProperty.OverrideDefaultValue<ColorPickerButton>(true);
			UseSpectrumProperty.OverrideDefaultValue<ColorPickerButton>(true);
			UseColorWheelProperty.OverrideDefaultValue<ColorPickerButton>(true);
			UseColorTriangleProperty.OverrideDefaultValue<ColorPickerButton>(true);
		}
		
		Type IStyleable.StyleKey => typeof(ColorPickerButton);
				
		/// <summary>
		/// Fired when the current <see cref="Color"/> property changes
		/// </summary>
		public event TypedEventHandler<ColorPickerButton, ColorChangedEventArgs> ColorChanged;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            if (_button != null)
			{
				_button.Click -= OnButtonClick;
			}

            base.OnApplyTemplate(e);

			_button = e.NameScope.Find<Button>("MainButton");
			if (_button != null)
			{
				_button.Click += OnButtonClick;
			}
        }

		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			if(_flyout == null)
			{
				_flyout = new ColorPickerFlyout();
			}
			_flyout.Closed += OnFlyoutClosed;
			_flyout.Confirmed += OnFlyoutConfirmed;
			_flyout.Dismissed += OnFlyoutDismissed;
		}

		protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnDetachedFromVisualTree(e);
			_flyout.Closed -= OnFlyoutClosed;
			_flyout.Confirmed -= OnFlyoutConfirmed;
			_flyout.Dismissed -= OnFlyoutDismissed;
		}

		protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
		{
			base.OnPropertyChanged(change);
			if (change.Property == ColorProperty)
			{
				ColorChanged?.Invoke(this, new ColorChangedEventArgs(change.OldValue.GetValueOrDefault<Color2>(),
					change.NewValue.GetValueOrDefault<Color2>()));
			}
		}

		private void OnButtonClick(object sender, RoutedEventArgs e)
		{
			// ColorPicker is a large control, so the flyout is shared among all the ColorButton instances
			// So we need to make sure the ColorPicker is properly set for this button
			_flyout.ColorPicker.PreviousColor = Color;
						
			_flyout.ColorPicker.Color = Color;

			_flyout.ShowHideButtons(ShowAcceptDismissButtons);
			
			// If not showing the buttons, we'll update the color in real time
			if (!ShowAcceptDismissButtons)
			{
				_flyout.ColorPicker.ColorChanged += OnColorPickerColorChanged;
			}

			_flyout.ColorPicker.IsMoreButtonVisible = IsMoreButtonVisible;
			_flyout.ColorPicker.IsCompact = IsCompact;
			_flyout.ColorPicker.IsAlphaEnabled = IsAlphaEnabled;
			_flyout.ColorPicker.UseSpectrum = UseSpectrum;
			_flyout.ColorPicker.UseColorWheel = UseColorWheel;
			_flyout.ColorPicker.UseColorTriangle = UseColorTriangle;
			_flyout.ColorPicker.UseColorPalette = UseColorPalette;
			_flyout.ColorPicker.CustomPaletteColors = CustomPaletteColors;
			_flyout.ColorPicker.PaletteColumnCount = PaletteColumnCount;
			_flyout.ShowAt(this);

			// Keep track of which button the flyout is active on
			_flyoutActive = true;
		}

		private void OnColorPickerColorChanged(ColorPicker sender, ColorChangedEventArgs args)
		{
			Color = args.NewColor;
		}

		private void OnFlyoutDismissed(ColorPickerFlyout sender, object args)
		{
			if (_flyoutActive)
			{
				_flyoutActive = false;				
			}
		}

		private void OnFlyoutConfirmed(ColorPickerFlyout sender, object args)
		{
			if (_flyoutActive)
			{
				_flyoutActive = false;

				Color = _flyout.ColorPicker.Color;
			}
		}

		private void OnFlyoutClosed(object sender, EventArgs e)
		{
			if (!ShowAcceptDismissButtons)
			{
				_flyout.ColorPicker.ColorChanged -= OnColorPickerColorChanged;
			}
		}

		private static ColorPickerFlyout _flyout;

		private bool _flyoutActive;
		private Button _button;		
	}
}
