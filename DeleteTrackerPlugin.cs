using NLog;
using System;
using System.IO;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Plugins;

namespace ALE_DeleteTracker {

    public class DeleteTrackerPlugin : TorchPluginBase, IWpfPlugin {

        public static DeleteTrackerPlugin Instance { get; private set; }
        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private UserControl _control;
        public UserControl GetControl() => _control ?? (_control = new Control(this));

        private Persistent<DeleteConfig> _config;
        public DeleteConfig Config => _config?.Data;

        public void Save() => _config.Save();

        /// <inheritdoc />
        public override void Init(ITorchBase torch) {

            base.Init(torch);
            var configFile = Path.Combine(StoragePath, "DeleteTracker.cfg");

            try {

                _config = Persistent<DeleteConfig>.Load(configFile);

            } catch (Exception e) {
                Log.Warn(e);
            }

            if (_config?.Data == null) {

                Log.Info("Create Default Config, because none was found!");

                _config = new Persistent<DeleteConfig>(configFile, new DeleteConfig());
                _config.Save();
            }

            Instance = this;

            torch.Managers.AddManager(new DeleteTrackerManager(torch));
        }
    }
}
