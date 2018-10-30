using System.Reactive.Disposables;

namespace FollowManager.Dispose
{
    /// <summary>
    /// リソースを管理するクラス
    /// </summary>
    public sealed class DisposeManager
    {
        // パブリックプロパティ

        public static DisposeManager Instance { get; } = new DisposeManager();

        public CompositeDisposable Disposables { get; } = new CompositeDisposable();

        // コンストラクタ

        private DisposeManager()
        {

        }
    }
}
