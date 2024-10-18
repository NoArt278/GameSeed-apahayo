public class Core : SingletonMB<Core> {
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}