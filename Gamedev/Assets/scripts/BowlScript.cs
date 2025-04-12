using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlScript : MonoBehaviour
{
    public string currentItem = "";
    public UIController uiController;

    public GameObject carrot2;
    public GameObject broccoli2;
    public GameObject cauliflower2;
    public GameObject lettuce2;
    public GameObject watermelon2;
    public GameObject pumpkin2;

    [SerializeField] private GameObject carrot;
    [SerializeField] private GameObject broccoli;
    [SerializeField] private GameObject cauliflower;
    [SerializeField] private GameObject lettuce;
    [SerializeField] private GameObject watermelon;
    [SerializeField] private GameObject pumpkin;

    public Carrot carrotScript;
    public Broccoli broccoliScript;
    public Cauliflower cauliflowerScript;
    public Lettuce lettuceScript;
    public Watermelon watermelonScript;
    public Pumpkin pumpkinScript;

    PlayerSoundEffects soundEffects;

    private void Awake()
    {
        if (soundEffects == null) {
            soundEffects = FindObjectOfType<PlayerSoundEffects>(); // Get SFXs
        }
        if (uiController == null)
        {
            uiController = FindObjectOfType<UIController>();
        }
        if (carrot == null)
        {
            carrot = GameObject.Find("player/character-male-b/root/torso/arm-left/carrot");
        }
        if (carrot != null)
        {
            carrotScript = carrot.GetComponent<Carrot>();
        }
        if (broccoli == null)
        {
            broccoli = GameObject.Find("player/character-male-b/root/torso/arm-left/broccoli");
        }
        if (broccoli != null)
        {
            broccoliScript = broccoli.GetComponent<Broccoli>();
        }
        if (cauliflower == null)
        {
            cauliflower = GameObject.Find("player/character-male-b/root/torso/arm-left/cauliflower");
        }
        if (cauliflower != null)
        {
            cauliflowerScript = cauliflower.GetComponent<Cauliflower>();
        }
        if (lettuce == null)
        {
            lettuce = GameObject.Find("player/character-male-b/root/torso/arm-left/mushroom");
        }
        if (lettuce != null)
        {
            lettuceScript = lettuce.GetComponent<Lettuce>();
        }
        if (pumpkin == null)
        {
            pumpkin = GameObject.Find("player/character-male-b/root/torso/arm-left/corn");
        }
        if (pumpkin != null)
        {
            pumpkinScript = pumpkin.GetComponent<Pumpkin>();
        }
        if (watermelon == null)
        {
            watermelon = GameObject.Find("player/character-male-b/root/torso/arm-left/sunflower");
        }
        if (watermelon != null)
        {
            watermelonScript = watermelon.GetComponent<Watermelon>();
        }
    }

    public void BowlInteract(string item)
    {
        if (item.ToLower() == "carrot")
        {
            carrot2.SetActive(true);
            currentItem = "carrot";
        }
        else if (item.ToLower() == "broccoli")
        {
            broccoli2.SetActive(true);
            currentItem = "broccoli";
        }
        else if (item.ToLower() == "cauliflower")
        {
            cauliflower2.SetActive(true);
            currentItem = "cauliflower";
        }
        else if (item.ToLower() == "lettuce")
        {
            lettuce2.SetActive(true);
            currentItem = "lettuce";
        }
        else if (item.ToLower() == "pumpkin")
        {
            pumpkin2.SetActive(true);
            currentItem = "pumpkin";
        }
        else if (item.ToLower() == "watermelon")
        {
            watermelon2.SetActive(true);
            currentItem = "watermelon";
        }

        StartCoroutine(BeforeEat(currentItem));

    }

    private IEnumerator BeforeEat(string item)
    {
        soundEffects.PlayBarkingSound();

        yield return new WaitForSeconds(2f);

        // Play the Dog Eat SFX after the bark
        if (soundEffects != null) {
            soundEffects.PlayDogEatSound();
        }

        yield return new WaitForSeconds(.5f);
        
        
        DogEat(item); 
    }

    public void DogEat(string item)
    {

        if (item.ToLower() == "carrot")
        {
            carrot2.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "broccoli")
        {
            broccoli2.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "cauliflower")
        {
            cauliflower2.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "lettuce")
        {
            lettuce2.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "pumpkin")
        {
            pumpkin2.SetActive(false);
            currentItem = "";
        }
        else if (item.ToLower() == "watermelon")
        {
            watermelon2.SetActive(false);
            currentItem = "";
        }
    }

    void Update()
    {
        if (uiController.IsNightPhase)
        {
            carrot2.SetActive(false);
            broccoli2.SetActive(false);
            cauliflower2.SetActive(false);
            lettuce2.SetActive(false);
            pumpkin2.SetActive(false);
            watermelon2.SetActive(false);
            currentItem = "";
        }
    }
}
