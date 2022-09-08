using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NodeObject : MonoBehaviour
{
    [NonSerialized] public Node from = null;
    [NonSerialized] public Node target = null; 
    public bool combine = false;
    private int m_value;
    public int value
    {
        get => m_value;
        set
        {
            this.m_value = value;
            this.valueText.text = value.ToString();
            SetColor(value);
        }
    }

    public Image blockImage;
    public TextMeshProUGUI valueText;

    private Color Hex2FloatRGB(string r, string g, string b)
    {
        float R = Convert.ToInt32(r, 16);
        float G = Convert.ToInt32(g, 16);
        float B = Convert.ToInt32(b, 16);
        return new Color(R / 255, G / 255, B / 255);
    }

    private void SetColor(int value)
    {
        Color colorBlock= new Color(0.0f, 0.0f, 0.0f);
        Color colorNumber = new Color(1.0f, 1.0f, 1.0f);
        
        
        switch (value)
        {
            case 2:
                colorBlock = Hex2FloatRGB("ee", "e4", "da");
                colorNumber = Hex2FloatRGB("77", "6e", "65");

                break;
            case 4:
                colorBlock = Hex2FloatRGB("ed", "e0", "c8");
                colorNumber = Hex2FloatRGB("77", "6e", "65");

                break;
            case 8:
                colorBlock = Hex2FloatRGB("f2", "b1", "79");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
            case 16:
                colorBlock = Hex2FloatRGB("f5", "95", "63");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
            case 32: 
                colorBlock = Hex2FloatRGB("f6", "7c", "5f");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
            case 64: 
                colorBlock = Hex2FloatRGB("f6", "5e", "3b");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
            case 128:
                colorBlock = Hex2FloatRGB("ed", "cf", "72");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
            case 256:
                colorBlock = Hex2FloatRGB("ed", "cc", "61");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
            case 512:
                colorBlock = Hex2FloatRGB("ed", "c8", "50");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
            case 1024:
                colorBlock = Hex2FloatRGB("ed", "c5", "3f");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
            case 2048:
                colorBlock = Hex2FloatRGB("ed", "c2", "2e");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
            case 4096:
                colorBlock = Hex2FloatRGB("3c", "3a", "32");
                colorNumber = Hex2FloatRGB("f9", "f6", "f2");

                break;
        }

        blockImage.color = colorBlock;
        valueText.color = colorNumber;

    }
    public void InitializeFirstValue()
    {
        int[] v = new int[] {2, 4}; 
        this.value = v[Random.Range(0, v.Length)];
    }  
    public void MoveToNode(Node from, Node to)
    {
        combine = false;
        this.from = from;
        this.target = to;
    }

    public void CombineToNode(Node from, Node to)
    {
        combine = true;
        this.from = from;
        this.target = to; 
    }
    public void OnEndMove()
    {
        if (target != null)
        {
            if (combine)
            {
                target.realNodeObj.value = value * 2;
                var t = target.realNodeObj.transform.DOPunchScale(new Vector3(.25f, .25f, .25f), 0.15f, 3);
                this.gameObject.SetActive(false);
                t.onComplete += () =>
                {
                    this.needDestroy = true;
                    this.target = null; 
                    this.from = null; 
                }; 
            }
            else
            {  
                this.from = null;
                this.target = null;
            }
        } 
    } 
    public bool needDestroy= false;

    public void StartMoveAnimation()
    {
        if (target != null)
        {
            this.name = target.point.ToString(); 
            var tween = this.blockImage.rectTransform.DOLocalMove(target.position, 0.1f);
            tween.onComplete += () =>
            {
                OnEndMove();  
            };
        }
        
    }
    public void UpdateMoveAnimation()
    {
        if (target != null)
        {
            this.name = target.point.ToString();
            var p = Vector2.Lerp(this.transform.localPosition, target.position, 0.35f);
            this.transform.localPosition = p;
            if (Vector2.Distance(this.transform.localPosition, target.position) < 0.25f)
            {
                OnEndMove();  
            }
        }
    }
}
