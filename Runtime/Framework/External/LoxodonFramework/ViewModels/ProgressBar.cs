using Loxodon.Framework.ViewModels;

namespace GhysX.Framework.External.ViewModels
{
    public class ProgressBar : ViewModelBase
    {
        private float _progress;
        private string _tip;
        private bool _enable;

        public bool Enable {
            get => this._enable;
            set => this.Set<bool> (ref this._enable, value);
        }

        public float Progress {
            get => this._progress;
            set => this.Set<float> (ref this._progress, value);
        }

        public string Tip {
            get => this._tip;
            set => this.Set<string> (ref this._tip, value);
        }
    }
}