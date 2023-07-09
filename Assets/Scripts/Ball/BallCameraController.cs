using UnityEngine;
using ElRaccoone.Timers;
using Cinemachine;

[RequireComponent(typeof(Cinemachine.CinemachineVirtualCamera))]
public class BallCameraController : MonoBehaviour
{
    public float DefaultStartZoom = 13f;

    public ParticleSystem TrailParticles;

    public Cinemachine.CinemachineVirtualCamera VirtualCamera { get; private set; }

    private int shakeTweenId = -1;
    public void Shake(float shakeAmplitude, float shakeFrequency, float shakeDuration)
    {
        CinemachineBasicMultiChannelPerlin noise = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = shakeAmplitude;
        noise.m_FrequencyGain = shakeFrequency;

        if (shakeTweenId != -1 && LeanTween.isTweening(shakeTweenId))
            LeanTween.cancel(shakeTweenId, true);

        shakeTweenId = LeanTween.value(gameObject, 0f, 1f, shakeDuration)
            .setOnUpdate((float t) => noise.m_AmplitudeGain = Mathf.Lerp(shakeAmplitude, 0f, t))
            .setOnComplete(() => ResetCameraShake())
            .id;
    }

    private void ResetCameraShake()
    {
        CinemachineBasicMultiChannelPerlin noise = VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    public void ShowTrailParticles()
    {
        TrailParticles.Play();
    }

    public void HideTrailParticles()
    {
        if (TrailParticles.isPlaying)
            TrailParticles.Stop();
    }

    private int zoomToTweenId = -1;
    public void ZoomTo(float zoom, float duration)
    {
        float currentZoom = VirtualCamera.m_Lens.OrthographicSize;

        if (zoomToTweenId != -1 && LeanTween.isTweening(zoomToTweenId))
            LeanTween.cancel(zoomToTweenId);

        zoomToTweenId = LeanTween.value(gameObject, (f) => VirtualCamera.m_Lens.OrthographicSize = f, VirtualCamera.m_Lens.OrthographicSize, zoom, duration)
            .setEase(LeanTweenType.linear)
            .id;
    }

    private void StopShake()
    {
        Cinemachine.CinemachineBasicMultiChannelPerlin noise = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }

    protected void Awake()
    {
        if (GameManager.Instance.BallCamera != null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.BallCamera = this;

        VirtualCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        VirtualCamera.m_Lens.OrthographicSize = 5f;
    }

    void Start()
    {

    }

    void Reset()
    {
        HideTrailParticles();
    }
}
