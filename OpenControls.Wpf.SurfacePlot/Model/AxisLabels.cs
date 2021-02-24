using System.Collections.Generic;

namespace OpenControls.Wpf.SurfacePlot.Model
{
    public class AxisLabels
    {
        public delegate string FormatLabel(float x);
        public delegate float MeasureTextLength(string text);

        public void Refresh(
            bool flipLabels,
            FormatLabel formatLabel,
            MeasureTextLength measureTextLength)
        {
            Labels = new List<LabelInfo>();
            MaxLabelLength = 0f;
            float xValue = flipLabels ? MaxValue : MinValue;
            float xValueInc = (MaxValue - MinValue) / (float)(NumberOfLabels - 1);
            if (flipLabels)
            {
                xValueInc = -xValueInc;
            }
            for (int i = 0; i < NumberOfLabels; ++i, xValue += xValueInc)
            {
                string text = formatLabel(xValue);
                float length = measureTextLength(text);
                if (length > MaxLabelLength)
                {
                    MaxLabelLength = length;
                }
                Labels.Add(new LabelInfo() { Text = text, Length = length });
            }
        }

        public static AxisLabels Build(
            float minValue, 
            float maxValue, 
            int numberOfLabels,
            bool flipLabels,
            FormatLabel formatLabel,
            MeasureTextLength measureTextLength)
        {
            AxisLabels axisLabels = new AxisLabels();

            axisLabels.Labels = new List<LabelInfo>();
            axisLabels.MinValue = minValue;
            axisLabels.MaxValue = maxValue;
            axisLabels.NumberOfLabels = numberOfLabels;
            axisLabels.MaxLabelLength = 0f;
            axisLabels.Refresh(flipLabels, formatLabel, measureTextLength);
            return axisLabels;
        }

        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }
        public float MaxLabelLength { get; private set; }

        public List<LabelInfo> Labels { get; private set; }
        public float NumberOfLabels { get; private set; }
    }
}
