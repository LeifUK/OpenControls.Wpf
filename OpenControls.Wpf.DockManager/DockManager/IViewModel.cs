namespace OpenControls.Wpf.DockManager
{
    public interface IViewModel
    {
        // A user friendly title
        string Title { get; }

        string Tooltip { get; }

        /*
         * Not used by tools.
         * Uniquely identifies a document instance.
         * For example a file path for a text document. 
         */
        string URL { get; }

        bool CanClose { get; }

        /*
         * Return true if there are edits that need to be saved
         * Not used by Tool view model
         */
        bool HasChanged { get; }
        void Save();
    }
}
