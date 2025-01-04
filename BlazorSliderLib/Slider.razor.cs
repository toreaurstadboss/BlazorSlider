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

        public double ValueAsDouble { get; set; }

        public double GetValueAsDouble()
        {
            if (typeof(T).IsEnum)
            {
                if (_isInitialized)
                {
                    var e = _enumValues.FirstOrDefault(v => Convert.ToDouble(v).Equals(Convert.ToDouble(Value)));
                    return Convert.ToDouble(Convert.ChangeType(Value, typeof(int)));
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return Convert.ToDouble(Value);
            }
        }        

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
        public AlternateStyle UseAlternateStyle { get; set; } = AlternateStyle.None;

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
            if (typeof(T).IsEnum && e.Value != null)
            {
                var enumValue = _enumValues.FirstOrDefault(v => Convert.ToDouble(v).Equals(Convert.ToDouble(e.Value))); 
                if (Enum.TryParse(typeof(T), enumValue.ToString(), out _)) {
                    Value = enumValue; //check that it was a non-null value set from the slider
                }
                else
                {
                    return; //if we cannot handle the enum value set, do not process further
                }
            }
            else
            {
                Value = (T)Convert.ChangeType(e.Value!, typeof(T));
            }

            ValueAsDouble = GetValueAsDouble();

            await ValueChanged.InvokeAsync(Value);
        }


        private string TickmarksId = "ticksmarks_" + Guid.NewGuid().ToString("N");

        protected override async Task OnParametersSetAsync()
        {
            if (_isInitialized)
            {
                return ; //initialize ONCE 
            }

            if (!typeof(T).IsEnum && Value.CompareTo(0) == 0)
            {
                Value = (T)Convert.ChangeType((Convert.ToDouble(Maximum) - Convert.ToDouble(Minimum)) / 2, typeof(T));
                ValueAsDouble = GetValueAsDouble();
            }

            if (Maximum.CompareTo(Minimum) < 1)
            {
                throw new ArgumentException("The value for parameter 'Maximum' is set to a smaller value than {Minimum}");
            }
            GenerateTickMarks();

            BuildEnumValuesListIfRequired();

            _isInitialized = true;

            await Task.CompletedTask;
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

    public enum AlternateStyle
    {
        /// <summary>
        /// No alternate style. Uses the ordinary styling for the slider (browser default of input type 'range')
        /// </summary>
        None,

        /// <summary>
        /// Applies alternate style, using in addition to the 'slider track' an additional visual hint with an additional 'slider track' right below that shows a reddish color for lowest parts of the scale to the slider and towards yellow and greenish hues for higher values
        /// The alternate style uses a larger 'slider thumb' and alternate style to the 'slider-track'. The alternate style gives a more interesting look, especially in Microsoft Edge Chromium.
        /// </summary>
        AlternateStyle,

        /// <summary>
        /// Similar in style to the alternate style, but uses the inverse scale for the colors along the slider
        /// </summary>
        AlternateStyleInverseColorScale
    }

}
