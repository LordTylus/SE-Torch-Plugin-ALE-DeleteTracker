using Torch;

namespace ALE_DeleteTracker {

    public class DeleteConfig : ViewModel {

        private bool _enableCompactLogging = true;
        private bool _enableFullLogging = true;
        private bool _removalOnShutdown = false;
        private bool _removalNobodyShips = true;
        private bool _removalNPCShips = true;
        private bool _logDuplicates = false;

        private string _loggingBasicFileName = "deleted-basic-${shortdate}.log";
        private string _loggingFullFileName = "deleted-${shortdate}.log";

        public string LoggingBasicFileName { get => _loggingBasicFileName; set => SetValue(ref _loggingBasicFileName, value); }

        public string LoggingFullFileName { get => _loggingFullFileName; set => SetValue(ref _loggingFullFileName, value); }

        public bool EnableCompactLogging { get => _enableCompactLogging; set => SetValue(ref _enableCompactLogging, value); }

        public bool EnableFullLogging { get => _enableFullLogging; set => SetValue(ref _enableFullLogging, value); }

        public bool RemovalOnShutdown { get => _removalOnShutdown; set => SetValue(ref _removalOnShutdown, value); }

        public bool RemovalNobodyGrids { get => _removalNobodyShips; set => SetValue(ref _removalNobodyShips, value); }

        public bool RemovalNPCShips { get => _removalNPCShips; set => SetValue(ref _removalNPCShips, value); }

        public bool LogDuplicates { get => _logDuplicates; set => SetValue(ref _logDuplicates, value); }
    }
}
