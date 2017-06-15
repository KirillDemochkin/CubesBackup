using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BrushMode { Filled, Neutral, Empty };
public class BrushController : MonoBehaviour {

	
    public event System.Action<Vector3, BrushMode> onVoxelCollided;
    public BrushMode mode;
    public Transform shootPoint, jumpPoint, wallPoint;
    public Collider drawSphere;
    private Transform currentPoint;
    public Transform player; 
    public Dictionary<AbilityType, Ability> abilities;
    public Text text;
    public Ability jumpPrefab, boomPrefab, shootPrefab, wavePrefab, wallPrefab;
    
    private Ability curAbilityLeftMouseKey, curAbilityRightMouseKey;
    private bool toggleDraw = true;

	void Start () {
        mode = BrushMode.Neutral;
        abilities = new Dictionary<AbilityType, Ability>();
        text.text = "Ability: Draw/Erase";
        abilities.Add(AbilityType.Jump, jumpPrefab);
        abilities.Add(AbilityType.Boom, boomPrefab);
        abilities.Add(AbilityType.Shoot, shootPrefab);
        abilities.Add(AbilityType.Wall, wallPrefab);
        abilities.Add(AbilityType.Wave, wavePrefab);
	}
	
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            toggleDraw = true;
            currentPoint = shootPoint;
            text.text = "Ability: Draw/Erase";
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            toggleDraw = false;
            currentPoint = shootPoint;
            curAbilityLeftMouseKey = abilities[AbilityType.Shoot];
            curAbilityRightMouseKey = abilities[AbilityType.Boom];
            text.text = "Ability: Erase/Boom";
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (rbController.Grounded)
            {
                var lP = currentPoint;
                currentPoint = jumpPoint;
                var a = Instantiate(abilities[AbilityType.Wave]) as Ability;
                a.onVoxelCollided += callOnVoxelCollided;
                a.transform.position = currentPoint.position;
                a.setMode(BrushMode.Filled);
                currentPoint = lP;
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {

            //add jump;
            var lP = currentPoint;
            currentPoint = jumpPoint;
            
            
            Ability a = Instantiate(abilities[AbilityType.Jump]) as JumpAbility;
            a.onVoxelCollided += callOnVoxelCollided;
            a.transform.position = currentPoint.position;
            

          
            currentPoint = lP;
            
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            var lP = currentPoint;
            currentPoint = wallPoint;
            var a = Instantiate(abilities[AbilityType.Wall]) as Ability;
            a.onVoxelCollided += callOnVoxelCollided;
            a.transform.position = currentPoint.position;
            a.transform.rotation = currentPoint.rotation;
            currentPoint = lP;
        }
        if (toggleDraw)
        {
            if (Input.GetMouseButtonDown (0) && !Input.GetMouseButton(1))
            {
               drawSphere.enabled = true;
                mode = BrushMode.Filled;
            } else if (Input.GetMouseButton(1) && !Input.GetMouseButton(0))
            {
                drawSphere.enabled = true;
                mode = BrushMode.Empty;
            }
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                drawSphere.enabled = false;
                mode = BrushMode.Neutral;
            }  
        } else
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                var a = Instantiate(curAbilityLeftMouseKey) as Ability;
                a.onVoxelCollided += callOnVoxelCollided;
                a.transform.position = currentPoint.position;
                a.transform.rotation = currentPoint.rotation;
             

            }
            else if (Input.GetMouseButtonDown(1))
            {
                var a = Instantiate(curAbilityRightMouseKey) as Ability;
                a.onVoxelCollided += callOnVoxelCollided;
                a.transform.position = currentPoint.position;
                a.transform.rotation = currentPoint.rotation;


            }
        }
    }

    public void callOnVoxelCollided(Vector3 position, BrushMode mode)
    {
        if (onVoxelCollided != null)
        {
            onVoxelCollided(position, mode);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collided with voxel");
        if(other.tag == "Voxel" && mode != BrushMode.Neutral)
        {
                if (onVoxelCollided != null)
                {
                    onVoxelCollided(other.transform.position, mode);
                }
            
                     
        }

    }

}
