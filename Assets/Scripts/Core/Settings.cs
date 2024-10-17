public class Settings : SingletonMB<Settings>
{
    private AudioSettings _audioSettings;

    // Getters
    public AudioSettings Audio => _audioSettings;

    protected override void Awake() {
        base.Awake();
        _audioSettings = GetComponent<AudioSettings>();
    }
}