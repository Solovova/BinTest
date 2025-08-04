using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BinGuiDev.Components.ContDateTime;

public class TimeValidationBehavior{
    private static readonly DependencyProperty LastValidValueProperty =
        DependencyProperty.RegisterAttached(
            "LastValidValue",
            typeof(string),
            typeof(TimeValidationBehavior),
            new PropertyMetadata("00:00:00"));

    public static readonly DependencyProperty EnableTimeValidationProperty =
        DependencyProperty.RegisterAttached(
            "EnableTimeValidation",
            typeof(bool),
            typeof(TimeValidationBehavior),
            new PropertyMetadata(false, OnEnableTimeValidationChanged));

    public static bool GetEnableTimeValidation(DependencyObject obj) =>
        (bool)obj.GetValue(EnableTimeValidationProperty);

    public static void SetEnableTimeValidation(DependencyObject obj, bool value) =>
        obj.SetValue(EnableTimeValidationProperty, value);

    private static string GetLastValidValue(DependencyObject obj) =>
        (string)obj.GetValue(LastValidValueProperty);

    private static void SetLastValidValue(DependencyObject obj, string value) =>
        obj.SetValue(LastValidValueProperty, value);

    private static void OnEnableTimeValidationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
        if (d is not TextBox textBox || !(bool)e.NewValue) return;
        textBox.PreviewTextInput += OnPreviewTextInput;
        textBox.PreviewKeyDown += OnPreviewKeyDown;
        textBox.GotFocus += OnGotFocus;
        textBox.LostFocus += OnLostFocus;
        textBox.KeyDown += OnKeyDown;

        textBox.MaxLength = 8;
        const string initialValue = "00:00:00";
        textBox.Text = initialValue;
        SetLastValidValue(textBox, initialValue);
    }

    private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e){
        if (sender is not TextBox textBox || !char.IsDigit(e.Text[0])){
            e.Handled = true;
            return;
        }

        int caretIndex = textBox.CaretIndex;

        if (caretIndex is 2 or 5){
            caretIndex += 1;
            textBox.CaretIndex = caretIndex;
        }

        if (caretIndex < 8){
            StringBuilder newText = new(textBox.Text){
                [caretIndex] = e.Text[0]
            };

            if (IsValidTimeValue(newText.ToString())){
                textBox.Text = newText.ToString();
                SetLastValidValue(textBox, newText.ToString());
            }

            textBox.CaretIndex = Math.Min(caretIndex + 1, 7);
            if (textBox.CaretIndex is 2 or 5)
                textBox.CaretIndex += 1;
        }

        e.Handled = true;
    }

    private static bool IsValidTimeValue(string timeText){
        if (timeText.Length != 8) return false;

        int hours = int.Parse(timeText.Substring(0, 2));
        int minutes = int.Parse(timeText.Substring(3, 2));
        int seconds = int.Parse(timeText.Substring(6, 2));

        return hours <= 23 && minutes <= 59 && seconds <= 59;
    }

    private static void OnKeyDown(object sender, KeyEventArgs e){
        if (e.Key == Key.Enter && sender is TextBox textBox){
            ValidateAndRestore(textBox);
        }
    }

    private static void OnPreviewKeyDown(object sender, KeyEventArgs e){
        if (e.Key is Key.Back or Key.Delete or Key.Space){
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