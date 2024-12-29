using Microsoft.AspNetCore.Components;

namespace BlazorSliderLib
{

    /// <summary>
    /// Slider to be used in Blazor. Uses input type='range' with HTML5 element datalist and custom css to show a slider.
    /// To add tick marks, set the <see cref="ShowTickmarks"/> to true (this is default)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class Slider<T> : ComponentBase
        where T : struct, IComparable
    {

        /// <summary>
        /// Initial value to set to the slider, data bound so it can also be read out
        /// </summary>
        [Parameter]
        public T Value { get; set; }

        [Parameter, EditorRequired]
        public required string Title { get; set; }

        [Parameter]
        public required T Minimum { get; set; } = (T)Convert.ChangeType(0, typeof(T));

        [Parameter]
        public required T Maximum { get; set; } = (T)Convert.ChangeType(100, typeof(T));

        [Parameter]
        public required T Stepsize { get; set; } = (T)Convert.ChangeType(5, typeof(T));

        [Parameter]
        public bool ShowTickmarks { get; set; } = true;

        [Parameter] public EventCallback<T> ValueChanged { get; set; }
        private async Task OnValueChanged(ChangeEventArgs e)
        {
            if (e.Value == null)
            {
                return;
            }
            Value = (T)Convert.ChangeType(e.Value, typeof(T));
            await ValueChanged.InvokeAsync(Value);
        }

        public List<double> Tickmarks { get; set; } = new List<double>();

        private string TickmarksId = "ticksmarks_" + Guid.NewGuid().ToString("N");

        protected override void OnParametersSet()
        {
            if (Value.CompareTo(null) == 0)
            {
                Value = (T)Convert.ChangeType((Convert.ToDouble(Maximum) - Convert.ToDouble(Minimum)) / 2, typeof(T));
            }

            if (Maximum.CompareTo(Minimum) < 1)
            {
                throw new ArgumentException("The value for parameter 'Maximum' is set to a smaller value than {Minimum}");
            }
            GenerateTickMarks();
        }

        private void GenerateTickMarks()
        {
            Tickmarks.Clear();
            if (!ShowTickmarks)
            {
                return;
            }
            for (double i = Convert.ToDouble(Minimum); i < Convert.ToDouble(Maximum); i += Convert.ToDouble(Stepsize))
            {
                Tickmarks.Add(i);
            }
        }

    }

}
