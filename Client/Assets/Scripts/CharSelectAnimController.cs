using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelectAnimController : MonoBehaviour
{

    public Animator charAnimator;
    public Text nameText;
    public int charValue;
    public Color baseColor;
    public Color selectedColor;
    private bool isHighlighted;

    void Start()
    {
        baseColor = new Color(.0980f, .0980f, .0980f, 1.0f);
        selectedColor = new Color(.2f,.519f,.1058f,1.0f);
        charAnimator = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TextColorToSelected()
    {
        charAnimator.enabled = true;
        isHighlighted = true;
        nameText.color = selectedColor;
    }
    public void TextColorToNormal()
    {
        charAnimator.enabled = false;
        isHighlighted = false;
        nameText.color = baseColor;
    }


}
