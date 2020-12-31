namespace OpenControls.Wpf.TabHeaderControlDemo.ViewModel
{
    public class TabHeaderItem
    {
        public string Label { get; set; }
        public int ID { get; set; }

        public string HeaderText
        {
            get
            {
                return Label + " : " + ID;
            }
        }
    }
}
