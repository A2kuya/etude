using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{

	Player player;

	void Start()
	{
		player = GetComponent<Player>();
	}

	void Update()
	{
		Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		player.SetDirectionalInput(directionalInput);
		if(Input.GetKeyDown(KeyCode.C))
        {
			player.Dash();
        }
		if (Input.GetKeyDown(KeyCode.X))
		{
			if (Input.GetKey(KeyCode.DownArrow))
				player.downJump = true;
			else
			{
				player.OnJumpInputDown();
			}
		}
		if (Input.GetKeyUp(KeyCode.X))
		{
			player.OnJumpInputUp();
		}
	}
}