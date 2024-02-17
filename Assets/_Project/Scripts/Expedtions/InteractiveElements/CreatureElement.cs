using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureElement : InteractiveElement
{
    public Creature[] CreaturesTeam = new Creature[5];
    public SpriteRenderer[] SpriteRenderers = new SpriteRenderer[3];
    private (Vector3, Vector3, Vector3)[] RenderPositions = {
        (new Vector3(0, -0.01f, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)),
        (new Vector3(-0.25f, -0.01f, 0), new Vector3(0.25f, -0.01f, 0), new Vector3(0, 0, 0)),
        (new Vector3(0, -0.25f, 0), new Vector3(-0.25f, -0.01f, 0), new Vector3(0.25f, -0.01f, 0))
    };
    private void Start()
    {
        SetSprites();
    }
    public override void Interact(CharacterInfo character = null)
    {
        base.Interact(character);
        //Iniciar combate
    }
    private void SetSprites()
    {

        int CreaturesInWorld = 0;
        for (int i = 0; i < SpriteRenderers.Length; i++)
        {
            if (CreaturesTeam[i].CreatureInfo != null && SpriteRenderers[i] != null)
            {
                CreaturesInWorld++;

                SpriteRenderers[i].sprite = CreaturesTeam[i].CreatureInfo.c_Sprite;
                if (CreaturesTeam[i].CreatureInfo.c_Animator != null)
                {
                    SpriteRenderers[i].GetComponent<Animator>().runtimeAnimatorController = CreaturesTeam[i].CreatureInfo.c_Animator;
                    SpriteRenderers[i].GetComponent<Animator>().enabled = true;
                }
                else
                {
                    Debug.LogWarning("CreaturesTeam[i].CreatureInfo.c_Animator missing");
                }
                SpriteRenderers[i].enabled = true;
            }
            else
            {
                SpriteRenderers[i].GetComponent<Animator>().enabled = false;

                SpriteRenderers[i].enabled = false;
            }
        }

        SpriteRenderers[0].transform.localPosition = RenderPositions[CreaturesInWorld - 1].Item1;
        SpriteRenderers[1].transform.localPosition = RenderPositions[CreaturesInWorld - 1].Item2;
        SpriteRenderers[2].transform.localPosition = RenderPositions[CreaturesInWorld - 1].Item3;
    }
}
