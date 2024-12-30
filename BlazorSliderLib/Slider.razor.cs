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
        public string? MinimumDescription { get; set; }

        [Parameter]
        public string? MaximumDescription { get; set; }

        [Parameter]
        public double Minimum { get; set; } = typeof(T).IsEnum ? Enum.GetValues(typeof(T)).Cast<int>().Select(e => Convert.ToDouble(e)).Min() : 0.0;

        [Parameter]
        public double Maximum { get; set; } = typeof(T).IsEnum ? Enum.GetValues(typeof(T)).Cast<int>().Select(e => Convert.ToDouble(e)).Max() : 100.0;

        [Parameter]
        public double? Stepsize { get; set; } = typeof(T).IsEnum ? 1 : 5.0;

        [Parameter]
        public bool ShowTickmarks { get; set; } = true;

        [Parameter]
        public bool UseAlternateStyle { get; set; } = false;

        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        public List<double> Tickmarks { get; set; } = new List<double>();

        private List<T> _enumValues { get; set; } = new List<T>();

        private bool _isInitialized = false;

        private async Task OnValueChanged(ChangeEventArgs e)
        {
            if (e.Value == null)
            {
                return;
            }
            if (typeof(T).IsEnum)
            {
                var enumValue = _enumValues.FirstOrDefault(v => Convert.ToDouble(v).Equals(Convert.ToDouble(e.Value))); if (!enumValue.Equals(default(T))) { Value = enumValue; }
            }
            else
            {
                Value = (T)Convert.ChangeType(e.Value, typeof(T));
            }
            await ValueChanged.InvokeAsync(Value);
        }


        private string TickmarksId = "ticksmarks_" + Guid.NewGuid().ToString("N");

        protected override void OnParametersSet()
        {
            if (!_isInitialized && !typeof(T).IsEnum && (Value.CompareTo(null) == 0 || Value.CompareTo(0) == 0))
            {
                Value = (T)Convert.ChangeType((Convert.ToDouble(Maximum) - Convert.ToDouble(Minimum)) / 2, typeof(T));
            }

            if (Maximum.CompareTo(Minimum) < 1)
            {
                throw new ArgumentException("The value for parameter 'Maximum' is set to a smaller value than {Minimum}");
            }
            GenerateTickMarks();

            BuildEnumValuesListIfRequired();

            _isInitialized = true;
        }

        private void BuildEnumValuesListIfRequired()
        {
            if (typeof(T).IsEnum)
            {
                foreach (var item in Enum.GetValues(typeof(T)))
                {
                    _enumValues.Add((T)item);
                }
            }
        }

        private void GenerateTickMarks()
        {
            Tickmarks.Clear();
            if (!ShowTickmarks)
            {
                return;
            }
            if (typeof(T).IsEnum)
            {
                int enumValuesCount = Enum.GetValues(typeof(T)).Length;
                double offsetEnum = 0;
                double minDoubleValue = Enum.GetValues(typeof(T)).Cast<int>().Select(e => Convert.ToDouble(e)).Min();
                double maxDoubleValue = Enum.GetValues(typeof(T)).Cast<int>().Select(e => Convert.ToDouble(e)).Max();
                double enumStepSizeCalculated = (maxDoubleValue - minDoubleValue) / enumValuesCount;

                foreach (var enumValue in Enum.GetValues(typeof(T)))
                {
                    Tickmarks.Add(offsetEnum);
                    offsetEnum += Math.Round(enumStepSizeCalculated, 0);
                }
                return;
            }

            for (double i = Convert.ToDouble(Minimum); i <= Convert.ToDouble(Maximum); i += Convert.ToDouble(Stepsize))
            {
                Tickmarks.Add(i);
            }

        }

    }

}
