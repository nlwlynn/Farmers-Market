using System.Collections;
using UnityEngine;

public class PlayerSoundEffects : MonoBehaviour {
    [Header("Sound Effects")]
    [SerializeField] private AudioClip wateringSound;
    [SerializeField] private AudioClip harvestingSound;
    [SerializeField] private AudioClip pesticideSound;
    [SerializeField] private AudioClip pickupItemSound;
    [SerializeField] private AudioClip fulfillOrderSound;
    [SerializeField] private AudioClip plantingSound;
    [SerializeField] private AudioClip handOverItemSound;
    [SerializeField] private AudioClip dogEatSound;
    [SerializeField] private AudioClip plotPlaceSound;
    [SerializeField] private AudioClip plotRemoveSound;
    [SerializeField] private AudioClip barkingSound;
    [SerializeField] private AudioClip purchaseItemSound;

    [Header("Volumes")]
    [SerializeField] private float wateringSoundVolume = 1f;
    [SerializeField] private float harvestingSoundVolume = 1f;
    [SerializeField] private float pesticideSoundVolume = 1f;
    [SerializeField] private float pickupItemSoundVolume = 1f;
    [SerializeField] private float fulfillOrderSoundVolume = 1f;
    [SerializeField] private float plantingSoundVolume = 1f;
    [SerializeField] private float handOverItemSoundVolume = 1f;
    [SerializeField] private float dogEatSoundVolume = 1f;
    [SerializeField] private float plotPlaceSoundVolume = 1f;
    [SerializeField] private float plotRemoveSoundVolume = 1f;
    [SerializeField] private float barkingSoundVolume = 1f;
    [SerializeField] private float purchaseItemSoundVolume = 1f;

    [Header("Trim Durations (seconds, 0 = full clip)")]
    [SerializeField] private float wateringTrim = 0f;
    [SerializeField] private float harvestingTrim = 0f;
    [SerializeField] private float pesticideTrim = 0f;
    [SerializeField] private float pickupItemTrim = 0f;
    [SerializeField] private float fulfillOrderTrim = 0f;
    [SerializeField] private float plantingTrim = 0f;
    [SerializeField] private float handOverItemTrim = 0f;
    [SerializeField] private float dogEatTrim = 0f;
    [SerializeField] private float plotPlaceTrim = 0f;
    [SerializeField] private float plotRemoveTrim = 0f;
    [SerializeField] private float barkingSoundTrim = 0f;
    [SerializeField] private float purchaseItemTrim = 0f;

    [Header("Playback Speeds (1 = normal, 2 = double speed, 0.5 = half speed)")]
    [SerializeField] private float wateringPitch = 2f;
    [SerializeField] private float harvestingPitch = 1f;
    [SerializeField] private float pesticidePitch = 1f;
    [SerializeField] private float pickupItemPitch = 1f;
    [SerializeField] private float fulfillOrderPitch = 1f;
    [SerializeField] private float plantingPitch = 1.5f;
    [SerializeField] private float handOverItemPitch = 1f;
    [SerializeField] private float dogEatPitch = 1f;
    [SerializeField] private float plotPlacePitch = 1f;
    [SerializeField] private float plotRemovePitch = 1f;
    [SerializeField] private float barkingSoundPitch = 1f;
    [SerializeField] private float purchaseItemPitch = 1f;

    [SerializeField] private AudioSource audioSource;

    private void Awake() {
        if (audioSource == null) {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayWateringSound() {
        PlayClippedSound(wateringSound, wateringSoundVolume, wateringTrim, wateringPitch);
    }

    public void PlayHarvestingSound() {
        PlayClippedSound(harvestingSound, harvestingSoundVolume, harvestingTrim, harvestingPitch);
    }

    public void PlayPesticideSound() {
        PlayClippedSound(pesticideSound, pesticideSoundVolume, pesticideTrim, pesticidePitch);
    }

    public void PlayPickupItemSound() {
        PlayClippedSound(pickupItemSound, pickupItemSoundVolume, pickupItemTrim, pickupItemPitch);
    }

    public void PlayFulfillOrderSound() {
        PlayClippedSound(fulfillOrderSound, fulfillOrderSoundVolume, fulfillOrderTrim, fulfillOrderPitch);
    }

    public void PlayPlantingSound() {
        PlayClippedSound(plantingSound, plantingSoundVolume, plantingTrim, plantingPitch);
    }

    public void PlayHandOverItemSound() {
        PlayClippedSound(handOverItemSound, handOverItemSoundVolume, handOverItemTrim, handOverItemPitch);
    }

    public void PlayDogEatSound() {
        PlayClippedSound(dogEatSound, dogEatSoundVolume, dogEatTrim, dogEatPitch);
    }

    public void PlayPlotPlaceSound() {
        PlayClippedSound(plotPlaceSound, plotPlaceSoundVolume, plotPlaceTrim, plotPlacePitch);
    }

    public void PlayPlotRemoveSound() {
        PlayClippedSound(plotRemoveSound, plotRemoveSoundVolume, plotRemoveTrim, plotRemovePitch);
    }

    public void PlayBarkingSound() {
        PlayClippedSound(barkingSound, barkingSoundVolume, barkingSoundTrim, barkingSoundPitch);
    }

    public void PlayPurchaseItemSound() {
        PlayClippedSound(purchaseItemSound, purchaseItemSoundVolume, purchaseItemTrim, purchaseItemPitch);
    }

    private void PlayClippedSound(AudioClip clip, float volume, float trimLength, float pitch) {
        if (clip == null || audioSource == null) return;

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();

        if (trimLength > 0f && trimLength < clip.length) {
            float adjustedDuration = trimLength / pitch;
            StartCoroutine(StopAfterSeconds(adjustedDuration));
        }
    }

    private IEnumerator StopAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        audioSource.Stop();
    }
}
