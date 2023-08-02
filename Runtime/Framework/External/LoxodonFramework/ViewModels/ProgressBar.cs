using Loxodon.Framework.ViewModels;

namespace GhysX.Framework.External.ViewModels
{
    public class ProgressBar : ViewModelBase
    {
        private float progress;
        private string tip;
        private bool enable;

        public bool Enable {
            get{ return this.enable; }
            set{ this.Set<bool> (ref this.enable, value); }
        }

        public float Progress {
            get{ return this.progress; }
            set{ this.Set<float> (ref this.progress, value); }
        }

        public string Tip {
            get{ return this.tip; }
            set{ this.Set<string> (ref this.tip, value); }
        }
    }
}