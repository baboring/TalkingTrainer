using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCoolDown : MonoBehaviour {

    public string abilityButtonAxisName = "Fire1";
    public Image darkMask;
    public Text coolDownTextDisplay;

    [SerializeField] private Ability ability;
    [SerializeField] private GameObject weaponHolder;
    private Image myButtonImage;
    private AudioSource abilitySource;
    private float coolDownDuration;
    private float nextReadyTime;
    private float coolDownTimeLeft;
	// Use this for initialization
	void Start () {
        Initialize (ability, weaponHolder); 
	}

    public void Initialize(Ability selectedAbility, GameObject weaponHolder)
    {
        ability = selectedAbility;
		if(null == ability)
			return;
        myButtonImage = GetComponent<Image> ();
        abilitySource = GetComponent<AudioSource> ();
		if(null == ability.aSprite)
			return;
        myButtonImage.sprite = ability.aSprite;
        darkMask.sprite = ability.aSprite;
        coolDownDuration = ability.aBaseCoolDown;
        ability.Initialize (weaponHolder);
        AbilityReady ();
    }

	// Update is called once per frame
	void Update () {
        bool coolDownComplete = (Time.time > nextReadyTime);
        if (coolDownComplete) 
        {
            AbilityReady ();
            if (Input.GetButtonDown (abilityButtonAxisName)) 
            {
                ButtonTriggered ();
            }
        } else 
        {
            CoolDown();
        }
		
	}

	 private void AbilityReady()
    {
        coolDownTextDisplay.enabled = false;
        darkMask.enabled = false;
    }

    private void CoolDown()
    {
        coolDownTimeLeft -= Time.deltaTime;
        float roundedCd = Mathf.Round (coolDownTimeLeft);
        coolDownTextDisplay.text = roundedCd.ToString ();
        darkMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
    }

    private void ButtonTriggered()
    {
        nextReadyTime = coolDownDuration + Time.time;
        coolDownTimeLeft = coolDownDuration;
        darkMask.enabled = true;
        coolDownTextDisplay.enabled = true;

		Debug.Assert(null == ability,"ability is nulll");
		if(null == ability) {
			Debug.Log("Ability not found!");
			return;
		}
		Debug.Assert(null == abilitySource,"audio source is nulll");
		if(null == abilitySource) {
			return;
		}
		abilitySource.clip = ability.aSound;
		abilitySource.Play ();
		ability.TriggerAbility ();
    }
}
