using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Serilog;

namespace BinGuiDev.Components.ContDateTime;

public class DurationValidationBehavior{
    private static readonly DependencyProperty LastValidValueProperty =
        DependencyProperty.RegisterAttached(
            "LastValidValue",
            typeof(string),
            typeof(DurationValidationBehavior),
            new PropertyMetadata("0000:00:00:00"));

    public static readonly DependencyProperty EnableDurationValidationProperty =
        DependencyProperty.RegisterAttached(
            "EnableDurationValidation",
            typeof(bool),
            typeof(DurationValidationBehavior),
            new PropertyMetadata(false, OnEnableDurationValidationChanged));

    public static bool GetEnableDurationValidation(DependencyObject obj) =>
        (bool)obj.GetValue(EnableDurationValidationProperty);

    public static void SetEnableDurationValidation(DependencyObject obj, bool value) =>
        obj.SetValue(EnableDurationValidationProperty, value);

    private static string GetLastValidValue(DependencyObject obj) =>
        (string)obj.GetValue(LastValidValueProperty);

    private static void SetLastValidValue(DependencyObject obj, string value) =>
        obj.SetValue(LastValidValueProperty, value);

    private static void OnEnableDurationValidationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
        if (d is TextBox textBox && (bool)e.NewValue){
            textBox.PreviewTextInput += OnPreviewTextInput;
            textBox.PreviewKeyDown += OnPreviewKeyDown;
            textBox.GotFocus += OnGotFocus;
            textBox.LostFocus += OnLostFocus;
            textBox.KeyDown += OnKeyDown;

            textBox.MaxLength = 8 + 5;
            var initialValue = "0000:00:00:00";
            textBox.Text = initialValue;
            SetLastValidValue(textBox, initialValue);
        }
    }

    private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e){
        if (sender is not TextBox textBox || !char.IsDigit(e.Text[0])){
            e.Handled = true;
            return;
        }

        var caretIndex = textBox.CaretIndex;

        if (caretIndex == 4 || caretIndex == 7 || caretIndex == 10){
            caretIndex += 1;
            textBox.CaretIndex = caretIndex;
        }

        if (caretIndex < 13){
            var newText = new StringBuilder(textBox.Text);
            newText[caretIndex] = e.Text[0];

            if (IsValidTimeValue(newText.ToString())){
                textBox.Text = newText.ToString();
                SetLastValidValue(textBox, newText.ToString());
            }

            textBox.CaretIndex = Math.Min(caretIndex + 1, 12);
            if (textBox.CaretIndex == 4 || textBox.CaretIndex == 7 || textBox.CaretIndex == 10)
                textBox.CaretIndex += 1;
        }

        e.Handled = true;
    }

    private static bool IsValidTimeValue(string timeText){
        if (timeText.Length != 13) return false;
        var hours = int.Parse(timeText.Substring(5, 2));
        var minutes = int.Parse(timeText.Substring(8, 2));
        var seconds = int.Parse(timeText.Substring(11, 2));
        return hours <= 23 && minutes <= 59 && seconds <= 59;
    }

    private static void OnKeyDown(object sender, KeyEventArgs e){
        if (e.Key == Key.Enter && sender is TextBox textBox){
            ValidateAndRestore(textBox);
        }
    }

    private static void OnPreviewKeyDown(object sender, KeyEventArgs e){
        if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Space){
            e.Handled = true;
        }
    }

    private static void OnGotFocus(object sender, RoutedEventArgs e){
        if (sender is TextBox textBox){
            textBox.CaretIndex = 0;
        }
    }

    private static void OnLostFocus(object sender, RoutedEventArgs e){
        if (sender is TextBox textBox){
            ValidateAndRestore(textBox);
        }
    }

    private static void ValidateAndRestore(TextBox textBox){
        if (!IsValidTimeValue(textBox.Text)){
            textBox.Text = GetLastValidValue(textBox);
        }
    }
}